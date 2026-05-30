using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Media;
using System.Collections;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Text;

namespace AuraUI.Controls.Display;

/// <summary>
/// An enhanced DataGrid with auto-column generation, sorting, filtering, grouping,
/// selection, virtualization, column resizing/reordering, frozen columns,
/// row details, and export support.
/// </summary>
public class AuraDataGrid : Avalonia.Controls.DataGrid
{
    #region Styled Properties — Appearance

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

    #endregion

    #region Styled Properties — Features

    /// <summary>
    /// Defines the <see cref="EnableSorting"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> EnableSortingProperty =
        AvaloniaProperty.Register<AuraDataGrid, bool>(nameof(EnableSorting), true);

    /// <summary>
    /// Defines the <see cref="EnableFiltering"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> EnableFilteringProperty =
        AvaloniaProperty.Register<AuraDataGrid, bool>(nameof(EnableFiltering), false);

    /// <summary>
    /// Defines the <see cref="EnableGrouping"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> EnableGroupingProperty =
        AvaloniaProperty.Register<AuraDataGrid, bool>(nameof(EnableGrouping), false);

    /// <summary>
    /// Defines the <see cref="GroupByPropertyName"/> styled property.
    /// The property name to group rows by.
    /// </summary>
    public static readonly StyledProperty<string?> GroupByPropertyNameProperty =
        AvaloniaProperty.Register<AuraDataGrid, string?>(nameof(GroupByPropertyName));

    /// <summary>
    /// Defines the <see cref="ShowSelectionCheckBoxes"/> styled property.
    /// When true, a checkbox column is shown for row selection.
    /// </summary>
    public static readonly StyledProperty<bool> ShowSelectionCheckBoxesProperty =
        AvaloniaProperty.Register<AuraDataGrid, bool>(nameof(ShowSelectionCheckBoxes), false);

    #endregion

    #region Instance Fields

    private DataGridSortDescriptionCollection _sortDescriptions = new();
    private DataGridFilterDescriptionCollection _filterDescriptions = new();
    private AvaloniaList<DataGridColumn> _customColumns = new();
    private IList<object>? _processedItems;

    #endregion

    #region Properties — Collections & State

    /// <summary>
    /// Gets the collection of active sort descriptions.
    /// </summary>
    public DataGridSortDescriptionCollection SortDescriptions
    {
        get => _sortDescriptions;
        set
        {
            _sortDescriptions = value ?? new DataGridSortDescriptionCollection();
            ApplySortAndFilter();
        }
    }

    /// <summary>
    /// Gets the collection of active filter descriptions.
    /// </summary>
    public DataGridFilterDescriptionCollection FilterDescriptions
    {
        get => _filterDescriptions;
        set
        {
            _filterDescriptions = value ?? new DataGridFilterDescriptionCollection();
            ApplySortAndFilter();
        }
    }

    /// <summary>
    /// Gets the collection of custom column definitions.
    /// </summary>
    public AvaloniaList<DataGridColumn> CustomColumns => _customColumns;

    #endregion

    #region Events

    /// <summary>
    /// Raised when the sort state changes.
    /// </summary>
    public event EventHandler<DataGridSortChangedEventArgs>? SortChanged;

    /// <summary>
    /// Raised when the filter state changes.
    /// </summary>
    public event EventHandler<DataGridFilterChangedEventArgs>? FilterChanged;

    /// <summary>
    /// Raised when a row is exported (for export operations).
    /// </summary>
    public event EventHandler<DataGridExportRowEventArgs>? ExportRow;

    /// <summary>
    /// Raised when columns are auto-generated from the data model.
    /// Allows cancellation or modification of auto-generated columns.
    /// </summary>
    public new event EventHandler<AutoGeneratingColumnEventArgs>? AutoGeneratingColumn;

    /// <summary>
    /// Raised when columns have been auto-generated.
    /// </summary>
    public event EventHandler? AutoGeneratedColumns;

    #endregion

    #region Static Constructor

    static AuraDataGrid()
    {
        AlternatingRowBackgroundProperty.Changed.AddClassHandler<AuraDataGrid>((x, _) => x.InvalidateVisual());
        EnableFilteringProperty.Changed.AddClassHandler<AuraDataGrid>((x, _) => x.ApplySortAndFilter());
        EnableGroupingProperty.Changed.AddClassHandler<AuraDataGrid>((x, _) => x.ApplySortAndFilter());
        GroupByPropertyNameProperty.Changed.AddClassHandler<AuraDataGrid>((x, _) => x.ApplySortAndFilter());
        ShowSelectionCheckBoxesProperty.Changed.AddClassHandler<AuraDataGrid>((x, _) => x.ApplyColumns());
        ItemsSourceProperty.Changed.AddClassHandler<AuraDataGrid>((x, _) => x.OnItemsSourceChanged());
    }

