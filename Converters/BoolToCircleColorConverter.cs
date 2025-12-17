using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace FluformApp.Converters;

public class BoolToCircleColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        => value is bool b && b ? Color.FromArgb("#74CF47") : Color.FromArgb("#94A1B2");

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}