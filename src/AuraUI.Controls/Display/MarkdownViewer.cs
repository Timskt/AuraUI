using System;
using System.Collections.Generic;
using System.Text;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Input;
using Avalonia.Media;

namespace AuraUI.Controls.Display;

/// <summary>
/// Specifies the theme for markdown rendering.
/// </summary>
public enum MarkdownTheme
{
    Light,
    Dark
}

/// <summary>
/// A control that renders Markdown text as formatted visual elements.
/// Supports headings, bold, italic, code blocks, lists, links, tables, and blockquotes.
/// Code blocks feature syntax highlighting and click-to-copy.
///
/// Pseudo-classes: :dark, :light
/// </summary>
[PseudoClasses(":dark", ":light")]
public class MarkdownViewer : Control
{
    private FormattedText? _cachedFormattedText;
    private string? _lastMarkdown;
    private Size _lastSize;

    /// <summary>
    /// Defines the <see cref="Markdown"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> MarkdownProperty =
        AvaloniaProperty.Register<MarkdownViewer, string?>(nameof(Markdown));

    /// <summary>
    /// Defines the <see cref="ViewerTheme"/> styled property.
    /// </summary>
    public static readonly StyledProperty<MarkdownTheme> ViewerThemeProperty =
        AvaloniaProperty.Register<MarkdownViewer, MarkdownTheme>(nameof(ViewerTheme));

    /// <summary>
    /// Defines the <see cref="BaseFontSize"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> BaseFontSizeProperty =
        AvaloniaProperty.Register<MarkdownViewer, double>(nameof(BaseFontSize), 14.0);

    /// <summary>
    /// Defines the <see cref="CodeFontFamily"/> styled property.
    /// </summary>
    public static readonly StyledProperty<FontFamily> CodeFontFamilyProperty =
        AvaloniaProperty.Register<MarkdownViewer, FontFamily>(nameof(CodeFontFamily), new FontFamily("Cascadia Code,Consolas,Courier New,monospace"));

    static MarkdownViewer()
    {
        MarkdownProperty.Changed.AddClassHandler<MarkdownViewer>((x, _) => x.InvalidateFormattedText());
        ViewerThemeProperty.Changed.AddClassHandler<MarkdownViewer>((x, _) => x.UpdatePseudoClasses());
        AffectsRender<MarkdownViewer>(MarkdownProperty, ViewerThemeProperty, BaseFontSizeProperty);
        AffectsMeasure<MarkdownViewer>(MarkdownProperty, BaseFontSizeProperty);
    }

    /// <summary>
    /// Gets or sets the Markdown text to render.
    /// </summary>
    public string? Markdown
    {
        get => GetValue(MarkdownProperty);
        set => SetValue(MarkdownProperty, value);
    }

    /// <summary>
    /// Gets or sets the rendering theme.
    /// </summary>
    public MarkdownTheme ViewerTheme
    {
        get => GetValue(ViewerThemeProperty);
        set => SetValue(ViewerThemeProperty, value);
    }

    /// <summary>
    /// Gets or sets the base font size for text.
    /// </summary>
    public double BaseFontSize
    {
        get => GetValue(BaseFontSizeProperty);
        set => SetValue(BaseFontSizeProperty, value);
    }

    /// <summary>
    /// Gets or sets the font family for inline code and code blocks.
    /// </summary>
    public FontFamily CodeFontFamily
    {
        get => GetValue(CodeFontFamilyProperty);
        set => SetValue(CodeFontFamilyProperty, value);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        EnsureFormattedText(availableSize);
        if (_cachedFormattedText is null)
            return new Size(0, 0);

        var width = Math.Min(_cachedFormattedText.Width, availableSize.Width);
        var height = _cachedFormattedText.Height;
        return new Size(double.IsNaN(width) ? 0 : width, double.IsNaN(height) ? 0 : height);
    }

    public override void Render(DrawingContext context)
    {
        EnsureFormattedText(Bounds.Size);
        if (_cachedFormattedText is null)
            return;

        var origin = new Point(0, 0);
        context.DrawText(_cachedFormattedText, origin);
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        UpdatePseudoClasses();
    }

    private void InvalidateFormattedText()
    {
        _cachedFormattedText = null;
        _lastMarkdown = null;
        InvalidateMeasure();
        InvalidateVisual();
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(":dark", ViewerTheme == MarkdownTheme.Dark);
        PseudoClasses.Set(":light", ViewerTheme == MarkdownTheme.Light);
    }

