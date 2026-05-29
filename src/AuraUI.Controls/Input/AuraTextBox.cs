using Avalonia;
using Avalonia.Automation;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Threading;

namespace AuraUI.Controls.Input;

/// <summary>
/// Defines the visual variant for the text box.
/// </summary>
public enum TextBoxVariant
{
    Outlined,
    Filled,
    Underlined
}

/// <summary>
/// Enhanced text input control with prefix/suffix, clear button, character count,
/// and AuraUI design token integration.
///
/// Supports variant classes: .filled, .outlined, .underlined
/// Supports size classes: .sm, .md, .lg
/// Pseudo-classes: :empty, :focus
///
/// Template parts:
///   PART_ClearButton      - Button to clear the text
///   PART_PrefixPresenter  - ContentPresenter for the prefix
///   PART_SuffixPresenter  - ContentPresenter for the suffix
///   PART_CharacterCount   - TextBlock displaying character count
/// </summary>
[TemplatePart("PART_ClearButton", typeof(Avalonia.Controls.Button))]
[TemplatePart("PART_PrefixPresenter", typeof(Avalonia.Controls.Presenters.ContentPresenter))]
[TemplatePart("PART_SuffixPresenter", typeof(Avalonia.Controls.Presenters.ContentPresenter))]
[TemplatePart("PART_CharacterCount", typeof(Avalonia.Controls.TextBlock))]
public class AuraTextBox : TextBox
{
    private Button? _clearButton;
    private ContentPresenter? _prefixPresenter;
    private ContentPresenter? _suffixPresenter;
    private TextBlock? _characterCountBlock;

    /// <summary>
    /// Defines the <see cref="Prefix"/> property.
    /// </summary>
    public static readonly StyledProperty<string?> PrefixProperty =
        AvaloniaProperty.Register<AuraTextBox, string?>(
            nameof(Prefix));

    /// <summary>
    /// Defines the <see cref="Suffix"/> property.
    /// </summary>
    public static readonly StyledProperty<string?> SuffixProperty =
        AvaloniaProperty.Register<AuraTextBox, string?>(
            nameof(Suffix));

    /// <summary>
    /// Defines the <see cref="IsClearable"/> property.
    /// </summary>
    public static readonly StyledProperty<bool> IsClearableProperty =
        AvaloniaProperty.Register<AuraTextBox, bool>(
            nameof(IsClearable));

    /// <summary>
    /// Defines the <see cref="ShowCount"/> property.
    /// </summary>
    public static readonly StyledProperty<bool> ShowCountProperty =
        AvaloniaProperty.Register<AuraTextBox, bool>(
            nameof(ShowCount));

    /// <summary>
    /// Defines the <see cref="Variant"/> property.
    /// </summary>
    public static readonly StyledProperty<TextBoxVariant> VariantProperty =
        AvaloniaProperty.Register<AuraTextBox, TextBoxVariant>(
            nameof(Variant),
            defaultValue: TextBoxVariant.Outlined);

    /// <summary>
    /// Defines the <see cref="MaxLengthDisplay"/> property.
    /// If set, overrides the MaxLength used in character count display.
    /// When 0, uses the base MaxLength property.
    /// </summary>
    public static readonly StyledProperty<int> MaxLengthDisplayProperty =
        AvaloniaProperty.Register<AuraTextBox, int>(
            nameof(MaxLengthDisplay),
            defaultValue: 0);

    /// <summary>
    /// Defines the <see cref="PlaceholderForeground"/> property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> PlaceholderForegroundProperty =
        AvaloniaProperty.Register<AuraTextBox, IBrush?>(nameof(PlaceholderForeground));

    /// <summary>
    /// Defines the <see cref="PlaceholderFontStyle"/> property.
    /// </summary>
    public static readonly StyledProperty<FontStyle> PlaceholderFontStyleProperty =
        AvaloniaProperty.Register<AuraTextBox, FontStyle>(
            nameof(PlaceholderFontStyle),
            defaultValue: FontStyle.Italic);

    /// <summary>
    /// Defines the <see cref="ClearButtonForeground"/> property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> ClearButtonForegroundProperty =
        AvaloniaProperty.Register<AuraTextBox, IBrush?>(nameof(ClearButtonForeground));

    /// <summary>
    /// Gets or sets the prefix text displayed before the input area.
    /// </summary>
    public string? Prefix
    {
        get => GetValue(PrefixProperty);
        set => SetValue(PrefixProperty, value);
    }

    /// <summary>
    /// Gets or sets the suffix text displayed after the input area.
    /// </summary>
    public string? Suffix
    {
        get => GetValue(SuffixProperty);
        set => SetValue(SuffixProperty, value);
    }

    /// <summary>
    /// Gets or sets whether a clear button is shown when text is not empty.
    /// </summary>
    public bool IsClearable
    {
        get => GetValue(IsClearableProperty);
        set => SetValue(IsClearableProperty, value);
    }

    /// <summary>
    /// Gets or sets whether to display a character count below the text box.
    /// Shows format: "current / maxLength".
    /// </summary>
    public bool ShowCount
    {
        get => GetValue(ShowCountProperty);
        set => SetValue(ShowCountProperty, value);
    }

    /// <summary>
    /// Gets or sets the visual variant of the text box.
    /// </summary>
    public TextBoxVariant Variant
    {
        get => GetValue(VariantProperty);
        set => SetValue(VariantProperty, value);
    }

    /// <summary>
    /// Gets or sets the maximum length for display purposes.
    /// When 0, the base MaxLength property is used.
    /// </summary>
    public int MaxLengthDisplay
    {
        get => GetValue(MaxLengthDisplayProperty);
        set => SetValue(MaxLengthDisplayProperty, value);
    }

