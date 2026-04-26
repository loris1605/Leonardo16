using Avalonia.Collections;
using DTO.Entity;
using DTO.Repository;
using Models.Repository;
using ReactiveUI;
using Splat;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using ViewModels.BindableObjects;

namespace ViewModels
{
    public class OperatoreGroupViewModel : GroupViewModelBase<OperatoreMap>, IGroupViewModelBase
    {
        public ReactiveCommand<Unit, Unit> PostazioniCommand { get; protected set; }
        public ReactiveCommand<Unit, Unit> SettoriCommand { get; protected set; }
        public ReactiveCommand<Unit, Unit> TariffeCommand { get; protected set; }
        public ReactiveCommand<Unit, Unit> PermessiCommand { get; protected set; }

        private IOperatoreRepository Q;

        protected IGroupScreen ConfigHost => HostScreen as IGroupScreen;

        protected override IObservable<bool> canDel => this.WhenAnyValue(
            x => x.GroupBindingT,
            (item) => item != null && item.CodicePermesso == 0);

        public OperatoreGroupViewModel(IScreen host,
                                       IOperatoreRepository Repository) : base(host)
        {
            Q = Repository ?? throw new ArgumentNullException(nameof(Repository));

            var canHasSelection = this.WhenAnyValue(x => x.GroupBindingT).Select(item => item != null);

            //var canAction = this.WhenAnyValue(x => x.GroupBindingT, x => x.IsLoading,
            //(item, loading) => item != null && !loading);

            //var canDelete = this.WhenAnyValue(x => x.GroupBindingT, x => x.IsLoading,
            //    (item, loading) => item != null &&
            //                        item.CodicePermesso == 0 &&
            //                        item.Id != -1 &&
            //                        !loading);


            // Navigazioni Semplici (NavigateAndReset)
            //PostazioniCommand = ReactiveCommand.CreateFromObservable(
            //() => NavigateToReset(new PostazioneGroupViewModel(ConfigHost, Locator.Current.GetService<IPostazioneRepository>())));

            //SettoriCommand = ReactiveCommand.CreateFromObservable(
            //() => NavigateToReset(new SettoreGroupViewModel(ConfigHost, Locator.Current.GetService<ISettoreRepository>())));

            //TariffeCommand = ReactiveCommand.CreateFromObservable(
            //    () => NavigateToReset(new TariffaGroupViewModel(ConfigHost, Locator.Current.GetService<ITariffaRepository>())));

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

        protected override IObservable<bool> IsAnythingExecuting =>
        base.IsAnythingExecuting.CombineLatest(
            PostazioniCommand.IsExecuting,
            SettoriCommand.IsExecuting,
            PermessiCommand.IsExecuting,
            TariffeCommand.IsExecuting,
            // Aggiungi gli altri...
            (b, p, s, perm, t) => b || p || s || perm || t);

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
            if (ConfigHost == null) return Observable.Return(Unit.Default);

            _isClosing = true; // Impedisce la navigazione multipla

            return ConfigHost.GroupRouter.NavigateAndReset.Execute(vm).Select(_ => Unit.Default);
        }

        protected IObservable<Unit> NavigateToInput(IRoutableViewModel vm)
        {
            return Observable.Start(() => ConfigHost.GroupEnabled = false, RxSchedulers.MainThreadScheduler)
                .SelectMany(_ => ConfigHost.InputRouter.Navigate.Execute(vm))
                .Select(_ => Unit.Default);
        }

        protected async override Task OnAdding()
        {
            await Task.CompletedTask;
            //await NavigateToInput(new OperatoreAddViewModel(ConfigHost,
            //                          Locator.Current.GetService<IOperatoreRepository>())).ToTask();
        }

        protected async override Task OnDeleting()
        {
            await Task.CompletedTask;
            //await NavigateToInput(new OperatoreDelViewModel(ConfigHost, GroupBindingT.Id,
            //                          Locator.Current.GetService<IOperatoreRepository>())).ToTask();
        }

        protected async override Task OnUpdating()
        {
            await Task.CompletedTask;
            //await NavigateToInput(new OperatoreUpdViewModel(ConfigHost, GroupBindingT.Id,
            //                          Locator.Current.GetService<IOperatoreRepository>())).ToTask();
        }

        protected override Task OnEsc() => Task.FromResult(Unit.Default);
        
    }
}