    #endregion

    #region Appearance Properties

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

    #endregion

    #region Feature Properties

    /// <summary>
    /// Gets or sets whether sorting is enabled.
    /// </summary>
    public bool EnableSorting
    {
        get => GetValue(EnableSortingProperty);
        set => SetValue(EnableSortingProperty, value);
    }

    /// <summary>
    /// Gets or sets whether filtering is enabled.
    /// </summary>
    public bool EnableFiltering
    {
        get => GetValue(EnableFilteringProperty);
        set => SetValue(EnableFilteringProperty, value);
    }

    /// <summary>
    /// Gets or sets whether grouping is enabled.
    /// </summary>
    public bool EnableGrouping
    {
        get => GetValue(EnableGroupingProperty);
        set => SetValue(EnableGroupingProperty, value);
    }

    /// <summary>
    /// Gets or sets the property name used for grouping rows.
    /// </summary>
    public string? GroupByPropertyName
    {
        get => GetValue(GroupByPropertyNameProperty);
        set => SetValue(GroupByPropertyNameProperty, value);
    }

    /// <summary>
    /// Gets or sets whether to show selection checkboxes.
    /// </summary>
    public bool ShowSelectionCheckBoxes
    {
        get => GetValue(ShowSelectionCheckBoxesProperty);
        set => SetValue(ShowSelectionCheckBoxesProperty, value);
    }

    #endregion

    #region Public Methods — Sorting

    /// <summary>
    /// Sorts by the specified property. Cycles: none -> asc -> desc -> none.
    /// </summary>
    /// <param name="propertyName">The property to sort by.</param>
    public void SortBy(string propertyName)
    {
        ArgumentException.ThrowIfNullOrEmpty(propertyName);

        var existing = _sortDescriptions.FirstOrDefault(s => s.PropertyName == propertyName);

        if (existing is null)
        {
            _sortDescriptions.Clear();
            _sortDescriptions.Add(DataGridSortDescription.Ascending(propertyName));
        }
        else if (existing.Direction == DataGridSortDirection.Ascending)
        {
            _sortDescriptions.Clear();
            _sortDescriptions.Add(DataGridSortDescription.Descending(propertyName));
        }
        else
        {
            _sortDescriptions.Clear();
        }

        ApplySortAndFilter();
        UpdateColumnSortDirections();
        SortChanged?.Invoke(this, new DataGridSortChangedEventArgs(_sortDescriptions.ToList()));
    }

    /// <summary>
    /// Adds a secondary sort (then-by) to the current sort.
    /// </summary>
    public void ThenSortBy(string propertyName)
    {
        ArgumentException.ThrowIfNullOrEmpty(propertyName);

        var existing = _sortDescriptions.FirstOrDefault(s => s.PropertyName == propertyName);
        if (existing is not null)
            _sortDescriptions.Remove(existing);

        _sortDescriptions.Add(DataGridSortDescription.Ascending(propertyName, _sortDescriptions.Count));
        ApplySortAndFilter();
        UpdateColumnSortDirections();
        SortChanged?.Invoke(this, new DataGridSortChangedEventArgs(_sortDescriptions.ToList()));
    }

    /// <summary>
    /// Clears all sorting.
    /// </summary>
    public void ClearSorting()
    {
        _sortDescriptions.Clear();
        ApplySortAndFilter();
        UpdateColumnSortDirections();
        SortChanged?.Invoke(this, new DataGridSortChangedEventArgs(new List<DataGridSortDescription>()));
    }

    #endregion

    #region Public Methods — Filtering

    /// <summary>
    /// Sets a filter on the specified property.
    /// </summary>
    /// <param name="propertyName">The property to filter on.</param>
    /// <param name="operator">The filter operator.</param>
    /// <param name="value">The filter value.</param>
    public void SetFilter(string propertyName, DataGridFilterOperator @operator, string value)
    {
        ArgumentException.ThrowIfNullOrEmpty(propertyName);

        var existing = _filterDescriptions.FirstOrDefault(f => f.PropertyName == propertyName);
        if (existing is not null)
            _filterDescriptions.Remove(existing);

        _filterDescriptions.Add(new DataGridFilterDescription(propertyName, @operator, value));

        var column = _customColumns.FirstOrDefault(c => c.PropertyName == propertyName);
        if (column is not null)
        {
            column.FilterValue = value;
            column.FilterOperator = @operator;
        }

        ApplySortAndFilter();
        FilterChanged?.Invoke(this, new DataGridFilterChangedEventArgs(_filterDescriptions.ToList()));
    }

