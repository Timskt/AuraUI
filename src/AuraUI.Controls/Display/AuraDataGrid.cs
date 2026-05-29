using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace AuraUI.Controls.Display;

/// <summary>
/// An enhanced DataGrid with customizable alternating row backgrounds, row hover/selected states,
/// header styling, grid lines, row height, corner radius, and virtualization support.
/// </summary>
public class AuraDataGrid : Avalonia.Controls.DataGrid
{
    /// <summary>
    /// Defines the <see cref="AlternatingRowBackground"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> AlternatingRowBackgroundProperty =
        AvaloniaProperty.Register<AuraDataGrid, IBrush?>(nameof(AlternatingRowBackground));

    /// <summary>
    /// Defines the <see cref="RowHoverBackground"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> RowHoverBackgroundProperty =
        AvaloniaProperty.Register<AuraDataGrid, IBrush?>(nameof(RowHoverBackground));

    /// <summary>
    /// Defines the <see cref="RowSelectedBackground"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> RowSelectedBackgroundProperty =
        AvaloniaProperty.Register<AuraDataGrid, IBrush?>(nameof(RowSelectedBackground));

    /// <summary>
    /// Defines the <see cref="HeaderBackground"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> HeaderBackgroundProperty =
        AvaloniaProperty.Register<AuraDataGrid, IBrush?>(nameof(HeaderBackground));

    /// <summary>
    /// Defines the <see cref="HeaderForeground"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> HeaderForegroundProperty =
        AvaloniaProperty.Register<AuraDataGrid, IBrush?>(nameof(HeaderForeground));

    /// <summary>
    /// Defines the <see cref="GridLineBrush"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> GridLineBrushProperty =
        AvaloniaProperty.Register<AuraDataGrid, IBrush?>(nameof(GridLineBrush));

    /// <summary>
    /// Defines the <see cref="RowHeight"/> styled property.
    /// </summary>
    public new static readonly StyledProperty<double> RowHeightProperty =
        AvaloniaProperty.Register<AuraDataGrid, double>(nameof(RowHeight), 36);

    /// <summary>
    /// Defines the <see cref="GridCornerRadius"/> styled property.
    /// </summary>
    public static readonly StyledProperty<CornerRadius> GridCornerRadiusProperty =
        AvaloniaProperty.Register<AuraDataGrid, CornerRadius>(nameof(GridCornerRadius), new CornerRadius(8));

    /// <summary>
    /// Defines the <see cref="ShowVerticalLines"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> ShowVerticalLinesProperty =
        AvaloniaProperty.Register<AuraDataGrid, bool>(nameof(ShowVerticalLines), true);

    /// <summary>
    /// Defines the <see cref="ShowHorizontalLines"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> ShowHorizontalLinesProperty =
        AvaloniaProperty.Register<AuraDataGrid, bool>(nameof(ShowHorizontalLines), true);

    /// <summary>
    /// Defines the <see cref="IsVirtualized"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsVirtualizedProperty =
        AvaloniaProperty.Register<AuraDataGrid, bool>(nameof(IsVirtualized), true);

    static AuraDataGrid()
    {
        AlternatingRowBackgroundProperty.Changed.AddClassHandler<AuraDataGrid>((x, _) => x.InvalidateVisual());
    }

    /// <summary>
    /// Gets or sets the background for alternating (odd) rows.
    /// </summary>
    public IBrush? AlternatingRowBackground
    {
        get => GetValue(AlternatingRowBackgroundProperty);
        set => SetValue(AlternatingRowBackgroundProperty, value);
    }

    /// <summary>
    /// Gets or sets the background shown when hovering over a row.
    /// </summary>
    public IBrush? RowHoverBackground
    {
        get => GetValue(RowHoverBackgroundProperty);
        set => SetValue(RowHoverBackgroundProperty, value);
    }

    /// <summary>
    /// Gets or sets the background for selected rows.
    /// </summary>
    public IBrush? RowSelectedBackground
    {
        get => GetValue(RowSelectedBackgroundProperty);
        set => SetValue(RowSelectedBackgroundProperty, value);
    }

    /// <summary>
    /// Gets or sets the background brush for the column header row.
    /// </summary>
    public IBrush? HeaderBackground
    {
        get => GetValue(HeaderBackgroundProperty);
        set => SetValue(HeaderBackgroundProperty, value);
    }

    /// <summary>
    /// Gets or sets the foreground brush for column header text.
    /// </summary>
    public IBrush? HeaderForeground
    {
        get => GetValue(HeaderForegroundProperty);
        set => SetValue(HeaderForegroundProperty, value);
    }

    /// <summary>
    /// Gets or sets the brush used to draw grid lines between cells and rows.
    /// </summary>
    public IBrush? GridLineBrush
    {
        get => GetValue(GridLineBrushProperty);
        set => SetValue(GridLineBrushProperty, value);
    }

    /// <summary>
    /// Gets or sets the height of each data row.
    /// </summary>
    public new double RowHeight
    {
        get => GetValue(RowHeightProperty);
        set => SetValue(RowHeightProperty, value);
    }

    /// <summary>
    /// Gets or sets the corner radius for the DataGrid border.
    /// </summary>
    public CornerRadius GridCornerRadius
    {
        get => GetValue(GridCornerRadiusProperty);
        set => SetValue(GridCornerRadiusProperty, value);
    }

    /// <summary>
    /// Gets or sets whether vertical grid lines between columns are shown.
    /// </summary>
    public bool ShowVerticalLines
    {
        get => GetValue(ShowVerticalLinesProperty);
        set => SetValue(ShowVerticalLinesProperty, value);
    }

    /// <summary>
    /// Gets or sets whether horizontal grid lines between rows are shown.
    /// </summary>
    public bool ShowHorizontalLines
    {
        get => GetValue(ShowHorizontalLinesProperty);
        set => SetValue(ShowHorizontalLinesProperty, value);
    }

    /// <summary>
    /// Gets or sets whether UI virtualization is enabled for large data sets.
    /// </summary>
    public bool IsVirtualized
    {
        get => GetValue(IsVirtualizedProperty);
        set => SetValue(IsVirtualizedProperty, value);
    }
}
