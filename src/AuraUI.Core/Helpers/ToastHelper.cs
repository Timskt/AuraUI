using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace AuraUI.Core.Helpers;

/// <summary>
/// Attached properties for styling Toast / Notification controls.
/// Usage: aura:ToastHelper.Position="TopRight"
/// </summary>
public static class ToastHelper
{
    #region Position

    public static readonly AttachedProperty<ToastPosition> PositionProperty =
        AvaloniaProperty.RegisterAttached<Control, ToastPosition>("Position", typeof(ToastHelper), ToastPosition.TopRight);

    public static ToastPosition GetPosition(Control element) => element.GetValue(PositionProperty);
    public static void SetPosition(Control element, ToastPosition value) => element.SetValue(PositionProperty, value);

    #endregion

    #region Duration

    public static readonly AttachedProperty<TimeSpan> DurationProperty =
        AvaloniaProperty.RegisterAttached<Control, TimeSpan>("Duration", typeof(ToastHelper), TimeSpan.FromSeconds(3));

    public static TimeSpan GetDuration(Control element) => element.GetValue(DurationProperty);
    public static void SetDuration(Control element, TimeSpan value) => element.SetValue(DurationProperty, value);

    #endregion

    #region CornerRadius

    public static readonly AttachedProperty<CornerRadius> CornerRadiusProperty =
        AvaloniaProperty.RegisterAttached<Control, CornerRadius>("CornerRadius", typeof(ToastHelper), new CornerRadius(8));

    public static CornerRadius GetCornerRadius(Control element) => element.GetValue(CornerRadiusProperty);
    public static void SetCornerRadius(Control element, CornerRadius value) => element.SetValue(CornerRadiusProperty, value);

    #endregion

    #region Shadow

    public static readonly AttachedProperty<BoxShadow> ShadowProperty =
        AvaloniaProperty.RegisterAttached<Control, BoxShadow>("Shadow", typeof(ToastHelper));

    public static BoxShadow GetShadow(Control element) => element.GetValue(ShadowProperty);
    public static void SetShadow(Control element, BoxShadow value) => element.SetValue(ShadowProperty, value);

    #endregion

    #region ShowIcon

    public static readonly AttachedProperty<bool> ShowIconProperty =
        AvaloniaProperty.RegisterAttached<Control, bool>("ShowIcon", typeof(ToastHelper), true);

    public static bool GetShowIcon(Control element) => element.GetValue(ShowIconProperty);
    public static void SetShowIcon(Control element, bool value) => element.SetValue(ShowIconProperty, value);

    #endregion

    #region ShowCloseButton

    public static readonly AttachedProperty<bool> ShowCloseButtonProperty =
        AvaloniaProperty.RegisterAttached<Control, bool>("ShowCloseButton", typeof(ToastHelper), true);

    public static bool GetShowCloseButton(Control element) => element.GetValue(ShowCloseButtonProperty);
    public static void SetShowCloseButton(Control element, bool value) => element.SetValue(ShowCloseButtonProperty, value);

    #endregion

    #region ShowProgress

    public static readonly AttachedProperty<bool> ShowProgressProperty =
        AvaloniaProperty.RegisterAttached<Control, bool>("ShowProgress", typeof(ToastHelper));

    public static bool GetShowProgress(Control element) => element.GetValue(ShowProgressProperty);
    public static void SetShowProgress(Control element, bool value) => element.SetValue(ShowProgressProperty, value);

    #endregion

    #region MaxToasts

    public static readonly AttachedProperty<int> MaxToastsProperty =
        AvaloniaProperty.RegisterAttached<Control, int>("MaxToasts", typeof(ToastHelper), 5);

    public static int GetMaxToasts(Control element) => element.GetValue(MaxToastsProperty);
    public static void SetMaxToasts(Control element, int value) => element.SetValue(MaxToastsProperty, value);

    #endregion

    #region Spacing

    public static readonly AttachedProperty<double> SpacingProperty =
        AvaloniaProperty.RegisterAttached<Control, double>("Spacing", typeof(ToastHelper), 8);

    public static double GetSpacing(Control element) => element.GetValue(SpacingProperty);
    public static void SetSpacing(Control element, double value) => element.SetValue(SpacingProperty, value);

    #endregion

    #region Margin

    public static readonly AttachedProperty<Thickness> MarginProperty =
        AvaloniaProperty.RegisterAttached<Control, Thickness>("Margin", typeof(ToastHelper), new Thickness(16));

    public static Thickness GetMargin(Control element) => element.GetValue(MarginProperty);
    public static void SetMargin(Control element, Thickness value) => element.SetValue(MarginProperty, value);

    #endregion
}

public enum ToastPosition
{
    TopLeft,
    TopCenter,
    TopRight,
    BottomLeft,
    BottomCenter,
    BottomRight
}
