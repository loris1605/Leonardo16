using Avalonia;
using Avalonia.Input;
using ReactiveUI;
using System.Reactive;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using ViewModels;

namespace Views;

public partial class SettoreInputView : BaseUserControl<SettoreInputBase>,
                                        IViewFor<SettoreAddViewModel>,
                                        IViewFor<SettoreUpdViewModel>,
                                        IViewFor<SettoreDelViewModel>

{
    protected override string RootControlName => "MainGrid";

    SettoreAddViewModel IViewFor<SettoreAddViewModel>.ViewModel
    {
        get => ViewModel as SettoreAddViewModel;
        set => ViewModel = value;
    }

    SettoreUpdViewModel IViewFor<SettoreUpdViewModel>.ViewModel
    {
        get => ViewModel as SettoreUpdViewModel;
        set => ViewModel = value;
    }

    SettoreDelViewModel IViewFor<SettoreDelViewModel>.ViewModel
    {
        get => ViewModel as SettoreDelViewModel;
        set => ViewModel = value;
    }

    public SettoreInputView()
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

            ViewModel?.LabelFocus
                    .RegisterHandler(interaction =>
                    {
                        EtichettaBox.Focus();
                        EtichettaBox.SelectAll();
                        interaction.SetOutput(Unit.Default);
                    })
                    .DisposeWith(d);

            this.OneWayBind(ViewModel,
                    vm => vm.EscFocus,
                    view => view.InputSaveBox.EscFocus)
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
                      vm => vm.BindingT.NomeSettore,
                      v => v.NomeBox.Text)
                .DisposeWith(d);

            //Bind Label to TextBox
            this.Bind(ViewModel,
                      vm => vm.BindingT.EtichettaSettore,
                      v => v.EtichettaBox.Text)
                .DisposeWith(d);

            //Bind SelectedValue To TipoPostazioneCombo
            this.Bind(ViewModel,
                      vm => vm.BindingT.CodiceTipoSettore,
                      v => v.TipoSettoreCombo.SelectedValue,
                      vmToView => vmToView, // Da int a object (automatico)
                      viewToVm => Convert.ToInt32(viewToVm)) // Da object a int (manuale)
                .DisposeWith(d);


            #endregion

            #region OneWay

            this.OneWayBind(ViewModel,
                    vm => vm.Titolo,
                    v => v.lblTitolo.Text)
            .DisposeWith(d);

            this.OneWayBind(ViewModel,
                    vm => vm.FieldsEnabled,
                    v => v.InputGrid.IsEnabled)
            .DisposeWith(d);


            this.OneWayBind(ViewModel,
                    vm => vm.InfoLabel,
                    v => v.InfoLabel.Text)
            .DisposeWith(d);

            #endregion

            
        });
    }
}