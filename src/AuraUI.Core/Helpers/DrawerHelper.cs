using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace AuraUI.Core.Helpers;

/// <summary>
/// Attached properties for styling Drawer controls.
/// Usage: aura:DrawerHelper.DrawerBackground="White"
/// </summary>
public static class DrawerHelper
{
    #region OverlayBrush

    public static readonly AttachedProperty<IBrush?> OverlayBrushProperty =
        AvaloniaProperty.RegisterAttached<Control, IBrush?>("OverlayBrush", typeof(DrawerHelper));

    public static IBrush? GetOverlayBrush(Control element) => element.GetValue(OverlayBrushProperty);
    public static void SetOverlayBrush(Control element, IBrush? value) => element.SetValue(OverlayBrushProperty, value);

    #endregion

    #region DrawerBackground

    public static readonly AttachedProperty<IBrush?> DrawerBackgroundProperty =
        AvaloniaProperty.RegisterAttached<Control, IBrush?>("DrawerBackground", typeof(DrawerHelper));

    public static IBrush? GetDrawerBackground(Control element) => element.GetValue(DrawerBackgroundProperty);
    public static void SetDrawerBackground(Control element, IBrush? value) => element.SetValue(DrawerBackgroundProperty, value);

    #endregion

    #region DrawerShadow

    public static readonly AttachedProperty<BoxShadow> DrawerShadowProperty =
        AvaloniaProperty.RegisterAttached<Control, BoxShadow>("DrawerShadow", typeof(DrawerHelper));

    public static BoxShadow GetDrawerShadow(Control element) => element.GetValue(DrawerShadowProperty);
    public static void SetDrawerShadow(Control element, BoxShadow value) => element.SetValue(DrawerShadowProperty, value);

    #endregion

    #region HandleSize

    public static readonly AttachedProperty<double> HandleSizeProperty =
        AvaloniaProperty.RegisterAttached<Control, double>("HandleSize", typeof(DrawerHelper), 44);

    public static double GetHandleSize(Control element) => element.GetValue(HandleSizeProperty);
    public static void SetHandleSize(Control element, double value) => element.SetValue(HandleSizeProperty, value);

    #endregion

    #region OverlayOpacity

    public static readonly AttachedProperty<double> OverlayOpacityProperty =
        AvaloniaProperty.RegisterAttached<Control, double>("OverlayOpacity", typeof(DrawerHelper), 0.5);

    public static double GetOverlayOpacity(Control element) => element.GetValue(OverlayOpacityProperty);
    public static void SetOverlayOpacity(Control element, double value) => element.SetValue(OverlayOpacityProperty, value);

    #endregion

    #region CornerRadius

    public static readonly AttachedProperty<CornerRadius> CornerRadiusProperty =
        AvaloniaProperty.RegisterAttached<Control, CornerRadius>("CornerRadius", typeof(DrawerHelper));

    public static CornerRadius GetCornerRadius(Control element) => element.GetValue(CornerRadiusProperty);
    public static void SetCornerRadius(Control element, CornerRadius value) => element.SetValue(CornerRadiusProperty, value);

    #endregion
}
