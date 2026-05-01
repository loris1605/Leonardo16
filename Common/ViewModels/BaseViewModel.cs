using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using ReactiveUI;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;

namespace ViewModels
{
    public abstract class BaseViewModel : ReactiveObject, IRoutableViewModel, IActivatableViewModel
    {
        //Sequenza di operazioni
        //Eliminare il public override string UrlPathSegment => "";
        //Eliminare il static int deadentries;
        //Eliminare il public ReactiveCommand<Unit, Unit> LoadCommand { get; }

        // Implementazione di IRoutableViewModel
        public string UrlPathSegment { get; }
        public IScreen HostScreen { get; }
        public ViewModelActivator Activator { get; } = new();

        protected bool _isClosing = false;
        protected CancellationTokenSource _cts;
        protected CancellationToken token => _cts?.Token ?? CancellationToken.None;

        // Implementazione di IActivatableViewModel
        
        public ReactiveCommand<Unit, Unit> AppExitCommand { get; }
        public ReactiveCommand<Unit, Unit> LoadCommand { get; }
        public ReactiveCommand<Unit, Unit> EscPressedCommand { get; set; }
        public ReactiveCommand<Unit, Unit> SaveCommand { get; }

        // Condizioni per l'esecuzione
        protected virtual IObservable<bool> canSave => Observable.Return(true);
        protected virtual IObservable<bool> canEsc => Observable.Return(true);

        // 1. Rendiamo IsLoading un Helper "pigro" (OAPH)
        protected ObservableAsPropertyHelper<bool> _isLoading;
        public bool IsLoading => _isLoading?.Value ?? false;

        // Observable centralizzato per lo stato dei comandi
        protected virtual IObservable<bool> IsAnythingExecuting =>
            Observable.CombineLatest(
                this.WhenAnyObservable(x => x.LoadCommand.IsExecuting).StartWith(false),
                this.WhenAnyObservable(x => x.SaveCommand.IsExecuting).StartWith(false),
                this.WhenAnyObservable(x => x.EscPressedCommand.IsExecuting).StartWith(false),
                (l, s, e) => l || s || e)
            .DistinctUntilChanged();


        public BaseViewModel(IScreen hostScreen = null, string urlPathSegment = default)
        {
            Debug.WriteLine($"***** [VM] {this.GetType().Name} {this.GetHashCode()} caricato *****");

            HostScreen = hostScreen;
            UrlPathSegment = urlPathSegment ?? this.GetType().Name;

            _isLoading = IsAnythingExecuting
                .ToProperty(this, x => x.IsLoading, scheduler: RxSchedulers.MainThreadScheduler);

            // Un solo Observable per lo stato di caricamento con throttle
            var canExecuteGeneral = this.WhenAnyValue(x => x.IsLoading)
                .Select(loading => !loading)
                .Throttle(TimeSpan.FromMilliseconds(100), RxSchedulers.MainThreadScheduler)
                .StartWith(true)
                .Publish().RefCount();

            // B. Uniamo la logica generale con le condizioni specifiche (canSave, canEsc)
            var canSaveEffective = canExecuteGeneral
                .CombineLatest(canSave, (gen, s) => gen && s)
                .ObserveOn(RxSchedulers.MainThreadScheduler);

            //// 3. Applichiamo il ritardo all'Esc (e correggiamo il riferimento a canEsc)
            var canEscEffective = canExecuteGeneral
                .CombineLatest(canEsc, (gen, e) => gen && e)
                .ObserveOn(RxSchedulers.MainThreadScheduler);

            LoadCommand = ReactiveCommand.CreateFromTask( () => ExecuteLoading(), canExecuteGeneral);
            
            SaveCommand = ReactiveCommand.CreateFromTask(ExecuteSaving, canSaveEffective);
            AppExitCommand = ReactiveCommand.Create(OnAppShutDown);
            EscPressedCommand = ReactiveCommand.CreateFromTask(ExecuteEscing, canEscEffective);
            

#if DEBUG

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            GC.WaitForPendingFinalizers();

#endif

            // Gestione dell'attivazione/disattivazione
            this.WhenActivated(disposables =>
            {
                _cts = new CancellationTokenSource();

                Observable.Return(Unit.Default)
                .InvokeCommand(LoadCommand)
                .DisposeWith(disposables);

                
                LoadCommand.ThrownExceptions
                    .Subscribe(ex =>
                    {
                        // Qui gestisci l'errore (es. mostri una notifica o logghi)
                        Debug.WriteLine($"***** [VM] {this.GetType().Name} Errore durante il caricamento: {ex.Message}");
                   
                    })
                .DisposeWith(disposables);

                
                Disposable.Create(() => {

                    var cts = _cts;
                    if (cts != null)
                    {
                        cts.Cancel();
                        cts.Dispose();
                        _cts = null;
                    }

                    OnFinalDestruction();
#if DEBUG
                    Debug.WriteLine($"***** [VM] {this.GetType().Name} {this.GetHashCode()} disposed *****");
#endif
                }).DisposeWith(disposables);

                AppExitCommand?.DisposeWith(disposables);
                SaveCommand?.DisposeWith(disposables);
                LoadCommand?.DisposeWith(disposables);
                EscPressedCommand?.DisposeWith(disposables);
            });
        }

