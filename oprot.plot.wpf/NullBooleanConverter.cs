using System;
using System.Windows.Data;
using System.Globalization;

namespace oprot.plot.wpf
{
    [ValueConversion(typeof(bool), typeof(bool))]
    public class NullBooleanConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                return true;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                return true;
            }
            return false;
        }
    }
}
