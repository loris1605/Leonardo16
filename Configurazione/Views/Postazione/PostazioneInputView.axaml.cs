using Avalonia;
using Avalonia.Input;
using ReactiveUI;
using System.Reactive;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using ViewModels;

namespace Views;

public partial class PostazioneInputView : BaseUserControl<PostazioneInputBase>,
                                        IViewFor<PostazioneAddViewModel>,
                                        IViewFor<PostazioneDelViewModel>,
                                        IViewFor<PostazioneUpdViewModel>
{
    protected override string RootControlName => "MainGrid";

    PostazioneAddViewModel IViewFor<PostazioneAddViewModel>.ViewModel
    {
        get => ViewModel as PostazioneAddViewModel;
        set => ViewModel = value;
    }
    
    PostazioneDelViewModel IViewFor<PostazioneDelViewModel>.ViewModel
    {
        get => ViewModel as PostazioneDelViewModel;
        set => ViewModel = value;
    }

    PostazioneUpdViewModel IViewFor<PostazioneUpdViewModel>.ViewModel
    {
        get => ViewModel as PostazioneUpdViewModel;
        set => ViewModel = value;
    }



    public PostazioneInputView()
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
                      vm => vm.BindingT.NomePostazione,
                      v => v.NomeBox.Text)
                .DisposeWith(d);


            //Bind SelectedValue To TipoPostazioneCombo
            this.Bind(ViewModel,
                      vm => vm.BindingT.CodiceTipoPostazione,
                      v => v.TipoPostazioneCombo.SelectedValue,
                      vmToView => vmToView, // Da int a object (automatico)
                      viewToVm => Convert.ToInt32(viewToVm)) // Da object a int (manuale)
                .DisposeWith(d);

            this.Bind(ViewModel,
                      vm => vm.BindingT.CodiceTipoRientro,
                      v => v.TipoRientroCombo.SelectedValue,
                      vmToView => vmToView,
                      viewToVm => Convert.ToInt32(viewToVm))
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
           

            //Bind TipoRientroCombo IsEnabled to RientroVisibile
            this.OneWayBind(ViewModel,
                    vm => vm.RientroVisibile,
                    v => v.TipoRientroCombo.IsVisible)
            .DisposeWith(d);

            this.OneWayBind(ViewModel,
                    vm => vm.RientroVisibile,
                    v => v.TipoRientroLabel.IsVisible)
            .DisposeWith(d);

            

            this.OneWayBind(ViewModel,
                    vm => vm.InfoLabel,
                    v => v.InfoLabel.Text)
            .DisposeWith(d);

            #endregion

            
        });
    }
}