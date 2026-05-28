using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace AuraUI.Core.Helpers;

/// <summary>
/// Attached properties for styling ComboBox controls.
/// Usage: aura:ComboBoxHelper.CornerRadius="8"
/// </summary>
public static class ComboBoxHelper
{
    #region CornerRadius

    public static readonly AttachedProperty<CornerRadius> CornerRadiusProperty =
        AvaloniaProperty.RegisterAttached<ComboBox, CornerRadius>("CornerRadius", typeof(ComboBoxHelper), new CornerRadius(4));

    public static CornerRadius GetCornerRadius(ComboBox element) => element.GetValue(CornerRadiusProperty);
    public static void SetCornerRadius(ComboBox element, CornerRadius value) => element.SetValue(CornerRadiusProperty, value);

    #endregion

    #region Placeholder

    public static readonly AttachedProperty<string?> PlaceholderProperty =
        AvaloniaProperty.RegisterAttached<ComboBox, string?>("Placeholder", typeof(ComboBoxHelper));

    public static string? GetPlaceholder(ComboBox element) => element.GetValue(PlaceholderProperty);
    public static void SetPlaceholder(ComboBox element, string? value) => element.SetValue(PlaceholderProperty, value);

    #endregion

    #region PlaceholderForeground

    public static readonly AttachedProperty<IBrush?> PlaceholderForegroundProperty =
        AvaloniaProperty.RegisterAttached<ComboBox, IBrush?>("PlaceholderForeground", typeof(ComboBoxHelper));

    public static IBrush? GetPlaceholderForeground(ComboBox element) => element.GetValue(PlaceholderForegroundProperty);
    public static void SetPlaceholderForeground(ComboBox element, IBrush? value) => element.SetValue(PlaceholderForegroundProperty, value);

    #endregion

    #region FocusBorderBrush

    public static readonly AttachedProperty<IBrush?> FocusBorderBrushProperty =
        AvaloniaProperty.RegisterAttached<ComboBox, IBrush?>("FocusBorderBrush", typeof(ComboBoxHelper));

    public static IBrush? GetFocusBorderBrush(ComboBox element) => element.GetValue(FocusBorderBrushProperty);
    public static void SetFocusBorderBrush(ComboBox element, IBrush? value) => element.SetValue(FocusBorderBrushProperty, value);

    #endregion

    #region HoverBorderBrush

    public static readonly AttachedProperty<IBrush?> HoverBorderBrushProperty =
        AvaloniaProperty.RegisterAttached<ComboBox, IBrush?>("HoverBorderBrush", typeof(ComboBoxHelper));

    public static IBrush? GetHoverBorderBrush(ComboBox element) => element.GetValue(HoverBorderBrushProperty);
    public static void SetHoverBorderBrush(ComboBox element, IBrush? value) => element.SetValue(HoverBorderBrushProperty, value);

    #endregion

    #region DropdownCornerRadius

    public static readonly AttachedProperty<CornerRadius> DropdownCornerRadiusProperty =
        AvaloniaProperty.RegisterAttached<ComboBox, CornerRadius>("DropdownCornerRadius", typeof(ComboBoxHelper), new CornerRadius(4));

    public static CornerRadius GetDropdownCornerRadius(ComboBox element) => element.GetValue(DropdownCornerRadiusProperty);
    public static void SetDropdownCornerRadius(ComboBox element, CornerRadius value) => element.SetValue(DropdownCornerRadiusProperty, value);

    #endregion

    #region DropdownShadow

    public static readonly AttachedProperty<BoxShadow> DropdownShadowProperty =
        AvaloniaProperty.RegisterAttached<ComboBox, BoxShadow>("DropdownShadow", typeof(ComboBoxHelper));

    public static BoxShadow GetDropdownShadow(ComboBox element) => element.GetValue(DropdownShadowProperty);
    public static void SetDropdownShadow(ComboBox element, BoxShadow value) => element.SetValue(DropdownShadowProperty, value);

    #endregion

    #region MaxDropdownHeight

    public static readonly AttachedProperty<double> MaxDropdownHeightProperty =
        AvaloniaProperty.RegisterAttached<ComboBox, double>("MaxDropdownHeight", typeof(ComboBoxHelper), 300);

    public static double GetMaxDropdownHeight(ComboBox element) => element.GetValue(MaxDropdownHeightProperty);
    public static void SetMaxDropdownHeight(ComboBox element, double value) => element.SetValue(MaxDropdownHeightProperty, value);

    #endregion

    #region IsSearchable

    public static readonly AttachedProperty<bool> IsSearchableProperty =
        AvaloniaProperty.RegisterAttached<ComboBox, bool>("IsSearchable", typeof(ComboBoxHelper));

    public static bool GetIsSearchable(ComboBox element) => element.GetValue(IsSearchableProperty);
    public static void SetIsSearchable(ComboBox element, bool value) => element.SetValue(IsSearchableProperty, value);

    #endregion

    #region PrefixIcon

    public static readonly AttachedProperty<object?> PrefixIconProperty =
        AvaloniaProperty.RegisterAttached<ComboBox, object?>("PrefixIcon", typeof(ComboBoxHelper));

    public static object? GetPrefixIcon(ComboBox element) => element.GetValue(PrefixIconProperty);
    public static void SetPrefixIcon(ComboBox element, object? value) => element.SetValue(PrefixIconProperty, value);

    #endregion

    #region Label

    public static readonly AttachedProperty<string?> LabelProperty =
        AvaloniaProperty.RegisterAttached<ComboBox, string?>("Label", typeof(ComboBoxHelper));

    public static string? GetLabel(ComboBox element) => element.GetValue(LabelProperty);
    public static void SetLabel(ComboBox element, string? value) => element.SetValue(LabelProperty, value);

    #endregion

    #region HasError

    public static readonly AttachedProperty<bool> HasErrorProperty =
        AvaloniaProperty.RegisterAttached<ComboBox, bool>("HasError", typeof(ComboBoxHelper));

    public static bool GetHasError(ComboBox element) => element.GetValue(HasErrorProperty);
    public static void SetHasError(ComboBox element, bool value) => element.SetValue(HasErrorProperty, value);

    #endregion

    #region ErrorText

    public static readonly AttachedProperty<string?> ErrorTextProperty =
        AvaloniaProperty.RegisterAttached<ComboBox, string?>("ErrorText", typeof(ComboBoxHelper));

    public static string? GetErrorText(ComboBox element) => element.GetValue(ErrorTextProperty);
    public static void SetErrorText(ComboBox element, string? value) => element.SetValue(ErrorTextProperty, value);

    #endregion
}
