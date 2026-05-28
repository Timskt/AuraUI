using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace AuraUI.Core.Helpers;

/// <summary>
/// Attached properties for styling Dialog / Modal controls.
/// Usage: aura:DialogHelper.OverlayBrush="#80000000"
/// </summary>
public static class DialogHelper
{
    #region OverlayBrush

    public static readonly AttachedProperty<IBrush?> OverlayBrushProperty =
        AvaloniaProperty.RegisterAttached<Control, IBrush?>("OverlayBrush", typeof(DialogHelper));

    public static IBrush? GetOverlayBrush(Control element) => element.GetValue(OverlayBrushProperty);
    public static void SetOverlayBrush(Control element, IBrush? value) => element.SetValue(OverlayBrushProperty, value);

    #endregion

    #region OverlayOpacity

    public static readonly AttachedProperty<double> OverlayOpacityProperty =
        AvaloniaProperty.RegisterAttached<Control, double>("OverlayOpacity", typeof(DialogHelper), 0.5);

    public static double GetOverlayOpacity(Control element) => element.GetValue(OverlayOpacityProperty);
    public static void SetOverlayOpacity(Control element, double value) => element.SetValue(OverlayOpacityProperty, value);

    #endregion

    #region DialogCornerRadius

    public static readonly AttachedProperty<CornerRadius> DialogCornerRadiusProperty =
        AvaloniaProperty.RegisterAttached<Control, CornerRadius>("DialogCornerRadius", typeof(DialogHelper), new CornerRadius(8));

    public static CornerRadius GetDialogCornerRadius(Control element) => element.GetValue(DialogCornerRadiusProperty);
    public static void SetDialogCornerRadius(Control element, CornerRadius value) => element.SetValue(DialogCornerRadiusProperty, value);

    #endregion

    #region DialogShadow

    public static readonly AttachedProperty<BoxShadow> DialogShadowProperty =
        AvaloniaProperty.RegisterAttached<Control, BoxShadow>("DialogShadow", typeof(DialogHelper));

    public static BoxShadow GetDialogShadow(Control element) => element.GetValue(DialogShadowProperty);
    public static void SetDialogShadow(Control element, BoxShadow value) => element.SetValue(DialogShadowProperty, value);

    #endregion

    #region DialogBackground

    public static readonly AttachedProperty<IBrush?> DialogBackgroundProperty =
        AvaloniaProperty.RegisterAttached<Control, IBrush?>("DialogBackground", typeof(DialogHelper));

    public static IBrush? GetDialogBackground(Control element) => element.GetValue(DialogBackgroundProperty);
    public static void SetDialogBackground(Control element, IBrush? value) => element.SetValue(DialogBackgroundProperty, value);

    #endregion

    #region DialogPadding

    public static readonly AttachedProperty<Thickness> DialogPaddingProperty =
        AvaloniaProperty.RegisterAttached<Control, Thickness>("DialogPadding", typeof(DialogHelper), new Thickness(24));

    public static Thickness GetDialogPadding(Control element) => element.GetValue(DialogPaddingProperty);
    public static void SetDialogPadding(Control element, Thickness value) => element.SetValue(DialogPaddingProperty, value);

    #endregion

    #region CloseOnOverlayClick

    public static readonly AttachedProperty<bool> CloseOnOverlayClickProperty =
        AvaloniaProperty.RegisterAttached<Control, bool>("CloseOnOverlayClick", typeof(DialogHelper), true);

    public static bool GetCloseOnOverlayClick(Control element) => element.GetValue(CloseOnOverlayClickProperty);
    public static void SetCloseOnOverlayClick(Control element, bool value) => element.SetValue(CloseOnOverlayClickProperty, value);

    #endregion

    #region IsModal

    public static readonly AttachedProperty<bool> IsModalProperty =
        AvaloniaProperty.RegisterAttached<Control, bool>("IsModal", typeof(DialogHelper), true);

    public static bool GetIsModal(Control element) => element.GetValue(IsModalProperty);
    public static void SetIsModal(Control element, bool value) => element.SetValue(IsModalProperty, value);

    #endregion

    #region AnimationDuration

    public static readonly AttachedProperty<TimeSpan> AnimationDurationProperty =
        AvaloniaProperty.RegisterAttached<Control, TimeSpan>("AnimationDuration", typeof(DialogHelper), TimeSpan.FromMilliseconds(250));

    public static TimeSpan GetAnimationDuration(Control element) => element.GetValue(AnimationDurationProperty);
    public static void SetAnimationDuration(Control element, TimeSpan value) => element.SetValue(AnimationDurationProperty, value);

    #endregion

    #region MaxWidth

    public static readonly AttachedProperty<double> MaxWidthProperty =
        AvaloniaProperty.RegisterAttached<Control, double>("MaxWidth", typeof(DialogHelper), 600);

    public static double GetMaxWidth(Control element) => element.GetValue(MaxWidthProperty);
    public static void SetMaxWidth(Control element, double value) => element.SetValue(MaxWidthProperty, value);

    #endregion

    #region MaxHeight

    public static readonly AttachedProperty<double> MaxHeightProperty =
        AvaloniaProperty.RegisterAttached<Control, double>("MaxHeight", typeof(DialogHelper), 800);

    public static double GetMaxHeight(Control element) => element.GetValue(MaxHeightProperty);
    public static void SetMaxHeight(Control element, double value) => element.SetValue(MaxHeightProperty, value);

    #endregion
}
