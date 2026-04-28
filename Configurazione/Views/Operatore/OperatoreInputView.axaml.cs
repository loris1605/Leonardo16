using Avalonia.Input;
using ReactiveUI;
using System.Reactive;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using ViewModels;

namespace Views;

public partial class OperatoreInputView : BaseUserControl<OperatoreInputBase>,
                                        IViewFor<OperatoreAddViewModel>,
                                        IViewFor<OperatoreDelViewModel>,
                                        IViewFor<OperatoreUpdViewModel>
{
    protected override string RootControlName => "MainGrid";

    OperatoreAddViewModel IViewFor<OperatoreAddViewModel>.ViewModel
    {
        get => ViewModel as OperatoreAddViewModel;
        set => ViewModel = value;
    }

    OperatoreDelViewModel IViewFor<OperatoreDelViewModel>.ViewModel
    {
        get => ViewModel as OperatoreDelViewModel;
        set => ViewModel = value;
    }

    OperatoreUpdViewModel IViewFor<OperatoreUpdViewModel>.ViewModel
    {
        get => ViewModel as OperatoreUpdViewModel;
        set => ViewModel = value;
    }

    public OperatoreInputView()
    {
        InitializeComponent();

        this.WhenActivated(d =>
            {
                ViewModel?.NomeFocus
                        .RegisterHandler(interaction =>
                        {
                            NomeBox.Focus();
                            NomeBox.SelectAll();
                            interaction.SetOutput(Unit.Default);
                        })
                        .DisposeWith(d);
                
                this.OneWayBind(ViewModel,
                    vm => vm.EscFocus,
                    view => view.InputSaveBox.EscFocus)
                .DisposeWith(d);

                
                ViewModel?.PasswordFocus
                        .RegisterHandler(interaction =>
                        {
                            PasswordBox.Focus();
                            PasswordBox.SelectAll();
                            interaction.SetOutput(Unit.Default);
                        })
                        .DisposeWith(d);

                // Esc Key Pressed
                Observable.FromEventPattern<EventHandler<KeyEventArgs>, KeyEventArgs>(
                            h => this.KeyUp += h,
                            h => this.KeyUp -= h)
                .Where(e => e.EventArgs.Key == Key.Escape)
                .ObserveOn(RxSchedulers.MainThreadScheduler)
                .Select(_ => Unit.Default) // Il comando si aspetta Unit
                .InvokeCommand(ViewModel, x => x.EscPressedCommand)
                .DisposeWith(d);

                #region TwoWay

                
                //Bind Nome to TextBox
                this.Bind(ViewModel,
                          vm => vm.BindingT.NomeOperatore,
                          v => v.NomeBox.Text)
                    .DisposeWith(d);

                //Bind Nome to TextBox
                this.Bind(ViewModel,
                          vm => vm.BindingT.Password,
                          v => v.PasswordBox.Text)
                    .DisposeWith(d);

                //Bind Nome to TextBox
                this.Bind(ViewModel,
                          vm => vm.BindingT.Badge,
                          v => v.BadgeBox.Text,
                          vmToView => vmToView.ToString(),          // Da int a string
                          viewToVm => int.TryParse(viewToVm, out var res) ? res : 0) // Da string a int
                    .DisposeWith(d);

                //Bind Nome to TextBox
                this.Bind(ViewModel,
                          vm => vm.BindingT.Abilitato,
                          v => v.AbilitatoCheckBox.IsChecked)
                    .DisposeWith(d);


                #endregion

                #region OneWay

                this.OneWayBind(ViewModel,
                        vm => vm.Titolo,
                        v => v.lblTitolo.Text)
                .DisposeWith(d);

                this.OneWayBind(ViewModel,
                        vm => vm.AbilitatoText,
                        v => v.AbilitatoText.Text)
                .DisposeWith(d);

                this.OneWayBind(ViewModel,
                        vm => vm.FieldsEnabled,
                        v => v.InputGrid.IsEnabled)
                .DisposeWith(d);

                //Bind Enabled to NomeBox
                this.OneWayBind(ViewModel,
                          vm => vm.NomeOperatoreEnabled,
                          v => v.NomeBox.IsEnabled)
                    .DisposeWith(d);

                //Bind Enabled to NomeBox
                this.OneWayBind(ViewModel,
                          vm => vm.NomeOperatoreEnabled,
                          v => v.AbilitatoCheckBox.IsEnabled)
                    .DisposeWith(d);

                
                this.OneWayBind(ViewModel,
                        vm => vm.InfoLabel,
                        v => v.InfoLabel.Text)
                .DisposeWith(d);

                #endregion

            });
    }
}