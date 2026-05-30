using Avalonia;
using Avalonia.Controls.Templates;
using Avalonia.Data;

namespace AuraUI.Controls.Display;

/// <summary>
/// Defines a column for <see cref="AuraDataGrid"/> with sorting, filtering, resizing,
/// reordering, frozen-column, and template support.
/// </summary>
public class DataGridColumn : AvaloniaObject
{
    /// <summary>
    /// Defines the <see cref="Header"/> property.
    /// </summary>
    public static readonly StyledProperty<string?> HeaderProperty =
        AvaloniaProperty.Register<DataGridColumn, string?>(nameof(Header));

    /// <summary>
    /// Defines the <see cref="Binding"/> property.
    /// </summary>
    public static readonly StyledProperty<BindingBase?> BindingProperty =
        AvaloniaProperty.Register<DataGridColumn, BindingBase?>(nameof(Binding));

    /// <summary>
    /// Defines the <see cref="PropertyName"/> property.
    /// The name of the property on the data model, used for sorting, filtering, and auto-generation.
    /// </summary>
    public static readonly StyledProperty<string?> PropertyNameProperty =
        AvaloniaProperty.Register<DataGridColumn, string?>(nameof(PropertyName));

    /// <summary>
    /// Defines the <see cref="Width"/> property.
    /// </summary>
    public static readonly StyledProperty<double> WidthProperty =
        AvaloniaProperty.Register<DataGridColumn, double>(nameof(Width), double.NaN);

    /// <summary>
    /// Defines the <see cref="MinWidth"/> property.
    /// </summary>
    public static readonly StyledProperty<double> MinWidthProperty =
        AvaloniaProperty.Register<DataGridColumn, double>(nameof(MinWidth), 40);

    /// <summary>
    /// Defines the <see cref="MaxWidth"/> property.
    /// </summary>
    public static readonly StyledProperty<double> MaxWidthProperty =
        AvaloniaProperty.Register<DataGridColumn, double>(nameof(MaxWidth), double.PositiveInfinity);

    /// <summary>
    /// Defines the <see cref="IsSortable"/> property.
    /// </summary>
    public static readonly StyledProperty<bool> IsSortableProperty =
        AvaloniaProperty.Register<DataGridColumn, bool>(nameof(IsSortable), true);

    /// <summary>
    /// Defines the <see cref="IsFilterable"/> property.
    /// </summary>
    public static readonly StyledProperty<bool> IsFilterableProperty =
        AvaloniaProperty.Register<DataGridColumn, bool>(nameof(IsFilterable), true);

    /// <summary>
    /// Defines the <see cref="IsResizable"/> property.
    /// </summary>
    public static readonly StyledProperty<bool> IsResizableProperty =
        AvaloniaProperty.Register<DataGridColumn, bool>(nameof(IsResizable), true);

    /// <summary>
    /// Defines the <see cref="IsReorderable"/> property.
    /// </summary>
    public static readonly StyledProperty<bool> IsReorderableProperty =
        AvaloniaProperty.Register<DataGridColumn, bool>(nameof(IsReorderable), true);

    /// <summary>
    /// Defines the <see cref="IsFrozen"/> property.
    /// When true, this column is pinned and does not scroll horizontally.
    /// </summary>
    public static readonly StyledProperty<bool> IsFrozenProperty =
        AvaloniaProperty.Register<DataGridColumn, bool>(nameof(IsFrozen), false);

    /// <summary>
    /// Defines the <see cref="IsVisible"/> property.
    /// </summary>
    public static readonly StyledProperty<bool> IsVisibleProperty =
        AvaloniaProperty.Register<DataGridColumn, bool>(nameof(IsVisible), true);

    /// <summary>
    /// Defines the <see cref="IsReadOnly"/> property.
    /// </summary>
    public static readonly StyledProperty<bool> IsReadOnlyProperty =
        AvaloniaProperty.Register<DataGridColumn, bool>(nameof(IsReadOnly), false);

