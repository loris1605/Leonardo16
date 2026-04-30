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

namespace ViewModels
{
   
    public partial class MenuViewModel : BaseViewModel, IMenuViewModel
    {
        private IMenuRepository Q;
        private IScreen _host;

        public ReactiveCommand<string, Unit> NavigateCommand { get; }
        public ReactiveCommand<MenuPostazioneMap, Unit> SelezionaPostazioneCommand { get; }
        public ReactiveCommand<Unit, Unit> LogoutCommand { get; }
        public ReactiveCommand<Unit, Unit> ConnectionCommand { get; }
        public ReactiveCommand<Unit,Unit> ConfigurazioneCommand { get; }
        public ReactiveCommand<Unit, Unit> ApriGiornataCommand { get; }

        protected override IObservable<bool> IsAnythingExecuting =>
            new[]
            {
                base.IsAnythingExecuting,
                NavigateCommand?.IsExecuting ?? Observable.Return(false),
                SelezionaPostazioneCommand?.IsExecuting ?? Observable.Return(false),
                LogoutCommand?.IsExecuting ?? Observable.Return(false),
                ConnectionCommand?.IsExecuting ?? Observable.Return(false),
                ConfigurazioneCommand?.IsExecuting ?? Observable.Return(false),
                ApriGiornataCommand?.IsExecuting ?? Observable.Return(false)
            }.CombineLatest(values => values.Any(x => x));


        public MenuViewModel(IMenuRepository Repository) : base(null)
        {
            Q = Repository ?? throw new ArgumentNullException(nameof(Repository));

            _chiudiGiornataEnabled = this.WhenAnyValue(x => x.ApriGiornataEnabled)
                .Select(x => !x)
                .ToProperty(this, x => x.ChiudiGiornataEnabled);

            // 2. Gestisci la stringa della Sessione
            _sessioneContabile = this.WhenAnyValue(x => x.ApriGiornataEnabled)
                    .Select(v => $"Sessione Contabile {(v ? "Chiusa" : "Aperta")}")
                    .ToProperty(this, x => x.SessioneContabile);

            var canNavigate = this.WhenAnyValue(x => x.IsLoading, x => !x);

            NavigateCommand = ReactiveCommand.CreateFromTask<string>(ExecuteNavigation, canNavigate);

            SelezionaPostazioneCommand = ReactiveCommand.CreateFromTask<MenuPostazioneMap>(GoToCassa, canNavigate);

            LogoutCommand = ReactiveCommand.CreateFromTask(GoToLogin, canNavigate);

            ConnectionCommand = ReactiveCommand.CreateFromTask(GoToConnection, canNavigate);

            ConfigurazioneCommand = ReactiveCommand.CreateFromTask(GoToConfigurazione, canNavigate);

            var canApriFinal = this.WhenAnyValue(x => x.ApriGiornataEnabled, x => x.IsLoading,
                    (enabled, loading) => enabled && !loading);

            ApriGiornataCommand = ReactiveCommand.CreateFromTask(ExecuteOpenGiornata, canApriFinal);

            this.WhenActivated(d =>
            {
                LogoutCommand?.DisposeWith(d);
                SelezionaPostazioneCommand?.DisposeWith(d);
                NavigateCommand?.DisposeWith(d);
                ConfigurazioneCommand?.DisposeWith(d);
                ApriGiornataCommand?.DisposeWith(d);
                ConnectionCommand?.DisposeWith(d);
            });

        }

        public void SetHost(IScreen host)
        {
            _host = host;
        }


        protected override void OnFinalDestruction()
        {
            CassaPostazioniDataSource?.Clear(); // Svuota la lista
            Q = null;
            base.OnFinalDestruction();
        }

        protected override async Task OnLoading()
        {
            if (GlobalValuesC.MySetting == null) return;

            AttivaPermessi();

            var listaDto = await Q.CaricaPostazioniCassa(GlobalValuesC.MySetting.IDOPERATORE, token);

            CassaPostazioniDataSource = listaDto
                                        .Select(dto => new MenuPostazioneMap(dto))
                                        .ToList();

            ApriGiornataEnabled = !(await Q.EsisteGiornataAperta(token));

            if (GlobalValuesC.MySetting.POSTAZIONI?.Count == 0) ApriPostazioneEnabled = false;

        }

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

