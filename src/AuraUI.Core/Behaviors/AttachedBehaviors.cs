using System.Text.RegularExpressions;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.Interactivity;

namespace AuraUI.Core.Behaviors;

/// <summary>
/// Attached properties for common input behaviors on TextBox controls.
/// Usage: aura:InputBehavior.IsNumericOnly="True"
/// </summary>
public static partial class InputBehavior
{
    #region IsNumericOnly

    public static readonly AttachedProperty<bool> IsNumericOnlyProperty =
        AvaloniaProperty.RegisterAttached<TextBox, bool>(
            "IsNumericOnly", typeof(InputBehavior));

    public static bool GetIsNumericOnly(TextBox element) => element.GetValue(IsNumericOnlyProperty);
    public static void SetIsNumericOnly(TextBox element, bool value) => element.SetValue(IsNumericOnlyProperty, value);

    #endregion

    #region MaxLength

    public static readonly AttachedProperty<int> MaxLengthProperty =
        AvaloniaProperty.RegisterAttached<TextBox, int>(
            "MaxLength", typeof(InputBehavior), 0);

    public static int GetMaxLength(TextBox element) => element.GetValue(MaxLengthProperty);
    public static void SetMaxLength(TextBox element, int value) => element.SetValue(MaxLengthProperty, value);

    #endregion

    #region SelectAllOnFocus

    public static readonly AttachedProperty<bool> SelectAllOnFocusProperty =
        AvaloniaProperty.RegisterAttached<TextBox, bool>(
            "SelectAllOnFocus", typeof(InputBehavior));

    public static bool GetSelectAllOnFocus(TextBox element) => element.GetValue(SelectAllOnFocusProperty);
    public static void SetSelectAllOnFocus(TextBox element, bool value) => element.SetValue(SelectAllOnFocusProperty, value);

    #endregion

    #region MoveFocusOnEnter

    public static readonly AttachedProperty<bool> MoveFocusOnEnterProperty =
        AvaloniaProperty.RegisterAttached<TextBox, bool>(
            "MoveFocusOnEnter", typeof(InputBehavior));

    public static bool GetMoveFocusOnEnter(TextBox element) => element.GetValue(MoveFocusOnEnterProperty);
    public static void SetMoveFocusOnEnter(TextBox element, bool value) => element.SetValue(MoveFocusOnEnterProperty, value);

    #endregion

    #region AllowDecimal

    public static readonly AttachedProperty<bool> AllowDecimalProperty =
        AvaloniaProperty.RegisterAttached<TextBox, bool>(
            "AllowDecimal", typeof(InputBehavior));

    public static bool GetAllowDecimal(TextBox element) => element.GetValue(AllowDecimalProperty);
    public static void SetAllowDecimal(TextBox element, bool value) => element.SetValue(AllowDecimalProperty, value);

    #endregion

    #region AllowNegative

    public static readonly AttachedProperty<bool> AllowNegativeProperty =
        AvaloniaProperty.RegisterAttached<TextBox, bool>(
            "AllowNegative", typeof(InputBehavior));

    public static bool GetAllowNegative(TextBox element) => element.GetValue(AllowNegativeProperty);
    public static void SetAllowNegative(TextBox element, bool value) => element.SetValue(AllowNegativeProperty, value);

    #endregion

    static InputBehavior()
    {
        IsNumericOnlyProperty.Changed.AddClassHandler<TextBox>(OnIsNumericOnlyChanged);
        MaxLengthProperty.Changed.AddClassHandler<TextBox>(OnMaxLengthChanged);
        SelectAllOnFocusProperty.Changed.AddClassHandler<TextBox>(OnSelectAllOnFocusChanged);
        MoveFocusOnEnterProperty.Changed.AddClassHandler<TextBox>(OnMoveFocusOnEnterChanged);
    }

