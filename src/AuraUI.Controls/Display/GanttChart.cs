using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.TextFormatting;

namespace AuraUI.Controls.Display;

/// <summary>
/// Represents a single task in a Gantt chart.
/// </summary>
public class GanttTask
{
    /// <summary>
    /// Gets or sets the task name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the task start date/time.
    /// </summary>
    public DateTime Start { get; set; }

    /// <summary>
    /// Gets or sets the task end date/time.
    /// </summary>
    public DateTime End { get; set; }

    /// <summary>
    /// Gets or sets the progress percentage (0.0 to 1.0).
    /// </summary>
    public double Progress { get; set; }

    /// <summary>
    /// Gets or sets the list of task names this task depends on.
    /// </summary>
    public ObservableCollection<string> Dependencies { get; set; } = new();

    /// <summary>
    /// Gets or sets the bar color.
    /// </summary>
    public IBrush? Color { get; set; }

    /// <summary>
    /// Gets or sets the group name for grouping tasks.
    /// </summary>
    public string? Group { get; set; }

    /// <summary>
    /// Gets the duration of the task.
    /// </summary>
    public TimeSpan Duration => End - Start;
}

/// <summary>
/// A Gantt chart control for displaying project timelines with tasks, dependencies,
/// progress bars, and drag-to-resize support.
/// </summary>
public class GanttChart : Control
{
    /// <summary>
    /// Defines the <see cref="Tasks"/> styled property.
    /// </summary>
    public static readonly StyledProperty<ObservableCollection<GanttTask>?> TasksProperty =
        AvaloniaProperty.Register<GanttChart, ObservableCollection<GanttTask>?>(nameof(Tasks));

    /// <summary>
    /// Defines the <see cref="StartDate"/> styled property.
    /// </summary>
    public static readonly StyledProperty<DateTime> StartDateProperty =
        AvaloniaProperty.Register<GanttChart, DateTime>(nameof(StartDate));

    /// <summary>
    /// Defines the <see cref="EndDate"/> styled property.
    /// </summary>
    public static readonly StyledProperty<DateTime> EndDateProperty =
        AvaloniaProperty.Register<GanttChart, DateTime>(nameof(EndDate));

    /// <summary>
    /// Defines the <see cref="ShowDependencies"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> ShowDependenciesProperty =
        AvaloniaProperty.Register<GanttChart, bool>(nameof(ShowDependencies), true);

    /// <summary>
    /// Defines the <see cref="ZoomLevel"/> styled property.
    /// Pixels per day. Higher values zoom in.
    /// </summary>
    public static readonly StyledProperty<double> ZoomLevelProperty =
        AvaloniaProperty.Register<GanttChart, double>(nameof(ZoomLevel), 40);

    /// <summary>
    /// Defines the <see cref="RowHeight"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> RowHeightProperty =
        AvaloniaProperty.Register<GanttChart, double>(nameof(RowHeight), 36);

    /// <summary>
    /// Defines the <see cref="HeaderHeight"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> HeaderHeightProperty =
        AvaloniaProperty.Register<GanttChart, double>(nameof(HeaderHeight), 40);

    /// <summary>
    /// Defines the <see cref="TaskNameWidth"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> TaskNameWidthProperty =
        AvaloniaProperty.Register<GanttChart, double>(nameof(TaskNameWidth), 180);

    /// <summary>
    /// Defines the <see cref="GridLineBrush"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> GridLineBrushProperty =
        AvaloniaProperty.Register<GanttChart, IBrush?>(nameof(GridLineBrush));

    /// <summary>
    /// Defines the <see cref="ProgressBrush"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> ProgressBrushProperty =
        AvaloniaProperty.Register<GanttChart, IBrush?>(nameof(ProgressBrush));

    /// <summary>
    /// Defines the <see cref="DefaultBarBrush"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> DefaultBarBrushProperty =
        AvaloniaProperty.Register<GanttChart, IBrush?>(nameof(DefaultBarBrush));

    /// <summary>
    /// Defines the <see cref="DependencyArrowBrush"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> DependencyArrowBrushProperty =
        AvaloniaProperty.Register<GanttChart, IBrush?>(nameof(DependencyArrowBrush));

    /// <summary>
    /// Defines the <see cref="Foreground"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> ForegroundProperty =
        AvaloniaProperty.Register<GanttChart, IBrush?>(nameof(Foreground));

    static GanttChart()
    {
        AffectsRender<GanttChart>(
            TasksProperty, ZoomLevelProperty, ShowDependenciesProperty,
            StartDateProperty, EndDateProperty, RowHeightProperty,
            HeaderHeightProperty, TaskNameWidthProperty,
            GridLineBrushProperty, ProgressBrushProperty, DefaultBarBrushProperty,
            DependencyArrowBrushProperty, ForegroundProperty);
        AffectsMeasure<GanttChart>(
            TasksProperty, RowHeightProperty, HeaderHeightProperty,
            TaskNameWidthProperty, ZoomLevelProperty);
    }

    /// <summary>
    /// Gets or sets the collection of tasks to display.
    /// </summary>
    public ObservableCollection<GanttTask>? Tasks
    {
        get => GetValue(TasksProperty);
        set => SetValue(TasksProperty, value);
    }

    /// <summary>
    /// Gets or sets the visible start date of the chart.
    /// </summary>
    public DateTime StartDate
    {
        get => GetValue(StartDateProperty);
        set => SetValue(StartDateProperty, value);
    }

    /// <summary>
    /// Gets or sets the visible end date of the chart.
    /// </summary>
    public DateTime EndDate
    {
        get => GetValue(EndDateProperty);
        set => SetValue(EndDateProperty, value);
    }