    /// <summary>
    /// Clears the filter on the specified property.
    /// </summary>
    public void ClearFilter(string propertyName)
    {
        var existing = _filterDescriptions.FirstOrDefault(f => f.PropertyName == propertyName);
        if (existing is not null)
        {
            _filterDescriptions.Remove(existing);

            var column = _customColumns.FirstOrDefault(c => c.PropertyName == propertyName);
            if (column is not null)
                column.FilterValue = null;

            ApplySortAndFilter();
            FilterChanged?.Invoke(this, new DataGridFilterChangedEventArgs(_filterDescriptions.ToList()));
        }
    }

    /// <summary>
    /// Clears all filters.
    /// </summary>
    public void ClearAllFilters()
    {
        _filterDescriptions.Clear();
        foreach (var col in _customColumns)
            col.FilterValue = null;

        ApplySortAndFilter();
        FilterChanged?.Invoke(this, new DataGridFilterChangedEventArgs(new List<DataGridFilterDescription>()));
    }

    #endregion

    #region Public Methods — Grouping

    /// <summary>
    /// Groups rows by the specified property name.
    /// </summary>
    public void GroupBy(string propertyName)
    {
        GroupByPropertyName = propertyName;
    }

    /// <summary>
    /// Clears grouping.
    /// </summary>
    public void ClearGrouping()
    {
        GroupByPropertyName = null;
    }

    #endregion

    #region Public Methods — Selection

    /// <summary>
    /// Toggles selection for the specified item.
    /// </summary>
    public void ToggleSelection(object item)
    {
        if (SelectedItems.Contains(item))
            SelectedItems.Remove(item);
        else
            SelectedItems.Add(item);
    }

    #endregion

    #region Public Methods — Export

    /// <summary>
    /// Exports the current visible data to a CSV string.
    /// </summary>
    /// <param name="includeHeaders">Whether to include column headers.</param>
    /// <param name="separator">The field separator character.</param>
    /// <returns>The CSV content as a string.</returns>
    public string ExportToCsv(bool includeHeaders = true, char separator = ',')
    {
        var sb = new StringBuilder();
        var visibleColumns = GetVisibleColumns().ToList();

        if (includeHeaders)
        {
            sb.AppendLine(string.Join(separator,
                visibleColumns.Select(c => EscapeCsvField(c.Header ?? c.PropertyName ?? "", separator))));
        }

        var items = _processedItems ?? GetSourceItems();
        if (items is not null)
        {
            foreach (var item in items)
            {
                var values = visibleColumns.Select(c =>
                {
                    var value = GetPropertyValue(item, c.PropertyName);
                    return EscapeCsvField(value?.ToString() ?? "", separator);
                });

                sb.AppendLine(string.Join(separator, values));
                ExportRow?.Invoke(this, new DataGridExportRowEventArgs(item));
            }
        }

        return sb.ToString();
    }

    /// <summary>
    /// Exports the current visible data to an Excel-compatible CSV (UTF-8 BOM, tab-separated).
    /// </summary>
    /// <returns>The Excel-compatible content as a string.</returns>
    public string ExportToExcel()
    {
        var sb = new StringBuilder();
        sb.Append('﻿'); // UTF-8 BOM for Excel compatibility
        sb.Append(ExportToCsv(includeHeaders: true, separator: '\t'));
        return sb.ToString();
    }

    #endregion

    #region Public Methods — Column Management

    /// <summary>
    /// Manually adds a column definition.
    /// </summary>
    public void AddColumn(DataGridColumn column)
    {
        ArgumentNullException.ThrowIfNull(column);
        _customColumns.Add(column);
        ApplyColumns();
    }

    /// <summary>
    /// Removes a column by property name.
    /// </summary>
    public void RemoveColumn(string propertyName)
    {
        var col = _customColumns.FirstOrDefault(c => c.PropertyName == propertyName);
        if (col is not null)
        {
            _customColumns.Remove(col);
            ApplyColumns();
        }
    }

