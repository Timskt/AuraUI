using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

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

    static GanttChart()
    {
        TasksProperty.Changed.AddClassHandler<GanttChart>((x, _) => x.InvalidateVisual());
        ZoomLevelProperty.Changed.AddClassHandler<GanttChart>((x, _) => x.InvalidateVisual());
        ShowDependenciesProperty.Changed.AddClassHandler<GanttChart>((x, _) => x.InvalidateVisual());
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
}
