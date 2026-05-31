using Avalonia.Collections;
using ReactiveUI;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;

namespace ViewModels
{
    public interface IGroupViewModelBase
    {
        bool GroupFocus { get; set; }
        Task CaricaDataSource(int id = 0);
        Task CaricaByModel(object model);

    }

    public abstract partial class GroupViewModelBase<TMap> : ViewModelBase where TMap : class, new()
    {

        // ---------------------------------------------------------------------
        // 1. Comandi Reattivi Centralizzati del Gruppo
        // ---------------------------------------------------------------------
        public ReactiveCommand<Unit, Unit> AddCommand { get; }
        public ReactiveCommand<Unit, Unit> UpdCommand { get; }
        public ReactiveCommand<Unit, Unit> DelCommand { get; }
        public ReactiveCommand<Unit, Unit> FilterCommand { get; }

        // ---------------------------------------------------------------------
        // 2. Condizioni di Esecuzione (Overridabili dalle classi figlie)
        // ---------------------------------------------------------------------
        protected virtual IObservable<bool> canAdd => Observable.Return(true);
        protected virtual IObservable<bool> canDel => Observable.Return(true);
        protected IObservable<bool> canUpd => this.WhenAnyValue(x => x.GroupBindingT)
                                                   .Select(item => item != null);

        // ---------------------------------------------------------------------
        // 3. Flussi Reattivi Centralizzati (Override Controllo Doppio Clic Senza "base")
        // ---------------------------------------------------------------------
        protected override IObservable<bool> IsAnythingExecuting =>
            Observable.CombineLatest(
                // 1. Monitoriamo i comandi ereditati dalla classe base
                this.WhenAnyObservable(x => x.LoadCommand.IsExecuting).StartWith(false),
                this.WhenAnyObservable(x => x.SaveCommand.IsExecuting).StartWith(false),
                this.WhenAnyObservable(x => x.EscPressedCommand.IsExecuting).StartWith(false),
                // 2. Monitoriamo i comandi locali astratti di questa classe intermedia
                this.WhenAnyValue(x => x.AddCommand).SelectMany(cmd => cmd?.IsExecuting ?? Observable.Return(false)),
                this.WhenAnyValue(x => x.UpdCommand).SelectMany(cmd => cmd?.IsExecuting ?? Observable.Return(false)),
                this.WhenAnyValue(x => x.DelCommand).SelectMany(cmd => cmd?.IsExecuting ?? Observable.Return(false)),
                // Se un qualunque comando sta elaborando, IsLoading diventa true e la UI si blocca a 0ms
                (l, s, e, add, upd, del) => l || s || e || add || upd || del)
            .DistinctUntilChanged();


        // ---------------------------------------------------------------------
        // Constructor
        // ---------------------------------------------------------------------
        public GroupViewModelBase(IScreen host) : base(host)
        {
            var canExecuteGeneral = this.WhenAnyValue(x => x.IsLoading).Select(l => !l);

            // Inizializzazione dei comandi incrociati con i vincoli generali e specifici delle classi figlie
            AddCommand = ReactiveCommand.CreateFromTask(ExecuteAdding,
                canExecuteGeneral.CombineLatest(canAdd, (gen, child) => gen && child));

            DelCommand = ReactiveCommand.CreateFromTask(ExecuteDeleting,
                canExecuteGeneral.CombineLatest(canDel, (gen, child) => gen && child));

            UpdCommand = ReactiveCommand.CreateFromTask(ExecuteUpdating,
                canExecuteGeneral.CombineLatest(canUpd, (gen, child) => gen && child));

            FilterCommand = LoadCommand;

            

            this.WhenActivated(d =>
            {
                // Registrazione degli errori dei comandi per evitare crash spuri
                AddCommand.ThrownExceptions.Subscribe(ex => Debug.WriteLine($"Errore comando Aggiungi: {ex.Message}")).DisposeWith(d);
                DelCommand.ThrownExceptions.Subscribe(ex => Debug.WriteLine($"Errore comando Elimina: {ex.Message}")).DisposeWith(d);
                UpdCommand.ThrownExceptions.Subscribe(ex => Debug.WriteLine($"Errore comando Modifica: {ex.Message}")).DisposeWith(d);

                Disposable.Create(() =>
                {
                    // PULIZIA CRITICA: Sgancia esplicitamente le sorgenti dati della View deattivata per il GC
                    GroupedDataSource = null;
                    DataSource = null;
                }).DisposeWith(d);

                HandleCommandsDisposal(d);
            });
        }

