using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace AuraUI.Core.Helpers;

/// <summary>
/// Attached properties for styling ProgressBar controls.
/// Usage: aura:ProgressBarHelper.CornerRadius="10"
/// </summary>
public static class ProgressBarHelper
{
    #region CornerRadius

    public static readonly AttachedProperty<CornerRadius> CornerRadiusProperty =
        AvaloniaProperty.RegisterAttached<ProgressBar, CornerRadius>("CornerRadius", typeof(ProgressBarHelper), new CornerRadius(4));

    public static CornerRadius GetCornerRadius(ProgressBar element) => element.GetValue(CornerRadiusProperty);
    public static void SetCornerRadius(ProgressBar element, CornerRadius value) => element.SetValue(CornerRadiusProperty, value);

    #endregion

    #region TrackBackground

    public static readonly AttachedProperty<IBrush?> TrackBackgroundProperty =
        AvaloniaProperty.RegisterAttached<ProgressBar, IBrush?>("TrackBackground", typeof(ProgressBarHelper));

    public static IBrush? GetTrackBackground(ProgressBar element) => element.GetValue(TrackBackgroundProperty);
    public static void SetTrackBackground(ProgressBar element, IBrush? value) => element.SetValue(TrackBackgroundProperty, value);

    #endregion

    #region ProgressBrush

    public static readonly AttachedProperty<IBrush?> ProgressBrushProperty =
        AvaloniaProperty.RegisterAttached<ProgressBar, IBrush?>("ProgressBrush", typeof(ProgressBarHelper));

    public static IBrush? GetProgressBrush(ProgressBar element) => element.GetValue(ProgressBrushProperty);
    public static void SetProgressBrush(ProgressBar element, IBrush? value) => element.SetValue(ProgressBrushProperty, value);

    #endregion

    #region IndeterminateBrush

    public static readonly AttachedProperty<IBrush?> IndeterminateBrushProperty =
        AvaloniaProperty.RegisterAttached<ProgressBar, IBrush?>("IndeterminateBrush", typeof(ProgressBarHelper));

    public static IBrush? GetIndeterminateBrush(ProgressBar element) => element.GetValue(IndeterminateBrushProperty);
    public static void SetIndeterminateBrush(ProgressBar element, IBrush? value) => element.SetValue(IndeterminateBrushProperty, value);

    #endregion

    #region ShowLabel

    public static readonly AttachedProperty<bool> ShowLabelProperty =
        AvaloniaProperty.RegisterAttached<ProgressBar, bool>("ShowLabel", typeof(ProgressBarHelper));

    public static bool GetShowLabel(ProgressBar element) => element.GetValue(ShowLabelProperty);
    public static void SetShowLabel(ProgressBar element, bool value) => element.SetValue(ShowLabelProperty, value);

    #endregion

    #region LabelFormat

    public static readonly AttachedProperty<string?> LabelFormatProperty =
        AvaloniaProperty.RegisterAttached<ProgressBar, string?>("LabelFormat", typeof(ProgressBarHelper), "{0:F0}%");

    public static string? GetLabelFormat(ProgressBar element) => element.GetValue(LabelFormatProperty);
    public static void SetLabelFormat(ProgressBar element, string? value) => element.SetValue(LabelFormatProperty, value);

    #endregion

    #region Height

    public static readonly AttachedProperty<double> HeightProperty =
        AvaloniaProperty.RegisterAttached<ProgressBar, double>("Height", typeof(ProgressBarHelper), 6);

    public static double GetHeight(ProgressBar element) => element.GetValue(HeightProperty);
    public static void SetHeight(ProgressBar element, double value) => element.SetValue(HeightProperty, value);

    #endregion

    #region IsStriped

    public static readonly AttachedProperty<bool> IsStripedProperty =
        AvaloniaProperty.RegisterAttached<ProgressBar, bool>("IsStriped", typeof(ProgressBarHelper));

    public static bool GetIsStriped(ProgressBar element) => element.GetValue(IsStripedProperty);
    public static void SetIsStriped(ProgressBar element, bool value) => element.SetValue(IsStripedProperty, value);

    #endregion

    #region IsAnimated

    public static readonly AttachedProperty<bool> IsAnimatedProperty =
        AvaloniaProperty.RegisterAttached<ProgressBar, bool>("IsAnimated", typeof(ProgressBarHelper), true);

    public static bool GetIsAnimated(ProgressBar element) => element.GetValue(IsAnimatedProperty);
    public static void SetIsAnimated(ProgressBar element, bool value) => element.SetValue(IsAnimatedProperty, value);

    #endregion
}
