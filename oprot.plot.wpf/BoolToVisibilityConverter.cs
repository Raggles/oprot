using OxyPlot;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using Windows.Media.Core;

namespace oprot.plot.wpf
{
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public sealed class BoolToVisibilityConverter : IValueConverter
    {
        public Visibility TrueValue { get; set; }
        public Visibility FalseValue { get; set; }

        public BoolToVisibilityConverter()
        {
            // set defaults
            TrueValue = Visibility.Visible;
            FalseValue = Visibility.Collapsed;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is bool))
                return null;
            return (bool)value ? TrueValue : FalseValue;
        }

        public object ConvertBack(object value, Type targetType,object parameter, CultureInfo culture)
        {
            if (Equals(value, TrueValue))
                return true;
            if (Equals(value, FalseValue))
                return false;
            return null;
        }
    }

    [ValueConversion(typeof(OxyColor), typeof(System.Windows.Media.Color))]
    public sealed class OxyColorToColorConverter : IValueConverter
    {

        public OxyColorToColorConverter() { }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is OxyColor))
                return null;
            var c = (OxyColor)value;
            return new System.Windows.Media.Color
            {
                A = c.A,
                R = c.R,
                G = c.G,
                B = c.B
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(!(value is Color))
                return null;
            var c = (System.Windows.Media.Color)value;
            return OxyColor.FromArgb(c.A, c.R, c.G, c.B);
        }
    }

}
