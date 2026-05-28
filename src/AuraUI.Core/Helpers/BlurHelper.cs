using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace AuraUI.Core.Helpers;

public static class BlurHelper
{
    public static readonly AttachedProperty<double> BlurProperty =
        AvaloniaProperty.RegisterAttached<Control, double>("Blur", typeof(BlurHelper), defaultValue: 0d);

    static BlurHelper()
    {
        BlurProperty.Changed.AddClassHandler<Control>(OnBlurChanged);
    }

    public static double GetBlur(Control element) => element.GetValue(BlurProperty);
    public static void SetBlur(Control element, double value) => element.SetValue(BlurProperty, value);

    private static void OnBlurChanged(Control control, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.NewValue is double blur && blur > 0)
        {
            control.Effect = new BlurEffect
            {
                Radius = blur
            };
        }
        else
        {
            control.Effect = null;
        }
    }
}
