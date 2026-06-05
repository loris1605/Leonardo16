using Common.InterViewModels;
using DTO.Repository;
using ReactiveUI;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using ViewModels.BindableObjects;

namespace ViewModels
{

    public partial class LoginViewModel : ViewModelBase, ILoginViewModel
    {
        // ---------------------------------------------------------------------
        // 1. Dipendenze e Campi Privati
        // ---------------------------------------------------------------------
        private ILoginRepository Q;
        private IScreen _host;


        // ---------------------------------------------------------------------
        // 3. Condizioni di Esecuzione (Override)
        // ---------------------------------------------------------------------
        protected override IObservable<bool> CanSave => this.WhenAnyValue(
            x => x.PasswordText,
            x => x.BindingT,
            (pass, operatore) =>
                !string.IsNullOrWhiteSpace(pass) &&
                operatore != null &&
                pass == operatore.Password)
            // Evita che ogni singolo carattere digitato intasi il flusso CombineLatest della base
            .DistinctUntilChanged()
            .ObserveOn(RxSchedulers.MainThreadScheduler);


        public LoginViewModel(IScreen host, ILoginRepository Repository) : base(host)
        {
            Q = Repository ?? throw new ArgumentNullException(nameof(Repository));
            _host = host;

        } 


        protected override void OnFinalDestruction()
        {
            // Pulizia esplicita per agevolare il Garbage Collector forzato della Base
            Q = null;
            _host = null;
            DataSource = null;
            BindingT = null;
            PasswordText = null;

            base.OnFinalDestruction();
        }

        // ---------------------------------------------------------------------
        // 4. Ciclo di Vita (Override dei Metodi Virtuali)
        // ---------------------------------------------------------------------
        protected override async Task OnLoading()
        {
            var dbData = await Q.GetOperatoriAbilitati(Token);

            if (Token.IsCancellationRequested) return;

            if (dbData?.Count > 0)
            {
                var localList = dbData.Select(dto => new LoginMap(dto)).ToList();
                // Trasforma l'Expression in una funzione e usala con LINQ .Select()
                // Aggiorna la DataSource della UI

                DataSource = localList;
                await Task.Delay(10, Token);
                if (Token.IsCancellationRequested) return;
                // Seleziona il primo operatore
                BindingT = localList[0];
            }

            if (!_isClosing && !Token.IsCancellationRequested)
            {
                await SetFocus(PasswordFocus);
            }

        }

        protected override async Task OnSaving()
        {
            
            try
            {
                // Salva le impostazioni dell'operatore selezionato
                await Q.SaveSettings(BindingT.ToDto(), Token);

                // Naviga al Menu principale resettando lo stack di navigazione
                // 2. Al posto di GoToMenu(), suoniamo il campanello!
                _isClosing = true;
                _loginSuccesso.OnNext(Unit.Default);
                _loginSuccesso.OnCompleted(); // Chiude il canale per sempre
            }
            catch (OperationCanceledException)
            {
                _isClosing = false;
                Debug.WriteLine("Salvataggio login annullato tramite Token.");
            }
            catch (Exception ex)
            {
                _isClosing = false;
                Debug.WriteLine($">>> [ERROR] Login fallito durante il salvataggio o la navigazione: {ex.Message}");
                // Qui potresti aggiungere un'interaction per mostrare un messaggio di errore all'utente
                throw; // Rilancia l'eccezione se vuoi che venga gestita a un livello superiore
            }

        }

        

        protected override Task OnEsc()
        {
            OnAppShutDown(); // Riutilizza il metodo centralizzato della base per spegnere l'app
            return Task.CompletedTask;
        }

    }

    public partial class LoginViewModel
    {
        // 1. Aggiungi questo Subject per notificare l'esterno
        private readonly Subject<Unit> _loginSuccesso = new();
        public IObservable<Unit> LoginSuccesso => _loginSuccesso.AsObservable();

        // ---------------------------------------------------------------------
        // 2. Proprietà e Stato della UI (con Bindings)
        // ---------------------------------------------------------------------
        private string _passwordText;
        public string PasswordText
        {
            get => _passwordText;
            set => this.RaiseAndSetIfChanged(ref _passwordText, value);
        }

        private LoginMap _bindingT;
        public LoginMap BindingT
        {
            get => _bindingT;
            set => this.RaiseAndSetIfChanged(ref _bindingT, value);
        }

        private List<LoginMap> _dataSource;
        public List<LoginMap> DataSource
        {
            get => _dataSource;
            set => this.RaiseAndSetIfChanged(ref _dataSource, value);
        }

        // Interazioni con la View
        public Interaction<Unit, Unit> PasswordFocus { get; } = new();

    }
}
