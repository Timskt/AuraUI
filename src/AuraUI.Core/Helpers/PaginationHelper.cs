using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace AuraUI.Core.Helpers;

/// <summary>
/// Attached properties for styling Pagination controls.
/// Usage: aura:PaginationHelper.ItemSize="32"
/// </summary>
public static class PaginationHelper
{
    #region ItemSize

    public static readonly AttachedProperty<double> ItemSizeProperty =
        AvaloniaProperty.RegisterAttached<Control, double>("ItemSize", typeof(PaginationHelper), 32);

    public static double GetItemSize(Control element) => element.GetValue(ItemSizeProperty);
    public static void SetItemSize(Control element, double value) => element.SetValue(ItemSizeProperty, value);

    #endregion

    #region ItemCornerRadius

    public static readonly AttachedProperty<CornerRadius> ItemCornerRadiusProperty =
        AvaloniaProperty.RegisterAttached<Control, CornerRadius>("ItemCornerRadius", typeof(PaginationHelper), new CornerRadius(4));

    public static CornerRadius GetItemCornerRadius(Control element) => element.GetValue(ItemCornerRadiusProperty);
    public static void SetItemCornerRadius(Control element, CornerRadius value) => element.SetValue(ItemCornerRadiusProperty, value);

    #endregion

    #region ActiveBackground

    public static readonly AttachedProperty<IBrush?> ActiveBackgroundProperty =
        AvaloniaProperty.RegisterAttached<Control, IBrush?>("ActiveBackground", typeof(PaginationHelper));

    public static IBrush? GetActiveBackground(Control element) => element.GetValue(ActiveBackgroundProperty);
    public static void SetActiveBackground(Control element, IBrush? value) => element.SetValue(ActiveBackgroundProperty, value);

    #endregion

    #region ActiveForeground

    public static readonly AttachedProperty<IBrush?> ActiveForegroundProperty =
        AvaloniaProperty.RegisterAttached<Control, IBrush?>("ActiveForeground", typeof(PaginationHelper));

    public static IBrush? GetActiveForeground(Control element) => element.GetValue(ActiveForegroundProperty);
    public static void SetActiveForeground(Control element, IBrush? value) => element.SetValue(ActiveForegroundProperty, value);

    #endregion

    #region HoverBackground

    public static readonly AttachedProperty<IBrush?> HoverBackgroundProperty =
        AvaloniaProperty.RegisterAttached<Control, IBrush?>("HoverBackground", typeof(PaginationHelper));

    public static IBrush? GetHoverBackground(Control element) => element.GetValue(HoverBackgroundProperty);
    public static void SetHoverBackground(Control element, IBrush? value) => element.SetValue(HoverBackgroundProperty, value);

    #endregion

    #region HoverForeground

    public static readonly AttachedProperty<IBrush?> HoverForegroundProperty =
        AvaloniaProperty.RegisterAttached<Control, IBrush?>("HoverForeground", typeof(PaginationHelper));

    public static IBrush? GetHoverForeground(Control element) => element.GetValue(HoverForegroundProperty);
    public static void SetHoverForeground(Control element, IBrush? value) => element.SetValue(HoverForegroundProperty, value);

    #endregion

    #region ItemSpacing

    public static readonly AttachedProperty<double> ItemSpacingProperty =
        AvaloniaProperty.RegisterAttached<Control, double>("ItemSpacing", typeof(PaginationHelper), 4);

    public static double GetItemSpacing(Control element) => element.GetValue(ItemSpacingProperty);
    public static void SetItemSpacing(Control element, double value) => element.SetValue(ItemSpacingProperty, value);

    #endregion

    #region DisabledForeground

    public static readonly AttachedProperty<IBrush?> DisabledForegroundProperty =
        AvaloniaProperty.RegisterAttached<Control, IBrush?>("DisabledForeground", typeof(PaginationHelper));

    public static IBrush? GetDisabledForeground(Control element) => element.GetValue(DisabledForegroundProperty);
    public static void SetDisabledForeground(Control element, IBrush? value) => element.SetValue(DisabledForegroundProperty, value);

    #endregion
}
