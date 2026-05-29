using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;

namespace AuraUI.Controls.Input;

/// <summary>
/// An auto-complete text input that shows filtered suggestions as the user types.
/// Supports keyboard navigation, debounced input, and highlighted matching text.
/// Inspired by Ant Design and Element Plus auto-complete components.
/// </summary>
[TemplatePart("PART_TextBox", typeof(TextBox))]
[TemplatePart("PART_SuggestionsList", typeof(ListBox))]
[TemplatePart("PART_Popup", typeof(Popup))]
[PseudoClasses(":open", ":loading", ":has-value")]
public class AutoComplete : TemplatedControl
{
    private TextBox? _textBox;
    private ListBox? _suggestionsList;
    private Popup? _popup;
    private DispatcherTimer? _debounceTimer;
    private int _selectedIndex = -1;
    private bool _isUpdatingText;

    /// <summary>
    /// Defines the <see cref="Value"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> ValueProperty =
        AvaloniaProperty.Register<AutoComplete, string?>(nameof(Value));

    /// <summary>
    /// Defines the <see cref="Options"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IList<string>?> OptionsProperty =
        AvaloniaProperty.Register<AutoComplete, IList<string>?>(nameof(Options));

    /// <summary>
    /// Defines the <see cref="Placeholder"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> PlaceholderProperty =
        AvaloniaProperty.Register<AutoComplete, string?>(nameof(Placeholder));

    /// <summary>
    /// Defines the <see cref="IsLoading"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsLoadingProperty =
        AvaloniaProperty.Register<AutoComplete, bool>(nameof(IsLoading));

    /// <summary>
    /// Defines the <see cref="MinChars"/> styled property.
    /// Minimum characters before suggestions appear.
    /// </summary>
    public static readonly StyledProperty<int> MinCharsProperty =
        AvaloniaProperty.Register<AutoComplete, int>(nameof(MinChars), 1);

    /// <summary>
    /// Defines the <see cref="MaxSuggestions"/> styled property.
    /// </summary>
    public static readonly StyledProperty<int> MaxSuggestionsProperty =
        AvaloniaProperty.Register<AutoComplete, int>(nameof(MaxSuggestions), 10);

    /// <summary>
    /// Defines the <see cref="HighlightMatch"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> HighlightMatchProperty =
        AvaloniaProperty.Register<AutoComplete, bool>(nameof(HighlightMatch), true);

    /// <summary>
    /// Defines the <see cref="CaseSensitive"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> CaseSensitiveProperty =
        AvaloniaProperty.Register<AutoComplete, bool>(nameof(CaseSensitive));

    /// <summary>
    /// Defines the <see cref="DebounceDelay"/> styled property.
    /// Delay in milliseconds before filtering after typing.
    /// </summary>
    public static readonly StyledProperty<int> DebounceDelayProperty =
        AvaloniaProperty.Register<AutoComplete, int>(nameof(DebounceDelay), 200);

    /// <summary>
    /// Defines the <see cref="IsDropDownOpen"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsDropDownOpenProperty =
        AvaloniaProperty.Register<AutoComplete, bool>(nameof(IsDropDownOpen));

    /// <summary>
    /// Defines the <see cref="FilteredOptions"/> direct property.
    /// </summary>
    public static readonly DirectProperty<AutoComplete, ObservableCollection<string>> FilteredOptionsProperty =
        AvaloniaProperty.RegisterDirect<AutoComplete, ObservableCollection<string>>(
            nameof(FilteredOptions), o => o.FilteredOptions);

    private readonly ObservableCollection<string> _filteredOptions = new();

    static AutoComplete()
    {
        ValueProperty.Changed.AddClassHandler<AutoComplete>((x, e) => x.OnValueChanged(e));
        IsDropDownOpenProperty.Changed.AddClassHandler<AutoComplete>((x, _) => x.UpdatePseudoClasses());
        IsLoadingProperty.Changed.AddClassHandler<AutoComplete>((x, _) => x.UpdatePseudoClasses());
        OptionsProperty.Changed.AddClassHandler<AutoComplete>((x, _) => x.FilterSuggestions());
    }

