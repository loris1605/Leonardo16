using Avalonia;
using Avalonia.Input;
using ReactiveUI;
using ReactiveUI.Avalonia;
using SysNet.Converters;
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

            this.GetObservable(TesseraFocusProperty)
            .Where(x => x != null)
            .Subscribe(interaction =>
            {
                // 2. Registra l'handler: quando il ViewModel chiama .Handle(), esegui questo:
                interaction!.RegisterHandler(async context =>
                {
                    // 3. Sposta il focus sul bottone fisico dentro lo UserControl
                   await Task.Delay(100);
                   TesseraBox.Focus();
                   TesseraBox.SelectAll();
                   context.SetOutput(Unit.Default);
                }).DisposeWith(d);
            })
            .DisposeWith(d);

            this.WhenAnyValue(x => x.ViewModel.BindingT.CodiceSocio)
                            .Select(codice => codice != 0)
                            .ObserveOn(RxSchedulers.MainThreadScheduler)
                            .BindTo(this, x => x.PosizioneBox.IsEnabled)
                            .DisposeWith(d);

            Observable.FromEventPattern<EventHandler<KeyEventArgs>, KeyEventArgs>(
                            h => this.KeyUp += h,
                            h => this.KeyUp -= h)
                .Where(e => e.EventArgs.Key == Key.Enter)
                .ObserveOn(RxSchedulers.MainThreadScheduler)
                .Select(_ => Unit.Default)
                .InvokeCommand(ViewModel, x => x.TesseraCommand)
                .DisposeWith(d);


            #region OneWay

            this.OneWayBind(ViewModel,
                    vm => vm.InfoLabel,
                    v => v.InfoLabel.Text)
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
                    vm => vm.BindingT.Natoil.DateIntToEta(),
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
}