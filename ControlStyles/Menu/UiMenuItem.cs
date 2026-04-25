using Avalonia.Controls;

namespace ControlStyles
{
    public class UiMenuItem : MenuItem
    {
        protected override Type StyleKeyOverride => typeof(MenuItem);

        public UiMenuItem()
        {
            this.Classes.Add("UiMenuItemStyle");
        }
    }
}
