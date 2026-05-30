using System.Globalization;
using Avalonia;
using Avalonia.Media;

namespace AuraUI.Controls.Charts.Rendering;

/// <summary>
/// Renders the chart legend: colored swatches with labels, custom icons,
/// scroll indicators, and selector buttons. Extracted from Chart.cs for
/// focused testing and reuse.
///
/// Performance: Uses manual iteration (no LINQ in render path). Maintains
/// a cached legend item list that is only rebuilt when the series
/// collection or legend configuration changes.
/// </summary>
internal static class ChartLegendRenderer
{
    private static readonly Typeface s_labelTypeface = new("Segoe UI", FontStyle.Normal, FontWeight.Normal);
    private static readonly CultureInfo s_culture = CultureInfo.CurrentCulture;

    // Cached legend items, rebuilt only when series or legend config changes
    private static long s_lastLegendHash;
    private static List<LegendItemEntry> s_cachedItems = new();

    /// <summary>
    /// An entry in the resolved legend items list (either from series or custom items).
    /// </summary>
    internal struct LegendItemEntry
    {
        public ChartSeries? Series;
        public LegendCustomItem? Custom;
        public string Title;
        public IBrush ItemColor;
        public IBrush ItemLabelBrush;
        public LegendIcon Icon;
        public bool IsActive;
    }

    /// <summary>
    /// Render the legend onto the drawing context.
    /// </summary>
    public static void Render(
        DrawingContext context,
        ChartLegend legend,
        IReadOnlyList<ChartSeries> series,
        Rect plotArea,
        Color[]? defaultPalette,
        Func<string, IBrush?>? tryFindResource = null)
    {
        var labelBrush = legend.LabelBrush
            ?? tryFindResource?.Invoke("AuraForegroundSecondaryBrush")
            ?? Brushes.Gray;
        var swatchSize = legend.SwatchSize;
        var fontSize = legend.FontSize;
        var spacing = legend.ItemSpacing;
        var icon = legend.Icon;
        var inactiveColor = legend.InactiveColor
            ?? tryFindResource?.Invoke("AuraForegroundDisabledBrush")
            ?? new SolidColorBrush(Colors.Gray, 0.3);
        var inactiveLabelBrush = legend.InactiveLabelBrush
            ?? new SolidColorBrush(Colors.Gray, 0.5);

        // Rebuild items cache if series or legend config changed
        var currentHash = ComputeLegendHash(series, legend);
        if (currentHash != s_lastLegendHash)
        {
            s_lastLegendHash = currentHash;
            RebuildItemsCache(series, legend, icon, labelBrush, inactiveColor, inactiveLabelBrush, defaultPalette);
        }

        var items = s_cachedItems;
        if (items.Count == 0) return;

        var x = legend.LayoutRect.Left;
        var y = legend.LayoutRect.Top;
        int renderedCount = 0;
        int scrollOffset = legend.ScrollOffset;

        // Apply scroll offset
        int startIdx = 0;
        if (legend.IsScrollable && legend.MaxVisibleItems > 0)
        {
            startIdx = Math.Min(scrollOffset, Math.Max(0, items.Count - legend.MaxVisibleItems));
        }

        for (int i = startIdx; i < items.Count; i++)
        {
            // Stop if we've hit the max visible items limit
            if (legend.IsScrollable && legend.MaxVisibleItems > 0 && renderedCount >= legend.MaxVisibleItems)
                break;

            var entry = items[i];
            if (!entry.IsActive && entry.Custom == null && string.IsNullOrEmpty(entry.Title)) continue;

            var itemColor = entry.ItemColor;
            var itemLabelBrush = entry.ItemLabelBrush;
            var itemIcon = entry.Icon;
            var title = entry.Title;

            // Draw icon swatch based on LegendIcon type
            var swatchCenterX = x + swatchSize / 2;
            var swatchCenterY = y + fontSize / 2;
            var halfSwatch = swatchSize / 2;

            RenderSwatch(context, itemIcon, swatchCenterX, swatchCenterY, halfSwatch,
                x, y, fontSize, swatchSize, itemColor, legend.SwatchCornerRadius);

            // Label
            var formattedText = new FormattedText(title,
                s_culture,
                FlowDirection.LeftToRight,
                new Typeface(legend.TextFontFamily ?? "Segoe UI", FontStyle.Normal, legend.TextFontWeight),
                fontSize,
                itemLabelBrush);

            context.DrawText(formattedText, new Point(x + swatchSize + 4, y));

            x += swatchSize + 4 + formattedText.Width + spacing;
            renderedCount++;

            // Wrap to next line if needed
            if (x > legend.LayoutRect.Right - 50)
            {
                x = legend.LayoutRect.Left;
                y += fontSize + 4;
            }
        }

        // Render scroll indicators if scrollable
        if (legend.IsScrollable && legend.MaxVisibleItems > 0 && items.Count > legend.MaxVisibleItems)
        {
            RenderScrollIndicators(context, legend, items.Count, startIdx, labelBrush);
        }

        // Render selector buttons
        if (legend.ShowSelectAll || legend.ShowInverse)
        {
            RenderSelectorButtons(context, legend, fontSize, labelBrush);
        }
    }

