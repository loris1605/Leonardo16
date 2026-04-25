using Avalonia.Controls;
using ReactiveUI;
using ReactiveUI.Avalonia;
using System.Diagnostics;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;

namespace Views
{
    public abstract class BaseUserControl<TViewModel> : ReactiveUserControl<TViewModel>
                                                where TViewModel : class
    {
        protected abstract string RootControlName { get; }

        public BaseUserControl()
        {

#if DEBUG
            Debug.WriteLine($"***** [VIEW] {this.GetType().Name} {this.GetHashCode()} caricata *****");
#endif

            this.WhenActivated(d =>
            {
                Disposable.Create(() => CleanUpView()).DisposeWith(d);
            });

        }

        private void CleanUpView()
        {
            // Cerchiamo il controllo chiamato "RootGrid" dinamicamente
            var rootGrid = this.FindControl<Panel>(RootControlName);
            if (rootGrid != null)
            {
                rootGrid.Children.Clear();
                rootGrid.Children.Add(new Panel());
            }

            // Logica del GC come nel tuo esempio
            Task.Run(async () =>
            {
                await Task.Delay(500);
#if DEBUG
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                Debug.WriteLine($"***** [VIEW] {this.GetType().Name} {this.GetHashCode()} deattivata, DataContext rimosso. *****");
                
#endif
            });
        }


#if DEBUG
        // Questo viene chiamato solo quando l'oggetto viene rimosso dalla RAM
        ~BaseUserControl()
        {

            Debug.WriteLine($">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> [GC SUCCESS] {this.GetType().Name} {this.GetHashCode()} DISTRUTTA");

        }
#endif
    }

}
