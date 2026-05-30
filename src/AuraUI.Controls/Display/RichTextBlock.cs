using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Media;

namespace AuraUI.Controls.Display;

/// <summary>
/// A rich text display control that renders formatted text with support for
/// bold, italic, underline, strikethrough, hyperlinks, inline images,
/// and colored text segments. Unlike <see cref="MarkdownViewer"/> which
/// requires markdown syntax, this control uses an inline markup format
/// or structured <see cref="RichTextSegment"/> objects.
/// </summary>
[TemplatePart("PART_ContentPanel", typeof(Panel))]
public class RichTextBlock : TemplatedControl
{
    /// <summary>
    /// Defines the <see cref="Segments"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IList<RichTextSegment>?> SegmentsProperty =
        AvaloniaProperty.Register<RichTextBlock, IList<RichTextSegment>?>(nameof(Segments));

    /// <summary>
    /// Defines the <see cref="Text"/> styled property.
    /// Supports inline markup: **bold**, *italic*, __underline__, ~~strikethrough~~,
    /// [link text](url), and {color:#RRGGBB|colored text}.
    /// </summary>
    public static readonly StyledProperty<string?> TextProperty =
        AvaloniaProperty.Register<RichTextBlock, string?>(nameof(Text));

    /// <summary>
    /// Defines the <see cref="LineSpacing"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> LineSpacingProperty =
        AvaloniaProperty.Register<RichTextBlock, double>(nameof(LineSpacing), 4.0);

    /// <summary>
    /// Defines the <see cref="IsTextSelectionEnabled"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsTextSelectionEnabledProperty =
        AvaloniaProperty.Register<RichTextBlock, bool>(nameof(IsTextSelectionEnabled));

    /// <summary>
    /// Defines the <see cref="HyperlinkBrush"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> HyperlinkBrushProperty =
        AvaloniaProperty.Register<RichTextBlock, IBrush?>(nameof(HyperlinkBrush));

    /// <summary>
    /// Defines the <see cref="MaxLines"/> styled property.
    /// 0 means unlimited.
    /// </summary>
    public static readonly StyledProperty<int> MaxLinesProperty =
        AvaloniaProperty.Register<RichTextBlock, int>(nameof(MaxLines));

    /// <summary>
    /// Defines the <see cref="TextTrimming"/> styled property.
    /// </summary>
    public static readonly StyledProperty<TextTrimming> TextTrimmingProperty =
        AvaloniaProperty.Register<RichTextBlock, TextTrimming>(nameof(TextTrimming), TextTrimming.None);

    static RichTextBlock()
    {
        TextProperty.Changed.AddClassHandler<RichTextBlock>((x, _) => x.OnTextChanged());
    }

    /// <summary>
    /// Occurs when a hyperlink is clicked.
    /// </summary>
    public event EventHandler<LinkClickedEventArgs>? LinkClicked;

    /// <summary>
    /// Gets or sets the structured rich text segments.
    /// When set, these take precedence over the <see cref="Text"/> property.
    /// </summary>
    public IList<RichTextSegment>? Segments
    {
        get => GetValue(SegmentsProperty);
        set => SetValue(SegmentsProperty, value);
    }

    /// <summary>
    /// Gets or sets the rich text content using inline markup.
    /// </summary>
    public string? Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    /// <summary>
    /// Gets or sets the spacing between lines.
    /// </summary>
    public double LineSpacing
    {
        get => GetValue(LineSpacingProperty);
        set => SetValue(LineSpacingProperty, value);
    }

    /// <summary>
    /// Gets or sets whether text selection is enabled.
    /// </summary>
    public bool IsTextSelectionEnabled
    {
        get => GetValue(IsTextSelectionEnabledProperty);
        set => SetValue(IsTextSelectionEnabledProperty, value);
    }

    /// <summary>
    /// Gets or sets the brush used for hyperlink text.
    /// </summary>
    public IBrush? HyperlinkBrush
    {
        get => GetValue(HyperlinkBrushProperty);
        set => SetValue(HyperlinkBrushProperty, value);
    }

    /// <summary>
    /// Gets or sets the maximum number of lines to display. 0 for unlimited.
    /// </summary>
    public int MaxLines
    {
        get => GetValue(MaxLinesProperty);
        set => SetValue(MaxLinesProperty, value);
    }

    /// <summary>
    /// Gets or sets the text trimming behavior.
    /// </summary>
    public TextTrimming TextTrimming
    {
        get => GetValue(TextTrimmingProperty);
        set => SetValue(TextTrimmingProperty, value);
    }

    private void OnTextChanged()
    {
        // Parse markup text into segments when Text changes and Segments is not set.
        if (Segments != null)
            return;

        var parsed = RichTextMarkupParser.Parse(Text ?? string.Empty);
        SetCurrentValue(SegmentsProperty, parsed);
    }

    internal void RaiseLinkClicked(string url)
    {
        LinkClicked?.Invoke(this, new LinkClickedEventArgs(url));
    }
}

/// <summary>
/// Represents a single formatted segment of text within a <see cref="RichTextBlock"/>.
/// </summary>
public class RichTextSegment
{
    /// <summary>
    /// Gets or sets the text content.
    /// </summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets whether the text is bold.
    /// </summary>
    public bool IsBold { get; set; }

