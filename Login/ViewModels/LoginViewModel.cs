using Common.InterViewModels;
using DTO.Repository;
using ReactiveUI;
using Splat;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Linq;
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
                pass == operatore.Password);


        public LoginViewModel(IScreen host, ILoginRepository Repository) : base(null)
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

            if (dbData?.Count > 0)
            {

                // Trasforma l'Expression in una funzione e usala con LINQ .Select()
                // Aggiorna la DataSource della UI
                DataSource = dbData.Select(dto => new LoginMap(dto)).ToList();

                // Seleziona il primo operatore
                BindingT = DataSource[0];
            }

            if (!_isClosing)
                await SetFocus(PasswordFocus);

        }

        protected override async Task OnSaving()
        {
            
            try
            {
                // Salva le impostazioni dell'operatore selezionato
                await Q.SaveSettings(BindingT.ToDto());

                // Naviga al Menu principale resettando lo stack di navigazione
                await GoToMenu();
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

        // ---------------------------------------------------------------------
        // 5. Metodi di Supporto Privati
        // ---------------------------------------------------------------------
        private async Task GoToMenu()
        {
            var menuVm = Locator.Current.GetService<IMenuViewModel>();

            if (menuVm == null)
            {
                _isClosing = false; // Permette di riprovare se il DI fallisce
                Debug.WriteLine("ERRORE CRITICO: IMenuViewModel non è stato risolto dal Locator.");
                return;
            }

           
            try
            {
                _isClosing = true;

                // Navigazione reattiva nativa e pulita sul thread della UI, senza wrapper Observable extra
                await _host.Router.NavigateAndReset.Execute(menuVm);
            }
            catch (Exception ex)
            {
                _isClosing = false;
                Debug.WriteLine($"ERRORE durante la navigazione al Menu: {ex.Message}");
            }

            
        }
    }

    public partial class LoginViewModel
    {
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
