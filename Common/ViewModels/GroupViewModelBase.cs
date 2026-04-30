using Avalonia.Collections;
using DTO.Entity;
using ReactiveUI;
using SysNet;
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

    public abstract partial class GroupViewModelBase<TMap> : BaseViewModel where TMap : class, new()
    {

        public ReactiveCommand<Unit, Unit> AddCommand { get; }
        public ReactiveCommand<Unit, Unit> UpdCommand { get; }
        public ReactiveCommand<Unit, Unit> DelCommand { get; }
        public ReactiveCommand<Unit, Unit> FilterCommand { get; }

        protected virtual IObservable<bool> canAdd => Observable.Return(true);
        protected virtual IObservable<bool> canDel => Observable.Return(true);
        protected IObservable<bool> canUpd => this.WhenAnyValue(x => x.GroupBindingT)
                                                           .Select(item => item != null);

        

        public GroupViewModelBase(IScreen host) : base(host)
        {
            
            var canExecuteGeneral = this.WhenAnyValue(x => x.IsLoading).Select(l => !l);


            
            // 2. Comandi legati all'IsLoading automatico della BaseViewModel
            AddCommand = ReactiveCommand.CreateFromTask(ExecuteAdding,
                canExecuteGeneral.CombineLatest(canAdd, (gen, child) => gen && child));

            DelCommand = ReactiveCommand.CreateFromTask(ExecuteDeleting,
                canExecuteGeneral.CombineLatest(canDel, (gen, child) => gen && child));

            UpdCommand = ReactiveCommand.CreateFromTask(ExecuteUpdating,
                canExecuteGeneral.CombineLatest(canUpd, (gen, child) => gen && child));

            FilterCommand = LoadCommand;

            InitializeLoadingHelper(); // Attivazione iniziale

            this.WhenActivated(d =>
            {
                Disposable.Create(() =>
                {
                    // PULIZIA CRITICA: Sgancia la View della griglia
                    GroupedDataSource = null;
                    DataSource = null;

                }).DisposeWith(d);

                

                HandleCommandsDisposal(d);

            });
        }

        protected override IObservable<bool> IsAnythingExecuting =>
            new[]
            {
                base.IsAnythingExecuting,
                AddCommand?.IsExecuting ?? Observable.Return(false),
                UpdCommand?.IsExecuting ?? Observable.Return(false),
                DelCommand?.IsExecuting ?? Observable.Return(false)
            }.CombineLatest(values => values.Any(x => x));

        private void HandleCommandsDisposal(CompositeDisposable d)
        {
            AddCommand?.DisposeWith(d);
            UpdCommand?.DisposeWith(d);
            DelCommand?.DisposeWith(d);
            FilterCommand?.DisposeWith(d);
        }

        protected override void OnFinalDestruction()
        {
            // Assicuriamoci che la collezione sia nulla per il GC
            GroupedDataSource = null;
            DataSource = null;
            base.OnFinalDestruction();
        }

        public virtual Task CaricaByModel(object model) { return Task.CompletedTask; }


        public async Task ExecuteAdding()
        {
            if (_isClosing) return;
            await Task.Delay(50, token);
            try { await OnAdding(); }
            catch (Exception ex) { Debug.WriteLine($"ERRORE ADD: {ex.Message}"); }
            // IsLoading torna false da solo se aggiungi AddCommand all'OAPH della base
        }

        public async Task ExecuteDeleting()
        {
            if (_isClosing) return;
            await Task.Delay(50, token);
            try { await OnDeleting(); }
            catch (Exception ex) { Debug.WriteLine($"ERRORE DELETE: {ex.Message}"); }
            // IsLoading torna false da solo se aggiungi DelCommand all'OAPH della base
        }
        public async Task ExecuteUpdating()
        {
            if (_isClosing) return;
            await Task.Delay(50, token);
            try { await OnUpdating(); }
            catch (Exception ex) { Debug.WriteLine($"ERRORE UPDATE: {ex.Message}"); }
            // IsLoading torna false da solo se aggiungi UpdCommand all'OAPH della base
        }

        protected abstract Task OnAdding();
        protected abstract Task OnDeleting();
        protected abstract Task OnUpdating();


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
