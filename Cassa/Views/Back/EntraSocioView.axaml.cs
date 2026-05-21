using ReactiveUI;
using System.Reactive.Disposables.Fluent;
using ViewModels;

namespace Views;

public partial class EntraSocioView : BaseUserControl<EntraSocioViewModel>
{
    protected override string RootControlName => "MainGrid";

    public EntraSocioView()
    {
        InitializeComponent();

        this.WhenActivated(d =>
        {

            this.OneWayBind(ViewModel,
                vm => vm.TesseraFocus,
                view => view.AnagraficaInput.TesseraFocus)
            .DisposeWith(d);
        });
    }
}