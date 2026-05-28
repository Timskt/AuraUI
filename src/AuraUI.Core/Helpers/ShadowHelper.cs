using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace AuraUI.Core.Helpers;

public static class ShadowHelper
{
    public static readonly AttachedProperty<string?> ShadowProperty =
        AvaloniaProperty.RegisterAttached<Control, string?>("Shadow", typeof(ShadowHelper));

    static ShadowHelper()
    {
        ShadowProperty.Changed.AddClassHandler<Control>(OnShadowChanged);
    }

    public static string? GetShadow(Control element) => element.GetValue(ShadowProperty);
    public static void SetShadow(Control element, string? value) => element.SetValue(ShadowProperty, value);

    private static void OnShadowChanged(Control control, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.NewValue is not string shadowName)
        {
            ClearShadow(control);
            return;
        }

        BoxShadows? boxShadow = shadowName.ToLowerInvariant() switch
        {
            "sm" => new BoxShadows(new BoxShadow
            {
                OffsetX = 0,
                OffsetY = 1,
                Blur = 2,
                Spread = 0,
                Color = Color.FromArgb(25, 0, 0, 0)
            }),
            "md" => new BoxShadows(new BoxShadow
            {
                OffsetX = 0,
                OffsetY = 4,
                Blur = 6,
                Spread = -1,
                Color = Color.FromArgb(25, 0, 0, 0)
            }),
            "lg" => new BoxShadows(new BoxShadow
            {
                OffsetX = 0,
                OffsetY = 10,
                Blur = 15,
                Spread = -3,
                Color = Color.FromArgb(25, 0, 0, 0)
            }),
            "xl" => new BoxShadows(new BoxShadow
            {
                OffsetX = 0,
                OffsetY = 20,
                Blur = 25,
                Spread = -5,
                Color = Color.FromArgb(25, 0, 0, 0)
            }),
            "2xl" => new BoxShadows(new BoxShadow
            {
                OffsetX = 0,
                OffsetY = 25,
                Blur = 50,
                Spread = -12,
                Color = Color.FromArgb(40, 0, 0, 0)
            }),
            "inner" => new BoxShadows(new BoxShadow
            {
                IsInset = true,
                OffsetX = 0,
                OffsetY = 2,
                Blur = 4,
                Spread = 0,
                Color = Color.FromArgb(25, 0, 0, 0)
            }),
            "none" => new BoxShadows(new BoxShadow
            {
                IsInset = false,
                OffsetX = 0,
                OffsetY = 0,
                Blur = 0,
                Spread = 0,
                Color = Colors.Transparent
            }),
            _ => ParseCustomShadow(shadowName)
        };

        if (boxShadow.HasValue)
        {
            ApplyShadow(control, boxShadow.Value);
        }
    }

    private static BoxShadows? ParseCustomShadow(string value)
    {
        // Support custom format: "x y blur spread color"
        // e.g., "0 4 8 0 #00000040"
        string[] parts = value.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length >= 4 &&
            double.TryParse(parts[0], System.Globalization.CultureInfo.InvariantCulture, out double x) &&
            double.TryParse(parts[1], System.Globalization.CultureInfo.InvariantCulture, out double y) &&
            double.TryParse(parts[2], System.Globalization.CultureInfo.InvariantCulture, out double blur) &&
            double.TryParse(parts[3], System.Globalization.CultureInfo.InvariantCulture, out double spread))
        {
            Color color = Colors.Black;
            if (parts.Length >= 5)
            {
                try { color = Color.Parse(parts[4]); } catch { /* keep default */ }
            }

            return new BoxShadows(new BoxShadow
            {
                OffsetX = x,
                OffsetY = y,
                Blur = blur,
                Spread = spread,
                Color = color
            });
        }

        return null;
    }

    private static void ApplyShadow(Control control, BoxShadows shadow)
    {
        if (control is Border border)
        {
            border.BoxShadow = shadow;
        }
    }

    private static void ClearShadow(Control control)
    {
        if (control is Border border)
        {
            border.BoxShadow = default;
        }
    }
}
