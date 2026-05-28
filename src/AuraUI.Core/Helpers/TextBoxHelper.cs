using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace AuraUI.Core.Helpers;

/// <summary>
/// Attached properties for styling TextBox / text input controls.
/// Usage: aura:TextBoxHelper.Placeholder="Enter text..."
/// </summary>
public static class TextBoxHelper
{
    #region Placeholder (Watermark)

    public static readonly AttachedProperty<string?> PlaceholderProperty =
        AvaloniaProperty.RegisterAttached<TextBox, string?>("Placeholder", typeof(TextBoxHelper));

    public static string? GetPlaceholder(TextBox element) => element.GetValue(PlaceholderProperty);
    public static void SetPlaceholder(TextBox element, string? value) => element.SetValue(PlaceholderProperty, value);

    #endregion

    #region PlaceholderForeground

    public static readonly AttachedProperty<IBrush?> PlaceholderForegroundProperty =
        AvaloniaProperty.RegisterAttached<TextBox, IBrush?>("PlaceholderForeground", typeof(TextBoxHelper));

    public static IBrush? GetPlaceholderForeground(TextBox element) => element.GetValue(PlaceholderForegroundProperty);
    public static void SetPlaceholderForeground(TextBox element, IBrush? value) => element.SetValue(PlaceholderForegroundProperty, value);

    #endregion

    #region CornerRadius

    public static readonly AttachedProperty<CornerRadius> CornerRadiusProperty =
        AvaloniaProperty.RegisterAttached<TextBox, CornerRadius>("CornerRadius", typeof(TextBoxHelper), new CornerRadius(4));

    public static CornerRadius GetCornerRadius(TextBox element) => element.GetValue(CornerRadiusProperty);
    public static void SetCornerRadius(TextBox element, CornerRadius value) => element.SetValue(CornerRadiusProperty, value);

    #endregion

    #region FocusBorderBrush

    public static readonly AttachedProperty<IBrush?> FocusBorderBrushProperty =
        AvaloniaProperty.RegisterAttached<TextBox, IBrush?>("FocusBorderBrush", typeof(TextBoxHelper));

    public static IBrush? GetFocusBorderBrush(TextBox element) => element.GetValue(FocusBorderBrushProperty);
    public static void SetFocusBorderBrush(TextBox element, IBrush? value) => element.SetValue(FocusBorderBrushProperty, value);

    #endregion

    #region HoverBorderBrush

    public static readonly AttachedProperty<IBrush?> HoverBorderBrushProperty =
        AvaloniaProperty.RegisterAttached<TextBox, IBrush?>("HoverBorderBrush", typeof(TextBoxHelper));

    public static IBrush? GetHoverBorderBrush(TextBox element) => element.GetValue(HoverBorderBrushProperty);
    public static void SetHoverBorderBrush(TextBox element, IBrush? value) => element.SetValue(HoverBorderBrushProperty, value);

    #endregion

    #region IsClearable

    public static readonly AttachedProperty<bool> IsClearableProperty =
        AvaloniaProperty.RegisterAttached<TextBox, bool>("IsClearable", typeof(TextBoxHelper));

    public static bool GetIsClearable(TextBox element) => element.GetValue(IsClearableProperty);
    public static void SetIsClearable(TextBox element, bool value) => element.SetValue(IsClearableProperty, value);

    #endregion

    #region Prefix

    public static readonly AttachedProperty<string?> PrefixProperty =
        AvaloniaProperty.RegisterAttached<TextBox, string?>("Prefix", typeof(TextBoxHelper));

    public static string? GetPrefix(TextBox element) => element.GetValue(PrefixProperty);
    public static void SetPrefix(TextBox element, string? value) => element.SetValue(PrefixProperty, value);

    #endregion

    #region Suffix

    public static readonly AttachedProperty<string?> SuffixProperty =
        AvaloniaProperty.RegisterAttached<TextBox, string?>("Suffix", typeof(TextBoxHelper));

    public static string? GetSuffix(TextBox element) => element.GetValue(SuffixProperty);
    public static void SetSuffix(TextBox element, string? value) => element.SetValue(SuffixProperty, value);

