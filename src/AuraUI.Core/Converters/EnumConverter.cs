using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using Avalonia;
using Avalonia.Data.Converters;

namespace AuraUI.Core.Converters;

/// <summary>
/// Converts an <see cref="Enum"/> value to its <c>[Description]</c>
/// display text (falling back to the member name), and converts a display string back to the
/// corresponding enum value. Use the static <see cref="Instance"/> property for XAML bindings.
/// </summary>
public class EnumConverter : IValueConverter
{
    public static EnumConverter Instance { get; } = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not Enum enumValue)
            return AvaloniaProperty.UnsetValue;

        return GetEnumDescription(enumValue);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string displayName)
            return AvaloniaProperty.UnsetValue;

        if (!targetType.IsEnum)
            return AvaloniaProperty.UnsetValue;

        foreach (var field in targetType.GetFields(BindingFlags.Public | BindingFlags.Static))
        {
            if (GetEnumDescription((Enum)field.GetValue(null)!) == displayName)
                return field.GetValue(null);

            if (string.Equals(field.Name, displayName, StringComparison.OrdinalIgnoreCase))
                return field.GetValue(null);
        }

        return AvaloniaProperty.UnsetValue;
    }

    private static string GetEnumDescription(Enum value)
    {
        FieldInfo? field = value.GetType().GetField(value.ToString());
        if (field is null)
            return value.ToString();

        DescriptionAttribute? attribute = field.GetCustomAttribute<DescriptionAttribute>();
        return attribute?.Description ?? value.ToString();
    }
}
