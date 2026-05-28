using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace AuraUI.Core.Helpers;

/// <summary>
/// Attached properties for styling Image controls.
/// Usage: aura:ImageHelper.CornerRadius="20"
/// </summary>
public static class ImageHelper
{
    #region CornerRadius

    public static readonly AttachedProperty<CornerRadius> CornerRadiusProperty =
        AvaloniaProperty.RegisterAttached<Image, CornerRadius>("CornerRadius", typeof(ImageHelper));

    public static CornerRadius GetCornerRadius(Image element) => element.GetValue(CornerRadiusProperty);
    public static void SetCornerRadius(Image element, CornerRadius value) => element.SetValue(CornerRadiusProperty, value);

    #endregion

    #region Stretch

    public static readonly AttachedProperty<Stretch> StretchProperty =
        AvaloniaProperty.RegisterAttached<Image, Stretch>("Stretch", typeof(ImageHelper), Stretch.UniformToFill);

    public static Stretch GetStretch(Image element) => element.GetValue(StretchProperty);
    public static void SetStretch(Image element, Stretch value) => element.SetValue(StretchProperty, value);

    #endregion

    #region Shadow

    public static readonly AttachedProperty<BoxShadow> ShadowProperty =
        AvaloniaProperty.RegisterAttached<Image, BoxShadow>("Shadow", typeof(ImageHelper));

    public static BoxShadow GetShadow(Image element) => element.GetValue(ShadowProperty);
    public static void SetShadow(Image element, BoxShadow value) => element.SetValue(ShadowProperty, value);

    #endregion

    #region Placeholder

    public static readonly AttachedProperty<IImage?> PlaceholderProperty =
        AvaloniaProperty.RegisterAttached<Image, IImage?>("Placeholder", typeof(ImageHelper));

    public static IImage? GetPlaceholder(Image element) => element.GetValue(PlaceholderProperty);
    public static void SetPlaceholder(Image element, IImage? value) => element.SetValue(PlaceholderProperty, value);

    #endregion

    #region IsLazyLoading

    public static readonly AttachedProperty<bool> IsLazyLoadingProperty =
        AvaloniaProperty.RegisterAttached<Image, bool>("IsLazyLoading", typeof(ImageHelper));

    public static bool GetIsLazyLoading(Image element) => element.GetValue(IsLazyLoadingProperty);
    public static void SetIsLazyLoading(Image element, bool value) => element.SetValue(IsLazyLoadingProperty, value);

    #endregion

    #region OverlayBrush

    public static readonly AttachedProperty<IBrush?> OverlayBrushProperty =
        AvaloniaProperty.RegisterAttached<Image, IBrush?>("OverlayBrush", typeof(ImageHelper));

    public static IBrush? GetOverlayBrush(Image element) => element.GetValue(OverlayBrushProperty);
    public static void SetOverlayBrush(Image element, IBrush? value) => element.SetValue(OverlayBrushProperty, value);

    #endregion

    #region OverlayOpacity

    public static readonly AttachedProperty<double> OverlayOpacityProperty =
        AvaloniaProperty.RegisterAttached<Image, double>("OverlayOpacity", typeof(ImageHelper), 0);

    public static double GetOverlayOpacity(Image element) => element.GetValue(OverlayOpacityProperty);
    public static void SetOverlayOpacity(Image element, double value) => element.SetValue(OverlayOpacityProperty, value);

    #endregion

    #region BlurRadius

    public static readonly AttachedProperty<double> BlurRadiusProperty =
        AvaloniaProperty.RegisterAttached<Image, double>("BlurRadius", typeof(ImageHelper));

    public static double GetBlurRadius(Image element) => element.GetValue(BlurRadiusProperty);
    public static void SetBlurRadius(Image element, double value) => element.SetValue(BlurRadiusProperty, value);

    #endregion

    #region Grayscale

    public static readonly AttachedProperty<bool> GrayscaleProperty =
        AvaloniaProperty.RegisterAttached<Image, bool>("Grayscale", typeof(ImageHelper));

    public static bool GetGrayscale(Image element) => element.GetValue(GrayscaleProperty);
    public static void SetGrayscale(Image element, bool value) => element.SetValue(GrayscaleProperty, value);

    #endregion
}
