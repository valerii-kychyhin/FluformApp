using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace FluformApp.Converters;

public class StrengthToColorConverter : IValueConverter
{
    public int Segment { get; set; }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not int strength)
            return Colors.LightGray;

        if (strength >= Segment)
        {
            return strength switch
            {
                1 => Color.FromArgb("#F04438"), // weak - red
                2 => Color.FromArgb("#FACC15"), // medium - yellow
                3 => Color.FromArgb("#22C55E"), // strong - green
                _ => Colors.LightGray
            };
        }

        return Colors.LightGray;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}