using Avalonia;
using Avalonia.Media;

namespace AuraUI.Controls.Charts.Components;

/// <summary>
/// ECharts-style rich text component. Supports rendering text with multiple
/// styled segments (different fonts, colors, sizes, and line heights) within
/// a single text block.
///
/// Used for complex labels, tooltips, and axis labels where different parts
/// of the text need different styling.
///
/// Example usage:
///   var rich = new RichText();
///   rich.Blocks.Add(new RichTextBlock { Text = "Sales: ", FontWeight = FontWeight.Bold });
///   rich.Blocks.Add(new RichTextBlock { Text = "$1,234", Color = Brushes.Green });
///
/// Rendering:
///   - Each block is measured and laid out sequentially
///   - Supports inline baseline alignment across different font sizes
///   - Text wrapping within a maximum width
/// </summary>
public class RichText : AvaloniaObject
{
    /// <summary>Maximum width for text wrapping. When 0, no wrapping.</summary>
    public static readonly StyledProperty<double> MaxWidthProperty =
        AvaloniaProperty.Register<RichText, double>(nameof(MaxWidth));

    /// <summary>Default font family for blocks that don't specify one.</summary>
    public static readonly StyledProperty<string?> FontFamilyProperty =
        AvaloniaProperty.Register<RichText, string?>(nameof(FontFamily));

    /// <summary>Default font size for blocks that don't specify one.</summary>
    public static readonly StyledProperty<double> FontSizeProperty =
        AvaloniaProperty.Register<RichText, double>(nameof(FontSize), 12.0);

    /// <summary>Default text color for blocks that don't specify one.</summary>
    public static readonly StyledProperty<IBrush?> TextColorProperty =
        AvaloniaProperty.Register<RichText, IBrush?>(nameof(TextColor));

    /// <summary>Line height multiplier (1.0 = normal, 1.5 = 1.5x spacing).</summary>
    public static readonly StyledProperty<double> LineHeightProperty =
        AvaloniaProperty.Register<RichText, double>(nameof(LineHeight), 1.4);

    // CLR wrappers
    public double MaxWidth { get => GetValue(MaxWidthProperty); set => SetValue(MaxWidthProperty, value); }
    public string? FontFamily { get => GetValue(FontFamilyProperty); set => SetValue(FontFamilyProperty, value); }
    public double FontSize { get => GetValue(FontSizeProperty); set => SetValue(FontSizeProperty, value); }
    public IBrush? TextColor { get => GetValue(TextColorProperty); set => SetValue(TextColorProperty, value); }
    public double LineHeight { get => GetValue(LineHeightProperty); set => SetValue(LineHeightProperty, value); }

    /// <summary>
    /// The styled text blocks to render. Each block can have its own font, color, size, etc.
    /// </summary>
    public List<RichTextBlock> Blocks { get; set; } = new();

    /// <summary>
    /// Measure the total size of the rich text block.
    /// </summary>
    /// <returns>The (width, height) of the rendered text.</returns>
    public (double Width, double Height) Measure()
    {
        if (Blocks.Count == 0) return (0, 0);

        var defaultFont = FontFamily ?? "Segoe UI";
        var defaultSize = FontSize;
        var defaultColor = TextColor ?? Brushes.Black;
        var maxWidth = MaxWidth;

        double totalWidth = 0;
        double totalHeight = 0;
        double currentLineWidth = 0;
        double currentLineHeight = 0;

        foreach (var block in Blocks)
        {
            var fontFamily = block.FontFamily ?? defaultFont;
            var fontSize = block.FontSize > 0 ? block.FontSize : defaultSize;
            var fontWeight = block.FontWeight;
            var fontStyle = block.FontStyle;

            var formatted = new FormattedText(
                block.Text ?? string.Empty,
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface(fontFamily, fontStyle, fontWeight),
                fontSize,
                defaultColor);

            // Check for line break
            if (block.IsLineBreak || (maxWidth > 0 && currentLineWidth + formatted.Width > maxWidth))
            {
                totalHeight += currentLineHeight * LineHeight;
                totalWidth = Math.Max(totalWidth, currentLineWidth);
                currentLineWidth = 0;
                currentLineHeight = 0;
            }

            currentLineWidth += formatted.Width;
            currentLineHeight = Math.Max(currentLineHeight, formatted.Height);
        }

        // Final line
        totalHeight += currentLineHeight * LineHeight;
        totalWidth = Math.Max(totalWidth, currentLineWidth);

        return (totalWidth, totalHeight);
    }

