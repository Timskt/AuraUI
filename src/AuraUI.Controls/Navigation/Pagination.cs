using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;

namespace AuraUI.Controls.Navigation;

/// <summary>
/// Event args for page changes.
/// </summary>
public class PageChangedEventArgs : RoutedEventArgs
{
    /// <summary>
    /// Gets the new page number (1-based).
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

    public PageChangedEventArgs(RoutedEvent routedEvent, int page, int pageSize) : base(routedEvent)
    {
        Page = page;
        PageSize = pageSize;
    }
}

/// <summary>
/// A pagination control that renders page buttons with ellipsis, previous/next buttons,
/// and optional page size changer and quick jumper.
/// </summary>
[PseudoClasses(":simple", ":full")]
public class Pagination : TemplatedControl
{
    /// <summary>
    /// Defines the <see cref="TotalItems"/> styled property.
    /// </summary>
    public static readonly StyledProperty<int> TotalItemsProperty =
        AvaloniaProperty.Register<Pagination, int>(nameof(TotalItems));

    /// <summary>
    /// Defines the <see cref="PageSize"/> styled property.
    /// </summary>
    public static readonly StyledProperty<int> PageSizeProperty =
        AvaloniaProperty.Register<Pagination, int>(nameof(PageSize), 10);

    /// <summary>
    /// Defines the <see cref="CurrentPage"/> styled property.
    /// </summary>
    public static readonly StyledProperty<int> CurrentPageProperty =
        AvaloniaProperty.Register<Pagination, int>(nameof(CurrentPage), 1, coerce: (o, v) => CoerceCurrentPage((Pagination)o, v));

    /// <summary>
    /// Defines the <see cref="ShowSizeChanger"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> ShowSizeChangerProperty =
        AvaloniaProperty.Register<Pagination, bool>(nameof(ShowSizeChanger));

    /// <summary>
    /// Defines the <see cref="ShowQuickJumper"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> ShowQuickJumperProperty =
        AvaloniaProperty.Register<Pagination, bool>(nameof(ShowQuickJumper));

    /// <summary>
    /// Defines the <see cref="PageSizeOptions"/> styled property.
    /// </summary>
    public static readonly StyledProperty<int[]> PageSizeOptionsProperty =
        AvaloniaProperty.Register<Pagination, int[]>(nameof(PageSizeOptions), new[] { 10, 20, 50, 100 });

    /// <summary>
    /// Defines the <see cref="MaxVisiblePages"/> styled property.
    /// </summary>
    public static readonly StyledProperty<int> MaxVisiblePagesProperty =
        AvaloniaProperty.Register<Pagination, int>(nameof(MaxVisiblePages), 7);

    /// <summary>
    /// Defines the routed event for page changed.
    /// </summary>
    public static readonly RoutedEvent<PageChangedEventArgs> PageChangedEvent =
        RoutedEvent.Register<Pagination, PageChangedEventArgs>(nameof(PageChanged), RoutingStrategies.Bubble);

    static Pagination()
    {
        TotalItemsProperty.Changed.AddClassHandler<Pagination>((x, _) => x.InvalidateMeasure());
        PageSizeProperty.Changed.AddClassHandler<Pagination>((x, _) => x.OnPageSizeChanged());
        CurrentPageProperty.Changed.AddClassHandler<Pagination>((x, e) => x.OnCurrentPageChanged(e));
    }

    /// <summary>
    /// Gets or sets the total number of items.
    /// </summary>
    public int TotalItems
    {
        get => GetValue(TotalItemsProperty);
        set => SetValue(TotalItemsProperty, value);
    }

    /// <summary>
    /// Gets or sets the number of items per page.
    /// </summary>
    public int PageSize
    {
        get => GetValue(PageSizeProperty);
        set => SetValue(PageSizeProperty, value);
    }

    /// <summary>
    /// Gets or sets the current page number (1-based).
    /// </summary>
    public int CurrentPage
    {
        get => GetValue(CurrentPageProperty);
        set => SetValue(CurrentPageProperty, value);
    }

