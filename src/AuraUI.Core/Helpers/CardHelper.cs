using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace AuraUI.Core.Helpers;

/// <summary>
/// Attached properties for styling Card controls.
/// Usage: aura:CardHelper.Elevation="2"
/// </summary>
public static class CardHelper
{
    #region Elevation

    public static readonly AttachedProperty<int> ElevationProperty =
        AvaloniaProperty.RegisterAttached<Control, int>("Elevation", typeof(CardHelper), 1);

    public static int GetElevation(Control element) => element.GetValue(ElevationProperty);
    public static void SetElevation(Control element, int value) => element.SetValue(ElevationProperty, value);

    #endregion

    #region HeaderPadding

    public static readonly AttachedProperty<Thickness> HeaderPaddingProperty =
        AvaloniaProperty.RegisterAttached<Control, Thickness>("HeaderPadding", typeof(CardHelper), new Thickness(0, 0, 0, 12));

    public static Thickness GetHeaderPadding(Control element) => element.GetValue(HeaderPaddingProperty);
    public static void SetHeaderPadding(Control element, Thickness value) => element.SetValue(HeaderPaddingProperty, value);

    #endregion

    #region FooterPadding

    public static readonly AttachedProperty<Thickness> FooterPaddingProperty =
        AvaloniaProperty.RegisterAttached<Control, Thickness>("FooterPadding", typeof(CardHelper), new Thickness(0, 12, 0, 0));

    public static Thickness GetFooterPadding(Control element) => element.GetValue(FooterPaddingProperty);
    public static void SetFooterPadding(Control element, Thickness value) => element.SetValue(FooterPaddingProperty, value);

    #endregion

    #region HeaderBorderBrush

    public static readonly AttachedProperty<IBrush?> HeaderBorderBrushProperty =
        AvaloniaProperty.RegisterAttached<Control, IBrush?>("HeaderBorderBrush", typeof(CardHelper));

    public static IBrush? GetHeaderBorderBrush(Control element) => element.GetValue(HeaderBorderBrushProperty);
    public static void SetHeaderBorderBrush(Control element, IBrush? value) => element.SetValue(HeaderBorderBrushProperty, value);

    #endregion

    #region Shadow

    public static readonly AttachedProperty<BoxShadow> ShadowProperty =
        AvaloniaProperty.RegisterAttached<Control, BoxShadow>("Shadow", typeof(CardHelper));

    public static BoxShadow GetShadow(Control element) => element.GetValue(ShadowProperty);
    public static void SetShadow(Control element, BoxShadow value) => element.SetValue(ShadowProperty, value);

    #endregion

    #region HeaderBackground

    public static readonly AttachedProperty<IBrush?> HeaderBackgroundProperty =
        AvaloniaProperty.RegisterAttached<Control, IBrush?>("HeaderBackground", typeof(CardHelper));

    public static IBrush? GetHeaderBackground(Control element) => element.GetValue(HeaderBackgroundProperty);
    public static void SetHeaderBackground(Control element, IBrush? value) => element.SetValue(HeaderBackgroundProperty, value);

    #endregion

    #region FooterBackground

    public static readonly AttachedProperty<IBrush?> FooterBackgroundProperty =
        AvaloniaProperty.RegisterAttached<Control, IBrush?>("FooterBackground", typeof(CardHelper));

    public static IBrush? GetFooterBackground(Control element) => element.GetValue(FooterBackgroundProperty);
    public static void SetFooterBackground(Control element, IBrush? value) => element.SetValue(FooterBackgroundProperty, value);

    #endregion

    #region IsHoverable

    public static readonly AttachedProperty<bool> IsHoverableProperty =
        AvaloniaProperty.RegisterAttached<Control, bool>("IsHoverable", typeof(CardHelper));

    public static bool GetIsHoverable(Control element) => element.GetValue(IsHoverableProperty);
    public static void SetIsHoverable(Control element, bool value) => element.SetValue(IsHoverableProperty, value);

    #endregion

    #region HoverShadow

    public static readonly AttachedProperty<BoxShadow> HoverShadowProperty =
        AvaloniaProperty.RegisterAttached<Control, BoxShadow>("HoverShadow", typeof(CardHelper));

    public static BoxShadow GetHoverShadow(Control element) => element.GetValue(HoverShadowProperty);
    public static void SetHoverShadow(Control element, BoxShadow value) => element.SetValue(HoverShadowProperty, value);

    #endregion
}
