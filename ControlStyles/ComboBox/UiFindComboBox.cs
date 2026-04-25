using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Layout;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ControlStyles
{
    public class UiFindComboBox : UiComboBox
    {
        protected override Type StyleKeyOverride => typeof(UiComboBox);

        // 1. Registrazione della StyledProperty (Equivalente di DependencyProperty)
        public static readonly StyledProperty<EnumComboType> ComboTypeProperty =
            AvaloniaProperty.Register<UiFindComboBox, EnumComboType>(
                nameof(ComboType),
                defaultValue: EnumComboType.Null);

        // 2. Proprietà CLR (Wrapper)
        public EnumComboType ComboType
        {
            get => GetValue(ComboTypeProperty);
            set => SetValue(ComboTypeProperty, value);
        }

        // 3. Costruttore statico per registrare il cambio di valore (Coerce/Changed)
        public UiFindComboBox()
        {
            this.HorizontalContentAlignment = HorizontalAlignment.Center;
            this.VerticalContentAlignment = VerticalAlignment.Center;

            // In Avalonia il "Changed" si sottoscrive tramite l'osservabile della proprietà
            ComboTypeProperty.Changed.AddClassHandler<UiFindComboBox>((x, e) => OnComboTypeChanged(x, e));
        }

        private static void OnComboTypeChanged(UiFindComboBox sender, AvaloniaPropertyChangedEventArgs e)
        {
            // Il nuovo valore è in e.NewValue
            if (e.NewValue is EnumComboType newType)
            {
                sender.InitCombo(newType);
            }
        }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);
            // Se ComboType è stato settato da XAML, rieseguiamo Init per sicurezza
            if (ComboType != EnumComboType.Null)
            {
                InitCombo(ComboType);
            }
        }

        public class UiFindComboRecord
        {
            public UiFindComboRecord()
            {

            }

            private int _myindex;
            public int Index
            {
                get { return _myindex; }
                set { _myindex = value; }
            }

            private string _myfindtype = string.Empty;
            public string FindType
            {
                get { return _myfindtype; }
                set { _myfindtype = value; }
            }
        }

        private void InitCombo(EnumComboType type)
        {
            List<UiFindComboRecord> comboCollection = [];

            switch (type)
            {
                case EnumComboType.String:
                    comboCollection.Add(new UiFindComboRecord { Index = 0, FindType = "No Ricerca" });
                    comboCollection.Add(new UiFindComboRecord { Index = 1, FindType = "Uguale a" });
                    comboCollection.Add(new UiFindComboRecord { Index = 2, FindType = "Inizia Con" });
                    comboCollection.Add(new UiFindComboRecord { Index = 3, FindType = "Che Contiene" });
                    break;

                case EnumComboType.Numeric:
                    comboCollection.Add(new UiFindComboRecord { Index = 0, FindType = "No Ricerca" });
                    comboCollection.Add(new UiFindComboRecord { Index = 1, FindType = " = Uguale a" });
                    comboCollection.Add(new UiFindComboRecord { Index = 2, FindType = " <> Diverso da" });
                    comboCollection.Add(new UiFindComboRecord { Index = 3, FindType = " > Maggiore di" });
                    comboCollection.Add(new UiFindComboRecord { Index = 4, FindType = " < Minore di" });
                    break;

                case EnumComboType.Date:
                    comboCollection.Add(new UiFindComboRecord { Index = 0, FindType = "No Ricerca" });
                    comboCollection.Add(new UiFindComboRecord { Index = 1, FindType = "Uguale a" });
                    comboCollection.Add(new UiFindComboRecord { Index = 2, FindType = "Prima del" });
                    comboCollection.Add(new UiFindComboRecord { Index = 3, FindType = "Dopo il" });
                    break;

                case EnumComboType.Null:
                default:
                    comboCollection.Clear();
                    break;
            }

            this.ItemsSource = comboCollection;
            this.DisplayMemberBinding = new Binding("FindType");
            this.SelectedValueBinding = new Binding("Index");

            this.SelectedIndex = 0;

        }
    }
}

namespace System
{
    public enum EnumComboType
    {
        Null = -1,
        String = 0,
        Date = 1,
        Numeric = 2
    }
}