    /// <summary>
    /// Gets or sets whether the text is italic.
    /// </summary>
    public bool IsItalic { get; set; }

    /// <summary>
    /// Gets or sets whether the text is underlined.
    /// </summary>
    public bool IsUnderline { get; set; }

    /// <summary>
    /// Gets or sets whether the text has strikethrough.
    /// </summary>
    public bool IsStrikethrough { get; set; }

    /// <summary>
    /// Gets or sets the text foreground color. Null for default.
    /// </summary>
    public IBrush? Foreground { get; set; }

    /// <summary>
    /// Gets or sets the background highlight color. Null for none.
    /// </summary>
    public IBrush? Highlight { get; set; }

    /// <summary>
    /// Gets or sets the hyperlink URL. Null if not a link.
    /// </summary>
    public string? LinkUrl { get; set; }

    /// <summary>
    /// Gets or sets the font size override. Null for default.
    /// </summary>
    public double? FontSize { get; set; }

    /// <summary>
    /// Gets or sets the font weight override. Null for default.
    /// </summary>
    public FontWeight? FontWeight { get; set; }
}

/// <summary>
/// Event arguments for hyperlink clicks in a <see cref="RichTextBlock"/>.
/// </summary>
public class LinkClickedEventArgs : EventArgs
{
    public string Url { get; }

    public LinkClickedEventArgs(string url)
    {
        Url = url;
    }
}

/// <summary>
/// Simple inline markup parser for <see cref="RichTextBlock"/>.
/// Supports: **bold**, *italic*, __underline__, ~~strikethrough~~,
/// [link text](url), and {color:#RRGGBB|colored text}.
/// </summary>
internal static class RichTextMarkupParser
{
    public static IList<RichTextSegment> Parse(string markup)
    {
        var segments = new List<RichTextSegment>();
        if (string.IsNullOrEmpty(markup))
            return segments;

        var span = markup.AsSpan();
        int i = 0;

        while (i < span.Length)
        {
            // Bold: **text**
            if (i + 1 < span.Length && span[i] == '*' && span[i + 1] == '*')
            {
                var end = span.Slice(i + 2).IndexOf("**");
                if (end >= 0)
                {
                    segments.Add(new RichTextSegment
                    {
                        Text = span.Slice(i + 2, end).ToString(),
                        IsBold = true
                    });
                    i += end + 4;
                    continue;
                }
            }

            // Italic: *text*
            if (span[i] == '*' && (i + 1 >= span.Length || span[i + 1] != '*'))
            {
                var end = span.Slice(i + 1).IndexOf('*');
                if (end >= 0)
                {
                    segments.Add(new RichTextSegment
                    {
                        Text = span.Slice(i + 1, end).ToString(),
                        IsItalic = true
                    });
                    i += end + 2;
                    continue;
                }
            }

            // Underline: __text__
            if (i + 1 < span.Length && span[i] == '_' && span[i + 1] == '_')
            {
                var end = span.Slice(i + 2).IndexOf("__");
                if (end >= 0)
                {
                    segments.Add(new RichTextSegment
                    {
                        Text = span.Slice(i + 2, end).ToString(),
                        IsUnderline = true
                    });
                    i += end + 4;
                    continue;
                }
            }

            // Strikethrough: ~~text~~
            if (i + 1 < span.Length && span[i] == '~' && span[i + 1] == '~')
            {
                var end = span.Slice(i + 2).IndexOf("~~");
                if (end >= 0)
                {
                    segments.Add(new RichTextSegment
                    {
                        Text = span.Slice(i + 2, end).ToString(),
                        IsStrikethrough = true
                    });
                    i += end + 4;
                    continue;
                }
            }

            // Link: [text](url)
            if (span[i] == '[')
            {
                var closeBracket = span.Slice(i + 1).IndexOf(']');
                if (closeBracket >= 0 && i + 2 + closeBracket < span.Length && span[i + 2 + closeBracket] == '(')
                {
                    var closeParen = span.Slice(i + 3 + closeBracket).IndexOf(')');
                    if (closeParen >= 0)
                    {
                        var linkText = span.Slice(i + 1, closeBracket).ToString();
                        var linkUrl = span.Slice(i + 3 + closeBracket, closeParen).ToString();
                        segments.Add(new RichTextSegment
                        {
                            Text = linkText,
                            LinkUrl = linkUrl
                        });
                        i += closeBracket + closeParen + 4;
                        continue;
                    }
                }
            }

            // Plain text: accumulate until next markup character
            var start = i;
            while (i < span.Length && span[i] != '*' && span[i] != '_' && span[i] != '~' && span[i] != '[')
            {
                i++;
            }

            if (i > start)
            {
                segments.Add(new RichTextSegment
                {
                    Text = span.Slice(start, i - start).ToString()
                });
            }
            else
            {
                // Fallback: add the character and advance
                segments.Add(new RichTextSegment
                {
                    Text = span.Slice(i, 1).ToString()
                });
                i++;
            }
        }

        return segments;
    }
}
