using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Threading;
using ReactiveUI;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using ViewModels;

namespace Views;

public partial class PersonGroupView : BaseUserControl<PersonGroupViewModel>
{
    protected override string RootControlName => "MainGrid";

    public PersonGroupView()
    {
        InitializeComponent();

        this.WhenActivated(d =>
        {

            SociDataGrid.LoadingRowGroup += OnLoadingRowGroup;

            Disposable.Create(() =>
            {
                SociDataGrid.LoadingRowGroup -= OnLoadingRowGroup;
                // Scolleghiamo i dati per liberare la memoria della griglia subito
                SociDataGrid.ItemsSource = null;
            }).DisposeWith(d);

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
                grid.CollapseRowGroup(group, false);
            }, DispatcherPriority.Render);
        }
    }


}