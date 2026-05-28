using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace AuraUI.Core.Helpers;

/// <summary>
/// Attached properties for styling Menu / MenuItem controls.
/// Usage: aura:MenuHelper.ItemCornerRadius="4"
/// </summary>
public static class MenuHelper
{
    #region ItemCornerRadius

    public static readonly AttachedProperty<CornerRadius> ItemCornerRadiusProperty =
        AvaloniaProperty.RegisterAttached<MenuItem, CornerRadius>("ItemCornerRadius", typeof(MenuHelper), new CornerRadius(4));

    public static CornerRadius GetItemCornerRadius(MenuItem element) => element.GetValue(ItemCornerRadiusProperty);
    public static void SetItemCornerRadius(MenuItem element, CornerRadius value) => element.SetValue(ItemCornerRadiusProperty, value);

    #endregion

    #region ItemPadding

    public static readonly AttachedProperty<Thickness> ItemPaddingProperty =
        AvaloniaProperty.RegisterAttached<MenuItem, Thickness>("ItemPadding", typeof(MenuHelper), new Thickness(8, 6));

    public static Thickness GetItemPadding(MenuItem element) => element.GetValue(ItemPaddingProperty);
    public static void SetItemPadding(MenuItem element, Thickness value) => element.SetValue(ItemPaddingProperty, value);

    #endregion

    #region ItemHoverBackground

    public static readonly AttachedProperty<IBrush?> ItemHoverBackgroundProperty =
        AvaloniaProperty.RegisterAttached<MenuItem, IBrush?>("ItemHoverBackground", typeof(MenuHelper));

    public static IBrush? GetItemHoverBackground(MenuItem element) => element.GetValue(ItemHoverBackgroundProperty);
    public static void SetItemHoverBackground(MenuItem element, IBrush? value) => element.SetValue(ItemHoverBackgroundProperty, value);

    #endregion

    #region ItemHoverForeground

    public static readonly AttachedProperty<IBrush?> ItemHoverForegroundProperty =
        AvaloniaProperty.RegisterAttached<MenuItem, IBrush?>("ItemHoverForeground", typeof(MenuHelper));

    public static IBrush? GetItemHoverForeground(MenuItem element) => element.GetValue(ItemHoverForegroundProperty);
    public static void SetItemHoverForeground(MenuItem element, IBrush? value) => element.SetValue(ItemHoverForegroundProperty, value);

    #endregion

    #region IconMargin

    public static readonly AttachedProperty<Thickness> IconMarginProperty =
        AvaloniaProperty.RegisterAttached<MenuItem, Thickness>("IconMargin", typeof(MenuHelper), new Thickness(0, 0, 8, 0));

    public static Thickness GetIconMargin(MenuItem element) => element.GetValue(IconMarginProperty);
    public static void SetIconMargin(MenuItem element, Thickness value) => element.SetValue(IconMarginProperty, value);

    #endregion

    #region SubmenuCornerRadius

    public static readonly AttachedProperty<CornerRadius> SubmenuCornerRadiusProperty =
        AvaloniaProperty.RegisterAttached<MenuItem, CornerRadius>("SubmenuCornerRadius", typeof(MenuHelper), new CornerRadius(8));

    public static CornerRadius GetSubmenuCornerRadius(MenuItem element) => element.GetValue(SubmenuCornerRadiusProperty);
    public static void SetSubmenuCornerRadius(MenuItem element, CornerRadius value) => element.SetValue(SubmenuCornerRadiusProperty, value);

    #endregion

    #region SubmenuShadow

    public static readonly AttachedProperty<BoxShadow> SubmenuShadowProperty =
        AvaloniaProperty.RegisterAttached<MenuItem, BoxShadow>("SubmenuShadow", typeof(MenuHelper));

    public static BoxShadow GetSubmenuShadow(MenuItem element) => element.GetValue(SubmenuShadowProperty);
    public static void SetSubmenuShadow(MenuItem element, BoxShadow value) => element.SetValue(SubmenuShadowProperty, value);

    #endregion

    #region SeparatorBrush

    public static readonly AttachedProperty<IBrush?> SeparatorBrushProperty =
        AvaloniaProperty.RegisterAttached<MenuItem, IBrush?>("SeparatorBrush", typeof(MenuHelper));

    public static IBrush? GetSeparatorBrush(MenuItem element) => element.GetValue(SeparatorBrushProperty);
    public static void SetSeparatorBrush(MenuItem element, IBrush? value) => element.SetValue(SeparatorBrushProperty, value);

    #endregion

    #region InputGestureForeground

    public static readonly AttachedProperty<IBrush?> InputGestureForegroundProperty =
        AvaloniaProperty.RegisterAttached<MenuItem, IBrush?>("InputGestureForeground", typeof(MenuHelper));

    public static IBrush? GetInputGestureForeground(MenuItem element) => element.GetValue(InputGestureForegroundProperty);
    public static void SetInputGestureForeground(MenuItem element, IBrush? value) => element.SetValue(InputGestureForegroundProperty, value);

    #endregion

    #region KeyboardNavigation

    public static readonly AttachedProperty<bool> KeyboardNavigationProperty =
        AvaloniaProperty.RegisterAttached<MenuItem, bool>("KeyboardNavigation", typeof(MenuHelper), true);

    public static bool GetKeyboardNavigation(MenuItem element) => element.GetValue(KeyboardNavigationProperty);
    public static void SetKeyboardNavigation(MenuItem element, bool value) => element.SetValue(KeyboardNavigationProperty, value);

    #endregion
}
