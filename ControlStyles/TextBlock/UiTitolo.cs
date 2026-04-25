using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using System.Reactive.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlStyles
{
    public class UiTitolo : TemplatedControl
    {
        //Text
        public static readonly StyledProperty<string> TextProperty =
            AvaloniaProperty.Register<UiTitolo, string>(nameof(Text));

        public string Text
        {
            get => GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }


        //SubTitolo
        public static readonly StyledProperty<bool> SubTitoloProperty =
            AvaloniaProperty.Register<UiTitolo, bool>(nameof(SubTitolo));

        public bool SubTitolo
        {
            get => GetValue(SubTitoloProperty);
            set => SetValue(SubTitoloProperty, value);
        }

        public UiTitolo()
        {
            this.GetObservable(SubTitoloProperty).Subscribe(v =>
            {
                PseudoClasses.Set(":subtitle", v);
            });
        }
    }
}
