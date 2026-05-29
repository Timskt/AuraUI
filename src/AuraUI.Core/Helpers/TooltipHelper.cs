using Avalonia;
using Avalonia.Controls;

namespace AuraUI.Core.Helpers;

/// <summary>
/// Provides attached properties for configuring tooltip behavior: <c>ShowDelay</c>
/// (milliseconds before the tooltip appears) and <c>HasArrow</c> (read by custom
/// tooltip templates to decide whether to render an arrow). These complement the
/// built-in <see cref="Avalonia.Controls.ToolTip"/> attached properties.
/// </summary>
public static class TooltipHelper
{
    public static readonly AttachedProperty<int> ShowDelayProperty =
        AvaloniaProperty.RegisterAttached<Control, int>("ShowDelay", typeof(TooltipHelper), defaultValue: 400);

    public static readonly AttachedProperty<bool> HasArrowProperty =
        AvaloniaProperty.RegisterAttached<Control, bool>("HasArrow", typeof(TooltipHelper), defaultValue: true);

    static TooltipHelper()
    {
        ShowDelayProperty.Changed.AddClassHandler<Control>(OnShowDelayChanged);
        HasArrowProperty.Changed.AddClassHandler<Control>(OnHasArrowChanged);
    }

    public static int GetShowDelay(Control element) => element.GetValue(ShowDelayProperty);
    public static void SetShowDelay(Control element, int value) => element.SetValue(ShowDelayProperty, value);

    public static bool GetHasArrow(Control element) => element.GetValue(HasArrowProperty);
    public static void SetHasArrow(Control element, bool value) => element.SetValue(HasArrowProperty, value);

    private static void OnShowDelayChanged(Control control, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.NewValue is int delay)
        {
            ToolTip.SetShowDelay(control, delay);
        }
    }

    private static void OnHasArrowChanged(Control control, AvaloniaPropertyChangedEventArgs e)
    {
        // The HasArrow property is stored as an attached property for
        // custom tooltip templates to read via TemplateBinding.
        // Avalonia's built-in ToolTip does not expose HasArrow directly,
        // so custom tooltip styles should bind to this property.
    }
}
