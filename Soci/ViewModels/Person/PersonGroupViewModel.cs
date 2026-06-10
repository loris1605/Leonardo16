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
using System.Reactive.Subjects;
using ViewModels.BindableObjects;

namespace ViewModels
{
 
    public partial class PersonGroupViewModel : GroupViewModelBase<PersonMap>, IGroupViewModelBase, IPersonGroupViewModel
    {
        // ---------------------------------------------------------------------
        // 1. Dipendenze e Comandi Reattivi Locali
        // ---------------------------------------------------------------------
        private IPersonRepository Q;
        protected ISociScreen _host;

        public ReactiveCommand<Unit, Unit> AddCodiceSocioCommand { get; protected set; }
        public ReactiveCommand<Unit, Unit> DelCodiceSocioCommand { get; protected set; }
        public ReactiveCommand<Unit, Unit> UpdCodiceSocioCommand { get; protected set; }
        public ReactiveCommand<Unit, Unit> AddTesseraCommand { get; protected set; }
        public ReactiveCommand<Unit, Unit> DelTesseraCommand { get; protected set; }
        public ReactiveCommand<Unit, Unit> UpdTesseraCommand { get; protected set; }
        public ReactiveCommand<Unit, Unit> PersonSearchCommand { get; protected set; }

        public IObservable<bool> CanAction { get; }

        // ---------------------------------------------------------------------
        // 2. Condizioni di Esecuzione (Override Regole di Validazione)
        // ---------------------------------------------------------------------
        protected override IObservable<bool> canDel => this.WhenAnyValue(
            x => x.GroupBindingT,
            x => x.GroupBindingT.CodiceSocio, // Osserva esplicitamente la proprietà interna nel model mappato
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

            var canSocioUpdate = this.WhenAnyValue(x => x.GroupBindingT, x => x.IsLoading,
                (item, loading) => item != null &&
                                   item.CodiceSocio != 0 &&
                                   !loading);


            var canTesseraExists = this.WhenAnyValue(x => x.GroupBindingT)
                                       .Select(item => item != null && item.CodiceSocio != 0 && item.CodiceTessera != 0);


            AddCodiceSocioCommand = ReactiveCommand.CreateFromTask(OnCodiceSocioAdding, canAction);
            DelCodiceSocioCommand = ReactiveCommand.CreateFromTask(OnCodiceSocioDeleting, canSocioDelete);
            UpdCodiceSocioCommand = ReactiveCommand.CreateFromTask(OnCodiceSocioUpdating, canSocioUpdate);
            AddTesseraCommand = ReactiveCommand.CreateFromTask(OnTesseraAdding, canSocioExists);
            DelTesseraCommand = ReactiveCommand.CreateFromTask(OnTesseraDeleting, canTesseraExists);
            UpdTesseraCommand = ReactiveCommand.CreateFromTask(OnTesseraUpdating, canTesseraExists);

            PersonSearchCommand = ReactiveCommand.CreateFromTask(async () => await NavigateTo<IPersonSearchViewModel>());


            this.WhenActivated(d =>
            {
                // Sottoscrizione centralizzata alle eccezioni per preservare la stabilità dell'applicazione
                AddCodiceSocioCommand.ThrownExceptions.Subscribe(ex => Debug.WriteLine($"Errore AddCodiceSocio: {ex.Message}")).DisposeWith(d);
                DelCodiceSocioCommand.ThrownExceptions.Subscribe(ex => Debug.WriteLine($"Errore DelCodiceSocio: {ex.Message}")).DisposeWith(d);
                UpdCodiceSocioCommand.ThrownExceptions.Subscribe(ex => Debug.WriteLine($"Errore UpdCodiceSocio: {ex.Message}")).DisposeWith(d);
                AddTesseraCommand.ThrownExceptions.Subscribe(ex => Debug.WriteLine($"Errore AddTessera: {ex.Message}")).DisposeWith(d);
                DelTesseraCommand.ThrownExceptions.Subscribe(ex => Debug.WriteLine($"Errore DelTessera: {ex.Message}")).DisposeWith(d);
                UpdTesseraCommand.ThrownExceptions.Subscribe(ex => Debug.WriteLine($"Errore UpdTessera: {ex.Message}")).DisposeWith(d);
                PersonSearchCommand.ThrownExceptions.Subscribe(ex => Debug.WriteLine($"Errore PersonSearch: {ex.Message}")).DisposeWith(d);

                // Pulizia automatica dei comandi locali alla disattivazione
                AddCodiceSocioCommand.DisposeWith(d);
                DelCodiceSocioCommand.DisposeWith(d);
                UpdCodiceSocioCommand.DisposeWith(d);
                AddTesseraCommand.DisposeWith(d);
                DelTesseraCommand.DisposeWith(d);
                UpdTesseraCommand.DisposeWith(d);
                PersonSearchCommand.DisposeWith(d);
            });


        }

