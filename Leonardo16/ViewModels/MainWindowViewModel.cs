using Common.InterViewModels;
using Leonardo16.Core.Repository;
using Microsoft.EntityFrameworkCore;
using Models.Context;
using ReactiveUI;
using Splat;
using SysNet;
using System;
using System.Diagnostics;
using System.Linq;
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
                    InitializeNavigation().ConfigureAwait(false);
                }
            });
        }

        private async Task InitializeNavigation()
        {
            try
            {
                // 1. Test connessione in background
                await Task.Run(() => SysNet.Connection.TestConnection());

                //await VerificaNecessitaAggiornamento();

                if (Flags.ServerAttivo)
                {
                    // 2. Controllo versione ed eventuali migrazioni
                    if (!await _settingRepository.CheckAppVersion(_currentVersion))
                    {
                        await VerificaNecessitaAggiornamento();
                    }

                    // 3. Risoluzione ViewModel e navigazione sul Main Thread
                    var loginVM = Locator.Current.GetService<ILoginViewModel>();

                    try
                    {
                        // Navigazione nativa e pulita con reset dello stack, forzata sul thread UI principale
                        await Router.NavigateAndReset.Execute(loginVM);

                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"ERRORE durante la navigazione al Login: {ex.Message}");
                    }

                }
                else
                {
                    // 4. Gestione caso Server spento / Errore connessione
                    var connectionVM = Locator.Current.GetService<IConnectionViewModel>();
                    if (connectionVM != null) await Router.NavigateAndReset.Execute(connectionVM);
                   
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
}
