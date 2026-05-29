using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace AuraUI.Core.Helpers;

/// <summary>
/// Attached properties for styling Carousel controls.
/// Usage: aura:CarouselHelper.IndicatorSize="8"
/// </summary>
public static class CarouselHelper
{
    #region IndicatorSize

    public static readonly AttachedProperty<double> IndicatorSizeProperty =
        AvaloniaProperty.RegisterAttached<Control, double>("IndicatorSize", typeof(CarouselHelper), 8);

    public static double GetIndicatorSize(Control element) => element.GetValue(IndicatorSizeProperty);
    public static void SetIndicatorSize(Control element, double value) => element.SetValue(IndicatorSizeProperty, value);

    #endregion

    #region IndicatorColor

    public static readonly AttachedProperty<IBrush?> IndicatorColorProperty =
        AvaloniaProperty.RegisterAttached<Control, IBrush?>("IndicatorColor", typeof(CarouselHelper));

    public static IBrush? GetIndicatorColor(Control element) => element.GetValue(IndicatorColorProperty);
    public static void SetIndicatorColor(Control element, IBrush? value) => element.SetValue(IndicatorColorProperty, value);

    #endregion

    #region IndicatorActiveColor

    public static readonly AttachedProperty<IBrush?> IndicatorActiveColorProperty =
        AvaloniaProperty.RegisterAttached<Control, IBrush?>("IndicatorActiveColor", typeof(CarouselHelper));

    public static IBrush? GetIndicatorActiveColor(Control element) => element.GetValue(IndicatorActiveColorProperty);
    public static void SetIndicatorActiveColor(Control element, IBrush? value) => element.SetValue(IndicatorActiveColorProperty, value);

    #endregion

    #region NavigationButtonSize

    public static readonly AttachedProperty<double> NavigationButtonSizeProperty =
        AvaloniaProperty.RegisterAttached<Control, double>("NavigationButtonSize", typeof(CarouselHelper), 32);

    public static double GetNavigationButtonSize(Control element) => element.GetValue(NavigationButtonSizeProperty);
    public static void SetNavigationButtonSize(Control element, double value) => element.SetValue(NavigationButtonSizeProperty, value);

    #endregion

    #region NavigationButtonBackground

    public static readonly AttachedProperty<IBrush?> NavigationButtonBackgroundProperty =
        AvaloniaProperty.RegisterAttached<Control, IBrush?>("NavigationButtonBackground", typeof(CarouselHelper));

    public static IBrush? GetNavigationButtonBackground(Control element) => element.GetValue(NavigationButtonBackgroundProperty);
    public static void SetNavigationButtonBackground(Control element, IBrush? value) => element.SetValue(NavigationButtonBackgroundProperty, value);

    #endregion

    #region IndicatorCornerRadius

    public static readonly AttachedProperty<CornerRadius> IndicatorCornerRadiusProperty =
        AvaloniaProperty.RegisterAttached<Control, CornerRadius>("IndicatorCornerRadius", typeof(CarouselHelper), new CornerRadius(999));

    public static CornerRadius GetIndicatorCornerRadius(Control element) => element.GetValue(IndicatorCornerRadiusProperty);
    public static void SetIndicatorCornerRadius(Control element, CornerRadius value) => element.SetValue(IndicatorCornerRadiusProperty, value);

    #endregion

    #region IndicatorSpacing

    public static readonly AttachedProperty<double> IndicatorSpacingProperty =
        AvaloniaProperty.RegisterAttached<Control, double>("IndicatorSpacing", typeof(CarouselHelper), 6);

    public static double GetIndicatorSpacing(Control element) => element.GetValue(IndicatorSpacingProperty);
    public static void SetIndicatorSpacing(Control element, double value) => element.SetValue(IndicatorSpacingProperty, value);

    #endregion
}
