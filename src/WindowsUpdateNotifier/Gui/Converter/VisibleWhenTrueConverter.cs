﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WindowsUpdateNotifier
{
    public class VisibleWhenTrueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool && (bool) value)
                return Visibility.Visible;

            return Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}