    /// <summary>
    /// Gets or sets whether to show the page size selector.
    /// </summary>
    public bool ShowSizeChanger
    {
        get => GetValue(ShowSizeChangerProperty);
        set => SetValue(ShowSizeChangerProperty, value);
    }

    /// <summary>
    /// Gets or sets whether to show the quick jumper input.
    /// </summary>
    public bool ShowQuickJumper
    {
        get => GetValue(ShowQuickJumperProperty);
        set => SetValue(ShowQuickJumperProperty, value);
    }

    /// <summary>
    /// Gets or sets the available page size options.
    /// </summary>
    public int[] PageSizeOptions
    {
        get => GetValue(PageSizeOptionsProperty);
        set => SetValue(PageSizeOptionsProperty, value);
    }

    /// <summary>
    /// Gets or sets the maximum number of visible page buttons.
    /// </summary>
    public int MaxVisiblePages
    {
        get => GetValue(MaxVisiblePagesProperty);
        set => SetValue(MaxVisiblePagesProperty, value);
    }

    /// <summary>
    /// Occurs when the page changes.
    /// </summary>
    public event EventHandler<PageChangedEventArgs>? PageChanged
    {
        add => AddHandler(PageChangedEvent, value);
        remove => RemoveHandler(PageChangedEvent, value);
    }

    /// <summary>
    /// Gets the total number of pages.
    /// </summary>
    public int TotalPages
    {
        get
        {
            var size = PageSize;
            if (size <= 0) return 1;
            return Math.Max(1, (int)Math.Ceiling((double)TotalItems / size));
        }
    }

    /// <summary>
    /// Navigates to the specified page (1-based).
    /// </summary>
    public void GoToPage(int page)
    {
        CurrentPage = Math.Clamp(page, 1, TotalPages);
    }

    /// <summary>
    /// Navigates to the next page.
    /// </summary>
    public void NextPage()
    {
        if (CurrentPage < TotalPages)
            CurrentPage++;
    }

    /// <summary>
    /// Navigates to the previous page.
    /// </summary>
    public void PreviousPage()
    {
        if (CurrentPage > 1)
            CurrentPage--;
    }

    /// <summary>
    /// Gets the list of page numbers/labels to display (includes -1 for ellipsis).
    /// </summary>
    public IReadOnlyList<int> GetPageNumbers()
    {
        var totalPages = TotalPages;
        var current = CurrentPage;
        var maxVisible = MaxVisiblePages;
        var pages = new List<int>();

        if (totalPages <= maxVisible)
        {
            for (int i = 1; i <= totalPages; i++)
                pages.Add(i);
        }
        else
        {
            pages.Add(1);

            var start = Math.Max(2, current - (maxVisible - 4) / 2);
            var end = Math.Min(totalPages - 1, start + maxVisible - 4);
            start = Math.Max(2, end - (maxVisible - 4));

            if (start > 2)
                pages.Add(-1); // Ellipsis

            for (int i = start; i <= end; i++)
                pages.Add(i);

            if (end < totalPages - 1)
                pages.Add(-1); // Ellipsis

            pages.Add(totalPages);
        }

        return pages;
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        UpdatePseudoClasses();
    }

    private static int CoerceCurrentPage(Pagination sender, int value)
    {
        return Math.Max(1, value);
    }

    private void OnCurrentPageChanged(AvaloniaPropertyChangedEventArgs e)
    {
        var page = (int)e.NewValue!;
        var args = new PageChangedEventArgs(PageChangedEvent, page, PageSize);
        RaiseEvent(args);
    }

    private void OnPageSizeChanged()
    {
        // Recalculate current page to stay within bounds
        var totalPages = TotalPages;
        if (CurrentPage > totalPages)
            CurrentPage = totalPages;
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(":simple", !ShowSizeChanger && !ShowQuickJumper);
        PseudoClasses.Set(":full", ShowSizeChanger || ShowQuickJumper);
    }
}
