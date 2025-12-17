using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace FluformApp.Converters;

public class BoolToIconConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is bool b && b
            ? "icon_check.png"
            : "icon_cross.png";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