    private static void RenderSwatch(
        DrawingContext context,
        LegendIcon itemIcon,
        double swatchCenterX, double swatchCenterY, double halfSwatch,
        double x, double y, double fontSize, double swatchSize,
        IBrush itemColor, double cornerRadius)
    {
        switch (itemIcon)
        {
            case LegendIcon.Circle:
                context.DrawEllipse(itemColor, null, new Point(swatchCenterX, swatchCenterY), halfSwatch, halfSwatch);
                break;
            case LegendIcon.RoundRect:
                var rrRect = new Rect(x, y + (fontSize - swatchSize) / 2, swatchSize, swatchSize);
                context.DrawRectangle(itemColor, null, rrRect, cornerRadius, cornerRadius);
                break;
            case LegendIcon.Triangle:
                DrawTriangleIcon(context, swatchCenterX, swatchCenterY, halfSwatch, itemColor);
                break;
            case LegendIcon.Diamond:
                DrawDiamondIcon(context, swatchCenterX, swatchCenterY, halfSwatch, itemColor);
                break;
            case LegendIcon.Pin:
                DrawPinIcon(context, swatchCenterX, swatchCenterY, halfSwatch, itemColor);
                break;
            case LegendIcon.None:
                break;
            case LegendIcon.Rect:
            default:
                var swatchRect = new Rect(x, y + (fontSize - swatchSize) / 2, swatchSize, swatchSize);
                context.DrawRectangle(itemColor, null, swatchRect);
                break;
        }
    }

    private static void RenderScrollIndicators(
        DrawingContext context, ChartLegend legend, int totalItems, int startIdx, IBrush labelBrush)
    {
        if (startIdx > 0)
        {
            var upText = new FormattedText("▲",
                s_culture,
                FlowDirection.LeftToRight, s_labelTypeface, 8, labelBrush);
            context.DrawText(upText, new Point(legend.LayoutRect.Right - 20, legend.LayoutRect.Top));
        }
        if (startIdx + legend.MaxVisibleItems < totalItems)
        {
            var downText = new FormattedText("▼",
                s_culture,
                FlowDirection.LeftToRight, s_labelTypeface, 8, labelBrush);
            context.DrawText(downText, new Point(legend.LayoutRect.Right - 20, legend.LayoutRect.Bottom - 12));
        }
    }

    private static void RenderSelectorButtons(
        DrawingContext context, ChartLegend legend, double fontSize, IBrush labelBrush)
    {
        var selectorX = legend.LayoutRect.Right - 80;
        var selectorY = legend.LayoutRect.Top;
        var selectorBrush = legend.SelectorBrush ?? labelBrush;
        var selectorFontSize = fontSize - 1;

        if (legend.ShowSelectAll)
        {
            var allText = new FormattedText("All",
                s_culture,
                FlowDirection.LeftToRight, s_labelTypeface, selectorFontSize, selectorBrush);
            context.DrawText(allText, new Point(selectorX, selectorY));
            selectorX += 44;
        }

        if (legend.ShowInverse)
        {
            var invText = new FormattedText("Inverse",
                s_culture,
                FlowDirection.LeftToRight, s_labelTypeface, selectorFontSize, selectorBrush);
            context.DrawText(invText, new Point(selectorX, selectorY));
        }
    }

    /// <summary>
    /// Compute a hash of the series and legend configuration to detect when
    /// the legend items cache needs rebuilding. O(n) in series count.
    /// </summary>
    private static long ComputeLegendHash(IReadOnlyList<ChartSeries> series, ChartLegend legend)
    {
        unchecked
        {
            long hash = 17;
            hash = hash * 31 + series.Count;
            for (int i = 0; i < series.Count; i++)
            {
                var s = series[i];
                hash = hash * 31 + (s.Title?.GetHashCode() ?? 0);
                hash = hash * 31 + (s.IsVisible ? 1 : 0);
            }
            hash = hash * 31 + (legend.CustomItems?.Count ?? 0);
            hash = hash * 31 + legend.LayoutRect.GetHashCode();
            hash = hash * 31 + legend.FontSize.GetHashCode();
            hash = hash * 31 + legend.SwatchSize.GetHashCode();
            hash = hash * 31 + legend.Icon.GetHashCode();
            hash = hash * 31 + legend.ScrollOffset;
            hash = hash * 31 + (legend.IsScrollable ? 1 : 0);
            hash = hash * 31 + legend.MaxVisibleItems;
            return hash;
        }
    }

