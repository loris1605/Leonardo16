using Avalonia;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using ReactiveUI;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using ViewModels;

namespace Views;

public partial class LoginView : BaseUserControl<LoginViewModel>
{
    protected override string RootControlName => "RootGrid";

    public LoginView()
    {
        InitializeComponent();

        this.WhenActivated(d =>
        {
            // BLINDARE L'ATTIVAZIONE: Eseguiamo i binding e i comandi solo quando
            // il ViewModel è realmente presente e agganciato alla View
            this.WhenAnyValue(x => x.ViewModel)
                .Where(vm => vm != null)
                .Subscribe(vm =>
                {
                    // Crea un CompositeDisposable dedicato al ciclo di vita del singolo ViewModel agganciato
                    var vmDisposables = new CompositeDisposable();

                    // 1. Gestione Focus Interaction
                    vm.PasswordFocus
                        .RegisterHandler(async interaction =>
                        {
                            await Dispatcher.UIThread.InvokeAsync(() =>
                            {
                                PasswordBox.Focus();
                                PasswordBox.SelectAll();
                            });
                            interaction.SetOutput(Unit.Default);
                        })
                        .DisposeWith(vmDisposables);

                    // 2. Gestione Tasto ESCAPE
                    Observable.FromEventPattern<KeyEventArgs>(this, nameof(this.KeyDown))
                        .Where(e => e.EventArgs.Key == Key.Escape)
                        .Select(_ => Unit.Default)
                        .InvokeCommand(vm.EscPressedCommand)
                        .DisposeWith(vmDisposables);

                    // 3. Gestione Tasto ENTER sulla PasswordBox (Bypass del blocco nativo)
                    Observable.Create<EventPattern<KeyEventArgs>>(observer =>
                    {
                        void handler(object s, KeyEventArgs e) => observer.OnNext(new EventPattern<KeyEventArgs>(s, e));

                        // CORRETTO: Usiamo RoutingStrategies.Tunneling (con la -ing finale)
                        PasswordBox.AddHandler(InputElement.KeyDownEvent, handler, RoutingStrategies.Tunnel, true);

                        return Disposable.Create(() => PasswordBox.RemoveHandler(InputElement.KeyDownEvent, handler));
                    })
                    .Where(e => e.EventArgs.Key == Key.Enter)
                    .Select(_ => Unit.Default)
                    .InvokeCommand(vm.SaveCommand) // Ora sicuro perché 'vm' non è nullo
                    .DisposeWith(vmDisposables);

                    // 4. BINDING REATTIVI
                    this.Bind(vm, viewModel => viewModel.PasswordText, view => view.PasswordBox.Text)
                        .DisposeWith(vmDisposables);

                    this.Bind(vm, viewModel => viewModel.BindingT, view => view.OperatoreCombo.SelectedItem)
                        .DisposeWith(vmDisposables);

                    // Pulisce tutto se il ViewModel cambia o la View viene disattivata
                    vmDisposables.DisposeWith(d);
                })
                .DisposeWith(d);

            // 5. EVENTO COMBO BOX (Indipendente dal ViewModel, legato solo alla View)
            Observable.FromEventPattern<EventHandler, EventArgs>(
                        h => OperatoreCombo.DropDownClosed += h,
                        h => OperatoreCombo.DropDownClosed -= h)
                .Select(_ => Unit.Default)
                .Subscribe(async _ =>
                {
                    await Dispatcher.UIThread.InvokeAsync(() =>
                    {
                        PasswordBox.Focus();
                        PasswordBox.SelectAll();
                    });
                })
                .DisposeWith(d);
        });
    }


}