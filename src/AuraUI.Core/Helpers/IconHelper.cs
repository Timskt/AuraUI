using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace AuraUI.Core.Helpers;

public static class IconHelper
{
    public static readonly AttachedProperty<string?> IconProperty =
        AvaloniaProperty.RegisterAttached<Control, string?>("Icon", typeof(IconHelper));

    public static readonly AttachedProperty<double> SizeProperty =
        AvaloniaProperty.RegisterAttached<Control, double>("Size", typeof(IconHelper), defaultValue: 16d);

    public static readonly AttachedProperty<Color?> ColorProperty =
        AvaloniaProperty.RegisterAttached<Control, Color?>("Color", typeof(IconHelper));

    static IconHelper()
    {
        IconProperty.Changed.AddClassHandler<Control>(OnIconPropertyChanged);
        SizeProperty.Changed.AddClassHandler<Control>(OnIconPropertyChanged);
        ColorProperty.Changed.AddClassHandler<Control>(OnIconPropertyChanged);
    }

    public static string? GetIcon(Control element) => element.GetValue(IconProperty);
    public static void SetIcon(Control element, string? value) => element.SetValue(IconProperty, value);

    public static double GetSize(Control element) => element.GetValue(SizeProperty);
    public static void SetSize(Control element, double value) => element.SetValue(SizeProperty, value);

    public static Color? GetColor(Control element) => element.GetValue(ColorProperty);
    public static void SetColor(Control element, Color? value) => element.SetValue(ColorProperty, value);

    private static void OnIconPropertyChanged(Control control, AvaloniaPropertyChangedEventArgs e)
    {
        // Store values on the control so that a registered IIconProvider
        // or data template can read them via the attached properties.
        string? icon = GetIcon(control);
        double size = GetSize(control);
        Color? color = GetColor(control);

        if (control is Image image && !string.IsNullOrEmpty(icon))
        {
            image.Width = size;
            image.Height = size;
        }

        // For Path-based icon controls, the parent template should read these
        // attached properties and apply them. We avoid directly referencing
        // Avalonia.Controls.Shapes.Path here to prevent ambiguity with
        // System.IO.Path in consuming projects.
    }
}
