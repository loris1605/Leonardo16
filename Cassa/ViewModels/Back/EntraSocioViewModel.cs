using Common.InterViewModels;
using DTO.Repository;
using ReactiveUI;
using Splat;
using SysNet.Converters;
using System.Reactive;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using ViewModels.BindableObjects;

namespace ViewModels
{
    public partial class EntraSocioViewModel : BaseViewModel, IEntraSocioViewModel
    {
        private int _postazioneId;
        private ICassaScreen _host;
        private string _posizione;

        private IStrisciataRepository _strisciataRepository;
        private IEntraSocioRepository Q;

        public ReactiveCommand<Unit, Unit> TesseraCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> F5Command { get; private set; }
        public ReactiveCommand<Unit, Unit> PosizioneEscCommand { get; private set; }

        protected override IObservable<bool> IsAnythingExecuting =>
            new[]
            {
                base.IsAnythingExecuting,
                TesseraCommand?.IsExecuting ?? Observable.Return(false),
                PosizioneEscCommand?.IsExecuting ?? Observable.Return(false),
                F5Command?.IsExecuting ?? Observable.Return(false)

            }.CombineLatest(values => values.Any(x => x));


        public EntraSocioViewModel(IStrisciataRepository strisciataRepository, IEntraSocioRepository Repository) : base()
        {
            _strisciataRepository = strisciataRepository ?? throw new ArgumentNullException(nameof(strisciataRepository));
            Q = Repository ?? throw new ArgumentNullException(nameof(Repository));

            // 1. Aggiungi .Skip(1) così ignora lo stato iniziale di default (false)
            var socioFoundStream = this.WhenAnyValue(x => x.IsSocioFound)
                                       .ObserveOn(RxSchedulers.MainThreadScheduler);

            // 2. Imposta l'initialValue desiderato all'apertura della pagina
            _tesseraLabel = socioFoundStream
                                .Select(found => found ? "TESSERA :" : "TESSERA (F5) :")
                                .ToProperty(this, x => x.TesseraLabel, initialValue: "TESSERA :");

            // 3. Allinea l'initialValue vuoto per l'avvio
            _infoLabel = socioFoundStream
                                .Select(found => found ? "" : "Socio non Trovato")
                                .Skip(1)
                                .ToProperty(this, x => x.InfoLabel, initialValue: "");


            TesseraCommand = ReactiveCommand.CreateFromTask(async vm => await OnTesseraEnter());
            F5Command = ReactiveCommand.CreateFromTask(async vm => await OnF5Pressed());
            PosizioneEscCommand = ReactiveCommand.CreateFromTask(async vm => await OnPosizioneEsc());

            this.WhenActivated(d =>
            {
                TesseraCommand?.DisposeWith(d);
                F5Command?.DisposeWith(d);
                PosizioneEscCommand?.DisposeWith(d);
                _tesseraLabel?.DisposeWith(d);
                _infoLabel?.DisposeWith(d);
            });
        }

        protected override void OnFinalDestruction()
        {
            // Assicuriamoci che la collezione sia nulla per il GC
            TesseraCommand = null;
            PosizioneEscCommand = null;
            F5Command = null;
            //AddTesseraCommand = DelTesseraCommand = UpdTesseraCommand = PersonSearchCommand = null;

            _strisciataRepository = null;
            Q = null;
            base.OnFinalDestruction();
        }

        protected override async Task OnLoading()
        {
            await _strisciataRepository.DevelopStrisciate(token);
            var data  = await Q.GetIngressiByPostazione(_postazioneId, token);
            IngressiList = data.Select(data => new EntraIngressiMap(data)).ToList();
            if (IngressiList.Count > 0)
            {
                SelectedIngresso = IngressiList[0];
            }
            await SetFocus(TesseraFocus);
        }

        public void SetHost(ICassaScreen host) => _host = host;

