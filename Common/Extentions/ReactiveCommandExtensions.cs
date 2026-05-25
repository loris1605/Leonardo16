using ReactiveUI;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;



public static class ReactiveCommandExtensions
{
    /// <summary>
    /// Crea un ReactiveCommand protetto dai doppi click ravvicinati.
    /// Il comando si disabilita istantaneamente al primo click per il tempo impostato.
    /// </summary>
    public static ReactiveCommand<TInput, TOutput> CreateSafeFromTask<TInput, TOutput>(
        Func<TInput, Task<TOutput>> execute,
        IObservable<bool> canExecute = null,
        int timeoutMilliseconds = 500)
    {
        // Soggetto per gestire lo stato di cooldown (blocco temporaneo temporale)
        var isCoolingDown = new BehaviorSubject<bool>(false);

        // Combina il canExecute originale dell'utente con lo stato di cooldown
        var combinedCanExecute = isCoolingDown
            .Select(cooling => !cooling)
            .CombineLatest(canExecute ?? Observable.Return(true), (noCool, userCanExec) => noCool && userCanExec);

        ReactiveCommand<TInput, TOutput> command = null;

        // Creiamo il comando intercettando l'esecuzione per iniettare il blocco temporale
        command = ReactiveCommand.CreateFromTask<TInput, TOutput>(async input =>
        {
            isCoolingDown.OnNext(true);
            try
            {
                return await execute(input);
            }
            finally
            {
                // Avvia il timer di sblocco dopo il click
                Observable.Timer(TimeSpan.FromMilliseconds(timeoutMilliseconds), RxSchedulers.MainThreadScheduler)
                    .Subscribe(_ => isCoolingDown.OnNext(false));
            }
        }, combinedCanExecute);

        return command;
    }

    /// <summary>
    /// Overload semplificato per i comandi asincroni più comuni senza parametri (Unit, Unit).
    /// </summary>
    public static ReactiveCommand<Unit, Unit> CreateSafeFromTask(
        Func<Task> execute,
        IObservable<bool> canExecute = null,
        int timeoutMilliseconds = 500)
    {
        return CreateSafeFromTask<Unit, Unit>(
            async _ => { await execute(); return Unit.Default; },
            canExecute,
            timeoutMilliseconds);
    }
}

