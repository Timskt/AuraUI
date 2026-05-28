using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace AuraUI.Core.Helpers;

/// <summary>
/// Attached properties for styling Window controls.
/// Usage: aura:WindowHelper.CaptionBackground="#005FB8"
/// </summary>
public static class WindowHelper
{
    #region CaptionBackground

    public static readonly AttachedProperty<IBrush?> CaptionBackgroundProperty =
        AvaloniaProperty.RegisterAttached<Window, IBrush?>("CaptionBackground", typeof(WindowHelper));

    public static IBrush? GetCaptionBackground(Window element) => element.GetValue(CaptionBackgroundProperty);
    public static void SetCaptionBackground(Window element, IBrush? value) => element.SetValue(CaptionBackgroundProperty, value);

    #endregion

    #region CaptionForeground

    public static readonly AttachedProperty<IBrush?> CaptionForegroundProperty =
        AvaloniaProperty.RegisterAttached<Window, IBrush?>("CaptionForeground", typeof(WindowHelper));

    public static IBrush? GetCaptionForeground(Window element) => element.GetValue(CaptionForegroundProperty);
    public static void SetCaptionForeground(Window element, IBrush? value) => element.SetValue(CaptionForegroundProperty, value);

    #endregion

    #region CaptionHeight

    public static readonly AttachedProperty<double> CaptionHeightProperty =
        AvaloniaProperty.RegisterAttached<Window, double>("CaptionHeight", typeof(WindowHelper), 32);

    public static double GetCaptionHeight(Window element) => element.GetValue(CaptionHeightProperty);
    public static void SetCaptionHeight(Window element, double value) => element.SetValue(CaptionHeightProperty, value);

    #endregion

    #region IsCaptionVisible

    public static readonly AttachedProperty<bool> IsCaptionVisibleProperty =
        AvaloniaProperty.RegisterAttached<Window, bool>("IsCaptionVisible", typeof(WindowHelper), true);

    public static bool GetIsCaptionVisible(Window element) => element.GetValue(IsCaptionVisibleProperty);
    public static void SetIsCaptionVisible(Window element, bool value) => element.SetValue(IsCaptionVisibleProperty, value);

    #endregion

    #region HeaderContent

    public static readonly AttachedProperty<object?> HeaderContentProperty =
        AvaloniaProperty.RegisterAttached<Window, object?>("HeaderContent", typeof(WindowHelper));

    public static object? GetHeaderContent(Window element) => element.GetValue(HeaderContentProperty);
    public static void SetHeaderContent(Window element, object? value) => element.SetValue(HeaderContentProperty, value);

    #endregion

    #region FooterContent

    public static readonly AttachedProperty<object?> FooterContentProperty =
        AvaloniaProperty.RegisterAttached<Window, object?>("FooterContent", typeof(WindowHelper));

    public static object? GetFooterContent(Window element) => element.GetValue(FooterContentProperty);
    public static void SetFooterContent(Window element, object? value) => element.SetValue(FooterContentProperty, value);

    #endregion

    #region CornerRadius

    public static readonly AttachedProperty<CornerRadius> CornerRadiusProperty =
        AvaloniaProperty.RegisterAttached<Window, CornerRadius>("CornerRadius", typeof(WindowHelper));

    public static CornerRadius GetCornerRadius(Window element) => element.GetValue(CornerRadiusProperty);
    public static void SetCornerRadius(Window element, CornerRadius value) => element.SetValue(CornerRadiusProperty, value);

    #endregion

    #region Shadow

    public static readonly AttachedProperty<BoxShadow> ShadowProperty =
        AvaloniaProperty.RegisterAttached<Window, BoxShadow>("Shadow", typeof(WindowHelper));

    public static BoxShadow GetShadow(Window element) => element.GetValue(ShadowProperty);
    public static void SetShadow(Window element, BoxShadow value) => element.SetValue(ShadowProperty, value);

    #endregion

    #region IsMinimizeEnabled

    public static readonly AttachedProperty<bool> IsMinimizeEnabledProperty =
        AvaloniaProperty.RegisterAttached<Window, bool>("IsMinimizeEnabled", typeof(WindowHelper), true);

    public static bool GetIsMinimizeEnabled(Window element) => element.GetValue(IsMinimizeEnabledProperty);
    public static void SetIsMinimizeEnabled(Window element, bool value) => element.SetValue(IsMinimizeEnabledProperty, value);

    #endregion

    #region IsMaximizeEnabled

    public static readonly AttachedProperty<bool> IsMaximizeEnabledProperty =
        AvaloniaProperty.RegisterAttached<Window, bool>("IsMaximizeEnabled", typeof(WindowHelper), true);

    public static bool GetIsMaximizeEnabled(Window element) => element.GetValue(IsMaximizeEnabledProperty);
    public static void SetIsMaximizeEnabled(Window element, bool value) => element.SetValue(IsMaximizeEnabledProperty, value);

    #endregion

    #region IsCloseEnabled

    public static readonly AttachedProperty<bool> IsCloseEnabledProperty =
        AvaloniaProperty.RegisterAttached<Window, bool>("IsCloseEnabled", typeof(WindowHelper), true);

    public static bool GetIsCloseEnabled(Window element) => element.GetValue(IsCloseEnabledProperty);
    public static void SetIsCloseEnabled(Window element, bool value) => element.SetValue(IsCloseEnabledProperty, value);

    #endregion

    #region CloseButtonHoverBackground

    public static readonly AttachedProperty<IBrush?> CloseButtonHoverBackgroundProperty =
        AvaloniaProperty.RegisterAttached<Window, IBrush?>("CloseButtonHoverBackground", typeof(WindowHelper));

    public static IBrush? GetCloseButtonHoverBackground(Window element) => element.GetValue(CloseButtonHoverBackgroundProperty);
    public static void SetCloseButtonHoverBackground(Window element, IBrush? value) => element.SetValue(CloseButtonHoverBackgroundProperty, value);

    #endregion
}
