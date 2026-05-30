using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using ReactiveUI;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;

namespace ViewModels
{
    public abstract class ViewModelBase : ReactiveObject, IRoutableViewModel, IActivatableViewModel
    {
        // ---------------------------------------------------------------------
        // 1. Proprietà e Implementazione Interfacce (Routing & Activation)
        // ---------------------------------------------------------------------
        public string UrlPathSegment { get; }
        public IScreen HostScreen { get; }
        public ViewModelActivator Activator { get; } = new();

        // ---------------------------------------------------------------------
        // 2. Comandi Reattivi (Sola Lettura per la View)
        // ---------------------------------------------------------------------
        public ReactiveCommand<Unit, Unit> LoadCommand { get; }
        public ReactiveCommand<Unit, Unit> SaveCommand { get; }
        public ReactiveCommand<Unit, Unit> EscPressedCommand { get; }
        public ReactiveCommand<Unit, Unit> AppExitCommand { get; }

        // ---------------------------------------------------------------------
        // 3. Gestione dello Stato (OAPH & Token)
        // ---------------------------------------------------------------------
        private readonly ObservableAsPropertyHelper<bool> _isLoading;
        public bool IsLoading => _isLoading.Value;

        protected bool _isClosing;
        protected CancellationTokenSource _cts;
        protected CancellationToken Token => _cts?.Token ?? CancellationToken.None;

        // ---------------------------------------------------------------------
        // 4. Condizioni di Esecuzione Sotto-Classi (Override facoltativi)
        // ---------------------------------------------------------------------
        protected virtual IObservable<bool> CanSave => Observable.Return(true);
        protected virtual IObservable<bool> CanEsc => Observable.Return(true);

        // ---------------------------------------------------------------------
        // 5. Flussi Reattivi Centralizzati
        // ---------------------------------------------------------------------
        protected virtual IObservable<bool> IsAnythingExecuting =>
            Observable.CombineLatest(
                this.WhenAnyObservable(x => x.LoadCommand.IsExecuting).StartWith(false),
                this.WhenAnyObservable(x => x.SaveCommand.IsExecuting).StartWith(false),
                this.WhenAnyObservable(x => x.EscPressedCommand.IsExecuting).StartWith(false),
                (l, s, e) => l || s || e)
            .DistinctUntilChanged();

        // ---------------------------------------------------------------------
        // Constructor
        // ---------------------------------------------------------------------
        public ViewModelBase(IScreen hostScreen = null, string urlPathSegment = default)
        {
            Debug.WriteLine($"***** [VM] {GetType().Name} {GetHashCode()} caricato *****");

            HostScreen = hostScreen;
            UrlPathSegment = urlPathSegment ?? GetType().Name;

            // Inizializzazione OAPH legata allo scheduler principale
            _isLoading = IsAnythingExecuting
                .ObserveOn(RxSchedulers.MainThreadScheduler)
                .ToProperty(this, x => x.IsLoading);

            var isCommandRunning = this.WhenAnyValue(x => x.IsLoading);

            // Regola generale: non eseguire comandi se uno di essi è già in esecuzione (con throttle)
            // Server per evitare i doppi click
            var canExecuteGeneral = isCommandRunning
                .Select(loading => !loading)
                // Evita transizioni spurie e repentine sotto i 100ms
                .Throttle(TimeSpan.FromMilliseconds(100), RxSchedulers.MainThreadScheduler)
                // FONDAMENTALE: Se un comando parte, la UI deve disabilitarsi SUBITO (0 ms) per bloccare il doppio clic
                .Merge(isCommandRunning.Where(loading => loading).Select(_ => false))
                .StartWith(true)
                .Publish()
                .RefCount();

            // Combinazione delle logiche generali con i vincoli specifici
            var canSaveEffective = canExecuteGeneral
                .CombineLatest(CanSave, (gen, s) => gen && s)
                .ObserveOn(RxSchedulers.MainThreadScheduler);

            var canEscEffective = canExecuteGeneral
                .CombineLatest(CanEsc, (gen, e) => gen && e)
                .ObserveOn(RxSchedulers.MainThreadScheduler);

            // Inizializzazione dei Comandi
            LoadCommand = ReactiveCommand.CreateFromTask(ExecuteLoading, canExecuteGeneral);
            SaveCommand = ReactiveCommand.CreateFromTask(ExecuteSaving, canSaveEffective);
            EscPressedCommand = ReactiveCommand.CreateFromTask(ExecuteEscing, canEscEffective);
            AppExitCommand = ReactiveCommand.Create(OnAppShutDown);

            TriggerGarbageCollection();

            // Gestione del Ciclo di Vita (Activation)
            this.WhenActivated(disposables =>
            {
                _cts = new CancellationTokenSource();

                // Auto-avvio del caricamento all'attivazione
                Observable.Return(Unit.Default)
                    .InvokeCommand(LoadCommand)
                    .DisposeWith(disposables);

                // Gestione centralizzata degli errori di caricamento
                LoadCommand.ThrownExceptions
                    .Subscribe(ex => Debug.WriteLine($"***** [VM] {GetType().Name} Errore nel caricamento: {ex.Message}"))
                    .DisposeWith(disposables);

                // Logica di smaltimento risorse (Teardown)
                Disposable.Create(() =>
                {
                    _cts?.Cancel();
                    _cts?.Dispose();
                    _cts = null;

                    OnFinalDestruction();
                }).DisposeWith(disposables);

                // Registrazione dei comandi nei disposables
                LoadCommand.DisposeWith(disposables);
                SaveCommand.DisposeWith(disposables);
                EscPressedCommand.DisposeWith(disposables);
                AppExitCommand.DisposeWith(disposables);
            });
        }

