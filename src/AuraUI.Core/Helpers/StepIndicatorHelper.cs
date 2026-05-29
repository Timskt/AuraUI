using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace AuraUI.Core.Helpers;

/// <summary>
/// Attached properties for styling StepIndicator controls.
/// Usage: aura:StepIndicatorHelper.CircleSize="32"
/// </summary>
public static class StepIndicatorHelper
{
    #region CircleSize

    public static readonly AttachedProperty<double> CircleSizeProperty =
        AvaloniaProperty.RegisterAttached<Control, double>("CircleSize", typeof(StepIndicatorHelper), 32);

    public static double GetCircleSize(Control element) => element.GetValue(CircleSizeProperty);
    public static void SetCircleSize(Control element, double value) => element.SetValue(CircleSizeProperty, value);

    #endregion

    #region CircleBackground

    public static readonly AttachedProperty<IBrush?> CircleBackgroundProperty =
        AvaloniaProperty.RegisterAttached<Control, IBrush?>("CircleBackground", typeof(StepIndicatorHelper));

    public static IBrush? GetCircleBackground(Control element) => element.GetValue(CircleBackgroundProperty);
    public static void SetCircleBackground(Control element, IBrush? value) => element.SetValue(CircleBackgroundProperty, value);

    #endregion

    #region ActiveColor

    public static readonly AttachedProperty<IBrush?> ActiveColorProperty =
        AvaloniaProperty.RegisterAttached<Control, IBrush?>("ActiveColor", typeof(StepIndicatorHelper));

    public static IBrush? GetActiveColor(Control element) => element.GetValue(ActiveColorProperty);
    public static void SetActiveColor(Control element, IBrush? value) => element.SetValue(ActiveColorProperty, value);

    #endregion

    #region CompletedColor

    public static readonly AttachedProperty<IBrush?> CompletedColorProperty =
        AvaloniaProperty.RegisterAttached<Control, IBrush?>("CompletedColor", typeof(StepIndicatorHelper));

    public static IBrush? GetCompletedColor(Control element) => element.GetValue(CompletedColorProperty);
    public static void SetCompletedColor(Control element, IBrush? value) => element.SetValue(CompletedColorProperty, value);

    #endregion

    #region ConnectorBrush

    public static readonly AttachedProperty<IBrush?> ConnectorBrushProperty =
        AvaloniaProperty.RegisterAttached<Control, IBrush?>("ConnectorBrush", typeof(StepIndicatorHelper));

    public static IBrush? GetConnectorBrush(Control element) => element.GetValue(ConnectorBrushProperty);
    public static void SetConnectorBrush(Control element, IBrush? value) => element.SetValue(ConnectorBrushProperty, value);

    #endregion

    #region ConnectorThickness

    public static readonly AttachedProperty<double> ConnectorThicknessProperty =
        AvaloniaProperty.RegisterAttached<Control, double>("ConnectorThickness", typeof(StepIndicatorHelper), 2);

    public static double GetConnectorThickness(Control element) => element.GetValue(ConnectorThicknessProperty);
    public static void SetConnectorThickness(Control element, double value) => element.SetValue(ConnectorThicknessProperty, value);

    #endregion

    #region CircleCornerRadius

    public static readonly AttachedProperty<CornerRadius> CircleCornerRadiusProperty =
        AvaloniaProperty.RegisterAttached<Control, CornerRadius>("CircleCornerRadius", typeof(StepIndicatorHelper), new CornerRadius(999));

    public static CornerRadius GetCircleCornerRadius(Control element) => element.GetValue(CircleCornerRadiusProperty);
    public static void SetCircleCornerRadius(Control element, CornerRadius value) => element.SetValue(CircleCornerRadiusProperty, value);

    #endregion

    #region ActiveForeground

    public static readonly AttachedProperty<IBrush?> ActiveForegroundProperty =
        AvaloniaProperty.RegisterAttached<Control, IBrush?>("ActiveForeground", typeof(StepIndicatorHelper));

    public static IBrush? GetActiveForeground(Control element) => element.GetValue(ActiveForegroundProperty);
    public static void SetActiveForeground(Control element, IBrush? value) => element.SetValue(ActiveForegroundProperty, value);

    #endregion
}
