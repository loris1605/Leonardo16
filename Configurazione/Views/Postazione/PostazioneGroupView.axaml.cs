using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Threading;
using ReactiveUI;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using ViewModels;

namespace Views;

public partial class PostazioneGroupView : BaseUserControl<PostazioneGroupViewModel>
{
    protected override string RootControlName => "MainGrid";
    public PostazioneGroupView()
    {
        InitializeComponent();

        this.WhenActivated(d =>
        {
            if (PostazioneDataGrid != null)
            {
                PostazioneDataGrid.LoadingRowGroup += OnLoadingRowGroup;

                Disposable.Create(() => PostazioneDataGrid.LoadingRowGroup -= OnLoadingRowGroup)
                    .DisposeWith(d);
            }

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