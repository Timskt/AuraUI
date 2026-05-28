using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Media;

namespace AuraUI.Core.Helpers;

/// <summary>
/// Attached properties for styling ScrollBar / ScrollViewer.
/// Usage: aura:ScrollBarHelper.Width="6"
/// </summary>
public static class ScrollBarHelper
{
    #region Width (scrollbar thickness)

    public static readonly AttachedProperty<double> WidthProperty =
        AvaloniaProperty.RegisterAttached<ScrollBar, double>("Width", typeof(ScrollBarHelper), 8);

    public static double GetWidth(ScrollBar element) => element.GetValue(WidthProperty);
    public static void SetWidth(ScrollBar element, double value) => element.SetValue(WidthProperty, value);

    #endregion

    #region TrackBackground

    public static readonly AttachedProperty<IBrush?> TrackBackgroundProperty =
        AvaloniaProperty.RegisterAttached<ScrollBar, IBrush?>("TrackBackground", typeof(ScrollBarHelper));

    public static IBrush? GetTrackBackground(ScrollBar element) => element.GetValue(TrackBackgroundProperty);
    public static void SetTrackBackground(ScrollBar element, IBrush? value) => element.SetValue(TrackBackgroundProperty, value);

    #endregion

    #region ThumbBackground

    public static readonly AttachedProperty<IBrush?> ThumbBackgroundProperty =
        AvaloniaProperty.RegisterAttached<ScrollBar, IBrush?>("ThumbBackground", typeof(ScrollBarHelper));

    public static IBrush? GetThumbBackground(ScrollBar element) => element.GetValue(ThumbBackgroundProperty);
    public static void SetThumbBackground(ScrollBar element, IBrush? value) => element.SetValue(ThumbBackgroundProperty, value);

    #endregion

    #region ThumbHoverBackground

    public static readonly AttachedProperty<IBrush?> ThumbHoverBackgroundProperty =
        AvaloniaProperty.RegisterAttached<ScrollBar, IBrush?>("ThumbHoverBackground", typeof(ScrollBarHelper));

    public static IBrush? GetThumbHoverBackground(ScrollBar element) => element.GetValue(ThumbHoverBackgroundProperty);
    public static void SetThumbHoverBackground(ScrollBar element, IBrush? value) => element.SetValue(ThumbHoverBackgroundProperty, value);

    #endregion

    #region ThumbCornerRadius

    public static readonly AttachedProperty<CornerRadius> ThumbCornerRadiusProperty =
        AvaloniaProperty.RegisterAttached<ScrollBar, CornerRadius>("ThumbCornerRadius", typeof(ScrollBarHelper), new CornerRadius(4));

    public static CornerRadius GetThumbCornerRadius(ScrollBar element) => element.GetValue(ThumbCornerRadiusProperty);
    public static void SetThumbCornerRadius(ScrollBar element, CornerRadius value) => element.SetValue(ThumbCornerRadiusProperty, value);

    #endregion

    #region ThumbMinHeight

    public static readonly AttachedProperty<double> ThumbMinHeightProperty =
        AvaloniaProperty.RegisterAttached<ScrollBar, double>("ThumbMinHeight", typeof(ScrollBarHelper), 20);

    public static double GetThumbMinHeight(ScrollBar element) => element.GetValue(ThumbMinHeightProperty);
    public static void SetThumbMinHeight(ScrollBar element, double value) => element.SetValue(ThumbMinHeightProperty, value);

    #endregion

    #region IsAutoHide

    public static readonly AttachedProperty<bool> IsAutoHideProperty =
        AvaloniaProperty.RegisterAttached<ScrollBar, bool>("IsAutoHide", typeof(ScrollBarHelper));

    public static bool GetIsAutoHide(ScrollBar element) => element.GetValue(IsAutoHideProperty);
    public static void SetIsAutoHide(ScrollBar element, bool value) => element.SetValue(IsAutoHideProperty, value);

    #endregion

    #region ShowButtons

    public static readonly AttachedProperty<bool> ShowButtonsProperty =
        AvaloniaProperty.RegisterAttached<ScrollBar, bool>("ShowButtons", typeof(ScrollBarHelper));

    public static bool GetShowButtons(ScrollBar element) => element.GetValue(ShowButtonsProperty);
    public static void SetShowButtons(ScrollBar element, bool value) => element.SetValue(ShowButtonsProperty, value);

    #endregion
}
