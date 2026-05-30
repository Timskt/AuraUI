using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using AuraUI.Core.Services;

namespace AuraUI.Controls.Navigation;

/// <summary>
/// Event arguments for when a search result is selected.
/// </summary>
public class SearchResultSelectedEventArgs : RoutedEventArgs
{
    /// <summary>
    /// Gets the selected search result.
    /// </summary>
    public SearchResult Result { get; }

    /// <summary>
    /// Gets the provider that produced this result.
    /// </summary>
    public ISearchProvider Provider { get; }

    public SearchResultSelectedEventArgs(SearchResult result, ISearchProvider provider)
        : base(SearchEverywhere.ResultSelectedEvent)
    {
        Result = result;
        Provider = provider;
    }
}

/// <summary>
/// An overlay search panel that queries multiple <see cref="ISearchProvider"/> instances
/// asynchronously and displays grouped results. Opens with Ctrl+K or Ctrl+P.
/// </summary>
[TemplatePart("PART_SearchBox", typeof(TextBox))]
[TemplatePart("PART_ResultsList", typeof(ListBox))]
[TemplatePart("PART_Overlay", typeof(Border))]
[PseudoClasses(":open", ":closed", ":searching", ":has-results", ":no-results")]
public class SearchEverywhere : TemplatedControl
{
    private TextBox? _searchBox;
    private ListBox? _resultsList;
    private Border? _overlay;
    private CancellationTokenSource? _searchCts;
    private readonly ObservableCollection<SearchResultEntry> _results = new();

    /// <summary>
    /// Defines the <see cref="IsOpen"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsOpenProperty =
        AvaloniaProperty.Register<SearchEverywhere, bool>(nameof(IsOpen));

    /// <summary>
    /// Defines the <see cref="Providers"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IList<ISearchProvider>?> ProvidersProperty =
        AvaloniaProperty.Register<SearchEverywhere, IList<ISearchProvider>?>(nameof(Providers));

    /// <summary>
    /// Defines the <see cref="SearchText"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> SearchTextProperty =
        AvaloniaProperty.Register<SearchEverywhere, string?>(nameof(SearchText));

    /// <summary>
    /// Defines the <see cref="Placeholder"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> PlaceholderProperty =
        AvaloniaProperty.Register<SearchEverywhere, string?>(nameof(Placeholder), "Search everywhere...");

    /// <summary>
    /// Defines the <see cref="MaxResults"/> styled property.
    /// </summary>
    public static readonly StyledProperty<int> MaxResultsProperty =
        AvaloniaProperty.Register<SearchEverywhere, int>(nameof(MaxResults), 20);

    /// <summary>
    /// Defines the <see cref="SelectedIndex"/> styled property.
    /// </summary>
    public static readonly StyledProperty<int> SelectedIndexProperty =
        AvaloniaProperty.Register<SearchEverywhere, int>(nameof(SelectedIndex));

    /// <summary>
    /// Defines the ResultSelected routed event.
    /// </summary>
    public static readonly RoutedEvent<SearchResultSelectedEventArgs> ResultSelectedEvent =
        RoutedEvent.Register<SearchEverywhere, SearchResultSelectedEventArgs>(nameof(ResultSelected), RoutingStrategies.Bubble);

    /// <summary>
    /// Defines the <see cref="SearchDelay"/> styled property.
    /// Delay in milliseconds before starting the search after the user stops typing.
    /// </summary>
    public static readonly StyledProperty<int> SearchDelayProperty =
        AvaloniaProperty.Register<SearchEverywhere, int>(nameof(SearchDelay), 200);

    static SearchEverywhere()
    {
        IsOpenProperty.Changed.AddClassHandler<SearchEverywhere>((x, _) => x.OnIsOpenChanged());
        SearchTextProperty.Changed.AddClassHandler<SearchEverywhere>((x, _) => x.OnSearchTextChanged());
    }

    /// <summary>
    /// Gets or sets whether the search panel is currently visible.
    /// </summary>
    public bool IsOpen
    {
        get => GetValue(IsOpenProperty);
        set => SetValue(IsOpenProperty, value);
    }

    /// <summary>
    /// Gets or sets the list of search providers to query.
    /// </summary>
    public IList<ISearchProvider>? Providers
    {
        get => GetValue(ProvidersProperty);
        set => SetValue(ProvidersProperty, value);
    }

    /// <summary>
    /// Gets or sets the search text entered by the user.
    /// </summary>
    public string? SearchText
    {
        get => GetValue(SearchTextProperty);
        set => SetValue(SearchTextProperty, value);
    }

