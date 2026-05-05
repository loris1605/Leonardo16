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
 
    public class PersonGroupViewModel : GroupViewModelBase<PersonMap>, IGroupViewModelBase, IPersonGroupViewModel
    {
        private IPersonRepository Q;

        public ReactiveCommand<Unit, Unit> AddCodiceSocioCommand { get; protected set; }
        public ReactiveCommand<Unit, Unit> DelCodiceSocioCommand { get; protected set; }
        public ReactiveCommand<Unit, Unit> UpdCodiceSocioCommand { get; protected set; }
        public ReactiveCommand<Unit, Unit> AddTesseraCommand { get; protected set; }
        public ReactiveCommand<Unit, Unit> DelTesseraCommand { get; protected set; }
        public ReactiveCommand<Unit, Unit> UpdTesseraCommand { get; protected set; }
        public ReactiveCommand<Unit, Unit> PersonSearchCommand { get; protected set; }

        protected ISociScreen _host;

        public IObservable<bool> CanAction { get; }

        protected override IObservable<bool> canDel => this.WhenAnyValue(
            x => x.GroupBindingT,
            x => x.GroupBindingT.CodiceSocio, // Osserva esplicitamente la proprietà interna
            (item, codiceSocio) => item != null && codiceSocio == 0
        );

        protected override IObservable<bool> IsAnythingExecuting =>
            new[]
            {
                base.IsAnythingExecuting,
                AddCodiceSocioCommand?.IsExecuting ?? Observable.Return(false),
                DelCodiceSocioCommand?.IsExecuting ?? Observable.Return(false),
                UpdCodiceSocioCommand?.IsExecuting ?? Observable.Return(false),
                AddTesseraCommand?.IsExecuting ?? Observable.Return(false),
                DelTesseraCommand?.IsExecuting ?? Observable.Return(false),
                UpdTesseraCommand?.IsExecuting ?? Observable.Return(false),
                PersonSearchCommand?.IsExecuting ?? Observable.Return(false)
            }.CombineLatest(values => values.Any(x => x));

        public PersonGroupViewModel(IPersonRepository Repository) : base(null)
        {
            Q = Repository ?? throw new ArgumentNullException(nameof(Repository));

            var canAction = this.WhenAnyValue(x => x.GroupBindingT).Select(item => item != null);

            var canSocioExists = this.WhenAnyValue(x => x.GroupBindingT)
                                     .Select(item => item != null && item.CodiceSocio != 0);


            var canSocioDelete = this.WhenAnyValue(x => x.GroupBindingT)
                                     .Select(item => item != null && item.CodiceSocio != 0 && item.CodiceTessera == 0);


            var canTesseraExists = this.WhenAnyValue(x => x.GroupBindingT)
                                       .Select(item => item != null && item.CodiceSocio != 0 && item.CodiceTessera != 0);


            //AddCodiceSocioCommand = ReactiveCommand.CreateFromObservable(
            //() => NavigateToInput(new CodiceSocioAddViewModel(ConfigHost, GroupBindingT!.Id, Q)), canAction);

            //DelCodiceSocioCommand = ReactiveCommand.CreateFromObservable(
            //    () => NavigateToInput(new CodiceSocioDelViewModel(ConfigHost, GroupBindingT.CodiceSocio, GroupBindingT.Id, Q)), canSocioDelete);

            //UpdCodiceSocioCommand = ReactiveCommand.CreateFromObservable(
            //    () => NavigateToInput(new CodiceSocioUpdViewModel(ConfigHost, GroupBindingT.CodiceSocio, GroupBindingT.Id, Q)), canSocioExists);

            //AddTesseraCommand = ReactiveCommand.CreateFromObservable(
            //    () => NavigateToInput(new TesseraAddViewModel(ConfigHost, GroupBindingT.Id, GroupBindingT.CodiceSocio, Q)), canSocioExists);

            //DelTesseraCommand = ReactiveCommand.CreateFromObservable(
            //    () => NavigateToInput(new TesseraDelViewModel(ConfigHost, GroupBindingT.CodiceTessera, GroupBindingT.Id, Q)), canTesseraExists);

            //UpdTesseraCommand = ReactiveCommand.CreateFromObservable(
            //    () => NavigateToInput(new TesseraUpdViewModel(ConfigHost, GroupBindingT.CodiceTessera, GroupBindingT.Id, Q)), canTesseraExists);

            PersonSearchCommand = ReactiveCommand.CreateFromTask(async () => await NavigateTo<IPersonSearchViewModel>());

            InitializeLoadingHelper();

            this.WhenActivated(d =>
            {
                AddCodiceSocioCommand?.DisposeWith(d);
                DelCodiceSocioCommand?.DisposeWith(d);
                UpdCodiceSocioCommand?.DisposeWith(d);
                AddTesseraCommand?.DisposeWith(d);
                DelTesseraCommand?.DisposeWith(d);
                UpdTesseraCommand?.DisposeWith(d);
                PersonSearchCommand?.DisposeWith(d);
            });


        }
      
        protected override void OnFinalDestruction()
        {
            // Assicuriamoci che la collezione sia nulla per il GC
            AddCodiceSocioCommand = DelCodiceSocioCommand = UpdCodiceSocioCommand = null;
            AddTesseraCommand = DelTesseraCommand = UpdTesseraCommand = PersonSearchCommand = null;

            Q = null;
            base.OnFinalDestruction();
        }

        public void SetHost(ISociScreen host) => _host = host;

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
                DataSource = new List<PersonMap>();
                GroupedDataSource = null;
            }
        }

        private async Task UpdateCollection(List<PersonDTO> data, int id)
        {
            var mapped = await Task.Run(() => data.Select(dto => new PersonMap(dto)).ToList(), token);
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
                token.ThrowIfCancellationRequested();
                await UpdateCollection(data, id);
            }
            catch (OperationCanceledException) { }
        }

        public override async Task CaricaByModel(object model)
        {
            // 1. Controllo di tipo sicuro (evita crash se model non è List<PersonMap>)
            if (model is List<PersonMap> list)
            {
                // Nota: IsLoading qui non serve settarlo a mano se questo metodo
                // viene chiamato da un comando già monitorato (come Search o Filter)

                // 2. Creazione della View (possiamo farlo in background per liste grandi)
                var view = await Task.Run(() =>
                {
                    var v = new DataGridCollectionView(list);
                    v.GroupDescriptions.Add(new DataGridPathGroupDescription("Titolo"));
                    return v;
                }, token);

                // 3. Aggiornamento UI sul thread principale
                GroupedDataSource = view;
                IdIndex = 0;

                // Seleziona il primo per attivare i comandi (canAction)
                GroupBindingT = list.FirstOrDefault();

                GroupFocus = true;
            }

            await Task.CompletedTask;
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

        protected async override Task OnAdding() => await NavigateTo<IPersonAddViewModel>();
        
        protected async override Task OnDeleting() => 
            await NavigateTo<IPersonDelViewModel>(vm => vm.SetIdDaModificare(GroupBindingT.Id));

        protected async override Task OnUpdating() => 
            await NavigateTo<IPersonUpdViewModel>(vm => vm.SetIdDaModificare(GroupBindingT.Id));

        protected override Task OnEsc() => Task.CompletedTask;
        

        public string NumeroSocio => GroupBindingT is null ? "" : GroupBindingT.NumeroSocio;
        public string NumeroTessera => GroupBindingT is null ? "" : GroupBindingT.NumeroTessera;
        public int CodiceSocio => GroupBindingT is null ? 0 : GroupBindingT.CodiceSocio;
        public int CodiceTessera => GroupBindingT is null ? 0 : GroupBindingT.CodiceTessera;
        public int Scadenza => GroupBindingT is null ? 0 : GroupBindingT.Scadenza;
    }
}
