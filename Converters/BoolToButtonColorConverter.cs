using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace FluformApp.Converters
{
    public class BoolToButtonColorConverter : IValueConverter
    {
        public Color EnabledColor { get; set; } = Color.FromArgb("#5C74E0");
        public Color DisabledColor { get; set; } = Color.FromArgb("#D1D5DB");

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b && b)
                return EnabledColor;

            return DisabledColor;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}