    /// <summary>
    /// Defines the <see cref="DisplayIndex"/> property.
    /// </summary>
    public static readonly StyledProperty<int> DisplayIndexProperty =
        AvaloniaProperty.Register<DataGridColumn, int>(nameof(DisplayIndex), -1);

    /// <summary>
    /// Defines the <see cref="SortDirection"/> property.
    /// </summary>
    public static readonly StyledProperty<DataGridSortDirection> SortDirectionProperty =
        AvaloniaProperty.Register<DataGridColumn, DataGridSortDirection>(nameof(SortDirection), DataGridSortDirection.None);

    /// <summary>
    /// Defines the <see cref="CellTemplate"/> property.
    /// </summary>
    public static readonly StyledProperty<Avalonia.Controls.Templates.IDataTemplate?> CellTemplateProperty =
        AvaloniaProperty.Register<DataGridColumn, Avalonia.Controls.Templates.IDataTemplate?>(nameof(CellTemplate));

    /// <summary>
    /// Defines the <see cref="CellEditTemplate"/> property.
    /// </summary>
    public static readonly StyledProperty<Avalonia.Controls.Templates.IDataTemplate?> CellEditTemplateProperty =
        AvaloniaProperty.Register<DataGridColumn, Avalonia.Controls.Templates.IDataTemplate?>(nameof(CellEditTemplate));

    /// <summary>
    /// Defines the <see cref="FilterValue"/> property.
    /// </summary>
    public static readonly StyledProperty<string?> FilterValueProperty =
        AvaloniaProperty.Register<DataGridColumn, string?>(nameof(FilterValue));

    /// <summary>
    /// Defines the <see cref="FilterOperator"/> property.
    /// </summary>
    public static readonly StyledProperty<DataGridFilterOperator> FilterOperatorProperty =
        AvaloniaProperty.Register<DataGridColumn, DataGridFilterOperator>(nameof(FilterOperator), DataGridFilterOperator.Contains);

    /// <summary>
    /// Defines the <see cref="IsGrouped"/> property.
    /// </summary>
    public static readonly StyledProperty<bool> IsGroupedProperty =
        AvaloniaProperty.Register<DataGridColumn, bool>(nameof(IsGrouped), false);

