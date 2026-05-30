namespace AuraUI.Core.Logging;

/// <summary>
/// Defines the severity levels for log messages.
/// </summary>
public enum AuraLogLevel
{
    /// <summary>Detailed diagnostic messages for development and troubleshooting.</summary>
    Debug,

    /// <summary>General informational messages about application flow.</summary>
    Info,

    /// <summary>Potential issues that do not prevent normal operation.</summary>
    Warning,

    /// <summary>Errors that affect a specific operation but do not crash the application.</summary>
    Error,

    /// <summary>Critical errors that may cause the application to terminate.</summary>
    Fatal
}

/// <summary>
/// Represents a structured log entry with metadata.
/// </summary>
public sealed class LogEntry
{
    /// <summary>Gets the UTC timestamp when the log entry was created.</summary>
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;

    /// <summary>Gets the severity level of the log entry.</summary>
    public AuraLogLevel Level { get; init; }

    /// <summary>Gets the log message.</summary>
    public string Message { get; init; } = string.Empty;

    /// <summary>Gets the optional exception associated with the log entry.</summary>
    public Exception? Exception { get; init; }

    /// <summary>Gets optional structured key-value properties.</summary>
    public IReadOnlyDictionary<string, object?> Properties { get; init; } = new Dictionary<string, object?>();

    /// <summary>Gets the source category or component that produced the log entry.</summary>
    public string? Category { get; init; }

    /// <summary>Returns a formatted string representation of the log entry.</summary>
    public override string ToString()
    {
        var ts = Timestamp.ToString("yyyy-MM-dd HH:mm:ss.fff");
        var cat = string.IsNullOrEmpty(Category) ? "" : $" [{Category}]";
        var msg = $"[{ts}] [{Level}]{cat} {Message}";

        if (Exception is not null)
            msg += $"{Environment.NewLine}{Exception}";

        if (Properties.Count > 0)
        {
            foreach (var (key, value) in Properties)
            {
                msg += $"{Environment.NewLine}  {key} = {value}";
            }
        }

        return msg;
    }
}

/// <summary>
/// Contract for logging within the AuraUI framework.
/// Implementations may write to Debug output, files, or custom sinks.
/// </summary>
public interface IAuraLogger
{
    /// <summary>Logs a detailed diagnostic message.</summary>
    void Debug(string message);

    /// <summary>Logs a structured diagnostic message with key-value properties.</summary>
    void Debug(string message, IReadOnlyDictionary<string, object?> properties);

    /// <summary>Logs a general informational message.</summary>
    void Info(string message);

    /// <summary>Logs a structured informational message with key-value properties.</summary>
    void Info(string message, IReadOnlyDictionary<string, object?> properties);

    /// <summary>Logs a warning message.</summary>
    void Warning(string message);

    /// <summary>Logs a structured warning message with key-value properties.</summary>
    void Warning(string message, IReadOnlyDictionary<string, object?> properties);

    /// <summary>Logs an error message, optionally associated with an exception.</summary>
    void Error(string message, Exception? ex = null);

    /// <summary>Logs a structured error message with key-value properties and an optional exception.</summary>
    void Error(string message, IReadOnlyDictionary<string, object?> properties, Exception? ex = null);

    /// <summary>Logs a fatal/critical message, optionally associated with an exception.</summary>
    void Fatal(string message, Exception? ex = null);

    /// <summary>Logs a structured fatal message with key-value properties and an optional exception.</summary>
    void Fatal(string message, IReadOnlyDictionary<string, object?> properties, Exception? ex = null);
}

/// <summary>
/// A delegate that receives <see cref="LogEntry"/> instances for custom log processing.
/// </summary>
/// <param name="entry">The log entry to process.</param>
public delegate void LogSink(LogEntry entry);

/// <summary>
/// Default implementation of <see cref="IAuraLogger"/> that routes log output to
/// <see cref="System.Diagnostics.Debug"/>, an optional file, and/or custom sinks.
/// Supports configurable minimum log levels and structured logging.
/// </summary>
public class AuraLogger : IAuraLogger
{
    private readonly AuraLogLevel _minimumLevel;
    private readonly string? _category;
    private readonly List<LogSink> _sinks = new();
    private readonly object _sinkLock = new();
    private readonly string? _filePath;

