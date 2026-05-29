using Avalonia;
using Avalonia.Media;

namespace AuraUI.Controls.Charts.Components;

/// <summary>
/// ECharts-style title component. Renders a title and subtitle with
/// configurable position, alignment, and text styles.
///
/// The title is positioned relative to the chart area using Left/Top offsets
/// which can be absolute pixels or percentages ("50%", "center", "left", "right").
///
/// Rendering:
///   - Title text drawn via DrawingContext at the configured position
///   - Subtitle drawn below the title with separate styling
///   - Supports left/center/right text alignment
///   - Configurable padding around the title block
/// </summary>
public class ChartTitle : AvaloniaObject
{
    /// <summary>The main title text.</summary>
    public static readonly StyledProperty<string?> TextProperty =
        AvaloniaProperty.Register<ChartTitle, string?>(nameof(Text));

    /// <summary>The subtitle text displayed below the main title.</summary>
    public static readonly StyledProperty<string?> SubtextProperty =
        AvaloniaProperty.Register<ChartTitle, string?>(nameof(Subtext));

    /// <summary>
    /// Horizontal position of the title. Accepts pixel value, percentage string
    /// ("50%"), or keywords: "left", "center", "right".
    /// Default is "left".
    /// </summary>
    public static readonly StyledProperty<string?> LeftProperty =
        AvaloniaProperty.Register<ChartTitle, string?>(nameof(Left), "left");

    /// <summary>
    /// Vertical position of the title. Accepts pixel value, percentage string
    /// ("50%"), or keywords: "top", "middle", "bottom".
    /// Default is "top".
    /// </summary>
    public static readonly StyledProperty<string?> TopProperty =
        AvaloniaProperty.Register<ChartTitle, string?>(nameof(Top), "top");

    /// <summary>Text alignment for the title text.</summary>
    public static readonly StyledProperty<TextAlignment> TextAlignProperty =
        AvaloniaProperty.Register<ChartTitle, TextAlignment>(nameof(TextAlign), TextAlignment.Left);

    /// <summary>Title text font size.</summary>
    public static readonly StyledProperty<double> FontSizeProperty =
        AvaloniaProperty.Register<ChartTitle, double>(nameof(FontSize), 16.0);

    /// <summary>Title text font weight.</summary>
    public static readonly StyledProperty<FontWeight> FontWeightProperty =
        AvaloniaProperty.Register<ChartTitle, FontWeight>(nameof(FontWeight), FontWeight.Bold);

    /// <summary>Title text color.</summary>
    public static readonly StyledProperty<IBrush?> TextColorProperty =
        AvaloniaProperty.Register<ChartTitle, IBrush?>(nameof(TextColor));

    /// <summary>Font family for the title text.</summary>
    public static readonly StyledProperty<string?> FontFamilyProperty =
        AvaloniaProperty.Register<ChartTitle, string?>(nameof(FontFamily));

    /// <summary>Subtitle text font size.</summary>
    public static readonly StyledProperty<double> SubtextFontSizeProperty =
        AvaloniaProperty.Register<ChartTitle, double>(nameof(SubtextFontSize), 12.0);

    /// <summary>Subtitle text color.</summary>
    public static readonly StyledProperty<IBrush?> SubtextColorProperty =
        AvaloniaProperty.Register<ChartTitle, IBrush?>(nameof(SubtextColor));

    /// <summary>Font weight for the subtitle text.</summary>
    public static readonly StyledProperty<FontWeight> SubtextFontWeightProperty =
        AvaloniaProperty.Register<ChartTitle, FontWeight>(nameof(SubtextFontWeight), FontWeight.Normal);

    /// <summary>Font family for the subtitle text.</summary>
    public static readonly StyledProperty<string?> SubtextFontFamilyProperty =
        AvaloniaProperty.Register<ChartTitle, string?>(nameof(SubtextFontFamily));

    /// <summary>Padding around the title block (left, top, right, bottom).</summary>
    public static readonly StyledProperty<Thickness> PaddingProperty =
        AvaloniaProperty.Register<ChartTitle, Thickness>(nameof(Padding), new Thickness(0));

