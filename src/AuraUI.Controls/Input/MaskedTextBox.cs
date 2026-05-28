using System.Text;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;

namespace AuraUI.Controls.Input;

/// <summary>
/// Text box with input masking support using pattern-based masks.
///
/// Mask pattern characters:
///   9  - Digit (0-9)
///   a  - Letter (A-Z, a-z)
///   *  - Any character (digit, letter, or symbol)
///   ?  - Optional: makes the preceding mask character optional
///   Any other character is treated as a literal and inserted automatically.
///
/// Examples:
///   (999) 999-9999   - US phone number: (123) 456-7890
///   9999-9999-9999-9999 - Credit card: 1234-5678-9012-3456
///   aa/99/9999        - Date: AB/12/3456
///   AAA-9999          - License plate: ABC-1234
/// </summary>
public class MaskedTextBox : TextBox
{
    private bool _isUpdatingText;
    private int _selectionStartBackup;
    private int _selectionEndBackup;

    /// <summary>
    /// Defines the <see cref="Mask"/> property.
    /// </summary>
    public static readonly StyledProperty<string?> MaskProperty =
        AvaloniaProperty.Register<MaskedTextBox, string?>(
            nameof(Mask));

    /// <summary>
    /// Defines the <see cref="PromptChar"/> property.
    /// </summary>
    public static readonly StyledProperty<char> PromptCharProperty =
        AvaloniaProperty.Register<MaskedTextBox, char>(
            nameof(PromptChar),
            defaultValue: '_');

    /// <summary>
    /// Defines the <see cref="AllowPromptAsInput"/> property.
    /// When true, the prompt character can be typed as valid input.
    /// </summary>
    public static readonly StyledProperty<bool> AllowPromptAsInputProperty =
        AvaloniaProperty.Register<MaskedTextBox, bool>(
            nameof(AllowPromptAsInput));

    /// <summary>
    /// Defines the <see cref="SkipLiterals"/> property.
    /// When true, cursor automatically skips over literal characters.
    /// </summary>
    public static readonly StyledProperty<bool> SkipLiteralsProperty =
        AvaloniaProperty.Register<MaskedTextBox, bool>(
            nameof(SkipLiterals),
            defaultValue: true);

    /// <summary>
    /// Defines the <see cref="RejectInputOnFirstFailure"/> property.
    /// When true, input is rejected immediately on first invalid character.
    /// When false, the system tries to find the next valid position.
    /// </summary>
    public static readonly StyledProperty<bool> RejectInputOnFirstFailureProperty =
        AvaloniaProperty.Register<MaskedTextBox, bool>(
            nameof(RejectInputOnFirstFailure));

    /// <summary>
    /// Defines the <see cref="MaskedText"/> read-only property.
    /// Returns the formatted masked text including literals.
    /// </summary>
    public static readonly DirectProperty<MaskedTextBox, string?> MaskedTextProperty =
        AvaloniaProperty.RegisterDirect<MaskedTextBox, string?>(
            nameof(MaskedText),
            o => o.MaskedText);

    /// <summary>
    /// Defines the <see cref="RawText"/> read-only property.
    /// Returns only the user-typed characters without literals or prompt chars.
    /// </summary>
    public static readonly DirectProperty<MaskedTextBox, string?> RawTextProperty =
        AvaloniaProperty.RegisterDirect<MaskedTextBox, string?>(
            nameof(RawText),
            o => o.RawText);

    /// <summary>
    /// Defines the <see cref="IsMaskFull"/> read-only property.
    /// Returns true when all required mask positions are filled.
    /// </summary>
    public static readonly DirectProperty<MaskedTextBox, bool> IsMaskFullProperty =
        AvaloniaProperty.RegisterDirect<MaskedTextBox, bool>(
            nameof(IsMaskFull),
            o => o.IsMaskFull);

    /// <summary>
    /// Gets or sets the input mask pattern.
    /// </summary>
    public string? Mask
    {
        get => GetValue(MaskProperty);
        set => SetValue(MaskProperty, value);
    }

