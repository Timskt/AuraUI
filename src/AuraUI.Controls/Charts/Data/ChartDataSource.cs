using System.Collections.Specialized;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace AuraUI.Controls.Charts.Data;

/// <summary>
/// Abstract base class for chart data sources. A data source provides data rows
/// to a <see cref="ChartDataset"/> from various origins: static lists, observable
/// streams, remote URLs, CSV files, or JSON payloads.
/// </summary>
public abstract class ChartDataSource
{
    /// <summary>Display name for this data source.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Raised when new data is available from the source.</summary>
    public event EventHandler<DataAvailableEventArgs>? DataAvailable;

    /// <summary>Raised when the data source encounters an error.</summary>
    public event EventHandler<DataErrorEventArgs>? Error;

    /// <summary>Fetch data rows from this source.</summary>
    public abstract Task<IList<DataRow>> FetchAsync(CancellationToken cancellationToken = default);

    /// <summary>Notify listeners that new data is available.</summary>
    protected void OnDataAvailable(IList<DataRow> rows)
    {
        DataAvailable?.Invoke(this, new DataAvailableEventArgs(rows));
    }

    /// <summary>Notify listeners of an error.</summary>
    protected void OnError(Exception exception)
    {
        Error?.Invoke(this, new DataErrorEventArgs(exception));
    }
}

/// <summary>
/// Event args for data availability.
/// </summary>
public class DataAvailableEventArgs : EventArgs
{
    public IList<DataRow> Rows { get; }

    public DataAvailableEventArgs(IList<DataRow> rows)
    {
        Rows = rows;
    }
}

/// <summary>
/// Event args for data source errors.
/// </summary>
public class DataErrorEventArgs : EventArgs
{
    public Exception Exception { get; }

    public DataErrorEventArgs(Exception exception)
    {
        Exception = exception;
    }
}

/// <summary>
/// A static data source backed by an in-memory list.
/// </summary>
public class StaticDataSource : ChartDataSource
{
    private readonly List<DataRow> _data;

    public StaticDataSource()
    {
        _data = new List<DataRow>();
        Name = "Static";
    }

    public StaticDataSource(IEnumerable<DataRow> data)
    {
        _data = data.ToList();
        Name = "Static";
    }

    public override Task<IList<DataRow>> FetchAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IList<DataRow>>(_data.ToList());
    }

    /// <summary>Replace the data and notify.</summary>
    public void Update(IEnumerable<DataRow> newData)
    {
        _data.Clear();
        _data.AddRange(newData);
        OnDataAvailable(_data);
    }
}

/// <summary>
/// A dynamic data source backed by an observable stream for real-time data.
/// Subscribes to an IObservable and buffers incoming data.
/// </summary>
public class DynamicDataSource : ChartDataSource, IDisposable
{
    private IDisposable? _subscription;
    private readonly List<DataRow> _buffer = new();
    private readonly object _lock = new();

    public DynamicDataSource()
    {
        Name = "Dynamic";
    }

    /// <summary>
    /// Subscribe to an observable data stream.
    /// Each emission is a batch of rows.
    /// </summary>
    public void Subscribe(IObservable<IList<DataRow>> observable)
    {
        _subscription?.Dispose();
        _subscription = observable.Subscribe(new BatchObserver(this));
    }

    /// <summary>
    /// Subscribe to an observable of single rows.
    /// </summary>
    public void Subscribe(IObservable<DataRow> observable)
    {
        _subscription?.Dispose();
        _subscription = observable.Subscribe(new SingleRowObserver(this));
    }

    private sealed class BatchObserver : IObserver<IList<DataRow>>
    {
        private readonly DynamicDataSource _owner;
        public BatchObserver(DynamicDataSource owner) => _owner = owner;
        public void OnNext(IList<DataRow> rows)
        {
            lock (_owner._lock) { _owner._buffer.AddRange(rows); }
            _owner.OnDataAvailable(rows);
        }
        public void OnError(Exception error) => _owner.OnError(error);
        public void OnCompleted() { }
    }

