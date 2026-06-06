using Common.InterViewModels;
using Leonardo16;
using Leonardo16.Core.Repository;
using Microsoft.EntityFrameworkCore;
using Models.Context;
using ReactiveUI;
using Splat;
using SysNet;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace ViewModels
{
    public partial class MainWindowViewModel : ReactiveObject, 
                                                   IScreen,
                                                   IRoutableViewModel,
                                                   IActivatableViewModel,
                                                   IMainWindowViewModel          

        {
        readonly int _currentVersion = 1; // Versione attuale del database, da aggiornare quando si modificano le entità
        private readonly ISettingRepository _settingRepository;
        private readonly IServiceProvider _sp;
        private bool _isInitialized;

        // Iniettiamo le interfacce dei ViewModel per la navigazione

        public RoutingState Router { get; } = new RoutingState();
            public string UrlPathSegment => "main";
            public IScreen HostScreen => this;
            public ViewModelActivator Activator { get; } = new();

            public MainWindowViewModel(ISettingRepository settingRepository, 
                                       IServiceProvider sp)
            {
                _settingRepository = settingRepository ?? throw new ArgumentNullException(nameof(settingRepository));
                _sp = sp ?? throw new ArgumentNullException(nameof(sp));

            // Spostiamo la logica di navigazione all'attivazione
            this.WhenActivated((CompositeDisposable disposables) =>
            {
                if (!_isInitialized)
                {
                    _isInitialized = true;
                    // Eseguiamo l'inizializzazione in modo che non blocchi l'attivazione della View
                    Task.Run(async () => await InitializeNavigation());
                }
            });
        }

        private async Task InitializeNavigation()
        {
            try
            {
                await Task.Run(() => SysNet.Connection.TestConnection());

                if (Flags.ServerAttivo)
                {
                    // 2. Controllo versione ed eventuali migrazioni
                    if (!await _settingRepository.CheckAppVersion(_currentVersion))
                    {
                        await VerificaNecessitaAggiornamento();
                    }

                    await GoToLogin();
                }
                else
                {
                    await Task.Run(() => ModuleLoader.EnsureConnectionModuleLoaded());

                    var tcs = new TaskCompletionSource();

                    // 3. Risoluzione ViewModel e navigazione sul Main Thread
                    RxSchedulers.MainThreadScheduler.Schedule(() =>
                    {
                        try
                        {
                            // Nascendo qui dentro, il costruttore del ConnectionViewModel 
                            // viene eseguito sul thread UI, azzerando l'errore Cross-Thread!
                            var connectionVM = Locator.Current.GetService<IConnectionViewModel>();

                            if (connectionVM != null)
                            {
                                // Eseguiamo la navigazione e segnaliamo il completamento del Task
                                Router.NavigateAndReset.Execute(connectionVM)
                                    .Subscribe(_ => tcs.SetResult(), ex => tcs.SetException(ex));
                            }
                            else
                            {
                                Debug.WriteLine(">>> [ERROR] Impossibile risolvere IConnectionViewModel.");
                                tcs.SetResult();
                            }
                        }
                        catch (Exception ex)
                        {
                            tcs.SetException(ex);
                        }

                    });

                    // Attendiamo che il thread della UI abbia finito l'operazione
                    await tcs.Task;

                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Errore critico durante InitializeNavigation: {ex.Message}");
                // Qui potresti navigare verso una ErrorView generica se necessario
            }
        }

        

        // Modificato in statico o assicurati che l'istanza di AppDbContext sia configurata correttamente
        private async Task VerificaNecessitaAggiornamento()
            {
                // Nota: AppDbContext andrebbe idealmente iniettato tramite una Factory (IDbContextFactory)
                // per evitare problemi di scope, specialmente in app desktop.
                using var ctx = new AppDbContext();
                if (await ctx.Database.CanConnectAsync())
                {
                    var pending = await ctx.Database.GetPendingMigrationsAsync();
                    if (pending.Any())
                    {
                        await ctx.Database.MigrateAsync();
                    }
                }
            }
     

        }

    public partial class MainWindowViewModel
    {
        private async Task GoToLogin()
        {
            await Task.Run(() => ModuleLoader.EnsureLoginModuleLoaded());

            var tcs = new TaskCompletionSource();

            // 3. Risoluzione ViewModel e navigazione sul Main Thread
            RxSchedulers.MainThreadScheduler.Schedule(() =>
            {
                try
                {
                    // Nascendo qui dentro, il costruttore del LoginViewModel 
                    // viene eseguito sul thread UI, azzerando l'errore Cross-Thread!
                    var loginVM = Locator.Current.GetService<ILoginViewModel>();

                    if (loginVM != null)
                    {
                        loginVM.LoginSuccesso
                            .Take(1) // Prendiamo solo il primo evento di successo
                            .ObserveOn(RxSchedulers.MainThreadScheduler)
                            .Subscribe(async _ =>
                            {
                                // Quando riceviamo il segnale di login riuscito, navighiamo al Menu
                                await GoToMenu();
                            });


                        // Eseguiamo la navigazione e segnaliamo il completamento del Task
                        Router.NavigateAndReset.Execute(loginVM)
                            .Subscribe(_ => tcs.SetResult(), ex => tcs.SetException(ex));
                    }
                    else
                    {
                        Debug.WriteLine(">>> [ERROR] Impossibile risolvere ILoginViewModel.");
                        tcs.SetResult();
                    }
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }

            });

            // Attendiamo che il thread della UI abbia finito l'operazione
            await tcs.Task;
        }

        private async Task GoToMenu()
        {
            await Task.Run(() => ModuleLoader.EnsureMenuModuleLoaded());

            var tcs = new TaskCompletionSource();

            try
            {
                // Nascendo qui dentro, il costruttore del MenuViewModel 
                // viene eseguito sul thread UI, azzerando l'errore Cross-Thread!
                var menuVM = Locator.Current.GetService<IMenuViewModel>();

                if (menuVM != null)
                {
                    menuVM.MenuToLogin
                        .Take(1) // Prendiamo solo il primo evento di successo
                        .ObserveOn(RxSchedulers.MainThreadScheduler)
                        .Subscribe(async _ =>
                        {
                            // Quando riceviamo il segnale di login riuscito, navighiamo al Menu
                            await GoToLogin();
                        });

                    menuVM.MenuToSoci
                        .Take(1) // Prendiamo solo il primo evento di successo
                        .ObserveOn(RxSchedulers.MainThreadScheduler)
                        .Subscribe(async _ =>
                        {
                            // Quando riceviamo il segnale di login riuscito, navighiamo al Menu
                            await GoToSoci();
                        });


                    // Eseguiamo la navigazione e segnaliamo il completamento del Task
                    Router.NavigateAndReset.Execute(menuVM)
                        .Subscribe(_ => tcs.SetResult(), ex => tcs.SetException(ex));
                }
                else
                {
                    Debug.WriteLine(">>> [ERROR] Impossibile risolvere IMenuViewModel.");
                    tcs.SetResult();
                }
            }
            catch (Exception ex)
            {
                tcs.SetException(ex);
            }

        }

        private async Task GoToSoci()
        {
            await Task.Run(() => ModuleLoader.EnsureSociModuleLoaded());

            var tcs = new TaskCompletionSource();

            // 3. Risoluzione ViewModel e navigazione sul Main Thread
            RxSchedulers.MainThreadScheduler.Schedule(() =>
            {
                try
                {
                    // Nascendo qui dentro, il costruttore del LoginViewModel 
                    // viene eseguito sul thread UI, azzerando l'errore Cross-Thread!
                    var sociVM = Locator.Current.GetService<ISociViewModel>();

                    if (sociVM != null)
                    {
                        sociVM.SociToMenu
                        .Take(1) // Prendiamo solo il primo evento di successo
                        .ObserveOn(RxSchedulers.MainThreadScheduler)
                        .Subscribe(async _ =>
                        {
                            // Quando riceviamo il segnale di login riuscito, navighiamo al Menu
                            await GoToMenu();
                        });


                        // Eseguiamo la navigazione e segnaliamo il completamento del Task
                        Router.NavigateAndReset.Execute(sociVM)
                            .Subscribe(_ => tcs.SetResult(), ex => tcs.SetException(ex));
                    }
                    else
                    {
                        Debug.WriteLine(">>> [ERROR] Impossibile risolvere ISociViewModel.");
                        tcs.SetResult();
                    }
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }

            });

            // Attendiamo che il thread della UI abbia finito l'operazione
            await tcs.Task;
        }
    }
}
