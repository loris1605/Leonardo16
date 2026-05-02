using ViewModels;

namespace Views;

public partial class CassaView : BaseUserControl<CassaViewModel>
{
    protected override string RootControlName => "MainGrid";

    public CassaView()
    {
        InitializeComponent();
    }
}