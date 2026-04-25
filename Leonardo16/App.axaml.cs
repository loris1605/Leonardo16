using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Splat;
using ViewModels;

namespace Leonardo16
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                //var vm = Locator.Current.GetService<MainWindowViewModel>();

                //// 2. Risolvi la MainWindow dal Locator
                //var view = Locator.Current.GetService<MainWindow>();

                //if (view != null)
                //{
                //    view.DataContext = vm;
                //    desktop.MainWindow = view;
                //}
                //else
                //{
                //    // Fallback manuale SE la registrazione della View fallisce
                //    desktop.MainWindow = new MainWindow
                //    {
                //        DataContext = vm
                //    };
                //}
                var vm = Locator.Current.GetService<MainWindowViewModel>();
                desktop.MainWindow = new MainWindow { DataContext = vm };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}