    /// <summary>
    /// Gets or sets the placeholder text for the search input.
    /// </summary>
    public string? Placeholder
    {
        get => GetValue(PlaceholderProperty);
        set => SetValue(PlaceholderProperty, value);
    }

    /// <summary>
    /// Gets or sets the maximum number of results to display.
    /// </summary>
    public int MaxResults
    {
        get => GetValue(MaxResultsProperty);
        set => SetValue(MaxResultsProperty, value);
    }

    /// <summary>
    /// Gets or sets the currently selected result index.
    /// </summary>
    public int SelectedIndex
    {
        get => GetValue(SelectedIndexProperty);
        set => SetValue(SelectedIndexProperty, value);
    }

    /// <summary>
    /// Gets or sets the search debounce delay in milliseconds.
    /// </summary>
    public int SearchDelay
    {
        get => GetValue(SearchDelayProperty);
        set => SetValue(SearchDelayProperty, value);
    }

    /// <summary>
    /// Gets the current list of search results.
    /// </summary>
    public IReadOnlyList<SearchResultEntry> Results => _results;

    /// <summary>
    /// Raised when a search result is selected.
    /// </summary>
    public event EventHandler<SearchResultSelectedEventArgs>? ResultSelected
    {
        add => AddHandler(ResultSelectedEvent, value);
        remove => RemoveHandler(ResultSelectedEvent, value);
    }

    /// <summary>
    /// Raised when the search panel is opened.
    /// </summary>
    public event EventHandler? Opened;

    /// <summary>
    /// Raised when the search panel is closed.
    /// </summary>
    public event EventHandler? Closed;

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        // Unsubscribe from old parts
        if (_searchBox != null)
        {
            _searchBox.TextChanged -= OnSearchBoxTextChanged;
            _searchBox.KeyDown -= OnSearchBoxKeyDown;
        }
        if (_overlay != null)
            _overlay.PointerPressed -= OnOverlayPointerPressed;

        // Subscribe to new parts
        _searchBox = e.NameScope.Find<TextBox>("PART_SearchBox");
        _resultsList = e.NameScope.Find<ListBox>("PART_ResultsList");
        _overlay = e.NameScope.Find<Border>("PART_Overlay");

        if (_searchBox != null)
        {
            _searchBox.TextChanged += OnSearchBoxTextChanged;
            _searchBox.KeyDown += OnSearchBoxKeyDown;
        }

        if (_resultsList != null)
            _resultsList.ItemsSource = _results;

