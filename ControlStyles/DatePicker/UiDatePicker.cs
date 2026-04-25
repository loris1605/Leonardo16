using Avalonia.Controls;

namespace ControlStyles
{
    public class UiDatePicker : DatePicker
    {
        protected override Type StyleKeyOverride => typeof(DatePicker);

        public UiDatePicker()
        {

            this.Classes.Add("UiDatePickerStyle");

        }
    }
}