    /// <summary>
    /// Creates a new <see cref="AuraLogger"/> with the specified minimum log level.
    /// </summary>
    /// <param name="minimumLevel">The minimum severity level to log. Messages below this level are discarded.</param>
    /// <param name="category">Optional source category name prepended to log messages.</param>
    public AuraLogger(AuraLogLevel minimumLevel = AuraLogLevel.Info, string? category = null)
    {
        _minimumLevel = minimumLevel;
        _category = category;
    }

    /// <summary>
    /// Creates a new <see cref="AuraLogger"/> that also writes to a file.
    /// </summary>
    /// <param name="filePath">The path to the log file.</param>
    /// <param name="minimumLevel">The minimum severity level to log.</param>
    /// <param name="category">Optional source category name.</param>
    public AuraLogger(string filePath, AuraLogLevel minimumLevel = AuraLogLevel.Info, string? category = null)
        : this(minimumLevel, category)
    {
        _filePath = filePath;
    }

    /// <summary>
    /// Registers a custom sink that receives all log entries at or above the minimum level.
    /// </summary>
    /// <param name="sink">The sink delegate to invoke for each log entry.</param>
    public void AddSink(LogSink sink)
    {
        if (sink is null) throw new ArgumentNullException(nameof(sink));

        lock (_sinkLock)
        {
            _sinks.Add(sink);
        }
    }

    /// <summary>
    /// Removes a previously registered custom sink.
    /// </summary>
    /// <param name="sink">The sink delegate to remove.</param>
    public void RemoveSink(LogSink sink)
    {
        lock (_sinkLock)
        {
            _sinks.Remove(sink);
        }
    }

    #region IAuraLogger Implementation

    /// <inheritdoc />
    public void Debug(string message) => Log(AuraLogLevel.Debug, message);

    /// <inheritdoc />
    public void Debug(string message, IReadOnlyDictionary<string, object?> properties) =>
        Log(AuraLogLevel.Debug, message, properties: properties);

    /// <inheritdoc />
    public void Info(string message) => Log(AuraLogLevel.Info, message);

    /// <inheritdoc />
    public void Info(string message, IReadOnlyDictionary<string, object?> properties) =>
        Log(AuraLogLevel.Info, message, properties: properties);

    /// <inheritdoc />
    public void Warning(string message) => Log(AuraLogLevel.Warning, message);

    /// <inheritdoc />
    public void Warning(string message, IReadOnlyDictionary<string, object?> properties) =>
        Log(AuraLogLevel.Warning, message, properties: properties);

    /// <inheritdoc />
    public void Error(string message, Exception? ex = null) => Log(AuraLogLevel.Error, message, ex);

    /// <inheritdoc />
    public void Error(string message, IReadOnlyDictionary<string, object?> properties, Exception? ex = null) =>
        Log(AuraLogLevel.Error, message, ex, properties);

    /// <inheritdoc />
    public void Fatal(string message, Exception? ex = null) => Log(AuraLogLevel.Fatal, message, ex);

    /// <inheritdoc />
    public void Fatal(string message, IReadOnlyDictionary<string, object?> properties, Exception? ex = null) =>
        Log(AuraLogLevel.Fatal, message, ex, properties);

    #endregion

    #region Core Logging

    private void Log(
        AuraLogLevel level,
        string message,
        Exception? exception = null,
        IReadOnlyDictionary<string, object?>? properties = null)
    {
        if (level < _minimumLevel)
            return;

        var entry = new LogEntry
        {
            Timestamp = DateTime.UtcNow,
            Level = level,
            Message = message,
            Exception = exception,
            Properties = properties ?? new Dictionary<string, object?>(),
            Category = _category
        };

        // Always write to Debug output (visible in IDE and debuggers)
        System.Diagnostics.Debug.WriteLine(entry.ToString());

        // Write to file if configured
        if (!string.IsNullOrEmpty(_filePath))
        {
            try
            {
                System.IO.File.AppendAllText(_filePath, entry.ToString() + Environment.NewLine);
            }
            catch
            {
                // Silently swallow file I/O errors to avoid cascading failures
            }
        }

        // Dispatch to registered sinks
        LogSink[] sinkCopy;
        lock (_sinkLock)
        {
            if (_sinks.Count == 0) return;
            sinkCopy = _sinks.ToArray();
        }

        foreach (var sink in sinkCopy)
        {
            try
            {
                sink(entry);
            }
            catch
            {
                // Prevent sink exceptions from affecting other sinks or the caller
            }
        }
    }

    #endregion
}
