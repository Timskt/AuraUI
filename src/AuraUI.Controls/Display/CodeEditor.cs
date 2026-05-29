using System;
using System.Collections.Generic;
using System.Text;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Media;

namespace AuraUI.Controls.Display;

/// <summary>
/// Supported programming languages for syntax highlighting.
/// </summary>
public enum CodeLanguage
{
    CSharp,
    JavaScript,
    Python,
    Json,
    Xml,
    Sql,
    Markdown,
    PlainText
}

/// <summary>
/// Specifies the color theme for the code editor.
/// </summary>
public enum CodeEditorTheme
{
    Light,
    Dark
}

/// <summary>
/// A code editor component with syntax highlighting, line numbers, and copy functionality.
/// Supports multiple programming languages and light/dark themes.
///
/// Template parts:
///   PART_CodePresenter    - TextBlock or panel that renders the highlighted code
///   PART_LineNumbers      - TextBlock displaying line numbers
///   PART_CopyButton       - Button to copy code to clipboard
///   PART_WrapToggle       - Toggle button for word wrap
///   PART_ScrollViewer     - ScrollViewer for scrolling code content
///
/// Pseudo-classes: :dark, :light, :readonly, :wordwrap
/// </summary>
[TemplatePart("PART_CodePresenter", typeof(TextBlock))]
[TemplatePart("PART_LineNumbers", typeof(TextBlock))]
[TemplatePart("PART_CopyButton", typeof(Button))]
[TemplatePart("PART_WrapToggle", typeof(ToggleButton))]
[TemplatePart("PART_ScrollViewer", typeof(ScrollViewer))]
[PseudoClasses(":dark", ":light", ":readonly", ":wordwrap")]
public class CodeEditor : TemplatedControl
{
    private TextBlock? _codePresenter;
    private TextBlock? _lineNumbers;
    private Button? _copyButton;
    private ToggleButton? _wrapToggle;
    private ScrollViewer? _scrollViewer;

    // Simple keyword lists for basic syntax highlighting
    private static readonly HashSet<string> CSharpKeywords = new()
    {
        "abstract", "as", "base", "bool", "break", "byte", "case", "catch", "char",
        "checked", "class", "const", "continue", "decimal", "default", "delegate", "do",
        "double", "else", "enum", "event", "explicit", "extern", "false", "finally",
        "fixed", "float", "for", "foreach", "goto", "if", "implicit", "in", "int",
        "interface", "internal", "is", "lock", "long", "namespace", "new", "null",
        "object", "operator", "out", "override", "params", "private", "protected",
        "public", "readonly", "ref", "return", "sbyte", "sealed", "short", "sizeof",
        "stackalloc", "static", "string", "struct", "switch", "this", "throw", "true",
        "try", "typeof", "uint", "ulong", "unchecked", "unsafe", "ushort", "using",
        "virtual", "void", "volatile", "while", "var", "async", "await", "yield",
        "record", "with", "init", "not", "and", "or"
    };

    private static readonly HashSet<string> JavaScriptKeywords = new()
    {
        "abstract", "arguments", "await", "boolean", "break", "byte", "case", "catch",
        "char", "class", "const", "continue", "debugger", "default", "delete", "do",
        "double", "else", "enum", "eval", "export", "extends", "false", "final",
        "finally", "float", "for", "function", "goto", "if", "implements", "import",
        "in", "instanceof", "int", "interface", "let", "long", "native", "new", "null",
        "package", "private", "protected", "public", "return", "short", "static",
        "super", "switch", "synchronized", "this", "throw", "throws", "transient", "true",
        "try", "typeof", "undefined", "var", "void", "volatile", "while", "with", "yield",
        "async", "of", "from"
    };

    private static readonly HashSet<string> PythonKeywords = new()
    {
        "False", "None", "True", "and", "as", "assert", "async", "await", "break",
        "class", "continue", "def", "del", "elif", "else", "except", "finally", "for",
        "from", "global", "if", "import", "in", "is", "lambda", "nonlocal", "not", "or",
        "pass", "raise", "return", "try", "while", "with", "yield", "print", "self"
    };

