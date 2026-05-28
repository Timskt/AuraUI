using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace AuraUI.Core.Helpers;

/// <summary>
/// Attached properties for styling TreeView / TreeViewItem controls.
/// Usage: aura:TreeViewHelper.ItemCornerRadius="4"
/// </summary>
public static class TreeViewHelper
{
    #region ItemCornerRadius

    public static readonly AttachedProperty<CornerRadius> ItemCornerRadiusProperty =
        AvaloniaProperty.RegisterAttached<TreeView, CornerRadius>("ItemCornerRadius", typeof(TreeViewHelper), new CornerRadius(4));

    public static CornerRadius GetItemCornerRadius(TreeView element) => element.GetValue(ItemCornerRadiusProperty);
    public static void SetItemCornerRadius(TreeView element, CornerRadius value) => element.SetValue(ItemCornerRadiusProperty, value);

    #endregion

    #region ItemPadding

    public static readonly AttachedProperty<Thickness> ItemPaddingProperty =
        AvaloniaProperty.RegisterAttached<TreeView, Thickness>("ItemPadding", typeof(TreeViewHelper), new Thickness(8, 4));

    public static Thickness GetItemPadding(TreeView element) => element.GetValue(ItemPaddingProperty);
    public static void SetItemPadding(TreeView element, Thickness value) => element.SetValue(ItemPaddingProperty, value);

    #endregion

    #region ItemHoverBackground

    public static readonly AttachedProperty<IBrush?> ItemHoverBackgroundProperty =
        AvaloniaProperty.RegisterAttached<TreeView, IBrush?>("ItemHoverBackground", typeof(TreeViewHelper));

    public static IBrush? GetItemHoverBackground(TreeView element) => element.GetValue(ItemHoverBackgroundProperty);
    public static void SetItemHoverBackground(TreeView element, IBrush? value) => element.SetValue(ItemHoverBackgroundProperty, value);

    #endregion

    #region ItemSelectedBackground

    public static readonly AttachedProperty<IBrush?> ItemSelectedBackgroundProperty =
        AvaloniaProperty.RegisterAttached<TreeView, IBrush?>("ItemSelectedBackground", typeof(TreeViewHelper));

    public static IBrush? GetItemSelectedBackground(TreeView element) => element.GetValue(ItemSelectedBackgroundProperty);
    public static void SetItemSelectedBackground(TreeView element, IBrush? value) => element.SetValue(ItemSelectedBackgroundProperty, value);

    #endregion

    #region ItemSelectedForeground

    public static readonly AttachedProperty<IBrush?> ItemSelectedForegroundProperty =
        AvaloniaProperty.RegisterAttached<TreeView, IBrush?>("ItemSelectedForeground", typeof(TreeViewHelper));

    public static IBrush? GetItemSelectedForeground(TreeView element) => element.GetValue(ItemSelectedForegroundProperty);
    public static void SetItemSelectedForeground(TreeView element, IBrush? value) => element.SetValue(ItemSelectedForegroundProperty, value);

    #endregion

    #region IndentSize

    public static readonly AttachedProperty<double> IndentSizeProperty =
        AvaloniaProperty.RegisterAttached<TreeView, double>("IndentSize", typeof(TreeViewHelper), 16);

    public static double GetIndentSize(TreeView element) => element.GetValue(IndentSizeProperty);
    public static void SetIndentSize(TreeView element, double value) => element.SetValue(IndentSizeProperty, value);

    #endregion

    #region ExpandIcon

    public static readonly AttachedProperty<object?> ExpandIconProperty =
        AvaloniaProperty.RegisterAttached<TreeView, object?>("ExpandIcon", typeof(TreeViewHelper));

    public static object? GetExpandIcon(TreeView element) => element.GetValue(ExpandIconProperty);
    public static void SetExpandIcon(TreeView element, object? value) => element.SetValue(ExpandIconProperty, value);

    #endregion

    #region CollapseIcon

    public static readonly AttachedProperty<object?> CollapseIconProperty =
        AvaloniaProperty.RegisterAttached<TreeView, object?>("CollapseIcon", typeof(TreeViewHelper));

    public static object? GetCollapseIcon(TreeView element) => element.GetValue(CollapseIconProperty);
    public static void SetCollapseIcon(TreeView element, object? value) => element.SetValue(CollapseIconProperty, value);

    #endregion

    #region LeafIcon

    public static readonly AttachedProperty<object?> LeafIconProperty =
        AvaloniaProperty.RegisterAttached<TreeView, object?>("LeafIcon", typeof(TreeViewHelper));

    public static object? GetLeafIcon(TreeView element) => element.GetValue(LeafIconProperty);
    public static void SetLeafIcon(TreeView element, object? value) => element.SetValue(LeafIconProperty, value);

    #endregion

    #region ShowLines

    public static readonly AttachedProperty<bool> ShowLinesProperty =
        AvaloniaProperty.RegisterAttached<TreeView, bool>("ShowLines", typeof(TreeViewHelper));

    public static bool GetShowLines(TreeView element) => element.GetValue(ShowLinesProperty);
    public static void SetShowLines(TreeView element, bool value) => element.SetValue(ShowLinesProperty, value);

    #endregion

    #region LineBrush

    public static readonly AttachedProperty<IBrush?> LineBrushProperty =
        AvaloniaProperty.RegisterAttached<TreeView, IBrush?>("LineBrush", typeof(TreeViewHelper));

    public static IBrush? GetLineBrush(TreeView element) => element.GetValue(LineBrushProperty);
    public static void SetLineBrush(TreeView element, IBrush? value) => element.SetValue(LineBrushProperty, value);

    #endregion
}
