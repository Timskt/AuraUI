using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace AuraUI.Core.Helpers;

/// <summary>
/// Attached properties for styling Tag controls.
/// Usage: aura:TagHelper.CloseButtonSize="10"
/// </summary>
public static class TagHelper
{
    #region IconMargin

    public static readonly AttachedProperty<Thickness> IconMarginProperty =
        AvaloniaProperty.RegisterAttached<Control, Thickness>("IconMargin", typeof(TagHelper), new Thickness(0, 0, 4, 0));

    public static Thickness GetIconMargin(Control element) => element.GetValue(IconMarginProperty);
    public static void SetIconMargin(Control element, Thickness value) => element.SetValue(IconMarginProperty, value);

    #endregion

    #region CloseButtonSize

    public static readonly AttachedProperty<double> CloseButtonSizeProperty =
        AvaloniaProperty.RegisterAttached<Control, double>("CloseButtonSize", typeof(TagHelper), 10);

    public static double GetCloseButtonSize(Control element) => element.GetValue(CloseButtonSizeProperty);
    public static void SetCloseButtonSize(Control element, double value) => element.SetValue(CloseButtonSizeProperty, value);

    #endregion

    #region CloseButtonHoverBackground

    public static readonly AttachedProperty<IBrush?> CloseButtonHoverBackgroundProperty =
        AvaloniaProperty.RegisterAttached<Control, IBrush?>("CloseButtonHoverBackground", typeof(TagHelper));

    public static IBrush? GetCloseButtonHoverBackground(Control element) => element.GetValue(CloseButtonHoverBackgroundProperty);
    public static void SetCloseButtonHoverBackground(Control element, IBrush? value) => element.SetValue(CloseButtonHoverBackgroundProperty, value);

    #endregion

    #region CloseIconForeground

    public static readonly AttachedProperty<IBrush?> CloseIconForegroundProperty =
        AvaloniaProperty.RegisterAttached<Control, IBrush?>("CloseIconForeground", typeof(TagHelper));

    public static IBrush? GetCloseIconForeground(Control element) => element.GetValue(CloseIconForegroundProperty);
    public static void SetCloseIconForeground(Control element, IBrush? value) => element.SetValue(CloseIconForegroundProperty, value);

    #endregion

    #region CornerRadius

    public static readonly AttachedProperty<CornerRadius> CornerRadiusProperty =
        AvaloniaProperty.RegisterAttached<Control, CornerRadius>("CornerRadius", typeof(TagHelper), new CornerRadius(4));

    public static CornerRadius GetCornerRadius(Control element) => element.GetValue(CornerRadiusProperty);
    public static void SetCornerRadius(Control element, CornerRadius value) => element.SetValue(CornerRadiusProperty, value);

    #endregion

    #region MinHeight

    public static readonly AttachedProperty<double> MinHeightProperty =
        AvaloniaProperty.RegisterAttached<Control, double>("MinHeight", typeof(TagHelper), 24);

    public static double GetMinHeight(Control element) => element.GetValue(MinHeightProperty);
    public static void SetMinHeight(Control element, double value) => element.SetValue(MinHeightProperty, value);

    #endregion
}
