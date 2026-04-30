using ViewModels;

namespace Views;

public partial class TariffaGroupView : BaseUserControl<TariffaGroupViewModel>
{
    protected override string RootControlName => "MainGrid";

    public TariffaGroupView()
    {
        InitializeComponent();
       

    }

}