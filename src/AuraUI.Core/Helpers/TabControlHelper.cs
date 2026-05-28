using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace AuraUI.Core.Helpers;

/// <summary>
/// Attached properties for styling TabControl / TabItem.
/// Usage: aura:TabControlHelper.TabCornerRadius="8 8 0 0"
/// </summary>
public static class TabControlHelper
{
    #region TabCornerRadius

    public static readonly AttachedProperty<CornerRadius> TabCornerRadiusProperty =
        AvaloniaProperty.RegisterAttached<TabControl, CornerRadius>("TabCornerRadius", typeof(TabControlHelper));

    public static CornerRadius GetTabCornerRadius(TabControl element) => element.GetValue(TabCornerRadiusProperty);
    public static void SetTabCornerRadius(TabControl element, CornerRadius value) => element.SetValue(TabCornerRadiusProperty, value);

    #endregion

    #region TabPadding

    public static readonly AttachedProperty<Thickness> TabPaddingProperty =
        AvaloniaProperty.RegisterAttached<TabControl, Thickness>("TabPadding", typeof(TabControlHelper), new Thickness(16, 8));

    public static Thickness GetTabPadding(TabControl element) => element.GetValue(TabPaddingProperty);
    public static void SetTabPadding(TabControl element, Thickness value) => element.SetValue(TabPaddingProperty, value);

    #endregion

    #region SelectedTabBackground

    public static readonly AttachedProperty<IBrush?> SelectedTabBackgroundProperty =
        AvaloniaProperty.RegisterAttached<TabControl, IBrush?>("SelectedTabBackground", typeof(TabControlHelper));

    public static IBrush? GetSelectedTabBackground(TabControl element) => element.GetValue(SelectedTabBackgroundProperty);
    public static void SetSelectedTabBackground(TabControl element, IBrush? value) => element.SetValue(SelectedTabBackgroundProperty, value);

    #endregion

    #region SelectedTabForeground

    public static readonly AttachedProperty<IBrush?> SelectedTabForegroundProperty =
        AvaloniaProperty.RegisterAttached<TabControl, IBrush?>("SelectedTabForeground", typeof(TabControlHelper));

    public static IBrush? GetSelectedTabForeground(TabControl element) => element.GetValue(SelectedTabForegroundProperty);
    public static void SetSelectedTabForeground(TabControl element, IBrush? value) => element.SetValue(SelectedTabForegroundProperty, value);

    #endregion

    #region HoverTabBackground

    public static readonly AttachedProperty<IBrush?> HoverTabBackgroundProperty =
        AvaloniaProperty.RegisterAttached<TabControl, IBrush?>("HoverTabBackground", typeof(TabControlHelper));

    public static IBrush? GetHoverTabBackground(TabControl element) => element.GetValue(HoverTabBackgroundProperty);
    public static void SetHoverTabBackground(TabControl element, IBrush? value) => element.SetValue(HoverTabBackgroundProperty, value);

    #endregion

    #region TabStripBackground

    public static readonly AttachedProperty<IBrush?> TabStripBackgroundProperty =
        AvaloniaProperty.RegisterAttached<TabControl, IBrush?>("TabStripBackground", typeof(TabControlHelper));

    public static IBrush? GetTabStripBackground(TabControl element) => element.GetValue(TabStripBackgroundProperty);
    public static void SetTabStripBackground(TabControl element, IBrush? value) => element.SetValue(TabStripBackgroundProperty, value);

    #endregion

    #region IsClosable

    public static readonly AttachedProperty<bool> IsClosableProperty =
        AvaloniaProperty.RegisterAttached<TabControl, bool>("IsClosable", typeof(TabControlHelper));

    public static bool GetIsClosable(TabControl element) => element.GetValue(IsClosableProperty);
    public static void SetIsClosable(TabControl element, bool value) => element.SetValue(IsClosableProperty, value);

    #endregion

    #region CloseButtonHoverBackground

    public static readonly AttachedProperty<IBrush?> CloseButtonHoverBackgroundProperty =
        AvaloniaProperty.RegisterAttached<TabControl, IBrush?>("CloseButtonHoverBackground", typeof(TabControlHelper));

    public static IBrush? GetCloseButtonHoverBackground(TabControl element) => element.GetValue(CloseButtonHoverBackgroundProperty);
    public static void SetCloseButtonHoverBackground(TabControl element, IBrush? value) => element.SetValue(CloseButtonHoverBackgroundProperty, value);

    #endregion

    #region IndicatorBrush

    public static readonly AttachedProperty<IBrush?> IndicatorBrushProperty =
        AvaloniaProperty.RegisterAttached<TabControl, IBrush?>("IndicatorBrush", typeof(TabControlHelper));

    public static IBrush? GetIndicatorBrush(TabControl element) => element.GetValue(IndicatorBrushProperty);
    public static void SetIndicatorBrush(TabControl element, IBrush? value) => element.SetValue(IndicatorBrushProperty, value);

    #endregion

    #region IndicatorThickness

    public static readonly AttachedProperty<double> IndicatorThicknessProperty =
        AvaloniaProperty.RegisterAttached<TabControl, double>("IndicatorThickness", typeof(TabControlHelper), 2);

    public static double GetIndicatorThickness(TabControl element) => element.GetValue(IndicatorThicknessProperty);
    public static void SetIndicatorThickness(TabControl element, double value) => element.SetValue(IndicatorThicknessProperty, value);

    #endregion

    #region TabSpacing

    public static readonly AttachedProperty<double> TabSpacingProperty =
        AvaloniaProperty.RegisterAttached<TabControl, double>("TabSpacing", typeof(TabControlHelper));

    public static double GetTabSpacing(TabControl element) => element.GetValue(TabSpacingProperty);
    public static void SetTabSpacing(TabControl element, double value) => element.SetValue(TabSpacingProperty, value);

    #endregion

    #region Header

    public static readonly AttachedProperty<object?> HeaderProperty =
        AvaloniaProperty.RegisterAttached<TabControl, object?>("Header", typeof(TabControlHelper));

    public static object? GetHeader(TabControl element) => element.GetValue(HeaderProperty);
    public static void SetHeader(TabControl element, object? value) => element.SetValue(HeaderProperty, value);

    #endregion

    #region Footer

    public static readonly AttachedProperty<object?> FooterProperty =
        AvaloniaProperty.RegisterAttached<TabControl, object?>("Footer", typeof(TabControlHelper));

    public static object? GetFooter(TabControl element) => element.GetValue(FooterProperty);
    public static void SetFooter(TabControl element, object? value) => element.SetValue(FooterProperty, value);

    #endregion
}
