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

        protected override IObservable<bool> IsAnythingExecuting =>
            new[]
            {
                base.IsAnythingExecuting,
                TesseraCommand?.IsExecuting ?? Observable.Return(false)
                
            }.CombineLatest(values => values.Any(x => x));

        public EntraSocioViewModel(IStrisciataRepository strisciataRepository, IEntraSocioRepository Repository) : base()
        {
            _strisciataRepository = strisciataRepository ?? throw new ArgumentNullException(nameof(strisciataRepository));
            Q = Repository ?? throw new ArgumentNullException(nameof(Repository));

            TesseraCommand = ReactiveCommand.CreateFromTask(async vm => await OnTesseraEnter());
            

            this.WhenActivated(d =>
            {
                TesseraCommand?.DisposeWith(d);
            });
        }

        protected override void OnFinalDestruction()
        {
            // Assicuriamoci che la collezione sia nulla per il GC
            TesseraCommand = null;
            //AddTesseraCommand = DelTesseraCommand = UpdTesseraCommand = PersonSearchCommand = null;

            _strisciataRepository = null;
            Q = null;
            base.OnFinalDestruction();
        }

        protected override async Task OnLoading()
        {
            await _strisciataRepository.DevelopStrisciate(token);
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
            var cassaPostazioneVm = Locator.Current.GetService<ICassaPostazioneViewModel>();
            if (cassaPostazioneVm is not null)
            {
                cassaPostazioneVm.SetHost(_host);
                cassaPostazioneVm.SetPostazioneId(_postazioneId);
                cassaPostazioneVm.SetPosizione(_posizione);

                await _host.CassaRouter.NavigateAndReset.Execute(cassaPostazioneVm);
            }
        }

        private async Task OnTesseraEnter()
        {
            var data = new EntraSocioMap(await Q.GetPersonByTessera(BindingT.NumeroTessera, token));

            if (data.CodiceSocio == 0)
            {
                InfoLabel = "Tessera non trovata";
                await SetFocus(TesseraFocus);
            }
            else
            {
                BindingT = data;
                Eta = BindingT.Natoil.DateIntToEta();
            }
        }
    }

    public partial class EntraSocioViewModel
    {
        public Interaction<Unit, Unit> TesseraFocus { get; } = new();

        private string infolabel = string.Empty;
        public string InfoLabel
        {
            get => infolabel;
            set => this.RaiseAndSetIfChanged(ref infolabel, value);
        }

        private EntraSocioMap _bindingt = new();
        public EntraSocioMap BindingT
        {
            get => this._bindingt;
            set => this.RaiseAndSetIfChanged(ref _bindingt, value);
        }

        private int _eta;
        public int Eta
        {
            get => _eta;
            set => this.RaiseAndSetIfChanged(ref _eta, value);
        }
    }
}
