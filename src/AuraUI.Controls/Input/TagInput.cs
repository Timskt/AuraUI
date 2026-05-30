using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;

namespace AuraUI.Controls.Input;

/// <summary>
/// A tag input control where users type text and press Enter (or a separator key)
/// to add tags. Supports autocomplete suggestions, duplicate prevention, maximum tag
/// limits, and keyboard navigation. Unlike <see cref="ChipInput"/> which is
/// mobile-focused, this control provides a desktop-oriented tag editing experience
/// with inline autocomplete.
/// </summary>
[TemplatePart("PART_TextBox", typeof(TextBox))]
[TemplatePart("PART_TagItemsControl", typeof(ItemsControl))]
[TemplatePart("PART_SuggestionsPopup", typeof(Popup))]
[TemplatePart("PART_SuggestionsList", typeof(ListBox))]
[PseudoClasses(":empty", ":focused", ":max-reached", ":has-suggestions")]
public class TagInput : TemplatedControl
{
    private TextBox? _textBox;
    private ListBox? _suggestionsList;
    private Popup? _suggestionsPopup;
    private DispatcherTimer? _debounceTimer;
    private int _selectedSuggestionIndex = -1;

    /// <summary>
    /// Defines the <see cref="Tags"/> styled property.
    /// </summary>
    public static readonly StyledProperty<ObservableCollection<string>?> TagsProperty =
        AvaloniaProperty.Register<TagInput, ObservableCollection<string>?>(nameof(Tags));

    /// <summary>
    /// Defines the <see cref="Suggestions"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IList<string>?> SuggestionsProperty =
        AvaloniaProperty.Register<TagInput, IList<string>?>(nameof(Suggestions));

    /// <summary>
    /// Defines the <see cref="Placeholder"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> PlaceholderProperty =
        AvaloniaProperty.Register<TagInput, string?>(nameof(Placeholder));

    /// <summary>
    /// Defines the <see cref="MaxTags"/> styled property.
    /// -1 means unlimited.
    /// </summary>
    public static readonly StyledProperty<int> MaxTagsProperty =
        AvaloniaProperty.Register<TagInput, int>(nameof(MaxTags), -1);

    /// <summary>
    /// Defines the <see cref="AllowDuplicates"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> AllowDuplicatesProperty =
        AvaloniaProperty.Register<TagInput, bool>(nameof(AllowDuplicates));

    /// <summary>
    /// Defines the <see cref="SeparatorKeys"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IList<Key>?> SeparatorKeysProperty =
        AvaloniaProperty.Register<TagInput, IList<Key>?>(nameof(SeparatorKeys));

    /// <summary>
    /// Defines the <see cref="MaxSuggestions"/> styled property.
    /// </summary>
    public static readonly StyledProperty<int> MaxSuggestionsProperty =
        AvaloniaProperty.Register<TagInput, int>(nameof(MaxSuggestions), 8);

    /// <summary>
    /// Defines the <see cref="DebounceDelay"/> styled property.
    /// </summary>
    public static readonly StyledProperty<int> DebounceDelayProperty =
        AvaloniaProperty.Register<TagInput, int>(nameof(DebounceDelay), 200);

    /// <summary>
    /// Defines the <see cref="ShowSuggestions"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> ShowSuggestionsProperty =
        AvaloniaProperty.Register<TagInput, bool>(nameof(ShowSuggestions), true);

    /// <summary>
    /// Defines the <see cref="CaseSensitiveDuplicates"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> CaseSensitiveDuplicatesProperty =
        AvaloniaProperty.Register<TagInput, bool>(nameof(CaseSensitiveDuplicates));

    private readonly ObservableCollection<string> _filteredSuggestions = new();

    static TagInput()
    {
        TagsProperty.Changed.AddClassHandler<TagInput>((x, _) => x.OnTagsChanged());
        MaxTagsProperty.Changed.AddClassHandler<TagInput>((x, _) => x.UpdatePseudoClasses());
    }

