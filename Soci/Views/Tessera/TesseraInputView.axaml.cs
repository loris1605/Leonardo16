using Avalonia;
using Avalonia.Input;
using Avalonia.Threading;
using ReactiveUI;
using System.Reactive;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using ViewModels;

namespace Views;

public partial class TesseraInputView : BaseUserControl<TesseraInputBase>,
                                        IViewFor<TesseraAddViewModel>,
                                        IViewFor<TesseraDelViewModel>,
                                        IViewFor<TesseraUpdViewModel>
{
    protected override string RootControlName => "MainGrid";

    TesseraAddViewModel IViewFor<TesseraAddViewModel>.ViewModel
    {
        get => ViewModel as TesseraAddViewModel;
        set => ViewModel = value;
    }

    TesseraDelViewModel IViewFor<TesseraDelViewModel>.ViewModel
    {
        get => ViewModel as TesseraDelViewModel;
        set => ViewModel = value;
    }

    TesseraUpdViewModel IViewFor<TesseraUpdViewModel>.ViewModel
    {
        get => ViewModel as TesseraUpdViewModel;
        set => ViewModel = value;
    }

    public TesseraInputView()
    {
        InitializeComponent();

        this.WhenActivated(d =>
        {
            ViewModel?.NumeroTesseraFocus
                    .RegisterHandler(interaction =>
                    {
                        Dispatcher.UIThread.Post(() =>
                        {
                            NumeroTesseraBox.Focus();
                            NumeroTesseraBox.SelectAll();
                        });
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

            //Bind Numero Tessera to TextBox
            this.Bind(ViewModel,
                      vm => vm.BindingT.NumeroTessera,
                      v => v.NumeroTesseraBox.Text)
                .DisposeWith(d);


            #endregion

            #region OneWay

            this.OneWayBind(ViewModel,
                    vm => vm.Titolo,
                    v => v.lblTitolo.Text)
            .DisposeWith(d);

            this.OneWayBind(ViewModel,
                    vm => vm.Titolo1,
                    v => v.lblTitolo1.Text)
            .DisposeWith(d);

            this.OneWayBind(ViewModel,
                    vm => vm.FieldsEnabled,
                    v => v.InputGrid.IsEnabled)
            .DisposeWith(d);

            this.OneWayBind(ViewModel,
                    vm => vm.FieldsVisibile,
                    v => v.InputGrid.IsVisible)
            .DisposeWith(d);

            this.OneWayBind(ViewModel,
                    vm => vm.FieldVisibile,
                    v => v.NumeroTesseraLabel.IsVisible)
            .DisposeWith(d);

            this.OneWayBind(ViewModel,
                    vm => vm.FieldVisibile,
                    v => v.NumeroTesseraBox.IsVisible)
            .DisposeWith(d);

            this.OneWayBind(ViewModel,
                    vm => vm.InfoLabel,
                    v => v.InfoLabel.Text)
            .DisposeWith(d);


            #endregion

        });
    }
}