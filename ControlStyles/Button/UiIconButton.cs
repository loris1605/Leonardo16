using Avalonia;
using Avalonia.Controls;
using FluentIcons.Common;
using FluentIcons.Avalonia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlStyles
{
    public class UiIconButton : Button
    {

        #region TypeButton 

        public static readonly StyledProperty<ButtonType> TypeButtonProperty =
                               AvaloniaProperty.Register<UiIconButton, ButtonType>
                               (nameof(TypeButton), ButtonType.Normal);

        public ButtonType TypeButton
        {
            get => GetValue(TypeButtonProperty);
            set => SetValue(TypeButtonProperty, value);
        }

        // Nel costruttore del controllo o in OnPropertyChanged
        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);
            if (change.Property == TypeButtonProperty)
            {
                UpdateAppearance((ButtonType)change.NewValue);
            }
        }

        #endregion

        #region XLabel

        public static readonly StyledProperty<string> XLabelProperty =
                               AvaloniaProperty.Register<UiIconButton, string>
                               (nameof(XLabel), string.Empty);

        public string XLabel
        {
            get => GetValue(XLabelProperty);
            set => SetValue(XLabelProperty, value);
        }


        #endregion

        // Assicurati che IconSymbol sia di questo tipo esatto
        public static readonly StyledProperty<FluentIcons.Common.Symbol> IconSymbolProperty =
            AvaloniaProperty.Register<UiIconButton, FluentIcons.Common.Symbol>(nameof(IconSymbol));


        public Symbol IconSymbol
        {
            get => GetValue(IconSymbolProperty);
            set => SetValue(IconSymbolProperty, value);
        }

        private void UpdateAppearance(ButtonType type)
        {
            switch (type)
            {
                case ButtonType.Adding:
                    SetFluentIcon(Symbol.Add);
                    XLabel = "AGGIUNGI ";
                    break;
                case ButtonType.Delete:
                case ButtonType.Cestino:
                    SetFluentIcon(Symbol.Delete);
                    XLabel = "RIMUOVI ";
                    break;
                case ButtonType.Save:
                    SetFluentIcon(Symbol.Save);
                    XLabel = "REGISTRA ";
                    break;
                case ButtonType.Search:
                    SetFluentIcon(Symbol.Search);
                    break;
                case ButtonType.Setting:
                    SetFluentIcon(Symbol.Settings);
                    break;
                case ButtonType.Print:
                    SetFluentIcon(Symbol.Print);
                    break;
                case ButtonType.Exit:
                    SetFluentIcon(Symbol.ArrowExit);
                    XLabel = "USCITA ";
                    break;
                case ButtonType.Close:
                    SetFluentIcon(Symbol.Dismiss);
                    break;
                case ButtonType.Database:
                    SetFluentIcon(Symbol.Database);
                    break;
                    // Aggiungi gli altri case seguendo i nomi su fluenticons.co
            }
        }

        private void SetFluentIcon(Symbol symbol)
        {
            // Se il tuo controllo ha un FluentIcon interno chiamato 'Part_Icon'
            // MyFluentIconControl.Symbol = symbol;
            var iconControl = this.FindControl<SymbolIcon>("PART_Icon");
            if (iconControl != null)
            {
                iconControl.Symbol = symbol;
            }
        }

    }
}