    /// <summary>Background brush for the title area.</summary>
    public static readonly StyledProperty<IBrush?> BackgroundProperty =
        AvaloniaProperty.Register<ChartTitle, IBrush?>(nameof(Background));

    /// <summary>Whether the title component is visible.</summary>
    public static readonly StyledProperty<bool> IsVisibleProperty =
        AvaloniaProperty.Register<ChartTitle, bool>(nameof(IsVisible), true);

    // CLR wrappers
    public string? Text { get => GetValue(TextProperty); set => SetValue(TextProperty, value); }
    public string? Subtext { get => GetValue(SubtextProperty); set => SetValue(SubtextProperty, value); }
    public string? Left { get => GetValue(LeftProperty); set => SetValue(LeftProperty, value); }
    public string? Top { get => GetValue(TopProperty); set => SetValue(TopProperty, value); }
    public TextAlignment TextAlign { get => GetValue(TextAlignProperty); set => SetValue(TextAlignProperty, value); }
    public double FontSize { get => GetValue(FontSizeProperty); set => SetValue(FontSizeProperty, value); }
    public FontWeight FontWeight { get => GetValue(FontWeightProperty); set => SetValue(FontWeightProperty, value); }
    public IBrush? TextColor { get => GetValue(TextColorProperty); set => SetValue(TextColorProperty, value); }
    public string? FontFamily { get => GetValue(FontFamilyProperty); set => SetValue(FontFamilyProperty, value); }
    public double SubtextFontSize { get => GetValue(SubtextFontSizeProperty); set => SetValue(SubtextFontSizeProperty, value); }
    public IBrush? SubtextColor { get => GetValue(SubtextColorProperty); set => SetValue(SubtextColorProperty, value); }
    public FontWeight SubtextFontWeight { get => GetValue(SubtextFontWeightProperty); set => SetValue(SubtextFontWeightProperty, value); }
    public string? SubtextFontFamily { get => GetValue(SubtextFontFamilyProperty); set => SetValue(SubtextFontFamilyProperty, value); }
    public Thickness Padding { get => GetValue(PaddingProperty); set => SetValue(PaddingProperty, value); }
    public IBrush? Background { get => GetValue(BackgroundProperty); set => SetValue(BackgroundProperty, value); }
    public bool IsVisible { get => GetValue(IsVisibleProperty); set => SetValue(IsVisibleProperty, value); }

    /// <summary>
    /// Compute the resolved Left position in pixels given the chart bounds.
    /// </summary>
    internal double ResolveLeft(Rect chartBounds)
    {
        var left = Left ?? "left";
        var padding = Padding;

        if (left == "center")
            return chartBounds.Left + chartBounds.Width / 2;
        if (left == "right")
            return chartBounds.Right - padding.Right;
        if (left.EndsWith('%'))
        {
            if (double.TryParse(left.TrimEnd('%'), out var pct))
                return chartBounds.Left + chartBounds.Width * pct / 100.0;
        }
        if (double.TryParse(left, out var px))
            return chartBounds.Left + px;

        return chartBounds.Left + padding.Left;
    }

    /// <summary>
    /// Compute the resolved Top position in pixels given the chart bounds.
    /// </summary>
    internal double ResolveTop(Rect chartBounds)
    {
        var top = Top ?? "top";
        var padding = Padding;

        if (top == "middle")
            return chartBounds.Top + chartBounds.Height / 2;
        if (top == "bottom")
            return chartBounds.Bottom - padding.Bottom;
        if (top.EndsWith('%'))
        {
            if (double.TryParse(top.TrimEnd('%'), out var pct))
                return chartBounds.Top + chartBounds.Height * pct / 100.0;
        }
        if (double.TryParse(top, out var px))
            return chartBounds.Top + px;

        return chartBounds.Top + padding.Top;
    }

