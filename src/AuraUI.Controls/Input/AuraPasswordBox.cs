using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace AuraUI.Controls.Input;

/// <summary>
/// Password input control with built-in show/hide toggle, styled with AuraUI design tokens.
/// Uses the TextBox PasswordChar property for masking.
///
/// Template parts:
///   PART_RevealButton - Toggle button to show/hide the password
/// </summary>
public class AuraPasswordBox : TextBox
{
    private Button? _revealButton;

    /// <summary>
    /// Defines the <see cref="ShowToggle"/> property.
    /// </summary>
    public static readonly StyledProperty<bool> ShowToggleProperty =
        AvaloniaProperty.Register<AuraPasswordBox, bool>(
            nameof(ShowToggle),
            defaultValue: true);

    /// <summary>
    /// Defines the <see cref="IsRevealed"/> property.
    /// </summary>
    public static readonly StyledProperty<bool> IsRevealedProperty =
        AvaloniaProperty.Register<AuraPasswordBox, bool>(
            nameof(IsRevealed));

    /// <summary>
    /// Defines the <see cref="RevealIcon"/> property.
    /// </summary>
    public static readonly StyledProperty<object?> RevealIconProperty =
        AvaloniaProperty.Register<AuraPasswordBox, object?>(
            nameof(RevealIcon));

    /// <summary>
    /// Defines the <see cref="HideIcon"/> property.
    /// </summary>
    public static readonly StyledProperty<object?> HideIconProperty =
        AvaloniaProperty.Register<AuraPasswordBox, object?>(
            nameof(HideIcon));

    /// <summary>
    /// Defines the <see cref="DefaultPasswordChar"/> property.
    /// The character used to mask the password. Default is '•' (bullet).
    /// </summary>
    public static readonly StyledProperty<char> DefaultPasswordCharProperty =
        AvaloniaProperty.Register<AuraPasswordBox, char>(
            nameof(DefaultPasswordChar),
            defaultValue: '•');

    /// <summary>
    /// Gets or sets whether the reveal/hide toggle button is shown.
    /// </summary>
    public bool ShowToggle
    {
        get => GetValue(ShowToggleProperty);
        set => SetValue(ShowToggleProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the password is currently revealed (plain text).
    /// </summary>
    public bool IsRevealed
    {
        get => GetValue(IsRevealedProperty);
        set => SetValue(IsRevealedProperty, value);
    }

    /// <summary>
    /// Gets or sets the icon shown when the password is hidden (click to reveal).
    /// </summary>
    public object? RevealIcon
    {
        get => GetValue(RevealIconProperty);
        set => SetValue(RevealIconProperty, value);
    }

    /// <summary>
    /// Gets or sets the icon shown when the password is revealed (click to hide).
    /// </summary>
    public object? HideIcon
    {
        get => GetValue(HideIconProperty);
        set => SetValue(HideIconProperty, value);
    }

    /// <summary>
    /// Gets or sets the character used to mask the password.
    /// </summary>
    public char DefaultPasswordChar
    {
        get => GetValue(DefaultPasswordCharProperty);
        set => SetValue(DefaultPasswordCharProperty, value);
    }

    private char _originalPasswordChar;

    static AuraPasswordBox()
    {
        IsRevealedProperty.Changed.AddClassHandler<AuraPasswordBox>((x, e) => x.OnIsRevealedChanged(e));
        ShowToggleProperty.Changed.AddClassHandler<AuraPasswordBox>((x, _) => x.UpdateToggleVisibility());
    }

    protected override Type StyleKeyOverride => typeof(TextBox);

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        if (_revealButton is not null)
        {
            _revealButton.Click -= OnRevealButtonClick;
        }

        base.OnApplyTemplate(e);

        _revealButton = e.NameScope.Find<Button>("PART_RevealButton");

        if (_revealButton is not null)
        {
            _revealButton.Click += OnRevealButtonClick;
        }

        // Initialize password mode
        _originalPasswordChar = PasswordChar;
        if (_originalPasswordChar == '\0')
        {
            _originalPasswordChar = DefaultPasswordChar;
        }

        ApplyPasswordState();
        UpdateToggleVisibility();
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);

        // Ensure password masking is active on attach
        if (!IsRevealed)
        {
            PasswordChar = _originalPasswordChar != '\0' ? _originalPasswordChar : DefaultPasswordChar;
        }
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);

        if (_revealButton is not null)
        {
            _revealButton.Click -= OnRevealButtonClick;
        }
    }

    private void OnIsRevealedChanged(AvaloniaPropertyChangedEventArgs e)
    {
        if (e.NewValue is bool isRevealed)
        {
            PseudoClasses.Set("revealed", isRevealed);
            PseudoClasses.Set("hidden", !isRevealed);
            ApplyPasswordState();
        }
    }

    private void OnRevealButtonClick(object? sender, RoutedEventArgs e)
    {
        IsRevealed = !IsRevealed;
    }

    private void ApplyPasswordState()
    {
        if (IsRevealed)
        {
            // Show plain text by clearing the password character
            PasswordChar = '\0';
        }
        else
        {
            // Mask the password with the configured character
            PasswordChar = _originalPasswordChar != '\0' ? _originalPasswordChar : DefaultPasswordChar;
        }
    }

    private void UpdateToggleVisibility()
    {
        if (_revealButton is not null)
        {
            _revealButton.IsVisible = ShowToggle;
        }
    }
}
