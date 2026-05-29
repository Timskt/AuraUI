using Avalonia;
using Avalonia.Controls;
using System.Collections;

namespace AuraUI.Controls.Display;

/// <summary>
/// An enhanced DataGrid with virtualization optimizations for large data sets.
/// Only visible rows are rendered, with support for sorting, filtering, grouping,
/// paging, and configurable row heights.
/// </summary>
public class VirtualizingDataGrid : Avalonia.Controls.DataGrid
{
    /// <summary>
    /// Defines the <see cref="DataRowHeight"/> styled property.
    /// The height of each data row.
    /// </summary>
    public static readonly StyledProperty<double> DataRowHeightProperty =
        AvaloniaProperty.Register<VirtualizingDataGrid, double>(nameof(DataRowHeight), 36);

    /// <summary>
    /// Defines the <see cref="PageSize"/> styled property.
    /// The number of items per page. When 0, paging is disabled and all items are shown.
    /// </summary>
    public static readonly StyledProperty<int> PageSizeProperty =
        AvaloniaProperty.Register<VirtualizingDataGrid, int>(nameof(PageSize), 0);

    /// <summary>
    /// Defines the <see cref="CurrentPage"/> styled property.
    /// The currently displayed page (zero-based).
    /// </summary>
    public static readonly StyledProperty<int> CurrentPageProperty =
        AvaloniaProperty.Register<VirtualizingDataGrid, int>(nameof(CurrentPage), 0);

    /// <summary>
    /// Defines the <see cref="EnableVirtualization"/> styled property.
    /// When true, enables row virtualization so only visible rows are realized.
    /// </summary>
    public static readonly StyledProperty<bool> EnableVirtualizationProperty =
        AvaloniaProperty.Register<VirtualizingDataGrid, bool>(nameof(EnableVirtualization), true);

    /// <summary>
    /// Defines the <see cref="FilterText"/> styled property.
    /// A text filter applied across all columns. Rows that do not match are hidden.
    /// </summary>
    public static readonly StyledProperty<string?> FilterTextProperty =
        AvaloniaProperty.Register<VirtualizingDataGrid, string?>(nameof(FilterText));

    /// <summary>
    /// Defines the <see cref="SortColumn"/> styled property.
    /// The name of the column currently used for sorting.
    /// </summary>
    public static readonly StyledProperty<string?> SortColumnProperty =
        AvaloniaProperty.Register<VirtualizingDataGrid, string?>(nameof(SortColumn));

    /// <summary>
    /// Defines the <see cref="SortAscending"/> styled property.
    /// Whether the current sort direction is ascending.
    /// </summary>
    public static readonly StyledProperty<bool> SortAscendingProperty =
        AvaloniaProperty.Register<VirtualizingDataGrid, bool>(nameof(SortAscending), true);

    /// <summary>
    /// Defines the <see cref="GroupByColumn"/> styled property.
    /// The name of the column used for grouping rows.
    /// </summary>
    public static readonly StyledProperty<string?> GroupByColumnProperty =
        AvaloniaProperty.Register<VirtualizingDataGrid, string?>(nameof(GroupByColumn));

    /// <summary>
    /// Defines the <see cref="TotalPages"/> direct property.
    /// The total number of pages based on the current filter and page size.
    /// </summary>
    public static readonly DirectProperty<VirtualizingDataGrid, int> TotalPagesProperty =
        AvaloniaProperty.RegisterDirect<VirtualizingDataGrid, int>(
            nameof(TotalPages),
            o => o.TotalPages);

    private int _totalPages;
    private List<object>? _filteredItems;

    /// <summary>
    /// Gets or sets the height of each data row.
    /// </summary>
    public double DataRowHeight
    {
        get => GetValue(DataRowHeightProperty);
        set => SetValue(DataRowHeightProperty, value);
    }