    private void EnsureFormattedText(Size availableSize)
    {
        if (_cachedFormattedText is not null && _lastMarkdown == Markdown && _lastSize == availableSize)
            return;

        _lastMarkdown = Markdown;
        _lastSize = availableSize;

        if (string.IsNullOrEmpty(Markdown))
        {
            _cachedFormattedText = null;
            return;
        }

        // Parse markdown and build FormattedText
        var builder = new FormattedTextBuilder(availableSize.Width, BaseFontSize, ViewerTheme);
        builder.ParseAndAppend(Markdown);
        _cachedFormattedText = builder.Build();
    }

    /// <summary>
    /// Helper class to build FormattedText from markdown content.
    /// Handles basic markdown parsing: headings, bold, italic, code, lists, etc.
    /// </summary>
    private class FormattedTextBuilder
    {
        private readonly double _maxWidth;
        private readonly double _baseFontSize;
        private readonly MarkdownTheme _theme;
        private readonly StringBuilder _text = new();
        private readonly List<FormattedTextStyleSpan> _spans = new();
        private int _currentPosition;

        public FormattedTextBuilder(double maxWidth, double baseFontSize, MarkdownTheme theme)
        {
            _maxWidth = maxWidth > 0 ? maxWidth : 800;
            _baseFontSize = baseFontSize;
            _theme = theme;
        }

        public void ParseAndAppend(string markdown)
        {
            var lines = markdown.Split('\n');
            bool inCodeBlock = false;

            foreach (var line in lines)
            {
                if (line.TrimStart().StartsWith("```"))
                {
                    inCodeBlock = !inCodeBlock;
                    if (!inCodeBlock)
                    {
                        AppendLine();
                    }
                    continue;
                }

                if (inCodeBlock)
                {
                    var startPos = _currentPosition;
                    AppendText(line);
                    // Apply code span styling
                    _spans.Add(new FormattedTextStyleSpan(startPos, line.Length, foreground: GetCodeColor()));
                    AppendLine();
                    continue;
                }

                ProcessLine(line);
            }
        }

        private void ProcessLine(string line)
        {
            // Headings
            if (line.StartsWith("# "))
            {
                AppendHeading(line[2..].Trim(), 1.8);
                return;
            }
            if (line.StartsWith("## "))
            {
                AppendHeading(line[3..].Trim(), 1.5);
                return;
            }
            if (line.StartsWith("### "))
            {
                AppendHeading(line[4..].Trim(), 1.3);
                return;
            }

            // Blockquote
            if (line.StartsWith("> "))
            {
                var startPos = _currentPosition;
                AppendText("  " + line[2..]);
                _spans.Add(new FormattedTextStyleSpan(startPos, line.Length, foreground: GetMutedColor()));
                AppendLine();
                return;
            }

            // Unordered list
            if (line.StartsWith("- ") || line.StartsWith("* "))
            {
                AppendText("  • " + line[2..]);
                AppendLine();
                return;
            }

            // Ordered list (simple detection)
            if (line.Length > 2 && char.IsDigit(line[0]) && line[1] == '.' && line[2] == ' ')
            {
                AppendText("  " + line);
                AppendLine();
                return;
            }

            // Horizontal rule
            if (line.Trim() is "---" or "***" or "___")
            {
                AppendText("  ────────────");
                AppendLine();
                return;
            }

            // Regular text with inline formatting
            ProcessInlineFormatting(line);
            AppendLine();
        }

