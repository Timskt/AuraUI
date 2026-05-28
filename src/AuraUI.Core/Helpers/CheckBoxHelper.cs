using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace AuraUI.Core.Helpers;

/// <summary>
/// Attached properties for styling CheckBox controls.
/// Usage: aura:CheckBoxHelper.CornerRadius="4"
/// </summary>
public static class CheckBoxHelper
{
    #region CornerRadius

    public static readonly AttachedProperty<CornerRadius> CornerRadiusProperty =
        AvaloniaProperty.RegisterAttached<CheckBox, CornerRadius>("CornerRadius", typeof(CheckBoxHelper), new CornerRadius(4));

    public static CornerRadius GetCornerRadius(CheckBox element) => element.GetValue(CornerRadiusProperty);
    public static void SetCornerRadius(CheckBox element, CornerRadius value) => element.SetValue(CornerRadiusProperty, value);

    #endregion

    #region CheckedBackground

    public static readonly AttachedProperty<IBrush?> CheckedBackgroundProperty =
        AvaloniaProperty.RegisterAttached<CheckBox, IBrush?>("CheckedBackground", typeof(CheckBoxHelper));

    public static IBrush? GetCheckedBackground(CheckBox element) => element.GetValue(CheckedBackgroundProperty);
    public static void SetCheckedBackground(CheckBox element, IBrush? value) => element.SetValue(CheckedBackgroundProperty, value);

    #endregion

    #region CheckedForeground

    public static readonly AttachedProperty<IBrush?> CheckedForegroundProperty =
        AvaloniaProperty.RegisterAttached<CheckBox, IBrush?>("CheckedForeground", typeof(CheckBoxHelper));

    public static IBrush? GetCheckedForeground(CheckBox element) => element.GetValue(CheckedForegroundProperty);
    public static void SetCheckedForeground(CheckBox element, IBrush? value) => element.SetValue(CheckedForegroundProperty, value);

    #endregion

    #region UncheckedBackground

    public static readonly AttachedProperty<IBrush?> UncheckedBackgroundProperty =
        AvaloniaProperty.RegisterAttached<CheckBox, IBrush?>("UncheckedBackground", typeof(CheckBoxHelper));

    public static IBrush? GetUncheckedBackground(CheckBox element) => element.GetValue(UncheckedBackgroundProperty);
    public static void SetUncheckedBackground(CheckBox element, IBrush? value) => element.SetValue(UncheckedBackgroundProperty, value);

    #endregion

    #region UncheckedBorderBrush

    public static readonly AttachedProperty<IBrush?> UncheckedBorderBrushProperty =
        AvaloniaProperty.RegisterAttached<CheckBox, IBrush?>("UncheckedBorderBrush", typeof(CheckBoxHelper));

    public static IBrush? GetUncheckedBorderBrush(CheckBox element) => element.GetValue(UncheckedBorderBrushProperty);
    public static void SetUncheckedBorderBrush(CheckBox element, IBrush? value) => element.SetValue(UncheckedBorderBrushProperty, value);

    #endregion

    #region CheckMarkColor

    public static readonly AttachedProperty<IBrush?> CheckMarkColorProperty =
        AvaloniaProperty.RegisterAttached<CheckBox, IBrush?>("CheckMarkColor", typeof(CheckBoxHelper));

    public static IBrush? GetCheckMarkColor(CheckBox element) => element.GetValue(CheckMarkColorProperty);
    public static void SetCheckMarkColor(CheckBox element, IBrush? value) => element.SetValue(CheckMarkColorProperty, value);

    #endregion

    #region BoxSize

    public static readonly AttachedProperty<double> BoxSizeProperty =
        AvaloniaProperty.RegisterAttached<CheckBox, double>("BoxSize", typeof(CheckBoxHelper), 18);

    public static double GetBoxSize(CheckBox element) => element.GetValue(BoxSizeProperty);
    public static void SetBoxSize(CheckBox element, double value) => element.SetValue(BoxSizeProperty, value);

    #endregion

    #region BorderThickness

    public static readonly AttachedProperty<Thickness> BorderThicknessProperty =
        AvaloniaProperty.RegisterAttached<CheckBox, Thickness>("BorderThickness", typeof(CheckBoxHelper), new Thickness(1.5));

    public static Thickness GetBorderThickness(CheckBox element) => element.GetValue(BorderThicknessProperty);
    public static void SetBorderThickness(CheckBox element, Thickness value) => element.SetValue(BorderThicknessProperty, value);

    #endregion

    #region HoverBackground

    public static readonly AttachedProperty<IBrush?> HoverBackgroundProperty =
        AvaloniaProperty.RegisterAttached<CheckBox, IBrush?>("HoverBackground", typeof(CheckBoxHelper));

    public static IBrush? GetHoverBackground(CheckBox element) => element.GetValue(HoverBackgroundProperty);
    public static void SetHoverBackground(CheckBox element, IBrush? value) => element.SetValue(HoverBackgroundProperty, value);

    #endregion

    #region IndeterminateBackground

    public static readonly AttachedProperty<IBrush?> IndeterminateBackgroundProperty =
        AvaloniaProperty.RegisterAttached<CheckBox, IBrush?>("IndeterminateBackground", typeof(CheckBoxHelper));

    public static IBrush? GetIndeterminateBackground(CheckBox element) => element.GetValue(IndeterminateBackgroundProperty);
    public static void SetIndeterminateBackground(CheckBox element, IBrush? value) => element.SetValue(IndeterminateBackgroundProperty, value);

    #endregion
}