    /// <summary>
    /// Render the rich text at the specified position.
    /// </summary>
    /// <param name="context">Drawing context.</param>
    /// <param name="position">Top-left position to start rendering.</param>
    /// <param name="defaultForeground">Fallback foreground brush from theme.</param>
    /// <returns>The total size consumed by the rendered text.</returns>
    public Size Render(DrawingContext context, Point position, IBrush? defaultForeground = null)
    {
        if (Blocks.Count == 0) return new Size(0, 0);

        var defaultFont = FontFamily ?? "Segoe UI";
        var defaultSize = FontSize;
        var defaultColor = TextColor ?? defaultForeground ?? Brushes.Black;
        var maxWidth = MaxWidth;

        double x = position.X;
        double y = position.Y;
        double maxLineWidth = 0;
        double currentLineHeight = 0;

        foreach (var block in Blocks)
        {
            var fontFamily = block.FontFamily ?? defaultFont;
            var fontSize = block.FontSize > 0 ? block.FontSize : defaultSize;
            var fontWeight = block.FontWeight;
            var fontStyle = block.FontStyle;
            var color = block.Color ?? defaultColor;

            var formatted = new FormattedText(
                block.Text ?? string.Empty,
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface(fontFamily, fontStyle, fontWeight),
                fontSize,
                color);

            // Check for line break
            if (block.IsLineBreak || (maxWidth > 0 && x - position.X + formatted.Width > maxWidth))
            {
                y += currentLineHeight * LineHeight;
                x = position.X;
                currentLineHeight = 0;
            }

            // Apply baseline offset for different font sizes
            var baselineOffset = (currentLineHeight - formatted.Height) / 2;
            if (baselineOffset < 0) baselineOffset = 0;

            // Draw underline if specified
            if (block.TextDecoration == RichTextDecoration.Underline)
            {
                var underlineY = y + currentLineHeight * LineHeight - 2;
                context.DrawLine(new Pen(color, 1), new Point(x, underlineY), new Point(x + formatted.Width, underlineY));
            }

            // Draw strikethrough if specified
            if (block.TextDecoration == RichTextDecoration.Strikethrough)
            {
                var strikeY = y + currentLineHeight * LineHeight / 2;
                context.DrawLine(new Pen(color, 1), new Point(x, strikeY), new Point(x + formatted.Width, strikeY));
            }

            // Draw text background if specified
            if (block.Background is IBrush bg)
            {
                var bgRect = new Rect(x, y, formatted.Width, formatted.Height);
                context.DrawRectangle(bg, null, bgRect, 2, 2);
            }

            context.DrawText(formatted, new Point(x, y + baselineOffset));

            x += formatted.Width;
            currentLineHeight = Math.Max(currentLineHeight, formatted.Height);
            maxLineWidth = Math.Max(maxLineWidth, x - position.X);
        }

        var totalHeight = y - position.Y + currentLineHeight * LineHeight;
        return new Size(maxLineWidth, totalHeight);
    }

    /// <summary>
    /// Create a RichText from a simple string with default styling.
    /// </summary>
    public static RichText FromString(string text, IBrush? color = null, double fontSize = 12, FontWeight fontWeight = FontWeight.Normal)
    {
        var rt = new RichText();
        rt.Blocks.Add(new RichTextBlock
        {
            Text = text,
            Color = color,
            FontSize = fontSize,
            FontWeight = fontWeight
        });
        return rt;
    }
}

/// <summary>
/// A single styled text segment within a <see cref="RichText"/> block.
/// </summary>
public class RichTextBlock
{
    /// <summary>The text content of this block.</summary>
    public string? Text { get; set; }

    /// <summary>Font family override for this block.</summary>
    public string? FontFamily { get; set; }

    /// <summary>Font size override for this block. When 0, uses the parent RichText default.</summary>
    public double FontSize { get; set; }

    /// <summary>Font weight for this block.</summary>
    public FontWeight FontWeight { get; set; } = FontWeight.Normal;

    /// <summary>Font style for this block.</summary>
    public FontStyle FontStyle { get; set; } = FontStyle.Normal;

    /// <summary>Text color override for this block.</summary>
    public IBrush? Color { get; set; }

    /// <summary>Background highlight color for this block.</summary>
    public IBrush? Background { get; set; }

    /// <summary>Text decoration (underline, strikethrough).</summary>
    public RichTextDecoration TextDecoration { get; set; } = RichTextDecoration.None;

    /// <summary>
    /// Whether this block represents a line break.
    /// When true, the Text property is ignored and a new line is started.
    /// </summary>
    public bool IsLineBreak { get; set; }

    /// <summary>Line height multiplier override for this block.</summary>
    public double LineHeight { get; set; }

    public RichTextBlock() { }

    public RichTextBlock(string text)
    {
        Text = text;
    }

    public RichTextBlock(string text, IBrush? color, double fontSize = 0, FontWeight fontWeight = FontWeight.Normal)
    {
        Text = text;
        Color = color;
        FontSize = fontSize;
        FontWeight = fontWeight;
    }
}

/// <summary>
/// Text decoration options for rich text blocks.
/// </summary>
public enum RichTextDecoration
{
    /// <summary>No decoration.</summary>
    None,

    /// <summary>Underline text.</summary>
    Underline,

    /// <summary>Strikethrough text.</summary>
    Strikethrough
}
