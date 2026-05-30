using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace AuraUI.Controls.Display;

/// <summary>
/// Specifies the size of keyboard shortcut key caps.
/// </summary>
public enum KeyboardShortcutSize
{
    Small,
    Medium,
    Large
}

/// <summary>
/// Displays a keyboard shortcut (e.g. "Ctrl+Shift+P") with styled key caps.
/// Modifier keys (Ctrl, Shift, Alt, Cmd) receive distinct styling.
/// </summary>
public class KeyboardShortcutDisplay : Control
{
    /// <summary>
    /// Defines the <see cref="Shortcut"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> ShortcutProperty =
        AvaloniaProperty.Register<KeyboardShortcutDisplay, string?>(nameof(Shortcut));

    /// <summary>
    /// Defines the <see cref="Size"/> styled property.
    /// </summary>
    public static readonly StyledProperty<KeyboardShortcutSize> SizeProperty =
        AvaloniaProperty.Register<KeyboardShortcutDisplay, KeyboardShortcutSize>(nameof(Size), KeyboardShortcutSize.Medium);

    /// <summary>
    /// Defines the <see cref="KeyBackground"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> KeyBackgroundProperty =
        AvaloniaProperty.Register<KeyboardShortcutDisplay, IBrush?>(nameof(KeyBackground));

    /// <summary>
    /// Defines the <see cref="KeyForeground"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> KeyForegroundProperty =
        AvaloniaProperty.Register<KeyboardShortcutDisplay, IBrush?>(nameof(KeyForeground));

    /// <summary>
    /// Defines the <see cref="KeyBorderBrush"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> KeyBorderBrushProperty =
        AvaloniaProperty.Register<KeyboardShortcutDisplay, IBrush?>(nameof(KeyBorderBrush));

    /// <summary>
    /// Defines the <see cref="ModifierBackground"/> styled property.
    /// Modifier keys get a distinct background to visually differentiate them.
    /// </summary>
    public static readonly StyledProperty<IBrush?> ModifierBackgroundProperty =
        AvaloniaProperty.Register<KeyboardShortcutDisplay, IBrush?>(nameof(ModifierBackground));

    /// <summary>
    /// Defines the <see cref="Separator"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string> SeparatorProperty =
        AvaloniaProperty.Register<KeyboardShortcutDisplay, string>(nameof(Separator), "+");

    static KeyboardShortcutDisplay()
    {
        ShortcutProperty.Changed.AddClassHandler<KeyboardShortcutDisplay>((x, _) => x.InvalidateVisual());
        SizeProperty.Changed.AddClassHandler<KeyboardShortcutDisplay>((x, _) => x.InvalidateMeasure());
        AffectsRender<KeyboardShortcutDisplay>(ShortcutProperty, SizeProperty, KeyBackgroundProperty,
            KeyForegroundProperty, KeyBorderBrushProperty, ModifierBackgroundProperty, SeparatorProperty);
        AffectsMeasure<KeyboardShortcutDisplay>(ShortcutProperty, SizeProperty, SeparatorProperty);
    }

    /// <summary>
    /// Gets or sets the shortcut string (e.g. "Ctrl+Shift+P").
    /// Keys are split by '+' and rendered as individual key caps.
    /// </summary>
    public string? Shortcut
    {
        get => GetValue(ShortcutProperty);
        set => SetValue(ShortcutProperty, value);
    }

    /// <summary>
    /// Gets or sets the display size of the key caps.
    /// </summary>
    public KeyboardShortcutSize Size
    {
        get => GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }

    /// <summary>
    /// Gets or sets the background brush for regular keys.
    /// </summary>
    public IBrush? KeyBackground
    {
        get => GetValue(KeyBackgroundProperty);
        set => SetValue(KeyBackgroundProperty, value);
    }

    /// <summary>
    /// Gets or sets the foreground brush for key text.
    /// </summary>
    public IBrush? KeyForeground
    {
        get => GetValue(KeyForegroundProperty);
        set => SetValue(KeyForegroundProperty, value);
    }

    /// <summary>
    /// Gets or sets the border brush for key caps.
    /// </summary>
    public IBrush? KeyBorderBrush
    {
        get => GetValue(KeyBorderBrushProperty);
        set => SetValue(KeyBorderBrushProperty, value);
    }

    /// <summary>
    /// Gets or sets the background brush for modifier keys.
    /// </summary>
    public IBrush? ModifierBackground
    {
        get => GetValue(ModifierBackgroundProperty);
        set => SetValue(ModifierBackgroundProperty, value);
    }

    /// <summary>
    /// Gets or sets the separator string between keys.
    /// </summary>
    public string Separator
    {
        get => GetValue(SeparatorProperty);
        set => SetValue(SeparatorProperty, value);
    }

    // Cached key parts to avoid re-parsing every render
    private string? _cachedShortcut;
    private string[] _cachedParts = Array.Empty<string>();

    // Default brushes
    private static readonly IBrush s_defaultKeyBackground = new SolidColorBrush(Color.Parse("#F0F0F0"));
    private static readonly IBrush s_defaultKeyForeground = new SolidColorBrush(Color.Parse("#333333"));
    private static readonly IBrush s_defaultKeyBorder = new SolidColorBrush(Color.Parse("#CCCCCC"));
    private static readonly IBrush s_defaultModifierBackground = new SolidColorBrush(Color.Parse("#E0E0E0"));
    private static readonly IBrush s_defaultKeyBackgroundDark = new SolidColorBrush(Color.Parse("#3C3C3C"));
    private static readonly IBrush s_defaultKeyForegroundDark = new SolidColorBrush(Color.Parse("#CCCCCC"));
    private static readonly IBrush s_defaultKeyBorderDark = new SolidColorBrush(Color.Parse("#555555"));
    private static readonly IBrush s_defaultModifierBackgroundDark = new SolidColorBrush(Color.Parse("#505050"));

