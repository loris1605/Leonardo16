using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ViewModels;

namespace Views;

public partial class EntraSocioView : BaseUserControl<EntraSocioViewModel>
{
    protected override string RootControlName => "MainGrid";

    public EntraSocioView()
    {
        InitializeComponent();
    }
}