    /// <summary>
    /// Rebuild the cached legend items list. Uses manual iteration instead of LINQ.
    /// </summary>
    private static void RebuildItemsCache(
        IReadOnlyList<ChartSeries> series,
        ChartLegend legend,
        LegendIcon defaultIcon,
        IBrush labelBrush,
        IBrush inactiveColor,
        IBrush inactiveLabelBrush,
        Color[]? defaultPalette)
    {
        s_cachedItems.Clear();

        if (legend.CustomItems != null && legend.CustomItems.Count > 0)
        {
            // Use custom items mapped to series by index
            var count = Math.Min(series.Count, legend.CustomItems.Count);
            for (int i = 0; i < count; i++)
            {
                var s = series[i];
                var custom = legend.CustomItems[i];
                s_cachedItems.Add(new LegendItemEntry
                {
                    Series = s,
                    Custom = custom,
                    Title = custom.Name,
                    IsActive = custom.IsActive,
                    ItemColor = custom.IsActive
                        ? (custom.Color ?? ResolveSeriesColor(s, defaultPalette))
                        : inactiveColor,
                    ItemLabelBrush = custom.IsActive ? labelBrush : inactiveLabelBrush,
                    Icon = custom.Icon ?? defaultIcon
                });
            }
        }
        else
        {
            // Auto-generate from series
            for (int i = 0; i < series.Count; i++)
            {
                var s = series[i];
                if (string.IsNullOrEmpty(s.Title)) continue;

                s_cachedItems.Add(new LegendItemEntry
                {
                    Series = s,
                    Custom = null,
                    Title = s.Title!,
                    IsActive = true,
                    ItemColor = ResolveSeriesColor(s, defaultPalette),
                    ItemLabelBrush = labelBrush,
                    Icon = defaultIcon
                });
            }
        }
    }

    private static IBrush ResolveSeriesColor(ChartSeries series, Color[]? defaultPalette)
    {
        if (series.Color is IBrush b)
            return b;

        var palette = defaultPalette ?? LineRenderer.DefaultPalette;
        return new SolidColorBrush(palette[series.SeriesIndex % palette.Length]);
    }

    // ────────────────────────────────────────────────
    //  Icon shape drawing helpers
    // ────────────────────────────────────────────────

    internal static void DrawTriangleIcon(DrawingContext context, double cx, double cy, double half, IBrush fill)
    {
        var geometry = new PathGeometry();
        var figure = new PathFigure { StartPoint = new Point(cx, cy - half), IsClosed = true };
        figure.Segments!.Add(new LineSegment { Point = new Point(cx + half, cy + half * 0.6) });
        figure.Segments!.Add(new LineSegment { Point = new Point(cx - half, cy + half * 0.6) });
        geometry.Figures!.Add(figure);
        context.DrawGeometry(fill, null, geometry);
    }

    internal static void DrawDiamondIcon(DrawingContext context, double cx, double cy, double half, IBrush fill)
    {
        var geometry = new PathGeometry();
        var figure = new PathFigure { StartPoint = new Point(cx, cy - half), IsClosed = true };
        figure.Segments!.Add(new LineSegment { Point = new Point(cx + half, cy) });
        figure.Segments!.Add(new LineSegment { Point = new Point(cx, cy + half) });
        figure.Segments!.Add(new LineSegment { Point = new Point(cx - half, cy) });
        geometry.Figures!.Add(figure);
        context.DrawGeometry(fill, null, geometry);
    }

    internal static void DrawPinIcon(DrawingContext context, double cx, double cy, double half, IBrush fill)
    {
        var geometry = new PathGeometry();
        var figure = new PathFigure { StartPoint = new Point(cx, cy + half), IsClosed = true };
        figure.Segments!.Add(new BezierSegment
        {
            Point1 = new Point(cx - half, cy),
            Point2 = new Point(cx - half, cy - half),
            Point3 = new Point(cx, cy - half)
        });
        figure.Segments!.Add(new BezierSegment
        {
            Point1 = new Point(cx + half, cy - half),
            Point2 = new Point(cx + half, cy),
            Point3 = new Point(cx, cy + half)
        });
        geometry.Figures!.Add(figure);
        context.DrawGeometry(fill, null, geometry);
    }

    /// <summary>
    /// Invalidate the legend items cache, forcing a rebuild on the next render.
    /// Call this when the series collection itself changes (not just data).
    /// </summary>
    internal static void InvalidateCache()
    {
        s_lastLegendHash = 0;
    }
}
