using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Media;

namespace AuraUI.Core.Helpers;

/// <summary>
/// Attached properties for styling ToggleButton controls.
/// Usage: aura:ToggleButtonHelper.CornerRadius="20"
/// </summary>
public static class ToggleButtonHelper
{
    #region CornerRadius

    public static readonly AttachedProperty<CornerRadius> CornerRadiusProperty =
        AvaloniaProperty.RegisterAttached<ToggleButton, CornerRadius>("CornerRadius", typeof(ToggleButtonHelper));

    public static CornerRadius GetCornerRadius(ToggleButton element) => element.GetValue(CornerRadiusProperty);
    public static void SetCornerRadius(ToggleButton element, CornerRadius value) => element.SetValue(CornerRadiusProperty, value);

    #endregion

    #region CheckedBackground

    public static readonly AttachedProperty<IBrush?> CheckedBackgroundProperty =
        AvaloniaProperty.RegisterAttached<ToggleButton, IBrush?>("CheckedBackground", typeof(ToggleButtonHelper));

    public static IBrush? GetCheckedBackground(ToggleButton element) => element.GetValue(CheckedBackgroundProperty);
    public static void SetCheckedBackground(ToggleButton element, IBrush? value) => element.SetValue(CheckedBackgroundProperty, value);

    #endregion

    #region CheckedForeground

    public static readonly AttachedProperty<IBrush?> CheckedForegroundProperty =
        AvaloniaProperty.RegisterAttached<ToggleButton, IBrush?>("CheckedForeground", typeof(ToggleButtonHelper));

    public static IBrush? GetCheckedForeground(ToggleButton element) => element.GetValue(CheckedForegroundProperty);
    public static void SetCheckedForeground(ToggleButton element, IBrush? value) => element.SetValue(CheckedForegroundProperty, value);

    #endregion

    #region CheckedBorderBrush

    public static readonly AttachedProperty<IBrush?> CheckedBorderBrushProperty =
        AvaloniaProperty.RegisterAttached<ToggleButton, IBrush?>("CheckedBorderBrush", typeof(ToggleButtonHelper));

    public static IBrush? GetCheckedBorderBrush(ToggleButton element) => element.GetValue(CheckedBorderBrushProperty);
    public static void SetCheckedBorderBrush(ToggleButton element, IBrush? value) => element.SetValue(CheckedBorderBrushProperty, value);

    #endregion

    #region UncheckedBackground

    public static readonly AttachedProperty<IBrush?> UncheckedBackgroundProperty =
        AvaloniaProperty.RegisterAttached<ToggleButton, IBrush?>("UncheckedBackground", typeof(ToggleButtonHelper));

    public static IBrush? GetUncheckedBackground(ToggleButton element) => element.GetValue(UncheckedBackgroundProperty);
    public static void SetUncheckedBackground(ToggleButton element, IBrush? value) => element.SetValue(UncheckedBackgroundProperty, value);

    #endregion

    #region UncheckedForeground

    public static readonly AttachedProperty<IBrush?> UncheckedForegroundProperty =
        AvaloniaProperty.RegisterAttached<ToggleButton, IBrush?>("UncheckedForeground", typeof(ToggleButtonHelper));

    public static IBrush? GetUncheckedForeground(ToggleButton element) => element.GetValue(UncheckedForegroundProperty);
    public static void SetUncheckedForeground(ToggleButton element, IBrush? value) => element.SetValue(UncheckedForegroundProperty, value);

    #endregion

    #region UncheckedBorderBrush

    public static readonly AttachedProperty<IBrush?> UncheckedBorderBrushProperty =
        AvaloniaProperty.RegisterAttached<ToggleButton, IBrush?>("UncheckedBorderBrush", typeof(ToggleButtonHelper));

    public static IBrush? GetUncheckedBorderBrush(ToggleButton element) => element.GetValue(UncheckedBorderBrushProperty);
    public static void SetUncheckedBorderBrush(ToggleButton element, IBrush? value) => element.SetValue(UncheckedBorderBrushProperty, value);

    #endregion

    #region HoverBackground

    public static readonly AttachedProperty<IBrush?> HoverBackgroundProperty =
        AvaloniaProperty.RegisterAttached<ToggleButton, IBrush?>("HoverBackground", typeof(ToggleButtonHelper));

    public static IBrush? GetHoverBackground(ToggleButton element) => element.GetValue(HoverBackgroundProperty);
    public static void SetHoverBackground(ToggleButton element, IBrush? value) => element.SetValue(HoverBackgroundProperty, value);

    #endregion

    #region ClickBackground

    public static readonly AttachedProperty<IBrush?> ClickBackgroundProperty =
        AvaloniaProperty.RegisterAttached<ToggleButton, IBrush?>("ClickBackground", typeof(ToggleButtonHelper));

    public static IBrush? GetClickBackground(ToggleButton element) => element.GetValue(ClickBackgroundProperty);
    public static void SetClickBackground(ToggleButton element, IBrush? value) => element.SetValue(ClickBackgroundProperty, value);

    #endregion

    #region Icon

    public static readonly AttachedProperty<object?> IconProperty =
        AvaloniaProperty.RegisterAttached<ToggleButton, object?>("Icon", typeof(ToggleButtonHelper));

    public static object? GetIcon(ToggleButton element) => element.GetValue(IconProperty);
    public static void SetIcon(ToggleButton element, object? value) => element.SetValue(IconProperty, value);

    #endregion

    #region IconMargin

    public static readonly AttachedProperty<Thickness> IconMarginProperty =
        AvaloniaProperty.RegisterAttached<ToggleButton, Thickness>("IconMargin", typeof(ToggleButtonHelper), new Thickness(0, 0, 4, 0));

    public static Thickness GetIconMargin(ToggleButton element) => element.GetValue(IconMarginProperty);
    public static void SetIconMargin(ToggleButton element, Thickness value) => element.SetValue(IconMarginProperty, value);

    #endregion

    #region Shadow

    public static readonly AttachedProperty<BoxShadow> ShadowProperty =
        AvaloniaProperty.RegisterAttached<ToggleButton, BoxShadow>("Shadow", typeof(ToggleButtonHelper));

    public static BoxShadow GetShadow(ToggleButton element) => element.GetValue(ShadowProperty);
    public static void SetShadow(ToggleButton element, BoxShadow value) => element.SetValue(ShadowProperty, value);

    #endregion

    #region ContentPadding

    public static readonly AttachedProperty<Thickness> ContentPaddingProperty =
        AvaloniaProperty.RegisterAttached<ToggleButton, Thickness>("ContentPadding", typeof(ToggleButtonHelper), new Thickness(16, 8));

    public static Thickness GetContentPadding(ToggleButton element) => element.GetValue(ContentPaddingProperty);
    public static void SetContentPadding(ToggleButton element, Thickness value) => element.SetValue(ContentPaddingProperty, value);

    #endregion
}
