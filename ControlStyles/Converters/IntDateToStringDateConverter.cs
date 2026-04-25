using System.Globalization;
using System.Windows.Data;

namespace ControlStyles
{
    public class IntDateToStringDateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int myvalue = (int)value;

            if (myvalue <= 0) { return String.Empty; }
            if (myvalue.ToString().Length != 8) { return DateTime.MinValue; }

            try
            {
                string strData = myvalue.ToString().Trim();

                string s1 = strData.Substring(6, 2);
                string s2 = strData.Substring(4, 2);
                string s3 = strData[..4];

                string s4 = s1 + "/" + s2 + "/" + s3;



                return s4;
            }
            catch (Exception)
            {
                return String.Empty;
                
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
