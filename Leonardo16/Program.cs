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
            RegisterRepositories(services);
            RegisterViewModels(services);
            RegisterViews(services);
            
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

            //container.UseMicrosoftDependencyResolver();

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
            services.AddTransient<IOperatoreDbContext, OperatoreDbContext>();
            services.AddTransient<IPostazioneDbContext, PostazioneDbContext>();
            services.AddTransient<ISettoreDbContext, SettoreDbContext>();
            services.AddTransient<ITariffaDbContext, TariffaDbContext>();
            services.AddTransient<IPermessoDbContext, PermessoDbContext>();
            services.AddTransient<IRepartoDbContext, RepartoDbContext>();
            services.AddTransient<IListinoDbContext, ListinoDbContext>();

        }

        private static void RegisterRepositories(IServiceCollection services)
        {
            // Repository
            services.AddTransient<ISettingRepository, SettingRepository>();
            services.AddTransient<ILoginRepository, LoginRepository>();
            services.AddTransient<IMenuRepository, MenuRepository>();
            services.AddTransient<IOperatoreRepository, OperatoreRepository>();
            services.AddTransient<IPostazioneRepository, PostazioneRepository>();
            services.AddTransient<ISettoreRepository, SettoreRepository >();
            services.AddTransient<ITariffaRepository, TariffaRepository>();
            services.AddTransient<IPermessoRepository, PermessoRepository>();
            services.AddTransient<IRepartoRepository, RepartoRepository>();
            services.AddTransient<IListinoRepository, ListinoRepository>();
        }

        private static void RegisterViewModels(IServiceCollection services)
        {
            
            // ViewModels
            services.AddTransient<IMainWindowViewModel, MainWindowViewModel>();

            services.AddTransient<IConnectionViewModel, ConnectionViewModel>();
            services.AddTransient<ILoginViewModel, LoginViewModel>();
            services.AddTransient<IMenuViewModel, MenuViewModel>();

            services.AddTransient<ConfigurazioneViewModel>();

            services.AddTransient<IOperatoreGroupViewModel, OperatoreGroupViewModel>();
            services.AddTransient<IOperatoreAddViewModel, OperatoreAddViewModel>();
            services.AddTransient<IOperatoreDelViewModel, OperatoreDelViewModel>();
            services.AddTransient<IOperatoreUpdViewModel, OperatoreUpdViewModel>();
            services.AddTransient<IPermessoViewModel, PermessiViewModel>();

            services.AddTransient<IPostazioneGroupViewModel, PostazioneGroupViewModel>();
            services.AddTransient<IPostazioneAddViewModel, PostazioneAddViewModel>();
            services.AddTransient<IPostazioneDelViewModel, PostazioneDelViewModel>();
            services.AddTransient<IPostazioneUpdViewModel, PostazioneUpdViewModel>();
            services.AddTransient<IRepartoViewModel, RepartiViewModel>();

            services.AddTransient<ISettoreGroupViewModel, SettoreGroupViewModel>();
            services.AddTransient<ISettoreAddViewModel, SettoreAddViewModel>();
            services.AddTransient<ISettoreDelViewModel, SettoreDelViewModel>();
            services.AddTransient<ISettoreUpdViewModel, SettoreUpdViewModel>();
            services.AddTransient<IListinoViewModel, ListinoViewModel>();   

            services.AddTransient<ITariffaGroupViewModel, TariffaGroupViewModel>();
            services.AddTransient<ITariffaAddViewModel, TariffaAddViewModel>();
            services.AddTransient<ITariffaDelViewModel, TariffaDelViewModel>();
            services.AddTransient<ITariffaUpdViewModel, TariffaUpdViewModel>();

            services.AddTransient<IConfigurazioneViewModel, ConfigurazioneViewModel>();
            
        }

        private static void RegisterIViewFor(IServiceCollection services)
        {
            
            
            // Views (Necessarie per il Routing di ReactiveUI)

            services.AddTransient<IViewFor<MainWindowViewModel>, MainWindow>();
            services.AddTransient<IViewFor<LoginViewModel>, LoginView>();
            services.AddTransient<IViewFor<ConnectionViewModel>, ConnectionView>();

            services.AddTransient<IViewFor<MenuViewModel>, MenuView>();

            services.AddTransient<IViewFor<ConfigurazioneViewModel>, ConfigurazioneView>();
            services.AddTransient<IViewFor<OperatoreGroupViewModel>, OperatoreGroupView>();
            services.AddTransient<IViewFor<OperatoreAddViewModel>, OperatoreInputView>();
            services.AddTransient<IViewFor<OperatoreDelViewModel>, OperatoreInputView>();
            services.AddTransient<IViewFor<OperatoreUpdViewModel>, OperatoreInputView>();
            services.AddTransient<IViewFor<PermessiViewModel>, PermessiView>();
            services.AddTransient<IViewFor<PostazioneGroupViewModel>, PostazioneGroupView>();
            services.AddTransient<IViewFor<PostazioneAddViewModel>, PostazioneInputView>();
            services.AddTransient<IViewFor<PostazioneDelViewModel>, PostazioneInputView>();
            services.AddTransient<IViewFor<PostazioneUpdViewModel>, PostazioneInputView>();
            services.AddTransient<IViewFor<RepartiViewModel>, RepartiView>();
            services.AddTransient<IViewFor<SettoreGroupViewModel>, SettoreGroupView>();
            services.AddTransient<IViewFor<SettoreAddViewModel>, SettoreInputView>();
            services.AddTransient<IViewFor<SettoreDelViewModel>, SettoreInputView>();
            services.AddTransient<IViewFor<SettoreUpdViewModel>, SettoreInputView>();
            services.AddTransient<IViewFor<TariffaGroupViewModel>, TariffaGroupView>();
            services.AddTransient<IViewFor<TariffaAddViewModel>, TariffaInputView>();
            services.AddTransient<IViewFor<TariffaDelViewModel>, TariffaInputView>();
            services.AddTransient<IViewFor<TariffaUpdViewModel>, TariffaInputView>();
        }

        private static void RegisterViews(IServiceCollection services)
        {
            services.AddTransient<MainWindow>();
            services.AddSingleton<IScreen>(sp => sp.GetRequiredService<MainWindowViewModel>());
            
        }

        

    }
}
