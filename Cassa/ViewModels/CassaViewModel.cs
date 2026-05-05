using Common.InterViewModels;
using ReactiveUI;
using Splat;
using System.Diagnostics;
using System.Reactive.Linq;
using ViewModels.BindableObjects;

namespace ViewModels
{
    

    public partial class CassaViewModel : BaseViewModel, ICassaScreen, ICassaViewModel
    {
        public RoutingState CassaRouter { get; } = new RoutingState();
        public RoutingState SettingsRouter { get; } = new RoutingState();
        public RoutingState Router => CassaRouter;


        private int _cassaPostazioneId;
        private IScreen _host;


        public CassaViewModel() : base(null)
        {
            
        }

        public void SetPostazioneId(int id)
        {
            _cassaPostazioneId = id;
        }

        public void SetHost(IScreen host) 
        {
            _host = host;
        }

        protected async override Task OnEsc()
        {
            await Task.CompletedTask;

        }

        protected override async Task OnLoading()
        {
            await Task.CompletedTask;
            var cassaPostazioneVm = Locator.Current.GetService<ICassaPostazioneViewModel>();
            if (cassaPostazioneVm is not null)
            {
                cassaPostazioneVm.SetHost(this);
                cassaPostazioneVm.SetPostazioneId(_cassaPostazioneId);
                await CassaRouter.NavigateAndReset.Execute(cassaPostazioneVm);
            }
            //await CassaRouter.NavigateAndReset.Execute(new CassaPostazioneViewModel(HostScreen, 
            //                                                                       _cassaPostazione));
        }

        public async Task OnClosing()
        {
            _isClosing = true;
            var menuVm = Locator.Current.GetService<IMenuViewModel>();
            if (menuVm != null)
            {
                CassaRouter.NavigationStack.Clear(); // Pulisce la navigazione attuale per evitare problemi di back navigation
                SettingsRouter.NavigationStack.Clear(); // Pulisce anche il router delle impostazioni se necessario

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

        protected override Task OnSaving()
        {
            throw new NotImplementedException();
        }
    }
}