    /// <summary>
    /// Occurs when a tag is added.
    /// </summary>
    public event EventHandler<TagEventArgs>? TagAdded;

    /// <summary>
    /// Occurs when a tag is removed.
    /// </summary>
    public event EventHandler<TagEventArgs>? TagRemoved;

    /// <summary>
    /// Occurs when the input text changes (for async suggestion loading).
    /// </summary>
    public event EventHandler<RoutedEventArgs>? SearchChanged;

    /// <summary>
    /// Gets or sets the collection of tags.
    /// </summary>
    public ObservableCollection<string>? Tags
    {
        get => GetValue(TagsProperty);
        set => SetValue(TagsProperty, value);
    }

    /// <summary>
    /// Gets or sets the autocomplete suggestions.
    /// </summary>
    public IList<string>? Suggestions
    {
        get => GetValue(SuggestionsProperty);
        set => SetValue(SuggestionsProperty, value);
    }

    /// <summary>
    /// Gets or sets the placeholder text.
    /// </summary>
    public string? Placeholder
    {
        get => GetValue(PlaceholderProperty);
        set => SetValue(PlaceholderProperty, value);
    }

    /// <summary>
    /// Gets or sets the maximum number of tags. -1 for unlimited.
    /// </summary>
    public int MaxTags
    {
        get => GetValue(MaxTagsProperty);
        set => SetValue(MaxTagsProperty, value);
    }

    /// <summary>
    /// Gets or sets whether duplicate tags are allowed.
    /// </summary>
    public bool AllowDuplicates
    {
        get => GetValue(AllowDuplicatesProperty);
        set => SetValue(AllowDuplicatesProperty, value);
    }

    /// <summary>
    /// Gets or sets the keys that trigger tag creation.
    /// Default is Enter only.
    /// </summary>
    public IList<Key>? SeparatorKeys
    {
        get => GetValue(SeparatorKeysProperty);
        set => SetValue(SeparatorKeysProperty, value);
    }

    /// <summary>
    /// Gets or sets the maximum number of suggestions to show.
    /// </summary>
    public int MaxSuggestions
    {
        get => GetValue(MaxSuggestionsProperty);
        set => SetValue(MaxSuggestionsProperty, value);
    }

    /// <summary>
    /// Gets or sets the debounce delay in milliseconds.
    /// </summary>
    public int DebounceDelay
    {
        get => GetValue(DebounceDelayProperty);
        set => SetValue(DebounceDelayProperty, value);
    }

    /// <summary>
    /// Gets or sets whether to show autocomplete suggestions.
    /// </summary>
    public bool ShowSuggestions
    {
        get => GetValue(ShowSuggestionsProperty);
        set => SetValue(ShowSuggestionsProperty, value);
    }

    /// <summary>
    /// Gets or sets whether duplicate detection is case sensitive.
    /// </summary>
    public bool CaseSensitiveDuplicates
    {
        get => GetValue(CaseSensitiveDuplicatesProperty);
        set => SetValue(CaseSensitiveDuplicatesProperty, value);
    }

    /// <summary>
    /// Gets the filtered suggestions for display.
    /// </summary>
    public ObservableCollection<string> FilteredSuggestions => _filteredSuggestions;

    /// <summary>
    /// Programmatically adds a tag.
    /// </summary>
    public bool AddTag(string tag)
    {
        if (string.IsNullOrWhiteSpace(tag))
            return false;

        tag = tag.Trim();

        if (MaxTags >= 0 && (Tags?.Count ?? 0) >= MaxTags)
            return false;

        if (!AllowDuplicates && Tags != null)
        {
            var comparison = CaseSensitiveDuplicates
                ? StringComparison.Ordinal
                : StringComparison.OrdinalIgnoreCase;

            if (Tags.Any(t => string.Equals(t, tag, comparison)))
                return false;
        }

        Tags ??= new ObservableCollection<string>();
        Tags.Add(tag);
        TagAdded?.Invoke(this, new TagEventArgs(tag));
        return true;
    }