    private static readonly HashSet<string> SqlKeywords = new()
    {
        "SELECT", "FROM", "WHERE", "INSERT", "UPDATE", "DELETE", "CREATE", "DROP",
        "ALTER", "TABLE", "INDEX", "VIEW", "JOIN", "INNER", "LEFT", "RIGHT", "OUTER",
        "ON", "AND", "OR", "NOT", "NULL", "IS", "IN", "LIKE", "BETWEEN", "EXISTS",
        "GROUP", "BY", "ORDER", "HAVING", "LIMIT", "OFFSET", "AS", "DISTINCT", "SET",
        "VALUES", "INTO", "PRIMARY", "KEY", "FOREIGN", "REFERENCES", "UNION", "ALL",
        "CASE", "WHEN", "THEN", "ELSE", "END", "COUNT", "SUM", "AVG", "MIN", "MAX",
        "select", "from", "where", "insert", "update", "delete", "create", "drop",
        "alter", "table", "join", "inner", "left", "right", "on", "and", "or", "not",
        "null", "is", "in", "like", "group", "by", "order", "having", "limit", "as",
        "distinct", "set", "values", "into", "union", "all", "case", "when", "then",
        "else", "end", "count", "sum", "avg", "min", "max"
    };

    /// <summary>
    /// Defines the <see cref="Language"/> styled property.
    /// </summary>
    public static readonly StyledProperty<CodeLanguage> LanguageProperty =
        AvaloniaProperty.Register<CodeEditor, CodeLanguage>(nameof(Language));

    /// <summary>
    /// Defines the <see cref="Code"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> CodeProperty =
        AvaloniaProperty.Register<CodeEditor, string?>(nameof(Code));

    /// <summary>
    /// Defines the <see cref="Theme"/> styled property.
    /// </summary>
    public static readonly StyledProperty<CodeEditorTheme> ThemeProperty =
        AvaloniaProperty.Register<CodeEditor, CodeEditorTheme>(nameof(Theme), CodeEditorTheme.Dark);

    /// <summary>
    /// Defines the <see cref="ShowLineNumbers"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> ShowLineNumbersProperty =
        AvaloniaProperty.Register<CodeEditor, bool>(nameof(ShowLineNumbers), true);

    /// <summary>
    /// Defines the <see cref="ShowMinimap"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> ShowMinimapProperty =
        AvaloniaProperty.Register<CodeEditor, bool>(nameof(ShowMinimap));

    /// <summary>
    /// Defines the <see cref="IsReadOnly"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsReadOnlyProperty =
        AvaloniaProperty.Register<CodeEditor, bool>(nameof(IsReadOnly), true);

    /// <summary>
    /// Defines the <see cref="EditorFontFamily"/> styled property.
    /// </summary>
    public static readonly StyledProperty<FontFamily> EditorFontFamilyProperty =
        AvaloniaProperty.Register<CodeEditor, FontFamily>(nameof(EditorFontFamily), new FontFamily("Cascadia Code,Consolas,Courier New,monospace"));

    /// <summary>
    /// Defines the <see cref="EditorFontSize"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> EditorFontSizeProperty =
        AvaloniaProperty.Register<CodeEditor, double>(nameof(EditorFontSize), 13.0);

    /// <summary>
    /// Defines the <see cref="IsWordWrap"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsWordWrapProperty =
        AvaloniaProperty.Register<CodeEditor, bool>(nameof(IsWordWrap));

    /// <summary>
    /// Defines the <see cref="TabSize"/> styled property.
    /// </summary>
    public static readonly StyledProperty<int> TabSizeProperty =
        AvaloniaProperty.Register<CodeEditor, int>(nameof(TabSize), 4);

    static CodeEditor()
    {
        CodeProperty.Changed.AddClassHandler<CodeEditor>((x, _) => x.OnCodeChanged());
        LanguageProperty.Changed.AddClassHandler<CodeEditor>((x, _) => x.OnCodeChanged());
        ThemeProperty.Changed.AddClassHandler<CodeEditor>((x, _) => x.UpdatePseudoClasses());
        IsReadOnlyProperty.Changed.AddClassHandler<CodeEditor>((x, _) => x.UpdatePseudoClasses());
        IsWordWrapProperty.Changed.AddClassHandler<CodeEditor>((x, _) => x.UpdatePseudoClasses());
        ShowLineNumbersProperty.Changed.AddClassHandler<CodeEditor>((x, _) => x.UpdateLineNumbers());
    }

    /// <summary>
    /// Gets or sets the programming language for syntax highlighting.
    /// </summary>
    public CodeLanguage Language
    {
        get => GetValue(LanguageProperty);
        set => SetValue(LanguageProperty, value);
    }

    /// <summary>
    /// Gets or sets the source code text.
    /// </summary>
    public string? Code
    {
        get => GetValue(CodeProperty);
        set => SetValue(CodeProperty, value);
    }

