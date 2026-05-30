using Avalonia;
using Avalonia.Controls;

namespace AuraUI.Core.Directives;

/// <summary>
/// Vue-like v-tooltip directive implemented as attached properties.
/// Provides enhanced tooltip configuration with placement control and
/// automatic wiring to Avalonia's built-in <see cref="ToolTip"/> system.
/// </summary>
/// <example>
/// <code>
/// // In AXAML:
/// &lt;Button Content="Hover me"
///          Directives:VTooltip.Text="Click to submit"
///          Directives:VTooltip.Placement="Top" /&gt;
///
/// // In code-behind:
/// VTooltip.SetText(myButton, "Click to submit");
/// VTooltip.SetPlacement(myButton, Placement.Top);
/// </code>
/// </example>
public static class VTooltip
{
    /// <summary>
    /// The tooltip text. Setting this also sets the ToolTip.Tip property on the control.
    /// </summary>
    public static readonly AttachedProperty<string?> TextProperty =
        AvaloniaProperty.RegisterAttached<Control, string?>("Text", typeof(VTooltip));

    /// <summary>
    /// The tooltip placement relative to the control.
    /// </summary>
    public static readonly AttachedProperty<PlacementMode> PlacementProperty =
        AvaloniaProperty.RegisterAttached<Control, PlacementMode>(
            "Placement", typeof(VTooltip), PlacementMode.Top);

    /// <summary>
    /// The delay in milliseconds before the tooltip appears.
    /// </summary>
    public static readonly AttachedProperty<int> DelayProperty =
        AvaloniaProperty.RegisterAttached<Control, int>(
            "Delay", typeof(VTooltip), 400);

    /// <summary>
    /// Whether the tooltip has an arrow pointing to the control.
    /// Custom tooltip templates can bind to this property.
    /// </summary>
    public static readonly AttachedProperty<bool> HasArrowProperty =
        AvaloniaProperty.RegisterAttached<Control, bool>(
            "HasArrow", typeof(VTooltip), true);

    /// <summary>
    /// The maximum width of the tooltip.
    /// </summary>
    public static readonly AttachedProperty<double> MaxWidthProperty =
        AvaloniaProperty.RegisterAttached<Control, double>(
            "MaxWidth", typeof(VTooltip), 300d);

    static VTooltip()
    {
        TextProperty.Changed.AddClassHandler<Control>(OnTextChanged);
        PlacementProperty.Changed.AddClassHandler<Control>(OnPlacementChanged);
        DelayProperty.Changed.AddClassHandler<Control>(OnDelayChanged);
    }

    public static string? GetText(Control element) => element.GetValue(TextProperty);
    public static void SetText(Control element, string? value) => element.SetValue(TextProperty, value);

    public static PlacementMode GetPlacement(Control element) => element.GetValue(PlacementProperty);
    public static void SetPlacement(Control element, PlacementMode value) => element.SetValue(PlacementProperty, value);

    public static int GetDelay(Control element) => element.GetValue(DelayProperty);
    public static void SetDelay(Control element, int value) => element.SetValue(DelayProperty, value);

    public static bool GetHasArrow(Control element) => element.GetValue(HasArrowProperty);
    public static void SetHasArrow(Control element, bool value) => element.SetValue(HasArrowProperty, value);

    public static double GetMaxWidth(Control element) => element.GetValue(MaxWidthProperty);
    public static void SetMaxWidth(Control element, double value) => element.SetValue(MaxWidthProperty, value);

    private static void OnTextChanged(Control control, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.NewValue is string text)
        {
            ToolTip.SetTip(control, text);
            ToolTip.SetIsOpen(control, false); // Reset open state when text changes
        }
        else
        {
            ToolTip.SetTip(control, null);
        }
    }

    private static void OnPlacementChanged(Control control, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.NewValue is PlacementMode placement)
        {
            ToolTip.SetPlacement(control, placement);
        }
    }

    private static void OnDelayChanged(Control control, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.NewValue is int delay)
        {
            ToolTip.SetShowDelay(control, delay);
        }
    }
}

/// <summary>
/// Specifies the placement of a tooltip relative to its target control.
/// Maps to Avalonia's <see cref="PlacementMode"/> enum.
/// </summary>
public enum Placement
{
    /// <summary>Tooltip appears above the control.</summary>
    Top,
    /// <summary>Tooltip appears below the control.</summary>
    Bottom,
    /// <summary>Tooltip appears to the left of the control.</summary>
    Left,
    /// <summary>Tooltip appears to the right of the control.</summary>
    Right,
    /// <summary>Tooltip appears centered over the control.</summary>
    Center,
    /// <summary>Tooltip appears at the top-left of the control.</summary>
    TopLeft,
    /// <summary>Tooltip appears at the top-right of the control.</summary>
    TopRight,
    /// <summary>Tooltip appears at the bottom-left of the control.</summary>
    BottomLeft,
    /// <summary>Tooltip appears at the bottom-right of the control.</summary>
    BottomRight
}
