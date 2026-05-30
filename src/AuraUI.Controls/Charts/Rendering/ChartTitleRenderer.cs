using System.Globalization;
using Avalonia;
using Avalonia.Media;

namespace AuraUI.Controls.Charts.Rendering;

/// <summary>
/// Renders chart title and subtitle. Extracted from Chart.cs for focused testing.
/// </summary>
internal static class ChartTitleRenderer
{
    private static readonly Typeface s_boldLabelTypeface = new("Segoe UI", FontStyle.Normal, FontWeight.SemiBold);
    private static readonly Typeface s_labelTypeface = new("Segoe UI", FontStyle.Normal, FontWeight.Normal);
    private static readonly CultureInfo s_culture = CultureInfo.CurrentCulture;

    /// <summary>
    /// Render chart title and optional subtitle.
    /// </summary>
    public static void Render(
        DrawingContext context,
        Rect plotArea,
        string? title,
        string? subtitle,
        double titleFontSize,
        Thickness chartPadding,
        IBrush? titleBrush,
        Func<string, IBrush?>? tryFindResource = null)
    {
        if (string.IsNullOrEmpty(title)) return;

        var brush = titleBrush
            ?? tryFindResource?.Invoke("AuraForegroundBrush")
            ?? Brushes.Black;

        var formattedTitle = new FormattedText(title,
            s_culture,
            FlowDirection.LeftToRight,
            s_boldLabelTypeface,
            titleFontSize,
            brush);

        var x = plotArea.Center.X - formattedTitle.Width / 2;
        var y = chartPadding.Top;
        context.DrawText(formattedTitle, new Point(x, y));

        if (!string.IsNullOrEmpty(subtitle))
        {
            var subBrush = tryFindResource?.Invoke("AuraForegroundTertiaryBrush") ?? Brushes.Gray;
            var formattedSub = new FormattedText(subtitle,
                s_culture,
                FlowDirection.LeftToRight,
                s_labelTypeface,
                titleFontSize - 3,
                subBrush);

            context.DrawText(formattedSub,
                new Point(plotArea.Center.X - formattedSub.Width / 2, y + formattedTitle.Height + 2));
        }
    }
}