    #endregion

    #region PrefixIcon

    public static readonly AttachedProperty<object?> PrefixIconProperty =
        AvaloniaProperty.RegisterAttached<TextBox, object?>("PrefixIcon", typeof(TextBoxHelper));

    public static object? GetPrefixIcon(TextBox element) => element.GetValue(PrefixIconProperty);
    public static void SetPrefixIcon(TextBox element, object? value) => element.SetValue(PrefixIconProperty, value);

    #endregion

    #region SuffixIcon

    public static readonly AttachedProperty<object?> SuffixIconProperty =
        AvaloniaProperty.RegisterAttached<TextBox, object?>("SuffixIcon", typeof(TextBoxHelper));

    public static object? GetSuffixIcon(TextBox element) => element.GetValue(SuffixIconProperty);
    public static void SetSuffixIcon(TextBox element, object? value) => element.SetValue(SuffixIconProperty, value);

    #endregion

    #region ShowCharacterCount

    public static readonly AttachedProperty<bool> ShowCharacterCountProperty =
        AvaloniaProperty.RegisterAttached<TextBox, bool>("ShowCharacterCount", typeof(TextBoxHelper));

    public static bool GetShowCharacterCount(TextBox element) => element.GetValue(ShowCharacterCountProperty);
    public static void SetShowCharacterCount(TextBox element, bool value) => element.SetValue(ShowCharacterCountProperty, value);

    #endregion

    #region MaxCharacters

    public static readonly AttachedProperty<int> MaxCharactersProperty =
        AvaloniaProperty.RegisterAttached<TextBox, int>("MaxCharacters", typeof(TextBoxHelper), 0);

    public static int GetMaxCharacters(TextBox element) => element.GetValue(MaxCharactersProperty);
    public static void SetMaxCharacters(TextBox element, int value) => element.SetValue(MaxCharactersProperty, value);

    #endregion

    #region ErrorText

    public static readonly AttachedProperty<string?> ErrorTextProperty =
        AvaloniaProperty.RegisterAttached<TextBox, string?>("ErrorText", typeof(TextBoxHelper));

    public static string? GetErrorText(TextBox element) => element.GetValue(ErrorTextProperty);
    public static void SetErrorText(TextBox element, string? value) => element.SetValue(ErrorTextProperty, value);

    #endregion

    #region HasError

    public static readonly AttachedProperty<bool> HasErrorProperty =
        AvaloniaProperty.RegisterAttached<TextBox, bool>("HasError", typeof(TextBoxHelper));

    public static bool GetHasError(TextBox element) => element.GetValue(HasErrorProperty);
    public static void SetHasError(TextBox element, bool value) => element.SetValue(HasErrorProperty, value);

    #endregion

    #region ErrorBorderBrush

    public static readonly AttachedProperty<IBrush?> ErrorBorderBrushProperty =
        AvaloniaProperty.RegisterAttached<TextBox, IBrush?>("ErrorBorderBrush", typeof(TextBoxHelper));

    public static IBrush? GetErrorBorderBrush(TextBox element) => element.GetValue(ErrorBorderBrushProperty);
    public static void SetErrorBorderBrush(TextBox element, IBrush? value) => element.SetValue(ErrorBorderBrushProperty, value);

    #endregion

    #region Label

    public static readonly AttachedProperty<string?> LabelProperty =
        AvaloniaProperty.RegisterAttached<TextBox, string?>("Label", typeof(TextBoxHelper));

    public static string? GetLabel(TextBox element) => element.GetValue(LabelProperty);
    public static void SetLabel(TextBox element, string? value) => element.SetValue(LabelProperty, value);

    #endregion

    #region HelperText

    public static readonly AttachedProperty<string?> HelperTextProperty =
        AvaloniaProperty.RegisterAttached<TextBox, string?>("HelperText", typeof(TextBoxHelper));

    public static string? GetHelperText(TextBox element) => element.GetValue(HelperTextProperty);
    public static void SetHelperText(TextBox element, string? value) => element.SetValue(HelperTextProperty, value);

    #endregion
}
