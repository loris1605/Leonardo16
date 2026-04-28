using Common.InterViewModels;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using Splat;
using System.Reactive.Linq;

namespace ViewModels
{
    
    public interface IConfigurazioneScreen : IScreen
    {
        RoutingState GroupRouter { get; }
        RoutingState InputRouter { get; }
        bool GroupEnabled { get; set; }

        void AggiornaGridByInt(int id);
    }

    public partial class ConfigurazioneViewModel : BaseViewModel, IConfigurazioneScreen, IConfigurazioneViewModel
    {
        public RoutingState GroupRouter { get; } = new RoutingState();
        public RoutingState InputRouter { get; } = new RoutingState();
        public RoutingState Router => GroupRouter;

        private readonly IServiceProvider _sp;

        public ConfigurazioneViewModel(IScreen host, IServiceProvider sp) : base(host)
        {
            _sp = sp;

            //_operatoreGroupViewModel = operatoreGroupViewModel;

            //this.WhenActivated(d =>
            //{

            //});

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
            var firstPage = ActivatorUtilities.CreateInstance<OperatoreGroupViewModel>(_sp, this);

            if (firstPage != null)
            {
                await GroupRouter.NavigateAndReset.Execute(firstPage);
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

        protected override Task OnSaving() => Task.CompletedTask;

        protected async override Task OnEsc()
        {
            _isClosing = true;
            try
            {
                await HostScreen.Router.NavigateBack.Execute();
                _isClosing = false;
            }
            catch (Exception)
            {
                _isClosing = false; // Riapri le interazioni se il cambio pagina fallisce
                System.Diagnostics.Debug.WriteLine("ERRORE: IMenuViewModel non è stato risolto dalla DI.");
            }
            
        }

    }

    public partial class ConfigurazioneViewModel
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
