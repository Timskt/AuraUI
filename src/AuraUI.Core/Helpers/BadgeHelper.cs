using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace AuraUI.Core.Helpers;

/// <summary>
/// Attached properties for styling Badge controls.
/// Usage: aura:BadgeHelper.MinWidth="20"
/// </summary>
public static class BadgeHelper
{
    #region MinWidth

    public static readonly AttachedProperty<double> MinWidthProperty =
        AvaloniaProperty.RegisterAttached<Control, double>("MinWidth", typeof(BadgeHelper), 20);

    public static double GetMinWidth(Control element) => element.GetValue(MinWidthProperty);
    public static void SetMinWidth(Control element, double value) => element.SetValue(MinWidthProperty, value);

    #endregion

    #region MinHeight

    public static readonly AttachedProperty<double> MinHeightProperty =
        AvaloniaProperty.RegisterAttached<Control, double>("MinHeight", typeof(BadgeHelper), 20);

    public static double GetMinHeight(Control element) => element.GetValue(MinHeightProperty);
    public static void SetMinHeight(Control element, double value) => element.SetValue(MinHeightProperty, value);

    #endregion

    #region OffsetX

    public static readonly AttachedProperty<double> OffsetXProperty =
        AvaloniaProperty.RegisterAttached<Control, double>("OffsetX", typeof(BadgeHelper), -6);

    public static double GetOffsetX(Control element) => element.GetValue(OffsetXProperty);
    public static void SetOffsetX(Control element, double value) => element.SetValue(OffsetXProperty, value);

    #endregion

    #region OffsetY

    public static readonly AttachedProperty<double> OffsetYProperty =
        AvaloniaProperty.RegisterAttached<Control, double>("OffsetY", typeof(BadgeHelper), -6);

    public static double GetOffsetY(Control element) => element.GetValue(OffsetYProperty);
    public static void SetOffsetY(Control element, double value) => element.SetValue(OffsetYProperty, value);

    #endregion

    #region DotSize

    public static readonly AttachedProperty<double> DotSizeProperty =
        AvaloniaProperty.RegisterAttached<Control, double>("DotSize", typeof(BadgeHelper), 10);

    public static double GetDotSize(Control element) => element.GetValue(DotSizeProperty);
    public static void SetDotSize(Control element, double value) => element.SetValue(DotSizeProperty, value);

    #endregion

    #region BadgeBackground

    public static readonly AttachedProperty<IBrush?> BadgeBackgroundProperty =
        AvaloniaProperty.RegisterAttached<Control, IBrush?>("BadgeBackground", typeof(BadgeHelper));

    public static IBrush? GetBadgeBackground(Control element) => element.GetValue(BadgeBackgroundProperty);
    public static void SetBadgeBackground(Control element, IBrush? value) => element.SetValue(BadgeBackgroundProperty, value);

    #endregion

    #region BadgeForeground

    public static readonly AttachedProperty<IBrush?> BadgeForegroundProperty =
        AvaloniaProperty.RegisterAttached<Control, IBrush?>("BadgeForeground", typeof(BadgeHelper));

    public static IBrush? GetBadgeForeground(Control element) => element.GetValue(BadgeForegroundProperty);
    public static void SetBadgeForeground(Control element, IBrush? value) => element.SetValue(BadgeForegroundProperty, value);

    #endregion

    #region CornerRadius

    public static readonly AttachedProperty<CornerRadius> CornerRadiusProperty =
        AvaloniaProperty.RegisterAttached<Control, CornerRadius>("CornerRadius", typeof(BadgeHelper), new CornerRadius(999));

    public static CornerRadius GetCornerRadius(Control element) => element.GetValue(CornerRadiusProperty);
    public static void SetCornerRadius(Control element, CornerRadius value) => element.SetValue(CornerRadiusProperty, value);

    #endregion
}