        protected override void OnFinalDestruction()
        {
            // Pulizia e dereferenziazione esplicita per agevolare l'intervento del Garbage Collector
            AddCodiceSocioCommand = DelCodiceSocioCommand = UpdCodiceSocioCommand = null;
            AddTesseraCommand = DelTesseraCommand = UpdTesseraCommand = PersonSearchCommand = null;
            _host = null;
            Q = null;

            base.OnFinalDestruction();
        }

        public void SetHost(ISociScreen host) => _host = host;

        // ---------------------------------------------------------------------
        // 4. Ciclo di Vita ed Implementazione IGroupViewModelBase
        // ---------------------------------------------------------------------
        protected override async Task OnLoading()
        {
            var data = await Q.Load(0, Token);
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
            var mapped = await Task.Run(() => data.Select(dto => new PersonMap(dto)).ToList(), Token);
            var view = new DataGridCollectionView(mapped);
            view.GroupDescriptions.Add(new DataGridPathGroupDescription("Titolo"));

            var backup = GroupBindingT;
            GroupBindingT = null;
            GroupedDataSource = view;
            GroupBindingT = backup;

            IdIndex = id;
            GroupFocus = true;
        }

        public override async Task CaricaDataSource(int id = 0)
        {
            try
            {
                var data = await Q.Load(id, Token);
                Token.ThrowIfCancellationRequested();
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
                }, Token);

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

        protected async override Task OnAdding()
        {
            _groupToPersonAdd.OnNext(Unit.Default);
            await Task.CompletedTask;
        }

        protected async override Task OnDeleting()
        {
            _groupToPersonDel.OnNext(GroupBindingT.Id);
            await Task.CompletedTask;
        }

        protected async override Task OnUpdating()
        {
            _groupToPersonUpd.OnNext(GroupBindingT.Id);
            await Task.CompletedTask;
        }

        protected async Task OnCodiceSocioAdding()
        {
            _groupToCodiceSocioAdd.OnNext(GroupBindingT.Id);
            await Task.CompletedTask;
        }

        protected async Task OnCodiceSocioDeleting()
        {
            _groupToCodiceSocioDel.OnNext(GroupBindingT.CodiceSocio);
            await Task.CompletedTask;
        }
        
        protected async Task OnCodiceSocioUpdating() =>
            await NavigateTo<ICodiceSocioUpdViewModel>(vm =>
            {
                vm.SetIdDaModificare(GroupBindingT.CodiceSocio);
                vm.SetIdRitorno(GroupBindingT.Id);
            });

        protected async Task OnTesseraAdding() =>
            await NavigateTo<ITesseraAddViewModel>(vm =>
            {
                vm.SetIdDaModificare(GroupBindingT.CodiceSocio);
                vm.SetIdRitorno(GroupBindingT.Id);
            });

        protected async Task OnTesseraDeleting() =>
            await NavigateTo<ITesseraDelViewModel>(vm =>
            {
                vm.SetIdDaModificare(GroupBindingT.CodiceTessera);
                vm.SetIdRitorno(GroupBindingT.Id);
            });

        protected async Task OnTesseraUpdating() =>
            await NavigateTo<ITesseraUpdViewModel>(vm =>
            {
                vm.SetIdDaModificare(GroupBindingT.CodiceTessera);
                vm.SetIdRitorno(GroupBindingT.Id);
            });

        protected override Task OnEsc() => Task.CompletedTask;
        

        public string NumeroSocio => GroupBindingT is null ? "" : GroupBindingT.NumeroSocio;
        public string NumeroTessera => GroupBindingT is null ? "" : GroupBindingT.NumeroTessera;
        public int CodiceSocio => GroupBindingT is null ? 0 : GroupBindingT.CodiceSocio;
        public int CodiceTessera => GroupBindingT is null ? 0 : GroupBindingT.CodiceTessera;
        public int Scadenza => GroupBindingT is null ? 0 : GroupBindingT.Scadenza;
    }

    public partial class PersonGroupViewModel
    {
        // 1. Aggiungi questo Subject per notificare l'esterno
        private readonly Subject<Unit> _groupToPersonAdd = new();
        public IObservable<Unit> GroupToPersonAdd => _groupToPersonAdd.AsObservable();

        private readonly Subject<int> _groupToPersonDel = new();
        public IObservable<int> GroupToPersonDel => _groupToPersonDel.AsObservable();

        private readonly Subject<int> _groupToPersonUpd = new();
        public IObservable<int> GroupToPersonUpd => _groupToPersonUpd.AsObservable();

        private readonly Subject<int> _groupToCodiceSocioAdd = new();
        public IObservable<int> GroupToCodiceSocioAdd => _groupToCodiceSocioAdd.AsObservable();

        private readonly Subject<int> _groupToCodiceSocioDel = new();
        public IObservable<int> GroupToCodiceSocioDel => _groupToCodiceSocioDel.AsObservable();

        private readonly Subject<int> _groupToCodiceSocioUpd = new();
        public IObservable<int> GroupToCodiceSocioUpd => _groupToCodiceSocioUpd.AsObservable();
    }
}