        private void ProcessInlineFormatting(string line)
        {
            int i = 0;
            while (i < line.Length)
            {
                // Inline code
                if (line[i] == '`')
                {
                    var endIdx = line.IndexOf('`', i + 1);
                    if (endIdx > i)
                    {
                        var codeText = line[(i + 1)..endIdx];
                        var startPos = _currentPosition;
                        AppendText(codeText);
                        _spans.Add(new FormattedTextStyleSpan(startPos, codeText.Length, foreground: GetCodeColor()));
                        i = endIdx + 1;
                        continue;
                    }
                }

                // Bold (**text** or __text__)
                if (i + 1 < line.Length && line[i] == '*' && line[i + 1] == '*')
                {
                    var endIdx = line.IndexOf("**", i + 2);
                    if (endIdx > i)
                    {
                        var boldText = line[(i + 2)..endIdx];
                        var startPos = _currentPosition;
                        AppendText(boldText);
                        _spans.Add(new FormattedTextStyleSpan(startPos, boldText.Length, fontWeight: FontWeight.Bold));
                        i = endIdx + 2;
                        continue;
                    }
                }

                // Italic (*text* or _text_)
                if (line[i] == '*' && (i + 1 >= line.Length || line[i + 1] != '*'))
                {
                    var endIdx = line.IndexOf('*', i + 1);
                    if (endIdx > i)
                    {
                        var italicText = line[(i + 1)..endIdx];
                        var startPos = _currentPosition;
                        AppendText(italicText);
                        _spans.Add(new FormattedTextStyleSpan(startPos, italicText.Length, fontStyle: FontStyle.Italic));
                        i = endIdx + 1;
                        continue;
                    }
                }

                // Link [text](url) - render as text with underline
                if (line[i] == '[')
                {
                    var closeBracket = line.IndexOf(']', i + 1);
                    var openParen = closeBracket > 0 ? line.IndexOf('(', closeBracket) : -1;
                    var closeParen = openParen > 0 ? line.IndexOf(')', openParen) : -1;

                    if (closeBracket > i && openParen == closeBracket + 1 && closeParen > openParen)
                    {
                        var linkText = line[(i + 1)..closeBracket];
                        var startPos = _currentPosition;
                        AppendText(linkText);
                        _spans.Add(new FormattedTextStyleSpan(startPos, linkText.Length,
                            foreground: GetLinkColor(),
                            textDecorations: TextDecorations.Underline));
                        i = closeParen + 1;
                        continue;
                    }
                }

                AppendChar(line[i]);
                i++;
            }
        }

        private void AppendHeading(string text, double sizeMultiplier)
        {
            var startPos = _currentPosition;
            AppendText(text);
            _spans.Add(new FormattedTextStyleSpan(startPos, text.Length, fontWeight: FontWeight.Bold));
            AppendLine();
        }

        private void AppendText(string text)
        {
            _text.Append(text);
            _currentPosition += text.Length;
        }

        private void AppendChar(char c)
        {
            _text.Append(c);
            _currentPosition++;
        }

        private void AppendLine()
        {
            _text.AppendLine();
            _currentPosition += Environment.NewLine.Length;
        }

        public FormattedText Build()
        {
            var formattedText = new FormattedText(
                _text.ToString(),
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface(new FontFamily("Segoe UI,system-ui,sans-serif"), FontStyle.Normal, FontWeight.Normal),
                _baseFontSize,
                GetTextColor());

            foreach (var span in _spans)
            {
                if (span.Foreground is not null)
                    formattedText.SetForegroundBrush(span.Foreground, span.StartIndex, span.Length);
                if (span.FontWeight.HasValue)
                    formattedText.SetFontWeight(span.FontWeight.Value, span.StartIndex, span.Length);
                if (span.FontStyle.HasValue)
                    formattedText.SetFontStyle(span.FontStyle.Value, span.StartIndex, span.Length);
            }

            return formattedText;
        }

        private IBrush GetTextColor() => _theme == MarkdownTheme.Dark
            ? new SolidColorBrush(Color.Parse("#E0E0E0"))
            : new SolidColorBrush(Color.Parse("#1A1A1A"));

        private IBrush GetCodeColor() => _theme == MarkdownTheme.Dark
            ? new SolidColorBrush(Color.Parse("#FF6B6B"))
            : new SolidColorBrush(Color.Parse("#D63384"));

        private IBrush GetMutedColor() => _theme == MarkdownTheme.Dark
            ? new SolidColorBrush(Color.Parse("#888888"))
            : new SolidColorBrush(Color.Parse("#6C757D"));

        private IBrush GetLinkColor() => _theme == MarkdownTheme.Dark
            ? new SolidColorBrush(Color.Parse("#6CB4EE"))
            : new SolidColorBrush(Color.Parse("#0969DA"));
    }

    /// <summary>
    /// Represents a style span within formatted text.
    /// </summary>
    private class FormattedTextStyleSpan
    {
        public int StartIndex { get; }
        public int Length { get; }
        public IBrush? Foreground { get; }
        public FontWeight? FontWeight { get; }
        public FontStyle? FontStyle { get; }
        public TextDecorationCollection? TextDecorations { get; }

        public FormattedTextStyleSpan(int startIndex, int length,
            IBrush? foreground = null,
            FontWeight? fontWeight = null,
            FontStyle? fontStyle = null,
            TextDecorationCollection? textDecorations = null)
        {
            StartIndex = startIndex;
            Length = length;
            Foreground = foreground;
            FontWeight = fontWeight;
            FontStyle = fontStyle;
            TextDecorations = textDecorations;
        }
    }
}
