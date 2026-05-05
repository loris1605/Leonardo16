using Common.InterViewModels;
using ReactiveUI;
using Splat;
using System.Diagnostics;
using System.Reactive.Linq;

namespace ViewModels
{
   
    public partial class SociViewModel : BaseViewModel, ISociScreen, ISociViewModel
    {
        public RoutingState GroupRouter { get; } = new RoutingState();
        public RoutingState InputRouter { get; } = new RoutingState();
        public RoutingState Router => GroupRouter;

        private IScreen _host;
    
        public SociViewModel() : base(null)
        {
            
        }

        public void SetHost(IScreen host)
        {
            _host = host;
        }

        protected override void OnFinalDestruction()
        {
            GroupRouter.NavigationStack.Clear();
            InputRouter.NavigationStack.Clear();
        }

        protected override async Task OnLoading()
        {
            // Creiamo l'istanza della prima pagina (OperatoreGroupViewModel)
            // Passando "this" come host, così il GroupViewModel saprà dove navigare
            var firstPage = Locator.Current.GetService<IPersonGroupViewModel>();
            if (firstPage != null)
            {
                firstPage?.SetHost(this);
                try
                {
                    // 3. Eseguiamo la navigazione FORZANDOLA sul Main Thread della UI
                    await Observable.Start(async () =>
                    {
                        await GroupRouter.NavigateAndReset.Execute(firstPage);
                    }, RxSchedulers.MainThreadScheduler);
                }
                catch (Exception ex)
                {
                    _isClosing = false;
                    Debug.WriteLine($"ERRORE durante la navigazione al PersonGroup: {ex.Message}");
                }
            }
        }

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

                // Se hai un comando di ricarica nel GroupViewModel:
                // groupVm.LoadCommand.Execute().Subscribe();
            }
        }

        protected override async Task OnSaving() => await Task.CompletedTask;

        protected async override Task OnEsc()
        {
            _isClosing = true;
            var menuVm = Locator.Current.GetService<IMenuViewModel>();
            if (menuVm != null)
            {
                // 2. Impostiamo l'host (lo screen principale)
                menuVm.SetHost(_host);
                try
                {
                    // 3. Eseguiamo la navigazione FORZANDOLA sul Main Thread della UI
                    await Observable.Start(async () =>
                    {
                        await _host.Router.NavigateAndReset.Execute(menuVm);
                    }, RxSchedulers.MainThreadScheduler);
                }
                catch (Exception ex)
                {
                    _isClosing = false;
                    Debug.WriteLine($"ERRORE durante la navigazione al Menu: {ex.Message}");
                }
            }
            else
            {
                _isClosing = false; // Permette all'utente di riprovare se il DI fallisce
                Debug.WriteLine("ERRORE CRITICO: IMenuViewModel non è stato risolto dal Locator.");
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
