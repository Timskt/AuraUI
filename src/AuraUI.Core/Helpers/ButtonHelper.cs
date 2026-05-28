using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace AuraUI.Core.Helpers;

/// <summary>
/// Attached properties for styling Button controls.
/// Usage: aura:ButtonHelper.CornerRadius="20"
/// </summary>
public static class ButtonHelper
{
    #region CornerRadius

    public static readonly AttachedProperty<CornerRadius> CornerRadiusProperty =
        AvaloniaProperty.RegisterAttached<Button, CornerRadius>("CornerRadius", typeof(ButtonHelper));

    public static CornerRadius GetCornerRadius(Button element) => element.GetValue(CornerRadiusProperty);
    public static void SetCornerRadius(Button element, CornerRadius value) => element.SetValue(CornerRadiusProperty, value);

    #endregion

    #region HoverBackground

    public static readonly AttachedProperty<IBrush?> HoverBackgroundProperty =
        AvaloniaProperty.RegisterAttached<Button, IBrush?>("HoverBackground", typeof(ButtonHelper));

    public static IBrush? GetHoverBackground(Button element) => element.GetValue(HoverBackgroundProperty);
    public static void SetHoverBackground(Button element, IBrush? value) => element.SetValue(HoverBackgroundProperty, value);

    #endregion

    #region HoverForeground

    public static readonly AttachedProperty<IBrush?> HoverForegroundProperty =
        AvaloniaProperty.RegisterAttached<Button, IBrush?>("HoverForeground", typeof(ButtonHelper));

    public static IBrush? GetHoverForeground(Button element) => element.GetValue(HoverForegroundProperty);
    public static void SetHoverForeground(Button element, IBrush? value) => element.SetValue(HoverForegroundProperty, value);

    #endregion

    #region HoverBorderBrush

    public static readonly AttachedProperty<IBrush?> HoverBorderBrushProperty =
        AvaloniaProperty.RegisterAttached<Button, IBrush?>("HoverBorderBrush", typeof(ButtonHelper));

    public static IBrush? GetHoverBorderBrush(Button element) => element.GetValue(HoverBorderBrushProperty);
    public static void SetHoverBorderBrush(Button element, IBrush? value) => element.SetValue(HoverBorderBrushProperty, value);

    #endregion

    #region ClickBackground

    public static readonly AttachedProperty<IBrush?> ClickBackgroundProperty =
        AvaloniaProperty.RegisterAttached<Button, IBrush?>("ClickBackground", typeof(ButtonHelper));

    public static IBrush? GetClickBackground(Button element) => element.GetValue(ClickBackgroundProperty);
    public static void SetClickBackground(Button element, IBrush? value) => element.SetValue(ClickBackgroundProperty, value);

    #endregion

    #region ClickForeground

    public static readonly AttachedProperty<IBrush?> ClickForegroundProperty =
        AvaloniaProperty.RegisterAttached<Button, IBrush?>("ClickForeground", typeof(ButtonHelper));

    public static IBrush? GetClickForeground(Button element) => element.GetValue(ClickForegroundProperty);
    public static void SetClickForeground(Button element, IBrush? value) => element.SetValue(ClickForegroundProperty, value);

    #endregion

    #region ClickBorderBrush

    public static readonly AttachedProperty<IBrush?> ClickBorderBrushProperty =
        AvaloniaProperty.RegisterAttached<Button, IBrush?>("ClickBorderBrush", typeof(ButtonHelper));

    public static IBrush? GetClickBorderBrush(Button element) => element.GetValue(ClickBorderBrushProperty);
    public static void SetClickBorderBrush(Button element, IBrush? value) => element.SetValue(ClickBorderBrushProperty, value);

    #endregion

    #region Icon

    public static readonly AttachedProperty<object?> IconProperty =
        AvaloniaProperty.RegisterAttached<Button, object?>("Icon", typeof(ButtonHelper));

    public static object? GetIcon(Button element) => element.GetValue(IconProperty);
    public static void SetIcon(Button element, object? value) => element.SetValue(IconProperty, value);

    #endregion

    #region IconWidth

    public static readonly AttachedProperty<double> IconWidthProperty =
        AvaloniaProperty.RegisterAttached<Button, double>("IconWidth", typeof(ButtonHelper), 16);

    public static double GetIconWidth(Button element) => element.GetValue(IconWidthProperty);
    public static void SetIconWidth(Button element, double value) => element.SetValue(IconWidthProperty, value);

    #endregion

    #region IconHeight

    public static readonly AttachedProperty<double> IconHeightProperty =
        AvaloniaProperty.RegisterAttached<Button, double>("IconHeight", typeof(ButtonHelper), 16);

    public static double GetIconHeight(Button element) => element.GetValue(IconHeightProperty);
    public static void SetIconHeight(Button element, double value) => element.SetValue(IconHeightProperty, value);

