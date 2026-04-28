using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Splat;
using System;
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
                var vm = Locator.Current.GetService<MainWindowViewModel>() 
                                ?? throw new InvalidOperationException("Impossibile risolvere MainWindowViewModel dal container.");
                desktop.MainWindow = new MainWindow
                {
                    ViewModel = vm // ReactiveUI usa ViewModel, Avalonia usa DataContext (ViewModel setta entrambi)
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}