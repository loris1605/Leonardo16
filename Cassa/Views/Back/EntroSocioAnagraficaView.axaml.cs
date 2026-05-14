using Avalonia;
using Avalonia.Input;
using ReactiveUI;
using ReactiveUI.Avalonia;
using System.Reactive;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using ViewModels;

namespace Views;

public partial class EntraSocioAnagraficaView : ReactiveUserControl<EntraSocioViewModel>
{
    public EntraSocioAnagraficaView()
    {
        InitializeComponent();

        this.WhenActivated(d =>
        {
            var tesseraHandlerDisposable = new System.Reactive.Disposables.SerialDisposable().DisposeWith(d);
            this.GetObservable(TesseraFocusProperty)
                .Where(x => x != null)
                .Subscribe(interaction =>
                {
                    tesseraHandlerDisposable.Disposable = interaction!.RegisterHandler(async context =>
                    {
                        await Task.Delay(100);
                        TesseraBox.Focus();
                        TesseraBox.SelectAll();
                        context.SetOutput(Unit.Default);
                    });
                })
                .DisposeWith(d);

            var posizioneHandlerDisposable = new System.Reactive.Disposables.SerialDisposable().DisposeWith(d);
            this.GetObservable(PosizioneFocusProperty)
                .Where(x => x != null)
                .Subscribe(interaction =>
                {
                    posizioneHandlerDisposable.Disposable = interaction!.RegisterHandler(async context =>
                    {
                        await Task.Delay(100);
                        PosizioneBox.Focus();
                        PosizioneBox.SelectAll();
                        context.SetOutput(Unit.Default);
                    });
                })
                .DisposeWith(d);

            this.WhenAnyValue(
                    x => x.ViewModel,
                    x => x.ViewModel!.BindingT,
                    (vm, binding) => binding != null && binding.CodiceSocio != 0)
                .ObserveOn(RxSchedulers.MainThreadScheduler)
                .BindTo(this, v => v.PosizioneBox.IsEnabled)
                .DisposeWith(d);

            var keyUpStream = Observable.FromEventPattern<EventHandler<KeyEventArgs>, KeyEventArgs>(
                        h => this.TesseraBox.KeyUp += h,
                        h => this.TesseraBox.KeyUp -= h)
                    .ObserveOn(RxSchedulers.MainThreadScheduler)
                    .Publish()
                    .RefCount();

            // Esegui TesseraCommand su INVIO
            keyUpStream
                .Where(e => e.EventArgs.Key == Key.Enter)
                .Select(_ => Unit.Default)
                .InvokeCommand(ViewModel, x => x.TesseraCommand)
                .DisposeWith(d);

            // Esegui F5Command su F5
            keyUpStream
                .Where(e => e.EventArgs.Key == Key.F5)
                .Select(_ => Unit.Default)
                .InvokeCommand(ViewModel, x => x.F5Command)
                .DisposeWith(d);


            #region OneWay

            this.OneWayBind(ViewModel,
                    vm => vm.InfoLabel,
                    v => v.InfoLabel.Text)
            .DisposeWith(d);

            this.OneWayBind(ViewModel,
                    vm => vm.TesseraLabel,
                    v => v.TesseraLabel.Text)
            .DisposeWith(d);

            this.OneWayBind(ViewModel,
                    vm => vm.BindingT.Cognome,
                    v => v.CognomeBlock.Text)
            .DisposeWith(d);

            this.OneWayBind(ViewModel,
                    vm => vm.BindingT.Nome,
                    v => v.NomeBlock.Text)
            .DisposeWith(d);

            this.OneWayBind(ViewModel,
                    vm => vm.Eta,
                    v => v.EtaBlock.Text)
            .DisposeWith(d);

            this.OneWayBind(ViewModel,
                    vm => vm.BindingT.NumeroSocio,
                    v => v.NumeroSocioBlock.Text)
            .DisposeWith(d);

            #endregion

            #region TwoWays

            this.Bind(ViewModel,
                    vm => vm.BindingT.NumeroTessera,
                    v => v.TesseraBox.Text)
            .DisposeWith(d);

            #endregion

        });
    }



    public static readonly StyledProperty<Interaction<Unit, Unit>> TesseraFocusProperty =
        AvaloniaProperty.Register<EntraSocioAnagraficaView, Interaction<Unit, Unit>>(nameof(TesseraFocus));

    public Interaction<Unit, Unit> TesseraFocus
    {
        get => GetValue(TesseraFocusProperty);
        set => SetValue(TesseraFocusProperty, value);
    }

    public static readonly StyledProperty<Interaction<Unit, Unit>> PosizioneFocusProperty =
        AvaloniaProperty.Register<EntraSocioAnagraficaView, Interaction<Unit, Unit>>(nameof(PosizioneFocus));

    public Interaction<Unit, Unit> PosizioneFocus
    {
        get => GetValue(PosizioneFocusProperty);
        set => SetValue(PosizioneFocusProperty, value);
    }
}