using ReactiveUI;
using System.Reactive.Disposables.Fluent;
using ViewModels;

namespace Views;

public partial class SociView : BaseUserControl<SociViewModel>
{
    protected override string RootControlName => "RootGrid";

    public SociView()
    {
        InitializeComponent();

        this.WhenActivated(d =>
        {

            #region OneWay

            this.OneWayBind(ViewModel,
                            vm => vm.GroupEnabled,
                            v => v.RouterHost.IsEnabled)
                .DisposeWith(d);

            #endregion

        });
    }

}