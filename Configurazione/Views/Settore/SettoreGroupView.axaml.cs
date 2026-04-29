using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Threading;
using ReactiveUI;
using ReactiveUI.Avalonia;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using ViewModels;

namespace Views;

public partial class SettoreGroupView : BaseUserControl<SettoreGroupViewModel>
{
    protected override string RootControlName => "MainGrid";

    public SettoreGroupView()
    {
        InitializeComponent();

        this.WhenActivated(d =>
        {
            if (SettoreDataGrid != null)
            {
                SettoreDataGrid.LoadingRowGroup += OnLoadingRowGroup;
                Disposable.Create(() => SettoreDataGrid.LoadingRowGroup -= OnLoadingRowGroup)
                    .DisposeWith(d);
            }
            // Enter Key Pressed


        });
    }

    private void OnLoadingRowGroup(object sender, DataGridRowGroupHeaderEventArgs e)
    {
        if (sender is DataGrid grid && e.RowGroupHeader.DataContext is DataGridCollectionViewGroup group)
        {
            // In Avalonia 11 si usa ExpandRowGroup con 'false' per chiudere
            // Il secondo parametro 'false' indica "NON espandere" -> quindi CHIUDI
            Dispatcher.UIThread.Post(() =>
            {
                grid.CollapseRowGroup(group, true);
            }, DispatcherPriority.Render);
        }
    }
}