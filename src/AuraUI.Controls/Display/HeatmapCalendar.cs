using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace AuraUI.Controls.Display;

/// <summary>
/// A GitHub-style contribution heatmap calendar. Displays day cells colored by value
/// intensity with month and day labels.
/// </summary>
public class HeatmapCalendar : Control
{
    /// <summary>
    /// Defines the <see cref="Values"/> styled property.
    /// </summary>
    public static readonly StyledProperty<Dictionary<DateTime, double>?> ValuesProperty =
        AvaloniaProperty.Register<HeatmapCalendar, Dictionary<DateTime, double>?>(nameof(Values));

    /// <summary>
    /// Defines the <see cref="StartDate"/> styled property.
    /// </summary>
    public static readonly StyledProperty<DateTime> StartDateProperty =
        AvaloniaProperty.Register<HeatmapCalendar, DateTime>(nameof(StartDate));

    /// <summary>
    /// Defines the <see cref="EndDate"/> styled property.
    /// </summary>
    public static readonly StyledProperty<DateTime> EndDateProperty =
        AvaloniaProperty.Register<HeatmapCalendar, DateTime>(nameof(EndDate));

    /// <summary>
    /// Defines the <see cref="ColorScale"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IList<IBrush>?> ColorScaleProperty =
        AvaloniaProperty.Register<HeatmapCalendar, IList<IBrush>?>(nameof(ColorScale));

    /// <summary>
    /// Defines the <see cref="CellSize"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> CellSizeProperty =
        AvaloniaProperty.Register<HeatmapCalendar, double>(nameof(CellSize), 12);

    /// <summary>
    /// Defines the <see cref="CellSpacing"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> CellSpacingProperty =
        AvaloniaProperty.Register<HeatmapCalendar, double>(nameof(CellSpacing), 2);

    /// <summary>
    /// Defines the <see cref="ShowMonthLabels"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> ShowMonthLabelsProperty =
        AvaloniaProperty.Register<HeatmapCalendar, bool>(nameof(ShowMonthLabels), true);

    /// <summary>
    /// Defines the <see cref="ShowDayLabels"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> ShowDayLabelsProperty =
        AvaloniaProperty.Register<HeatmapCalendar, bool>(nameof(ShowDayLabels), true);

    /// <summary>
    /// Defines the <see cref="EmptyColor"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> EmptyColorProperty =
        AvaloniaProperty.Register<HeatmapCalendar, IBrush?>(nameof(EmptyColor));

    /// <summary>
    /// Defines the <see cref="MinValue"/> styled property. NaN = auto.
    /// </summary>
    public static readonly StyledProperty<double> MinValueProperty =
        AvaloniaProperty.Register<HeatmapCalendar, double>(nameof(MinValue), double.NaN);

    /// <summary>
    /// Defines the <see cref="MaxValue"/> styled property. NaN = auto.
    /// </summary>
    public static readonly StyledProperty<double> MaxValueProperty =
        AvaloniaProperty.Register<HeatmapCalendar, double>(nameof(MaxValue), double.NaN);

    // Default GitHub-style color scale
    private static readonly IBrush[] s_defaultColorScale =
    {
        new SolidColorBrush(Color.Parse("#ebedf0")), // empty
        new SolidColorBrush(Color.Parse("#9be9a8")), // level 1
        new SolidColorBrush(Color.Parse("#40c463")), // level 2
        new SolidColorBrush(Color.Parse("#30a14e")), // level 3
        new SolidColorBrush(Color.Parse("#216e39")), // level 4
    };

    private static readonly SolidColorBrush s_defaultEmpty = new(Color.Parse("#ebedf0"));
    private static readonly Typeface s_labelTypeface = new("Segoe UI", FontStyle.Normal, FontWeight.Normal);

