using Avalonia.Collections;
using Common.InterViewModels;
using DTO.Entity;
using DTO.Repository;
using ReactiveUI;
using Splat;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using ViewModels.BindableObjects;

namespace ViewModels
{
        

    public class OperatoreGroupViewModel : GroupViewModelBase<OperatoreMap>, IGroupViewModelBase, IOperatoreGroupViewModel
    {
        public ReactiveCommand<Unit, Unit> PostazioniCommand { get; protected set; }
        public ReactiveCommand<Unit, Unit> SettoriCommand { get; protected set; }
        public ReactiveCommand<Unit, Unit> TariffeCommand { get; protected set; }
        public ReactiveCommand<Unit, Unit> PermessiCommand { get; protected set; }

        private IOperatoreRepository Q;
        
        protected IConfigurazioneScreen _host;

        protected override IObservable<bool> canDel => this.WhenAnyValue(
            x => x.GroupBindingT,
            (item) => item != null && item.CodicePermesso == 0);

        protected override IObservable<bool> IsAnythingExecuting =>
            new[]
            {
                base.IsAnythingExecuting,
                PostazioniCommand?.IsExecuting ?? Observable.Return(false),
                SettoriCommand?.IsExecuting ?? Observable.Return(false),
                PermessiCommand?.IsExecuting ?? Observable.Return(false),
                TariffeCommand?.IsExecuting ?? Observable.Return(false)
            }.CombineLatest(values => values.Any(x => x));

        public OperatoreGroupViewModel(IOperatoreRepository Repository) : base(null)
        {
            Q = Repository ?? throw new ArgumentNullException(nameof(Repository));

            var canHasSelection = this.WhenAnyValue(x => x.GroupBindingT).Select(item => item != null);


            // Navigazioni Semplici (NavigateAndReset)
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

            TariffeCommand = ReactiveCommand.CreateFromObservable(
            () =>
            {
                var setVm = Locator.Current.GetService<ITariffaGroupViewModel>();
                if (setVm != null)
                {
                    setVm.SetHost(_host);
                    return NavigateToReset(setVm);
                }
                else
                {
                    Debug.WriteLine("ERRORE CRITICO: ITariffaGroupViewModel non è stato risolto dal Locator.");
                    return Observable.Return(Unit.Default);
                }
            });

            //PermessiCommand = ReactiveCommand.CreateFromObservable(
            //    () => NavigateToInput(new PermessiViewModel(ConfigHost, GroupBindingT!.Id, 
            //    Locator.Current.GetService<IOperatoreRepository>())), canHasSelection);

            InitializeLoadingHelper();

            this.WhenActivated(d =>
            {
               
                PostazioniCommand?.DisposeWith(d);
                SettoriCommand?.DisposeWith(d);
                TariffeCommand?.DisposeWith(d);
                PermessiCommand?.DisposeWith(d);

            });
          

        }

        public void SetHost(IConfigurazioneScreen host) => _host = host;

        protected override void OnFinalDestruction()
        {
            // Assicuriamoci che la collezione sia nulla per il GC
            Q = null;
            PostazioniCommand = null;
            SettoriCommand = null;
            PermessiCommand = null;
            TariffeCommand = null;
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
                DataSource = new List<OperatoreMap>();
                GroupedDataSource = null;
            }
        }

        private async Task UpdateCollection(List<OperatoreDTO> data, int id)
        {
            var mapped = await Task.Run(() => data.Select(dto => new OperatoreMap(dto)).ToList(), token);
            var view = new DataGridCollectionView(mapped);
            view.GroupDescriptions.Add(new DataGridPathGroupDescription("Titolo"));

            // Sparisce IsLoading = true manuale
            var backup = GroupBindingT;
            GroupBindingT = null;
            GroupedDataSource = view;
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

        protected async override Task OnAdding() => await NavigateTo<IOperatoreAddViewModel>();

        protected async override Task OnDeleting() =>
                                    await NavigateTo<IOperatoreDelViewModel>(vm => vm.SetIdDaModificare(GroupBindingT.Id));

        protected async override Task OnUpdating() =>
                                    await NavigateTo<IOperatoreUpdViewModel>(vm => vm.SetIdDaModificare(GroupBindingT.Id));

        protected override Task OnEsc() => Task.FromResult(Unit.Default);
        
    }
}