    /// <summary>
    /// Gets or sets the foreground brush for the placeholder text.
    /// </summary>
    public IBrush? PlaceholderForeground
    {
        get => GetValue(PlaceholderForegroundProperty);
        set => SetValue(PlaceholderForegroundProperty, value);
    }

    /// <summary>
    /// Gets or sets the font style for the placeholder text.
    /// </summary>
    public FontStyle PlaceholderFontStyle
    {
        get => GetValue(PlaceholderFontStyleProperty);
        set => SetValue(PlaceholderFontStyleProperty, value);
    }

    /// <summary>
    /// Gets or sets the foreground brush for the clear button.
    /// </summary>
    public IBrush? ClearButtonForeground
    {
        get => GetValue(ClearButtonForegroundProperty);
        set => SetValue(ClearButtonForegroundProperty, value);
    }

    static AuraTextBox()
    {
        AffectsMeasure<AuraTextBox>(PrefixProperty, SuffixProperty, IsClearableProperty, ShowCountProperty, VariantProperty);

        TextProperty.Changed.AddClassHandler<AuraTextBox>((x, e) => x.OnTextChanged(e));
        IsClearableProperty.Changed.AddClassHandler<AuraTextBox>((x, _) => x.UpdateClearButtonVisibility());
        ShowCountProperty.Changed.AddClassHandler<AuraTextBox>((x, _) => x.UpdateCharacterCount());
        VariantProperty.Changed.AddClassHandler<AuraTextBox>((x, _) => x.UpdateVariantClasses());
        PrefixProperty.Changed.AddClassHandler<AuraTextBox>((x, _) => x.UpdateAffixVisibility());
        SuffixProperty.Changed.AddClassHandler<AuraTextBox>((x, _) => x.UpdateAffixVisibility());
    }

    protected override Type StyleKeyOverride => typeof(TextBox);

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        // Detach from old template parts
        if (_clearButton is not null)
        {
            _clearButton.Click -= OnClearButtonClick;
        }

        base.OnApplyTemplate(e);

        // Attach to new template parts
        _clearButton = e.NameScope.Find<Button>("PART_ClearButton");
        _prefixPresenter = e.NameScope.Find<ContentPresenter>("PART_PrefixPresenter");
        _suffixPresenter = e.NameScope.Find<ContentPresenter>("PART_SuffixPresenter");
        _characterCountBlock = e.NameScope.Find<TextBlock>("PART_CharacterCount");

        if (_clearButton is not null)
        {
            _clearButton.Click += OnClearButtonClick;
            _clearButton.SetValue(AutomationProperties.NameProperty, "Clear");
        }

        UpdateClearButtonVisibility();
        UpdateCharacterCount();
        UpdateVariantClasses();
        UpdateAffixVisibility();
        UpdateEmptyPseudoClass();
    }

    protected override void OnGotFocus(GotFocusEventArgs e)
    {
        base.OnGotFocus(e);
        PseudoClasses.Set("focus", true);
    }

    protected override void OnLostFocus(RoutedEventArgs e)
    {
        base.OnLostFocus(e);
        PseudoClasses.Set("focus", false);
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        UpdateEmptyPseudoClass();
        UpdateAutomationName();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        if (_clearButton is not null)
        {
            _clearButton.Click -= OnClearButtonClick;
        }
    }

    private void OnTextChanged(AvaloniaPropertyChangedEventArgs e)
    {
        UpdateClearButtonVisibility();
        UpdateCharacterCount();
        UpdateEmptyPseudoClass();
    }

    private void OnClearButtonClick(object? sender, RoutedEventArgs e)
    {
        Text = string.Empty;
        Focus();
    }

    protected virtual void UpdateClearButtonVisibility()
    {
        if (_clearButton is not null)
        {
            var hasText = !string.IsNullOrEmpty(Text);
            _clearButton.IsVisible = IsClearable && hasText;
        }
    }

    protected virtual void UpdateCharacterCount()
    {
        if (_characterCountBlock is not null)
        {
            if (ShowCount)
            {
                var currentLength = Text?.Length ?? 0;
                var maxLen = MaxLengthDisplay > 0 ? MaxLengthDisplay : MaxLength;

                if (maxLen > 0)
                {
                    _characterCountBlock.Text = $"{currentLength} / {maxLen}";
                }
                else
                {
                    _characterCountBlock.Text = currentLength.ToString();
                }

                _characterCountBlock.IsVisible = true;
            }
            else
            {
                _characterCountBlock.IsVisible = false;
            }
        }
    }

    private void UpdateEmptyPseudoClass()
    {
        var isEmpty = string.IsNullOrEmpty(Text);
        PseudoClasses.Set("empty", isEmpty);
    }

    protected virtual void UpdateVariantClasses()
    {
        Classes.Remove("filled");
        Classes.Remove("outlined");
        Classes.Remove("underlined");

        var variantClass = Variant switch
        {
            TextBoxVariant.Filled => "filled",
            TextBoxVariant.Outlined => "outlined",
            TextBoxVariant.Underlined => "underlined",
            _ => "outlined"
        };

        Classes.Add(variantClass);
    }

    private void UpdateAffixVisibility()
    {
        if (_prefixPresenter is not null)
        {
            _prefixPresenter.IsVisible = !string.IsNullOrEmpty(Prefix);
            _prefixPresenter.Content = Prefix;
        }

        if (_suffixPresenter is not null)
        {
            _suffixPresenter.IsVisible = !string.IsNullOrEmpty(Suffix);
            _suffixPresenter.Content = Suffix;
        }
    }

    private void UpdateAutomationName()
    {
        var watermark = Watermark;
        if (!string.IsNullOrEmpty(watermark))
        {
            SetValue(AutomationProperties.NameProperty, watermark);
        }
    }
}
