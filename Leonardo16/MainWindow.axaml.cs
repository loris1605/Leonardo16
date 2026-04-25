using ReactiveUI;
using ReactiveUI.Avalonia;
using System;
using System.Threading.Tasks;
using ViewModels;

namespace Leonardo16
{
    public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
    {
        public MainWindow()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
                // Monitoriamo quando il Router cambia vista
                if (ViewModel == null) return;
                ViewModel.Router.NavigationStack.CollectionChanged += (s, e) =>
                {
                    // Ogni volta che navighiamo, forziamo una pulizia dopo la transizione
                    Task.Delay(1000).ContinueWith(_ =>
                    {
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                        GC.Collect();
                        System.Diagnostics.Debug.WriteLine(">>> [MAIN] GC Forzato dopo cambio vista.");
                    });
                };
            });
        }
    }
}