    /// <summary>
    /// Programmatically removes a tag.
    /// </summary>
    public bool RemoveTag(string tag)
    {
        if (Tags == null)
            return false;

        var removed = Tags.Remove(tag);
        if (removed)
            TagRemoved?.Invoke(this, new TagEventArgs(tag));
        return removed;
    }

    /// <summary>
    /// Removes the last tag (backspace behavior).
    /// </summary>
    public void RemoveLastTag()
    {
        if (Tags is { Count: > 0 })
        {
            var last = Tags[^1];
            Tags.RemoveAt(Tags.Count - 1);
            TagRemoved?.Invoke(this, new TagEventArgs(last));
        }
    }

    /// <summary>
    /// Clears all tags.
    /// </summary>
    public void Clear()
    {
        Tags?.Clear();
        _filteredSuggestions.Clear();
        _textBox?.Clear();
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        DetachTextBox();

        _textBox = e.NameScope.Find<TextBox>("PART_TextBox");
        _suggestionsList = e.NameScope.Find<ListBox>("PART_SuggestionsList");
        _suggestionsPopup = e.NameScope.Find<Popup>("PART_SuggestionsPopup");

        AttachTextBox();

        if (_suggestionsList != null)
        {
            _suggestionsList.SelectionChanged += OnSuggestionSelected;
        }

        UpdatePseudoClasses();
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);

        if (_textBox == null) return;

        var text = _textBox.Text?.Trim();

        // Check separator keys
        var separatorKeys = SeparatorKeys;
        if (separatorKeys != null && separatorKeys.Contains(e.Key) && !string.IsNullOrEmpty(text))
        {
            if (AddTag(text))
            {
                _textBox.Text = string.Empty;
                e.Handled = true;
            }
            return;
        }

        // Enter to add tag (default behavior when no separator keys defined)
        if (e.Key == Key.Enter && separatorKeys == null && !string.IsNullOrEmpty(text))
        {
            if (_selectedSuggestionIndex >= 0 && _selectedSuggestionIndex < _filteredSuggestions.Count)
            {
                AddTag(_filteredSuggestions[_selectedSuggestionIndex]);
                _textBox.Text = string.Empty;
                CloseSuggestions();
                e.Handled = true;
                return;
            }

            if (AddTag(text))
            {
                _textBox.Text = string.Empty;
                CloseSuggestions();
                e.Handled = true;
            }
            return;
        }

        // Backspace on empty input removes last tag
        if (e.Key == Key.Back && string.IsNullOrEmpty(_textBox.Text))
        {
            RemoveLastTag();
            e.Handled = true;
            return;
        }

