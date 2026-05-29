using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace AuraUI.Core.Helpers;

/// <summary>
/// Attached properties for styling Timeline controls.
/// Usage: aura:TimelineHelper.LineBrush="Gray"
/// </summary>
public static class TimelineHelper
{
    #region LineBrush

    public static readonly AttachedProperty<IBrush?> LineBrushProperty =
        AvaloniaProperty.RegisterAttached<Control, IBrush?>("LineBrush", typeof(TimelineHelper));

    public static IBrush? GetLineBrush(Control element) => element.GetValue(LineBrushProperty);
    public static void SetLineBrush(Control element, IBrush? value) => element.SetValue(LineBrushProperty, value);

    #endregion

    #region LineWidth

    public static readonly AttachedProperty<double> LineWidthProperty =
        AvaloniaProperty.RegisterAttached<Control, double>("LineWidth", typeof(TimelineHelper), 2);

    public static double GetLineWidth(Control element) => element.GetValue(LineWidthProperty);
    public static void SetLineWidth(Control element, double value) => element.SetValue(LineWidthProperty, value);

    #endregion

    #region DotSize

    public static readonly AttachedProperty<double> DotSizeProperty =
        AvaloniaProperty.RegisterAttached<Control, double>("DotSize", typeof(TimelineHelper), 12);

    public static double GetDotSize(Control element) => element.GetValue(DotSizeProperty);
    public static void SetDotSize(Control element, double value) => element.SetValue(DotSizeProperty, value);

    #endregion

    #region DotColor

    public static readonly AttachedProperty<IBrush?> DotColorProperty =
        AvaloniaProperty.RegisterAttached<Control, IBrush?>("DotColor", typeof(TimelineHelper));

    public static IBrush? GetDotColor(Control element) => element.GetValue(DotColorProperty);
    public static void SetDotColor(Control element, IBrush? value) => element.SetValue(DotColorProperty, value);

    #endregion

    #region DotCornerRadius

    public static readonly AttachedProperty<CornerRadius> DotCornerRadiusProperty =
        AvaloniaProperty.RegisterAttached<Control, CornerRadius>("DotCornerRadius", typeof(TimelineHelper), new CornerRadius(999));

    public static CornerRadius GetDotCornerRadius(Control element) => element.GetValue(DotCornerRadiusProperty);
    public static void SetDotCornerRadius(Control element, CornerRadius value) => element.SetValue(DotCornerRadiusProperty, value);

    #endregion

    #region ItemSpacing

    public static readonly AttachedProperty<double> ItemSpacingProperty =
        AvaloniaProperty.RegisterAttached<Control, double>("ItemSpacing", typeof(TimelineHelper), 16);

    public static double GetItemSpacing(Control element) => element.GetValue(ItemSpacingProperty);
    public static void SetItemSpacing(Control element, double value) => element.SetValue(ItemSpacingProperty, value);

    #endregion
}
