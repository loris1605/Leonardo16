using Avalonia;
using Avalonia.Controls;

namespace ControlStyles
{
    public class UiButton : Button
    {
        //protected override Type StyleKeyOverride => typeof(Button);

        
        public UiButton()
        {
            this.Classes.Add("UiButtonStyle");
        }

        #region ToolTipText Property

        public static readonly StyledProperty<string> ToolTipTextProperty =
        AvaloniaProperty.Register<UiButton, string>(nameof(ToolTipText), defaultValue: string.Empty);

        public string ToolTipText
        {
            get => GetValue(ToolTipTextProperty);
            set => SetValue(ToolTipTextProperty, value);
        }

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == ToolTipTextProperty)
            {
                var newValue = change.GetNewValue<string>();

                if (!string.IsNullOrWhiteSpace(newValue))
                {
                    // 1. Imposta il testo reale del ToolTip di Avalonia
                    ToolTip.SetTip(this, newValue);

                    // 2. Attiva la PseudoClasse per eventuali stili XAML
                    PseudoClasses.Set(":ToolTipText", true);
                }
                else
                {
                    // 3. Rimuove il ToolTip (evita che si apra vuoto)
                    this.ClearValue(ToolTip.TipProperty);

                    // 4. Rimuove la PseudoClasse
                    PseudoClasses.Set(":ToolTipText", false);
                }
            }
        }

        #endregion
    }
}