    // Key cap metrics by size
    private double FontSize => Size switch
    {
        KeyboardShortcutSize.Small => 10,
        KeyboardShortcutSize.Medium => 12,
        KeyboardShortcutSize.Large => 15,
        _ => 12
    };

    private double PaddingH => Size switch
    {
        KeyboardShortcutSize.Small => 4,
        KeyboardShortcutSize.Medium => 6,
        KeyboardShortcutSize.Large => 8,
        _ => 6
    };

    private double PaddingV => Size switch
    {
        KeyboardShortcutSize.Small => 1,
        KeyboardShortcutSize.Medium => 2,
        KeyboardShortcutSize.Large => 3,
        _ => 2
    };

    private double CornerRadius => Size switch
    {
        KeyboardShortcutSize.Small => 2,
        KeyboardShortcutSize.Medium => 3,
        KeyboardShortcutSize.Large => 4,
        _ => 3
    };

    private double Gap => Size switch
    {
        KeyboardShortcutSize.Small => 3,
        KeyboardShortcutSize.Medium => 4,
        KeyboardShortcutSize.Large => 5,
        _ => 4
    };

    private static readonly Typeface s_typeface = new("Segoe UI");

    private string[] GetKeyParts()
    {
        var shortcut = Shortcut;
        if (shortcut == _cachedShortcut) return _cachedParts;

        _cachedShortcut = shortcut;
        _cachedParts = string.IsNullOrWhiteSpace(shortcut)
            ? Array.Empty<string>()
            : shortcut.Split('+', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        return _cachedParts;
    }

    private static bool IsModifier(string key) => key.ToUpperInvariant() switch
    {
        "CTRL" or "CONTROL" or "ALT" or "SHIFT" or "META" or "CMD" or "COMMAND" or "WIN" => true,
        _ => false
    };

    private static string NormalizeKeyDisplay(string key) => key.ToUpperInvariant() switch
    {
        "CTRL" or "CONTROL" => "Ctrl",
        "ALT" => "Alt",
        "SHIFT" => "Shift",
        "META" or "CMD" or "COMMAND" or "WIN" => "Cmd",
        _ => key
    };

    private IBrush GetKeyBackground(string key)
    {
        if (KeyBackground != null && !IsModifier(key)) return KeyBackground;
        if (ModifierBackground != null && IsModifier(key)) return ModifierBackground;
        return IsModifier(key) ? s_defaultModifierBackground : s_defaultKeyBackground;
    }

    private IBrush GetKeyForeground() => KeyForeground ?? s_defaultKeyForeground;
    private IBrush GetKeyBorder() => KeyBorderBrush ?? s_defaultKeyBorder;

    public override void Render(DrawingContext context)
    {
        var parts = GetKeyParts();
        if (parts.Length == 0) return;

        var fontSize = FontSize;
        var padH = PaddingH;
        var padV = PaddingV;
        var gap = Gap;
        var cornerRadius = CornerRadius;
        var foreground = GetKeyForeground();
        var border = GetKeyBorder();
        var separatorBrush = foreground;

        var x = 0.0;

        for (int i = 0; i < parts.Length; i++)
        {
            var key = parts[i];
            var displayText = NormalizeKeyDisplay(key);
            var bg = GetKeyBackground(key);

            var ft = new FormattedText(
                displayText,
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                s_typeface,
                fontSize,
                foreground);

            var keyWidth = ft.Width + padH * 2;
            var keyHeight = ft.Height + padV * 2;
            var keyY = (Bounds.Height - keyHeight) / 2;

            // Draw key cap background with rounded rect
            var rect = new RoundedRect(new Rect(x, keyY, keyWidth, keyHeight), cornerRadius);
            context.DrawRectangle(bg, new Pen(border, 1), rect);

            // Draw key text centered in the cap
            var textX = x + (keyWidth - ft.Width) / 2;
            var textY = keyY + (keyHeight - ft.Height) / 2;
            context.DrawText(ft, new Point(textX, textY));

            x += keyWidth;

            // Draw separator between keys
            if (i < parts.Length - 1)
            {
                var sepFt = new FormattedText(
                    Separator,
                    System.Globalization.CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    s_typeface,
                    fontSize * 0.8,
                    separatorBrush);

                var sepY = (Bounds.Height - sepFt.Height) / 2;
                context.DrawText(sepFt, new Point(x + gap / 2, sepY));
                x += gap + sepFt.Width + gap / 2;
            }
        }
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        var parts = GetKeyParts();
        if (parts.Length == 0)
            return new Size(0, 0);

        var fontSize = FontSize;
        var padH = PaddingH;
        var padV = PaddingV;
        var gap = Gap;
        var foreground = GetKeyForeground();

        double totalWidth = 0;
        double maxHeight = 0;

        for (int i = 0; i < parts.Length; i++)
        {
            var displayText = NormalizeKeyDisplay(parts[i]);

            var ft = new FormattedText(
                displayText,
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                s_typeface,
                fontSize,
                foreground);

            totalWidth += ft.Width + padH * 2;
            maxHeight = Math.Max(maxHeight, ft.Height + padV * 2);

            if (i < parts.Length - 1)
            {
                var sepFt = new FormattedText(
                    Separator,
                    System.Globalization.CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    s_typeface,
                    fontSize * 0.8,
                    foreground);
                totalWidth += gap + sepFt.Width + gap / 2;
            }
        }

        return new Size(totalWidth, maxHeight);
    }
}
