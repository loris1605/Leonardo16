using Avalonia.Controls;

namespace ControlStyles
{
    public class UiMenu : Menu
    {
        protected override Type StyleKeyOverride => typeof(Menu);

        public UiMenu()
        {
            this.Classes.Add("UiMenuStyle");
        }

    }
}
