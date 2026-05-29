using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace AuraUI.Core.Helpers;

/// <summary>
/// Attached properties for styling Avatar controls.
/// Usage: aura:AvatarHelper.Size="48"
/// </summary>
public static class AvatarHelper
{
    #region Size

    public static readonly AttachedProperty<double> SizeProperty =
        AvaloniaProperty.RegisterAttached<Control, double>("Size", typeof(AvatarHelper), 36);

    public static double GetSize(Control element) => element.GetValue(SizeProperty);
    public static void SetSize(Control element, double value) => element.SetValue(SizeProperty, value);

    #endregion

    #region Shape

    public static readonly AttachedProperty<AvatarShapeAttached> ShapeProperty =
        AvaloniaProperty.RegisterAttached<Control, AvatarShapeAttached>("Shape", typeof(AvatarHelper), AvatarShapeAttached.Circle);

    public static AvatarShapeAttached GetShape(Control element) => element.GetValue(ShapeProperty);
    public static void SetShape(Control element, AvatarShapeAttached value) => element.SetValue(ShapeProperty, value);

    #endregion

    #region FallbackBackground

    public static readonly AttachedProperty<IBrush?> FallbackBackgroundProperty =
        AvaloniaProperty.RegisterAttached<Control, IBrush?>("FallbackBackground", typeof(AvatarHelper));

    public static IBrush? GetFallbackBackground(Control element) => element.GetValue(FallbackBackgroundProperty);
    public static void SetFallbackBackground(Control element, IBrush? value) => element.SetValue(FallbackBackgroundProperty, value);

    #endregion

    #region FallbackForeground

    public static readonly AttachedProperty<IBrush?> FallbackForegroundProperty =
        AvaloniaProperty.RegisterAttached<Control, IBrush?>("FallbackForeground", typeof(AvatarHelper));

    public static IBrush? GetFallbackForeground(Control element) => element.GetValue(FallbackForegroundProperty);
    public static void SetFallbackForeground(Control element, IBrush? value) => element.SetValue(FallbackForegroundProperty, value);

    #endregion

    #region BorderBrush

    public static readonly AttachedProperty<IBrush?> BorderBrushProperty =
        AvaloniaProperty.RegisterAttached<Control, IBrush?>("BorderBrush", typeof(AvatarHelper));

    public static IBrush? GetBorderBrush(Control element) => element.GetValue(BorderBrushProperty);
    public static void SetBorderBrush(Control element, IBrush? value) => element.SetValue(BorderBrushProperty, value);

    #endregion

    #region BorderThickness

    public static readonly AttachedProperty<Thickness> BorderThicknessProperty =
        AvaloniaProperty.RegisterAttached<Control, Thickness>("BorderThickness", typeof(AvatarHelper));

    public static Thickness GetBorderThickness(Control element) => element.GetValue(BorderThicknessProperty);
    public static void SetBorderThickness(Control element, Thickness value) => element.SetValue(BorderThicknessProperty, value);

    #endregion

    #region CornerRadius

    public static readonly AttachedProperty<CornerRadius> CornerRadiusProperty =
        AvaloniaProperty.RegisterAttached<Control, CornerRadius>("CornerRadius", typeof(AvatarHelper), new CornerRadius(999));

    public static CornerRadius GetCornerRadius(Control element) => element.GetValue(CornerRadiusProperty);
    public static void SetCornerRadius(Control element, CornerRadius value) => element.SetValue(CornerRadiusProperty, value);

    #endregion
}

/// <summary>
/// Shape options for the AvatarHelper attached property.
/// </summary>
public enum AvatarShapeAttached
{
    Circle,
    Square,
    Rounded
}
