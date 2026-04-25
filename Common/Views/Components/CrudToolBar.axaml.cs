using Avalonia;
using ReactiveUI;
using ReactiveUI.Avalonia;
using System.Reactive;
using ViewModels;

namespace Views;

public partial class CrudToolBar : BaseUserControl<BaseViewModel>
{
    protected override string RootControlName => "MainRoot";

    public CrudToolBar()
    {
        InitializeComponent();

        this.WhenActivated(d =>
        {
            
        });
    }

    public static readonly StyledProperty<ReactiveCommand<Unit,Unit>> AddCommandProperty =
        AvaloniaProperty.Register<CrudToolBar, ReactiveCommand<Unit, Unit>>(nameof(AddCommand));

    public ReactiveCommand<Unit, Unit> AddCommand
    { 
        get => GetValue(AddCommandProperty); 
        set => SetValue(AddCommandProperty, value);
    }

    public static readonly StyledProperty<ReactiveCommand<Unit, Unit>> UpdCommandProperty =
        AvaloniaProperty.Register<CrudToolBar, ReactiveCommand<Unit, Unit>>(nameof(UpdCommand));

    public ReactiveCommand<Unit, Unit> UpdCommand
    {
        get => GetValue(UpdCommandProperty);
        set => SetValue(UpdCommandProperty, value);
    }

    public static readonly StyledProperty<ReactiveCommand<Unit, Unit>> DelCommandProperty =
        AvaloniaProperty.Register<CrudToolBar, ReactiveCommand<Unit, Unit>>(nameof(DelCommand));

    public ReactiveCommand<Unit, Unit> DelCommand
    {
        get => GetValue(DelCommandProperty);
        set => SetValue(DelCommandProperty, value);
    }

    public static readonly StyledProperty<ReactiveCommand<Unit, Unit>> CancelFilterCommandProperty =
        AvaloniaProperty.Register<CrudToolBar, ReactiveCommand<Unit, Unit>>(nameof(CancelFilterCommand));

    public ReactiveCommand<Unit, Unit> CancelFilterCommand
    {
        get => GetValue(CancelFilterCommandProperty);
        set => SetValue(CancelFilterCommandProperty, value);
    }

    public static readonly StyledProperty<ReactiveCommand<Unit, Unit>> FilterCommandProperty =
        AvaloniaProperty.Register<CrudToolBar, ReactiveCommand<Unit, Unit>>(nameof(FilterCommand));

    public ReactiveCommand<Unit, Unit> FilterCommand
    {
        get => GetValue(FilterCommandProperty);
        set => SetValue(FilterCommandProperty, value);
    }

    public static readonly StyledProperty<bool> FilterCommandVisibileProperty =
        AvaloniaProperty.Register<CrudToolBar, bool>(nameof(FilterCommandVisibile), defaultValue : false);

    public bool FilterCommandVisibile
    {
        get => GetValue(FilterCommandVisibileProperty);
        set => SetValue(FilterCommandVisibileProperty, value);
    }

    public static readonly StyledProperty<string> GroupTextProperty =
        AvaloniaProperty.Register<CrudToolBar, string>(nameof(GroupText), defaultValue: string.Empty);

    public string GroupText
    {
        get => GetValue(GroupTextProperty);
        set => SetValue(GroupTextProperty, value);
    }
}