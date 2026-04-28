using Avalonia.Collections;
using DTO.Entity;
using DTO.Repository;
using ReactiveUI;
using System.Reactive;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using ViewModels.BindableObjects;

namespace ViewModels
{
    public class PostazioneGroupViewModel : GroupViewModelBase<PostazioneMap>, IGroupViewModelBase
    {
        public ReactiveCommand<Unit, Unit> OperatoriCommand { get; protected set; }
        public ReactiveCommand<Unit, Unit> SettoriCommand { get; protected set; }
        public ReactiveCommand<Unit, Unit> TariffeCommand { get; protected set; }
        public ReactiveCommand<Unit, Unit> RepartiCommand { get; protected set; }

        private IPostazioneRepository Q;
        private readonly IServiceProvider _sp;

        protected IConfigurazioneScreen _host;

        //fa il merge con la IObservable base
        protected override IObservable<bool> canDel => this.WhenAnyValue(
            x => x.GroupBindingT,
            x => x.GroupBindingT.CodiceReparto,
            x => x.GroupBindingT.HasPermesso,
            (item, codiceSocio, hasP) => item != null && codiceSocio == 0 && !hasP
        );

        public PostazioneGroupViewModel(IConfigurazioneScreen host,
                                        IPostazioneRepository Repository,
                                        IServiceProvider sp) : base(host)
        {
            Q = Repository ?? throw new ArgumentNullException(nameof(Repository));
            _sp = sp ?? throw new ArgumentNullException(nameof(sp));
            _host = host;

            var canHasSelection = this.WhenAnyValue(x => x.GroupBindingT).Select(item => item != null);


            //OperatoriCommand = ReactiveCommand.CreateFromObservable(
            //() => NavigateToReset(new OperatoreGroupViewModel(ConfigHost, Locator.Current.GetService<IOperatoreRepository>())));

            //SettoriCommand = ReactiveCommand.CreateFromObservable(
            //    () => NavigateToReset(new SettoreGroupViewModel(ConfigHost, Locator.Current.GetService<ISettoreRepository>())));

            //TariffeCommand = ReactiveCommand.CreateFromObservable(
            //    () => NavigateToReset(new TariffaGroupViewModel(ConfigHost, Locator.Current.GetService<ITariffaRepository>())));

            //RepartiCommand = ReactiveCommand.CreateFromObservable(
            //    () => NavigateToInput(new RepartiViewModel(ConfigHost, GroupBindingT!.Id, Q)),
            //    canHasSelection);

            InitializeLoadingHelper();

            this.WhenActivated(d =>
            {

                OperatoriCommand.DisposeWith(d);
                SettoriCommand.DisposeWith(d);
                TariffeCommand.DisposeWith(d);
                RepartiCommand.DisposeWith(d);

            });
                  
        }

        protected override IObservable<bool> IsAnythingExecuting =>
            new[]
            {
                base.IsAnythingExecuting,
                OperatoriCommand?.IsExecuting ?? Observable.Return(false),
                SettoriCommand?.IsExecuting ?? Observable.Return(false),
                RepartiCommand?.IsExecuting ?? Observable.Return(false),
                TariffeCommand?.IsExecuting ?? Observable.Return(false)
            }.CombineLatest(values => values.Any(x => x));

        
        protected override void OnFinalDestruction()
        {
            OperatoriCommand = SettoriCommand = TariffeCommand = RepartiCommand = null;
            Q = null;
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
                DataSource = new List<PostazioneMap>();
                GroupedDataSource = null;
            }
        }

        private async Task UpdateCollection(List<PostazioneDTO> data, int id)
        {
            var mapped = await Task.Run(() => data.Select(dto => new PostazioneMap(dto)).ToList(), token);
            var view = new DataGridCollectionView(mapped);
            view.GroupDescriptions.Add(new DataGridPathGroupDescription("Titolo"));

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
            if (HostScreen == null) return Observable.Return(Unit.Default);

            _isClosing = true; // Impedisce la navigazione multipla

            return _host.GroupRouter.NavigateAndReset.Execute(vm).Select(_ => Unit.Default);
        }

        protected IObservable<Unit> NavigateToInput(IRoutableViewModel vm)
        {
            return Observable.Start(() => _host.GroupEnabled = false, RxSchedulers.MainThreadScheduler)
                .SelectMany(_ => _host.InputRouter.Navigate.Execute(vm))
                .Select(_ => Unit.Default);
        }

        protected async override Task OnAdding()
        {
            await Task.CompletedTask;
            //await NavigateToInput(new PostazioneAddViewModel(ConfigHost,
            //                          Locator.Current.GetService<IPostazioneRepository>())).ToTask();
        }

        protected async override Task OnDeleting()
        {
            await Task.CompletedTask;
            //await NavigateToInput(new PostazioneDelViewModel(ConfigHost, GroupBindingT.Id,
            //                          Locator.Current.GetService<IPostazioneRepository>())).ToTask();
        }

        protected async override Task OnUpdating()
        {
            await Task.CompletedTask;
            //await NavigateToInput(new PostazioneUpdViewModel(ConfigHost, GroupBindingT.Id,
            //                          Locator.Current.GetService<IPostazioneRepository>())).ToTask();
        }

        protected override Task OnEsc()
        {
            throw new NotImplementedException();
        }
    }
}