    /// <summary>
    /// Gets or sets the prompt character displayed for unfilled positions.
    /// Default is '_'.
    /// </summary>
    public char PromptChar
    {
        get => GetValue(PromptCharProperty);
        set => SetValue(PromptCharProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the prompt character can be typed as valid input.
    /// </summary>
    public bool AllowPromptAsInput
    {
        get => GetValue(AllowPromptAsInputProperty);
        set => SetValue(AllowPromptAsInputProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the cursor automatically skips over literal characters.
    /// </summary>
    public bool SkipLiterals
    {
        get => GetValue(SkipLiteralsProperty);
        set => SetValue(SkipLiteralsProperty, value);
    }

    /// <summary>
    /// Gets or sets whether input is rejected immediately on first failure.
    /// </summary>
    public bool RejectInputOnFirstFailure
    {
        get => GetValue(RejectInputOnFirstFailureProperty);
        set => SetValue(RejectInputOnFirstFailureProperty, value);
    }

    private string? _maskedText;

    /// <summary>
    /// Gets the formatted masked text including literals and prompt characters.
    /// </summary>
    public string? MaskedText
    {
        get => _maskedText;
        private set => SetAndRaise(MaskedTextProperty, ref _maskedText, value);
    }

    private string? _rawText;

    /// <summary>
    /// Gets only the user-typed characters (no literals or prompt characters).
    /// </summary>
    public string? RawText
    {
        get => _rawText;
        private set => SetAndRaise(RawTextProperty, ref _rawText, value);
    }

    private bool _isMaskFull;

    /// <summary>
    /// Gets whether all required mask positions are filled.
    /// </summary>
    public bool IsMaskFull
    {
        get => _isMaskFull;
        private set => SetAndRaise(IsMaskFullProperty, ref _isMaskFull, value);
    }

    /// <summary>
    /// Raised when the raw text value changes.
    /// </summary>
    public event EventHandler<string?>? RawTextChanged;

    static MaskedTextBox()
    {
        MaskProperty.Changed.AddClassHandler<MaskedTextBox>((x, _) => x.OnMaskChanged());
        PromptCharProperty.Changed.AddClassHandler<MaskedTextBox>((x, _) => x.OnMaskChanged());
        TextProperty.Changed.AddClassHandler<MaskedTextBox>((x, e) => x.OnTextPropertyChanged(e));
    }

    protected override Type StyleKeyOverride => typeof(TextBox);

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        OnMaskChanged();
    }

    protected override void OnTextInput(TextInputEventArgs e)
    {
        if (_isUpdatingText || string.IsNullOrEmpty(Mask))
        {
            base.OnTextInput(e);
            return;
        }

        e.Handled = true;

        var inputText = e.Text;
        if (string.IsNullOrEmpty(inputText))
            return;

        var caretPos = CaretIndex;
        var currentText = Text ?? BuildEmptyMask();

        _selectionStartBackup = SelectionStart;
        _selectionEndBackup = SelectionEnd;

        // Process each input character
        foreach (var ch in inputText)
        {
            var result = TryInsertCharacter(currentText, ch, ref caretPos);
            if (result is not null)
            {
                currentText = result;
            }
            else if (RejectInputOnFirstFailure)
            {
                break;
            }
        }

        _isUpdatingText = true;
        try
        {
            Text = currentText;
            CaretIndex = Math.Min(caretPos, currentText?.Length ?? 0);
            UpdateDerivedProperties();
        }
        finally
        {
            _isUpdatingText = false;
        }
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        if (_isUpdatingText || string.IsNullOrEmpty(Mask))
        {
            base.OnKeyDown(e);
            return;
        }

        // Handle backspace and delete specially to respect mask positions
        if (e.Key == Key.Back)
        {
            e.Handled = true;
            HandleBackspace();
            return;
        }

        if (e.Key == Key.Delete)
        {
            e.Handled = true;
            HandleDelete();
            return;
        }

        base.OnKeyDown(e);
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        OnMaskChanged();
    }

    private void OnMaskChanged()
    {
        if (_isUpdatingText)
            return;

        if (string.IsNullOrEmpty(Mask))
        {
            MaskedText = null;
            RawText = null;
            IsMaskFull = false;
            return;
        }

        _isUpdatingText = true;
        try
        {
            var emptyMask = BuildEmptyMask();
            Text = emptyMask;
            CaretIndex = FindFirstInputPosition(emptyMask);
            UpdateDerivedProperties();
        }
        finally
        {
            _isUpdatingText = false;
        }
    }

    private void OnTextPropertyChanged(AvaloniaPropertyChangedEventArgs e)
    {
        if (_isUpdatingText || string.IsNullOrEmpty(Mask))
            return;

        UpdateDerivedProperties();
    }

    private string BuildEmptyMask()
    {
        if (string.IsNullOrEmpty(Mask))
            return string.Empty;

        var sb = new StringBuilder(Mask.Length);
        var promptChar = PromptChar;

        foreach (var maskChar in Mask)
        {
            if (IsMaskSpecifier(maskChar))
            {
                sb.Append(promptChar);
            }
            else
            {
                // Literal character
                sb.Append(maskChar);
            }
        }

        return sb.ToString();
    }

    private string? TryInsertCharacter(string? currentText, char inputChar, ref int caretPos)
    {
        if (currentText is null || string.IsNullOrEmpty(Mask))
            return null;

        // Find the next input position at or after caret
        var mask = Mask;
        var pos = caretPos;

        // Skip to next input position
        while (pos < mask.Length && pos < currentText.Length && !IsMaskSpecifier(mask[pos]))
        {
            if (SkipLiterals)
            {
                pos++;
            }
            else
            {
                break;
            }
        }

        if (pos >= mask.Length || pos >= currentText.Length)
            return null;

        var maskChar = mask[pos];

        if (!IsMaskSpecifier(maskChar))
        {
            // At a literal position - skip if SkipLiterals is on
            if (SkipLiterals)
            {
                caretPos = pos + 1;
                return TryInsertCharacter(currentText, inputChar, ref caretPos);
            }
            return null;
        }

        // Validate input against the mask specifier
        if (!IsValidForMask(maskChar, inputChar))
        {
            return null;
        }

        // Insert the character
        var sb = new StringBuilder(currentText);
        sb[pos] = inputChar;
        caretPos = pos + 1;

        return sb.ToString();
    }

    private void HandleBackspace()
    {
        var caretPos = CaretIndex;
        var currentText = Text;
        if (currentText is null || string.IsNullOrEmpty(Mask))
            return;

        if (caretPos <= 0)
            return;

        // Move caret back, skipping literals
        var newPos = caretPos - 1;
        while (newPos >= 0 && !IsMaskSpecifier(Mask[newPos]))
        {
            if (SkipLiterals)
                newPos--;
            else
                break;
        }

        if (newPos < 0)
            return;

        // Clear the character at this position (replace with prompt)
        var sb = new StringBuilder(currentText);
        if (newPos < sb.Length && newPos < Mask.Length && IsMaskSpecifier(Mask[newPos]))
        {
            sb[newPos] = PromptChar;
        }

        _isUpdatingText = true;
        try
        {
            Text = sb.ToString();
            CaretIndex = newPos;
            UpdateDerivedProperties();
        }
        finally
        {
            _isUpdatingText = false;
        }
    }

    private void HandleDelete()
    {
        var caretPos = CaretIndex;
        var currentText = Text;
        if (currentText is null || string.IsNullOrEmpty(Mask))
            return;

        var pos = caretPos;

        // Skip literals if needed
        while (pos < Mask.Length && !IsMaskSpecifier(Mask[pos]))
        {
            if (SkipLiterals)
                pos++;
            else
                break;
        }

        if (pos >= Mask.Length || pos >= currentText.Length)
            return;

        // Clear the character at this position
        if (IsMaskSpecifier(Mask[pos]))
        {
            var sb = new StringBuilder(currentText);
            sb[pos] = PromptChar;

            _isUpdatingText = true;
            try
            {
                Text = sb.ToString();
                CaretIndex = pos;
                UpdateDerivedProperties();
            }
            finally
            {
                _isUpdatingText = false;
            }
        }
    }

    private int FindFirstInputPosition(string text)
    {
        if (string.IsNullOrEmpty(Mask))
            return 0;

        for (var i = 0; i < Mask.Length; i++)
        {
            if (IsMaskSpecifier(Mask[i]))
                return i;
        }

        return 0;
    }

    private bool IsMaskSpecifier(char c)
    {
        return c == '9' || c == 'a' || c == '*';
    }

    private bool IsValidForMask(char maskChar, char inputChar)
    {
        return maskChar switch
        {
            '9' => char.IsDigit(inputChar),
            'a' => char.IsLetter(inputChar),
            '*' => true,
            _ => false
        };
    }

    private void UpdateDerivedProperties()
    {
        var text = Text;
        var mask = Mask;

        MaskedText = text;

        if (text is null || string.IsNullOrEmpty(mask))
        {
            RawText = null;
            IsMaskFull = false;
            return;
        }

        // Extract only user-typed characters (not literals or prompts)
        var sb = new StringBuilder();
        var allFilled = true;

        for (var i = 0; i < Math.Min(text.Length, mask.Length); i++)
        {
            if (IsMaskSpecifier(mask[i]))
            {
                var ch = text[i];
                if (ch != PromptChar)
                {
                    sb.Append(ch);
                }
                else
                {
                    allFilled = false;
                }
            }
        }

        var raw = sb.ToString();
        RawText = raw;
        IsMaskFull = allFilled;

        RawTextChanged?.Invoke(this, raw);
    }
}
