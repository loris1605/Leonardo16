using ReactiveUI;
using System.Reactive.Disposables.Fluent;
using ViewModels;

namespace Views;

public partial class ConfigurazioneView : BaseUserControl<ConfigurazioneViewModel>
{
    protected override string RootControlName => "MainGrid";
    public ConfigurazioneView()
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