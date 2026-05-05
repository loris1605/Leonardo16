using Avalonia.Input;
using Avalonia.Threading;
using ReactiveUI;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using ViewModels;

namespace Views;

public partial class PersonSearchView : BaseUserControl<PersonSearchViewModel>
{
    protected override string RootControlName => "MainGrid";

    public PersonSearchView()
    {
        InitializeComponent();

        this.WhenActivated(d =>
        {

            ViewModel?.CognomeFocus
                    .RegisterHandler(interaction =>
                    {
                        Dispatcher.UIThread.Post(() =>
                        {
                            CognomeBox.Focus();
                            CognomeBox.SelectAll();
                        });
                        interaction.SetOutput(Unit.Default);
                    })
                    .DisposeWith(d);

            ViewModel?.NomeFocus
                    .RegisterHandler(interaction =>
                    {
                        Dispatcher.UIThread.Post(() =>
                        {
                            NomeBox.Focus();
                            NomeBox.SelectAll();
                        });
                        interaction.SetOutput(Unit.Default);
                    })
                    .DisposeWith(d);

            ViewModel?.SocioFocus
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

            ViewModel?.TesseraFocus
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

            ViewModel?.EscFocus
                .RegisterHandler(interaction =>
                {
                    Dispatcher.UIThread.Post(() =>
                    {
                        InputSaveBox.EscFocusAction();
                        interaction.SetOutput(Unit.Default);
                    });
                    
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


            // Enter Key Pressed

            #region TwoWay

            //Bind Cognome to TextBox
            this.Bind(ViewModel,
                      vm => vm.BindingT.Cognome,
                      v => v.CognomeBox.Text)
                .DisposeWith(d);

            //Bind SelectedValue to CognomeCombo
            this.Bind(ViewModel,
                      vm => vm.CognomeSelectedValue,
                      v => v.CognomeCombo.SelectedValue)
                .DisposeWith(d);

            //Bind IsChecked to AnagraficaRadioButton
            this.Bind(ViewModel,
                      vm => vm.AnagraficaChecked,
                      v => v.AnagraficaRadioButton.IsChecked)
                .DisposeWith(d);

            //Bind IsChecked to SocioRadioButton
            this.Bind(ViewModel,
                      vm => vm.SocioChecked,
                      v => v.SocioRadioButton.IsChecked)
                .DisposeWith(d);

            //Bind IsChecked to SocioRadioButton
            this.Bind(ViewModel,
                      vm => vm.TesseraChecked,
                      v => v.TesseraRadioButton.IsChecked)
                .DisposeWith(d);

            //Bind Nome to TextBox
            this.Bind(ViewModel,
                      vm => vm.BindingT.Nome,
                      v => v.NomeBox.Text)
                .DisposeWith(d);

            //Bind SelectedValue to CognomeCombo
            this.Bind(ViewModel,
                      vm => vm.NomeSelectedValue,
                      v => v.NomeCombo.SelectedValue)
                .DisposeWith(d);

            //Bind DataNascitaOffset to DataNascitaPicker
            this.Bind(ViewModel,
                      vm => vm.DataNascitaOffSet,
                      v => v.DataNascitaPicker.SelectedDate)
                .DisposeWith(d);

            //Bind SelectedValue to NatoilCombo
            this.Bind(ViewModel,
                      vm => vm.NatoilSelectedValue,
                      v => v.NatoilCombo.SelectedValue)
                .DisposeWith(d);

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
                    vm => vm.AnagraficaChecked,
                    v => v.CognomeBox.IsEnabled)
            .DisposeWith(d);

            this.OneWayBind(ViewModel,
                    vm => vm.AnagraficaChecked,
                    v => v.CognomeCombo.IsEnabled)
            .DisposeWith(d);

            this.OneWayBind(ViewModel,
                    vm => vm.AnagraficaChecked,
                    v => v.NomeBox.IsEnabled)
            .DisposeWith(d);

            this.OneWayBind(ViewModel,
                    vm => vm.AnagraficaChecked,
                    v => v.NomeCombo.IsEnabled)
            .DisposeWith(d);

            this.OneWayBind(ViewModel,
                    vm => vm.AnagraficaChecked,
                    v => v.DataNascitaPicker.IsEnabled)
            .DisposeWith(d);

            this.OneWayBind(ViewModel,
                    vm => vm.AnagraficaChecked,
                    v => v.NatoilCombo.IsEnabled)
            .DisposeWith(d);

            this.OneWayBind(ViewModel,
                    vm => vm.AnagraficaChecked,
                    v => v.NatoilCombo.IsEnabled)
            .DisposeWith(d);

            this.OneWayBind(ViewModel,
                    vm => vm.SocioChecked,
                    v => v.CodiceSocioBox.IsEnabled)
            .DisposeWith(d);

            this.OneWayBind(ViewModel,
                    vm => vm.TesseraChecked,
                    v => v.NumeroTesseraBox.IsEnabled)
            .DisposeWith(d);


            this.OneWayBind(ViewModel,
                    vm => vm.InfoLabel,
                    v => v.InfoLabel.Text)
            .DisposeWith(d);

            #endregion

            

            //Evento DropDownClose sulla Combo Cognome
            Observable.FromEventPattern<EventHandler, EventArgs>(
                        h => CognomeCombo.DropDownClosed += h,
                        h => CognomeCombo.DropDownClosed -= h)
            .Subscribe(_ =>
            {
                // Rimando al dispatcher per sicurezza
                Dispatcher.UIThread.InvokeAsync(() =>
                {
                    CognomeBox.Focus();
                    CognomeBox.SelectAll();
                    if ((int)CognomeCombo.SelectedValue! == 0)
                    {
                        CognomeBox.Text = String.Empty;
                    }
                });
            })
            .DisposeWith(d);

            //Evento DropDownClose sulla Combo Nome
            Observable.FromEventPattern<EventHandler, EventArgs>(
                        h => NomeCombo.DropDownClosed += h,
                        h => NomeCombo.DropDownClosed -= h)
            .Subscribe(_ =>
            {
                // Rimando al dispatcher per sicurezza
                Dispatcher.UIThread.InvokeAsync(() =>
                {
                    NomeBox.Focus();
                    NomeBox.SelectAll();
                    if ((int)NomeCombo.SelectedValue! == 0)
                    {
                        NomeBox.Text = String.Empty;
                    }
                });
            })
            .DisposeWith(d);

            //Evento DropDownClose sulla Combo Cognome
            Observable.FromEventPattern<EventHandler, EventArgs>(
                        h => NatoilCombo.DropDownClosed += h,
                        h => NatoilCombo.DropDownClosed -= h)
            .Subscribe(_ =>
            {
                // Rimando al dispatcher per sicurezza
                Dispatcher.UIThread.InvokeAsync(() =>
                {
                    DataNascitaPicker.Focus();
                });
            })
            .DisposeWith(d);

            Disposable.Create(() => {
                this.DataContext = null;
                System.Diagnostics.Debug.WriteLine(">>> [VIEW] PersonSearchView deattivata, DataContext rimosso.");
            }).DisposeWith(d);

        });
    }
}