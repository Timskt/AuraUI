using System.Globalization;

namespace AuraUI.Core.Localization;

/// <summary>
/// Provides localization services for retrieving culture-specific strings
/// and managing the active UI culture at runtime.
/// </summary>
public interface ILocalizationService
{
    /// <summary>
    /// Gets the currently active culture used for string lookups and formatting.
    /// </summary>
    CultureInfo CurrentCulture { get; }

    /// <summary>
    /// Gets the set of cultures for which translation resources have been loaded.
    /// </summary>
    CultureInfo[] AvailableCultures { get; }

    /// <summary>
    /// Switches the active culture. All bound <see cref="LocalizedString"/> instances
    /// and attached-property-driven controls will update automatically.
    /// </summary>
    /// <param name="culture">The target culture.</param>
    void SetCulture(CultureInfo culture);

    /// <summary>
    /// Looks up a localized string by key using the current culture.
    /// Falls back to the default (en) culture when the key is missing.
    /// </summary>
    /// <param name="key">The resource key.</param>
    /// <returns>The localized string, or the key itself if not found.</returns>
    string GetString(string key);

    /// <summary>
    /// Looks up a localized string by key and formats it with the supplied arguments
    /// using the current culture.
    /// </summary>
    /// <param name="key">The resource key.</param>
    /// <param name="args">Format arguments.</param>
    /// <returns>The formatted localized string.</returns>
    string GetString(string key, params object[] args);

    /// <summary>
    /// Raised after the active culture has changed.
    /// </summary>
    event EventHandler<CultureInfo>? CultureChanged;
}
