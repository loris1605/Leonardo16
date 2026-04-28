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

        }

        private static void RegisterRepositories(IServiceCollection services)
        {
            // Repository
            services.AddTransient<ISettingRepository, SettingRepository>();
            services.AddTransient<ILoginRepository, LoginRepository>();
            services.AddTransient<IMenuRepository, MenuRepository>();
            services.AddTransient<IOperatoreRepository, OperatoreRepository>();
            services.AddTransient<IPostazioneRepository, PostazioneRepository>();

        }

        private static void RegisterViewModels(IServiceCollection services)
        {
            // ViewModels
            services.AddSingleton<MainWindowViewModel>();
            services.AddTransient<IConnectionViewModel, ConnectionViewModel>();
            services.AddTransient<ILoginViewModel, LoginViewModel>();
            services.AddTransient<IMenuViewModel, MenuViewModel>();

            services.AddTransient<ConfigurazioneViewModel>();

            services.AddTransient<OperatoreGroupViewModel>();
            services.AddTransient<OperatoreAddViewModel>();
            services.AddTransient<OperatoreDelViewModel>();
            services.AddTransient<OperatoreUpdViewModel>();

            services.AddTransient<PostazioneGroupViewModel>();

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

        }

        private static void RegisterViews(IServiceCollection services)
        {
            services.AddTransient<MainWindow>();
            services.AddSingleton<IScreen>(sp => sp.GetRequiredService<MainWindowViewModel>());
            services.AddTransient<IConfigurazioneScreen>(sp => sp.GetRequiredService<ConfigurazioneViewModel>());
        }

        

    }
}
