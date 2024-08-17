using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Shop.Converters
{
    public class ErrorToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && !string.IsNullOrEmpty(value.ToString()))
            {
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#65a8e6"));
            }
            else
            {
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#e4eaec"));
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
