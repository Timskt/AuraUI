using System.Collections;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using AuraUI.Core.Input;

namespace AuraUI.Controls.Navigation;

/// <summary>
/// Event arguments for when a command is executed via the command palette.
/// </summary>
public class CommandExecutedEventArgs : RoutedEventArgs
{
    /// <summary>
    /// Gets the command that was executed.
    /// </summary>
    public CommandItem Command { get; }

    public CommandExecutedEventArgs(CommandItem command) : base(CommandPalette.CommandExecutedEvent)
    {
        Command = command;
    }
}

/// <summary>
/// A modal command palette overlay with fuzzy search, keyboard navigation,
/// category grouping, and recent commands. Inspired by VS Code's command palette.
/// Opens with Ctrl+Shift+P, closes with Escape.
/// </summary>
[TemplatePart("PART_SearchBox", typeof(TextBox))]
[TemplatePart("PART_ResultsList", typeof(ListBox))]
[TemplatePart("PART_Overlay", typeof(Border))]
[PseudoClasses(":open", ":closed", ":has-results", ":no-results")]
public class CommandPalette : TemplatedControl
{
    private TextBox? _searchBox;
    private ListBox? _resultsList;
    private Border? _overlay;
    private readonly ObservableCollection<CommandItem> _filteredCommands = new();
    /// <summary>
    /// Defines the <see cref="IsOpen"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsOpenProperty =
        AvaloniaProperty.Register<CommandPalette, bool>(nameof(IsOpen));

    /// <summary>
    /// Defines the <see cref="Commands"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IList<CommandItem>?> CommandsProperty =
        AvaloniaProperty.Register<CommandPalette, IList<CommandItem>?>(nameof(Commands));

    /// <summary>
    /// Defines the <see cref="SearchText"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> SearchTextProperty =
        AvaloniaProperty.Register<CommandPalette, string?>(nameof(SearchText));

    /// <summary>
    /// Defines the <see cref="Placeholder"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> PlaceholderProperty =
        AvaloniaProperty.Register<CommandPalette, string?>(nameof(Placeholder), "Type a command...");

    /// <summary>
    /// Defines the <see cref="MaxResults"/> styled property.
    /// </summary>
    public static readonly StyledProperty<int> MaxResultsProperty =
        AvaloniaProperty.Register<CommandPalette, int>(nameof(MaxResults), 15);

    /// <summary>
    /// Defines the <see cref="SelectedIndex"/> styled property.
    /// </summary>
    public static readonly StyledProperty<int> SelectedIndexProperty =
        AvaloniaProperty.Register<CommandPalette, int>(nameof(SelectedIndex));

    /// <summary>
    /// Defines the <see cref="FilterText"/> styled property.
    /// Displays the current filter mode (e.g. ">" for commands).
    /// </summary>
    public static readonly StyledProperty<string?> FilterTextProperty =
        AvaloniaProperty.Register<CommandPalette, string?>(nameof(FilterText));

    /// <summary>
    /// Defines the CommandExecuted routed event.
    /// </summary>
    public static readonly RoutedEvent<CommandExecutedEventArgs> CommandExecutedEvent =
        RoutedEvent.Register<CommandPalette, CommandExecutedEventArgs>(nameof(CommandExecuted), RoutingStrategies.Bubble);

    static CommandPalette()
    {
        IsOpenProperty.Changed.AddClassHandler<CommandPalette>((x, _) => x.OnIsOpenChanged());
        SearchTextProperty.Changed.AddClassHandler<CommandPalette>((x, _) => x.OnSearchTextChanged());
        SelectedIndexProperty.Changed.AddClassHandler<CommandPalette>((x, e) => x.OnSelectedIndexChanged());
    }

    /// <summary>
    /// Gets or sets whether the command palette is currently visible.
    /// </summary>
    public bool IsOpen
    {
        get => GetValue(IsOpenProperty);
        set => SetValue(IsOpenProperty, value);
    }

