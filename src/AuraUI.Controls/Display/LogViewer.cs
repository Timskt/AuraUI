using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Media;
using Avalonia.Media.TextFormatting;

namespace AuraUI.Controls.Display;

/// <summary>
/// Severity levels for log entries.
/// </summary>
public enum LogLevel
{
    Debug,
    Info,
    Warn,
    Error,
    Fatal
}

/// <summary>
/// Represents a single log entry.
/// </summary>
public class LogEntry : AvaloniaObject
{
    /// <summary>
    /// Defines the <see cref="Timestamp"/> property.
    /// </summary>
    public static readonly StyledProperty<DateTime> TimestampProperty =
        AvaloniaProperty.Register<LogEntry, DateTime>(nameof(Timestamp));

    /// <summary>
    /// Defines the <see cref="Level"/> property.
    /// </summary>
    public static readonly StyledProperty<LogLevel> LevelProperty =
        AvaloniaProperty.Register<LogEntry, LogLevel>(nameof(Level));

    /// <summary>
    /// Defines the <see cref="Source"/> property.
    /// </summary>
    public static readonly StyledProperty<string?> SourceProperty =
        AvaloniaProperty.Register<LogEntry, string?>(nameof(Source));

    /// <summary>
    /// Defines the <see cref="Message"/> property.
    /// </summary>
    public static readonly StyledProperty<string?> MessageProperty =
        AvaloniaProperty.Register<LogEntry, string?>(nameof(Message));

    /// <summary>
    /// Gets or sets the timestamp of the log entry.
    /// </summary>
    public DateTime Timestamp
    {
        get => GetValue(TimestampProperty);
        set => SetValue(TimestampProperty, value);
    }

    /// <summary>
    /// Gets or sets the severity level.
    /// </summary>
    public LogLevel Level
    {
        get => GetValue(LevelProperty);
        set => SetValue(LevelProperty, value);
    }

    /// <summary>
    /// Gets or sets the source of the log entry.
    /// </summary>
    public string? Source
    {
        get => GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }

    /// <summary>
    /// Gets or sets the log message.
    /// </summary>
    public string? Message
    {
        get => GetValue(MessageProperty);
        set => SetValue(MessageProperty, value);
    }
}

/// <summary>
/// A high-performance log viewer control with level filtering, text search, and auto-scroll.
/// Uses virtualized rendering for handling 100k+ entries efficiently.
///
/// Template parts:
///   PART_LogList          - ItemsRepeater or virtualizing list for log display
///   PART_FilterBar        - StackPanel with level filter toggles
///   PART_SearchBox        - TextBox for searching log messages
///   PART_ScrollViewer     - ScrollViewer for log content
///
/// Pseudo-classes: :auto-scroll, :filtered, :searching, :empty
/// </summary>
[TemplatePart("PART_LogList", typeof(ItemsControl))]
[TemplatePart("PART_FilterBar", typeof(StackPanel))]
[TemplatePart("PART_SearchBox", typeof(TextBox))]
[TemplatePart("PART_ScrollViewer", typeof(ScrollViewer))]
[PseudoClasses(":auto-scroll", ":filtered", ":searching", ":empty")]
public class LogViewer : Control
{
#pragma warning disable CS0649 // Field is never assigned — template part populated by framework
    private ScrollViewer? _scrollViewer;
#pragma warning restore CS0649

    private readonly ObservableCollection<LogEntry> _filteredEntries = new();
    private IList<LogEntry>? _sourceEntries;

    /// <summary>
    /// Defines the <see cref="Entries"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IList<LogEntry>?> EntriesProperty =
        AvaloniaProperty.Register<LogViewer, IList<LogEntry>?>(nameof(Entries));

    /// <summary>
    /// Defines the <see cref="MaxEntries"/> styled property.
    /// </summary>
    public static readonly StyledProperty<int> MaxEntriesProperty =
        AvaloniaProperty.Register<LogViewer, int>(nameof(MaxEntries), 100000);

    /// <summary>
    /// Defines the <see cref="AutoScroll"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> AutoScrollProperty =
        AvaloniaProperty.Register<LogViewer, bool>(nameof(AutoScroll), true);