    /// <summary>
    /// Moves a column from one display position to another.
    /// </summary>
    public void MoveColumn(int fromIndex, int toIndex)
    {
        if (fromIndex < 0 || fromIndex >= _customColumns.Count ||
            toIndex < 0 || toIndex >= _customColumns.Count)
            return;

        var column = _customColumns[fromIndex];
        _customColumns.RemoveAt(fromIndex);
        _customColumns.Insert(toIndex, column);
        ApplyColumns();
    }

    /// <summary>
    /// Freezes (pins) the specified number of columns to the left.
    /// </summary>
    public void FreezeColumns(int count)
    {
        var clamped = Math.Max(0, Math.Min(count, _customColumns.Count));
        for (int i = 0; i < _customColumns.Count; i++)
            _customColumns[i].IsFrozen = i < clamped;
    }

    /// <summary>
    /// Auto-generates columns from the current data model's public properties.
    /// </summary>
    public void AutoGenerateFromSource()
    {
        GenerateColumnsFromSource();
    }

    #endregion

    #region Private Methods — Column Auto-Generation

    private void OnItemsSourceChanged()
    {
        if (AutoGenerateColumns)
            GenerateColumnsFromSource();

        ApplySortAndFilter();
    }

    private void GenerateColumnsFromSource()
    {
        var sourceType = GetSourceType();
        if (sourceType is null) return;

        var properties = sourceType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanRead && IsSimpleType(p.PropertyType))
            .ToList();

        _customColumns.Clear();

        for (int i = 0; i < properties.Count; i++)
        {
            var prop = properties[i];
            var column = new DataGridColumn
            {
                Header = FormatHeaderName(prop.Name),
                PropertyName = prop.Name,
                DisplayIndex = i,
                IsSortable = true,
                IsFilterable = true,
                IsResizable = true,
                IsReadOnly = !prop.CanWrite,
                Width = EstimateColumnWidth(prop),
            };

            var args = new AutoGeneratingColumnEventArgs(column, prop);
            AutoGeneratingColumn?.Invoke(this, args);

            if (!args.Cancel)
                _customColumns.Add(args.Column);
        }

