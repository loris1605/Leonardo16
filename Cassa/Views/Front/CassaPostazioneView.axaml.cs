using Avalonia;
using Avalonia.Input;
using ReactiveUI;
using System.Reactive;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using ViewModels;

namespace Views;

public partial class CassaPostazioneView : BaseUserControl<CassaPostazioneViewModel>
{
    protected override string RootControlName => "MainGrid";

    public CassaPostazioneView()
    {
        InitializeComponent();

        this.WhenActivated(d =>
        {

            ViewModel?.PosizioneFocus
                        .RegisterHandler(interaction =>
                        {
                            PosizioneTextBox.Focus();
                            PosizioneTextBox.SelectAll();
                            interaction.SetOutput(Unit.Default);
                        })
                        .DisposeWith(d);

            // Posizione Enter Key Pressed
            this.WhenAnyValue(x => x.PosizioneTextBox)
                .WhereNotNull()
                .SelectMany(txt =>
                    // Creiamo l'osservabile manualmente sull'istanza del controllo
                    Observable.FromEventPattern<EventHandler<KeyEventArgs>, KeyEventArgs>(
                        h => txt.KeyUp += h,
                        h => txt.KeyUp -= h)
                )
                .Where(e => e.EventArgs.Key == Key.Enter) // Filtro sul tasto Enter
                .ObserveOn(RxSchedulers.MainThreadScheduler)
                .Select(_ => Unit.Default)
                .InvokeCommand(ViewModel, vm => vm.PosizioneEnterCommand)
                .DisposeWith(d);

            #region OneWay

            this.OneWayBind(ViewModel,
                            vm => vm.Titolo,
                            v => v.Title.TitoloPagina)
                .DisposeWith(d);

            this.OneWayBind(ViewModel,
                            vm => vm.IsOpen,
                            v => v.InfoStackPanel.IsVisible)
                .DisposeWith(d);

            #endregion

            #region Twoways

            this.Bind(ViewModel,
                      vm => vm.BindingT.Posizione,
                      v => v.PosizioneTextBox.Text)
                .DisposeWith(d);

            #endregion


        });
    }
}