    private static void OnIsNumericOnlyChanged(TextBox textBox, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.NewValue is true)
        {
            textBox.TextInput += OnNumericTextInput;
            textBox.PastingFromClipboard += OnNumericPaste;
        }
        else
        {
            textBox.TextInput -= OnNumericTextInput;
            textBox.PastingFromClipboard -= OnNumericPaste;
        }
    }

    [GeneratedRegex(@"[0-9]")]
    private static partial Regex NumericRegex();

    private static void OnNumericTextInput(object? sender, TextInputEventArgs e)
    {
        if (sender is not TextBox textBox || string.IsNullOrEmpty(e.Text))
            return;

        bool allowDecimal = GetAllowDecimal(textBox);
        bool allowNegative = GetAllowNegative(textBox);

        foreach (char c in e.Text)
        {
            if (char.IsDigit(c))
                continue;

            if (allowDecimal && c == '.' && !textBox.Text?.Contains('.') == true)
                continue;

            if (allowNegative && c == '-' && textBox.CaretIndex == 0 && !textBox.Text?.Contains('-') == true)
                continue;

            e.Handled = true;
            return;
        }
    }

    private static async void OnNumericPaste(object? sender, RoutedEventArgs e)
    {
        if (sender is not TextBox textBox)
            return;

        var clipboard = TopLevel.GetTopLevel(textBox)?.Clipboard;
        if (clipboard == null)
            return;

        var text = await clipboard.TryGetTextAsync();
        if (string.IsNullOrEmpty(text))
            return;

        bool allowDecimal = GetAllowDecimal(textBox);
        bool allowNegative = GetAllowNegative(textBox);

        foreach (char c in text)
        {
            if (char.IsDigit(c))
                continue;

            if (allowDecimal && c == '.')
                continue;

            if (allowNegative && c == '-')
                continue;

            // Invalid character found, cancel the paste
            e.Handled = true;
            return;
        }
    }

    private static void OnMaxLengthChanged(TextBox textBox, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.NewValue is int maxLength && maxLength > 0)
        {
            textBox.TextInput += OnMaxLengthTextInput;
        }
        else
        {
            textBox.TextInput -= OnMaxLengthTextInput;
        }
    }

    private static void OnMaxLengthTextInput(object? sender, TextInputEventArgs e)
    {
        if (sender is not TextBox textBox || string.IsNullOrEmpty(e.Text))
            return;

        int maxLength = GetMaxLength(textBox);
        if (maxLength <= 0)
            return;

        int currentLength = textBox.Text?.Length ?? 0;
        int selectionLength = textBox.SelectionEnd - textBox.SelectionStart;

        // Account for selected text being replaced
        int newLength = currentLength - selectionLength + e.Text.Length;

        if (newLength > maxLength)
        {
            e.Handled = true;
        }
    }

    private static void OnSelectAllOnFocusChanged(TextBox textBox, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.NewValue is true)
        {
            textBox.GotFocus += OnSelectAllGotFocus;
        }
        else
        {
            textBox.GotFocus -= OnSelectAllGotFocus;
        }
    }

    private static void OnSelectAllGotFocus(object? sender, FocusChangedEventArgs e)
    {
        if (sender is TextBox textBox)
        {
            Avalonia.Threading.Dispatcher.UIThread.Post(() =>
            {
                textBox.SelectAll();
            }, Avalonia.Threading.DispatcherPriority.Input);
        }
    }

    private static void OnMoveFocusOnEnterChanged(TextBox textBox, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.NewValue is true)
        {
            textBox.KeyDown += OnMoveFocusKeyDown;
        }
        else
        {
            textBox.KeyDown -= OnMoveFocusKeyDown;
        }
    }

    private static void OnMoveFocusKeyDown(object? sender, KeyEventArgs e)
    {
        if (sender is not TextBox textBox)
            return;

        if (e.Key == Key.Enter)
        {
            // Mark Enter as handled to prevent default behavior
            // Focus advancement can be configured via AuraUI.Core.Navigation.FocusManager
            e.Handled = true;
        }
    }
}
