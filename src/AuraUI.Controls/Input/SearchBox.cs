using Avalonia;
using Avalonia.Automation;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;

namespace AuraUI.Controls.Input;

/// <summary>
/// Search text input with built-in search icon, clear button, and debounced search support.
///
/// Template parts:
///   PART_SearchIconPresenter - ContentPresenter for the search icon
///   PART_ClearButton         - Button to clear the search text
/// </summary>
public class SearchBox : TextBox
{
    private Button? _clearButton;
    private ContentPresenter? _searchIconPresenter;
    private DispatcherTimer? _debounceTimer;
    private bool _isDebounceActive;

    /// <summary>
    /// Defines the <see cref="SearchIcon"/> property.
    /// </summary>
    public static readonly StyledProperty<object?> SearchIconProperty =
        AvaloniaProperty.Register<SearchBox, object?>(
            nameof(SearchIcon));

    /// <summary>
    /// Defines the <see cref="IsClearable"/> property.
    /// </summary>
    public static readonly StyledProperty<bool> IsClearableProperty =
        AvaloniaProperty.Register<SearchBox, bool>(
            nameof(IsClearable),
            defaultValue: true);

    /// <summary>
    /// Defines the <see cref="SearchDelay"/> property.
    /// The debounce delay in milliseconds before firing the Search event.
    /// Set to 0 to disable debouncing (fire immediately on each change).
    /// </summary>
    public static readonly StyledProperty<int> SearchDelayProperty =
        AvaloniaProperty.Register<SearchBox, int>(
            nameof(SearchDelay),
            defaultValue: 300);

    /// <summary>
    /// Defines the SearchWatermark property.
    /// Provides default search watermark text.
    /// </summary>
    public static readonly StyledProperty<string?> SearchWatermarkProperty =
        AvaloniaProperty.Register<SearchBox, string?>(
            nameof(SearchWatermark),
            defaultValue: "Search...");

    /// <summary>
    /// Defines the <see cref="HasSearchText"/> property.
    /// </summary>
    public static readonly DirectProperty<SearchBox, bool> HasSearchTextProperty =
        AvaloniaProperty.RegisterDirect<SearchBox, bool>(
            nameof(HasSearchText),
            o => o.HasSearchText);

    /// <summary>
    /// Gets or sets the search icon content.
    /// </summary>
    public object? SearchIcon
    {
        get => GetValue(SearchIconProperty);
        set => SetValue(SearchIconProperty, value);
    }

    /// <summary>
    /// Gets or sets whether a clear button is shown when there is search text.
    /// </summary>
    public bool IsClearable
    {
        get => GetValue(IsClearableProperty);
        set => SetValue(IsClearableProperty, value);
    }

    /// <summary>
    /// Gets or sets the debounce delay in milliseconds.
    /// Set to 0 to fire search immediately on each text change.
    /// </summary>
    public int SearchDelay
    {
        get => GetValue(SearchDelayProperty);
        set => SetValue(SearchDelayProperty, value);
    }

    /// <summary>
    /// Gets or sets the watermark text shown when the search box is empty.
    /// </summary>
    public string? SearchWatermark
    {
        get => GetValue(SearchWatermarkProperty);
        set => SetValue(SearchWatermarkProperty, value);
    }

    private bool _hasSearchText;

    /// <summary>
    /// Gets whether the search box currently has text.
    /// </summary>
    public bool HasSearchText
    {
        get => _hasSearchText;
        private set => SetAndRaise(HasSearchTextProperty, ref _hasSearchText, value);
    }

    /// <summary>
    /// Raised when a search should be performed.
    /// When SearchDelay > 0, this is debounced; otherwise it fires on every text change.
    /// </summary>
    public event EventHandler<string>? Search;

    /// <summary>
    /// Raised immediately when the search text changes (not debounced).
    /// </summary>
    public event EventHandler<string>? SearchTextChanged;

