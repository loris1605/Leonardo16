using Common.InterViewModels;
using Contracts.ViewModels;
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
    

    public partial class ConnectionViewModel : ViewModelBase, IConnectionViewModel
    {
        // ---------------------------------------------------------------------
        // 1. Campi Privati e Stato Interno
        // ---------------------------------------------------------------------
        protected IScreen _host;
        private readonly ObservableAsPropertyHelper<bool> _isUiReady;

        // ---------------------------------------------------------------------
        // 2. Proprietà di Sola Lettura e Interazioni
        // ---------------------------------------------------------------------
        public bool IsUiReady => _isUiReady.Value;
        public Interaction<Unit, Unit> UserIdFocus { get; } = new();


        // Comandi Reattivi esposti alla View
        public ReactiveCommand<Unit, Unit> CheckCommand { get; protected set; }
        public ReactiveCommand<Unit, Unit> AvviaCommand { get; protected set; }

        // ---------------------------------------------------------------------
        // 3. Condizioni di Esecuzione e Flussi Reattivi
        // ---------------------------------------------------------------------
        private IObservable<bool> canCheck => this.WhenAnyValue(
            x => x.DatabaseText, x => x.PasswordText, x => x.UserIdText, x => x.SelectedInstance,
            (db, pass, user, server) =>
                !string.IsNullOrWhiteSpace(db) && !string.IsNullOrWhiteSpace(pass) &&
                !string.IsNullOrWhiteSpace(user) && !string.IsNullOrWhiteSpace(server));

        // ---------------------------------------------------------------------
        // 5. Flussi Reattivi Centralizzati (Override Corretto)
        // ---------------------------------------------------------------------
        protected override IObservable<bool> IsAnythingExecuting =>
            Observable.CombineLatest(
                // 1. Monitoriamo i comandi della classe base
                this.WhenAnyObservable(
                    x => x.LoadCommand.IsExecuting,
                    x => x.SaveCommand.IsExecuting,
                    x => x.EscPressedCommand.IsExecuting),
                // 2. Monitoriamo i due comandi specifici di questa schermata
                this.WhenAnyObservable(
                    x => x.CheckCommand.IsExecuting,
                    x => x.AvviaCommand.IsExecuting),
                // Se anche uno solo di questi 5 comandi è in esecuzione, restituisce true
                (baseExec, localExec) => baseExec || localExec)
            .DistinctUntilChanged();

        public ConnectionViewModel(IScreen host) : base(null)
        {
            _host = host;
            // 1. Inizializzazione dello stato di prontezza della UI (già presente)
            _isUiReady = this.WhenAnyValue(
                    x => x.IsLoading,
                    x => x.IsDataLoaded,
                    (loading, loaded) => !loading && loaded)
                .ObserveOn(RxSchedulers.MainThreadScheduler)
                .ToProperty(this, x => x.IsUiReady);

            // 2. Inizializzazione dei Comandi dedicati della schermata
            CheckCommand = ReactiveCommand.CreateFromTask(OnCheckConnectionAsync, canCheck);
            AvviaCommand = ReactiveCommand.CreateFromTask(GoToLogin, this.WhenAnyValue(x => x.IsLoading, loading => !loading));

            // =====================================================================
            // SOLUZIONE REATTIVA: Uniamo l'esecuzione dei comandi locali a IsLoading della base
            // =====================================================================
            this.WhenAnyObservable(
                    x => x.CheckCommand.IsExecuting,
                    x => x.AvviaCommand.IsExecuting)
                .Where(executing => executing) // Se uno dei comandi parte (true)
                .Subscribe(_ =>
                {
                    // Forziamo lo stato di chiusura/blocco temporaneo per fermare i doppi clic
                    // Questo si integra perfettamente con i controlli della tua ViewModelBase
                    _isClosing = true;
                });

            // Quando i comandi finiscono, ripristiniamo lo stato se l'app non sta cambiando schermata
            CheckCommand.IsExecuting
                .CombineLatest(AvviaCommand.IsExecuting, (c, a) => c || a)
                .Where(anyExecuting => !anyExecuting) // Quando entrambi hanno finito (false)
                .Subscribe(_ =>
                {
                    // Se GoToLogin non ha impostato definitivamente _isClosing a true per cambiare pagina, sblocchiamo
                    if (AvviaCommand.IsExecuting.Latest().First() == false && !_isClosing)
                    {
                        _isClosing = false;
                    }
                });
            // =====================================================================

            // 3. Gestione del ciclo di vita (Activation)
            this.WhenActivated(d =>
            {
                CheckCommand.ThrownExceptions
                    .Subscribe(ex =>
                    {
                        Debug.WriteLine($"[WARN] Errore nel comando Check: {ex.Message}");
                        IsDataLoaded = true;
                    })
                    .DisposeWith(d);

                CheckCommand.DisposeWith(d);
                AvviaCommand.DisposeWith(d);
            });
        }

    
        protected override void OnFinalDestruction()
        {
            SqlInstances?.Clear();
            _host = null;
            CheckCommand = null;
            AvviaCommand = null;
            base.OnFinalDestruction();
        }

        // ---------------------------------------------------------------------
        // 5. Metodi di Logica Interna (Task dei Comandi)
        // ---------------------------------------------------------------------
        private async Task OnCheckConnectionAsync()
        {
            var connectionString = $"Server={SelectedInstance?.Trim()};Database={DatabaseText?.Trim()};" +
                                   $"User Id={UserIdText?.Trim()};Password={PasswordText};" +
                                   "TrustServerCertificate=true;Connect Timeout=5;";

            try
            {
                SysNet.Connection.SetConnectionString(connectionString);

                using (var db = new AppDbContext())
                {
                    // Esegue le migrazioni del database usando il Token della classe base
                    await db.Database.MigrateAsync(Token);
                }

                AvviaVisibile = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Fallimento Migrazione / Connessione: {ex.Message}");
                // Qui puoi inserire un'interazione per mostrare un popup di errore grafico nella View
                AvviaVisibile = false;
            }
            finally
            {
                await SetFocus(UserIdFocus, delay: 50);
            }
        }


        private async Task GoToLogin()
        {
            _isClosing = true; // Blocca i comandi generali durante la transizione di schermata

            var loginVm = Locator.Current.GetService<ILoginViewModel>();
            if (loginVm != null)
            {
                try
                {
                    // Navigazione nativa e pulita con reset dello stack, forzata sul thread UI principale
                    await _host.Router.NavigateAndReset.Execute(loginVm);
                        
                }
                catch (Exception ex)
                {
                    _isClosing = false;
                    Debug.WriteLine($"ERRORE durante la navigazione al Login: {ex.Message}");
                }
            }
            else
            {
                _isClosing = false; // Consente di riprovare se la risoluzione IoC fallisce
                Debug.WriteLine("ERRORE CRITICO: ILoginViewModel non è stato risolto dal Locator.");
            }
        }


        // ---------------------------------------------------------------------
        // 4. Ciclo di Vita (Override dei Metodi Virtuali della Base)
        // ---------------------------------------------------------------------
        protected override async Task OnLoading()
        {
            IsDataLoaded = false; // Reset dello stato di caricamento dati

            // 1. Recupero delle istanze SQL in background (utilizzando il Token ereditato dalla base)
            var instances = await Task.Run(() => SqlInstanceFinder.GetInstances(), Token) ?? new List<string>();

            // 2. Ritorno sicuro sul thread della UI per aggiornare la collezione
            await Observable.Start(() =>
            {
                SqlInstances.Clear();
                foreach (var i in instances)
                {
                    SqlInstances.Add(i);
                }

                if (SqlInstances.Count > 0)
                {
                    SelectedInstance = SqlInstances[0];
                }
            }, RxSchedulers.MainThreadScheduler);

            IsDataLoaded = true;

            // Spostamento del focus sull'ID utente in modo asincrono e thread-safe tramite la base
            if (!_isClosing)
            {
                await SetFocus(UserIdFocus, delay: 300);
            }
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