    /// <summary>
    /// Gets or sets the current input value.
    /// </summary>
    public string? Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    /// <summary>
    /// Gets or sets the available options.
    /// </summary>
    public IList<string>? Options
    {
        get => GetValue(OptionsProperty);
        set => SetValue(OptionsProperty, value);
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
    /// Gets or sets whether results are loading (for async data sources).
    /// </summary>
    public bool IsLoading
    {
        get => GetValue(IsLoadingProperty);
        set => SetValue(IsLoadingProperty, value);
    }

    /// <summary>
    /// Gets or sets the minimum characters before showing suggestions.
    /// </summary>
    public int MinChars
    {
        get => GetValue(MinCharsProperty);
        set => SetValue(MinCharsProperty, value);
    }

    /// <summary>
    /// Gets or sets the maximum number of suggestions to display.
    /// </summary>
    public int MaxSuggestions
    {
        get => GetValue(MaxSuggestionsProperty);
        set => SetValue(MaxSuggestionsProperty, value);
    }

    /// <summary>
    /// Gets or sets whether to highlight matching text in suggestions.
    /// </summary>
    public bool HighlightMatch
    {
        get => GetValue(HighlightMatchProperty);
        set => SetValue(HighlightMatchProperty, value);
    }

    /// <summary>
    /// Gets or sets whether filtering is case sensitive.
    /// </summary>
    public bool CaseSensitive
    {
        get => GetValue(CaseSensitiveProperty);
        set => SetValue(CaseSensitiveProperty, value);
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
    /// Gets or sets whether the suggestions dropdown is open.
    /// </summary>
    public bool IsDropDownOpen
    {
        get => GetValue(IsDropDownOpenProperty);
        set => SetValue(IsDropDownOpenProperty, value);
    }

    /// <summary>
    /// Gets the currently filtered options.
    /// </summary>
    public ObservableCollection<string> FilteredOptions => _filteredOptions;

    /// <summary>
    /// Occurs when the selected suggestion changes.
    /// </summary>
    public event EventHandler<SelectionChangedEventArgs>? SelectionChanged;

    /// <summary>
    /// Occurs when the search text changes (after debounce).
    /// </summary>
    public event EventHandler<RoutedEventArgs>? SearchChanged;

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        DetachTextBox();

        _textBox = e.NameScope.Find<TextBox>("PART_TextBox");
        _suggestionsList = e.NameScope.Find<ListBox>("PART_SuggestionsList");
        _popup = e.NameScope.Find<Popup>("PART_Popup");

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

        if (!IsDropDownOpen)
        {
            if (e.Key == Key.Down && _filteredOptions.Count > 0)
            {
                IsDropDownOpen = true;
                _selectedIndex = 0;
                UpdateSelectedSuggestion();
                e.Handled = true;
            }
            return;
        }

        switch (e.Key)
        {
            case Key.Down:
                _selectedIndex = Math.Min(_selectedIndex + 1, _filteredOptions.Count - 1);
                UpdateSelectedSuggestion();
                e.Handled = true;
                break;
            case Key.Up:
                _selectedIndex = Math.Max(_selectedIndex - 1, 0);
                UpdateSelectedSuggestion();
                e.Handled = true;
                break;
            case Key.Enter:
                if (_selectedIndex >= 0 && _selectedIndex < _filteredOptions.Count)
                {
                    SelectOption(_filteredOptions[_selectedIndex]);
                    e.Handled = true;
                }
                break;
            case Key.Escape:
                IsDropDownOpen = false;
                e.Handled = true;
                break;
        }
    }

    /// <summary>
    /// Selects a suggestion option.
    /// </summary>
    public void SelectOption(string option)
    {
        _isUpdatingText = true;
        Value = option;
        _isUpdatingText = false;
        IsDropDownOpen = false;
        _selectedIndex = -1;
    }

    /// <summary>
    /// Clears the current value and suggestions.
    /// </summary>
    public void Clear()
    {
        _isUpdatingText = true;
        Value = string.Empty;
        _isUpdatingText = false;
        IsDropDownOpen = false;
        _filteredOptions.Clear();
    }

    private void OnValueChanged(AvaloniaPropertyChangedEventArgs e)
    {
        PseudoClasses.Set(":has-value", !string.IsNullOrEmpty(Value));

        if (!_isUpdatingText)
        {
            StartDebounce();
        }
    }

    private void AttachTextBox()
    {
        if (_textBox != null)
        {
            _textBox.TextChanged += OnTextBoxTextChanged;
        }
    }

    private void DetachTextBox()
    {
        if (_textBox != null)
        {
            _textBox.TextChanged -= OnTextBoxTextChanged;
            _textBox = null;
        }
    }

    private void OnTextBoxTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (!_isUpdatingText)
        {
            _isUpdatingText = true;
            Value = _textBox?.Text;
            _isUpdatingText = false;
            StartDebounce();
        }
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
        var value = Value;
        var options = Options;

        if (string.IsNullOrEmpty(value) || value.Length < MinChars || options == null)
        {
            _filteredOptions.Clear();
            IsDropDownOpen = false;
            SearchChanged?.Invoke(this, new RoutedEventArgs());
            return;
        }

        var comparison = CaseSensitive
            ? StringComparison.Ordinal
            : StringComparison.OrdinalIgnoreCase;

        var filtered = options
            .Where(o => !string.IsNullOrEmpty(o) && o.Contains(value, comparison))
            .Take(MaxSuggestions)
            .ToList();

        _filteredOptions.Clear();
        foreach (var item in filtered)
        {
            _filteredOptions.Add(item);
        }

        IsDropDownOpen = _filteredOptions.Count > 0;
        _selectedIndex = -1;
        SearchChanged?.Invoke(this, new RoutedEventArgs());
    }

    private void OnSuggestionSelected(object? sender, SelectionChangedEventArgs e)
    {
        if (_suggestionsList?.SelectedItem is string selected)
        {
            SelectOption(selected);
            SelectionChanged?.Invoke(this, e);
        }
    }

    private void UpdateSelectedSuggestion()
    {
        if (_suggestionsList != null && _selectedIndex >= 0)
        {
            _suggestionsList.SelectedIndex = _selectedIndex;
        }
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(":open", IsDropDownOpen);
        PseudoClasses.Set(":loading", IsLoading);
        PseudoClasses.Set(":has-value", !string.IsNullOrEmpty(Value));
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        StopDebounce();
    }
}
