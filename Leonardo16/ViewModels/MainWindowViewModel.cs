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
        readonly int _currentVersion = 1; // Versione attuale del database, da aggiornare quando si modificano le entità

        // Iniettiamo le interfacce dei ViewModel per la navigazione
        
        public RoutingState Router { get; } = new RoutingState();
        public string UrlPathSegment => "main";
        public IScreen HostScreen => this;
        public ViewModelActivator Activator { get; } = new();

        public MainWindowViewModel()
        {
                  
            // Spostiamo la logica di navigazione all'attivazione
            Observable.StartAsync(InitializeNavigation, RxSchedulers.MainThreadScheduler);
        }

        private async Task InitializeNavigation()
        {
            await Task.Run(() => SysNet.Connection.TestConnection());

            if (Flags.ServerAttivo)
            {
                var r = Locator.Current.GetService<ISettingRepository>();
                // 2. Controllo versione tramite il repository iniettato
                if (!await r.CheckAppVersion(_currentVersion))
                {
                    await VerificaNecessitaAggiornamento();
                }
                // 3. Navigazione al VM iniettato
                await Router.NavigateAndReset.Execute(Locator.Current.GetService<ILoginViewModel>());
            }
            else
            {
                await Router.NavigateAndReset.Execute(Locator.Current.GetService<IConnectionViewModel>());
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
