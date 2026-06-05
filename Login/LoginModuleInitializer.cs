using Common.InterViewModels;
using DTO.Repository;
using Models.Context;
using ReactiveUI;
using Splat;
using ViewModels;
using Views;

namespace Login
{
    public static class LoginModuleInitializer
    {
        public static void Initialize()
        {
            // 1. REGISTRAZIONE COMPONENTI DATI (Interni alla DLL)
            // Registriamo il DbContext specifico del modulo
            Locator.CurrentMutable.Register(() => new LoginDbContext(), typeof(ILoginDbContext));

            // CORRETTO: Spostiamo il GetService dentro l'ambito della Lambda () => ...
            // In questo modo, il DbContext verrà cercato solo quando verrà creato il Repository
            Locator.CurrentMutable.Register(() =>
            {
                var context = Locator.Current.GetService<ILoginDbContext>();
                return new LoginRepository(context);
            }, typeof(ILoginRepository));

            // 2. REGISTRAZIONE COMPONENTI UI (Modello B - Usa e Getta)
            // CORRETTO: Spostiamo i resolver dentro la Lambda. 
            // Il ViewModel nascerà solo quando il costruttore verrà invocato dal thread UI di MainWindow
            Locator.CurrentMutable.Register(() =>
            {
                var screen = Locator.Current.GetService<IScreen>();
                var repository = Locator.Current.GetService<ILoginRepository>();
                return new LoginViewModel(screen, repository);
            }, typeof(ILoginViewModel));

            // Registriamo la View associata all'interfaccia e alla classe concreta per il Router
            Locator.CurrentMutable.Register(() => new LoginView(), typeof(IViewFor<ILoginViewModel>));
            Locator.CurrentMutable.Register(() => new LoginView(), typeof(IViewFor<LoginViewModel>));

            System.Diagnostics.Debug.WriteLine("***** [DLL-INIT] Login Registrazioni Splat completate in modalità Lazy *****");
        }
    }
}
