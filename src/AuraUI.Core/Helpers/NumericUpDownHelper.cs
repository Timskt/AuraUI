using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace AuraUI.Core.Helpers;

/// <summary>
/// Attached properties for styling NumericUpDown controls.
/// Usage: aura:NumericUpDownHelper.CornerRadius="8"
/// </summary>
public static class NumericUpDownHelper
{
    #region CornerRadius

    public static readonly AttachedProperty<CornerRadius> CornerRadiusProperty =
        AvaloniaProperty.RegisterAttached<NumericUpDown, CornerRadius>("CornerRadius", typeof(NumericUpDownHelper), new CornerRadius(4));

    public static CornerRadius GetCornerRadius(NumericUpDown element) => element.GetValue(CornerRadiusProperty);
    public static void SetCornerRadius(NumericUpDown element, CornerRadius value) => element.SetValue(CornerRadiusProperty, value);

    #endregion

    #region ButtonWidth

    public static readonly AttachedProperty<double> ButtonWidthProperty =
        AvaloniaProperty.RegisterAttached<NumericUpDown, double>("ButtonWidth", typeof(NumericUpDownHelper), 32);

    public static double GetButtonWidth(NumericUpDown element) => element.GetValue(ButtonWidthProperty);
    public static void SetButtonWidth(NumericUpDown element, double value) => element.SetValue(ButtonWidthProperty, value);

    #endregion

    #region ButtonBackground

    public static readonly AttachedProperty<IBrush?> ButtonBackgroundProperty =
        AvaloniaProperty.RegisterAttached<NumericUpDown, IBrush?>("ButtonBackground", typeof(NumericUpDownHelper));

    public static IBrush? GetButtonBackground(NumericUpDown element) => element.GetValue(ButtonBackgroundProperty);
    public static void SetButtonBackground(NumericUpDown element, IBrush? value) => element.SetValue(ButtonBackgroundProperty, value);

    #endregion

    #region ButtonHoverBackground

    public static readonly AttachedProperty<IBrush?> ButtonHoverBackgroundProperty =
        AvaloniaProperty.RegisterAttached<NumericUpDown, IBrush?>("ButtonHoverBackground", typeof(NumericUpDownHelper));

    public static IBrush? GetButtonHoverBackground(NumericUpDown element) => element.GetValue(ButtonHoverBackgroundProperty);
    public static void SetButtonHoverBackground(NumericUpDown element, IBrush? value) => element.SetValue(ButtonHoverBackgroundProperty, value);

    #endregion

    #region FocusBorderBrush

    public static readonly AttachedProperty<IBrush?> FocusBorderBrushProperty =
        AvaloniaProperty.RegisterAttached<NumericUpDown, IBrush?>("FocusBorderBrush", typeof(NumericUpDownHelper));

    public static IBrush? GetFocusBorderBrush(NumericUpDown element) => element.GetValue(FocusBorderBrushProperty);
    public static void SetFocusBorderBrush(NumericUpDown element, IBrush? value) => element.SetValue(FocusBorderBrushProperty, value);

    #endregion

    #region HoverBorderBrush

    public static readonly AttachedProperty<IBrush?> HoverBorderBrushProperty =
        AvaloniaProperty.RegisterAttached<NumericUpDown, IBrush?>("HoverBorderBrush", typeof(NumericUpDownHelper));

    public static IBrush? GetHoverBorderBrush(NumericUpDown element) => element.GetValue(HoverBorderBrushProperty);
    public static void SetHoverBorderBrush(NumericUpDown element, IBrush? value) => element.SetValue(HoverBorderBrushProperty, value);

    #endregion

    #region ShowButtons

    public static readonly AttachedProperty<bool> ShowButtonsProperty =
        AvaloniaProperty.RegisterAttached<NumericUpDown, bool>("ShowButtons", typeof(NumericUpDownHelper), true);

    public static bool GetShowButtons(NumericUpDown element) => element.GetValue(ShowButtonsProperty);
    public static void SetShowButtons(NumericUpDown element, bool value) => element.SetValue(ShowButtonsProperty, value);

    #endregion

    #region Label

    public static readonly AttachedProperty<string?> LabelProperty =
        AvaloniaProperty.RegisterAttached<NumericUpDown, string?>("Label", typeof(NumericUpDownHelper));

    public static string? GetLabel(NumericUpDown element) => element.GetValue(LabelProperty);
    public static void SetLabel(NumericUpDown element, string? value) => element.SetValue(LabelProperty, value);

    #endregion
}