        private async Task ExecuteNavigation(string dest)
        {
            _isClosing = true;
            IRoutableViewModel page = dest switch
            {
                //"Connection" => new ConnectionViewModel(HostScreen),
                //"Soci" => new SociViewModel(HostScreen),
                "Configurazione" => Locator.Current.GetService<IConfigurazioneViewModel>(),
                _ => null
            };

            if (page != null)
                
                await HostScreen.Router.Navigate.Execute(page);
            else
                _isClosing = false; // Ripristino se la destinazione è nulla
        }

        private async Task GoToCassa(MenuPostazioneMap map)
        {
            _isClosing = true;
            await Task.CompletedTask;
            //await HostScreen.Router.NavigateAndReset.Execute(new CassaViewModel(HostScreen, map));
        }

        private async Task GoToLogin()
        {
            _isClosing = true; // Impedisce ulteriori interazioni durante la navigazione
            var loginVm = Locator.Current.GetService<ILoginViewModel>();
            if (loginVm != null)
            {
                // 2. Impostiamo l'host (lo screen principale)
                loginVm.SetHost(_host);
                try
                {
                    // 3. Eseguiamo la navigazione FORZANDOLA sul Main Thread della UI
                    await Observable.Start(async () =>
                    {
                        await _host.Router.NavigateAndReset.Execute(loginVm);
                    }, RxSchedulers.MainThreadScheduler);
                }
                catch (Exception ex)
                {
                    _isClosing = false;
                    Debug.WriteLine($"ERRORE durante la navigazione al Login: {ex.Message}");
                }
            }
            else
            {
                _isClosing = false; // Permette all'utente di riprovare se il DI fallisce
                Debug.WriteLine("ERRORE CRITICO: ILoginViewModel non è stato risolto dal Locator.");
            }
        }

        private async Task GoToConnection()
        {
            _isClosing = true; // Impedisce ulteriori interazioni durante la navigazione
            var connectionVm = Locator.Current.GetService<IConnectionViewModel>();
            if (connectionVm != null)
            {
                // 2. Impostiamo l'host (lo screen principale)
                connectionVm.SetHost(_host);
                try
                {
                    // 3. Eseguiamo la navigazione FORZANDOLA sul Main Thread della UI
                    await Observable.Start(async () =>
                    {
                        await _host.Router.NavigateAndReset.Execute(connectionVm);
                    }, RxSchedulers.MainThreadScheduler);
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
                    await Observable.Start(async () =>
                    {
                        await _host.Router.NavigateAndReset.Execute(configurazioneVm);
                    }, RxSchedulers.MainThreadScheduler);
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

        //protected override async Task OnSaving() => await Task.CompletedTask;

        private async Task ExecuteOpenGiornata()
        {
            // OnOpenGiornata dovrebbe essere asincrono nel repository
            bool result = await Task.Run(() => Q.OpenGiornata(token));
            if (result) ApriGiornataEnabled = false;
        }

        protected override async Task OnEsc() => await GoToLogin();

    }

    public partial class MenuViewModel
    {
        //Voci visibili nel Menu
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

        private List<MenuPostazioneMap> _mycassapostazionidatasource = null;
        public List<MenuPostazioneMap> CassaPostazioniDataSource
        {
            get => _mycassapostazionidatasource;
            set => this.RaiseAndSetIfChanged(ref _mycassapostazionidatasource, value);
        }

        private string _myoperatorename = string.Empty;
        public string OperatoreName
        {
            get => _myoperatorename;
            set => this.RaiseAndSetIfChanged(ref _myoperatorename, value);
        }

        readonly ObservableAsPropertyHelper<string> _sessioneContabile;
        public string SessioneContabile => _sessioneContabile.Value;

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

        readonly ObservableAsPropertyHelper<bool> _chiudiGiornataEnabled;
        public bool ChiudiGiornataEnabled => _chiudiGiornataEnabled.Value;

        private bool _isMenuReady = false;
        public bool IsMenuReady
        {
            get => _isMenuReady;
            set => this.RaiseAndSetIfChanged(ref _isMenuReady, value);
        }

        private MenuPostazioneMap _selectedPostazione;
        public MenuPostazioneMap SelectedPostazione
        {
            get => _selectedPostazione;
            set => this.RaiseAndSetIfChanged(ref _selectedPostazione, value);
        }

    }

    
}
