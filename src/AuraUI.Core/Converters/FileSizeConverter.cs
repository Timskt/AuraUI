using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;

namespace AuraUI.Core.Converters;

public class FileSizeConverter : IValueConverter
{
    public static FileSizeConverter Instance { get; } = new();

    private static readonly string[] SizeUnits = ["B", "KB", "MB", "GB", "TB", "PB"];

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        double bytes;

        switch (value)
        {
            case long l:
                bytes = l;
                break;
            case int i:
                bytes = i;
                break;
            case ulong ul:
                bytes = ul;
                break;
            case double d:
                bytes = d;
                break;
            case float f:
                bytes = f;
                break;
            default:
                return AvaloniaProperty.UnsetValue;
        }

        if (bytes < 0)
            return AvaloniaProperty.UnsetValue;

        int unitIndex = 0;
        double size = bytes;

        while (size >= 1024.0 && unitIndex < SizeUnits.Length - 1)
        {
            size /= 1024.0;
            unitIndex++;
        }

        string format = unitIndex == 0 ? "F0" : "F1";
        return string.Format(culture, "{0:" + format + "} {1}", size, SizeUnits[unitIndex]);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException("FileSizeConverter does not support ConvertBack.");
    }
}
