using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using System.Text.Json;

namespace AuraUI.Core.Localization;

/// <summary>
/// Default implementation of <see cref="ILocalizationService"/>.
/// Stores string resources per culture and supports loading from dictionaries,
/// JSON strings, or embedded resource files shipped with the assembly.
/// </summary>
public class LocalizationManager : ILocalizationService
{
    private static LocalizationManager? _instance;

    /// <summary>
    /// Singleton instance used by <see cref="LocalizedString"/>, attached properties,
    /// and the <see cref="TranslateExtension"/>.
    /// </summary>
    public static LocalizationManager Instance => _instance ??= new LocalizationManager();

    private readonly Dictionary<string, Dictionary<string, string>> _resources = new(StringComparer.OrdinalIgnoreCase);
    private CultureInfo _currentCulture = new("en");

    /// <inheritdoc />
    public CultureInfo CurrentCulture => _currentCulture;

    /// <inheritdoc />
    public CultureInfo[] AvailableCultures => _resources.Keys
        .Select(k => new CultureInfo(k))
        .ToArray();

    /// <inheritdoc />
    public event EventHandler<CultureInfo>? CultureChanged;

    /// <summary>
    /// The fallback culture used when a key is missing from the current culture's resources.
    /// Defaults to <c>en</c>.
    /// </summary>
    public string DefaultCultureName { get; set; } = "en";

    // -------------------------------------------------------------------
    //  Loading
    // -------------------------------------------------------------------

    /// <summary>
    /// Loads (or merges) a set of key/value pairs for the given culture.
    /// </summary>
    public void LoadResources(CultureInfo culture, Dictionary<string, string> resources)
    {
        var name = culture.Name;
        if (!_resources.TryGetValue(name, out var existing))
        {
            existing = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            _resources[name] = existing;
        }

        foreach (var kvp in resources)
        {
            existing[kvp.Key] = kvp.Value;
        }
    }

    /// <summary>
    /// Deserializes a JSON object into key/value pairs and loads them for the given culture.
    /// The JSON must be a flat object whose values are strings.
    /// </summary>
    public void LoadFromJson(CultureInfo culture, string json)
    {
        var dict = JsonSerializer.Deserialize<Dictionary<string, string>>(json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (dict is not null)
        {
            LoadResources(culture, dict);
        }
    }

    /// <summary>
    /// Loads resources from a JSON file embedded in the AuraUI.Core assembly.
    /// The embedded resource name is matched by convention
    /// (<c>AuraUI.Core.Localization.Resources.&lt;fileName&gt;</c>).
    /// </summary>
    /// <param name="culture">The target culture.</param>
    /// <param name="fileName">
    /// The file name (e.g. <c>en.json</c>). The method searches for an embedded
    /// resource whose name ends with this value.
    /// </param>
    public void LoadFromEmbeddedResource(CultureInfo culture, string fileName)
    {
        var assembly = typeof(LocalizationManager).Assembly;
        var resourceName = assembly.GetManifestResourceNames()
            .FirstOrDefault(n => n.EndsWith(fileName, StringComparison.OrdinalIgnoreCase));

        if (resourceName is null)
            return;

        using var stream = assembly.GetManifestResourceStream(resourceName);
        if (stream is null) return;

        using var reader = new StreamReader(stream);
        var json = reader.ReadToEnd();
        LoadFromJson(culture, json);
    }

    /// <summary>
    /// Loads resources from a JSON file on disk.
    /// </summary>
    public void LoadFromResourceFile(CultureInfo culture, string filePath)
    {
        if (!File.Exists(filePath))
            return;

        var json = File.ReadAllText(filePath);
        LoadFromJson(culture, json);
    }

    /// <summary>
    /// Loads all built-in embedded resource files (en, zh-CN, ja, ko, de, fr, es, ar, he).
    /// Call this once at application startup.
    /// </summary>
    public void LoadBuiltInResources()
    {
        var builtIn = new[]
        {
            "en.json", "zh-CN.json", "ja.json", "ko.json",
            "de.json", "fr.json", "es.json", "ar.json", "he.json"
        };

        foreach (var file in builtIn)
        {
            var cultureName = Path.GetFileNameWithoutExtension(file);
            try
            {
                var culture = new CultureInfo(cultureName);
                LoadFromEmbeddedResource(culture, file);
            }
            catch (CultureNotFoundException)
            {
                // Skip files that don't map to a valid culture.
            }
        }
    }

    // -------------------------------------------------------------------
    //  Lookups
    // -------------------------------------------------------------------

    /// <inheritdoc />
    public string GetString(string key) => GetString(key, Array.Empty<object>());

    /// <inheritdoc />
    public string GetString(string key, params object[] args)
    {
        var format = ResolveString(key);

        if (args.Length == 0)
            return format;

        try
        {
            return string.Format(_currentCulture, format, args);
        }
        catch (FormatException)
        {
            return format;
        }
    }

    // -------------------------------------------------------------------
    //  Culture switching
    // -------------------------------------------------------------------

    /// <inheritdoc />
    public void SetCulture(CultureInfo culture)
    {
        if (culture.Name == _currentCulture.Name)
            return;

        _currentCulture = culture;
        CultureChanged?.Invoke(this, culture);
    }

    // -------------------------------------------------------------------
    //  Private helpers
    // -------------------------------------------------------------------

    private string ResolveString(string key)
    {
        // Try current culture
        if (_resources.TryGetValue(_currentCulture.Name, out var dict) &&
            dict.TryGetValue(key, out var value))
        {
            return value;
        }

        // Try parent culture (e.g. "de" when current is "de-AT")
        if (!string.IsNullOrEmpty(_currentCulture.Parent.Name) &&
            _currentCulture.Parent.Name != _currentCulture.Name &&
            _resources.TryGetValue(_currentCulture.Parent.Name, out var parentDict) &&
            parentDict.TryGetValue(key, out var parentValue))
        {
            return parentValue;
        }

        // Fallback to default culture
        if (_currentCulture.Name != DefaultCultureName &&
            _resources.TryGetValue(DefaultCultureName, out var defaultDict) &&
            defaultDict.TryGetValue(key, out var defaultValue))
        {
            return defaultValue;
        }

        // Return the key itself so missing translations are visible during development.
        return key;
    }
}
