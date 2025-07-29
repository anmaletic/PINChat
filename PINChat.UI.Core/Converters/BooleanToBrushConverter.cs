using System.Globalization;
using Avalonia.Data.Converters;

namespace PINChat.UI.Core.Converters;

public class BooleanToBrushConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool b)
        {
            if (b) return TrueBrush;
            return FalseBrush;
        }
        return null;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

    public required Avalonia.Media.IBrush TrueBrush { get; set; }
    public required Avalonia.Media.IBrush FalseBrush { get; set; }
}