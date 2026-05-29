using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;

namespace AuraUI.Controls.Display;

/// <summary>
/// Specifies the status of a pipeline stage.
/// </summary>
public enum PipelineStageStatus
{
    Pending,
    Running,
    Success,
    Failed,
    Skipped
}

/// <summary>
/// Represents a single stage in a pipeline.
/// </summary>
public class PipelineStage
{
    /// <summary>
    /// Gets or sets the stage name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the stage status.
    /// </summary>
    public PipelineStageStatus Status { get; set; } = PipelineStageStatus.Pending;

    /// <summary>
    /// Gets or sets the stage duration.
    /// </summary>
    public TimeSpan Duration { get; set; }

    /// <summary>
    /// Gets or sets the log output for this stage.
    /// </summary>
    public string? Logs { get; set; }

    /// <summary>
    /// Gets the formatted duration string.
    /// </summary>
    public string FormattedDuration
    {
        get
        {
            if (Duration.TotalMinutes >= 1)
                return $"{(int)Duration.TotalMinutes}m {Duration.Seconds}s";
            return $"{Duration.TotalSeconds:F1}s";
        }
    }
}

/// <summary>
/// A visual pipeline viewer that displays connected stages with status indicators,
/// expandable details, and horizontal or vertical orientation.
/// </summary>
public class PipelineViewer : Control
{
    /// <summary>
    /// Defines the <see cref="Stages"/> styled property.
    /// </summary>
    public static readonly StyledProperty<ObservableCollection<PipelineStage>?> StagesProperty =
        AvaloniaProperty.Register<PipelineViewer, ObservableCollection<PipelineStage>?>(nameof(Stages));

    /// <summary>
    /// Defines the <see cref="Orientation"/> styled property.
    /// </summary>
    public static readonly StyledProperty<Orientation> OrientationProperty =
        AvaloniaProperty.Register<PipelineViewer, Orientation>(nameof(Orientation), Orientation.Horizontal);

    /// <summary>
    /// Defines the <see cref="StageSpacing"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> StageSpacingProperty =
        AvaloniaProperty.Register<PipelineViewer, double>(nameof(StageSpacing), 48);

    /// <summary>
    /// Defines the <see cref="ConnectorThickness"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> ConnectorThicknessProperty =
        AvaloniaProperty.Register<PipelineViewer, double>(nameof(ConnectorThickness), 2);

    /// <summary>
    /// Defines the <see cref="ConnectorBrush"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> ConnectorBrushProperty =
        AvaloniaProperty.Register<PipelineViewer, IBrush?>(nameof(ConnectorBrush));

    /// <summary>
    /// Defines the <see cref="StageNodeSize"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> StageNodeSizeProperty =
        AvaloniaProperty.Register<PipelineViewer, double>(nameof(StageNodeSize), 32);

    /// <summary>
    /// Defines the <see cref="SelectedStageIndex"/> styled property.
    /// </summary>
    public static readonly StyledProperty<int> SelectedStageIndexProperty =
        AvaloniaProperty.Register<PipelineViewer, int>(nameof(SelectedStageIndex), -1);

    static PipelineViewer()
    {
        StagesProperty.Changed.AddClassHandler<PipelineViewer>((x, _) => x.InvalidateVisual());
        OrientationProperty.Changed.AddClassHandler<PipelineViewer>((x, _) => x.InvalidateVisual());
        SelectedStageIndexProperty.Changed.AddClassHandler<PipelineViewer>((x, _) => x.InvalidateVisual());
    }

    /// <summary>
    /// Gets or sets the collection of pipeline stages.
    /// </summary>
    public ObservableCollection<PipelineStage>? Stages
    {
        get => GetValue(StagesProperty);
        set => SetValue(StagesProperty, value);
    }

    /// <summary>
    /// Gets or sets the layout orientation.
    /// </summary>
    public Orientation Orientation
    {
        get => GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }

    /// <summary>
    /// Gets or sets the spacing between stage nodes in pixels.
    /// </summary>
    public double StageSpacing
    {
        get => GetValue(StageSpacingProperty);
        set => SetValue(StageSpacingProperty, value);
    }

    /// <summary>
    /// Gets or sets the connector line thickness.
    /// </summary>
    public double ConnectorThickness
    {
        get => GetValue(ConnectorThicknessProperty);
        set => SetValue(ConnectorThicknessProperty, value);
    }

    /// <summary>
    /// Gets or sets the connector line brush.
    /// </summary>
    public IBrush? ConnectorBrush
    {
        get => GetValue(ConnectorBrushProperty);
        set => SetValue(ConnectorBrushProperty, value);
    }

    /// <summary>
    /// Gets or sets the stage node size.
    /// </summary>
    public double StageNodeSize
    {
        get => GetValue(StageNodeSizeProperty);
        set => SetValue(StageNodeSizeProperty, value);
    }

    /// <summary>
    /// Gets or sets the index of the selected/expanded stage (-1 for none).
    /// </summary>
    public int SelectedStageIndex
    {
        get => GetValue(SelectedStageIndexProperty);
        set => SetValue(SelectedStageIndexProperty, value);
    }

    /// <summary>
    /// Gets the color for a given pipeline stage status.
    /// </summary>
    public static IBrush GetStatusColor(PipelineStageStatus status) => status switch
    {
        PipelineStageStatus.Pending => new SolidColorBrush(Color.Parse("#BDBDBD")),
        PipelineStageStatus.Running => new SolidColorBrush(Color.Parse("#2196F3")),
        PipelineStageStatus.Success => new SolidColorBrush(Color.Parse("#4CAF50")),
        PipelineStageStatus.Failed => new SolidColorBrush(Color.Parse("#F44336")),
        PipelineStageStatus.Skipped => new SolidColorBrush(Color.Parse("#9E9E9E")),
        _ => Brushes.Gray
    };
}
