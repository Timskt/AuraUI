using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace AuraUI.Core.Helpers;

/// <summary>
/// Attached properties for styling ListBox controls.
/// Usage: aura:ListBoxHelper.ItemCornerRadius="8"
/// </summary>
public static class ListBoxHelper
{
    #region ItemCornerRadius

    public static readonly AttachedProperty<CornerRadius> ItemCornerRadiusProperty =
        AvaloniaProperty.RegisterAttached<ListBox, CornerRadius>("ItemCornerRadius", typeof(ListBoxHelper));

    public static CornerRadius GetItemCornerRadius(ListBox element) => element.GetValue(ItemCornerRadiusProperty);
    public static void SetItemCornerRadius(ListBox element, CornerRadius value) => element.SetValue(ItemCornerRadiusProperty, value);

    #endregion

    #region ItemPadding

    public static readonly AttachedProperty<Thickness> ItemPaddingProperty =
        AvaloniaProperty.RegisterAttached<ListBox, Thickness>("ItemPadding", typeof(ListBoxHelper), new Thickness(8, 6));

    public static Thickness GetItemPadding(ListBox element) => element.GetValue(ItemPaddingProperty);
    public static void SetItemPadding(ListBox element, Thickness value) => element.SetValue(ItemPaddingProperty, value);

    #endregion

    #region ItemHoverBackground

    public static readonly AttachedProperty<IBrush?> ItemHoverBackgroundProperty =
        AvaloniaProperty.RegisterAttached<ListBox, IBrush?>("ItemHoverBackground", typeof(ListBoxHelper));

    public static IBrush? GetItemHoverBackground(ListBox element) => element.GetValue(ItemHoverBackgroundProperty);
    public static void SetItemHoverBackground(ListBox element, IBrush? value) => element.SetValue(ItemHoverBackgroundProperty, value);

    #endregion

    #region ItemSelectedBackground

    public static readonly AttachedProperty<IBrush?> ItemSelectedBackgroundProperty =
        AvaloniaProperty.RegisterAttached<ListBox, IBrush?>("ItemSelectedBackground", typeof(ListBoxHelper));

    public static IBrush? GetItemSelectedBackground(ListBox element) => element.GetValue(ItemSelectedBackgroundProperty);
    public static void SetItemSelectedBackground(ListBox element, IBrush? value) => element.SetValue(ItemSelectedBackgroundProperty, value);

    #endregion

    #region ItemSelectedForeground

    public static readonly AttachedProperty<IBrush?> ItemSelectedForegroundProperty =
        AvaloniaProperty.RegisterAttached<ListBox, IBrush?>("ItemSelectedForeground", typeof(ListBoxHelper));

    public static IBrush? GetItemSelectedForeground(ListBox element) => element.GetValue(ItemSelectedForegroundProperty);
    public static void SetItemSelectedForeground(ListBox element, IBrush? value) => element.SetValue(ItemSelectedForegroundProperty, value);

    #endregion

    #region ItemSpacing

    public static readonly AttachedProperty<double> ItemSpacingProperty =
        AvaloniaProperty.RegisterAttached<ListBox, double>("ItemSpacing", typeof(ListBoxHelper));

    public static double GetItemSpacing(ListBox element) => element.GetValue(ItemSpacingProperty);
    public static void SetItemSpacing(ListBox element, double value) => element.SetValue(ItemSpacingProperty, value);

    #endregion

    #region ShowScrollBar

    public static readonly AttachedProperty<bool> ShowScrollBarProperty =
        AvaloniaProperty.RegisterAttached<ListBox, bool>("ShowScrollBar", typeof(ListBoxHelper), true);

    public static bool GetShowScrollBar(ListBox element) => element.GetValue(ShowScrollBarProperty);
    public static void SetShowScrollBar(ListBox element, bool value) => element.SetValue(ShowScrollBarProperty, value);

    #endregion

    #region ScrollBarWidth

    public static readonly AttachedProperty<double> ScrollBarWidthProperty =
        AvaloniaProperty.RegisterAttached<ListBox, double>("ScrollBarWidth", typeof(ListBoxHelper), 8);

    public static double GetScrollBarWidth(ListBox element) => element.GetValue(ScrollBarWidthProperty);
    public static void SetScrollBarWidth(ListBox element, double value) => element.SetValue(ScrollBarWidthProperty, value);

    #endregion

    #region ItemIcon

    public static readonly AttachedProperty<object?> ItemIconProperty =
        AvaloniaProperty.RegisterAttached<ListBox, object?>("ItemIcon", typeof(ListBoxHelper));

    public static object? GetItemIcon(ListBox element) => element.GetValue(ItemIconProperty);
    public static void SetItemIcon(ListBox element, object? value) => element.SetValue(ItemIconProperty, value);

    #endregion

    #region IsVirtualized

    public static readonly AttachedProperty<bool> IsVirtualizedProperty =
        AvaloniaProperty.RegisterAttached<ListBox, bool>("IsVirtualized", typeof(ListBoxHelper), true);

    public static bool GetIsVirtualized(ListBox element) => element.GetValue(IsVirtualizedProperty);
    public static void SetIsVirtualized(ListBox element, bool value) => element.SetValue(IsVirtualizedProperty, value);

    #endregion
}
