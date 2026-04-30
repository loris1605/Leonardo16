using Avalonia.Input;
using FluentIcons.Common.Internals;
using ReactiveUI;
using ReactiveUI.Avalonia;
using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using ViewModels;

namespace Views;

public partial class TariffaInputView : BaseUserControl<TariffaInputBase>,
                                        IViewFor<TariffaAddViewModel>,
                                        IViewFor<TariffaUpdViewModel>,
                                        IViewFor<TariffaDelViewModel>
{
    protected override string RootControlName => "MainGrid";

    TariffaAddViewModel IViewFor<TariffaAddViewModel>.ViewModel
    {
        get => ViewModel as TariffaAddViewModel;
        set => ViewModel = value;
    }

    TariffaUpdViewModel IViewFor<TariffaUpdViewModel>.ViewModel
    {
        get => ViewModel as TariffaUpdViewModel;
        set => ViewModel = value;
    }

    TariffaDelViewModel IViewFor<TariffaDelViewModel>.ViewModel
    {
        get => ViewModel as TariffaDelViewModel;
        set => ViewModel = value;
    }

    public TariffaInputView()
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

            //Bind Nome to TextBox
            this.Bind(ViewModel,
                      vm => vm.BindingT.NomeTariffa,
                      v => v.NomeBox.Text)
                .DisposeWith(d);

            //Bind Label to TextBox
            this.Bind(ViewModel,
                      vm => vm.BindingT.EtichettaTariffa,
                      v => v.EtichettaBox.Text)
                .DisposeWith(d);

            this.Bind(ViewModel,
                      vm => vm.BindingT.PrezzoTariffa,
                      v => v.PrezzoBox.Value)
                .DisposeWith(d);

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