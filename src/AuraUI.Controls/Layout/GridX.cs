using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace AuraUI.Controls.Layout;

/// <summary>
/// Specifies which grid lines are visible.
/// </summary>
[Flags]
public enum GridLinesVisibility
{
    None = 0,
    Horizontal = 1,
    Vertical = 2,
    Both = Horizontal | Vertical
}

/// <summary>
/// An extended <see cref="Grid"/> that renders visible grid lines.
/// Useful for debugging layouts and form-based layouts.
/// </summary>
public class GridX : Grid
{
    /// <summary>
    /// Defines the <see cref="HorizontalGridLinesBrush"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> HorizontalGridLinesBrushProperty =
        AvaloniaProperty.Register<GridX, IBrush?>(nameof(HorizontalGridLinesBrush));

    /// <summary>
    /// Defines the <see cref="VerticalGridLinesBrush"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> VerticalGridLinesBrushProperty =
        AvaloniaProperty.Register<GridX, IBrush?>(nameof(VerticalGridLinesBrush));

    /// <summary>
    /// Defines the <see cref="GridLinesVisibility"/> styled property.
    /// </summary>
    public static readonly StyledProperty<GridLinesVisibility> GridLinesVisibilityProperty =
        AvaloniaProperty.Register<GridX, GridLinesVisibility>(
            nameof(GridLinesVisibility),
            GridLinesVisibility.Both);

    /// <summary>
    /// Defines the <see cref="GridLineThickness"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> GridLineThicknessProperty =
        AvaloniaProperty.Register<GridX, double>(
            nameof(GridLineThickness),
            1.0);

    /// <summary>
    /// Defines the <see cref="GridLineDashArray"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double[]?> GridLineDashArrayProperty =
        AvaloniaProperty.Register<GridX, double[]?>(nameof(GridLineDashArray));

    static GridX()
    {
        AffectsRender<GridX>(
            HorizontalGridLinesBrushProperty,
            VerticalGridLinesBrushProperty,
            GridLinesVisibilityProperty,
            GridLineThicknessProperty,
            GridLineDashArrayProperty);
    }

    /// <summary>
    /// Gets or sets the brush for horizontal grid lines.
    /// </summary>
    public IBrush? HorizontalGridLinesBrush
    {
        get => GetValue(HorizontalGridLinesBrushProperty);
        set => SetValue(HorizontalGridLinesBrushProperty, value);
    }

    /// <summary>
    /// Gets or sets the brush for vertical grid lines.
    /// </summary>
    public IBrush? VerticalGridLinesBrush
    {
        get => GetValue(VerticalGridLinesBrushProperty);
        set => SetValue(VerticalGridLinesBrushProperty, value);
    }

    /// <summary>
    /// Gets or sets which grid lines are visible.
    /// </summary>
    public GridLinesVisibility GridLinesVisibility
    {
        get => GetValue(GridLinesVisibilityProperty);
        set => SetValue(GridLinesVisibilityProperty, value);
    }

    /// <summary>
    /// Gets or sets the thickness of grid lines.
    /// </summary>
    public double GridLineThickness
    {
        get => GetValue(GridLineThicknessProperty);
        set => SetValue(GridLineThicknessProperty, value);
    }

    /// <summary>
    /// Gets or sets the dash pattern for grid lines. Null for solid lines.
    /// </summary>
    public double[]? GridLineDashArray
    {
        get => GetValue(GridLineDashArrayProperty);
        set => SetValue(GridLineDashArrayProperty, value);
    }

    public new void Render(DrawingContext context)
    {
        base.Render(context);

        var visibility = GridLinesVisibility;
        if (visibility == GridLinesVisibility.None) return;

        var bounds = new Rect(Bounds.Size);
        if (bounds.Width <= 0 || bounds.Height <= 0) return;

        var thickness = GridLineThickness;
        var halfThickness = thickness / 2.0;

        // Draw horizontal lines at row boundaries
        if ((visibility & GridLinesVisibility.Horizontal) != 0 && RowDefinitions.Count > 0)
        {
            var brush = HorizontalGridLinesBrush ?? Brushes.LightGray;
            var pen = new Pen(brush, thickness, new DashStyle(GridLineDashArray ?? Array.Empty<double>(), 0));
            var y = 0.0;

            foreach (var row in RowDefinitions)
            {
                y += row.ActualHeight;
                if (y > bounds.Height) break;
                context.DrawLine(pen, new Point(0, y - halfThickness), new Point(bounds.Width, y - halfThickness));
            }
        }

        // Draw vertical lines at column boundaries
        if ((visibility & GridLinesVisibility.Vertical) != 0 && ColumnDefinitions.Count > 0)
        {
            var brush = VerticalGridLinesBrush ?? Brushes.LightGray;
            var pen = new Pen(brush, thickness, new DashStyle(GridLineDashArray ?? Array.Empty<double>(), 0));
            var x = 0.0;

            foreach (var col in ColumnDefinitions)
            {
                x += col.ActualWidth;
                if (x > bounds.Width) break;
                context.DrawLine(pen, new Point(x - halfThickness, 0), new Point(x - halfThickness, bounds.Height));
            }
        }
    }
}
