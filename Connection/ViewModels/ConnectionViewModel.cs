using Common.InterViewModels;
using Microsoft.EntityFrameworkCore;
using Models.Context;
using ReactiveUI;
using Splat;
using SysNet;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;

namespace ViewModels
{
    public partial class ConnectionViewModel : BaseViewModel, IConnectionViewModel
    {

        public Interaction<Unit, Unit> UserIdFocus { get; } = new();

        #region Commands

        public ReactiveCommand<Unit, Unit> CheckCommand { get; protected set; }
        public ReactiveCommand<Unit, Unit> AvviaCommand { get; protected set; }

        private readonly ObservableAsPropertyHelper<bool> _isUiReady;
        public bool IsUiReady => _isUiReady.Value;

        #endregion

        private IObservable<bool> canCheck => this.WhenAnyValue(
        x => x.DatabaseText, x => x.PasswordText, x => x.UserIdText, x => x.SelectedInstance,
        (db, pass, user, server) =>
            !string.IsNullOrWhiteSpace(db) && !string.IsNullOrWhiteSpace(pass) &&
            !string.IsNullOrWhiteSpace(user) && !string.IsNullOrWhiteSpace(server));

        public ConnectionViewModel(IScreen host) : base(host)
        {
            InitializeLoadingHelper();

            _isUiReady = this.WhenAnyValue(
                                x => x.IsLoading,
                                x => x.IsDataLoaded,
                                (loading, loaded) => !loading && loaded)
                            .ToProperty(this, x => x.IsUiReady);



            CheckCommand = ReactiveCommand.CreateFromTask(OnCheckConnectionAsync, canCheck);
            AvviaCommand = ReactiveCommand.CreateFromTask(GoToLogin, this.WhenAnyValue(x => x.IsLoading, x => !x));

            // 3. Gestione del ciclo di vita (Activation)
            this.WhenActivated(d =>
            {
                CheckCommand.ThrownExceptions
    .               Subscribe(ex => {
                       // Qui l'app NON crasha più. Puoi mostrare un messaggio all'utente.
                       Debug.WriteLine($"Errore nel comando Check: {ex.Message}");
                       IsDataLoaded = true; // Sblocca la UI se necessario
    }               ).DisposeWith(d); // 'd' dal WhenActivated

                // Pulizia delle sottoscrizioni dei comandi quando la View viene deattivata
                CheckCommand.DisposeWith(d);
                AvviaCommand.DisposeWith(d);

                // Se hai altre sottoscrizioni WhenAnyValue specifiche, vanno qui
            });

        }

        // Estendiamo IsAnythingExecuting per includere CheckCommand
        protected override IObservable<bool> IsAnythingExecuting =>
            base.IsAnythingExecuting.CombineLatest(
                // Usiamo StartWith(false) per gestire il momento in cui il comando è null
                this.WhenAnyValue(x => x.CheckCommand)
                    .SelectMany(cmd => cmd?.IsExecuting ?? Observable.Return(false)),
                this.WhenAnyValue(x => x.AvviaCommand)
                    .SelectMany(cmd => cmd?.IsExecuting ?? Observable.Return(false)),
                (baseExec, check, avvia) => baseExec || check || avvia);


        protected override void OnFinalDestruction()
        {
            SqlInstances?.Clear();
            CheckCommand = null;
            AvviaCommand = null;
            base.OnFinalDestruction();
        }

        private async Task OnCheckConnectionAsync()
        {

            // Non serve IsLoading = true manuale
            var connectionString = $"Server={SelectedInstance?.Trim()};Database={DatabaseText?.Trim()};" +
                                   $"User Id={UserIdText?.Trim()};Password={PasswordText};" +
                                   "TrustServerCertificate=true;Connect Timeout=5;";

            try
            {
                // ... preparazione connectionString ...
                SysNet.Connection.SetConnectionString(connectionString);

                using (var db = new AppDbContext())
                {
                    // Se qui scoppia l'eccezione, viene catturata dal catch sotto
                    await db.Database.MigrateAsync(token);
                }

                AvviaVisibile = true;
            }
            catch (Exception ex)
            {
                // Gestisci l'errore localmente così il comando finisce "pulito"
                Debug.WriteLine($"Fallimento Migrazione: {ex.Message}");
                // Qui potresti triggerare un'Interaction per mostrare un messaggio all'utente
            }
            finally
            {
                await SetFocus(UserIdFocus);
            }
        }

