using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace AuraUI.Controls.Input;

/// <summary>
/// Specifies the keyboard layout type.
/// </summary>
public enum KeyboardLayout
{
    /// <summary>Standard QWERTY layout.</summary>
    QWERTY,
    /// <summary>Numeric keypad layout.</summary>
    Numeric,
    /// <summary>Phone dial pad layout.</summary>
    Phone,
    /// <summary>User-defined custom layout.</summary>
    Custom
}

/// <summary>
/// Event arguments for key press events on the soft keyboard.
/// </summary>
public class SoftKeyboardEventArgs : RoutedEventArgs
{
    /// <summary>
    /// Gets the key value that was pressed.
    /// </summary>
    public string KeyValue { get; }

    /// <summary>
    /// Gets whether the key is a special key (e.g., Shift, CapsLock, Backspace).
    /// </summary>
    public bool IsSpecialKey { get; }

    public SoftKeyboardEventArgs(string keyValue, bool isSpecialKey)
    {
        KeyValue = keyValue;
        IsSpecialKey = isSpecialKey;
    }
}

/// <summary>
/// An on-screen soft keyboard for touch devices with multiple layout support.
/// Supports QWERTY, numeric, phone, and custom layouts with key press animations
/// and language switching. Inspired by Material UI's virtual keyboard patterns.
/// </summary>
[TemplatePart("PART_KeysPanel", typeof(Panel))]
[PseudoClasses(":visible", ":hidden", ":capslock", ":shift", ":qwerty", ":numeric", ":phone", ":custom")]
public class SoftKeyboard : TemplatedControl
{
    private Panel? _keysPanel;

    /// <summary>
    /// Defines the <see cref="Layout"/> styled property.
    /// </summary>
    public static readonly StyledProperty<KeyboardLayout> LayoutProperty =
        AvaloniaProperty.Register<SoftKeyboard, KeyboardLayout>(
            nameof(Layout), KeyboardLayout.QWERTY);

    /// <summary>
    /// Defines the <see cref="IsKeyboardVisible"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsKeyboardVisibleProperty =
        AvaloniaProperty.Register<SoftKeyboard, bool>(nameof(IsKeyboardVisible));

    /// <summary>
    /// Defines the <see cref="TargetInput"/> styled property.
    /// The text input control that the keyboard types into.
    /// </summary>
    public static readonly StyledProperty<TextBox?> TargetInputProperty =
        AvaloniaProperty.Register<SoftKeyboard, TextBox?>(nameof(TargetInput));

    /// <summary>
    /// Defines the <see cref="CapsLock"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> CapsLockProperty =
        AvaloniaProperty.Register<SoftKeyboard, bool>(nameof(CapsLock));

    /// <summary>
    /// Defines the <see cref="ShiftActive"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> ShiftActiveProperty =
        AvaloniaProperty.Register<SoftKeyboard, bool>(nameof(ShiftActive));

    /// <summary>
    /// Defines the <see cref="Language"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> LanguageProperty =
        AvaloniaProperty.Register<SoftKeyboard, string?>(nameof(Language), "en");

    /// <summary>
    /// Defines the <see cref="CustomKeys"/> styled property.
    /// Custom key layout for KeyboardLayout.Custom mode.
    /// </summary>
    public static readonly StyledProperty<IList<IList<string>>?> CustomKeysProperty =
        AvaloniaProperty.Register<SoftKeyboard, IList<IList<string>>?>(nameof(CustomKeys));

    static SoftKeyboard()
    {
        LayoutProperty.Changed.AddClassHandler<SoftKeyboard>((x, _) => x.UpdatePseudoClasses());
        IsKeyboardVisibleProperty.Changed.AddClassHandler<SoftKeyboard>((x, _) => x.UpdatePseudoClasses());
        CapsLockProperty.Changed.AddClassHandler<SoftKeyboard>((x, _) => x.UpdatePseudoClasses());
        ShiftActiveProperty.Changed.AddClassHandler<SoftKeyboard>((x, _) => x.UpdatePseudoClasses());
    }

