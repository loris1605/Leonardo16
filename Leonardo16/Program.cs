using Avalonia;
using Common.InterViewModels;
using DTO.Repository;
using Leonardo16.Core.Context;
using Leonardo16.Core.Repository;
using Microsoft.Extensions.DependencyInjection;
using Models.Context;
using ReactiveUI;
using ReactiveUI.Avalonia;
using Splat;
using Splat.Microsoft.Extensions.DependencyInjection;
using System;
using ViewModels;
using Views;


namespace Leonardo16
{
    internal class Program
    {
        [STAThread]
        public static void Main(string[] args) => BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);

        public static AppBuilder BuildAvaloniaApp()
        {
            var services = new ServiceCollection();

            // 1. Registra le tue cose (Repository, VM, View)
            RegisterContexts(services);
            RegisterServices(services);
            RegisterViews(services);
            RegisterViewModels(services);
            RegisterIViewFor(services);

            // 2. PREPARA il resolver di Splat PRIMA di buildare
            var resolver = new MicrosoftDependencyResolver(services);

            // 3. Collega Splat al resolver (senza inizializzazioni extra che scrivono nel provider)
            Locator.SetLocator(resolver);

            var appBuilder = AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace()
            .UseReactiveUI(_ => { });

            // 4. Ora puoi buildare il provider di Microsoft
            var container = services.BuildServiceProvider();

            // 5. Opzionale: passa il container finale al resolver se necessario
            //resolver.UpdateContainer(container);

            return appBuilder;
        }

        private static void RegisterContexts(IServiceCollection services)
        {
            services.AddTransient<AppDbContext>();
            services.AddTransient<ISettingDbContext, SettingDbContext>();
            services.AddTransient<ILoginDbContext, LoginDbContext>();
            services.AddTransient<IMenuDbContext, MenuDbContext>();

        }

        private static void RegisterServices(IServiceCollection services)
        {
            // Repository
            services.AddTransient<ISettingRepository, SettingRepository>();
            services.AddTransient<ILoginRepository, LoginRepository>();
            services.AddTransient<IMenuRepository, MenuRepository>();
            //services.AddTransient<IPersonRepository, PersonRepository>();
            //services.AddTransient<IOperatoreRepository, OperatoreRepository>();
            //services.AddTransient<IPostazioneRepository, PostazioneRepository>();
            //services.AddTransient<ISettoreRepository, SettoreRepository>();
            //services.AddTransient<ITariffaRepository, TariffaRepository>();
        }

        private static void RegisterViewModels(IServiceCollection services)
        {
            // ViewModels
            services.AddSingleton<MainWindowViewModel>();
            services.AddTransient<IConnectionViewModel, ConnectionViewModel>();
            services.AddTransient<ILoginViewModel, LoginViewModel>();
            services.AddTransient<IMenuViewModel, MenuViewModel>();
            services.AddTransient<IConfigurazioneViewModel, ConfigurazioneViewModel>();
            services.AddTransient<IGroupScreen>(sp => sp.GetRequiredService<IConfigurazioneViewModel>() as IGroupScreen);
        }

        private static void RegisterIViewFor(IServiceCollection services)
        {
            services.AddSingleton<IScreen>(sp => sp.GetRequiredService<MainWindowViewModel>());
            // Views (Necessarie per il Routing di ReactiveUI)

            services.AddTransient<IViewFor<MainWindowViewModel>, MainWindow>();
            services.AddTransient<IViewFor<LoginViewModel>, LoginView>();
            services.AddTransient<IViewFor<ConnectionViewModel>, ConnectionView>();

            services.AddTransient<IViewFor<MenuViewModel>, MenuView>();
            services.AddTransient<IViewFor<ConfigurazioneViewModel>, ConfigurazioneView>();

            RegisterIViewForSoci(services);
            RegisterIViewForConfigurazione(services);
            RegisterIViewForCassa(services);

        }

        private static void RegisterViews(IServiceCollection services)
        {
            services.AddTransient<MainWindow>();
        }