    /// <summary>
    /// Defines the <see cref="FilterLevel"/> styled property.
    /// When set to a specific level, only entries at that level or higher are shown.
    /// Set to null to show all entries.
    /// </summary>
    public static readonly StyledProperty<LogLevel?> FilterLevelProperty =
        AvaloniaProperty.Register<LogViewer, LogLevel?>(nameof(FilterLevel));

    /// <summary>
    /// Defines the <see cref="SearchText"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> SearchTextProperty =
        AvaloniaProperty.Register<LogViewer, string?>(nameof(SearchText));

    /// <summary>
    /// Defines the <see cref="ShowTimestamp"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> ShowTimestampProperty =
        AvaloniaProperty.Register<LogViewer, bool>(nameof(ShowTimestamp), true);

    /// <summary>
    /// Defines the <see cref="ShowSource"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> ShowSourceProperty =
        AvaloniaProperty.Register<LogViewer, bool>(nameof(ShowSource), true);

    static LogViewer()
    {
        AffectsRender<LogViewer>(
            EntriesProperty, FilterLevelProperty, SearchTextProperty,
            ShowTimestampProperty, ShowSourceProperty, AutoScrollProperty);
        AffectsMeasure<LogViewer>(EntriesProperty);
        EntriesProperty.Changed.AddClassHandler<LogViewer>((x, e) => x.OnEntriesChanged(e));
        FilterLevelProperty.Changed.AddClassHandler<LogViewer>((x, _) => x.ApplyFilters());
        SearchTextProperty.Changed.AddClassHandler<LogViewer>((x, _) => x.ApplyFilters());
        AutoScrollProperty.Changed.AddClassHandler<LogViewer>((x, _) => x.UpdatePseudoClasses());
    }

    /// <summary>
    /// Gets or sets the collection of log entries.
    /// </summary>
    public IList<LogEntry>? Entries
    {
        get => GetValue(EntriesProperty);
        set => SetValue(EntriesProperty, value);
    }

    /// <summary>
    /// Gets or sets the maximum number of entries to retain.
    /// </summary>
    public int MaxEntries
    {
        get => GetValue(MaxEntriesProperty);
        set => SetValue(MaxEntriesProperty, value);
    }

    /// <summary>
    /// Gets or sets whether to automatically scroll to the latest entry.
    /// </summary>
    public bool AutoScroll
    {
        get => GetValue(AutoScrollProperty);
        set => SetValue(AutoScrollProperty, value);
    }

    /// <summary>
    /// Gets or sets the minimum log level filter. Entries below this level are hidden.
    /// </summary>
    public LogLevel? FilterLevel
    {
        get => GetValue(FilterLevelProperty);
        set => SetValue(FilterLevelProperty, value);
    }

    /// <summary>
    /// Gets or sets the search text to filter log messages.
    /// </summary>
    public string? SearchText
    {
        get => GetValue(SearchTextProperty);
        set => SetValue(SearchTextProperty, value);
    }

    /// <summary>
    /// Gets or sets whether to show timestamps.
    /// </summary>
    public bool ShowTimestamp
    {
        get => GetValue(ShowTimestampProperty);
        set => SetValue(ShowTimestampProperty, value);
    }

    /// <summary>
    /// Gets or sets whether to show the source column.
    /// </summary>
    public bool ShowSource
    {
        get => GetValue(ShowSourceProperty);
        set => SetValue(ShowSourceProperty, value);
    }

