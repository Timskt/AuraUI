using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace AuraUI.Core.Helpers;

/// <summary>
/// Attached properties for styling Breadcrumb controls.
/// Usage: aura:BreadcrumbHelper.SeparatorColor="Gray"
/// </summary>
public static class BreadcrumbHelper
{
    #region SeparatorColor

    public static readonly AttachedProperty<IBrush?> SeparatorColorProperty =
        AvaloniaProperty.RegisterAttached<Control, IBrush?>("SeparatorColor", typeof(BreadcrumbHelper));

    public static IBrush? GetSeparatorColor(Control element) => element.GetValue(SeparatorColorProperty);
    public static void SetSeparatorColor(Control element, IBrush? value) => element.SetValue(SeparatorColorProperty, value);

    #endregion

    #region ItemHoverForeground

    public static readonly AttachedProperty<IBrush?> ItemHoverForegroundProperty =
        AvaloniaProperty.RegisterAttached<Control, IBrush?>("ItemHoverForeground", typeof(BreadcrumbHelper));

    public static IBrush? GetItemHoverForeground(Control element) => element.GetValue(ItemHoverForegroundProperty);
    public static void SetItemHoverForeground(Control element, IBrush? value) => element.SetValue(ItemHoverForegroundProperty, value);

    #endregion

    #region SeparatorMargin

    public static readonly AttachedProperty<Thickness> SeparatorMarginProperty =
        AvaloniaProperty.RegisterAttached<Control, Thickness>("SeparatorMargin", typeof(BreadcrumbHelper), new Thickness(8, 0));

    public static Thickness GetSeparatorMargin(Control element) => element.GetValue(SeparatorMarginProperty);
    public static void SetSeparatorMargin(Control element, Thickness value) => element.SetValue(SeparatorMarginProperty, value);

    #endregion

    #region ItemHoverBackground

    public static readonly AttachedProperty<IBrush?> ItemHoverBackgroundProperty =
        AvaloniaProperty.RegisterAttached<Control, IBrush?>("ItemHoverBackground", typeof(BreadcrumbHelper));

    public static IBrush? GetItemHoverBackground(Control element) => element.GetValue(ItemHoverBackgroundProperty);
    public static void SetItemHoverBackground(Control element, IBrush? value) => element.SetValue(ItemHoverBackgroundProperty, value);

    #endregion

    #region SeparatorContent

    public static readonly AttachedProperty<string?> SeparatorContentProperty =
        AvaloniaProperty.RegisterAttached<Control, string?>("SeparatorContent", typeof(BreadcrumbHelper), "/");

    public static string? GetSeparatorContent(Control element) => element.GetValue(SeparatorContentProperty);
    public static void SetSeparatorContent(Control element, string? value) => element.SetValue(SeparatorContentProperty, value);

    #endregion

    #region ActiveItemForeground

    public static readonly AttachedProperty<IBrush?> ActiveItemForegroundProperty =
        AvaloniaProperty.RegisterAttached<Control, IBrush?>("ActiveItemForeground", typeof(BreadcrumbHelper));

    public static IBrush? GetActiveItemForeground(Control element) => element.GetValue(ActiveItemForegroundProperty);
    public static void SetActiveItemForeground(Control element, IBrush? value) => element.SetValue(ActiveItemForegroundProperty, value);

    #endregion

    #region FontSize

    public static readonly AttachedProperty<double> FontSizeProperty =
        AvaloniaProperty.RegisterAttached<Control, double>("FontSize", typeof(BreadcrumbHelper), 14);

    public static double GetFontSize(Control element) => element.GetValue(FontSizeProperty);
    public static void SetFontSize(Control element, double value) => element.SetValue(FontSizeProperty, value);

    #endregion
}
