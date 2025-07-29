using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Layout;

namespace PINChat.UI.Core.Converters;

public class BooleanToHorizontalAlignmentConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool b)
        {
            if (b) return HorizontalAlignment.Right;
            return HorizontalAlignment.Left;
        }
        return HorizontalAlignment.Left; // Default
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}