using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
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
    

    public partial class LoginViewModel : BaseViewModel, ILoginViewModel
    {
        private ILoginRepository Q;
        private IScreen _host;


        protected override IObservable<bool> canSave => this.WhenAnyValue(
            x => x.PasswordText,
            x => x.BindingT,
            (pass, operatore) =>
                !string.IsNullOrWhiteSpace(pass) &&
                operatore != null &&
                pass == operatore.Password);


        public LoginViewModel(ILoginRepository Repository) : base(null)
        {
            Q = Repository ?? throw new ArgumentNullException(nameof(Repository));
            
        } 

        public void SetHost(IScreen host)
        {
            _host = host;
        }

        protected override void OnFinalDestruction()
        {
            Q = null;
            DataSource = null;
            BindingT = null;
            base.OnFinalDestruction();
        }

        protected override async Task OnLoading()
        {
            var dbData = await Q.GetOperatoriAbilitati(token);

            if (dbData?.Count > 0)
            {

                // Trasforma l'Expression in una funzione e usala con LINQ .Select()
                // Aggiorna la DataSource della UI
                DataSource = dbData.Select(dto => new LoginMap(dto)).ToList();

                // Seleziona il primo
                BindingT = DataSource[0];
            }

            if (!_isClosing)
                await SetFocus(PasswordFocus);

        }

        protected override async Task OnSaving()
        {
            _isClosing = true; // Blocco preventivo immediato

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

        private async Task GoToMenu()
        {
            _isClosing = true;
            var menuVm = Locator.Current.GetService<IMenuViewModel>();
            if (menuVm != null)
            {
                // 2. Impostiamo l'host (lo screen principale)
                menuVm.SetHost(_host);

                try
                {
                    // 3. Eseguiamo la navigazione FORZANDOLA sul Main Thread della UI
                    await Observable.Start(async () =>
                    {
                        await _host.Router.NavigateAndReset.Execute(menuVm);
                    }, RxSchedulers.MainThreadScheduler);
                }
                catch (Exception ex)
                {
                    _isClosing = false;
                    Debug.WriteLine($"ERRORE durante la navigazione al Menu: {ex.Message}");
                }
            }
            else
            {
                _isClosing = false; // Permette all'utente di riprovare se il DI fallisce
                Debug.WriteLine("ERRORE CRITICO: IMenuViewModel non è stato risolto dal Locator.");
            }
        }

        protected async override Task OnEsc()
        {
            _isClosing = true; // Blocco preventivo immediato

            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime)
                lifetime.Shutdown();

            await Task.CompletedTask;


        }
    }

    public partial class LoginViewModel
    {
        //DataSource della ComboBox
        private List<LoginMap> _datasource;
        public List<LoginMap> DataSource
        {
            get => _datasource;
            set => this.RaiseAndSetIfChanged(ref _datasource, value);
        }

        private string _mypassordtext = string.Empty;
        public string PasswordText
        {
            get => _mypassordtext;
            set => this.RaiseAndSetIfChanged(ref _mypassordtext, value);
        }

        private LoginMap bindingt = new();
        public LoginMap BindingT
        {
            get => bindingt;
            set => this.RaiseAndSetIfChanged(ref bindingt, value);
        }

        #region Observable

        public Interaction<Unit, Unit> PasswordFocus { get; } = new();

        #endregion

    }
}
