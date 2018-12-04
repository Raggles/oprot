using OxyPlot;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace oprot.plot.wpf
{
    [ValueConversion(typeof(OxyColor), typeof(Brush))]
    public sealed class OxyColorToBrushConverter : IValueConverter
    {
        public OxyColorToBrushConverter() { }
     
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is OxyColor))
                return null;
            var o = (OxyColor)value;
            return new SolidColorBrush(Color.FromArgb(o.A, o.R, o.G, o.B));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is SolidColorBrush))
                return null;
            var o = (SolidColorBrush)value;
            return OxyColor.FromArgb(o.Color.A, o.Color.R, o.Color.G, o.Color.B);
        }
    }


    [ValueConversion(typeof(OxyColor), typeof(Brush))]
    public sealed class OxyColorToBrushFadedConverter : IValueConverter
    {
        public OxyColorToBrushFadedConverter() { }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is OxyColor))
                return null;
            var o = (OxyColor)value;
            
            o = o.ChangeIntensity(4);
            o = o.ChangeSaturation(0.25);
            o = OxyColor.FromAColor(127, o);
            return new SolidColorBrush(Color.FromArgb(o.A, o.R, o.G, o.B));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is SolidColorBrush))
                return null;
            var o = (SolidColorBrush)value;
            return OxyColor.FromArgb(o.Color.A, o.Color.R, o.Color.G, o.Color.B);
        }
    }
}
