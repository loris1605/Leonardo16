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
using System.Reactive.Concurrency;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace ViewModels
{
    public partial class MainWindowViewModel : ReactiveObject, 
                                               IScreen, 
                                               IRoutableViewModel,
                                               IActivatableViewModel
                                                
    {
        readonly int currentVersion = 1; // Versione attuale del database, da aggiornare quando si modificano le entità
        
        public RoutingState Router { get; } = new RoutingState();
        public string UrlPathSegment => "main";
        public IScreen HostScreen => this;

        public ViewModelActivator Activator { get; } = new();

        public MainWindowViewModel()
        {
            // Spostiamo la logica di navigazione all'attivazione
            this.WhenActivated(d =>
            {
                Observable.StartAsync(async () => await InitializeNavigation())
                .Subscribe()
                .DisposeWith(d);
            });
        }

        private async Task InitializeNavigation()
        {
            await Task.Run(() => SysNet.Connection.TestConnection());

            if (Flags.ServerAttivo)
            {
                var r = Locator.Current.GetService<ISettingRepository>() 
                    ?? throw new InvalidOperationException("ISettingRepository non è registrato.");

                if (!await r.CheckAppVersion(currentVersion))
                    await VerificaNecessitaAggiornamento();
                RxSchedulers.MainThreadScheduler.Schedule(() => GoToLogin());
            }
            else
                RxSchedulers.MainThreadScheduler.Schedule(() => GoToConnection());
        }

        // Modificato in statico o assicurati che l'istanza di AppDbContext sia configurata correttamente
        private async Task VerificaNecessitaAggiornamento()
        {
            using var _ctx = new AppDbContext();

            if (await _ctx.Database.CanConnectAsync())
            {
                var pendingMigrations = await _ctx.Database.GetPendingMigrationsAsync();
                if (pendingMigrations.Any())
                {
                    // Applica le migrazioni se necessario
                    await _ctx.Database.MigrateAsync();
                    Debug.WriteLine("✅ Database aggiornato con successo.");
                }
            }
        }

        private void GoToLogin()
        {
            var vm = Locator.Current.GetService<ILoginViewModel>();
            if (vm != null)
            {
                HostScreen.Router.NavigateAndReset.Execute(vm);
            }
        }

        private void GoToConnection()
        {
            var vm = Locator.Current.GetService<IConnectionViewModel>();
            if (vm != null)
            {
                HostScreen.Router.NavigateAndReset.Execute(vm);
            }
        }

        

    }
}
