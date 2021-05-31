using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Otto.Theme.Tools.Converter
{
    public class Int2BoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            var integer = (int)value;
            //Console.WriteLine($"i=>>>{integer}");
            return integer != 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