    static HeatmapCalendar()
    {
        ValuesProperty.Changed.AddClassHandler<HeatmapCalendar>((x, _) => x.InvalidateVisual());
        StartDateProperty.Changed.AddClassHandler<HeatmapCalendar>((x, _) => x.InvalidateVisual());
        EndDateProperty.Changed.AddClassHandler<HeatmapCalendar>((x, _) => x.InvalidateVisual());
        ColorScaleProperty.Changed.AddClassHandler<HeatmapCalendar>((x, _) => x.InvalidateVisual());
        CellSizeProperty.Changed.AddClassHandler<HeatmapCalendar>((x, _) => x.InvalidateVisual());
        CellSpacingProperty.Changed.AddClassHandler<HeatmapCalendar>((x, _) => x.InvalidateVisual());
        ShowMonthLabelsProperty.Changed.AddClassHandler<HeatmapCalendar>((x, _) => x.InvalidateVisual());
        ShowDayLabelsProperty.Changed.AddClassHandler<HeatmapCalendar>((x, _) => x.InvalidateVisual());
        EmptyColorProperty.Changed.AddClassHandler<HeatmapCalendar>((x, _) => x.InvalidateVisual());
    }

    /// <summary>
    /// Gets or sets the values keyed by date.
    /// </summary>
    public Dictionary<DateTime, double>? Values
    {
        get => GetValue(ValuesProperty);
        set => SetValue(ValuesProperty, value);
    }

    /// <summary>
    /// Gets or sets the start date.
    /// </summary>
    public DateTime StartDate
    {
        get => GetValue(StartDateProperty);
        set => SetValue(StartDateProperty, value);
    }

    /// <summary>
    /// Gets or sets the end date.
    /// </summary>
    public DateTime EndDate
    {
        get => GetValue(EndDateProperty);
        set => SetValue(EndDateProperty, value);
    }

    /// <summary>
    /// Gets or sets the color scale (from empty to maximum).
    /// </summary>
    public IList<IBrush>? ColorScale
    {
        get => GetValue(ColorScaleProperty);
        set => SetValue(ColorScaleProperty, value);
    }

    /// <summary>
    /// Gets or sets the cell size in pixels.
    /// </summary>
    public double CellSize
    {
        get => GetValue(CellSizeProperty);
        set => SetValue(CellSizeProperty, value);
    }

    /// <summary>
    /// Gets or sets the spacing between cells.
    /// </summary>
    public double CellSpacing
    {
        get => GetValue(CellSpacingProperty);
        set => SetValue(CellSpacingProperty, value);
    }

    /// <summary>
    /// Gets or sets whether to show month labels.
    /// </summary>
    public bool ShowMonthLabels
    {
        get => GetValue(ShowMonthLabelsProperty);
        set => SetValue(ShowMonthLabelsProperty, value);
    }

    /// <summary>
    /// Gets or sets whether to show day-of-week labels.
    /// </summary>
    public bool ShowDayLabels
    {
        get => GetValue(ShowDayLabelsProperty);
        set => SetValue(ShowDayLabelsProperty, value);
    }

    /// <summary>
    /// Gets or sets the color for empty cells.
    /// </summary>
    public IBrush? EmptyColor
    {
        get => GetValue(EmptyColorProperty);
        set => SetValue(EmptyColorProperty, value);
    }

    /// <summary>
    /// Gets or sets the minimum value for the color scale. NaN = auto.
    /// </summary>
    public double MinValue
    {
        get => GetValue(MinValueProperty);
        set => SetValue(MinValueProperty, value);
    }

