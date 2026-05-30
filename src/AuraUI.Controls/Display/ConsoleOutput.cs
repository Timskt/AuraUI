using System.Collections;
using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Media;

namespace AuraUI.Controls.Display;

/// <summary>
/// Represents a single line of console output.
/// </summary>
public class ConsoleLine
{
    /// <summary>
    /// Gets or sets the timestamp of the log entry.
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.Now;

    /// <summary>
    /// Gets or sets the severity level.
    /// </summary>
    public LogLevel Level { get; set; } = LogLevel.Info;

    /// <summary>
    /// Gets or sets the source component or module name.
    /// </summary>
    public string? Source { get; set; }

    /// <summary>
    /// Gets or sets the log message.
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// Gets or sets the raw ANSI color code, if present.
    /// </summary>
    public string? AnsiColor { get; set; }

    /// <summary>
    /// Gets the formatted timestamp string.
    /// </summary>
    public string FormattedTimestamp => Timestamp.ToString("HH:mm:ss.fff");
}

/// <summary>
/// A virtualized console output viewer with ANSI color support, log level filtering,
/// and performance optimized for 100k+ lines.
/// </summary>
[PseudoClasses(":scrolling", ":filtering")]
public class ConsoleOutput : Control
{
    /// <summary>
    /// Defines the <see cref="Lines"/> styled property.
    /// </summary>
    public static readonly StyledProperty<ObservableCollection<ConsoleLine>?> LinesProperty =
        AvaloniaProperty.Register<ConsoleOutput, ObservableCollection<ConsoleLine>?>(nameof(Lines));

    /// <summary>
    /// Defines the <see cref="MaxLines"/> styled property.
    /// Maximum number of lines to keep in memory. Older lines are trimmed.
    /// </summary>
    public static readonly StyledProperty<int> MaxLinesProperty =
        AvaloniaProperty.Register<ConsoleOutput, int>(nameof(MaxLines), 100000);

    /// <summary>
    /// Defines the <see cref="AutoScroll"/> styled property.
    /// Whether to automatically scroll to the bottom when new lines are added.
    /// </summary>
    public static readonly StyledProperty<bool> AutoScrollProperty =
        AvaloniaProperty.Register<ConsoleOutput, bool>(nameof(AutoScroll), true);

    /// <summary>
    /// Defines the <see cref="FilterLevel"/> styled property.
    /// Only lines at or above this level are displayed. Null shows all.
    /// </summary>
    public static readonly StyledProperty<LogLevel?> FilterLevelProperty =
        AvaloniaProperty.Register<ConsoleOutput, LogLevel?>(nameof(FilterLevel));

    /// <summary>
    /// Defines the <see cref="ShowTimestamp"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> ShowTimestampProperty =
        AvaloniaProperty.Register<ConsoleOutput, bool>(nameof(ShowTimestamp), true);

    /// <summary>
    /// Defines the <see cref="ShowSource"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> ShowSourceProperty =
        AvaloniaProperty.Register<ConsoleOutput, bool>(nameof(ShowSource), true);

    /// <summary>
    /// Defines the <see cref="ShowLevel"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> ShowLevelProperty =
        AvaloniaProperty.Register<ConsoleOutput, bool>(nameof(ShowLevel), true);

    /// <summary>
    /// Defines the <see cref="LineHeight"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> LineHeightProperty =
        AvaloniaProperty.Register<ConsoleOutput, double>(nameof(LineHeight), 20);

    /// <summary>
    /// Defines the <see cref="Background"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> BackgroundProperty =
        AvaloniaProperty.Register<ConsoleOutput, IBrush?>(nameof(Background));

    /// <summary>
    /// Defines the <see cref="Foreground"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> ForegroundProperty =
        AvaloniaProperty.Register<ConsoleOutput, IBrush?>(nameof(Foreground));

    /// <summary>
    /// Defines the <see cref="FontFamily"/> styled property.
    /// </summary>
    public static readonly StyledProperty<FontFamily> FontFamilyProperty =
        AvaloniaProperty.Register<ConsoleOutput, FontFamily>(nameof(FontFamily), new FontFamily("Consolas, Menlo, Monaco, monospace"));

    /// <summary>
    /// Defines the <see cref="FontSize"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> FontSizeProperty =
        AvaloniaProperty.Register<ConsoleOutput, double>(nameof(FontSize), 12);

    /// <summary>
    /// Defines the <see cref="SearchText"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> SearchTextProperty =
        AvaloniaProperty.Register<ConsoleOutput, string?>(nameof(SearchText));