    /// <summary>
    /// Gets or sets the keyboard layout.
    /// </summary>
    public KeyboardLayout Layout
    {
        get => GetValue(LayoutProperty);
        set => SetValue(LayoutProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the keyboard is visible.
    /// </summary>
    public bool IsKeyboardVisible
    {
        get => GetValue(IsKeyboardVisibleProperty);
        set => SetValue(IsKeyboardVisibleProperty, value);
    }

    /// <summary>
    /// Gets or sets the target text input.
    /// </summary>
    public TextBox? TargetInput
    {
        get => GetValue(TargetInputProperty);
        set => SetValue(TargetInputProperty, value);
    }

    /// <summary>
    /// Gets or sets whether CapsLock is active.
    /// </summary>
    public bool CapsLock
    {
        get => GetValue(CapsLockProperty);
        set => SetValue(CapsLockProperty, value);
    }

    /// <summary>
    /// Gets or sets whether Shift is active.
    /// </summary>
    public bool ShiftActive
    {
        get => GetValue(ShiftActiveProperty);
        set => SetValue(ShiftActiveProperty, value);
    }

    /// <summary>
    /// Gets or sets the language code (e.g., "en", "fr", "ja").
    /// </summary>
    public string? Language
    {
        get => GetValue(LanguageProperty);
        set => SetValue(LanguageProperty, value);
    }

    /// <summary>
    /// Gets or sets custom key definitions for KeyboardLayout.Custom mode.
    /// Each inner list represents a row of keys.
    /// </summary>
    public IList<IList<string>>? CustomKeys
    {
        get => GetValue(CustomKeysProperty);
        set => SetValue(CustomKeysProperty, value);
    }

    /// <summary>
    /// Occurs when a key is pressed.
    /// </summary>
    public event EventHandler<SoftKeyboardEventArgs>? KeyPressed;

    /// <summary>
    /// Occurs when the layout changes.
    /// </summary>
    public event EventHandler<RoutedEventArgs>? LayoutChanged;

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _keysPanel = e.NameScope.Find<Panel>("PART_KeysPanel");
        UpdatePseudoClasses();
    }

    /// <summary>
    /// Shows the keyboard.
    /// </summary>
    public void Show()
    {
        IsKeyboardVisible = true;
    }

    /// <summary>
    /// Hides the keyboard.
    /// </summary>
    public void Hide()
    {
        IsKeyboardVisible = false;
    }

    /// <summary>
    /// Toggles keyboard visibility.
    /// </summary>
    public void Toggle()
    {
        IsKeyboardVisible = !IsKeyboardVisible;
    }

    /// <summary>
    /// Switches to the specified layout.
    /// </summary>
    public void SwitchLayout(KeyboardLayout newLayout)
    {
        Layout = newLayout;
        LayoutChanged?.Invoke(this, new RoutedEventArgs());
    }

    /// <summary>
    /// Toggles CapsLock state.
    /// </summary>
    public void ToggleCapsLock()
    {
        CapsLock = !CapsLock;
    }

    /// <summary>
    /// Processes a key press from the keyboard UI.
    /// </summary>
    public virtual void ProcessKeyPress(string keyValue)
    {
        var isSpecial = false;

        switch (keyValue)
        {
            case "CapsLock":
                ToggleCapsLock();
                isSpecial = true;
                break;
            case "Shift":
                ShiftActive = !ShiftActive;
                isSpecial = true;
                break;
            case "Backspace":
                var target = TargetInput;
                if (target != null && target.Text?.Length > 0)
                {
                    target.Text = target.Text[..^1];
                }
                isSpecial = true;
                break;
            case "Enter":
                TargetInput?.RaiseEvent(new KeyEventArgs
                {
                    Key = Key.Enter
                });
                isSpecial = true;
                break;
            case "Space":
                InsertText(" ");
                isSpecial = true;
                break;
            case "Tab":
                isSpecial = true;
                break;
            default:
                InsertText(keyValue);
                break;
        }

        // Reset shift after a key press
        if (ShiftActive && !isSpecial)
        {
            ShiftActive = false;
        }

        KeyPressed?.Invoke(this, new SoftKeyboardEventArgs(keyValue, isSpecial));
    }

    /// <summary>
    /// Gets the keys for the current layout.
    /// </summary>
    public IList<IList<string>> GetLayoutKeys()
    {
        return Layout switch
        {
            KeyboardLayout.QWERTY => GetQwertyLayout(),
            KeyboardLayout.Numeric => GetNumericLayout(),
            KeyboardLayout.Phone => GetPhoneLayout(),
            KeyboardLayout.Custom => CustomKeys ?? GetQwertyLayout(),
            _ => GetQwertyLayout()
        };
    }

    private void InsertText(string text)
    {
        var target = TargetInput;
        if (target == null) return;

        var shouldUpper = CapsLock ^ ShiftActive;
        var insertText = shouldUpper ? text.ToUpper() : text.ToLower();

        var caretIndex = target.CaretIndex;
        var currentText = target.Text ?? string.Empty;
        target.Text = currentText.Insert(caretIndex, insertText);
        target.CaretIndex = caretIndex + insertText.Length;
    }

    private static IList<IList<string>> GetQwertyLayout()
    {
        return new List<IList<string>>
        {
            new List<string> { "1", "2", "3", "4", "5", "6", "7", "8", "9", "0" },
            new List<string> { "q", "w", "e", "r", "t", "y", "u", "i", "o", "p" },
            new List<string> { "a", "s", "d", "f", "g", "h", "j", "k", "l" },
            new List<string> { "CapsLock", "z", "x", "c", "v", "b", "n", "m", "Backspace" },
            new List<string> { "Shift", ",", "Space", ".", "Enter" }
        };
    }

    private static IList<IList<string>> GetNumericLayout()
    {
        return new List<IList<string>>
        {
            new List<string> { "7", "8", "9" },
            new List<string> { "4", "5", "6" },
            new List<string> { "1", "2", "3" },
            new List<string> { ".", "0", "Backspace" },
            new List<string> { "Enter" }
        };
    }

    private static IList<IList<string>> GetPhoneLayout()
    {
        return new List<IList<string>>
        {
            new List<string> { "1", "2", "3" },
            new List<string> { "4", "5", "6" },
            new List<string> { "7", "8", "9" },
            new List<string> { "*", "0", "#" },
            new List<string> { "Backspace", "Enter" }
        };
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(":visible", IsKeyboardVisible);
        PseudoClasses.Set(":hidden", !IsKeyboardVisible);
        PseudoClasses.Set(":capslock", CapsLock);
        PseudoClasses.Set(":shift", ShiftActive);
        PseudoClasses.Set(":qwerty", Layout == KeyboardLayout.QWERTY);
        PseudoClasses.Set(":numeric", Layout == KeyboardLayout.Numeric);
        PseudoClasses.Set(":phone", Layout == KeyboardLayout.Phone);
        PseudoClasses.Set(":custom", Layout == KeyboardLayout.Custom);
    }
}