        private void HandleCommandsDisposal(CompositeDisposable d)
        {
            AddCommand?.DisposeWith(d);
            UpdCommand?.DisposeWith(d);
            DelCommand?.DisposeWith(d);
            FilterCommand?.DisposeWith(d);
        }

        protected override void OnFinalDestruction()
        {
            // Assicuriamoci che le collezioni pesanti siano nulle prima della rimozione dallo stack
            GroupedDataSource = null;
            DataSource = null;
            base.OnFinalDestruction();
        }

        


        // ---------------------------------------------------------------------
        // 4. Wrapper di Esecuzione Protetti (Invocati dai pulsanti della View)
        // ---------------------------------------------------------------------
        public async Task ExecuteAdding()
        {
            if (_isClosing) return;
            await Task.Delay(50, Token);
            try { await OnAdding(); }
            catch (Exception ex) { Debug.WriteLine($"ERRORE ADD: {ex.Message}"); }
        }

        public async Task ExecuteDeleting()
        {
            if (_isClosing) return;
            await Task.Delay(50, Token);
            try { await OnDeleting(); }
            catch (Exception ex) { Debug.WriteLine($"ERRORE DELETE: {ex.Message}"); }
        }

        public async Task ExecuteUpdating()
        {
            if (_isClosing) return;
            await Task.Delay(50, Token);
            try { await OnUpdating(); }
            catch (Exception ex) { Debug.WriteLine($"ERRORE UPDATE: {ex.Message}"); }
        }

        // ---------------------------------------------------------------------
        // 5. Metodi Virtuali ed Astratti (Hook obbligatori per le classi concrete)
        // ---------------------------------------------------------------------
        protected abstract Task OnAdding();
        protected abstract Task OnDeleting();
        protected abstract Task OnUpdating();

        public virtual Task CaricaByModel(object model) => Task.CompletedTask;
        public virtual Task CaricaDataSource(int id) => Task.CompletedTask; // Integrato per supportare ISociScreen

       


        #region DataSource

        private IList<TMap> _datasource = [];
        public IList<TMap> DataSource
        {
            get => _datasource;
            set => this.RaiseAndSetIfChanged(ref _datasource, value);
        }

        #endregion

        #region BindingT

        private TMap _mybindingt = new();

        public TMap BindingT
        {
            get => _mybindingt;
            set => this.RaiseAndSetIfChanged(ref _mybindingt, value);
        }

        #endregion

        #region CheckNullBindingT

        private bool _checknullbindingt = false;

        public bool CheckNullBindingT
        {
            get => _checknullbindingt;
            set => this.RaiseAndSetIfChanged(ref _checknullbindingt, value);
        }

        #endregion

        #region GroupBindingT

        private TMap _mygroupbindingt = null;

        public TMap GroupBindingT
        {
            get => _mygroupbindingt;
            set => this.RaiseAndSetIfChanged(ref _mygroupbindingt, value);
        }

        #endregion

        #region GroupFocus

        private bool _groupfocus = false;
        public bool GroupFocus
        {
            get => _groupfocus;
            set => this.RaiseAndSetIfChanged(ref _groupfocus, value);
        }

        #endregion

        #region IdValue

        private int _idvalue = 0;
        public int IdValue
        {
            get => _idvalue;
            set => this.RaiseAndSetIfChanged(ref _idvalue, value);
        }

        #endregion

        #region IdIndex

        private int _idindex = 0;
        public int IdIndex
        {
            get => _idindex;
            set => this.RaiseAndSetIfChanged(ref _idindex, value);
        }

        #endregion

        #region SelectedIndex

        private int _selectedindex = 0;
        public int SelectedIndex
        {
            get => _selectedindex;
            set => this.RaiseAndSetIfChanged(ref _selectedindex, value);
        }

        #endregion

        #region EnabledButton

        private bool _enabledbutton;
        public bool EnabledButton
        {
            get => _enabledbutton;
            set => this.RaiseAndSetIfChanged(ref _enabledbutton, value);
        }

        #endregion

        private DataGridCollectionView _groupedDataSource;
        public DataGridCollectionView GroupedDataSource
        {
            get => _groupedDataSource;
            set => this.RaiseAndSetIfChanged(ref _groupedDataSource, value);
        }

        protected override Task OnLoading() => Task.CompletedTask;

        protected override Task OnSaving()
        {
            throw new NotImplementedException();
        }
    }


}