    private sealed class SingleRowObserver : IObserver<DataRow>
    {
        private readonly DynamicDataSource _owner;
        public SingleRowObserver(DynamicDataSource owner) => _owner = owner;
        public void OnNext(DataRow row)
        {
            var batch = new List<DataRow> { row };
            lock (_owner._lock) { _owner._buffer.Add(row); }
            _owner.OnDataAvailable(batch);
        }
        public void OnError(Exception error) => _owner.OnError(error);
        public void OnCompleted() { }
    }

    public override Task<IList<DataRow>> FetchAsync(CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            return Task.FromResult<IList<DataRow>>(_buffer.ToList());
        }
    }

    /// <summary>Clear the buffer.</summary>
    public void ClearBuffer()
    {
        lock (_lock)
        {
            _buffer.Clear();
        }
    }

    public void Dispose()
    {
        _subscription?.Dispose();
        _subscription = null;
        GC.SuppressFinalize(this);
    }
}

/// <summary>
/// A remote data source that loads data asynchronously from a URL.
/// Supports JSON and CSV response formats.
/// </summary>
public partial class RemoteDataSource : ChartDataSource
{
    private static readonly HttpClient SharedHttpClient = new();

    /// <summary>The URL to fetch data from.</summary>
    public string Url { get; set; } = string.Empty;

    /// <summary>The response format (Json or Csv).</summary>
    public DataFormat Format { get; set; } = DataFormat.Json;

    /// <summary>Optional HTTP headers to include in the request.</summary>
    public Dictionary<string, string> Headers { get; } = new();

    /// <summary>Request timeout.</summary>
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);

    /// <summary>
    /// For JSON format: path to the array of records in the JSON response.
    /// e.g., "data.results" for { "data": { "results": [...] } }
    /// When null, the top-level value is used (must be a JSON array).
    /// </summary>
    public string? JsonPath { get; set; }

    public RemoteDataSource()
    {
        Name = "Remote";
    }

    public RemoteDataSource(string url, DataFormat format = DataFormat.Json)
    {
        Url = url;
        Format = format;
        Name = "Remote";
    }

    public override async Task<IList<DataRow>> FetchAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, Url);
            foreach (var header in Headers)
            {
                request.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cts.CancelAfter(Timeout);

            var response = await SharedHttpClient.SendAsync(request, cts.Token);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync(cts.Token);

            var rows = Format switch
            {
                DataFormat.Json => ParseJson(content),
                DataFormat.Csv => ParseCsv(content),
                _ => throw new NotSupportedException($"Format {Format} is not supported.")
            };

            OnDataAvailable(rows);
            return rows;
        }
        catch (Exception ex)
        {
            OnError(ex);
            throw;
        }
    }

    private IList<DataRow> ParseJson(string json)
    {
        using var doc = JsonDocument.Parse(json);
        var element = doc.RootElement;

        // Navigate to the specified path
        if (!string.IsNullOrEmpty(JsonPath))
        {
            foreach (var segment in JsonPath.Split('.'))
            {
                if (element.TryGetProperty(segment, out var child))
                    element = child;
                else
                    throw new JsonException($"JSON path segment '{segment}' not found.");
            }
        }

        if (element.ValueKind != JsonValueKind.Array)
            throw new JsonException("JSON root (or path target) must be an array.");

        var rows = new List<DataRow>();
        foreach (var item in element.EnumerateArray())
        {
            var row = new DataRow();
            foreach (var prop in item.EnumerateObject())
            {
                row[prop.Name] = prop.Value.ValueKind switch
                {
                    JsonValueKind.Number => prop.Value.GetDouble(),
                    JsonValueKind.String => prop.Value.GetString(),
                    JsonValueKind.True => true,
                    JsonValueKind.False => false,
                    JsonValueKind.Null => null,
                    _ => prop.Value.ToString()
                };
            }
            rows.Add(row);
        }

        return rows;
    }

    private static IList<DataRow> ParseCsv(string csv)
    {
        var lines = csv.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        if (lines.Length < 2) return new List<DataRow>();

        var headers = ParseCsvLine(lines[0]);
        var rows = new List<DataRow>();

        for (int i = 1; i < lines.Length; i++)
        {
            var values = ParseCsvLine(lines[i]);
            var row = new DataRow();
            for (int j = 0; j < headers.Length && j < values.Length; j++)
            {
                var val = values[j].Trim();
                if (double.TryParse(val, System.Globalization.NumberStyles.Any,
                        System.Globalization.CultureInfo.InvariantCulture, out var numVal))
                    row[headers[j]] = numVal;
                else if (bool.TryParse(val, out var boolVal))
                    row[headers[j]] = boolVal;
                else
                    row[headers[j]] = val;
            }
            rows.Add(row);
        }

        return rows;
    }

    private static string[] ParseCsvLine(string line)
    {
        var result = new List<string>();
        var current = new StringBuilder();
        bool inQuotes = false;

        for (int i = 0; i < line.Length; i++)
        {
            var c = line[i];
            if (c == '"')
            {
                if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                {
                    current.Append('"');
                    i++;
                }
                else
                {
                    inQuotes = !inQuotes;
                }
            }
            else if (c == ',' && !inQuotes)
            {
                result.Add(current.ToString());
                current.Clear();
            }
            else
            {
                current.Append(c);
            }
        }
        result.Add(current.ToString());

        return result.ToArray();
    }
}

