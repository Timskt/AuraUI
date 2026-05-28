using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace AuraUI.Core.Helpers;

/// <summary>
/// Attached properties for styling Slider controls.
/// Usage: aura:SliderHelper.TrackHeight="6"
/// </summary>
public static class SliderHelper
{
    #region TrackHeight

    public static readonly AttachedProperty<double> TrackHeightProperty =
        AvaloniaProperty.RegisterAttached<Slider, double>("TrackHeight", typeof(SliderHelper), 4);

    public static double GetTrackHeight(Slider element) => element.GetValue(TrackHeightProperty);
    public static void SetTrackHeight(Slider element, double value) => element.SetValue(TrackHeightProperty, value);

    #endregion

    #region TrackCornerRadius

    public static readonly AttachedProperty<CornerRadius> TrackCornerRadiusProperty =
        AvaloniaProperty.RegisterAttached<Slider, CornerRadius>("TrackCornerRadius", typeof(SliderHelper), new CornerRadius(2));

    public static CornerRadius GetTrackCornerRadius(Slider element) => element.GetValue(TrackCornerRadiusProperty);
    public static void SetTrackCornerRadius(Slider element, CornerRadius value) => element.SetValue(TrackCornerRadiusProperty, value);

    #endregion

    #region TrackBackground

    public static readonly AttachedProperty<IBrush?> TrackBackgroundProperty =
        AvaloniaProperty.RegisterAttached<Slider, IBrush?>("TrackBackground", typeof(SliderHelper));

    public static IBrush? GetTrackBackground(Slider element) => element.GetValue(TrackBackgroundProperty);
    public static void SetTrackBackground(Slider element, IBrush? value) => element.SetValue(TrackBackgroundProperty, value);

    #endregion

    #region ProgressBrush

    public static readonly AttachedProperty<IBrush?> ProgressBrushProperty =
        AvaloniaProperty.RegisterAttached<Slider, IBrush?>("ProgressBrush", typeof(SliderHelper));

    public static IBrush? GetProgressBrush(Slider element) => element.GetValue(ProgressBrushProperty);
    public static void SetProgressBrush(Slider element, IBrush? value) => element.SetValue(ProgressBrushProperty, value);

    #endregion

    #region ThumbSize

    public static readonly AttachedProperty<double> ThumbSizeProperty =
        AvaloniaProperty.RegisterAttached<Slider, double>("ThumbSize", typeof(SliderHelper), 16);

    public static double GetThumbSize(Slider element) => element.GetValue(ThumbSizeProperty);
    public static void SetThumbSize(Slider element, double value) => element.SetValue(ThumbSizeProperty, value);

    #endregion

    #region ThumbBackground

    public static readonly AttachedProperty<IBrush?> ThumbBackgroundProperty =
        AvaloniaProperty.RegisterAttached<Slider, IBrush?>("ThumbBackground", typeof(SliderHelper));

    public static IBrush? GetThumbBackground(Slider element) => element.GetValue(ThumbBackgroundProperty);
    public static void SetThumbBackground(Slider element, IBrush? value) => element.SetValue(ThumbBackgroundProperty, value);

    #endregion

    #region ThumbShadow

    public static readonly AttachedProperty<BoxShadow> ThumbShadowProperty =
        AvaloniaProperty.RegisterAttached<Slider, BoxShadow>("ThumbShadow", typeof(SliderHelper));

    public static BoxShadow GetThumbShadow(Slider element) => element.GetValue(ThumbShadowProperty);
    public static void SetThumbShadow(Slider element, BoxShadow value) => element.SetValue(ThumbShadowProperty, value);

    #endregion

    #region ShowTooltip

    public static readonly AttachedProperty<bool> ShowTooltipProperty =
        AvaloniaProperty.RegisterAttached<Slider, bool>("ShowTooltip", typeof(SliderHelper));

    public static bool GetShowTooltip(Slider element) => element.GetValue(ShowTooltipProperty);
    public static void SetShowTooltip(Slider element, bool value) => element.SetValue(ShowTooltipProperty, value);

    #endregion

    #region TooltipFormat

    public static readonly AttachedProperty<string?> TooltipFormatProperty =
        AvaloniaProperty.RegisterAttached<Slider, string?>("TooltipFormat", typeof(SliderHelper), "{0:F0}");

    public static string? GetTooltipFormat(Slider element) => element.GetValue(TooltipFormatProperty);
    public static void SetTooltipFormat(Slider element, string? value) => element.SetValue(TooltipFormatProperty, value);

    #endregion

    #region IsSnapToTick

    public static readonly AttachedProperty<bool> IsSnapToTickProperty =
        AvaloniaProperty.RegisterAttached<Slider, bool>("IsSnapToTick", typeof(SliderHelper));

    public static bool GetIsSnapToTick(Slider element) => element.GetValue(IsSnapToTickProperty);
    public static void SetIsSnapToTick(Slider element, bool value) => element.SetValue(IsSnapToTickProperty, value);

    #endregion

    #region TickFrequency

    public static readonly AttachedProperty<double> TickFrequencyProperty =
        AvaloniaProperty.RegisterAttached<Slider, double>("TickFrequency", typeof(SliderHelper));

    public static double GetTickFrequency(Slider element) => element.GetValue(TickFrequencyProperty);
    public static void SetTickFrequency(Slider element, double value) => element.SetValue(TickFrequencyProperty, value);

    #endregion
}
