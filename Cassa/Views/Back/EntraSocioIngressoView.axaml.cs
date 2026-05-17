using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ReactiveUI.Avalonia;
using ViewModels;

namespace Views;

public partial class EntraSocioIngressoView : ReactiveUserControl<EntraSocioViewModel>
{
    public EntraSocioIngressoView()
    {
        InitializeComponent();
    }
}