        ApplyColumns();
        AutoGeneratedColumns?.Invoke(this, EventArgs.Empty);
    }

    private static string FormatHeaderName(string propertyName)
    {
        var sb = new StringBuilder();
        for (int i = 0; i < propertyName.Length; i++)
        {
            if (i > 0 && char.IsUpper(propertyName[i]) && !char.IsUpper(propertyName[i - 1]))
                sb.Append(' ');
            sb.Append(propertyName[i]);
        }
        return sb.ToString();
    }

    private static double EstimateColumnWidth(PropertyInfo prop)
    {
        if (prop.PropertyType == typeof(bool))
            return 60;
        if (prop.PropertyType == typeof(int) || prop.PropertyType == typeof(long))
            return 80;
        if (prop.PropertyType == typeof(DateTime))
            return 120;
        if (prop.PropertyType == typeof(decimal) || prop.PropertyType == typeof(double))
            return 100;
        return double.NaN; // Auto-size
    }

    private static bool IsSimpleType(Type type)
    {
        var underlying = Nullable.GetUnderlyingType(type) ?? type;
        return underlying.IsPrimitive
            || underlying == typeof(string)
            || underlying == typeof(decimal)
            || underlying == typeof(DateTime)
            || underlying == typeof(DateTimeOffset)
            || underlying == typeof(TimeSpan)
            || underlying == typeof(Guid)
            || underlying.IsEnum;
    }

    private Type? GetSourceType()
    {
        if (ItemsSource is IEnumerable source)
        {
            foreach (var item in source)
                return item?.GetType();
        }
        return null;
    }

    #endregion

    #region Private Methods — Apply Columns

    private void ApplyColumns()
    {
        Columns.Clear();

        if (ShowSelectionCheckBoxes)
        {
            var checkBoxColumn = new DataGridCheckBoxColumn
            {
                Header = "",
                Width = new DataGridLength(40, DataGridLengthUnitType.Pixel),
                IsReadOnly = false,
                Binding = new Binding(),
            };
            Columns.Add(checkBoxColumn);
        }

        var frozenCols = new List<DataGridColumn>();
        var scrollableCols = new List<DataGridColumn>();

        foreach (var col in _customColumns.Where(c => c.IsVisible).OrderBy(c => c.DisplayIndex))
        {
            if (col.IsFrozen)
                frozenCols.Add(col);
            else
                scrollableCols.Add(col);
        }

        foreach (var col in frozenCols.Concat(scrollableCols))
        {
            var gridColumn = new DataGridTextColumn
            {
                Header = col.Header,
                IsReadOnly = col.IsReadOnly,
                Width = double.IsNaN(col.Width)
                    ? DataGridLength.Auto
                    : new DataGridLength(col.Width, DataGridLengthUnitType.Pixel),
                MinWidth = col.MinWidth,
                MaxWidth = col.MaxWidth,
            };

            if (!string.IsNullOrEmpty(col.PropertyName))
                gridColumn.Binding = new Binding(col.PropertyName);

            Columns.Add(gridColumn);
        }
    }

    #endregion

    #region Private Methods — Sorting & Filtering Pipeline

    private void ApplySortAndFilter()
    {
        var sourceItems = GetSourceItems();
        if (sourceItems is null)
        {
            _processedItems = null;
            return;
        }

        IEnumerable<object> result = sourceItems;

        // Step 1: Apply filters
        if (_filterDescriptions.Count > 0)
        {
            result = _filterDescriptions.ApplyTo(result);
        }

        // Step 2: Apply grouping (reorder to group)
        if (EnableGrouping && !string.IsNullOrEmpty(GroupByPropertyName))
        {
            var propName = GroupByPropertyName;
            result = result.OrderBy(item => GetPropertyValue(item, propName)?.ToString());
        }

        // Step 3: Apply sorting
        if (_sortDescriptions.Count > 0)
        {
            result = _sortDescriptions.ApplyTo(result);
        }

        _processedItems = result.ToList();

        // Update the base ItemsSource
        base.ItemsSource = _processedItems;
    }

    private List<object>? GetSourceItems()
    {
        if (ItemsSource is IEnumerable source)
            return source.Cast<object>().ToList();
        return null;
    }

    private static object? GetPropertyValue(object item, string? propertyName)
    {
        if (item is null || string.IsNullOrEmpty(propertyName))
            return null;

        return item.GetType()
            .GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance)
            ?.GetValue(item);
    }

    private void UpdateColumnSortDirections()
    {
        foreach (var col in _customColumns)
        {
            var sort = _sortDescriptions.FirstOrDefault(s => s.PropertyName == col.PropertyName);
            col.SortDirection = sort?.Direction ?? DataGridSortDirection.None;
        }
    }

    #endregion

    #region Private Methods — CSV Helpers

    private static string EscapeCsvField(string field, char separator)
    {
        if (field.Contains(separator) || field.Contains('"') || field.Contains('\n') || field.Contains('\r'))
        {
            return "\"" + field.Replace("\"", "\"\"") + "\"";
        }
        return field;
    }

    private IEnumerable<DataGridColumn> GetVisibleColumns()
    {
        return _customColumns.Where(c => c.IsVisible).OrderBy(c => c.DisplayIndex);
    }

    #endregion
}

#region Event Args

/// <summary>
/// Event arguments for sort-changed events.
/// </summary>
public class DataGridSortChangedEventArgs : EventArgs
{
    public IReadOnlyList<DataGridSortDescription> SortDescriptions { get; }

    public DataGridSortChangedEventArgs(IReadOnlyList<DataGridSortDescription> sortDescriptions)
    {
        SortDescriptions = sortDescriptions;
    }
}

/// <summary>
/// Event arguments for filter-changed events.
/// </summary>
public class DataGridFilterChangedEventArgs : EventArgs
{
    public IReadOnlyList<DataGridFilterDescription> FilterDescriptions { get; }

    public DataGridFilterChangedEventArgs(IReadOnlyList<DataGridFilterDescription> filterDescriptions)
    {
        FilterDescriptions = filterDescriptions;
    }
}

/// <summary>
/// Event arguments for auto-generating-column events.
/// </summary>
public class AutoGeneratingColumnEventArgs : EventArgs
{
    public DataGridColumn Column { get; set; }
    public PropertyInfo PropertyInfo { get; }
    public bool Cancel { get; set; }

    public AutoGeneratingColumnEventArgs(DataGridColumn column, PropertyInfo propertyInfo)
    {
        Column = column;
        PropertyInfo = propertyInfo;
    }
}

/// <summary>
/// Event arguments for export-row events.
/// </summary>
public class DataGridExportRowEventArgs : EventArgs
{
    public object Item { get; }

    public DataGridExportRowEventArgs(object item)
    {
        Item = item;
    }
}

#endregion
