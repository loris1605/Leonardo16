using Avalonia;
using Avalonia.Controls;
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
                        await System.Windows.Threading.Dispatcher.CurrentDispatcher.InvokeAsync(() =>
                        {
                            TesseraBox.SelectAll();
                        }, System.Windows.Threading.DispatcherPriority.Background);
                        context.SetOutput(Unit.Default);
                    });
                });


            PosizioneBox.GetObservable(Avalonia.Controls.Control.IsEnabledProperty)
                        .Where(enabled => enabled == true) // Agisci solo quando passa da False a True
                        .ObserveOn(RxSchedulers.MainThreadScheduler)
                        .Subscribe(async _ =>
                        {
                            // Attendi la fine del ciclo di disposizione dei controlli a schermo
                            await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
                            {
                                PosizioneBox.Focus();
                                PosizioneBox.SelectAll();
                            }, Avalonia.Threading.DispatcherPriority.Background);
                        })
                        .DisposeWith(d);


            // 1. Definisci il flusso sorgente centralizzato e rendilo condiviso (.Publish().RefCount())
            var isPosizioneEnabled = this.WhenAnyValue(
                    x => x.ViewModel,
                    x => x.ViewModel!.BindingT,
                    (vm, binding) => vm != null && binding != null && binding.CodiceSocio != 0)
                .ObserveOn(RxSchedulers.MainThreadScheduler)
                .Publish()
                .RefCount(); // Mantiene il flusso attivo finché c'è almeno un BindTo collegato

            // 2. Lega il flusso direttamente a PosizioneBox
            isPosizioneEnabled
                .BindTo(this, v => v.PosizioneBox.IsEnabled)
                .DisposeWith(d);

            // 3. Inverti il flusso (Negazione logica) e legalo a TesseraBox
            isPosizioneEnabled
                .Select(enabled => !enabled)
                .BindTo(this, v => v.TesseraBox.IsEnabled)
                .DisposeWith(d);

            // --- STREAM EVENTI TASTIERA ---
            var keyUpStream = Observable.FromEventPattern<EventHandler<KeyEventArgs>, KeyEventArgs>(
                        h => this.TesseraBox.KeyUp += h,
                        h => this.TesseraBox.KeyUp -= h)
                    .ObserveOn(RxSchedulers.MainThreadScheduler)
                    .Publish()
                    .RefCount();

            // Esegui TesseraCommand su INVIO (Esegue solo se il ViewModel è istanziato)
            keyUpStream
                .Where(e => e.EventArgs.Key == Key.Enter)
                .Where(_ => ViewModel != null)
                .Select(_ => Unit.Default)
                .InvokeCommand(ViewModel!, x => x.TesseraCommand)
                .DisposeWith(d);

            // Esegui F5Command su F5 (Esegue solo se il ViewModel è istanziato)
            keyUpStream
                .Where(e => e.EventArgs.Key == Key.F5)
                .Where(_ => ViewModel != null)
                .Select(_ => Unit.Default)
                .InvokeCommand(ViewModel!, x => x.F5Command)
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

            #region TwoWays Ottimizzato con Throttle

            // 1. Dalla View al ViewModel con Throttle (Scrittura controllata)
            this.TesseraBox.GetObservable(TextBox.TextProperty) // Nota: Se usi WPF usa: Observable.FromEventPattern per il TextChanged
                .Throttle(TimeSpan.FromMilliseconds(400), RxSchedulers.MainThreadScheduler)
                .DistinctUntilChanged()
                .Where(_ => ViewModel?.BindingT != null)
                .Subscribe(testo => ViewModel!.BindingT.NumeroTessera = testo)
                .DisposeWith(d);

            // 2. Dal ViewModel alla View (Lettura standard)
            this.OneWayBind(ViewModel,
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