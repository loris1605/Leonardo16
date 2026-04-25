using Avalonia;
using ReactiveUI;
using ReactiveUI.Avalonia;
using System;
using System.Reactive;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using ViewModels;

namespace Leonardo;

public partial class InputSaveBox : ReactiveUserControl<BaseViewModel>
{
    public InputSaveBox()
    {
        InitializeComponent();

        this.WhenActivated(d =>
        {
            
            this.GetObservable(EscFocusProperty)
            .Where(x => x != null)
            .Subscribe(interaction =>
            {
                // 2. Registra l'handler: quando il ViewModel chiama .Handle(), esegui questo:
                interaction!.RegisterHandler(context =>
                {
                    // 3. Sposta il focus sul bottone fisico dentro lo UserControl
                    EsciButton.Focus();

                    context.SetOutput(Unit.Default);
                }).DisposeWith(d);
            })
            .DisposeWith(d);

        });

    }

    public void EscFocusAction()
    {
        EsciButton.Focus();
    }

    // Proprietà Comando (Usa ReactiveCommand invece di ICommand)
    public static readonly StyledProperty<ReactiveCommand<Unit, Unit>> ExitCommandProperty =
        AvaloniaProperty.Register<InputSaveBox, ReactiveCommand<Unit, Unit>>(nameof(ExitCommand));

    public ReactiveCommand<Unit, Unit> ExitCommand
    {
        get => GetValue(ExitCommandProperty);
        set => SetValue(ExitCommandProperty, value);
    }

    // Proprietà Comando (Usa ReactiveCommand invece di ICommand)
    public static readonly StyledProperty<ReactiveCommand<Unit, Unit>> SaveCommandProperty =
        AvaloniaProperty.Register<InputSaveBox, ReactiveCommand<Unit, Unit>>(nameof(SaveCommand));

    public ReactiveCommand<Unit, Unit> SaveCommand
    {
        get => GetValue(SaveCommandProperty);
        set => SetValue(SaveCommandProperty, value);
    }

    public static readonly StyledProperty<Interaction<Unit, Unit>> EscFocusProperty =
        AvaloniaProperty.Register<InputSaveBox, Interaction<Unit, Unit>>(nameof(EscFocus));

    public Interaction<Unit, Unit> EscFocus
    {
        get => GetValue(EscFocusProperty);
        set => SetValue(EscFocusProperty, value);
    }
}