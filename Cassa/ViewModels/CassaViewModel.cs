using Common.InterViewModels;
using ReactiveUI;
using ViewModels.BindableObjects;

namespace ViewModels
{
    

    public partial class CassaViewModel : BaseViewModel, ICassaScreen, ICassaViewModel
    {
        public RoutingState CassaRouter { get; } = new RoutingState();
        public RoutingState SettingsRouter { get; } = new RoutingState();
        public RoutingState Router => CassaRouter;


        private PostazioneMap _cassaPostazione = new();
        private IScreen _host;


        public CassaViewModel() : base(null)
        {
            
        }

        public void SetPostazioneId(int id)
        {
            _cassaPostazione.Id = id;
        }

        public void SetHost(IScreen host) 
        {
            _host = host;
        }

        protected override Task OnEsc()
        {
            throw new NotImplementedException();
        }

        protected override async Task OnLoading()
        {
            await Task.CompletedTask;
            //await CassaRouter.NavigateAndReset.Execute(new CassaPostazioneViewModel(HostScreen, 
            //                                                                       _cassaPostazione));
        }

        protected override Task OnSaving()
        {
            throw new NotImplementedException();
        }
    }
}
