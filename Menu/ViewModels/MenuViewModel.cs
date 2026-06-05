using Common.InterViewModels;
using DTO.Repository;
using Menu.ViewModels.Map;
using Models.Entity.Global;
using ReactiveUI;
using Splat;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace ViewModels
{
   
    public partial class MenuViewModel : ViewModelBase, IMenuViewModel
    {
        // ---------------------------------------------------------------------
        // 1. Dipendenze e Campi Privati
        // ---------------------------------------------------------------------
        private IMenuRepository Q;
        private IScreen _host;

        // Implementazione dell'interfaccia IRoutableViewModel richiesta da ReactiveUI
        public new IScreen HostScreen => _host;

        // ---------------------------------------------------------------------
        // 2. Comandi Reattivi Esposti alla View
        // ---------------------------------------------------------------------
        public ReactiveCommand<string, Unit> NavigateCommand { get; }
        public ReactiveCommand<int, Unit> SelezionaPostazioneCommand { get; }
        public ReactiveCommand<Unit, Unit> LogoutCommand { get; }
        public ReactiveCommand<Unit, Unit> ConnectionCommand { get; }
        public ReactiveCommand<Unit, Unit> ConfigurazioneCommand { get; }
        public ReactiveCommand<Unit, Unit> SociCommand { get; }
        public ReactiveCommand<Unit, Unit> ApriGiornataCommand { get; }

        // ---------------------------------------------------------------------
        // 3. Flussi Reattivi Centralizzati (Override Controllo Doppio Clic Senza "base")
        // ---------------------------------------------------------------------
        protected override IObservable<bool> IsAnythingExecuting =>
            Observable.CombineLatest(
                // 1. Comandi ereditati dalla classe base
                this.WhenAnyObservable(x => x.LoadCommand.IsExecuting).StartWith(false),
                this.WhenAnyObservable(x => x.SaveCommand.IsExecuting).StartWith(false),
                this.WhenAnyObservable(x => x.EscPressedCommand.IsExecuting).StartWith(false),
                // 2. Comandi specifici di questa schermata (gestiti in modo safe se null all'avvio)
                this.WhenAnyValue(x => x.SelezionaPostazioneCommand).SelectMany(cmd => cmd?.IsExecuting ?? Observable.Return(false)),
                this.WhenAnyValue(x => x.LogoutCommand).SelectMany(cmd => cmd?.IsExecuting ?? Observable.Return(false)),
                this.WhenAnyValue(x => x.ConnectionCommand).SelectMany(cmd => cmd?.IsExecuting ?? Observable.Return(false)),
                this.WhenAnyValue(x => x.ConfigurazioneCommand).SelectMany(cmd => cmd?.IsExecuting ?? Observable.Return(false)),
                this.WhenAnyValue(x => x.SociCommand).SelectMany(cmd => cmd?.IsExecuting ?? Observable.Return(false)),
                this.WhenAnyValue(x => x.ApriGiornataCommand).SelectMany(cmd => cmd?.IsExecuting ?? Observable.Return(false)),
                // Se anche uno solo dei 9 comandi totali è in esecuzione, IsLoading diventa true e la UI si blocca a 0ms
                (l, s, e, sel, log, conn, conf, soc, apri) => l || s || e || sel || log || conn || conf || soc || apri)
            .DistinctUntilChanged();


        // ---------------------------------------------------------------------
        // Constructor
        // ---------------------------------------------------------------------
        public MenuViewModel(IScreen host, IMenuRepository Repository) : base(null)
        {
            Q = Repository ?? throw new ArgumentNullException(nameof(Repository));
            _host = host;

            // 1. Collegamento e aggiornamento delle proprietà OAPH definite nel file parziale
            _chiudiGiornataEnabled = this.WhenAnyValue(x => x.ApriGiornataEnabled)
                .Select(x => !x)
                .ObserveOn(RxSchedulers.MainThreadScheduler)
                .ToProperty(this, x => x.ChiudiGiornataEnabled);

            _sessioneContabile = this.WhenAnyValue(x => x.ApriGiornataEnabled)
                .Select(v => $"Sessione Contabile {(v ? "Chiusa" : "Aperta")}")
                .ObserveOn(RxSchedulers.MainThreadScheduler)
                .ToProperty(this, x => x.SessioneContabile);

            // Vincolo generale di navigazione basato sullo stato globale IsLoading della base
            var canNavigate = this.WhenAnyValue(x => x.IsLoading)
                .Select(isLoading => !isLoading)
                .ObserveOn(RxSchedulers.MainThreadScheduler);

            // Inizializzazione dei Comandi
            SelezionaPostazioneCommand = ReactiveCommand.CreateFromTask<int>(GoToCassa, canNavigate);
            LogoutCommand = ReactiveCommand.CreateFromTask(GoToLogin, canNavigate);
            ConnectionCommand = ReactiveCommand.CreateFromTask(GoToConnection, canNavigate);
            ConfigurazioneCommand = ReactiveCommand.CreateFromTask(GoToConfigurazione, canNavigate);
            SociCommand = ReactiveCommand.CreateFromTask(GoToSoci, canNavigate);

            var canApriFinal = this.WhenAnyValue(x => x.ApriGiornataEnabled, x => x.IsLoading,
                    (enabled, loading) => enabled && !loading);

            ApriGiornataCommand = ReactiveCommand.CreateFromTask(ExecuteOpenGiornata, canApriFinal);

            // Gestione del Ciclo di Vita (Activation)
            this.WhenActivated(d =>
            {
                // Gestione e tracciamento centralizzato delle eccezioni
                SelezionaPostazioneCommand.ThrownExceptions.Subscribe(ex => Debug.WriteLine($"Errore comando Selezione Cassa: {ex.Message}")).DisposeWith(d);
                LogoutCommand.ThrownExceptions.Subscribe(ex => Debug.WriteLine($"Errore comando Logout: {ex.Message}")).DisposeWith(d);
                ConnectionCommand.ThrownExceptions.Subscribe(ex => Debug.WriteLine($"Errore comando Connessione: {ex.Message}")).DisposeWith(d);
                ConfigurazioneCommand.ThrownExceptions.Subscribe(ex => Debug.WriteLine($"Errore comando Configurazione: {ex.Message}")).DisposeWith(d);
                SociCommand.ThrownExceptions.Subscribe(ex => Debug.WriteLine($"Errore comando Soci: {ex.Message}")).DisposeWith(d);
                ApriGiornataCommand.ThrownExceptions.Subscribe(ex => Debug.WriteLine($"Errore comando Apertura Giornata: {ex.Message}")).DisposeWith(d);

                // Pulizia delle risorse e delle sottoscrizioni all'attivazione/disattivazione della View
                LogoutCommand.DisposeWith(d);
                SelezionaPostazioneCommand.DisposeWith(d);
                ConfigurazioneCommand.DisposeWith(d);
                ApriGiornataCommand.DisposeWith(d);
                SociCommand.DisposeWith(d);
                ConnectionCommand.DisposeWith(d);
            });

        }

        // ---------------------------------------------------------------------
        // 4. Ciclo di Vita (Override dei Metodi Virtuali della Base)
        // ---------------------------------------------------------------------
        
        protected override async Task OnLoading()
        {
            if (GlobalValuesC.MySetting == null) return;

            AttivaPermessi();

            // Caricamento dei dati asincroni passando correttamente il Token della base
            var listaDto = await Q.CaricaPostazioniCassa(GlobalValuesC.MySetting.IDOPERATORE, Token);

            CassaPostazioniDataSource = listaDto
                .Select(dto => new MenuPostazioneMap(dto))
                .ToList();

            ApriGiornataEnabled = !(await Q.EsisteGiornataAperta(Token));

            if (GlobalValuesC.MySetting.POSTAZIONI?.Count == 0)
            {
                ApriPostazioneEnabled = false;
            }
        }
        protected override async Task OnEsc() => await GoToLogin();
        protected override void OnFinalDestruction()
        {
            CassaPostazioniDataSource?.Clear();
            Q = null;
            _host = null;

            base.OnFinalDestruction();
        }

        // 1. Aggiungi questo Subject per notificare l'esterno
        private readonly Subject<Unit> _menuToLogin = new();
        public IObservable<Unit> MenuToLogin => _menuToLogin.AsObservable();

        private readonly Subject<Unit> _menuToSoci = new();
        public IObservable<Unit> MenuToSoci => _menuToSoci.AsObservable();


        private void AttivaPermessi()
        {
            if (GlobalValuesC.MySetting is null) return;

            OperatoreName = "Operatore : " + GlobalValuesC.MySetting.NOMEOPERATORE;
            //SessioneContabile = "Sessione Contabile " + (ApriGiornataEnabled ? "Chiusa" : "Aperta");

            if (GlobalValuesC.MySetting.POSTAZIONI is null) return;

            try
            {
                foreach (PostazioneXC Element in GlobalValuesC.MySetting.POSTAZIONI)
                {
                    switch (Element.TIPOPOSTAZIONE)
                    {
                        case (int)Enums.Postazioni.Amministratore:
                            AmministratoreVisible = true;
                            ReportVisible = true;
                            break;

                        case (int)Enums.Postazioni.Cassa:
                            CassaVisible = true;
                            ReportVisible = true;
                            break;

                        case (int)Enums.Postazioni.Bar:
                            BarVisible = true;
                            break;

                        case (int)Enums.Postazioni.Guardaroba:
                            GuardarobaVisible = true;
                            break;

                        case (int)Enums.Postazioni.Pulizie:
                            PulizieVisible = true;
                            break;

                    }
                }
            }
            catch (NullReferenceException)
            {
                return;
            }

            IsMenuReady = true;


        }

                

    }

    public partial class MenuViewModel
    {
        // ---------------------------------------------------------------------
        // 5. Logica Interna (Task dei Comandi)
        // ---------------------------------------------------------------------
        private async Task GoToCassa(int postazioneId)
        {
            _isClosing = true;

            var cassaVm = Locator.Current.GetService<ICassaViewModel>();
            if (cassaVm != null)
            {
                cassaVm.SetHost(_host);
                cassaVm.SetPostazioneId(postazioneId);
                try
                {
                    await _host.Router.NavigateAndReset.Execute(cassaVm);
                }
                catch (Exception ex)
                {
                    _isClosing = false;
                    Debug.WriteLine($"ERRORE durante la navigazione alla Cassa: {ex.Message}");
                }
            }
            else
            {
                _isClosing = false; // Permette all'utente di riprovare se il DI fallisce
                Debug.WriteLine("ERRORE CRITICO: ICassaViewModel non è stato risolto dal Locator.");
            }
            await Task.CompletedTask;
            //await HostScreen.Router.NavigateAndReset.Execute(new CassaViewModel(HostScreen, postazioneId));
        }

        private async Task GoToLogin()
        {
            _isClosing = true; // Impedisce ulteriori interazioni durante la navigazione
            _menuToLogin.OnNext(Unit.Default);
            _menuToLogin.OnCompleted();

            await Task.CompletedTask;
        }

        private async Task GoToConnection()
        {
            _isClosing = true; // Impedisce ulteriori interazioni durante la navigazione
            var connectionVm = Locator.Current.GetService<IConnectionViewModel>();
            if (connectionVm != null)
            {
                // 2. Impostiamo l'host (lo screen principale)
                try
                {
                    // 3. Eseguiamo la navigazione FORZANDOLA sul Main Thread della UI
                    await _host.Router.NavigateAndReset.Execute(connectionVm);
                }
                catch (Exception ex)
                {
                    _isClosing = false;
                    Debug.WriteLine($"ERRORE durante la navigazione al Connection: {ex.Message}");
                }
            }
            else
            {
                _isClosing = false; // Permette all'utente di riprovare se il DI fallisce
                Debug.WriteLine("ERRORE CRITICO: IConnectionViewModel non è stato risolto dal Locator.");
            }
        }

        private async Task GoToConfigurazione()
        {
            _isClosing = true; // Impedisce ulteriori interazioni durante la navigazione
            var configurazioneVm = Locator.Current.GetService<IConfigurazioneViewModel>();
            if (configurazioneVm != null)
            {
                // 2. Impostiamo l'host (lo screen principale)
                configurazioneVm.SetHost(_host);
                try
                {
                    // 3. Eseguiamo la navigazione FORZANDOLA sul Main Thread della UI
                    await _host.Router.NavigateAndReset.Execute(configurazioneVm);
                }
                catch (Exception ex)
                {
                    _isClosing = false;
                    Debug.WriteLine($"ERRORE durante la navigazione alla Configurazione: {ex.Message}");
                }
            }
            else
            {
                _isClosing = false; // Permette all'utente di riprovare se il DI fallisce
                Debug.WriteLine("ERRORE CRITICO: IConfigurazioneViewModel non è stato risolto dal Locator.");
            }
        }

        private async Task GoToSoci()
        {
            _isClosing = true; // Impedisce ulteriori interazioni durante la navigazione
            _menuToSoci.OnNext(Unit.Default);
            _menuToSoci.OnCompleted();

            await Task.CompletedTask;

            //var sociVm = Locator.Current.GetService<ISociViewModel>();
            //if (sociVm != null)
            //{
            //    // 2. Impostiamo l'host (lo screen principale)
            //    try
            //    {
            //        // 3. Eseguiamo la navigazione FORZANDOLA sul Main Thread della UI
            //        await _host.Router.NavigateAndReset.Execute(sociVm);
            //    }
            //    catch (Exception ex)
            //    {
            //        _isClosing = false;
            //        Debug.WriteLine($"ERRORE durante la navigazione alla Soci: {ex.Message}");
            //    }
            //}
            //else
            //{
            //    _isClosing = false; // Permette all'utente di riprovare se il DI fallisce
            //    Debug.WriteLine("ERRORE CRITICO: ISociViewModel non è stato risolto dal Locator.");
            //}
        }

        private async Task ExecuteOpenGiornata()
        {
            // Utilizzo del Task.Run combinato con il Token ereditato per preservare la reattività della UI
            bool result = await Task.Run(() => Q.OpenGiornata(Token), Token);
            if (result)
            {
                ApriGiornataEnabled = false;
            }
        }
    }

    public partial class MenuViewModel
    {
        // ---------------------------------------------------------------------
        // 1. Visibilità dei Moduli e Permessi del Menu
        // ---------------------------------------------------------------------
        #region Visibility Properties

        private List<bool> _visibile = [];
        public List<bool> Visibile
        {
            get => _visibile;
            set => this.RaiseAndSetIfChanged(ref _visibile, value);
        }

        private bool _myamministratorevisible;
        public bool AmministratoreVisible
        {
            get => _myamministratorevisible;
            set => this.RaiseAndSetIfChanged(ref _myamministratorevisible, value);
        }

        private bool _myreportvisible;
        public bool ReportVisible
        {
            get => _myreportvisible;
            set => this.RaiseAndSetIfChanged(ref _myreportvisible, value);
        }

        private bool _mycassavisible;
        public bool CassaVisible
        {
            get => _mycassavisible;
            set => this.RaiseAndSetIfChanged(ref _mycassavisible, value);
        }

        private bool _mybarvisible;
        public bool BarVisible
        {
            get => _mybarvisible;
            set => this.RaiseAndSetIfChanged(ref _mybarvisible, value);
        }

        private bool _myguardarobavisible;
        public bool GuardarobaVisible
        {
            get => _myguardarobavisible;
            set => this.RaiseAndSetIfChanged(ref _myguardarobavisible, value);
        }

        private bool _mypulizievisible;
        public bool PulizieVisible
        {
            get => _mypulizievisible;
            set => this.RaiseAndSetIfChanged(ref _mypulizievisible, value);
        }

        #endregion

        // ---------------------------------------------------------------------
        // 2. Dati Operatore e Postazioni (Cassa)
        // ---------------------------------------------------------------------
        #region Operator and Workstation Data

        private string _myoperatorename = string.Empty;
        public string OperatoreName
        {
            get => _myoperatorename;
            set => this.RaiseAndSetIfChanged(ref _myoperatorename, value);
        }

        private List<MenuPostazioneMap> _mycassapostazionidatasource = null;
        public List<MenuPostazioneMap> CassaPostazioniDataSource
        {
            get => _mycassapostazionidatasource;
            set => this.RaiseAndSetIfChanged(ref _mycassapostazionidatasource, value);
        }

        private MenuPostazioneMap _selectedPostazione;
        public MenuPostazioneMap SelectedPostazione
        {
            get => _selectedPostazione;
            set => this.RaiseAndSetIfChanged(ref _selectedPostazione, value);
        }

        #endregion

        // ---------------------------------------------------------------------
        // 3. Gestione Stato Sessione Contabile (Giornata / Postazione)
        // ---------------------------------------------------------------------
        #region Accounting Session Properties

        // Definizioni degli OAPH (saranno valorizzati tramite .ToProperty() nel costruttore)
        private readonly ObservableAsPropertyHelper<string> _sessioneContabile;
        public string SessioneContabile => _sessioneContabile.Value;

        private readonly ObservableAsPropertyHelper<bool> _chiudiGiornataEnabled;
        public bool ChiudiGiornataEnabled => _chiudiGiornataEnabled.Value;

        private bool _apriGiornataEnabled;
        public bool ApriGiornataEnabled
        {
            get => _apriGiornataEnabled;
            set => this.RaiseAndSetIfChanged(ref _apriGiornataEnabled, value);
        }

        private bool _myapripostazioneenabled = false;
        public bool ApriPostazioneEnabled
        {
            get => _myapripostazioneenabled;
            set => this.RaiseAndSetIfChanged(ref _myapripostazioneenabled, value);
        }

        #endregion

        // ---------------------------------------------------------------------
        // 4. Stato Generale del Menu
        // ---------------------------------------------------------------------
        #region General UI State

        private bool _isMenuReady = false;
        public bool IsMenuReady
        {
            get => _isMenuReady;
            set => this.RaiseAndSetIfChanged(ref _isMenuReady, value);
        }

        #endregion
    }


}
