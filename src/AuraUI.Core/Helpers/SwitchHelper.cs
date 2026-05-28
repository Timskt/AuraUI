using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Media;

namespace AuraUI.Core.Helpers;

/// <summary>
/// Attached properties for styling Switch (ToggleButton) controls.
/// Usage: aura:SwitchHelper.ThumbSize="20"
/// </summary>
public static class SwitchHelper
{
    #region TrackWidth

    public static readonly AttachedProperty<double> TrackWidthProperty =
        AvaloniaProperty.RegisterAttached<ToggleButton, double>("TrackWidth", typeof(SwitchHelper), 44);

    public static double GetTrackWidth(ToggleButton element) => element.GetValue(TrackWidthProperty);
    public static void SetTrackWidth(ToggleButton element, double value) => element.SetValue(TrackWidthProperty, value);

    #endregion

    #region TrackHeight

    public static readonly AttachedProperty<double> TrackHeightProperty =
        AvaloniaProperty.RegisterAttached<ToggleButton, double>("TrackHeight", typeof(SwitchHelper), 24);

    public static double GetTrackHeight(ToggleButton element) => element.GetValue(TrackHeightProperty);
    public static void SetTrackHeight(ToggleButton element, double value) => element.SetValue(TrackHeightProperty, value);

    #endregion

    #region ThumbSize

    public static readonly AttachedProperty<double> ThumbSizeProperty =
        AvaloniaProperty.RegisterAttached<ToggleButton, double>("ThumbSize", typeof(SwitchHelper), 18);

    public static double GetThumbSize(ToggleButton element) => element.GetValue(ThumbSizeProperty);
    public static void SetThumbSize(ToggleButton element, double value) => element.SetValue(ThumbSizeProperty, value);

    #endregion

    #region TrackCornerRadius

    public static readonly AttachedProperty<CornerRadius> TrackCornerRadiusProperty =
        AvaloniaProperty.RegisterAttached<ToggleButton, CornerRadius>("TrackCornerRadius", typeof(SwitchHelper), new CornerRadius(12));

    public static CornerRadius GetTrackCornerRadius(ToggleButton element) => element.GetValue(TrackCornerRadiusProperty);
    public static void SetTrackCornerRadius(ToggleButton element, CornerRadius value) => element.SetValue(TrackCornerRadiusProperty, value);

    #endregion

    #region ThumbCornerRadius

    public static readonly AttachedProperty<CornerRadius> ThumbCornerRadiusProperty =
        AvaloniaProperty.RegisterAttached<ToggleButton, CornerRadius>("ThumbCornerRadius", typeof(SwitchHelper), new CornerRadius(9));

    public static CornerRadius GetThumbCornerRadius(ToggleButton element) => element.GetValue(ThumbCornerRadiusProperty);
    public static void SetThumbCornerRadius(ToggleButton element, CornerRadius value) => element.SetValue(ThumbCornerRadiusProperty, value);

    #endregion

    #region CheckedTrackBackground

    public static readonly AttachedProperty<IBrush?> CheckedTrackBackgroundProperty =
        AvaloniaProperty.RegisterAttached<ToggleButton, IBrush?>("CheckedTrackBackground", typeof(SwitchHelper));

    public static IBrush? GetCheckedTrackBackground(ToggleButton element) => element.GetValue(CheckedTrackBackgroundProperty);
    public static void SetCheckedTrackBackground(ToggleButton element, IBrush? value) => element.SetValue(CheckedTrackBackgroundProperty, value);

    #endregion

    #region UncheckedTrackBackground

    public static readonly AttachedProperty<IBrush?> UncheckedTrackBackgroundProperty =
        AvaloniaProperty.RegisterAttached<ToggleButton, IBrush?>("UncheckedTrackBackground", typeof(SwitchHelper));

    public static IBrush? GetUncheckedTrackBackground(ToggleButton element) => element.GetValue(UncheckedTrackBackgroundProperty);
    public static void SetUncheckedTrackBackground(ToggleButton element, IBrush? value) => element.SetValue(UncheckedTrackBackgroundProperty, value);

    #endregion

    #region CheckedThumbBackground

    public static readonly AttachedProperty<IBrush?> CheckedThumbBackgroundProperty =
        AvaloniaProperty.RegisterAttached<ToggleButton, IBrush?>("CheckedThumbBackground", typeof(SwitchHelper));

    public static IBrush? GetCheckedThumbBackground(ToggleButton element) => element.GetValue(CheckedThumbBackgroundProperty);
    public static void SetCheckedThumbBackground(ToggleButton element, IBrush? value) => element.SetValue(CheckedThumbBackgroundProperty, value);

    #endregion

    #region UncheckedThumbBackground

    public static readonly AttachedProperty<IBrush?> UncheckedThumbBackgroundProperty =
        AvaloniaProperty.RegisterAttached<ToggleButton, IBrush?>("UncheckedThumbBackground", typeof(SwitchHelper));

    public static IBrush? GetUncheckedThumbBackground(ToggleButton element) => element.GetValue(UncheckedThumbBackgroundProperty);
    public static void SetUncheckedThumbBackground(ToggleButton element, IBrush? value) => element.SetValue(UncheckedThumbBackgroundProperty, value);

    #endregion

    #region ThumbShadow

    public static readonly AttachedProperty<BoxShadow> ThumbShadowProperty =
        AvaloniaProperty.RegisterAttached<ToggleButton, BoxShadow>("ThumbShadow", typeof(SwitchHelper));

    public static BoxShadow GetThumbShadow(ToggleButton element) => element.GetValue(ThumbShadowProperty);
    public static void SetThumbShadow(ToggleButton element, BoxShadow value) => element.SetValue(ThumbShadowProperty, value);

    #endregion

    #region OnContent

    public static readonly AttachedProperty<string?> OnContentProperty =
        AvaloniaProperty.RegisterAttached<ToggleButton, string?>("OnContent", typeof(SwitchHelper));

    public static string? GetOnContent(ToggleButton element) => element.GetValue(OnContentProperty);
    public static void SetOnContent(ToggleButton element, string? value) => element.SetValue(OnContentProperty, value);

    #endregion

    #region OffContent

    public static readonly AttachedProperty<string?> OffContentProperty =
        AvaloniaProperty.RegisterAttached<ToggleButton, string?>("OffContent", typeof(SwitchHelper));

    public static string? GetOffContent(ToggleButton element) => element.GetValue(OffContentProperty);
    public static void SetOffContent(ToggleButton element, string? value) => element.SetValue(OffContentProperty, value);

    #endregion
}