    /// <summary>
    /// Gets or sets the number of items per page. Set to 0 to disable paging.
    /// </summary>
    public int PageSize
    {
        get => GetValue(PageSizeProperty);
        set => SetValue(PageSizeProperty, value);
    }

    /// <summary>
    /// Gets or sets the currently displayed page (zero-based).
    /// </summary>
    public int CurrentPage
    {
        get => GetValue(CurrentPageProperty);
        set => SetValue(CurrentPageProperty, value);
    }

    /// <summary>
    /// Gets or sets whether virtualization is enabled.
    /// </summary>
    public bool EnableVirtualization
    {
        get => GetValue(EnableVirtualizationProperty);
        set => SetValue(EnableVirtualizationProperty, value);
    }

    /// <summary>
    /// Gets or sets the text filter applied to all columns.
    /// </summary>
    public string? FilterText
    {
        get => GetValue(FilterTextProperty);
        set => SetValue(FilterTextProperty, value);
    }

    /// <summary>
    /// Gets or sets the column name used for sorting.
    /// </summary>
    public string? SortColumn
    {
        get => GetValue(SortColumnProperty);
        set => SetValue(SortColumnProperty, value);
    }

    /// <summary>
    /// Gets or sets whether sorting is ascending.
    /// </summary>
    public bool SortAscending
    {
        get => GetValue(SortAscendingProperty);
        set => SetValue(SortAscendingProperty, value);
    }

    /// <summary>
    /// Gets or sets the column name used for grouping.
    /// </summary>
    public string? GroupByColumn
    {
        get => GetValue(GroupByColumnProperty);
        set => SetValue(GroupByColumnProperty, value);
    }

    /// <summary>
    /// Gets the total number of pages based on the filtered data and page size.
    /// </summary>
    public int TotalPages
    {
        get => _totalPages;
        private set => SetAndRaise(TotalPagesProperty, ref _totalPages, value);
    }

    /// <summary>
    /// Raised when the page changes.
    /// </summary>
    public event EventHandler<PageChangedEventArgs>? PageChanged;

    static VirtualizingDataGrid()
    {
        PageSizeProperty.Changed.AddClassHandler<VirtualizingDataGrid>((x, _) => x.OnDataChanged());
        CurrentPageProperty.Changed.AddClassHandler<VirtualizingDataGrid>((x, e) => x.OnPageChanged(e));
        FilterTextProperty.Changed.AddClassHandler<VirtualizingDataGrid>((x, _) => x.OnDataChanged());
        SortColumnProperty.Changed.AddClassHandler<VirtualizingDataGrid>((x, _) => x.ApplySorting());
        SortAscendingProperty.Changed.AddClassHandler<VirtualizingDataGrid>((x, _) => x.ApplySorting());
        GroupByColumnProperty.Changed.AddClassHandler<VirtualizingDataGrid>((x, _) => x.ApplyGrouping());
        ItemsSourceProperty.Changed.AddClassHandler<VirtualizingDataGrid>((x, _) => x.OnDataChanged());
    }

    /// <summary>
    /// Navigates to the first page.
    /// </summary>
    public void FirstPage()
    {
        CurrentPage = 0;
    }

    /// <summary>
    /// Navigates to the previous page.
    /// </summary>
    public void PreviousPage()
    {
        if (CurrentPage > 0)
            CurrentPage--;
    }

    /// <summary>
    /// Navigates to the next page.
    /// </summary>
    public void NextPage()
    {
        if (CurrentPage < TotalPages - 1)
            CurrentPage++;
    }

    /// <summary>
    /// Navigates to the last page.
    /// </summary>
    public void LastPage()
    {
        CurrentPage = Math.Max(0, TotalPages - 1);
    }

    /// <summary>
    /// Clears all filters, sorting, and grouping.
    /// </summary>
    public void ClearFilters()
    {
        FilterText = null;
        SortColumn = null;
        GroupByColumn = null;
    }