        public void SetPostazioneId(int posizioneId)
        {
            _postazioneId = posizioneId;
        }

        public void SetPosizione(string posizione)
        {
            _posizione = posizione;
        }

        protected async override Task OnEsc()
        {
            _isClosing = true; // Imposta il flag per indicare che stiamo chiudendo la pagina
            var cassaPostazioneVm = Locator.Current.GetService<ICassaPostazioneViewModel>();
            if (cassaPostazioneVm is not null)
            {
                cassaPostazioneVm.SetHost(_host);
                cassaPostazioneVm.SetPostazioneId(_postazioneId);
                cassaPostazioneVm.SetPosizione(_posizione);

                await _host.CassaRouter.NavigateAndReset.Execute(cassaPostazioneVm);
            }
            else _isClosing = false; // Se non riesce a navigare, resetta il flag per evitare di bloccare la pagina


        }

        private async Task OnTesseraEnter()
        {

            var data = new EntraSocioMap(await Q.GetPersonByTessera(BindingT.NumeroTessera, token));

            if (data.CodiceSocio == 0)
            {
                IsSocioFound = false;
                string tessera = BindingT.NumeroTessera;
                BindingT = new(); // Resetta i dati
                Eta = string.Empty;
                BindingT.NumeroTessera = tessera; // Mantieni la tessera inserita
                await SetFocus(TesseraFocus);
            }
            else
            {
                IsSocioFound = true;
                BindingT = data;
                Eta = BindingT.Natoil.DateIntToEta().ToString();
                
            }
        }

        private async Task OnF5Pressed()
        {

            if (BindingT.NumeroTessera== string.Empty) return;
            IsSocioFound = true;
            BuildVirtualSocio();

            await Task.CompletedTask;

        }

        private async Task OnPosizioneEsc()
        {
            BindingT = new(); // Resetta i dati
            Eta = string.Empty;
            await SetFocus(TesseraFocus);
        }

        private void BuildVirtualSocio()
        {
            BindingT.Cognome = "Socio";
            BindingT.Nome = "Virtuale";
            BindingT.NumeroSocio = "-" + BindingT.NumeroTessera;
            Eta = string.Empty;
            BindingT.CodiceSocio = -1; // Indica che è un socio virtuale

            // Qui puoi fare ulteriori operazioni con virtualSocio, come salvarlo o passarlo ad altri componenti
        }
    }

    public partial class EntraSocioViewModel
    {
        public Interaction<Unit, Unit> TesseraFocus { get; } = new();
        public Interaction<Unit, Unit> PosizioneFocus { get; } = new();


        private EntraSocioMap _bindingt = new();
        public EntraSocioMap BindingT
        {
            get => this._bindingt;
            set => this.RaiseAndSetIfChanged(ref _bindingt, value);
        }

        private string _eta;
        public string Eta
        {
            get => _eta;
            set => this.RaiseAndSetIfChanged(ref _eta, value);
        }

        private bool _isSocioFound = true;
        public bool IsSocioFound
        {
            get => _isSocioFound;
            set => this.RaiseAndSetIfChanged(ref _isSocioFound, value);
        }

        // 2. Proprietà calcolata (OAPH) per la Label
        private readonly ObservableAsPropertyHelper<string> _tesseraLabel;
        public string TesseraLabel => _tesseraLabel.Value;

        private readonly ObservableAsPropertyHelper<string> _infoLabel;
        public string InfoLabel => _infoLabel.Value;

        private List<EntraIngressiMap> _ingressiList = new();
        public List<EntraIngressiMap> IngressiList
        {
            get => _ingressiList;
            set => this.RaiseAndSetIfChanged(ref _ingressiList, value);
        }

        private EntraIngressiMap _selectedIngressi;
        public EntraIngressiMap SelectedIngresso
        {
            get => _selectedIngressi;
            set => this.RaiseAndSetIfChanged(ref _selectedIngressi, value);

        }
    }
}