    /// <summary>
    /// Gets or sets the maximum value for the color scale. NaN = auto.
    /// </summary>
    public double MaxValue
    {
        get => GetValue(MaxValueProperty);
        set => SetValue(MaxValueProperty, value);
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        var bounds = new Rect(Bounds.Size);
        if (bounds.Width <= 0 || bounds.Height <= 0) return;

        var startDate = StartDate.Date;
        var endDate = EndDate.Date;
        if (endDate <= startDate) return;

        var values = Values;
        var cellSize = CellSize;
        var spacing = CellSpacing;
        var step = cellSize + spacing;

        // Auto-calculate min/max from values
        double dataMin = 0, dataMax = 0;
        if (values != null && values.Count > 0)
        {
            dataMin = double.MaxValue;
            dataMax = double.MinValue;
            foreach (var kv in values)
            {
                if (kv.Value < dataMin) dataMin = kv.Value;
                if (kv.Value > dataMax) dataMax = kv.Value;
            }
        }
        var scaleMin = double.IsNaN(MinValue) ? dataMin : MinValue;
        var scaleMax = double.IsNaN(MaxValue) ? dataMax : MaxValue;
        var scaleRange = scaleMax - scaleMin;
        if (scaleRange < 1e-10) scaleRange = 1;

        var colorScale = ColorScale ?? s_defaultColorScale;
        var emptyColor = EmptyColor ?? s_defaultEmpty;

        // Layout offsets
        var dayLabelWidth = ShowDayLabels ? 30.0 : 0;
        var monthLabelHeight = ShowMonthLabels ? 18.0 : 0;
        var gridLeft = bounds.X + dayLabelWidth;
        var gridTop = bounds.Y + monthLabelHeight;

        // Day-of-week labels (Mon, Wed, Fri)
        if (ShowDayLabels)
        {
            var dayLabels = new[] { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };
            for (var d = 0; d < 7; d++)
            {
                if (d % 2 == 1) // Show Mon, Wed, Fri
                {
                    var label = new FormattedText(dayLabels[d], CultureInfo.CurrentCulture, FlowDirection.LeftToRight,
                        s_labelTypeface, 9, Brushes.Gray);
                    context.DrawText(label, new Point(bounds.X, gridTop + d * step + (cellSize - label.Height) / 2));
                }
            }
        }

        // Iterate through weeks
        var current = startDate;
        // Align to start of week (Sunday)
        while (current.DayOfWeek != DayOfWeek.Sunday && current > startDate.AddDays(-7))
            current = current.AddDays(-1);

        var weekIndex = 0;
        var lastMonth = -1;

        while (current <= endDate.AddDays(6))
        {
            for (var dayOfWeek = 0; dayOfWeek < 7; dayOfWeek++)
            {
                var date = current.AddDays(dayOfWeek);
                if (date < startDate || date > endDate) continue;

                var x = gridLeft + weekIndex * step;
                var y = gridTop + dayOfWeek * step;

                // Month label
                if (ShowMonthLabels && dayOfWeek == 0 && date.Month != lastMonth)
                {
                    lastMonth = date.Month;
                    var monthName = date.ToString("MMM", CultureInfo.CurrentCulture);
                    var monthLabel = new FormattedText(monthName, CultureInfo.CurrentCulture, FlowDirection.LeftToRight,
                        s_labelTypeface, 9, Brushes.Gray);
                    context.DrawText(monthLabel, new Point(x, bounds.Y));
                }

                // Determine color
                IBrush cellColor;
                if (values != null && values.TryGetValue(date, out var value))
                {
                    var normalized = Math.Clamp((value - scaleMin) / scaleRange, 0, 1);
                    var colorIndex = (int)Math.Round(normalized * (colorScale.Count - 1));
                    colorIndex = Math.Clamp(colorIndex, 0, colorScale.Count - 1);
                    cellColor = colorScale[colorIndex];
                }
                else
                {
                    cellColor = emptyColor;
                }

                // Draw cell (rounded rect)
                var cellRect = new Rect(x, y, cellSize, cellSize);
                context.DrawRectangle(cellColor, null, cellRect, 2, 2);
            }

            current = current.AddDays(7);
            weekIndex++;
        }
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        var startDate = StartDate.Date;
        var endDate = EndDate.Date;
        var cellSize = CellSize;
        var spacing = CellSpacing;
        var step = cellSize + spacing;

        var dayLabelWidth = ShowDayLabels ? 30.0 : 0;
        var monthLabelHeight = ShowMonthLabels ? 18.0 : 0;

        if (endDate <= startDate)
            return new Size(dayLabelWidth + step, monthLabelHeight + 7 * step);

        // Calculate number of weeks
        var totalDays = (endDate - startDate).Days + 1;
        var weeks = (totalDays / 7) + 2; // extra for alignment

        var width = dayLabelWidth + weeks * step;
        var height = monthLabelHeight + 7 * step;

        if (double.IsInfinity(availableSize.Width))
            return new Size(width, height);

        return new Size(Math.Min(width, availableSize.Width), Math.Min(height, availableSize.Height));
    }
}
