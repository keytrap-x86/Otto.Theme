using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Otto.Theme.Tools.Converter
{
    public class Int2VisibilityReConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            var integer = (int)value;
            if (integer == 0)
                return Visibility.Visible;
            else
                return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
