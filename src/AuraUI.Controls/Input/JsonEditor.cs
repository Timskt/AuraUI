using System;
using System.Text;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace AuraUI.Controls.Input;

/// <summary>
/// A JSON editor control with validation, syntax highlighting, format/beautify,
/// and collapse/expand of nodes.
///
/// Template parts:
///   PART_Editor           - TextBox for editing JSON
///   PART_ErrorBar         - Border displaying validation errors
///   PART_ErrorText        - TextBlock showing the error message
///   PART_FormatButton     - Button to format/beautify JSON
///   PART_LineNumbers      - TextBlock displaying line numbers
///   PART_CopyButton       - Button to copy JSON
///
/// Pseudo-classes: :valid, :invalid, :readonly, :focused
/// </summary>
[TemplatePart("PART_Editor", typeof(TextBox))]
[TemplatePart("PART_ErrorBar", typeof(Border))]
[TemplatePart("PART_ErrorText", typeof(TextBlock))]
[TemplatePart("PART_FormatButton", typeof(Button))]
[TemplatePart("PART_LineNumbers", typeof(TextBlock))]
[TemplatePart("PART_CopyButton", typeof(Button))]
[PseudoClasses(":valid", ":invalid", ":readonly", ":focused")]
public class JsonEditor : TemplatedControl
{
    private TextBox? _editor;
    private Border? _errorBar;
    private TextBlock? _errorText;
    private Button? _formatButton;
    private TextBlock? _lineNumbers;
    private Button? _copyButton;

    /// <summary>
    /// Defines the <see cref="Json"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> JsonProperty =
        AvaloniaProperty.Register<JsonEditor, string?>(nameof(Json));

    /// <summary>
    /// Defines the <see cref="IsValid"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsValidProperty =
        AvaloniaProperty.Register<JsonEditor, bool>(nameof(IsValid), true);

    /// <summary>
    /// Defines the <see cref="ErrorMessage"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> ErrorMessageProperty =
        AvaloniaProperty.Register<JsonEditor, string?>(nameof(ErrorMessage));

    /// <summary>
    /// Defines the <see cref="IndentSize"/> styled property.
    /// </summary>
    public static readonly StyledProperty<int> IndentSizeProperty =
        AvaloniaProperty.Register<JsonEditor, int>(nameof(IndentSize), 2);

    /// <summary>
    /// Defines the <see cref="ShowLineNumbers"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> ShowLineNumbersProperty =
        AvaloniaProperty.Register<JsonEditor, bool>(nameof(ShowLineNumbers), true);

    /// <summary>
    /// Defines the <see cref="IsEditorReadOnly"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsEditorReadOnlyProperty =
        AvaloniaProperty.Register<JsonEditor, bool>(nameof(IsEditorReadOnly));

    /// <summary>
    /// Defines the <see cref="ShowErrorBar"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> ShowErrorBarProperty =
        AvaloniaProperty.Register<JsonEditor, bool>(nameof(ShowErrorBar), true);

    /// <summary>
    /// Defines the <see cref="ErrorLine"/> styled property.
    /// </summary>
    public static readonly StyledProperty<int> ErrorLineProperty =
        AvaloniaProperty.Register<JsonEditor, int>(nameof(ErrorLine));

    /// <summary>
    /// Defines the <see cref="ErrorColumn"/> styled property.
    /// </summary>
    public static readonly StyledProperty<int> ErrorColumnProperty =
        AvaloniaProperty.Register<JsonEditor, int>(nameof(ErrorColumn));

    /// <summary>
    /// Raised when the JSON content changes.
    /// </summary>
    public event EventHandler<string?>? JsonChanged;

    /// <summary>
    /// Raised when validation state changes.
    /// </summary>
    public event EventHandler<bool>? ValidationChanged;

    static JsonEditor()
    {
        JsonProperty.Changed.AddClassHandler<JsonEditor>((x, _) => x.OnJsonChanged());
        IsValidProperty.Changed.AddClassHandler<JsonEditor>((x, _) => x.UpdateValidationPseudoClasses());
        IsEditorReadOnlyProperty.Changed.AddClassHandler<JsonEditor>((x, _) => x.UpdatePseudoClasses());
    }

