using Common.InterViewModels;
using DTO.Repository;
using Models.Context;
using ReactiveUI;
using Splat;
using ViewModels;
using Views;

namespace Menu
{
    public static class MenuModuleInitializer
    {
        public static void Initialize()
        {
            // 1. REGISTRAZIONE COMPONENTI DATI (Interni alla DLL)
            // Registriamo il DbContext specifico del modulo
            Locator.CurrentMutable.Register(() => new MenuDbContext(), typeof(IMenuDbContext));

            // CORRETTO: Spostiamo il GetService dentro l'ambito della Lambda () => ...
            // In questo modo, il DbContext verrà cercato solo quando verrà creato il Repository
            Locator.CurrentMutable.Register(() =>
            {
                var context = Locator.Current.GetService<IMenuDbContext>();
                return new MenuRepository();
            }, typeof(IMenuRepository));

            // 2. REGISTRAZIONE COMPONENTI UI (Modello B - Usa e Getta)
            // CORRETTO: Spostiamo i resolver dentro la Lambda. 
            // Il ViewModel nascerà solo quando il costruttore verrà invocato dal thread UI di MainWindow
            Locator.CurrentMutable.Register(() =>
            {
                var screen = Locator.Current.GetService<IScreen>();
                var repository = Locator.Current.GetService<IMenuRepository>();
                return new MenuViewModel(screen, repository);
            }, typeof(IMenuViewModel));

            // Registriamo la View associata all'interfaccia e alla classe concreta per il Router
            Locator.CurrentMutable.Register(() => new MenuView(), typeof(IViewFor<IMenuViewModel>));
            Locator.CurrentMutable.Register(() => new MenuView(), typeof(IViewFor<MenuViewModel>));

            System.Diagnostics.Debug.WriteLine("***** [DLL-INIT] Menu Registrazioni Splat completate in modalità Lazy *****");
        }
    }
}
