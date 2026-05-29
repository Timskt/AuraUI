using System.Text.Json;

namespace AuraUI.Core.Hooks;

/// <summary>
/// React-like hook that persists a value to local (file-based) storage.
/// Values are serialized as JSON and stored in the application's local data directory.
/// </summary>
/// <typeparam name="T">The type of the stored value.</typeparam>
/// <example>
/// <code>
/// // Persist a user preference:
/// using var pref = new UseLocalStorage&lt;int&gt;("fontSize", defaultValue: 14);
/// pref.Changed += () => Console.WriteLine($"Font size: {pref.Value}");
/// pref.Value = 16; // Automatically persisted
///
/// // With custom storage directory:
/// using var store = new UseLocalStorage&lt;string&gt;("username", storageDir: "/tmp/myapp");
/// </code>
/// </example>
public class UseLocalStorage<T> : IDisposable
{
    private readonly string _key;
    private readonly string _filePath;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly object _lock = new();
    private bool _disposed;
    private T _value;

    /// <summary>
    /// Gets or sets the current value. Setting this property persists the value to disk
    /// and raises the <see cref="Changed"/> event.
    /// </summary>
    public T Value
    {
        get => _value;
        set
        {
            if (EqualityComparer<T>.Default.Equals(_value, value))
                return;

            _value = value;
            Persist();
            Changed?.Invoke();
        }
    }

    /// <summary>
    /// Raised whenever <see cref="Value"/> changes.
    /// </summary>
    public event Action? Changed;

    /// <summary>
    /// Creates a new persistent state hook.
    /// </summary>
    /// <param name="key">A unique key that identifies this stored value.</param>
    /// <param name="defaultValue">The default value if nothing is stored yet.</param>
    /// <param name="storageDir">
    /// The directory where values are persisted.
    /// Defaults to <c>{AppContext.BaseDirectory}/.auraui/localStorage</c>.
    /// </param>
    public UseLocalStorage(string key, T defaultValue = default!, string? storageDir = null)
    {
        _key = key ?? throw new ArgumentNullException(nameof(key));

        var dir = storageDir ?? Path.Combine(AppContext.BaseDirectory, ".auraui", "localStorage");
        Directory.CreateDirectory(dir);

        // Sanitize key for use as filename
        var safeFileName = string.Join("_", key.Split(Path.GetInvalidFileNameChars()));
        _filePath = Path.Combine(dir, $"{safeFileName}.json");

        _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = false,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        _value = LoadFromDisk() ?? defaultValue;
    }

    /// <summary>
    /// Reloads the value from disk. Useful if another process may have changed it.
    /// </summary>
    public void Reload()
    {
        var loaded = LoadFromDisk();
        if (!EqualityComparer<T>.Default.Equals(_value, loaded))
        {
            _value = loaded!;
            Changed?.Invoke();
        }
    }

    /// <summary>
    /// Removes the persisted value from disk and resets to the default.
    /// </summary>
    public void Remove()
    {
        lock (_lock)
        {
            if (File.Exists(_filePath))
            {
                File.Delete(_filePath);
            }
        }

        _value = default!;
        Changed?.Invoke();
    }

    private T? LoadFromDisk()
    {
        try
        {
            lock (_lock)
            {
                if (!File.Exists(_filePath))
                    return default;

                var json = File.ReadAllText(_filePath);
                return JsonSerializer.Deserialize<T>(json, _jsonOptions);
            }
        }
        catch
        {
            return default;
        }
    }

    private void Persist()
    {
        try
        {
            lock (_lock)
            {
                var json = JsonSerializer.Serialize(_value, _jsonOptions);
                File.WriteAllText(_filePath, json);
            }
        }
        catch
        {
            // Silently fail on persistence errors (e.g. read-only filesystem)
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        GC.SuppressFinalize(this);
    }
}