        // Suggestion navigation
        if (_filteredSuggestions.Count > 0)
        {
            switch (e.Key)
            {
                case Key.Down:
                    _selectedSuggestionIndex = Math.Min(
                        _selectedSuggestionIndex + 1,
                        _filteredSuggestions.Count - 1);
                    UpdateSelectedSuggestion();
                    e.Handled = true;
                    break;
                case Key.Up:
                    _selectedSuggestionIndex = Math.Max(_selectedSuggestionIndex - 1, 0);
                    UpdateSelectedSuggestion();
                    e.Handled = true;
                    break;
                case Key.Escape:
                    CloseSuggestions();
                    e.Handled = true;
                    break;
            }
        }
    }

    private void AttachTextBox()
    {
        if (_textBox != null)
        {
            _textBox.TextChanged += OnTextBoxTextChanged;
            _textBox.GotFocus += OnTextBoxGotFocus;
            _textBox.LostFocus += OnTextBoxLostFocus;
        }
    }

    private void DetachTextBox()
    {
        if (_textBox != null)
        {
            _textBox.TextChanged -= OnTextBoxTextChanged;
            _textBox.GotFocus -= OnTextBoxGotFocus;
            _textBox.LostFocus -= OnTextBoxLostFocus;
            _textBox = null;
        }
    }

    private void OnTextBoxTextChanged(object? sender, TextChangedEventArgs e)
    {
        StartDebounce();
        UpdatePseudoClasses();
    }

    private void OnTextBoxGotFocus(object? sender, FocusChangedEventArgs e)
    {
        PseudoClasses.Set(":focused", true);
    }

    private void OnTextBoxLostFocus(object? sender, RoutedEventArgs e)
    {
        PseudoClasses.Set(":focused", false);
        // Close suggestions after a delay to allow click events
        Dispatcher.UIThread.Post(() => CloseSuggestions(), DispatcherPriority.Background);
    }

    private void OnTagsChanged()
    {
        UpdatePseudoClasses();
    }

    private void StartDebounce()
    {
        StopDebounce();
        _debounceTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(DebounceDelay)
        };
        _debounceTimer.Tick += OnDebounceTick;
        _debounceTimer.Start();
    }

    private void OnDebounceTick(object? sender, EventArgs e)
    {
        StopDebounce();
        FilterSuggestions();
    }

    private void StopDebounce()
    {
        if (_debounceTimer != null)
        {
            _debounceTimer.Tick -= OnDebounceTick;
            _debounceTimer.Stop();
            _debounceTimer = null;
        }
    }

    private void FilterSuggestions()
    {
        if (!ShowSuggestions || _textBox == null)
            return;

        var text = _textBox.Text?.Trim();
        var suggestions = Suggestions;

        if (string.IsNullOrEmpty(text) || suggestions == null)
        {
            CloseSuggestions();
            SearchChanged?.Invoke(this, new RoutedEventArgs());
            return;
        }

        var comparison = CaseSensitiveDuplicates
            ? StringComparison.Ordinal
            : StringComparison.OrdinalIgnoreCase;

        // Exclude already-added tags from suggestions
        var existingTags = Tags != null
            ? new HashSet<string>(Tags, StringComparer.FromComparison(comparison))
            : new HashSet<string>(StringComparer.FromComparison(comparison));

        var filtered = suggestions
            .Where(s => !string.IsNullOrEmpty(s)
                        && s.Contains(text, comparison)
                        && !existingTags.Contains(s))
            .Take(MaxSuggestions)
            .ToList();

        _filteredSuggestions.Clear();
        foreach (var item in filtered)
            _filteredSuggestions.Add(item);

        if (_filteredSuggestions.Count > 0)
        {
            _selectedSuggestionIndex = -1;
            PseudoClasses.Set(":has-suggestions", true);
            if (_suggestionsPopup != null)
                _suggestionsPopup.IsOpen = true;
        }
        else
        {
            CloseSuggestions();
        }

        SearchChanged?.Invoke(this, new RoutedEventArgs());
    }

    private void CloseSuggestions()
    {
        _filteredSuggestions.Clear();
        _selectedSuggestionIndex = -1;
        PseudoClasses.Set(":has-suggestions", false);
        if (_suggestionsPopup != null)
            _suggestionsPopup.IsOpen = false;
    }

    private void OnSuggestionSelected(object? sender, SelectionChangedEventArgs e)
    {
        if (_suggestionsList?.SelectedItem is string selected)
        {
            AddTag(selected);
            _textBox!.Text = string.Empty;
            CloseSuggestions();
        }
    }

    private void UpdateSelectedSuggestion()
    {
        if (_suggestionsList != null && _selectedSuggestionIndex >= 0)
            _suggestionsList.SelectedIndex = _selectedSuggestionIndex;
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(":empty", Tags == null || Tags.Count == 0);
        PseudoClasses.Set(":max-reached", MaxTags >= 0 && (Tags?.Count ?? 0) >= MaxTags);
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        StopDebounce();
    }
}

/// <summary>
/// Event arguments for tag events.
/// </summary>
public class TagEventArgs : EventArgs
{
    public string Tag { get; }

    public TagEventArgs(string tag)
    {
        Tag = tag;
    }
}