    /// <summary>
    /// Render the title and subtitle onto the drawing context.
    /// </summary>
    /// <param name="context">Drawing context.</param>
    /// <param name="chartBounds">Full chart bounds for position resolution.</param>
    /// <param name="defaultForeground">Fallback foreground brush from theme.</param>
    /// <returns>The height consumed by the title block (for layout).</returns>
    public double Render(DrawingContext context, Rect chartBounds, IBrush? defaultForeground)
    {
        if (!IsVisible) return 0;
        if (string.IsNullOrEmpty(Text) && string.IsNullOrEmpty(Subtext)) return 0;

        var padding = Padding;
        var fontFamily = FontFamily ?? "Segoe UI";
        var subFontFamily = SubtextFontFamily ?? fontFamily;
        var resolvedLeft = ResolveLeft(chartBounds);
        var resolvedTop = ResolveTop(chartBounds);

        // Draw background if set
        if (Background is IBrush bg)
        {
            context.DrawRectangle(bg, null, chartBounds);
        }

        double totalHeight = 0;
        double titleWidth = 0;
        double titleHeight = 0;
        FormattedText? formattedTitle = null;

        // Measure and draw title
        if (!string.IsNullOrEmpty(Text))
        {
            var titleBrush = TextColor ?? defaultForeground ?? Brushes.Black;
            formattedTitle = new FormattedText(
                Text,
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface(fontFamily, FontStyle.Normal, FontWeight),
                FontSize,
                titleBrush);

            titleWidth = formattedTitle.Width;
            titleHeight = formattedTitle.Height;
            totalHeight += titleHeight;
        }

        // Measure and draw subtitle
        FormattedText? formattedSub = null;
        if (!string.IsNullOrEmpty(Subtext))
        {
            var subBrush = SubtextColor ?? defaultForeground ?? Brushes.Gray;
            formattedSub = new FormattedText(
                Subtext,
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface(subFontFamily, FontStyle.Normal, SubtextFontWeight),
                SubtextFontSize,
                subBrush);

            totalHeight += formattedSub.Height + 2;
        }

        // Compute X position based on alignment
        var maxWidth = Math.Max(titleWidth, formattedSub?.Width ?? 0);
        double drawX = resolvedLeft;
        if (Left == "center")
        {
            drawX = resolvedLeft - maxWidth / 2;
        }
        else if (Left == "right")
        {
            drawX = resolvedLeft - maxWidth - padding.Right;
        }

        // Apply alignment offset
        if (TextAlign == TextAlignment.Center)
        {
            drawX = resolvedLeft - maxWidth / 2;
        }
        else if (TextAlign == TextAlignment.Right)
        {
            drawX = resolvedLeft - maxWidth;
        }

        // Draw title
        if (formattedTitle != null)
        {
            double titleDrawX = drawX;
            if (TextAlign == TextAlignment.Center)
                titleDrawX = resolvedLeft - titleWidth / 2;
            else if (TextAlign == TextAlignment.Right)
                titleDrawX = resolvedLeft - titleWidth;

            context.DrawText(formattedTitle, new Point(titleDrawX, resolvedTop));
        }

        // Draw subtitle below title
        if (formattedSub != null)
        {
            double subDrawX = drawX;
            if (TextAlign == TextAlignment.Center)
                subDrawX = resolvedLeft - formattedSub.Width / 2;
            else if (TextAlign == TextAlignment.Right)
                subDrawX = resolvedLeft - formattedSub.Width;

            var subY = resolvedTop + titleHeight + 2;
            context.DrawText(formattedSub, new Point(subDrawX, subY));
        }

        return totalHeight + padding.Top + padding.Bottom;
    }

    /// <summary>
    /// Measure the total height the title block will consume.
    /// </summary>
    public double MeasureHeight()
    {
        if (!IsVisible) return 0;
        if (string.IsNullOrEmpty(Text) && string.IsNullOrEmpty(Subtext)) return 0;

        double height = 0;
        if (!string.IsNullOrEmpty(Text))
            height += FontSize * 1.3; // Approximate line height
        if (!string.IsNullOrEmpty(Subtext))
            height += SubtextFontSize * 1.3 + 2;

        return height + Padding.Top + Padding.Bottom;
    }
}
