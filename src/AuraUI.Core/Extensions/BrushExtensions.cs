using Avalonia.Media;

namespace AuraUI.Core.Extensions;

public static class BrushExtensions
{
    /// <summary>
    /// Returns a new SolidColorBrush with the specified opacity (0.0 to 1.0).
    /// </summary>
    public static SolidColorBrush WithOpacity(this IBrush brush, double opacity)
    {
        ArgumentNullException.ThrowIfNull(brush);

        Color baseColor = GetBaseColor(brush);
        byte alpha = (byte)Math.Clamp((int)(opacity * 255), 0, 255);
        return new SolidColorBrush(Color.FromArgb(alpha, baseColor.R, baseColor.G, baseColor.B));
    }

    /// <summary>
    /// Returns a new SolidColorBrush darkened by the specified amount (0.0 to 1.0).
    /// </summary>
    public static SolidColorBrush Darken(this IBrush brush, double amount)
    {
        ArgumentNullException.ThrowIfNull(brush);

        Color baseColor = GetBaseColor(brush);
        amount = Math.Clamp(amount, 0, 1);

        byte r = (byte)(baseColor.R * (1 - amount));
        byte g = (byte)(baseColor.G * (1 - amount));
        byte b = (byte)(baseColor.B * (1 - amount));

        return new SolidColorBrush(Color.FromArgb(baseColor.A, r, g, b));
    }

    /// <summary>
    /// Returns a new SolidColorBrush lightened by the specified amount (0.0 to 1.0).
    /// </summary>
    public static SolidColorBrush Lighten(this IBrush brush, double amount)
    {
        ArgumentNullException.ThrowIfNull(brush);

        Color baseColor = GetBaseColor(brush);
        amount = Math.Clamp(amount, 0, 1);

        byte r = (byte)(baseColor.R + (255 - baseColor.R) * amount);
        byte g = (byte)(baseColor.G + (255 - baseColor.G) * amount);
        byte b = (byte)(baseColor.B + (255 - baseColor.B) * amount);

        return new SolidColorBrush(Color.FromArgb(baseColor.A, r, g, b));
    }

    /// <summary>
    /// Returns a new Color with the specified opacity (0.0 to 1.0).
    /// </summary>
    public static Color WithOpacity(this Color color, double opacity)
    {
        byte alpha = (byte)Math.Clamp((int)(opacity * 255), 0, 255);
        return Color.FromArgb(alpha, color.R, color.G, color.B);
    }

    /// <summary>
    /// Returns a new Color darkened by the specified amount (0.0 to 1.0).
    /// </summary>
    public static Color Darken(this Color color, double amount)
    {
        amount = Math.Clamp(amount, 0, 1);

        byte r = (byte)(color.R * (1 - amount));
        byte g = (byte)(color.G * (1 - amount));
        byte b = (byte)(color.B * (1 - amount));

        return Color.FromArgb(color.A, r, g, b);
    }

    /// <summary>
    /// Returns a new Color lightened by the specified amount (0.0 to 1.0).
    /// </summary>
    public static Color Lighten(this Color color, double amount)
    {
        amount = Math.Clamp(amount, 0, 1);

        byte r = (byte)(color.R + (255 - color.R) * amount);
        byte g = (byte)(color.G + (255 - color.G) * amount);
        byte b = (byte)(color.B + (255 - color.B) * amount);

        return Color.FromArgb(color.A, r, g, b);
    }

    private static Color GetBaseColor(IBrush brush)
    {
        return brush switch
        {
            SolidColorBrush solid => solid.Color,
            ISolidColorBrush solidInterface => solidInterface.Color,
            _ => Colors.Transparent
        };
    }
}
