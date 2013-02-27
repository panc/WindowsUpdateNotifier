using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WindowsUpdateNotifier.Converter
{
    public class BooleanToFontWeightConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool && (bool)value)
                return FontWeights.Medium;

            return FontWeights.Light;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}