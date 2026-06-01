using Common.InterViewModels;
using ReactiveUI;
using Splat;
using System.Diagnostics;
using System.Reactive.Linq;

namespace ViewModels
{
   
    public partial class SociViewModel : ViewModelBase, ISociScreen, ISociViewModel
    {
        // ---------------------------------------------------------------------
        // 1. Router Interni (Sub-Routing) e Dipendenze
        // ---------------------------------------------------------------------
        public RoutingState GroupRouter { get; } = new RoutingState();
        public RoutingState InputRouter { get; } = new RoutingState();

        // Espone il router principale richiesto dall'infrastruttura ReactiveUI
        public RoutingState Router => GroupRouter;

        private IScreen _host;
        public new IScreen HostScreen => _host;

        // ---------------------------------------------------------------------
        // 2. Controllo Esecuzione Centralizzato (Prevenzione Doppi Clic)
        // ---------------------------------------------------------------------
        protected override IObservable<bool> IsAnythingExecuting =>
            Observable.CombineLatest(
                // 1. Comandi base ereditati
                this.WhenAnyObservable(x => x.LoadCommand.IsExecuting).StartWith(false),
                this.WhenAnyObservable(x => x.SaveCommand.IsExecuting).StartWith(false),
                this.WhenAnyObservable(x => x.EscPressedCommand.IsExecuting).StartWith(false),
                // 2. Monitoraggio delle esecuzioni dei router (Navigazioni in corso)
                this.WhenAnyObservable(x => x.GroupRouter.NavigateAndReset.IsExecuting).StartWith(false),
                this.WhenAnyObservable(x => x.GroupRouter.Navigate.IsExecuting).StartWith(false),
                this.WhenAnyObservable(x => x.InputRouter.NavigateAndReset.IsExecuting).StartWith(false),
                // Se qualunque operazione o cambio pagina è attivo, blocca la UI
                (l, s, e, gReset, gNav, iReset) => l || s || e || gReset || gNav || iReset)
            .DistinctUntilChanged();

        // ---------------------------------------------------------------------
        // Constructor
        // ---------------------------------------------------------------------
        public SociViewModel(IScreen host) : base(null)
        {
            _host = host;            
        }

        // ---------------------------------------------------------------------
        // 3. Ciclo di Vita (Override dei Metodi Virtuali della Base)
        // ---------------------------------------------------------------------

        protected override void OnFinalDestruction()
        {
            // Svuotiamo gli stack di navigazione dei router interni per liberare le View collegate
            GroupRouter?.NavigationStack.Clear();
            InputRouter?.NavigationStack.Clear();
            _host = null;

            base.OnFinalDestruction();
        }

        protected override async Task OnLoading()
        {
            // Recuperiamo l'istanza della prima pagina (IPersonGroupViewModel)
            var firstPage = Locator.Current.GetService<IPersonGroupViewModel>();
            if (firstPage != null)
            {
                // Passiamo "this" come host, così il GroupViewModel saprà dove navigare
                firstPage.SetHost(this);
                try
                {
                    // Eseguiamo la navigazione iniziale sul Main Thread in modo nativo
                    await GroupRouter.NavigateAndReset.Execute(firstPage)
                        .ObserveOn(RxSchedulers.MainThreadScheduler);
                }
                catch (Exception ex)
                {
                    _isClosing = false;
                    Debug.WriteLine($"ERRORE durante la navigazione al PersonGroup: {ex.Message}");
                }
            }
        }
        protected override async Task OnSaving() => await Task.CompletedTask;
        protected override async Task OnEsc()
        {
            _isClosing = true;
            var menuVm = Locator.Current.GetService<IMenuViewModel>();
            if (menuVm != null)
            {
                try
                {
                    // Ritorno sicuro e asincrono al Menu principale resettando lo stack
                    await _host.Router.NavigateAndReset.Execute(menuVm)
                        .ObserveOn(RxSchedulers.MainThreadScheduler);
                }
                catch (Exception ex)
                {
                    _isClosing = false;
                    Debug.WriteLine($"ERRORE durante la navigazione al Menu: {ex.Message}");
                }
            }
            else
            {
                _isClosing = false; // Consente di riprovare se il DI fallisce
                Debug.WriteLine("ERRORE CRITICO: IMenuViewModel non è stato risolto dal Locator.");
            }
        }

        // ---------------------------------------------------------------------
        // 4. Metodi di Interfaccia e Sincronizzazione Griglie (ISociScreen)
        // ---------------------------------------------------------------------
        public void AggiornaGridByObject(object model)
        {
            if (GroupRouter.GetCurrentViewModel() is IGroupViewModelBase groupVm)
            {
                groupVm.CaricaByModel(model);
            }
        }

        public void AggiornaGridByInt(int id)
        {
            if (GroupRouter.GetCurrentViewModel() is IGroupViewModelBase groupVm)
            {
                // Passiamo l'ID al metodo di caricamento della lista
                groupVm.CaricaDataSource(id);
            }
        }

    }

    public partial class SociViewModel
    {
        #region GroupEnabled

        private bool _groupenabled = true;
        public bool GroupEnabled
        {
            get => _groupenabled;
            set => this.RaiseAndSetIfChanged(ref _groupenabled, value);
        }

        #endregion
    }
}