        private async Task OnUserIdFocus()
        {
            // Fondamentale: aspetta un attimo che la View sia "viva" e l'handler registrato
            await Task.Delay(200);

            try
            {
                await UserIdFocus.Handle(Unit.Default);
            }
            catch (Exception ex)
            {
                // Evita crash se l'handler non è ancora pronto o la vista è già chiusa
                System.Diagnostics.Debug.WriteLine("Interaction Focus fallita: " + ex.Message);
            }
        }


        private async Task GoToLogin()
        {
            _isClosing = true; // Impedisce ulteriori interazioni durante la navigazione
            var vm = Locator.Current.GetService<ILoginViewModel>();
            await HostScreen.Router.NavigateAndReset.Execute(vm);
        }

        //Carica la ComboBox con le IstanzeSql
        protected override async Task OnLoading()
        {
            IsDataLoaded = false; // Reset


            // 1. Recupero dati in background (Task.Run va bene per SqlInstanceFinder)
            var instances = await Task.Run(() => SqlInstanceFinder.GetInstances(), token) ?? new List<string>();

            // 2. Ritorno sul thread principale per aggiornare la UI
            // Usiamo l'estensione di ReactiveUI per essere sicuri
            await Observable.Start(() =>
            {
                SqlInstances.Clear();
                foreach (var i in instances)
                {
                    SqlInstances.Add(i);
                }

                if (SqlInstances.Count > 0)
                    SelectedInstance = SqlInstances[0];

            }, RxSchedulers.MainThreadScheduler); // RxApp è più standard di RxSchedulers

            IsDataLoaded = true;

            _ = Task.Delay(300).ContinueWith(async _ =>
            {
                await SetFocus(UserIdFocus);
            });
        }

        protected override Task OnSaving() => Task.CompletedTask;

        protected override Task OnEsc() => Task.CompletedTask;
    }

    public partial class ConnectionViewModel
    {
        #region ListOfServers
        public ObservableCollection<string> SqlInstances { get; } = [];

        #endregion

        #region SelectedSqlInstance

        private string _selectedInstance;
        public string SelectedInstance
        {
            get => _selectedInstance;
            set => this.RaiseAndSetIfChanged(ref _selectedInstance, value);
        }

        #endregion

        
        //User Id
        private string useridtext = string.Empty;
        public string UserIdText
        {
            get => useridtext;
            set => this.RaiseAndSetIfChanged(ref useridtext, value);
        }

        //Password
        private string passwordtext = string.Empty;
        public string PasswordText
        {
            get => passwordtext;
            set => this.RaiseAndSetIfChanged(ref passwordtext, value);
        }

        //Database
        private string databasetext = string.Empty;
        public string DatabaseText
        {
            get => databasetext;
            set => this.RaiseAndSetIfChanged(ref databasetext, value);
        }

        //AvviaVisibile
        private bool avviavisibile = false;
        public bool AvviaVisibile
        {
            get => avviavisibile;
            set => this.RaiseAndSetIfChanged(ref avviavisibile, value);
        }
        

        private bool _enabledcheck;
        public bool EnabledCheck
        {
            get => _enabledcheck;
            set => this.RaiseAndSetIfChanged(ref _enabledcheck, value);
        }

        #region Observable

        private bool _isDataLoaded;
        public bool IsDataLoaded
        {
            get => _isDataLoaded;
            set => this.RaiseAndSetIfChanged(ref _isDataLoaded, value);
        }


        #endregion
    }
}
