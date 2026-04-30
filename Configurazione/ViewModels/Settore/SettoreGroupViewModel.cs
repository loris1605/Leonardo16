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
    public class SettoreGroupViewModel : GroupViewModelBase<SettoreMap>, IGroupViewModelBase, ISettoreGroupViewModel
    {
        public ReactiveCommand<Unit, Unit> OperatoriCommand { get; protected set; }
        public ReactiveCommand<Unit, Unit> PostazioniCommand { get; protected set; }
        public ReactiveCommand<Unit, Unit> TariffeCommand { get; protected set; }
        public ReactiveCommand<Unit, Unit> ListiniCommand { get; protected set; }
        public ReactiveCommand<Unit, Unit> RepartiCommand { get; protected set; }

        private ISettoreRepository Q;
        
        protected IConfigurazioneScreen _host;

        //fa il merge con la IObservable base
        protected override IObservable<bool> canDel => this.WhenAnyValue(
            x => x.GroupBindingT,
            x => x.GroupBindingT.CodiceListino, // Osserva esplicitamente la proprietà interna
            x => x.GroupBindingT.HasReparto,
            (item, codiceSocio, hasReparto) => item != null && codiceSocio == 0 && !hasReparto
        );

        // Registriamo i nuovi comandi nell'IsLoading globale per automatizzare l'icona di attesa
        protected override IObservable<bool> IsAnythingExecuting =>
            new[]
            {
                base.IsAnythingExecuting,
                OperatoriCommand?.IsExecuting ?? Observable.Return(false),
                PostazioniCommand?.IsExecuting ?? Observable.Return(false),
                RepartiCommand?.IsExecuting ?? Observable.Return(false),
                TariffeCommand?.IsExecuting ?? Observable.Return(false)
            }.CombineLatest(values => values.Any(x => x));

        public SettoreGroupViewModel(ISettoreRepository Repository) : base(null)
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

            //ListiniCommand = ReactiveCommand.CreateFromObservable(
            //    () => NavigateToInput(new ListiniViewModel(ConfigHost, GroupBindingT!.Id,
            //    Locator.Current.GetService<ISettoreRepository>())), canHasSelection);

            //RepartiCommand = ReactiveCommand.CreateFromObservable(
            //    () => NavigateToReset(new OperatoreAddViewModel(ConfigHost)), isNotLoading);

            InitializeLoadingHelper();

            this.WhenActivated(d =>
            {

                OperatoriCommand?.DisposeWith(d);
                TariffeCommand?.DisposeWith(d);
                PostazioniCommand?.DisposeWith(d);
                RepartiCommand?.DisposeWith(d);

            });
        }

        public void SetHost(IConfigurazioneScreen host) => _host = host;

        protected override void OnFinalDestruction()
        {
            // Assicuriamoci che la collezione sia nulla per il GC
            Q = null;
            OperatoriCommand = PostazioniCommand = TariffeCommand = ListiniCommand = RepartiCommand = null;
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
                DataSource = new List<SettoreMap>();
                GroupedDataSource = null;
            }
        }

        private async Task UpdateCollection(List<SettoreDTO> data, int id)
        {
            var mapped = await Task.Run(() => data.Select(dto => new SettoreMap(dto)).ToList(), token);
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

        protected async override Task OnAdding()
        {
            var addVm = Locator.Current.GetService<ISettoreAddViewModel>();
            if (addVm != null)
            {
                // 2. Impostiamo l'host (lo screen principale)
                addVm.SetHost(_host);
                try
                {
                    // 3. Eseguiamo la navigazione FORZANDOLA sul Main Thread della UI
                    await Observable.Start(async () =>
                    {
                        await NavigateToInput(addVm);
                    }, RxSchedulers.MainThreadScheduler);
                }
                catch (Exception ex)
                {
                    _isClosing = false;
                    Debug.WriteLine($"ERRORE durante la navigazione alla Add Settore: {ex.Message}");
                }
            }
            else
            {
                _isClosing = false; // Permette all'utente di riprovare se il DI fallisce
                Debug.WriteLine("ERRORE CRITICO: ISettoreAddViewModel non è stato risolto dal Locator.");
            }
        }

        protected async override Task OnDeleting()
        {
            var delVm = Locator.Current.GetService<ISettoreDelViewModel>();
            if (delVm != null)
            {
                // 2. Impostiamo l'host (lo screen principale)
                delVm.SetHost(_host);
                delVm.SetIdDaModificare(GroupBindingT.Id);
                try
                {
                    // 3. Eseguiamo la navigazione FORZANDOLA sul Main Thread della UI
                    await Observable.Start(async () =>
                    {
                        await NavigateToInput(delVm);
                    }, RxSchedulers.MainThreadScheduler);
                }
                catch (Exception ex)
                {
                    _isClosing = false;
                    Debug.WriteLine($"ERRORE durante la navigazione alla Delete Settore: {ex.Message}");
                }
            }
            else
            {
                _isClosing = false; // Permette all'utente di riprovare se il DI fallisce
                Debug.WriteLine("ERRORE CRITICO: ISettoreDelViewModel non è stato risolto dal Locator.");
            }
        }

        protected async override Task OnUpdating()
        {
            var updVm = Locator.Current.GetService<ISettoreUpdViewModel>();
            if (updVm != null)
            {
                // 2. Impostiamo l'host (lo screen principale)
                updVm.SetHost(_host);
                updVm.SetIdDaModificare(GroupBindingT.Id);
                try
                {
                    // 3. Eseguiamo la navigazione FORZANDOLA sul Main Thread della UI
                    await Observable.Start(async () =>
                    {
                        await NavigateToInput(updVm);
                    }, RxSchedulers.MainThreadScheduler);
                }
                catch (Exception ex)
                {
                    _isClosing = false;
                    Debug.WriteLine($"ERRORE durante la navigazione alla Update Settore: {ex.Message}");
                }
            }
            else
            {
                _isClosing = false; // Permette all'utente di riprovare se il DI fallisce
                Debug.WriteLine("ERRORE CRITICO: ISettoreUpdViewModel non è stato risolto dal Locator.");
            }
        }

        protected override async Task OnEsc() => await Task.CompletedTask;
        

    }
}
