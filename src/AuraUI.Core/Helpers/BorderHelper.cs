using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace AuraUI.Core.Helpers;

/// <summary>
/// Attached properties for styling Border elements.
/// Usage: aura:BorderHelper.HoverBackground="#F5F5F5"
/// </summary>
public static class BorderHelper
{
    #region HoverBackground

    public static readonly AttachedProperty<IBrush?> HoverBackgroundProperty =
        AvaloniaProperty.RegisterAttached<Border, IBrush?>("HoverBackground", typeof(BorderHelper));

    public static IBrush? GetHoverBackground(Border element) => element.GetValue(HoverBackgroundProperty);
    public static void SetHoverBackground(Border element, IBrush? value) => element.SetValue(HoverBackgroundProperty, value);

    #endregion

    #region HoverBorderBrush

    public static readonly AttachedProperty<IBrush?> HoverBorderBrushProperty =
        AvaloniaProperty.RegisterAttached<Border, IBrush?>("HoverBorderBrush", typeof(BorderHelper));

    public static IBrush? GetHoverBorderBrush(Border element) => element.GetValue(HoverBorderBrushProperty);
    public static void SetHoverBorderBrush(Border element, IBrush? value) => element.SetValue(HoverBorderBrushProperty, value);

    #endregion

    #region ClickBackground

    public static readonly AttachedProperty<IBrush?> ClickBackgroundProperty =
        AvaloniaProperty.RegisterAttached<Border, IBrush?>("ClickBackground", typeof(BorderHelper));

    public static IBrush? GetClickBackground(Border element) => element.GetValue(ClickBackgroundProperty);
    public static void SetClickBackground(Border element, IBrush? value) => element.SetValue(ClickBackgroundProperty, value);

    #endregion

    #region Shadow

    public static readonly AttachedProperty<BoxShadow> ShadowProperty =
        AvaloniaProperty.RegisterAttached<Border, BoxShadow>("Shadow", typeof(BorderHelper));

    public static BoxShadow GetShadow(Border element) => element.GetValue(ShadowProperty);
    public static void SetShadow(Border element, BoxShadow value) => element.SetValue(ShadowProperty, value);

    #endregion

    #region HoverShadow

    public static readonly AttachedProperty<BoxShadow> HoverShadowProperty =
        AvaloniaProperty.RegisterAttached<Border, BoxShadow>("HoverShadow", typeof(BorderHelper));

    public static BoxShadow GetHoverShadow(Border element) => element.GetValue(HoverShadowProperty);
    public static void SetHoverShadow(Border element, BoxShadow value) => element.SetValue(HoverShadowProperty, value);

    #endregion

    #region AnimationDuration

    public static readonly AttachedProperty<TimeSpan> AnimationDurationProperty =
        AvaloniaProperty.RegisterAttached<Border, TimeSpan>("AnimationDuration", typeof(BorderHelper), TimeSpan.FromMilliseconds(200));

    public static TimeSpan GetAnimationDuration(Border element) => element.GetValue(AnimationDurationProperty);
    public static void SetAnimationDuration(Border element, TimeSpan value) => element.SetValue(AnimationDurationProperty, value);

    #endregion

    #region IsInteractive

    public static readonly AttachedProperty<bool> IsInteractiveProperty =
        AvaloniaProperty.RegisterAttached<Border, bool>("IsInteractive", typeof(BorderHelper));

    public static bool GetIsInteractive(Border element) => element.GetValue(IsInteractiveProperty);
    public static void SetIsInteractive(Border element, bool value) => element.SetValue(IsInteractiveProperty, value);

    #endregion

    #region Cursor

    public static readonly AttachedProperty<Avalonia.Input.Cursor?> CursorProperty =
        AvaloniaProperty.RegisterAttached<Border, Avalonia.Input.Cursor?>("Cursor", typeof(BorderHelper));

    public static Avalonia.Input.Cursor? GetCursor(Border element) => element.GetValue(CursorProperty);
    public static void SetCursor(Border element, Avalonia.Input.Cursor? value) => element.SetValue(CursorProperty, value);

    #endregion

    #region FocusBorderBrush

    public static readonly AttachedProperty<IBrush?> FocusBorderBrushProperty =
        AvaloniaProperty.RegisterAttached<Border, IBrush?>("FocusBorderBrush", typeof(BorderHelper));

    public static IBrush? GetFocusBorderBrush(Border element) => element.GetValue(FocusBorderBrushProperty);
    public static void SetFocusBorderBrush(Border element, IBrush? value) => element.SetValue(FocusBorderBrushProperty, value);

    #endregion

    #region ClipToBounds

    public static readonly AttachedProperty<bool> ClipToBoundsProperty =
        AvaloniaProperty.RegisterAttached<Border, bool>("ClipToBounds", typeof(BorderHelper), true);

    public static bool GetClipToBounds(Border element) => element.GetValue(ClipToBoundsProperty);
    public static void SetClipToBounds(Border element, bool value) => element.SetValue(ClipToBoundsProperty, value);

    #endregion
}