    static SearchBox()
    {
        TextProperty.Changed.AddClassHandler<SearchBox>((x, e) => x.OnSearchTextChanged(e));
        IsClearableProperty.Changed.AddClassHandler<SearchBox>((x, _) => x.UpdateClearButtonVisibility());
        SearchDelayProperty.Changed.AddClassHandler<SearchBox>((x, e) => x.OnSearchDelayChanged(e));
    }

    protected override Type StyleKeyOverride => typeof(TextBox);

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        if (_clearButton is not null)
        {
            _clearButton.Click -= OnClearButtonClick;
        }

        base.OnApplyTemplate(e);

        _clearButton = e.NameScope.Find<Button>("PART_ClearButton");
        _searchIconPresenter = e.NameScope.Find<ContentPresenter>("PART_SearchIconPresenter");

        if (_clearButton is not null)
        {
            _clearButton.Click += OnClearButtonClick;
            _clearButton.SetValue(AutomationProperties.NameProperty, "Clear search");
        }

        // Apply default watermark if not already set
        if (string.IsNullOrEmpty(Watermark) && !string.IsNullOrEmpty(SearchWatermark))
        {
            Watermark = SearchWatermark;
        }

        SetValue(AutomationProperties.NameProperty, "Search");

        UpdateClearButtonVisibility();
        UpdateHasSearchText();
    }

    protected override void OnGotFocus(GotFocusEventArgs e)
    {
        base.OnGotFocus(e);
        PseudoClasses.Set("focus", true);
    }

    protected override void OnLostFocus(RoutedEventArgs e)
    {
        base.OnLostFocus(e);
        PseudoClasses.Set("focus", false);

        // Fire search immediately on focus loss if there is pending text
        if (_isDebounceActive)
        {
            CancelDebounce();
            FireSearch(Text ?? string.Empty);
        }
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);

        if (e.Key == Key.Escape && !string.IsNullOrEmpty(Text))
        {
            Text = string.Empty;
            e.Handled = true;
        }
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);

        if (_clearButton is not null)
        {
            _clearButton.Click -= OnClearButtonClick;
        }

        CancelDebounce();
    }

    private void OnSearchTextChanged(AvaloniaPropertyChangedEventArgs e)
    {
        var newText = e.NewValue as string ?? string.Empty;

        UpdateClearButtonVisibility();
        UpdateHasSearchText();

        SearchTextChanged?.Invoke(this, newText);

        if (SearchDelay > 0)
        {
            // Debounce: reset timer on each change
            StartDebounce(newText);
        }
        else
        {
            // No debounce: fire immediately
            FireSearch(newText);
        }
    }

    private void OnClearButtonClick(object? sender, RoutedEventArgs e)
    {
        Text = string.Empty;
        Focus();
    }

    private void OnSearchDelayChanged(AvaloniaPropertyChangedEventArgs e)
    {
        // Reset debounce timer with new delay
        CancelDebounce();
    }

    private void UpdateClearButtonVisibility()
    {
        if (_clearButton is not null)
        {
            var hasText = !string.IsNullOrEmpty(Text);
            _clearButton.IsVisible = IsClearable && hasText;
        }
    }

    private void UpdateHasSearchText()
    {
        HasSearchText = !string.IsNullOrEmpty(Text);
    }

    private string? _pendingSearchText;

    private void StartDebounce(string searchText)
    {
        CancelDebounce();

        _pendingSearchText = searchText;
        _debounceTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(SearchDelay)
        };

        _debounceTimer.Tick += OnDebounceTick;
        _isDebounceActive = true;
        _debounceTimer.Start();
    }

    private void OnDebounceTick(object? sender, EventArgs e)
    {
        CancelDebounce();
        if (_pendingSearchText != null)
        {
            FireSearch(_pendingSearchText);
            _pendingSearchText = null;
        }
    }

    private void CancelDebounce()
    {
        if (_debounceTimer is not null)
        {
            _debounceTimer.Tick -= OnDebounceTick;
            _debounceTimer.Stop();
            _debounceTimer = null;
        }

        _isDebounceActive = false;
    }

    private void FireSearch(string searchText)
    {
        Search?.Invoke(this, searchText);
    }
}