        protected void InitializeLoadingHelper()
        {
            // Smaltisce il vecchio helper se esiste (opzionale ma pulito)
            _isLoading?.Dispose();

            _isLoading = IsAnythingExecuting
                .ObserveOn(RxSchedulers.MainThreadScheduler)
                .ToProperty(this, x => x.IsLoading);
        }

        protected async Task ExecuteLoading()
        {
            // Se stiamo già caricando o chiudendo, usciamo
            if (_isClosing) return;

            try { await OnLoading(); }
            catch (OperationCanceledException) { Debug.WriteLine("Loading annullato."); }
            catch (Exception ex) { Debug.WriteLine($"ERRORE CARICAMENTO: {ex.Message}"); }

        }


        protected async Task ExecuteSaving()
        {
            if (_isClosing) return;
            try { await OnSaving(); }
            catch (Exception ex) { Debug.WriteLine($"ERRORE SALVATAGGIO: {ex.Message}"); }
        }


        protected async Task ExecuteEscing()
        {
            if (_isClosing) return;
            await Task.Delay(50);
            try { await OnEsc(); }
            catch (Exception ex) { Debug.WriteLine($"ERRORE ESC: {ex.Message}"); }
        }


        protected virtual void OnFinalDestruction()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            // Questo log conferma che la LOGICA di rimozione ha funzionato
            Debug.WriteLine($"***** [VM] {this.GetType().Name} {this.GetHashCode()}  +" +
                            $"rimosso correttamente dallo stack *****");
        }

#if DEBUG
        ~BaseViewModel()
        {
            // Questo apparirà nella finestra "Output" di Visual Studio
            Debug.WriteLine($"************************************************** [GC] {this.GetType().Name} {this.GetHashCode()} DISTRUTTO *****");
        }
#endif

        protected virtual Task OnLoading() => Task.CompletedTask;
        protected virtual Task OnSaving() => Task.CompletedTask;
        protected virtual Task OnEsc() => Task.CompletedTask;

        protected void OnAppShutDown()
        {
            _isClosing = true; // Impedisce ulteriori operazioni durante la chiusura
            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime)
                lifetime.Shutdown();
            
        }

        protected async Task SetFocus(Interaction<Unit, Unit> focusInteraction, int delay = 100)
        {
            // Chiama il tuo metodo esistente passando Unit.Default come input
            await TriggerInteraction(focusInteraction, Unit.Default, delay);
        }


        protected async Task TriggerInteraction<TInput, TOutput>(
                Interaction<TInput, TOutput> interaction,
                TInput input,
                int delayMs = 200)
            {
                // Attendiamo che la View sia agganciata e pronta
                

            try
            {
            await Task.Delay(delayMs, token);
            // Verifichiamo se l'interazione ha almeno un handler registrato
            // per evitare eccezioni se la View è già stata deattivata
            await Observable.StartAsync(async () => await interaction.Handle(input), RxSchedulers.MainThreadScheduler);
            }
            catch (UnhandledInteractionException<TInput, TOutput>)
            {
            Debug.WriteLine($">>> [WARN] Interaction {typeof(TInput).Name}->{typeof(TOutput).Name} non gestita (View deattivata).");
            }
            catch (OperationCanceledException)
            {
                // Silenzioso: la VM è stata chiusa durante il delay, tutto normale.
            }
            catch (Exception ex)
                {
                    Debug.WriteLine($">>> [ERROR] Errore Interaction: {ex.Message}");
                }
            }

        

    }
}