    /// <summary>
    /// Gets or sets the column header text.
    /// </summary>
    public string? Header
    {
        get => GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    /// <summary>
    /// Gets or sets the data binding for cell values.
    /// </summary>
    public BindingBase? Binding
    {
        get => GetValue(BindingProperty);
        set => SetValue(BindingProperty, value);
    }

    /// <summary>
    /// Gets or sets the property name on the data model.
    /// Used for sorting, filtering, auto-generation, and reflection-based binding.
    /// </summary>
    public string? PropertyName
    {
        get => GetValue(PropertyNameProperty);
        set => SetValue(PropertyNameProperty, value);
    }

    /// <summary>
    /// Gets or sets the column width. Use <c>double.NaN</c> for auto-sizing.
    /// </summary>
    public double Width
    {
        get => GetValue(WidthProperty);
        set => SetValue(WidthProperty, value);
    }

    /// <summary>
    /// Gets or sets the minimum column width.
    /// </summary>
    public double MinWidth
    {
        get => GetValue(MinWidthProperty);
        set => SetValue(MinWidthProperty, value);
    }

    /// <summary>
    /// Gets or sets the maximum column width.
    /// </summary>
    public double MaxWidth
    {
        get => GetValue(MaxWidthProperty);
        set => SetValue(MaxWidthProperty, value);
    }

    /// <summary>
    /// Gets or sets whether this column supports sorting.
    /// </summary>
    public bool IsSortable
    {
        get => GetValue(IsSortableProperty);
        set => SetValue(IsSortableProperty, value);
    }

    /// <summary>
    /// Gets or sets whether this column supports filtering.
    /// </summary>
    public bool IsFilterable
    {
        get => GetValue(IsFilterableProperty);
        set => SetValue(IsFilterableProperty, value);
    }

    /// <summary>
    /// Gets or sets whether this column can be resized.
    /// </summary>
    public bool IsResizable
    {
        get => GetValue(IsResizableProperty);
        set => SetValue(IsResizableProperty, value);
    }

    /// <summary>
    /// Gets or sets whether this column can be reordered by dragging.
    /// </summary>
    public bool IsReorderable
    {
        get => GetValue(IsReorderableProperty);
        set => SetValue(IsReorderableProperty, value);
    }

    /// <summary>
    /// Gets or sets whether this column is frozen (pinned).
    /// Frozen columns stay in place during horizontal scrolling.
    /// </summary>
    public bool IsFrozen
    {
        get => GetValue(IsFrozenProperty);
        set => SetValue(IsFrozenProperty, value);
    }

    /// <summary>
    /// Gets or sets whether this column is visible.
    /// </summary>
    public bool IsVisible
    {
        get => GetValue(IsVisibleProperty);
        set => SetValue(IsVisibleProperty, value);
    }

    /// <summary>
    /// Gets or sets whether this column is read-only.
    /// </summary>
    public bool IsReadOnly
    {
        get => GetValue(IsReadOnlyProperty);
        set => SetValue(IsReadOnlyProperty, value);
    }

    /// <summary>
    /// Gets or sets the display order index of this column.
    /// </summary>
    public int DisplayIndex
    {
        get => GetValue(DisplayIndexProperty);
        set => SetValue(DisplayIndexProperty, value);
    }

    /// <summary>
    /// Gets or sets the current sort direction for this column.
    /// </summary>
    public DataGridSortDirection SortDirection
    {
        get => GetValue(SortDirectionProperty);
        set => SetValue(SortDirectionProperty, value);
    }

    /// <summary>
    /// Gets or sets the template used to render cells in this column.
    /// </summary>
    public Avalonia.Controls.Templates.IDataTemplate? CellTemplate
    {
        get => GetValue(CellTemplateProperty);
        set => SetValue(CellTemplateProperty, value);
    }

    /// <summary>
    /// Gets or sets the template used to edit cells in this column.
    /// </summary>
    public Avalonia.Controls.Templates.IDataTemplate? CellEditTemplate
    {
        get => GetValue(CellEditTemplateProperty);
        set => SetValue(CellEditTemplateProperty, value);
    }

    /// <summary>
    /// Gets or sets the current filter value for this column.
    /// </summary>
    public string? FilterValue
    {
        get => GetValue(FilterValueProperty);
        set => SetValue(FilterValueProperty, value);
    }

    /// <summary>
    /// Gets or sets the filter operator for this column.
    /// </summary>
    public DataGridFilterOperator FilterOperator
    {
        get => GetValue(FilterOperatorProperty);
        set => SetValue(FilterOperatorProperty, value);
    }

    /// <summary>
    /// Gets or sets whether this column is currently used for grouping.
    /// </summary>
    public bool IsGrouped
    {
        get => GetValue(IsGroupedProperty);
        set => SetValue(IsGroupedProperty, value);
    }
}

/// <summary>
/// Sort direction for DataGrid columns.
/// </summary>
public enum DataGridSortDirection
{
    /// <summary>No sorting.</summary>
    None,
    /// <summary>Ascending sort.</summary>
    Ascending,
    /// <summary>Descending sort.</summary>
    Descending
}

/// <summary>
/// Filter operators for DataGrid column filtering.
/// </summary>
public enum DataGridFilterOperator
{
    /// <summary>Contains the filter text.</summary>
    Contains,
    /// <summary>Starts with the filter text.</summary>
    StartsWith,
    /// <summary>Ends with the filter text.</summary>
    EndsWith,
    /// <summary>Equals the filter text exactly.</summary>
    Equals,
    /// <summary>Does not equal the filter text.</summary>
    NotEquals,
    /// <summary>Greater than the filter value.</summary>
    GreaterThan,
    /// <summary>Less than the filter value.</summary>
    LessThan,
    /// <summary>Greater than or equal to the filter value.</summary>
    GreaterThanOrEqual,
    /// <summary>Less than or equal to the filter value.</summary>
    LessThanOrEqual
}