    #endregion

    #region IconMargin

    public static readonly AttachedProperty<Thickness> IconMarginProperty =
        AvaloniaProperty.RegisterAttached<Button, Thickness>("IconMargin", typeof(ButtonHelper), new Thickness(0, 0, 4, 0));

    public static Thickness GetIconMargin(Button element) => element.GetValue(IconMarginProperty);
    public static void SetIconMargin(Button element, Thickness value) => element.SetValue(IconMarginProperty, value);

    #endregion

    #region IconPlacement

    public static readonly AttachedProperty<IconPlacement> IconPlacementProperty =
        AvaloniaProperty.RegisterAttached<Button, IconPlacement>("IconPlacement", typeof(ButtonHelper), IconPlacement.Left);

    public static IconPlacement GetIconPlacement(Button element) => element.GetValue(IconPlacementProperty);
    public static void SetIconPlacement(Button element, IconPlacement value) => element.SetValue(IconPlacementProperty, value);

    #endregion

    #region LoadingContent

    public static readonly AttachedProperty<object?> LoadingContentProperty =
        AvaloniaProperty.RegisterAttached<Button, object?>("LoadingContent", typeof(ButtonHelper), "Loading...");

    public static object? GetLoadingContent(Button element) => element.GetValue(LoadingContentProperty);
    public static void SetLoadingContent(Button element, object? value) => element.SetValue(LoadingContentProperty, value);

    #endregion

    #region IsLoading

    public static readonly AttachedProperty<bool> IsLoadingProperty =
        AvaloniaProperty.RegisterAttached<Button, bool>("IsLoading", typeof(ButtonHelper));

    public static bool GetIsLoading(Button element) => element.GetValue(IsLoadingProperty);
    public static void SetIsLoading(Button element, bool value) => element.SetValue(IsLoadingProperty, value);

    #endregion

    #region DisabledBackground

    public static readonly AttachedProperty<IBrush?> DisabledBackgroundProperty =
        AvaloniaProperty.RegisterAttached<Button, IBrush?>("DisabledBackground", typeof(ButtonHelper));

    public static IBrush? GetDisabledBackground(Button element) => element.GetValue(DisabledBackgroundProperty);
    public static void SetDisabledBackground(Button element, IBrush? value) => element.SetValue(DisabledBackgroundProperty, value);

    #endregion

    #region DisabledForeground

    public static readonly AttachedProperty<IBrush?> DisabledForegroundProperty =
        AvaloniaProperty.RegisterAttached<Button, IBrush?>("DisabledForeground", typeof(ButtonHelper));

    public static IBrush? GetDisabledForeground(Button element) => element.GetValue(DisabledForegroundProperty);
    public static void SetDisabledForeground(Button element, IBrush? value) => element.SetValue(DisabledForegroundProperty, value);

    #endregion

    #region Shadow

    public static readonly AttachedProperty<BoxShadow> ShadowProperty =
        AvaloniaProperty.RegisterAttached<Button, BoxShadow>("Shadow", typeof(ButtonHelper));

    public static BoxShadow GetShadow(Button element) => element.GetValue(ShadowProperty);
    public static void SetShadow(Button element, BoxShadow value) => element.SetValue(ShadowProperty, value);

    #endregion

    #region HoverShadow

    public static readonly AttachedProperty<BoxShadow> HoverShadowProperty =
        AvaloniaProperty.RegisterAttached<Button, BoxShadow>("HoverShadow", typeof(ButtonHelper));

    public static BoxShadow GetHoverShadow(Button element) => element.GetValue(HoverShadowProperty);
    public static void SetHoverShadow(Button element, BoxShadow value) => element.SetValue(HoverShadowProperty, value);

    #endregion

    #region ContentPadding

    public static readonly AttachedProperty<Thickness> ContentPaddingProperty =
        AvaloniaProperty.RegisterAttached<Button, Thickness>("ContentPadding", typeof(ButtonHelper), new Thickness(16, 8));

    public static Thickness GetContentPadding(Button element) => element.GetValue(ContentPaddingProperty);
    public static void SetContentPadding(Button element, Thickness value) => element.SetValue(ContentPaddingProperty, value);

    #endregion

    #region HoverBorderThickness

    public static readonly AttachedProperty<Thickness> HoverBorderThicknessProperty =
        AvaloniaProperty.RegisterAttached<Button, Thickness>("HoverBorderThickness", typeof(ButtonHelper));

    public static Thickness GetHoverBorderThickness(Button element) => element.GetValue(HoverBorderThicknessProperty);
    public static void SetHoverBorderThickness(Button element, Thickness value) => element.SetValue(HoverBorderThicknessProperty, value);

    #endregion
}

public enum IconPlacement
{
    Left,
    Right,
    Top,
    Bottom
}
