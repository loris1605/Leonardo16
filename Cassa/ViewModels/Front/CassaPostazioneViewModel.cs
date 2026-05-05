using Common.InterViewModels;
using ReactiveUI;
using System.Reactive;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Subjects;
using ViewModels.BindableObjects;

namespace ViewModels
{
    public partial class CassaPostazioneViewModel : BaseViewModel, ICassaPostazioneViewModel
    {
        private int _postazioneId;
        private ICassaScreen _host;

        public ReactiveCommand<Unit, Unit> EntraSocioCommand { get; }
        public ReactiveCommand<Unit, Unit> EsceSocioCommand { get; }
        public ReactiveCommand<Unit, Unit> ListaSociCommand { get; }
        public ReactiveCommand<Unit, Unit> PosizioneEnterCommand { get; }


        public CassaPostazioneViewModel() : base()
        {
            //Titolo = $"Postazione {cassaPostazione.NomePostazione}";

            _isOpen = _isOpenManualTrigger.ToProperty(this, x => x.IsOpen);

            PosizioneEnterCommand = ReactiveCommand.CreateFromTask(OnApriScheda);
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


        public void SetPostazioneId(int postazioneId)
        {
            _postazioneId = postazioneId;
        }

        public void SetHost(ICassaScreen host) => _host = host;

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
            //if (string.IsNullOrWhiteSpace(BindingT.Posizione))
            //{
            //    _isOpenManualTrigger.OnNext(false);
            //    return;
            //}

            //BindingT.Nome = "Loris"; // Simulazione di un nome associato alla posizione, da sostituire con la logica reale
            //BindingT.Cognome = "Rossi"; // Simulazione di un cognome associato alla posizione, da sostituire con la logica reale

            //_isOpenManualTrigger.OnNext(true);
            // Logica per entrare nella postazione
            // Esempio: await PostazioneService.EntraPostazioneAsync(BindingT.Posizione);
            await Task.CompletedTask;
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