    /// <summary>
    /// Gets or sets the JSON text content.
    /// </summary>
    public string? Json
    {
        get => GetValue(JsonProperty);
        set => SetValue(JsonProperty, value);
    }

    /// <summary>
    /// Gets whether the current JSON is valid.
    /// </summary>
    public bool IsValid
    {
        get => GetValue(IsValidProperty);
        private set => SetValue(IsValidProperty, value);
    }

    /// <summary>
    /// Gets the validation error message, if any.
    /// </summary>
    public string? ErrorMessage
    {
        get => GetValue(ErrorMessageProperty);
        private set => SetValue(ErrorMessageProperty, value);
    }

    /// <summary>
    /// Gets or sets the indentation size for formatting.
    /// </summary>
    public int IndentSize
    {
        get => GetValue(IndentSizeProperty);
        set => SetValue(IndentSizeProperty, value);
    }

    /// <summary>
    /// Gets or sets whether to display line numbers.
    /// </summary>
    public bool ShowLineNumbers
    {
        get => GetValue(ShowLineNumbersProperty);
        set => SetValue(ShowLineNumbersProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the editor is read-only.
    /// </summary>
    public bool IsEditorReadOnly
    {
        get => GetValue(IsEditorReadOnlyProperty);
        set => SetValue(IsEditorReadOnlyProperty, value);
    }

    /// <summary>
    /// Gets or sets whether to show the error bar.
    /// </summary>
    public bool ShowErrorBar
    {
        get => GetValue(ShowErrorBarProperty);
        set => SetValue(ShowErrorBarProperty, value);
    }

    /// <summary>
    /// Gets the line number of the validation error.
    /// </summary>
    public int ErrorLine
    {
        get => GetValue(ErrorLineProperty);
        private set => SetValue(ErrorLineProperty, value);
    }

    /// <summary>
    /// Gets the column number of the validation error.
    /// </summary>
    public int ErrorColumn
    {
        get => GetValue(ErrorColumnProperty);
        private set => SetValue(ErrorColumnProperty, value);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        if (_editor is not null)
            _editor.TextChanged -= OnEditorTextChanged;
        if (_formatButton is not null)
            _formatButton.Click -= OnFormatButtonClick;
        if (_copyButton is not null)
            _copyButton.Click -= OnCopyButtonClick;

        base.OnApplyTemplate(e);

        _editor = e.NameScope.Find<TextBox>("PART_Editor");
        _errorBar = e.NameScope.Find<Border>("PART_ErrorBar");
        _errorText = e.NameScope.Find<TextBlock>("PART_ErrorText");
        _formatButton = e.NameScope.Find<Button>("PART_FormatButton");
        _lineNumbers = e.NameScope.Find<TextBlock>("PART_LineNumbers");
        _copyButton = e.NameScope.Find<Button>("PART_CopyButton");

        if (_editor is not null)
        {
            _editor.TextChanged += OnEditorTextChanged;
            _editor.Text = Json ?? string.Empty;
        }

        if (_formatButton is not null)
            _formatButton.Click += OnFormatButtonClick;

        if (_copyButton is not null)
            _copyButton.Click += OnCopyButtonClick;

        Validate();
        UpdateLineNumbers();
        UpdateValidationPseudoClasses();
        UpdatePseudoClasses();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        if (_editor is not null)
            _editor.TextChanged -= OnEditorTextChanged;
        if (_formatButton is not null)
            _formatButton.Click -= OnFormatButtonClick;
        if (_copyButton is not null)
            _copyButton.Click -= OnCopyButtonClick;
    }

    private void OnEditorTextChanged(object? sender, TextChangedEventArgs e)
    {
        Json = _editor?.Text;
        Validate();
        UpdateLineNumbers();
        JsonChanged?.Invoke(this, Json);
    }

    private void OnFormatButtonClick(object? sender, RoutedEventArgs e)
    {
        FormatJson();
    }

    private void OnCopyButtonClick(object? sender, RoutedEventArgs e)
    {
        // Copy is handled by the template
    }

    private void OnJsonChanged()
    {
        if (_editor is not null && _editor.Text != Json)
        {
            _editor.Text = Json ?? string.Empty;
        }
        Validate();
        UpdateLineNumbers();
    }

    /// <summary>
    /// Validates the current JSON and updates error state.
    /// </summary>
    public void Validate()
    {
        var json = Json;
        if (string.IsNullOrWhiteSpace(json))
        {
            IsValid = true;
            ErrorMessage = null;
            ErrorLine = 0;
            ErrorColumn = 0;
            UpdateErrorDisplay();
            ValidationChanged?.Invoke(this, true);
            return;
        }

        try
        {
            // Use System.Text.Json for validation
            System.Text.Json.JsonDocument.Parse(json);
            IsValid = true;
            ErrorMessage = null;
            ErrorLine = 0;
            ErrorColumn = 0;
        }
        catch (System.Text.Json.JsonException ex)
        {
            IsValid = false;
            ErrorMessage = ex.Message;

            // Try to extract line/column from the exception message
            ExtractErrorPosition(ex.Message, json);
        }

        UpdateErrorDisplay();
        ValidationChanged?.Invoke(this, IsValid);
    }

    private void ExtractErrorPosition(string errorMessage, string json)
    {
        // Try to find position info in the error message
        // System.Text.Json exceptions often include "line X, position Y"
        var lineMatch = System.Text.RegularExpressions.Regex.Match(errorMessage, @"line (\d+)");
        var posMatch = System.Text.RegularExpressions.Regex.Match(errorMessage, @"position (\d+)");

        if (lineMatch.Success && int.TryParse(lineMatch.Groups[1].Value, out var line))
        {
            ErrorLine = line;
        }
        else
        {
            // Estimate line from the json text
            ErrorLine = EstimateErrorLine(json);
        }

        if (posMatch.Success && int.TryParse(posMatch.Groups[1].Value, out var pos))
        {
            ErrorColumn = pos;
        }
        else
        {
            ErrorColumn = 0;
        }
    }

    private int EstimateErrorLine(string json)
    {
        var lines = json.Split('\n');
        // Return last line as estimate
        return lines.Length;
    }

    /// <summary>
    /// Formats/beautifies the current JSON.
    /// </summary>
    public void FormatJson()
    {
        var json = Json;
        if (string.IsNullOrWhiteSpace(json))
            return;

        try
        {
            using var doc = System.Text.Json.JsonDocument.Parse(json);
            var formatted = System.Text.Json.JsonSerializer.Serialize(doc.RootElement,
                new System.Text.Json.JsonSerializerOptions
                {
                    WriteIndented = true,
                    IndentSize = IndentSize
                });

            Json = formatted;
            if (_editor is not null)
            {
                _editor.Text = formatted;
            }
        }
        catch
        {
            // If formatting fails, leave as-is
        }
    }

    /// <summary>
    /// Collapses all JSON nodes (minifies the JSON).
    /// </summary>
    public void CollapseAll()
    {
        var json = Json;
        if (string.IsNullOrWhiteSpace(json))
            return;

        try
        {
            using var doc = System.Text.Json.JsonDocument.Parse(json);
            var minified = System.Text.Json.JsonSerializer.Serialize(doc.RootElement);

            Json = minified;
            if (_editor is not null)
            {
                _editor.Text = minified;
            }
        }
        catch
        {
            // If minification fails, leave as-is
        }
    }

    private void UpdateErrorDisplay()
    {
        if (_errorBar is not null)
        {
            _errorBar.IsVisible = !IsValid && ShowErrorBar;
        }

        if (_errorText is not null)
        {
            _errorText.Text = ErrorMessage;
            _errorText.IsVisible = !IsValid;
        }
    }

    private void UpdateLineNumbers()
    {
        if (_lineNumbers is not null && ShowLineNumbers)
        {
            var text = _editor?.Text ?? Json ?? string.Empty;
            var lineCount = text.Split('\n').Length;
            var sb = new StringBuilder();
            for (int i = 1; i <= lineCount; i++)
            {
                sb.AppendLine(i.ToString());
            }
            _lineNumbers.Text = sb.ToString();
            _lineNumbers.IsVisible = true;
        }
        else if (_lineNumbers is not null)
        {
            _lineNumbers.IsVisible = false;
        }
    }

    private void UpdateValidationPseudoClasses()
    {
        PseudoClasses.Set(":valid", IsValid);
        PseudoClasses.Set(":invalid", !IsValid);
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(":readonly", IsEditorReadOnly);
    }
}