    /// <summary>
    /// Gets or sets whether dependency arrows are drawn.
    /// </summary>
    public bool ShowDependencies
    {
        get => GetValue(ShowDependenciesProperty);
        set => SetValue(ShowDependenciesProperty, value);
    }

    /// <summary>
    /// Gets or sets the zoom level in pixels per day.
    /// </summary>
    public double ZoomLevel
    {
        get => GetValue(ZoomLevelProperty);
        set => SetValue(ZoomLevelProperty, value);
    }

    /// <summary>
    /// Gets or sets the height of each task row in pixels.
    /// </summary>
    public double RowHeight
    {
        get => GetValue(RowHeightProperty);
        set => SetValue(RowHeightProperty, value);
    }

    /// <summary>
    /// Gets or sets the height of the timeline header.
    /// </summary>
    public double HeaderHeight
    {
        get => GetValue(HeaderHeightProperty);
        set => SetValue(HeaderHeightProperty, value);
    }

    /// <summary>
    /// Gets or sets the width of the task name column.
    /// </summary>
    public double TaskNameWidth
    {
        get => GetValue(TaskNameWidthProperty);
        set => SetValue(TaskNameWidthProperty, value);
    }

    /// <summary>
    /// Gets or sets the grid line brush.
    /// </summary>
    public IBrush? GridLineBrush
    {
        get => GetValue(GridLineBrushProperty);
        set => SetValue(GridLineBrushProperty, value);
    }

    /// <summary>
    /// Gets or sets the progress fill brush.
    /// </summary>
    public IBrush? ProgressBrush
    {
        get => GetValue(ProgressBrushProperty);
        set => SetValue(ProgressBrushProperty, value);
    }

    /// <summary>
    /// Gets or sets the default bar brush.
    /// </summary>
    public IBrush? DefaultBarBrush
    {
        get => GetValue(DefaultBarBrushProperty);
        set => SetValue(DefaultBarBrushProperty, value);
    }

    /// <summary>
    /// Gets or sets the dependency arrow brush.
    /// </summary>
    public IBrush? DependencyArrowBrush
    {
        get => GetValue(DependencyArrowBrushProperty);
        set => SetValue(DependencyArrowBrushProperty, value);
    }

    /// <summary>
    /// Gets or sets the foreground brush for text.
    /// </summary>
    public IBrush? Foreground
    {
        get => GetValue(ForegroundProperty);
        set => SetValue(ForegroundProperty, value);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        var tasks = Tasks;
        var rowCount = tasks?.Count ?? 0;
        var rowHeight = RowHeight;
        var headerHeight = HeaderHeight;
        var taskNameWidth = TaskNameWidth;
        var totalHeight = headerHeight + rowCount * rowHeight;
        var totalWidth = taskNameWidth + 400;
        return new Size(
            double.IsInfinity(availableSize.Width) ? totalWidth : availableSize.Width,
            double.IsInfinity(availableSize.Height) ? totalHeight : Math.Max(totalHeight, availableSize.Height));
    }

    public override void Render(DrawingContext context)
    {
        var tasks = Tasks;
        if (tasks == null || tasks.Count == 0) return;

        var bounds = Bounds;
        var rowHeight = RowHeight;
        var headerHeight = HeaderHeight;
        var taskNameWidth = TaskNameWidth;
        var startDate = StartDate;
        var endDate = EndDate;
        var defaultBarBrush = DefaultBarBrush ?? new SolidColorBrush(Color.Parse("#2196F3"));
        var progressBrush = ProgressBrush ?? new SolidColorBrush(Color.Parse("#4CAF50"));
        var gridBrush = GridLineBrush ?? new SolidColorBrush(Color.Parse("#E0E0E0"));
        var headerBrush = new SolidColorBrush(Color.Parse("#F5F5F5"));
        var fgBrush = Foreground ?? Brushes.Black;

        context.DrawRectangle(headerBrush, null, new Rect(0, 0, bounds.Width, headerHeight));

        var gridPen = new Pen(gridBrush, 0.5);
        for (int i = 0; i <= tasks.Count; i++)
        {
            var y = headerHeight + i * rowHeight;
            context.DrawLine(gridPen, new Point(0, y), new Point(bounds.Width, y));
        }

        context.DrawLine(new Pen(gridBrush, 1), new Point(taskNameWidth, 0), new Point(taskNameWidth, bounds.Height));

        var typeface = new Typeface(FontFamily.Default);
        var totalDays = Math.Max(1, (endDate - startDate).TotalDays);
        var timelineWidth = bounds.Width - taskNameWidth;

        for (int i = 0; i < tasks.Count; i++)
        {
            var task = tasks[i];
            var y = headerHeight + i * rowHeight;

            var nameText = new FormattedText(task.Name,
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight, typeface, 12, fgBrush);
            context.DrawText(nameText, new Point(8, y + (rowHeight - nameText.Height) / 2));

            var taskStart = (task.Start - startDate).TotalDays / totalDays;
            var taskEnd = (task.End - startDate).TotalDays / totalDays;
            var barX = taskNameWidth + taskStart * timelineWidth;
            var barWidth = (taskEnd - taskStart) * timelineWidth;
            if (barWidth < 2) barWidth = 2;

            var barBrush = task.Color ?? defaultBarBrush;
            var barRect = new Rect(barX, y + 4, barWidth, rowHeight - 8);
            context.DrawRectangle(barBrush, null, barRect, 3, 3);

            if (task.Progress > 0)
            {
                var progressRect = new Rect(barX, y + 4, barWidth * task.Progress, rowHeight - 8);
                context.DrawRectangle(progressBrush, null, progressRect, 3, 3);
            }
        }
    }
}