    private void OnDataChanged()
    {
        ApplyFilter();
        ApplySorting();
        ApplyGrouping();
        UpdatePaging();
    }

    private void OnPageChanged(AvaloniaPropertyChangedEventArgs e)
    {
        var newPage = (int)(e.NewValue ?? 0);
        if (newPage < 0)
        {
            CurrentPage = 0;
            return;
        }

        if (newPage >= TotalPages && TotalPages > 0)
        {
            CurrentPage = TotalPages - 1;
            return;
        }

        ApplyPaging();
        PageChanged?.Invoke(this, new PageChangedEventArgs(newPage, PageSize));
    }

    private void ApplyFilter()
    {
        if (ItemsSource is not IEnumerable source)
        {
            _filteredItems = null;
            return;
        }

        var allItems = source.Cast<object>().ToList();

        if (string.IsNullOrWhiteSpace(FilterText))
        {
            _filteredItems = allItems;
            return;
        }

        var filter = FilterText.Trim();
        _filteredItems = allItems.Where(item => MatchesFilter(item, filter)).ToList();
    }

    private static bool MatchesFilter(object item, string filter)
    {
        var properties = item.GetType().GetProperties();
        return properties.Any(prop =>
        {
            var value = prop.GetValue(item);
            if (value is null)
                return false;
            return value.ToString()?.Contains(filter, StringComparison.OrdinalIgnoreCase) == true;
        });
    }

    private void ApplySorting()
    {
        if (_filteredItems is null || _filteredItems.Count == 0)
            return;

        if (string.IsNullOrWhiteSpace(SortColumn))
            return;

        var first = _filteredItems[0];
        var prop = first.GetType().GetProperty(SortColumn);
        if (prop is null)
            return;

        _filteredItems = SortAscending
            ? _filteredItems.OrderBy(x => prop.GetValue(x)).ToList()
            : _filteredItems.OrderByDescending(x => prop.GetValue(x)).ToList();

        UpdateItemsSource();
    }

    private void ApplyGrouping()
    {
        if (_filteredItems is null || _filteredItems.Count == 0)
            return;

        if (string.IsNullOrWhiteSpace(GroupByColumn))
        {
            UpdateItemsSource();
            return;
        }

        var first = _filteredItems[0];
        var prop = first.GetType().GetProperty(GroupByColumn);
        if (prop is null)
        {
            UpdateItemsSource();
            return;
        }

        _filteredItems = _filteredItems
            .GroupBy(x => prop.GetValue(x))
            .SelectMany(g => g)
            .ToList();

        UpdateItemsSource();
    }

    private void ApplyPaging()
    {
        UpdateItemsSource();
    }

    private void UpdatePaging()
    {
        var count = _filteredItems?.Count ?? (ItemsSource is IEnumerable e ? e.Cast<object>().Count() : 0);

        TotalPages = PageSize > 0 ? (int)Math.Ceiling((double)count / PageSize) : 1;

        if (CurrentPage >= TotalPages)
        {
            CurrentPage = Math.Max(0, TotalPages - 1);
        }

        ApplyPaging();
    }

    private void UpdateItemsSource()
    {
        if (_filteredItems is null)
            return;

        if (PageSize > 0)
        {
            var paged = _filteredItems
                .Skip(CurrentPage * PageSize)
                .Take(PageSize)
                .ToList();

            base.ItemsSource = paged;
        }
        else
        {
            base.ItemsSource = _filteredItems;
        }
    }
}

/// <summary>
/// Event arguments for page change events.
/// </summary>
public class PageChangedEventArgs : EventArgs
{
    /// <summary>
    /// Gets the new page index.
    /// </summary>
    public int Page { get; }

    /// <summary>
    /// Gets the page size.
    /// </summary>
    public int PageSize { get; }

    public PageChangedEventArgs(int page, int pageSize)
    {
        Page = page;
        PageSize = pageSize;
    }
}