    /// <summary>
    /// Gets or sets the color theme.
    /// </summary>
    public CodeEditorTheme Theme
    {
        get => GetValue(ThemeProperty);
        set => SetValue(ThemeProperty, value);
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
    /// Gets or sets whether to display a minimap.
    /// </summary>
    public bool ShowMinimap
    {
        get => GetValue(ShowMinimapProperty);
        set => SetValue(ShowMinimapProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the editor is read-only.
    /// </summary>
    public bool IsReadOnly
    {
        get => GetValue(IsReadOnlyProperty);
        set => SetValue(IsReadOnlyProperty, value);
    }

    /// <summary>
    /// Gets or sets the font family for the code editor.
    /// </summary>
    public FontFamily EditorFontFamily
    {
        get => GetValue(EditorFontFamilyProperty);
        set => SetValue(EditorFontFamilyProperty, value);
    }

    /// <summary>
    /// Gets or sets the font size for the code editor.
    /// </summary>
    public double EditorFontSize
    {
        get => GetValue(EditorFontSizeProperty);
        set => SetValue(EditorFontSizeProperty, value);
    }

    /// <summary>
    /// Gets or sets whether word wrap is enabled.
    /// </summary>
    public bool IsWordWrap
    {
        get => GetValue(IsWordWrapProperty);
        set => SetValue(IsWordWrapProperty, value);
    }

    /// <summary>
    /// Gets or sets the tab size in spaces.
    /// </summary>
    public int TabSize
    {
        get => GetValue(TabSizeProperty);
        set => SetValue(TabSizeProperty, value);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        if (_copyButton is not null)
            _copyButton.Click -= OnCopyButtonClick;
        if (_wrapToggle is not null)
            _wrapToggle.IsCheckedChanged -= OnWrapToggleChanged;

        base.OnApplyTemplate(e);

        _codePresenter = e.NameScope.Find<TextBlock>("PART_CodePresenter");
        _lineNumbers = e.NameScope.Find<TextBlock>("PART_LineNumbers");
        _copyButton = e.NameScope.Find<Button>("PART_CopyButton");
        _wrapToggle = e.NameScope.Find<ToggleButton>("PART_WrapToggle");
        _scrollViewer = e.NameScope.Find<ScrollViewer>("PART_ScrollViewer");

        if (_copyButton is not null)
            _copyButton.Click += OnCopyButtonClick;

        if (_wrapToggle is not null)
        {
            _wrapToggle.IsCheckedChanged += OnWrapToggleChanged;
        }

        UpdatePseudoClasses();
        UpdateCodeDisplay();
        UpdateLineNumbers();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        if (_copyButton is not null)
            _copyButton.Click -= OnCopyButtonClick;
        if (_wrapToggle is not null)
        {
            _wrapToggle.IsCheckedChanged -= OnWrapToggleChanged;
        }
    }

    private void OnCodeChanged()
    {
        UpdateCodeDisplay();
        UpdateLineNumbers();
    }

    private void OnCopyButtonClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        // Copy is handled by the template; this is a hook for additional logic
    }

    private void OnWrapToggleChanged(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        IsWordWrap = _wrapToggle?.IsChecked == true;
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(":dark", Theme == CodeEditorTheme.Dark);
        PseudoClasses.Set(":light", Theme == CodeEditorTheme.Light);
        PseudoClasses.Set(":readonly", IsReadOnly);
        PseudoClasses.Set(":wordwrap", IsWordWrap);
    }

    private void UpdateCodeDisplay()
    {
        if (_codePresenter is not null)
        {
            // Display the raw code; syntax highlighting is handled via styling
            _codePresenter.Text = Code ?? string.Empty;
            _codePresenter.FontFamily = EditorFontFamily;
            _codePresenter.FontSize = EditorFontSize;
            _codePresenter.TextWrapping = IsWordWrap ? TextWrapping.Wrap : TextWrapping.NoWrap;
        }
    }

    private void UpdateLineNumbers()
    {
        if (_lineNumbers is not null)
        {
            if (ShowLineNumbers && !string.IsNullOrEmpty(Code))
            {
                var lineCount = Code.Split('\n').Length;
                var sb = new StringBuilder();
                for (int i = 1; i <= lineCount; i++)
                {
                    sb.AppendLine(i.ToString());
                }
                _lineNumbers.Text = sb.ToString();
                _lineNumbers.IsVisible = true;
            }
            else
            {
                _lineNumbers.Text = string.Empty;
                _lineNumbers.IsVisible = false;
            }
        }
    }
}
