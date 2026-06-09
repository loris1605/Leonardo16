using Common.InterViewModels;
using ReactiveUI;
using Splat;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace ViewModels
{

    public partial class SociViewModel : ViewModelBase, ISociScreen, ISociViewModel
    {
        // ---------------------------------------------------------------------
        // 1. Router Interni (Sub-Routing) e Dipendenze
        // ---------------------------------------------------------------------
        public RoutingState GroupRouter { get; } = new RoutingState();
        public RoutingState InputRouter { get; } = new RoutingState();

        // Espone il router principale richiesto dall'infrastruttura ReactiveUI
        public RoutingState Router => GroupRouter;

        private IScreen _host;
        public new IScreen HostScreen => _host;

        // ---------------------------------------------------------------------
        // 2. Controllo Esecuzione Centralizzato (Prevenzione Doppi Clic)
        // ---------------------------------------------------------------------
        protected override IObservable<bool> IsAnythingExecuting =>
            Observable.CombineLatest(
                // 1. Comandi base ereditati
                this.WhenAnyObservable(x => x.LoadCommand.IsExecuting).StartWith(false),
                this.WhenAnyObservable(x => x.SaveCommand.IsExecuting).StartWith(false),
                this.WhenAnyObservable(x => x.EscPressedCommand.IsExecuting).StartWith(false),
                // 2. Monitoraggio delle esecuzioni dei router (Navigazioni in corso)
                this.WhenAnyObservable(x => x.GroupRouter.NavigateAndReset.IsExecuting).StartWith(false),
                this.WhenAnyObservable(x => x.GroupRouter.Navigate.IsExecuting).StartWith(false),
                this.WhenAnyObservable(x => x.InputRouter.NavigateAndReset.IsExecuting).StartWith(false),
                // Se qualunque operazione o cambio pagina è attivo, blocca la UI
                (l, s, e, gReset, gNav, iReset) => l || s || e || gReset || gNav || iReset)
            .DistinctUntilChanged();

        // 1. Aggiungi questo Subject per notificare l'esterno
        private readonly Subject<Unit> _sociToMenu = new();
        public IObservable<Unit> SociToMenu => _sociToMenu.AsObservable();

        // ---------------------------------------------------------------------
        // Constructor
        // ---------------------------------------------------------------------
        public SociViewModel(IScreen host) : base(host)
        {
            _host = host;
        }

        // ---------------------------------------------------------------------
        // 3. Ciclo di Vita (Override dei Metodi Virtuali della Base)
        // ---------------------------------------------------------------------

        protected override void OnFinalDestruction()
        {
            // Svuotiamo gli stack di navigazione dei router interni per liberare le View collegate
            GroupRouter?.NavigationStack.Clear();
            InputRouter?.NavigationStack.Clear();
            _host = null;

            base.OnFinalDestruction();
        }

        protected override async Task OnLoading() => await GoToPersonGroup();
        protected override async Task OnSaving() => await Task.CompletedTask;
        protected override async Task OnEsc()
        {
            _isClosing = true;
            _sociToMenu.OnNext(Unit.Default);
            _sociToMenu.OnCompleted(); // Chiude il canale per sempre, prevenendo ulteriori notifiche

            await Task.CompletedTask;
         
        }

        // ---------------------------------------------------------------------
        // 4. Metodi di Interfaccia e Sincronizzazione Griglie (ISociScreen)
        // ---------------------------------------------------------------------
        public void AggiornaGridByObject(object model)
        {
            if (GroupRouter.GetCurrentViewModel() is IGroupViewModelBase groupVm)
            {
                groupVm.CaricaByModel(model);
            }
        }

        public void AggiornaGridByInt(int id)
        {
            if (GroupRouter.GetCurrentViewModel() is IGroupViewModelBase groupVm)
            {
                // Passiamo l'ID al metodo di caricamento della lista
                groupVm.CaricaDataSource(id);
            }
        }

    }

    public partial class SociViewModel
    {
        #region GroupEnabled

        private bool _groupenabled = true;
        public bool GroupEnabled
        {
            get => _groupenabled;
            set => this.RaiseAndSetIfChanged(ref _groupenabled, value);
        }

        #endregion


    }

    public partial class SociViewModel
    {
        private async Task GoToPersonGroup()
        {
            
            var tcs = new TaskCompletionSource();

            // 3. Risoluzione ViewModel e navigazione sul Main Thread
            RxSchedulers.MainThreadScheduler.Schedule(() =>
            {
                try
                {
                    // Nascendo qui dentro, il costruttore del LoginViewModel 
                    // viene eseguito sul thread UI, azzerando l'errore Cross-Thread!
                    var personVM = Locator.Current.GetService<IPersonGroupViewModel>();
                   
                    if (personVM != null)
                    {
                        personVM.SetHost(this); // Passiamo il riferimento alla schermata ospite al ViewModel del gruppo

                        var disposables = new CompositeDisposable();

                        personVM.GroupToPersonAdd
                            .ObserveOn(RxSchedulers.MainThreadScheduler)
                            .Subscribe(async _ =>
                            {
                                // Quando riceviamo il segnale di richiesta Add da parte del gruppo, navighiamo alla schermata di input
                                GroupEnabled = false; // Disabilitiamo il gruppo per evitare navigazioni multiple
                                await GoToPersonAdd();
                            }).DisposeWith(disposables);

                        personVM.GroupToPersonDel
                            .ObserveOn(RxSchedulers.MainThreadScheduler)
                            .Subscribe(async id =>
                            {
                                // Quando riceviamo il segnale di richiesta Del da parte del gruppo, navighiamo alla schermata di input
                                GroupEnabled = false; // Disabilitiamo il gruppo per evitare navigazioni multiple
                                await GoToPersonDel(id);
                            }).DisposeWith(disposables);

                        personVM.GroupToPersonUpd
                            .ObserveOn(RxSchedulers.MainThreadScheduler)
                            .Subscribe(async id =>
                            {
                                // Quando riceviamo il segnale di richiesta Upd da parte del gruppo, navighiamo alla schermata di input
                                GroupEnabled = false; // Disabilitiamo il gruppo per evitare navigazioni multiple
                                await GoToPersonUpd(id);
                            }).DisposeWith(disposables);


                        // Eseguiamo la navigazione e segnaliamo il completamento del Task
                        GroupRouter.NavigateAndReset.Execute(personVM)
                            .Subscribe(
                                _ => tcs.SetResult(),
                                ex => {
                                    tcs.SetException(ex);
                                    disposables.Dispose(); // Pulisce in caso di errore
                                }
                            );
                    }
                    else
                    {
                        Debug.WriteLine(">>> [ERROR] Impossibile risolvere IPersonGroupViewModel.");
                        tcs.SetResult();
                    }
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }

            });

            // Attendiamo che il thread della UI abbia finito l'operazione
            await tcs.Task;
        }

        private async Task GoToPersonAdd()
        {

            var tcs = new TaskCompletionSource();

            // 3. Risoluzione ViewModel e navigazione sul Main Thread
            RxSchedulers.MainThreadScheduler.Schedule(() =>
            {
                try
                {
                    // Nascendo qui dentro, il costruttore del LoginViewModel 
                    // viene eseguito sul thread UI, azzerando l'errore Cross-Thread!
                    var VM = Locator.Current.GetService<IPersonAddViewModel>();
                    

                    if (VM != null)
                    {
                        VM.SetHost(this); // Passiamo il riferimento alla schermata ospite al ViewModel DELL'INPUT
                        var disposables = new CompositeDisposable();

                        VM.InputEsc
                            .ObserveOn(RxSchedulers.MainThreadScheduler)
                            .Take(1)
                            .Subscribe(_ =>
                            {
                                // Quando riceviamo il segnale di login riuscito, navighiamo al Menu
                                InputRouter?.NavigationStack.Clear();
                                GroupEnabled = true; // Riabilitiamo il gruppo per permettere nuove navigazioni
                            }).DisposeWith(disposables);

                        VM.InputBack
                            .ObserveOn(RxSchedulers.MainThreadScheduler)
                            .Take(1)
                            .Subscribe(value =>
                            {
                                try
                                {
                                    InputRouter.NavigateBack.Execute();
                                    InputRouter.NavigationStack.Clear();
                                    AggiornaGridByInt(value);
                                    GroupEnabled = true;
                                }
                                catch (Exception ex)
                                {
                                    Debug.WriteLine($"Errore navigazione: {ex.Message}");
                                    _isClosing = false;
                                }

                                // Quando riceviamo il segnale di login riuscito, navighiamo al Menu
                                InputRouter?.NavigationStack.Clear();
                                GroupEnabled = true; // Riabilitiamo il gruppo per permettere nuove navigazioni
                            }).DisposeWith(disposables);


                        // Eseguiamo la navigazione e segnaliamo il completamento del Task
                        InputRouter.NavigateAndReset.Execute(VM)
                            .Subscribe(_ => tcs.SetResult(), ex => tcs.SetException(ex));
                    }
                    else
                    {
                        Debug.WriteLine(">>> [ERROR] Impossibile risolvere IPersonAddViewModel.");
                        tcs.SetResult();
                    }
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }

            });

            // Attendiamo che il thread della UI abbia finito l'operazione
            await tcs.Task;
        }

        private async Task GoToPersonDel(int id)
        {

            var tcs = new TaskCompletionSource();

            // 3. Risoluzione ViewModel e navigazione sul Main Thread
            RxSchedulers.MainThreadScheduler.Schedule(() =>
            {
                try
                {
                    // Nascendo qui dentro, il costruttore del LoginViewModel 
                    // viene eseguito sul thread UI, azzerando l'errore Cross-Thread!
                    var VM = Locator.Current.GetService<IPersonDelViewModel>();
                    
                    if (VM != null)
                    {

                        VM.SetHost(this); // Passiamo il riferimento alla schermata ospite al ViewModel DELL'INPUT
                        VM.SetIdDaModificare(id); // Passiamo l'ID da eliminare al ViewModel di input

                        var disposables = new CompositeDisposable();


                        VM.InputEsc
                            .ObserveOn(RxSchedulers.MainThreadScheduler)
                            .Take(1)
                            .Subscribe(_ =>
                            {
                                // Quando riceviamo il segnale di login riuscito, navighiamo al Menu
                                InputRouter?.NavigationStack.Clear();
                                GroupEnabled = true; // Riabilitiamo il gruppo per permettere nuove navigazioni
                            }).DisposeWith(disposables);


                        // Eseguiamo la navigazione e segnaliamo il completamento del Task
                        InputRouter.NavigateAndReset.Execute(VM)
                            .Subscribe(_ => tcs.SetResult(), ex => tcs.SetException(ex));
                    }
                    else
                    {
                        Debug.WriteLine(">>> [ERROR] Impossibile risolvere IPersonDelViewModel.");
                        tcs.SetResult();
                    }
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }

            });

            // Attendiamo che il thread della UI abbia finito l'operazione
            await tcs.Task;
        }

        private async Task GoToPersonUpd(int id)
        {

            var tcs = new TaskCompletionSource();

            // 3. Risoluzione ViewModel e navigazione sul Main Thread
            RxSchedulers.MainThreadScheduler.Schedule(() =>
            {
                try
                {
                    // Nascendo qui dentro, il costruttore del LoginViewModel 
                    // viene eseguito sul thread UI, azzerando l'errore Cross-Thread!
                    var VM = Locator.Current.GetService<IPersonUpdViewModel>();

                    if (VM != null)
                    {

                        VM.SetHost(this); // Passiamo il riferimento alla schermata ospite al ViewModel DELL'INPUT
                        VM.SetIdDaModificare(id); // Passiamo l'ID da eliminare al ViewModel di input

                        var disposables = new CompositeDisposable();

                        VM.InputEsc
                            .ObserveOn(RxSchedulers.MainThreadScheduler)
                            .Take(1)
                            .Subscribe(_ =>
                            {
                                // Quando riceviamo il segnale di login riuscito, navighiamo al Menu
                                InputRouter?.NavigationStack.Clear();
                                GroupEnabled = true; // Riabilitiamo il gruppo per permettere nuove navigazioni
                            }).DisposeWith(disposables);


                        // Eseguiamo la navigazione e segnaliamo il completamento del Task
                        InputRouter.NavigateAndReset.Execute(VM)
                            .Subscribe(_ => tcs.SetResult(), ex => tcs.SetException(ex));
                    }
                    else
                    {
                        Debug.WriteLine(">>> [ERROR] Impossibile risolvere IPersonUpdViewModel.");
                        tcs.SetResult();
                    }
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }

            });

            // Attendiamo che il thread della UI abbia finito l'operazione
            await tcs.Task;
        }
    }
}