/// <summary>
/// Supported data formats for remote and file data sources.
/// </summary>
public enum DataFormat
{
    /// <summary>JSON format.</summary>
    Json,

    /// <summary>CSV format.</summary>
    Csv
}

/// <summary>
/// A file-based data source that loads CSV or JSON from a local file path.
/// </summary>
public class FileDataSource : ChartDataSource
{
    /// <summary>The file path to load.</summary>
    public string FilePath { get; set; } = string.Empty;

    /// <summary>The file format.</summary>
    public DataFormat Format { get; set; } = DataFormat.Csv;

    public FileDataSource()
    {
        Name = "File";
    }

    public FileDataSource(string filePath, DataFormat format = DataFormat.Csv)
    {
        FilePath = filePath;
        Format = format;
        Name = "File";
    }

    public override async Task<IList<DataRow>> FetchAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var content = await File.ReadAllTextAsync(FilePath, cancellationToken);

            var rows = Format switch
            {
                DataFormat.Json => RemoteDataSource.ParseJsonStatic(content),
                DataFormat.Csv => RemoteDataSource.ParseCsvStatic(content),
                _ => throw new NotSupportedException($"Format {Format} is not supported.")
            };

            OnDataAvailable(rows);
            return rows;
        }
        catch (Exception ex)
        {
            OnError(ex);
            throw;
        }
    }
}

// Provide internal static access to parsing methods for FileDataSource
public partial class RemoteDataSource
{
    internal static IList<DataRow> ParseJsonStatic(string json)
    {
        using var doc = JsonDocument.Parse(json);
        var element = doc.RootElement;

        if (element.ValueKind != JsonValueKind.Array)
            throw new JsonException("JSON root must be an array.");

        var rows = new List<DataRow>();
        foreach (var item in element.EnumerateArray())
        {
            var row = new DataRow();
            foreach (var prop in item.EnumerateObject())
            {
                row[prop.Name] = prop.Value.ValueKind switch
                {
                    JsonValueKind.Number => prop.Value.GetDouble(),
                    JsonValueKind.String => prop.Value.GetString(),
                    JsonValueKind.True => true,
                    JsonValueKind.False => false,
                    JsonValueKind.Null => null,
                    _ => prop.Value.ToString()
                };
            }
            rows.Add(row);
        }

        return rows;
    }

    internal static IList<DataRow> ParseCsvStatic(string csv)
    {
        var lines = csv.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        if (lines.Length < 2) return new List<DataRow>();

        var headers = ParseCsvLine(lines[0]);
        var rows = new List<DataRow>();

        for (int i = 1; i < lines.Length; i++)
        {
            var values = ParseCsvLine(lines[i]);
            var row = new DataRow();
            for (int j = 0; j < headers.Length && j < values.Length; j++)
            {
                var val = values[j].Trim();
                if (double.TryParse(val, System.Globalization.NumberStyles.Any,
                        System.Globalization.CultureInfo.InvariantCulture, out var numVal))
                    row[headers[j]] = numVal;
                else if (bool.TryParse(val, out var boolVal))
                    row[headers[j]] = boolVal;
                else
                    row[headers[j]] = val;
            }
            rows.Add(row);
        }

        return rows;
    }
}