    /// <summary>
    /// Gets or sets the list of available commands.
    /// </summary>
    public IList<CommandItem>? Commands
    {
        get => GetValue(CommandsProperty);
        set => SetValue(CommandsProperty, value);
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
    /// Gets or sets a filter prefix (e.g. ">" to show only commands).
    /// </summary>
    public string? FilterText
    {
        get => GetValue(FilterTextProperty);
        set => SetValue(FilterTextProperty, value);
    }

    /// <summary>
    /// Gets the filtered list of commands currently displayed.
    /// </summary>
    public IReadOnlyList<CommandItem> FilteredCommands => _filteredCommands;

    /// <summary>
    /// Raised when a command is executed via the palette.
    /// </summary>
    public event EventHandler<CommandExecutedEventArgs>? CommandExecuted
    {
        add => AddHandler(CommandExecutedEvent, value);
        remove => RemoveHandler(CommandExecutedEvent, value);
    }

    /// <summary>
    /// Raised when the palette is opened.
    /// </summary>
    public event EventHandler? Opened;

    /// <summary>
    /// Raised when the palette is closed.
    /// </summary>
    public event EventHandler? Closed;

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        // Unsubscribe from old template parts
        if (_searchBox != null)
        {
            _searchBox.TextChanged -= OnSearchBoxTextChanged;
            _searchBox.KeyDown -= OnSearchBoxKeyDown;
        }
        if (_resultsList != null)
        {
            _resultsList.SelectionChanged -= OnResultsListSelectionChanged;
        }
        if (_overlay != null)
        {
            _overlay.PointerPressed -= OnOverlayPointerPressed;
        }

        // Subscribe to new template parts
        _searchBox = e.NameScope.Find<TextBox>("PART_SearchBox");
        _resultsList = e.NameScope.Find<ListBox>("PART_ResultsList");
        _overlay = e.NameScope.Find<Border>("PART_Overlay");

        if (_searchBox != null)
        {
            _searchBox.TextChanged += OnSearchBoxTextChanged;
            _searchBox.KeyDown += OnSearchBoxKeyDown;
        }

        if (_resultsList != null)
        {
            _resultsList.ItemsSource = _filteredCommands;
        }

        if (_overlay != null)
        {
            _overlay.PointerPressed += OnOverlayPointerPressed;
        }
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
        // Let the parent OnKeyDown handle navigation keys
        if (e.Key is Key.Up or Key.Down or Key.Enter or Key.Escape)
        {
            e.Handled = true;
            RaiseEvent(new KeyEventArgs
            {
                Key = e.Key,
                KeyModifiers = e.KeyModifiers,
                Route = RoutingStrategies.Bubble,
                Source = this
            });
            // Process locally
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

    private void OnResultsListSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (_resultsList?.SelectedIndex >= 0)
            SelectedIndex = _resultsList.SelectedIndex;
    }

    private void OnOverlayPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        // Close when clicking outside the palette
        Close();
    }

    private void OnIsOpenChanged()
    {
        UpdatePseudoClasses();

        if (IsOpen)
        {
            SearchText = null;
            _searchBox?.Focus();
            RefreshFilteredCommands();
            Opened?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            Closed?.Invoke(this, EventArgs.Empty);
        }
    }

    private void OnSearchTextChanged()
    {
        RefreshFilteredCommands();
    }

    private void OnSelectedIndexChanged()
    {
        if (_resultsList != null && SelectedIndex >= 0 && SelectedIndex < _filteredCommands.Count)
        {
            _resultsList.SelectedIndex = SelectedIndex;
            _resultsList.ScrollIntoView(SelectedIndex);
        }
    }

    private void RefreshFilteredCommands()
    {
        _filteredCommands.Clear();

        var allCommands = Commands;
        if (allCommands is null) return;

        var query = SearchText ?? string.Empty;
        IEnumerable<CommandItem> results;

        if (string.IsNullOrWhiteSpace(query))
        {
            // Show all commands, sorted alphabetically by category then name
            results = allCommands
                .Where(c => c.IsEnabled)
                .OrderBy(c => c.Category ?? string.Empty)
                .ThenBy(c => c.Name);
        }
        else
        {
            // Score and rank commands using fuzzy matching
            var scored = new List<(CommandItem item, double score)>();
            foreach (var cmd in allCommands)
            {
                if (!cmd.IsEnabled) continue;

                var score = CommandPaletteService.ComputeFuzzyScore(query, cmd.Name);
                if (cmd.Description != null)
                    score = Math.Max(score, CommandPaletteService.ComputeFuzzyScore(query, cmd.Description) * 0.8);
                if (cmd.Category != null)
                    score = Math.Max(score, CommandPaletteService.ComputeFuzzyScore(query, cmd.Category) * 0.5);

                if (score > 0)
                    scored.Add((cmd, score));
            }

            results = scored.OrderByDescending(x => x.score).Select(x => x.item);
        }

        foreach (var cmd in results.Take(MaxResults))
            _filteredCommands.Add(cmd);

        // Update pseudo classes
        PseudoClasses.Set(":has-results", _filteredCommands.Count > 0);
        PseudoClasses.Set(":no-results", _filteredCommands.Count == 0);

        // Reset selection
        SelectedIndex = _filteredCommands.Count > 0 ? 0 : -1;
    }

    private void NavigateUp()
    {
        if (_filteredCommands.Count == 0) return;
        SelectedIndex = SelectedIndex <= 0 ? _filteredCommands.Count - 1 : SelectedIndex - 1;
    }

    private void NavigateDown()
    {
        if (_filteredCommands.Count == 0) return;
        SelectedIndex = SelectedIndex >= _filteredCommands.Count - 1 ? 0 : SelectedIndex + 1;
    }

    private void ExecuteSelected()
    {
        if (SelectedIndex < 0 || SelectedIndex >= _filteredCommands.Count) return;

        var command = _filteredCommands[SelectedIndex];
        if (command.Execute())
        {
            RaiseEvent(new CommandExecutedEventArgs(command));
            Close();
        }
    }

    /// <summary>
    /// Opens the command palette.
    /// </summary>
    public void Open()
    {
        IsOpen = true;
    }

    /// <summary>
    /// Closes the command palette.
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
}