        if (_overlay != null)
            _overlay.PointerPressed += OnOverlayPointerPressed;
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        UpdatePseudoClasses();
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);

        if (!IsOpen) return;

        switch (e.Key)
        {
            case Key.Escape:
                Close();
                e.Handled = true;
                break;
            case Key.Up:
                NavigateUp();
                e.Handled = true;
                break;
            case Key.Down:
                NavigateDown();
                e.Handled = true;
                break;
            case Key.Enter:
                ExecuteSelected();
                e.Handled = true;
                break;
        }
    }

    private void OnSearchBoxTextChanged(object? sender, TextChangedEventArgs e)
    {
        SearchText = _searchBox?.Text;
    }

    private void OnSearchBoxKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key is Key.Up or Key.Down or Key.Enter or Key.Escape)
        {
            e.Handled = true;
            switch (e.Key)
            {
                case Key.Escape:
                    Close();
                    break;
                case Key.Up:
                    NavigateUp();
                    break;
                case Key.Down:
                    NavigateDown();
                    break;
                case Key.Enter:
                    ExecuteSelected();
                    break;
            }
        }
    }

    private void OnOverlayPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        Close();
    }

    private void OnIsOpenChanged()
    {
        UpdatePseudoClasses();

        if (IsOpen)
        {
            SearchText = null;
            _results.Clear();
            _searchBox?.Focus();
            Opened?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            CancelPendingSearch();
            Closed?.Invoke(this, EventArgs.Empty);
        }
    }

    private void OnSearchTextChanged()
    {
        CancelPendingSearch();

        var query = SearchText;
        if (string.IsNullOrWhiteSpace(query))
        {
            _results.Clear();
            UpdateResultPseudoClasses();
            return;
        }

        PseudoClasses.Set(":searching", true);

        // Debounce: start search after delay
        var cts = new CancellationTokenSource();
        _searchCts = cts;
        var delay = SearchDelay;

        _ = Task.Run(async () =>
        {
            try
            {
                await Task.Delay(delay, cts.Token);
                if (cts.IsCancellationRequested) return;

                await PerformSearchAsync(query, cts.Token);
            }
            catch (OperationCanceledException)
            {
                // Expected when search is cancelled
            }
        });
    }

    private async Task PerformSearchAsync(string query, CancellationToken ct)
    {
        var providers = Providers;
        if (providers is null || providers.Count == 0)
        {
            await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
            {
                _results.Clear();
                PseudoClasses.Set(":searching", false);
                UpdateResultPseudoClasses();
            });
            return;
        }

        var allResults = new List<SearchResultEntry>();
        var searchTasks = new List<Task>();

        foreach (var provider in providers)
        {
            if (!provider.CanSearch(query)) continue;

            searchTasks.Add(Task.Run(async () =>
            {
                try
                {
                    var results = await provider.SearchAsync(query, ct);
                    if (ct.IsCancellationRequested || results is null) return;

                    lock (allResults)
                    {
                        foreach (var result in results)
                        {
                            allResults.Add(new SearchResultEntry(result, provider));
                        }
                    }
                }
                catch (OperationCanceledException) { }
                catch { /* Provider threw; skip silently */ }
            }, ct));
        }

        await Task.WhenAll(searchTasks);

        if (ct.IsCancellationRequested) return;

        // Sort by score descending, then group by provider
        var sorted = allResults
            .OrderByDescending(r => r.Result.Score)
            .Take(MaxResults)
            .ToList();

        await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
        {
            _results.Clear();
            foreach (var entry in sorted)
                _results.Add(entry);

            PseudoClasses.Set(":searching", false);
            UpdateResultPseudoClasses();

            SelectedIndex = _results.Count > 0 ? 0 : -1;
        });
    }

    private void CancelPendingSearch()
    {
        _searchCts?.Cancel();
        _searchCts = null;
    }

    private void NavigateUp()
    {
        if (_results.Count == 0) return;
        SelectedIndex = SelectedIndex <= 0 ? _results.Count - 1 : SelectedIndex - 1;
        ScrollToSelected();
    }

    private void NavigateDown()
    {
        if (_results.Count == 0) return;
        SelectedIndex = SelectedIndex >= _results.Count - 1 ? 0 : SelectedIndex + 1;
        ScrollToSelected();
    }

    private void ScrollToSelected()
    {
        if (_resultsList != null && SelectedIndex >= 0)
        {
            _resultsList.SelectedIndex = SelectedIndex;
            _resultsList.ScrollIntoView(SelectedIndex);
        }
    }

    private void ExecuteSelected()
    {
        if (SelectedIndex < 0 || SelectedIndex >= _results.Count) return;

        var entry = _results[SelectedIndex];

        // Invoke the result's OnSelect action
        entry.Result.OnSelect?.Invoke();

        // Raise the event
        RaiseEvent(new SearchResultSelectedEventArgs(entry.Result, entry.Provider));

        Close();
    }

    /// <summary>
    /// Opens the search panel.
    /// </summary>
    public void Open()
    {
        IsOpen = true;
    }

    /// <summary>
    /// Closes the search panel.
    /// </summary>
    public void Close()
    {
        IsOpen = false;
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(":open", IsOpen);
        PseudoClasses.Set(":closed", !IsOpen);
    }

    private void UpdateResultPseudoClasses()
    {
        PseudoClasses.Set(":has-results", _results.Count > 0);
        PseudoClasses.Set(":no-results", !string.IsNullOrWhiteSpace(SearchText) && _results.Count == 0);
    }
}

/// <summary>
/// Wraps a <see cref="SearchResult"/> with its originating <see cref="ISearchProvider"/>
/// for display in the SearchEverywhere results list.
/// </summary>
public class SearchResultEntry
{
    /// <summary>
    /// Gets the search result.
    /// </summary>
    public SearchResult Result { get; }

    /// <summary>
    /// Gets the provider that produced this result.
    /// </summary>
    public ISearchProvider Provider { get; }

    public SearchResultEntry(SearchResult result, ISearchProvider provider)
    {
        Result = result ?? throw new ArgumentNullException(nameof(result));
        Provider = provider ?? throw new ArgumentNullException(nameof(provider));
    }

    /// <summary>
    /// Gets the display title.
    /// </summary>
    public string Title => Result.Title;

    /// <summary>
    /// Gets the display description.
    /// </summary>
    public string? Description => Result.Description;

    /// <summary>
    /// Gets the provider name for grouping display.
    /// </summary>
    public string ProviderName => Provider.Name;
}
