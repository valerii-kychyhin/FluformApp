using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace FluformApp.Converters
{
    public class BoolToEyeIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool hidden)
                return hidden ? "icon_hidden.png" : "icon_view.png";
            return "icon_hidden.png";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}