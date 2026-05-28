using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace AuraUI.Core.Helpers;

/// <summary>
/// Attached properties for styling Expander controls.
/// Usage: aura:ExpanderHelper.HeaderCornerRadius="8"
/// </summary>
public static class ExpanderHelper
{
    #region HeaderCornerRadius

    public static readonly AttachedProperty<CornerRadius> HeaderCornerRadiusProperty =
        AvaloniaProperty.RegisterAttached<Expander, CornerRadius>("HeaderCornerRadius", typeof(ExpanderHelper));

    public static CornerRadius GetHeaderCornerRadius(Expander element) => element.GetValue(HeaderCornerRadiusProperty);
    public static void SetHeaderCornerRadius(Expander element, CornerRadius value) => element.SetValue(HeaderCornerRadiusProperty, value);

    #endregion

    #region HeaderBackground

    public static readonly AttachedProperty<IBrush?> HeaderBackgroundProperty =
        AvaloniaProperty.RegisterAttached<Expander, IBrush?>("HeaderBackground", typeof(ExpanderHelper));

    public static IBrush? GetHeaderBackground(Expander element) => element.GetValue(HeaderBackgroundProperty);
    public static void SetHeaderBackground(Expander element, IBrush? value) => element.SetValue(HeaderBackgroundProperty, value);

    #endregion

    #region HeaderHoverBackground

    public static readonly AttachedProperty<IBrush?> HeaderHoverBackgroundProperty =
        AvaloniaProperty.RegisterAttached<Expander, IBrush?>("HeaderHoverBackground", typeof(ExpanderHelper));

    public static IBrush? GetHeaderHoverBackground(Expander element) => element.GetValue(HeaderHoverBackgroundProperty);
    public static void SetHeaderHoverBackground(Expander element, IBrush? value) => element.SetValue(HeaderHoverBackgroundProperty, value);

    #endregion

    #region HeaderPadding

    public static readonly AttachedProperty<Thickness> HeaderPaddingProperty =
        AvaloniaProperty.RegisterAttached<Expander, Thickness>("HeaderPadding", typeof(ExpanderHelper), new Thickness(12, 10));

    public static Thickness GetHeaderPadding(Expander element) => element.GetValue(HeaderPaddingProperty);
    public static void SetHeaderPadding(Expander element, Thickness value) => element.SetValue(HeaderPaddingProperty, value);

    #endregion

    #region ContentPadding

    public static readonly AttachedProperty<Thickness> ContentPaddingProperty =
        AvaloniaProperty.RegisterAttached<Expander, Thickness>("ContentPadding", typeof(ExpanderHelper), new Thickness(12));

    public static Thickness GetContentPadding(Expander element) => element.GetValue(ContentPaddingProperty);
    public static void SetContentPadding(Expander element, Thickness value) => element.SetValue(ContentPaddingProperty, value);

    #endregion

    #region ContentBackground

    public static readonly AttachedProperty<IBrush?> ContentBackgroundProperty =
        AvaloniaProperty.RegisterAttached<Expander, IBrush?>("ContentBackground", typeof(ExpanderHelper));

    public static IBrush? GetContentBackground(Expander element) => element.GetValue(ContentBackgroundProperty);
    public static void SetContentBackground(Expander element, IBrush? value) => element.SetValue(ContentBackgroundProperty, value);

    #endregion

    #region CornerRadius

    public static readonly AttachedProperty<CornerRadius> CornerRadiusProperty =
        AvaloniaProperty.RegisterAttached<Expander, CornerRadius>("CornerRadius", typeof(ExpanderHelper), new CornerRadius(8));

    public static CornerRadius GetCornerRadius(Expander element) => element.GetValue(CornerRadiusProperty);
    public static void SetCornerRadius(Expander element, CornerRadius value) => element.SetValue(CornerRadiusProperty, value);

    #endregion

    #region ExpandIcon

    public static readonly AttachedProperty<object?> ExpandIconProperty =
        AvaloniaProperty.RegisterAttached<Expander, object?>("ExpandIcon", typeof(ExpanderHelper));

    public static object? GetExpandIcon(Expander element) => element.GetValue(ExpandIconProperty);
    public static void SetExpandIcon(Expander element, object? value) => element.SetValue(ExpandIconProperty, value);

    #endregion

    #region IconPlacement

    public static readonly AttachedProperty<ExpandIconPlacement> IconPlacementProperty =
        AvaloniaProperty.RegisterAttached<Expander, ExpandIconPlacement>("IconPlacement", typeof(ExpanderHelper), ExpandIconPlacement.Left);

    public static ExpandIconPlacement GetIconPlacement(Expander element) => element.GetValue(IconPlacementProperty);
    public static void SetIconPlacement(Expander element, ExpandIconPlacement value) => element.SetValue(IconPlacementProperty, value);

    #endregion

    #region AnimationDuration

    public static readonly AttachedProperty<TimeSpan> AnimationDurationProperty =
        AvaloniaProperty.RegisterAttached<Expander, TimeSpan>("AnimationDuration", typeof(ExpanderHelper), TimeSpan.FromMilliseconds(200));

    public static TimeSpan GetAnimationDuration(Expander element) => element.GetValue(AnimationDurationProperty);
    public static void SetAnimationDuration(Expander element, TimeSpan value) => element.SetValue(AnimationDurationProperty, value);

    #endregion

    #region Shadow

    public static readonly AttachedProperty<BoxShadow> ShadowProperty =
        AvaloniaProperty.RegisterAttached<Expander, BoxShadow>("Shadow", typeof(ExpanderHelper));

    public static BoxShadow GetShadow(Expander element) => element.GetValue(ShadowProperty);
    public static void SetShadow(Expander element, BoxShadow value) => element.SetValue(ShadowProperty, value);

    #endregion
}

public enum ExpandIconPlacement
{
    Left,
    Right
}
