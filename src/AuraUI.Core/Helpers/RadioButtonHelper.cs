using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace AuraUI.Core.Helpers;

/// <summary>
/// Attached properties for styling RadioButton controls.
/// Usage: aura:RadioButtonHelper.CircleSize="20"
/// </summary>
public static class RadioButtonHelper
{
    #region CircleSize

    public static readonly AttachedProperty<double> CircleSizeProperty =
        AvaloniaProperty.RegisterAttached<RadioButton, double>("CircleSize", typeof(RadioButtonHelper), 20);

    public static double GetCircleSize(RadioButton element) => element.GetValue(CircleSizeProperty);
    public static void SetCircleSize(RadioButton element, double value) => element.SetValue(CircleSizeProperty, value);

    #endregion

    #region CornerRadius

    public static readonly AttachedProperty<CornerRadius> CornerRadiusProperty =
        AvaloniaProperty.RegisterAttached<RadioButton, CornerRadius>("CornerRadius", typeof(RadioButtonHelper), new CornerRadius(4));

    public static CornerRadius GetCornerRadius(RadioButton element) => element.GetValue(CornerRadiusProperty);
    public static void SetCornerRadius(RadioButton element, CornerRadius value) => element.SetValue(CornerRadiusProperty, value);

    #endregion

    #region CheckedBackground

    public static readonly AttachedProperty<IBrush?> CheckedBackgroundProperty =
        AvaloniaProperty.RegisterAttached<RadioButton, IBrush?>("CheckedBackground", typeof(RadioButtonHelper));

    public static IBrush? GetCheckedBackground(RadioButton element) => element.GetValue(CheckedBackgroundProperty);
    public static void SetCheckedBackground(RadioButton element, IBrush? value) => element.SetValue(CheckedBackgroundProperty, value);

    #endregion

    #region CheckedForeground

    public static readonly AttachedProperty<IBrush?> CheckedForegroundProperty =
        AvaloniaProperty.RegisterAttached<RadioButton, IBrush?>("CheckedForeground", typeof(RadioButtonHelper));

    public static IBrush? GetCheckedForeground(RadioButton element) => element.GetValue(CheckedForegroundProperty);
    public static void SetCheckedForeground(RadioButton element, IBrush? value) => element.SetValue(CheckedForegroundProperty, value);

    #endregion

    #region UncheckedBackground

    public static readonly AttachedProperty<IBrush?> UncheckedBackgroundProperty =
        AvaloniaProperty.RegisterAttached<RadioButton, IBrush?>("UncheckedBackground", typeof(RadioButtonHelper));

    public static IBrush? GetUncheckedBackground(RadioButton element) => element.GetValue(UncheckedBackgroundProperty);
    public static void SetUncheckedBackground(RadioButton element, IBrush? value) => element.SetValue(UncheckedBackgroundProperty, value);

    #endregion

    #region UncheckedBorderBrush

    public static readonly AttachedProperty<IBrush?> UncheckedBorderBrushProperty =
        AvaloniaProperty.RegisterAttached<RadioButton, IBrush?>("UncheckedBorderBrush", typeof(RadioButtonHelper));

    public static IBrush? GetUncheckedBorderBrush(RadioButton element) => element.GetValue(UncheckedBorderBrushProperty);
    public static void SetUncheckedBorderBrush(RadioButton element, IBrush? value) => element.SetValue(UncheckedBorderBrushProperty, value);

    #endregion

    #region DotSize

    public static readonly AttachedProperty<double> DotSizeProperty =
        AvaloniaProperty.RegisterAttached<RadioButton, double>("DotSize", typeof(RadioButtonHelper), 10);

    public static double GetDotSize(RadioButton element) => element.GetValue(DotSizeProperty);
    public static void SetDotSize(RadioButton element, double value) => element.SetValue(DotSizeProperty, value);

    #endregion

    #region DotBrush

    public static readonly AttachedProperty<IBrush?> DotBrushProperty =
        AvaloniaProperty.RegisterAttached<RadioButton, IBrush?>("DotBrush", typeof(RadioButtonHelper));

    public static IBrush? GetDotBrush(RadioButton element) => element.GetValue(DotBrushProperty);
    public static void SetDotBrush(RadioButton element, IBrush? value) => element.SetValue(DotBrushProperty, value);

    #endregion

    #region HoverBackground

    public static readonly AttachedProperty<IBrush?> HoverBackgroundProperty =
        AvaloniaProperty.RegisterAttached<RadioButton, IBrush?>("HoverBackground", typeof(RadioButtonHelper));

    public static IBrush? GetHoverBackground(RadioButton element) => element.GetValue(HoverBackgroundProperty);
    public static void SetHoverBackground(RadioButton element, IBrush? value) => element.SetValue(HoverBackgroundProperty, value);

    #endregion

    #region BorderThickness

    public static readonly AttachedProperty<Thickness> BorderThicknessProperty =
        AvaloniaProperty.RegisterAttached<RadioButton, Thickness>("BorderThickness", typeof(RadioButtonHelper), new Thickness(1.5));

    public static Thickness GetBorderThickness(RadioButton element) => element.GetValue(BorderThicknessProperty);
    public static void SetBorderThickness(RadioButton element, Thickness value) => element.SetValue(BorderThicknessProperty, value);

    #endregion
}
