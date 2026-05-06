using Common.InterViewModels;
using ReactiveUI;
using Splat;
using System.Reactive;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using ViewModels.BindableObjects;

namespace ViewModels
{
    public partial class CassaPostazioneViewModel : BaseViewModel, ICassaPostazioneViewModel
    {
        private int _postazioneId;
        
        private ICassaScreen _host;

        public ReactiveCommand<Unit, Unit> EntraSocioCommand { get; protected set; }
        public ReactiveCommand<Unit, Unit> EsceSocioCommand { get; }
        public ReactiveCommand<Unit, Unit> ListaSociCommand { get; }
        public ReactiveCommand<Unit, Unit> PosizioneEnterCommand { get; }

        protected override IObservable<bool> IsAnythingExecuting =>
            new[]
            {
                base.IsAnythingExecuting,
                EntraSocioCommand?.IsExecuting ?? Observable.Return(false)
                //SettoriCommand?.IsExecuting ?? Observable.Return(false),
                //PermessiCommand?.IsExecuting ?? Observable.Return(false),
                //TariffeCommand?.IsExecuting ?? Observable.Return(false)
            }.CombineLatest(values => values.Any(x => x));


        public CassaPostazioneViewModel() : base()
        {
            //Titolo = $"Postazione {cassaPostazione.NomePostazione}";

            _isOpen = _isOpenManualTrigger.ToProperty(this, x => x.IsOpen);

            PosizioneEnterCommand = ReactiveCommand.CreateFromTask(OnApriScheda);
            EntraSocioCommand = ReactiveCommand.CreateFromTask(OnEntraSocio);
            //ListaSociCommand = ReactiveCommand.CreateFromTask(async () =>
            //{
            //    await HostScreen.Router.Navigate.Execute(new ListaSociViewModel(HostScreen, cassaPostazione,
            //        Locator.Current.GetService<ISchedaRepository>()));
            //});

            this.WhenActivated(d =>
            {
                EntraSocioCommand?.DisposeWith(d);
                EsceSocioCommand?.DisposeWith(d);
                ListaSociCommand?.DisposeWith(d);
                PosizioneEnterCommand?.DisposeWith(d);
                _isOpenManualTrigger?.DisposeWith(d);
            });
        }

        protected override void OnFinalDestruction()
        {
            // Assicuriamoci che la collezione sia nulla per il GC
            //Q = null;
            EntraSocioCommand = null;
            //SettoriCommand = null;
            //PermessiCommand = null;
            //TariffeCommand = null;
            base.OnFinalDestruction();
        }


        public void SetPostazioneId(int posizioneId)
        {
            _postazioneId = posizioneId;
        }

        public void SetHost(ICassaScreen host) => _host = host;

        public void SetPosizione(string numPosizione)
        {
            BindingT.Posizione = numPosizione;
        }

        protected async override Task OnEsc() => await _host.OnClosing();
                                                        

        protected override async Task OnLoading()
        {
            await SetFocus(PosizioneFocus);
            await Task.CompletedTask;
        }

        protected override async Task OnSaving()
        {
            await Task.CompletedTask;
        }

        private async Task OnApriScheda()
        {
            if (string.IsNullOrWhiteSpace(BindingT.Posizione))
            {
                _isOpenManualTrigger.OnNext(false);
                return;
            }

            //BindingT.Nome = "Loris"; // Simulazione di un nome associato alla posizione, da sostituire con la logica reale
            //BindingT.Cognome = "Rossi"; // Simulazione di un cognome associato alla posizione, da sostituire con la logica reale

            _isOpenManualTrigger.OnNext(true);
            // Logica per entrare nella postazione
            // Esempio: await PostazioneService.EntraPostazioneAsync(BindingT.Posizione);
            await Task.CompletedTask;
        }

        private async Task OnEntraSocio()
        {
            _isClosing = true;
            var entrasocioVm = Locator.Current.GetService<IEntraSocioViewModel>();
            if (entrasocioVm is not null)
            {
                entrasocioVm.SetHost(_host);
                entrasocioVm.SetPostazioneId(_postazioneId);
                entrasocioVm.SetPosizione(BindingT.Posizione);
                await _host.CassaRouter.NavigateAndReset.Execute(entrasocioVm);
            }
            else
            {
                _isClosing = false;
                await SetFocus(PosizioneFocus);
            }
        }
        
    }

    public partial class CassaPostazioneViewModel
    {
        private string _titolo = string.Empty;
        public string Titolo
        {
            get => _titolo;
            set => this.RaiseAndSetIfChanged(ref _titolo, value);
        }

        private SchedaMap bindingt = new();
        public SchedaMap BindingT
        {
            get => bindingt;
            set => this.RaiseAndSetIfChanged(ref bindingt, value);

        }

        public Interaction<Unit, Unit> PosizioneFocus { get; } = new();

        readonly ObservableAsPropertyHelper<bool> _isOpen;
        public bool IsOpen => _isOpen.Value;
        private readonly Subject<bool> _isOpenManualTrigger = new Subject<bool>();

    }
}