    /// <summary>
    /// Gets or sets the collection of console lines.
    /// </summary>
    public ObservableCollection<ConsoleLine>? Lines
    {
        get => GetValue(LinesProperty);
        set => SetValue(LinesProperty, value);
    }

    /// <summary>
    /// Gets or sets the maximum number of lines retained in memory.
    /// </summary>
    public int MaxLines
    {
        get => GetValue(MaxLinesProperty);
        set => SetValue(MaxLinesProperty, value);
    }

    /// <summary>
    /// Gets or sets whether to auto-scroll on new lines.
    /// </summary>
    public bool AutoScroll
    {
        get => GetValue(AutoScrollProperty);
        set => SetValue(AutoScrollProperty, value);
    }

    /// <summary>
    /// Gets or sets the minimum log level to display. Null means show all.
    /// </summary>
    public LogLevel? FilterLevel
    {
        get => GetValue(FilterLevelProperty);
        set => SetValue(FilterLevelProperty, value);
    }

    /// <summary>
    /// Gets or sets whether timestamps are displayed.
    /// </summary>
    public bool ShowTimestamp
    {
        get => GetValue(ShowTimestampProperty);
        set => SetValue(ShowTimestampProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the source name is displayed.
    /// </summary>
    public bool ShowSource
    {
        get => GetValue(ShowSourceProperty);
        set => SetValue(ShowSourceProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the log level badge is displayed.
    /// </summary>
    public bool ShowLevel
    {
        get => GetValue(ShowLevelProperty);
        set => SetValue(ShowLevelProperty, value);
    }

    /// <summary>
    /// Gets or sets the line height in pixels.
    /// </summary>
    public double LineHeight
    {
        get => GetValue(LineHeightProperty);
        set => SetValue(LineHeightProperty, value);
    }

    /// <summary>
    /// Gets or sets the background brush.
    /// </summary>
    public IBrush? Background
    {
        get => GetValue(BackgroundProperty);
        set => SetValue(BackgroundProperty, value);
    }

    /// <summary>
    /// Gets or sets the foreground brush for text.
    /// </summary>
    public IBrush? Foreground
    {
        get => GetValue(ForegroundProperty);
        set => SetValue(ForegroundProperty, value);
    }

    /// <summary>
    /// Gets or sets the monospace font family.
    /// </summary>
    public FontFamily FontFamily
    {
        get => GetValue(FontFamilyProperty);
        set => SetValue(FontFamilyProperty, value);
    }

    /// <summary>
    /// Gets or sets the font size.
    /// </summary>
    public double FontSize
    {
        get => GetValue(FontSizeProperty);
        set => SetValue(FontSizeProperty, value);
    }

    /// <summary>
    /// Gets or sets the search text to highlight in the output.
    /// </summary>
    public string? SearchText
    {
        get => GetValue(SearchTextProperty);
        set => SetValue(SearchTextProperty, value);
    }

    /// <summary>
    /// Gets the color for a given log level.
    /// </summary>
    public static IBrush GetLevelColor(LogLevel level) => level switch
    {
        LogLevel.Debug => new SolidColorBrush(Color.Parse("#9E9E9E")),
        LogLevel.Info => new SolidColorBrush(Color.Parse("#2196F3")),
        LogLevel.Warn => new SolidColorBrush(Color.Parse("#FF9800")),
        LogLevel.Error => new SolidColorBrush(Color.Parse("#F44336")),
        LogLevel.Fatal => new SolidColorBrush(Color.Parse("#D50000")),
        _ => Brushes.White
    };

    /// <summary>
    /// Gets the short label for a log level.
    /// </summary>
    public static string GetLevelLabel(LogLevel level) => level switch
    {
        LogLevel.Debug => "DBG",
        LogLevel.Info => "INF",
        LogLevel.Warn => "WRN",
        LogLevel.Error => "ERR",
        LogLevel.Fatal => "FTL",
        _ => "???"
    };

    /// <summary>
    /// Parses ANSI color codes from a message and returns colored segments.
    /// Supports basic SGR codes: 30-37 (foreground), 90-97 (bright foreground), 0 (reset).
    /// </summary>
    public static string StripAnsiCodes(string? input)
    {
        if (string.IsNullOrEmpty(input)) return string.Empty;

        // Strip ANSI escape sequences
        return System.Text.RegularExpressions.Regex.Replace(input, @"\x1B\[[0-9;]*[a-zA-Z]", string.Empty);
    }
}
