using Avalonia;
using Avalonia.Controls;

namespace AuraUI.Core.Helpers;

public static class CornerRadiusHelper
{
    public static readonly AttachedProperty<CornerRadius> CornerRadiusProperty =
        AvaloniaProperty.RegisterAttached<Border, CornerRadius>(
            "CornerRadius",
            typeof(CornerRadiusHelper),
            defaultValue: default);

    static CornerRadiusHelper()
    {
        CornerRadiusProperty.Changed.AddClassHandler<Border>(OnCornerRadiusChanged);
    }

    public static CornerRadius GetCornerRadius(Border element) => element.GetValue(CornerRadiusProperty);
    public static void SetCornerRadius(Border element, CornerRadius value) => element.SetValue(CornerRadiusProperty, value);

    private static void OnCornerRadiusChanged(Border border, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.NewValue is CornerRadius cornerRadius)
        {
            border.CornerRadius = cornerRadius;
        }
    }
}
