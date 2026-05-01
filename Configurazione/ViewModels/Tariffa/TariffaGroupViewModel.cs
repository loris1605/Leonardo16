using Common.InterViewModels;
using DTO.Entity;
using DTO.Repository;
using ReactiveUI;
using Splat;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using ViewModels.BindableObjects;

namespace ViewModels
{
    public class TariffaGroupViewModel : GroupViewModelBase<TariffaMap>, IGroupViewModelBase, ITariffaGroupViewModel
    {

        public ReactiveCommand<Unit, Unit> PostazioniCommand { get; protected set; }
        public ReactiveCommand<Unit, Unit> SettoriCommand { get; protected set; }
        public ReactiveCommand<Unit, Unit> OperatoriCommand { get; protected set; }
        public ReactiveCommand<Unit, Unit> ListiniCommand { get; protected set; }

        private ITariffaRepository Q;
        protected IConfigurazioneScreen _host;

        // Logica di cancellazione reattiva (IsLoading gestito dalla base)
        protected override IObservable<bool> canDel => this.WhenAnyValue(
            x => x.GroupBindingT,
            x => x.GroupBindingT.HasListino,
            (item, hasListino) => item != null && !hasListino);

        protected override IObservable<bool> IsAnythingExecuting =>
            new[]
            {
                base.IsAnythingExecuting,
                OperatoriCommand?.IsExecuting ?? Observable.Return(false),
                PostazioniCommand?.IsExecuting ?? Observable.Return(false),
                ListiniCommand?.IsExecuting ?? Observable.Return(false),
                SettoriCommand?.IsExecuting ?? Observable.Return(false)
            }.CombineLatest(values => values.Any(x => x));



        public TariffaGroupViewModel(ITariffaRepository Repository) : base(null)
        {
            Q = Repository ?? throw new ArgumentNullException(nameof(Repository));
            var canHasSelection = this.WhenAnyValue(x => x.GroupBindingT).Select(item => item != null);

            PostazioniCommand = ReactiveCommand.CreateFromObservable(
            () =>
            {
                var posVm = Locator.Current.GetService<IPostazioneGroupViewModel>();
                if (posVm != null)
                {
                    posVm.SetHost(_host);
                    return NavigateToReset(posVm);
                }
                else
                {
                    Debug.WriteLine("ERRORE CRITICO: IPostazioneGroupViewModel non è stato risolto dal Locator.");
                    return Observable.Return(Unit.Default);
                }
            });

            SettoriCommand = ReactiveCommand.CreateFromObservable(
            () =>
            {
                var setVm = Locator.Current.GetService<ISettoreGroupViewModel>();
                if (setVm != null)
                {
                    setVm.SetHost(_host);
                    return NavigateToReset(setVm);
                }
                else
                {
                    Debug.WriteLine("ERRORE CRITICO: ISettoreGroupViewModel non è stato risolto dal Locator.");
                    return Observable.Return(Unit.Default);
                }
            });

            OperatoriCommand = ReactiveCommand.CreateFromObservable(
            () =>
            {
                var opeVm = Locator.Current.GetService<IOperatoreGroupViewModel>();
                if (opeVm != null)
                {
                    opeVm.SetHost(_host);
                    return NavigateToReset(opeVm);
                }
                else
                {
                    Debug.WriteLine("ERRORE CRITICO: IOperatoreGroupViewModel non è stato risolto dal Locator.");
                    return Observable.Return(Unit.Default);
                }
            });

            InitializeLoadingHelper();

            this.WhenActivated(d =>
            {
                // Nota: Add/Upd/Del sono gestiti dal DisposeWith della classe base
                PostazioniCommand?.DisposeWith(d);
                SettoriCommand?.DisposeWith(d);
                OperatoriCommand?.DisposeWith(d);
            });
        }

        public void SetHost(IConfigurazioneScreen host) => _host = host;


        protected override void OnFinalDestruction()
        {
            // Assicuriamoci che la collezione sia nulla per il GC
            Q = null;
            OperatoriCommand = PostazioniCommand = ListiniCommand = SettoriCommand = null;
            base.OnFinalDestruction();
        }

        protected override async Task OnLoading()
        {
            var data = await Q.Load(0, token);
            if (data?.Count > 0)
            {
                await UpdateCollection(data, 0);
                GroupBindingT = DataSource.FirstOrDefault();
            }
            else
            {
                DataSource = new List<TariffaMap>();
                GroupedDataSource = null;
            }
        }

        private async Task UpdateCollection(List<TariffaDTO> data, int id)
        {
            var mapped = await Task.Run(() => data.Select(dto => new TariffaMap(dto)).ToList(), token);

            var backup = GroupBindingT;
            GroupBindingT = null;
            DataSource = mapped;
            GroupBindingT = backup;

            IdIndex = id;
            GroupFocus = true;
        }

        public async Task CaricaDataSource(int id = 0)
        {
            try
            {
                var data = await Q.Load(id, token);
                await UpdateCollection(data, id);
            }
            catch (OperationCanceledException) { }
        }

        protected IObservable<Unit> NavigateToReset(IRoutableViewModel vm)
        {
            if (_host == null) return Observable.Return(Unit.Default);

            _isClosing = true; // Impedisce la navigazione multipla

            return _host.GroupRouter.NavigateAndReset.Execute(vm).Select(_ => Unit.Default);
        }

        protected IObservable<Unit> NavigateToInput(IRoutableViewModel vm)
        {
            return Observable.Start(() => _host.GroupEnabled = false, RxSchedulers.MainThreadScheduler)
                .SelectMany(_ => _host.InputRouter.Navigate.Execute(vm))
                .Select(_ => Unit.Default);
        }

        protected async Task NavigateTo<T>(Action<T> configure = null) where T : class // Assumi che abbiano un'interfaccia base per SetHost
        {
            // 1. Blocchiamo la UI
            _isClosing = true;

            var viewModel = Locator.Current.GetService<T>();
            if (viewModel != null)
            {
                try
                {
                    // 2. Configurazione (SetHost e altro)
                    // Assicurati che le tue interfacce derivino da una base o usa dynamic
                    (viewModel as dynamic).SetHost(_host);
                    configure?.Invoke(viewModel);

                    // 3. Navigazione sul Main Thread
                    await Observable.Start(async () =>
                    {
                        _isClosing = false;
                        await NavigateToInput(viewModel as IRoutableViewModel);
                    }, RxSchedulers.MainThreadScheduler);
                }
                catch (Exception ex)
                {
                    _isClosing = false;
                    Debug.WriteLine($"ERRORE durante la navigazione a {typeof(T).Name}: {ex.Message}");
                }
            }
            else
            {
                _isClosing = false;
                Debug.WriteLine($"ERRORE CRITICO: {typeof(T).Name} non risolto.");
            }
        }

        protected async override Task OnAdding() => await NavigateTo<ITariffaAddViewModel>();

        protected async override Task OnDeleting() =>
                                    await NavigateTo<ITariffaDelViewModel>(vm => vm.SetIdDaModificare(GroupBindingT.Id));

        protected async override Task OnUpdating() =>
                                    await NavigateTo<ITariffaUpdViewModel>(vm => vm.SetIdDaModificare(GroupBindingT.Id));

        protected override async Task OnEsc() => await Task.CompletedTask;

    }
}