        // ---------------------------------------------------------------------
        // 6. Wrapper di Esecuzione Protetto (Invocati dai Comandi)
        // ---------------------------------------------------------------------
        private async Task ExecuteLoading()
        {
            if (_isClosing) return;
            try { await OnLoading(); }
            catch (OperationCanceledException) { Debug.WriteLine("Loading annullato."); }
            catch (Exception ex) { Debug.WriteLine($"ERRORE CARICAMENTO: {ex.Message}"); }
        }

        private async Task ExecuteSaving()
        {
            if (_isClosing) return;
            try { await OnSaving(); }
            catch (Exception ex) { Debug.WriteLine($"ERRORE SALVATAGGIO: {ex.Message}"); }
        }

        private async Task ExecuteEscing()
        {
            if (_isClosing) return;
            await Task.Delay(50);
            try { await OnEsc(); }
            catch (Exception ex) { Debug.WriteLine($"ERRORE ESC: {ex.Message}"); }
        }

        // ---------------------------------------------------------------------
        // 7. Metodi Virtuali di Ciclo di Vita (Hook per le classi derivate)
        // ---------------------------------------------------------------------
        protected virtual Task OnLoading() => Task.CompletedTask;
        protected virtual Task OnSaving() => Task.CompletedTask;
        protected virtual Task OnEsc() => Task.CompletedTask;

        //  CORRETTO: Deve essere void, non Task
        protected virtual void OnFinalDestruction()
        {
            TriggerGarbageCollection();
            Debug.WriteLine($"***** [VM] {GetType().Name} {GetHashCode()} rimosso dallo stack *****");
        }

        // ---------------------------------------------------------------------
        // 8. Metodi Helper Privati / Protetti
        // ---------------------------------------------------------------------
        protected void OnAppShutDown()
        {
            _isClosing = true;
            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime)
            {
                lifetime.Shutdown();
            }
        }

        protected async Task SetFocus(Interaction<Unit, Unit> focusInteraction, int delay = 100)
        {
            await TriggerInteraction(focusInteraction, Unit.Default, delay);
        }

        private static void TriggerGarbageCollection()
        {
#if DEBUG
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            GC.WaitForPendingFinalizers();
#endif
        }

#if DEBUG
        ~ViewModelBase()
        {
            Debug.WriteLine($"************************************************** [GC] {GetType().Name} {GetHashCode()} DISTRUTTO *****");
        }
#endif

        protected async Task TriggerInteraction<TInput, TOutput>(
                                                        Interaction<TInput, TOutput> interaction,
                                                        TInput input,
                                                        int delayMs = 200)
        {
            try

            {
                await Task.Delay(delayMs, Token);

                // Soluzione ultra-compatibile senza ambiguità di GetAwaiter
                await Observable.StartAsync(
                    () => interaction.Handle(input).ToTask(),
                    RxSchedulers.MainThreadScheduler
                );
            }
            catch (UnhandledInteractionException<TInput, TOutput>)
            {
                Debug.WriteLine($">>> [WARN] Interaction {typeof(TInput).Name}->{typeof(TOutput).Name} non gestita (View disattivata).");
            }
            catch (OperationCanceledException)
            {
                // Silenzioso
            }
            catch (Exception ex)
            {
                Debug.WriteLine($">>> [ERROR] Errore imprevisto nell'Interaction: {ex.Message}");
            }
        }
    }
}

