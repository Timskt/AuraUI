using System.Globalization;

namespace AuraUI.Core.Extensions;

/// <summary>
/// Common string extension methods for truncation, null/whitespace checks, and
/// culture-aware title-casing.
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Truncates the string to the specified maximum length, appending a suffix if truncated.
    /// </summary>
    public static string Truncate(this string text, int maxLength, string suffix = "...")
    {
        ArgumentNullException.ThrowIfNull(text);
        ArgumentNullException.ThrowIfNull(suffix);

        if (maxLength < 0)
            throw new ArgumentOutOfRangeException(nameof(maxLength), "Maximum length must be non-negative.");

        if (text.Length <= maxLength)
            return text;

        int truncationLength = maxLength - suffix.Length;
        if (truncationLength <= 0)
            return suffix[..maxLength];

        return string.Concat(text.AsSpan(0, truncationLength), suffix);
    }

    /// <summary>
    /// Returns true if the string is null, empty, or consists only of whitespace characters.
    /// </summary>
    public static bool IsNullOrWhitespace(this string? text)
    {
        return string.IsNullOrWhiteSpace(text);
    }

    /// <summary>
    /// Converts the string to title case using the specified culture's TextInfo.
    /// </summary>
    public static string ToTitleCase(this string text, CultureInfo? culture = null)
    {
        ArgumentNullException.ThrowIfNull(text);

        if (text.Length == 0)
            return text;

        culture ??= CultureInfo.CurrentCulture;
        return culture.TextInfo.ToTitleCase(text.ToLower(culture));
    }
}
