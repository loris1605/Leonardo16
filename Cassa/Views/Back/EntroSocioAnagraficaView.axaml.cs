using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Leonardo;
using ReactiveUI;
using ReactiveUI.Avalonia;
using System.Reactive;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using ViewModels;

namespace Views;

public partial class EntraSocioAnagraficaView : ReactiveUserControl<BaseViewModel>
{
    public EntraSocioAnagraficaView()
    {
        InitializeComponent();

        this.WhenActivated(d =>
        {

            this.GetObservable(TesseraFocusProperty)
            .Where(x => x != null)
            .Subscribe(interaction =>
            {
                // 2. Registra l'handler: quando il ViewModel chiama .Handle(), esegui questo:
                interaction!.RegisterHandler(async context =>
                {
                    // 3. Sposta il focus sul bottone fisico dentro lo UserControl
                   await Task.Delay(100);
                   TesseraBox.Focus();
                   TesseraBox.SelectAll();
                   context.SetOutput(Unit.Default);
                }).DisposeWith(d);
            })
            .DisposeWith(d);

        });
    }



    public static readonly StyledProperty<Interaction<Unit, Unit>> TesseraFocusProperty =
        AvaloniaProperty.Register<EntraSocioAnagraficaView, Interaction<Unit, Unit>>(nameof(TesseraFocus));

    public Interaction<Unit, Unit> TesseraFocus
    {
        get => GetValue(TesseraFocusProperty);
        set => SetValue(TesseraFocusProperty, value);
    }
}