        private static void RegisterIViewForSoci(IServiceCollection services)
        {
            //services.AddTransient<IViewFor<SociViewModel>, SociView>();

            //services.AddTransient<IViewFor<PersonGroupViewModel>, PersonGroupView>();

            //services.AddTransient<IViewFor<PersonAddViewModel>, PersonInputView>();
            //services.AddTransient<IViewFor<PersonUpdViewModel>, PersonInputView>();
            //services.AddTransient<IViewFor<PersonDelViewModel>, PersonInputView>();
            //services.AddTransient<IViewFor<PersonSearchViewModel>, PersonSearchView>();

            //services.AddTransient<IViewFor<CodiceSocioAddViewModel>, SocioInputView>();
            //services.AddTransient<IViewFor<CodiceSocioDelViewModel>, SocioInputView>();
            //services.AddTransient<IViewFor<CodiceSocioUpdViewModel>, SocioInputView>();

            //services.AddTransient<IViewFor<TesseraAddViewModel>, TesseraInputView>();
            //services.AddTransient<IViewFor<TesseraDelViewModel>, TesseraInputView>();
            //services.AddTransient<IViewFor<TesseraUpdViewModel>, TesseraInputView>();
        }

        private static void RegisterIViewForConfigurazione(IServiceCollection services)
        {
            //services.AddTransient<IViewFor<ConfigurazioneViewModel>, ConfigurazioneView>();

            //services.AddTransient<IViewFor<OperatoreGroupViewModel>, OperatoreGroupView>();
            //services.AddTransient<IViewFor<OperatoreAddViewModel>, OperatoreInputView>();
            //services.AddTransient<IViewFor<OperatoreUpdViewModel>, OperatoreInputView>();
            //services.AddTransient<IViewFor<OperatoreDelViewModel>, OperatoreInputView>();

            //services.AddTransient<IViewFor<PermessiViewModel>, PermessiView>();

            //services.AddTransient<IViewFor<PostazioneGroupViewModel>, PostazioneGroupView>();
            //services.AddTransient<IViewFor<PostazioneAddViewModel>, PostazioneInputView>();
            //services.AddTransient<IViewFor<PostazioneUpdViewModel>, PostazioneInputView>();
            //services.AddTransient<IViewFor<PostazioneDelViewModel>, PostazioneInputView>();

            //services.AddTransient<IViewFor<RepartiViewModel>, RepartiView>();

            //services.AddTransient<IViewFor<SettoreGroupViewModel>, SettoreGroupView>();
            //services.AddTransient<IViewFor<SettoreAddViewModel>, SettoreInputView>();
            //services.AddTransient<IViewFor<SettoreUpdViewModel>, SettoreInputView>();
            //services.AddTransient<IViewFor<SettoreDelViewModel>, SettoreInputView>();

            //services.AddTransient<IViewFor<ListiniViewModel>, ListiniView>();

            //services.AddTransient<IViewFor<TariffaGroupViewModel>, TariffaGroupView>();
            //services.AddTransient<IViewFor<TariffaAddViewModel>, TariffaInputView>();
            //services.AddTransient<IViewFor<TariffaUpdViewModel>, TariffaInputView>();
            //services.AddTransient<IViewFor<TariffaDelViewModel>, TariffaInputView>();
        }

        private static void RegisterIViewForCassa(IServiceCollection services)
        {
            //services.AddTransient<IViewFor<CassaViewModel>, CassaView>();
            //services.AddTransient<IViewFor<CassaPostazioneViewModel>, CassaPostazioneView>();
        }


        private static void RegisterMappers()
        {

        }

        //private static void LoadModules(IServiceCollection services)
        //{
        //    // 1. Definiamo dove sono i moduli (es. sottocartella "Modules")
        //    string modulesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Modules");

        //    if (!Directory.Exists(modulesPath))
        //        Directory.CreateDirectory(modulesPath);

        //    // 2. Cerchiamo tutte le DLL
        //    var files = Directory.GetFiles(modulesPath, "*.dll");

        //    foreach (var file in files)
        //    {
        //        try
        //        {
        //            var assembly = Assembly.LoadFrom(file);

        //            // 3. Cerchiamo la classe che implementa IAppModule (dal tuo progetto Core)
        //            var moduleType = assembly.GetTypes()
        //                .FirstOrDefault(t => typeof(IAppModule).IsAssignableFrom(t) && !t.IsInterface);

        //            if (moduleType != null)
        //            {
        //                var module = (IAppModule)Activator.CreateInstance(moduleType)!;
        //                // 4. Eseguiamo la registrazione dinamica!
        //                module.Register(services);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Debug.WriteLine($"Errore caricamento modulo {file}: {ex.Message}");
        //        }
        //    }
        //}



    }
}
