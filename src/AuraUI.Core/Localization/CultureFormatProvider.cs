using System.Globalization;

namespace AuraUI.Core.Localization;

/// <summary>
/// An <see cref="IFormatProvider"/> that delegates formatting to the
/// <see cref="LocalizationManager"/>'s current culture. Use this when calling
/// <c>ToString</c> or <c>string.Format</c> to ensure culture-aware output
/// for numbers, dates, currencies, and percentages.
/// </summary>
/// <example>
/// <code>
/// var provider = new CultureFormatProvider();
/// var formatted = string.Format(provider, "{0:C2}", 1234.56); // culture-aware currency
/// </code>
/// </example>
public class CultureFormatProvider : IFormatProvider
{
    private readonly LocalizationManager _manager;

    /// <summary>
    /// Creates a new instance backed by <see cref="LocalizationManager.Instance"/>.
    /// </summary>
    public CultureFormatProvider() : this(LocalizationManager.Instance) { }

    /// <summary>
    /// Creates a new instance backed by the specified <see cref="LocalizationManager"/>.
    /// </summary>
    public CultureFormatProvider(LocalizationManager manager)
    {
        _manager = manager ?? throw new ArgumentNullException(nameof(manager));
    }

    /// <inheritdoc />
    public object? GetFormat(Type? formatType)
    {
        if (formatType == typeof(NumberFormatInfo))
            return _manager.CurrentCulture.NumberFormat;

        if (formatType == typeof(DateTimeFormatInfo))
            return _manager.CurrentCulture.DateTimeFormat;

        return _manager.CurrentCulture.GetFormat(formatType);
    }

    // -------------------------------------------------------------------
    //  Convenience methods
    // -------------------------------------------------------------------

    /// <summary>
    /// Formats a number using the current culture.
    /// </summary>
    /// <param name="value">The number to format.</param>
    /// <param name="format">
    /// A standard or custom numeric format string (e.g. <c>"N2"</c>, <c>"C"</c>, <c>"P"</c>).
    /// Defaults to the general format.
    /// </param>
    public string FormatNumber(double value, string? format = null)
        => value.ToString(format ?? "G", _manager.CurrentCulture);

    /// <summary>
    /// Formats a <see cref="DateTime"/> using the current culture.
    /// </summary>
    /// <param name="value">The date/time to format.</param>
    /// <param name="format">
    /// A standard or custom date/time format string (e.g. <c>"d"</c>, <c>"D"</c>, <c>"f"</c>).
    /// Defaults to the general short format.
    /// </param>
    public string FormatDateTime(DateTime value, string? format = null)
        => value.ToString(format ?? "g", _manager.CurrentCulture);

    /// <summary>
    /// Formats a value as currency using the current culture.
    /// </summary>
    /// <param name="value">The amount.</param>
    /// <param name="decimalPlaces">Number of decimal places. Defaults to 2.</param>
    public string FormatCurrency(decimal value, int decimalPlaces = 2)
        => value.ToString($"C{decimalPlaces}", _manager.CurrentCulture);

    /// <summary>
    /// Formats a value as a percentage using the current culture.
    /// </summary>
    /// <param name="value">The value (e.g. 0.75 for 75%).</param>
    /// <param name="decimalPlaces">Number of decimal places. Defaults to 1.</param>
    public string FormatPercentage(double value, int decimalPlaces = 1)
        => value.ToString($"P{decimalPlaces}", _manager.CurrentCulture);
}
