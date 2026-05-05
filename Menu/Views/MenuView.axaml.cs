using Avalonia.Controls;
using Common.InterViewModels;
using ReactiveUI;
using ReactiveUI.Avalonia;
using System.Diagnostics;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using ViewModels;

namespace Views;

public partial class MenuView : BaseUserControl<MenuViewModel>
{
    protected override string RootControlName => "RootGrid";

    public MenuView()
    {

        InitializeComponent();

        this.WhenActivated(d =>
        {
            #region TwoWay



            #endregion

            #region OneWay

            this.OneWayBind(ViewModel,
                            vm => vm.AmministratoreVisible,
                            v => v.AmministratoreItem.IsVisible)
                .DisposeWith(d);

            this.OneWayBind(ViewModel,
                            vm => vm.CassaVisible,
                            v => v.CassaItem.IsVisible)
                .DisposeWith(d);

            this.OneWayBind(ViewModel,
                            vm => vm.ChiudiGiornataEnabled,
                            v => v.CassaItem.IsEnabled)
                .DisposeWith(d);

            this.OneWayBind(ViewModel,
                            vm => vm.BarVisible,
                            v => v.BarItem.IsVisible)
                .DisposeWith(d);

            this.OneWayBind(ViewModel,
                            vm => vm.ChiudiGiornataEnabled,
                            v => v.BarItem.IsEnabled)
                .DisposeWith(d);

            this.OneWayBind(ViewModel,
                            vm => vm.PulizieVisible,
                            v => v.PulizieItem.IsVisible)
                .DisposeWith(d);

            this.OneWayBind(ViewModel,
                            vm => vm.ChiudiGiornataEnabled,
                            v => v.PulizieItem.IsEnabled)
                .DisposeWith(d);

            this.OneWayBind(ViewModel,
                            vm => vm.GuardarobaVisible,
                            v => v.GuardarobaItem.IsVisible)
                .DisposeWith(d);

            this.OneWayBind(ViewModel,
                            vm => vm.ChiudiGiornataEnabled,
                            v => v.GuardarobaItem.IsEnabled)
                .DisposeWith(d);

            this.OneWayBind(ViewModel,
                            vm => vm.ReportVisible,
                            v => v.ReportItem.IsVisible)
                .DisposeWith(d);

            this.OneWayBind(ViewModel,
                            vm => vm.ApriGiornataEnabled,
                            v => v.ApriGiornataItem.IsEnabled)
                .DisposeWith(d);

            this.OneWayBind(ViewModel,
                            vm => vm.ChiudiGiornataEnabled,
                            v => v.ChiudiGiornataItem.IsEnabled)
                .DisposeWith(d);

            this.OneWayBind(ViewModel,
                            vm => vm.ChiudiGiornataEnabled,
                            v => v.ApriTurnoItem.IsEnabled)
                .DisposeWith(d);

            this.OneWayBind(ViewModel,
                            vm => vm.ChiudiGiornataEnabled,
                            v => v.ChiudiTurnoItem.IsEnabled)
                .DisposeWith(d);

            this.OneWayBind(ViewModel,
                            vm => vm.IsMenuReady,
                            v => v.MainMenu.IsVisible)
                .DisposeWith(d);

            #endregion

            // 4. BINDING COMANDI (Se non fatti in XAML)
            //this.Bind(ViewModel, vm => vm.LogoutCommand, v => v.Title.ExitCommand).DisposeWith(d);

            
            

        });
    }


}