using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Common.InterViewModels;
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
                // 1. Risolvi la MainWindow dal container (che a sua volta risolverà le sue dipendenze)
                var mainWindow = Locator.Current.GetService<MainWindow>()
                                ?? throw new InvalidOperationException("Impossibile risolvere MainWindow.");

                var vm = Locator.Current.GetService<IMainWindowViewModel>() 
                                        ?? throw new InvalidOperationException("Non risolta classe MainViewmodel");

                if (vm is MainWindowViewModel concreteVm)
                {
                    mainWindow.ViewModel = concreteVm;
                }
                else
                {
                    throw new InvalidOperationException("Il ViewModel risolto non è di tipo MainWindowViewModel.");
                }

                desktop.MainWindow = mainWindow;
                
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}