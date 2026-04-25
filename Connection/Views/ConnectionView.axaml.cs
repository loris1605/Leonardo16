using Avalonia.Threading;
using ReactiveUI;
using System.Reactive;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using ViewModels;

namespace Views;

public partial class ConnectionView : BaseUserControl<ConnectionViewModel>
{
    protected override string RootControlName => "RootGrid";

    public ConnectionView()
    {
        InitializeComponent();

        this.WhenActivated(d =>
        {

            ViewModel?.UserIdFocus
                .RegisterHandler(async interaction =>
                {
                    await Dispatcher.UIThread.InvokeAsync(() =>
                    {
                        // Se UserNameText è un controllo personalizzato, 
                        // prova a cercare la TextBox interna se Focus() non basta
                        UserNameText.Focus();
                        UserNameText.SelectAll();
                    }, DispatcherPriority.Background); // Background è fondamentale qui

                    interaction.SetOutput(Unit.Default);
                })
                .DisposeWith(d);

            #region TwoWay

            //Bind UserIdText to TextBox
            this.Bind(ViewModel,
                      vm => vm.UserIdText,
                      v => v.UserNameText.Text)
                .DisposeWith(d);

            ////Bind PasswordText to TextBox
            this.Bind(ViewModel,
                      vm => vm.PasswordText,
                      v => v.PasswordTextBox.Text)
                .DisposeWith(d);

            ////Bind DatabaseText to TextBox
            this.Bind(ViewModel,
                      vm => vm.DatabaseText,
                      v => v.DatabaseTextBox.Text)
                .DisposeWith(d);

            // Bind SelectedItem to ComboBox
            this.Bind(ViewModel,
                      vm => vm.SelectedInstance,
                      v => v.SqlCombo.SelectedItem)
                .DisposeWith(d);


            #endregion

            #region OneWay
            //ComboBox ItemSource
            this.OneWayBind(ViewModel,
                                vm => vm.SqlInstances,
                                v => v.SqlCombo.ItemsSource)
                .DisposeWith(d);

            this.OneWayBind(ViewModel,
                            vm => vm.IsUiReady,
                            v => v.InputGrid.IsVisible,
                            isUiReady => isUiReady)
                .DisposeWith(d);


            this.OneWayBind(ViewModel,
                                vm => vm.IsUiReady,
                                v => v.InfoLabel.IsVisible,
                                isUiReady => !isUiReady)
                 .DisposeWith(d);

            this.OneWayBind(ViewModel,
                                vm => vm.IsUiReady,
                                v => v.ButtonGrid.IsVisible,
                                isUiReady => isUiReady)
                .DisposeWith(d);

            this.OneWayBind(ViewModel,
                                vm => vm.EnabledCheck,
                                v => v.CheckButton.IsEnabled,
                                l => l)
                .DisposeWith(d);

            this.OneWayBind(ViewModel,
                                vm => vm.IsLoading,
                                v => v.CheckButton.IsEnabled,
                                loading => !loading) // Converte true -> false e viceversa
                                .DisposeWith(d);

            this.OneWayBind(ViewModel,
                                vm => vm.AvviaVisibile,
                                v => v.AvviaButton.IsEnabled,
                                l => l)
                .DisposeWith(d);


            #endregion

            #region Commands

            this.BindCommand(ViewModel,
                             vm => vm.LoadCommand,
                             v => v.CercaButton).DisposeWith(d);

            this.BindCommand(ViewModel,
                             vm => vm.CheckCommand,
                             v => v.CheckButton).DisposeWith(d);

            this.BindCommand(ViewModel,
                             vm => vm.AvviaCommand,
                             v => v.AvviaButton).DisposeWith(d);

            #endregion

            //Evento DropDownClose sulla Combo
            Observable.FromEventPattern<EventHandler, EventArgs>(
                        h => SqlCombo.DropDownClosed += h,
                        h => SqlCombo.DropDownClosed -= h)
            .Subscribe(_ =>
            {
                // Rimando al dispatcher per sicurezza
                Dispatcher.UIThread.InvokeAsync(() =>
                {
                    UserNameText.Focus();
                });
            })
            .DisposeWith(d);

            
        });
    }
}