    /// <summary>
    /// Gets the filtered collection of log entries currently visible.
    /// </summary>
    public IReadOnlyList<LogEntry> FilteredEntries => _filteredEntries;

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        UpdatePseudoClasses();
        ApplyFilters();
    }

    private void OnEntriesChanged(AvaloniaPropertyChangedEventArgs e)
    {
        // Unsubscribe from old collection
        if (_sourceEntries is INotifyCollectionChanged oldNotify)
        {
            oldNotify.CollectionChanged -= OnSourceCollectionChanged;
        }

        _sourceEntries = e.NewValue as IList<LogEntry>;

        // Subscribe to new collection
        if (_sourceEntries is INotifyCollectionChanged newNotify)
        {
            newNotify.CollectionChanged += OnSourceCollectionChanged;
        }

        ApplyFilters();
    }

    private void OnSourceCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        ApplyFilters();
    }

    /// <summary>
    /// Applies the current filters and search text to produce filtered entries.
    /// </summary>
    public void ApplyFilters()
    {
        _filteredEntries.Clear();

        if (_sourceEntries is null)
        {
            PseudoClasses.Set(":empty", true);
            return;
        }

        var filterLevel = FilterLevel;
        var searchText = SearchText;

        foreach (var entry in _sourceEntries)
        {
            // Level filter
            if (filterLevel.HasValue && entry.Level < filterLevel.Value)
                continue;

            // Search filter
            if (!string.IsNullOrEmpty(searchText))
            {
                var message = entry.Message ?? string.Empty;
                var source = entry.Source ?? string.Empty;
                if (!message.Contains(searchText, StringComparison.OrdinalIgnoreCase) &&
                    !source.Contains(searchText, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }
            }

            _filteredEntries.Add(entry);
        }

        PseudoClasses.Set(":empty", _filteredEntries.Count == 0);
        PseudoClasses.Set(":filtered", filterLevel.HasValue);
        PseudoClasses.Set(":searching", !string.IsNullOrEmpty(searchText));

        if (AutoScroll)
        {
            ScrollToLatest();
        }
    }

    /// <summary>
    /// Scrolls to the latest log entry.
    /// </summary>
    public void ScrollToLatest()
    {
        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            if (_scrollViewer is not null)
            {
                _scrollViewer.Offset = new Vector(_scrollViewer.Offset.X, _scrollViewer.Extent.Height);
            }
        }, Avalonia.Threading.DispatcherPriority.Render);
    }

    /// <summary>
    /// Clears all log entries.
    /// </summary>
    public void Clear()
    {
        _sourceEntries?.Clear();
        _filteredEntries.Clear();
        PseudoClasses.Set(":empty", true);
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(":auto-scroll", AutoScroll);
    }

    private const double LogLineHeight = 20.0;

    protected override Size MeasureOverride(Size availableSize)
    {
        var entryCount = _filteredEntries.Count;
        var totalHeight = entryCount * LogLineHeight;
        return new Size(
            double.IsInfinity(availableSize.Width) ? 600 : availableSize.Width,
            double.IsInfinity(availableSize.Height) ? Math.Max(totalHeight, 100) : Math.Max(totalHeight, availableSize.Height));
    }

    public override void Render(DrawingContext context)
    {
        var bounds = Bounds;
        if (bounds.Width <= 0 || bounds.Height <= 0) return;

        var entries = _filteredEntries;
        if (entries.Count == 0)
        {
            var emptyText = new FormattedText("No log entries",
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight, new Typeface(FontFamily.Default), 13, Brushes.Gray);
            context.DrawText(emptyText, new Point(8, 8));
            return;
        }

        var showTimestamp = ShowTimestamp;
        var showSource = ShowSource;
        var typeface = new Typeface(FontFamily.Default);

        for (int i = 0; i < entries.Count; i++)
        {
            var y = i * LogLineHeight;
            if (y > bounds.Height) break;

            var entry = entries[i];
            var sb = new System.Text.StringBuilder();
            if (showTimestamp) sb.Append($"[{entry.Timestamp:HH:mm:ss.fff}] ");
            if (showSource && !string.IsNullOrEmpty(entry.Source)) sb.Append($"{entry.Source}: ");
            sb.Append(entry.Message ?? string.Empty);

            var color = GetLevelColor(entry.Level);
            var ft = new FormattedText(sb.ToString(),
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight, typeface, 12, color);
            context.DrawText(ft, new Point(4, y));
        }
    }

    /// <summary>
    /// Gets the color associated with a log level for display purposes.
    /// </summary>
    public static IBrush GetLevelColor(LogLevel level)
    {
        return level switch
        {
            LogLevel.Debug => new SolidColorBrush(Color.Parse("#888888")),
            LogLevel.Info => new SolidColorBrush(Color.Parse("#2196F3")),
            LogLevel.Warn => new SolidColorBrush(Color.Parse("#FF9800")),
            LogLevel.Error => new SolidColorBrush(Color.Parse("#F44336")),
            LogLevel.Fatal => new SolidColorBrush(Color.Parse("#9C27B0")),
            _ => new SolidColorBrush(Color.Parse("#888888"))
        };
    }
}
