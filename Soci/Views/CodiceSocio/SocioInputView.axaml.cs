using Avalonia.Input;
using Avalonia.Threading;
using ReactiveUI;
using System.Reactive;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using ViewModels;

namespace Views;

public partial class SocioInputView : BaseUserControl<CodiceSocioInputBase>,
                                        IViewFor<CodiceSocioAddViewModel>,
                                        IViewFor<CodiceSocioDelViewModel>,
                                        IViewFor<CodiceSocioUpdViewModel>

{
    protected override string RootControlName => "MainGrid";

    CodiceSocioAddViewModel IViewFor<CodiceSocioAddViewModel>.ViewModel
    {
        get => ViewModel as CodiceSocioAddViewModel;
        set => ViewModel = value;
    }

    CodiceSocioDelViewModel IViewFor<CodiceSocioDelViewModel>.ViewModel
    {
        get => ViewModel as CodiceSocioDelViewModel;
        set => ViewModel = value;
    }

    CodiceSocioUpdViewModel IViewFor<CodiceSocioUpdViewModel>.ViewModel
    {
        get => ViewModel as CodiceSocioUpdViewModel;
        set => ViewModel = value;
    }

    public SocioInputView()
    {
        InitializeComponent();

        this.WhenActivated(d =>
        {
            ViewModel?.NumeroSocioFocus
                    .RegisterHandler(interaction =>
                    {
                        Dispatcher.UIThread.Post(() =>
                        {
                            CodiceSocioBox.Focus();
                            CodiceSocioBox.SelectAll();
                        });
                        interaction.SetOutput(Unit.Default);
                    })
                    .DisposeWith(d);

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

            //Bind Codice Socio to TextBox
            this.Bind(ViewModel,
                      vm => vm.BindingT.NumeroSocio,
                      v => v.CodiceSocioBox.Text)
                .DisposeWith(d);

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