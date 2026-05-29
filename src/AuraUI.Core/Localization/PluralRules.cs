using System.Globalization;

namespace AuraUI.Core.Localization;

/// <summary>
/// Provides language-specific pluralization rules for common languages.
/// Handles the CLDR plural categories: zero, one, two, few, many, and other.
/// </summary>
/// <remarks>
/// Rules are based on the Unicode CLDR data
/// (https://unicode.org/cldr/charts/latest/supplemental/language_plural_rules.html).
/// </remarks>
public static class PluralRules
{
    /// <summary>
    /// Returns the appropriate string based on the count and the active culture's
    /// plural rules. For languages without complex plural forms (e.g. Chinese, Japanese)
    /// only <paramref name="other"/> is used.
    /// </summary>
    /// <param name="zero">Form for zero items (used by Arabic, etc.).</param>
    /// <param name="one">Form for exactly one item.</param>
    /// <param name="two">Form for exactly two items (used by Arabic, Hebrew, etc.).</param>
    /// <param name="few">Form for few items (used by Arabic, Russian, Polish, etc.).</param>
    /// <param name="many">Form for many items (used by Arabic, Russian, etc.).</param>
    /// <param name="other">General / fallback form.</param>
    /// <param name="count">The numeric count.</param>
    /// <param name="culture">The culture to use for rule selection.</param>
    public static string Select(
        string zero, string one, string two, string few, string many, string other,
        int count, CultureInfo culture)
    {
        var form = GetPluralForm(count, culture);
        return form switch
        {
            "zero" => zero,
            "one" => one,
            "two" => two,
            "few" => few,
            "many" => many,
            _ => other
        };
    }

    /// <summary>
    /// Simplified overload for languages with only singular/plural distinction.
    /// </summary>
    public static string Pluralize(string singular, string plural, int count, CultureInfo culture)
    {
        var form = GetPluralForm(count, culture);
        return form == "one" ? singular : plural;
    }

    /// <summary>
    /// Returns the CLDR plural category for the given count and culture.
    /// </summary>
    /// <returns>One of: "zero", "one", "two", "few", "many", "other".</returns>
    public static string GetPluralForm(int count, CultureInfo culture)
    {
        var lang = culture.TwoLetterISOLanguageName;

        return lang switch
        {
            "ar" => ArabicRule(count),
            "ru" or "uk" or "be" => SlavicRule(count),
            "pl" => PolishRule(count),
            "cs" or "sk" => CzechRule(count),
            "ro" => RomanianRule(count),
            "lt" => LithuanianRule(count),
            "lv" => LatvianRule(count),
            "ga" => IrishRule(count),
            "he" or "iw" => HebrewRule(count),
            "fr" or "pt" or "es" or "it" or "de" or "en" or "nl" or "sv" or "da" or "no" or "nb" or "nn" =>
                WesternRule(count),
            "ja" or "ko" or "zh" or "vi" or "th" or "id" or "ms" or "tr" or "hu" or "fi" or "et" =>
                OtherOnly(),
            _ => WesternRule(count)
        };
    }

    // -------------------------------------------------------------------
    //  Language-specific rules
    // -------------------------------------------------------------------

    private static string WesternRule(int count) => count == 1 ? "one" : "other";

    private static string OtherOnly() => "other";

    private static string ArabicRule(int count)
    {
        if (count == 0) return "zero";
        if (count == 1) return "one";
        if (count == 2) return "two";
        var mod100 = count % 100;
        if (mod100 >= 3 && mod100 <= 10) return "few";
        if (mod100 >= 11 && mod100 <= 99) return "many";
        return "other";
    }

    private static string SlavicRule(int count)
    {
        var mod10 = count % 10;
        var mod100 = count % 100;

        if (mod10 == 1 && mod100 != 11) return "one";
        if (mod10 >= 2 && mod10 <= 4 && (mod100 < 12 || mod100 > 14)) return "few";
        if (mod10 == 0 || (mod10 >= 5 && mod10 <= 9) || (mod100 >= 11 && mod100 <= 14)) return "many";
        return "other";
    }

    private static string PolishRule(int count)
    {
        var mod10 = count % 10;
        var mod100 = count % 100;

        if (count == 1) return "one";
        if (mod10 >= 2 && mod10 <= 4 && (mod100 < 12 || mod100 > 14)) return "few";
        if (mod10 == 0 || mod10 == 1 || (mod10 >= 5 && mod10 <= 9) || (mod100 >= 12 && mod100 <= 14)) return "many";
        return "other";
    }

    private static string CzechRule(int count)
    {
        if (count == 1) return "one";
        if (count >= 2 && count <= 4) return "few";
        return "other";
    }

    private static string RomanianRule(int count)
    {
        if (count == 1) return "one";
        var mod100 = count % 100;
        if (count == 0 || (mod100 >= 2 && mod100 <= 19)) return "few";
        return "other";
    }

    private static string LithuanianRule(int count)
    {
        var mod10 = count % 10;
        var mod100 = count % 100;

        if (mod10 == 1 && (mod100 < 11 || mod100 > 19)) return "one";
        if (mod10 >= 2 && mod10 <= 9 && (mod100 < 11 || mod100 > 19)) return "few";
        return "other";
    }

    private static string LatvianRule(int count)
    {
        if (count == 0) return "zero";
        var mod10 = count % 10;
        var mod100 = count % 100;
        if (mod10 == 1 && mod100 != 11) return "one";
        return "other";
    }

    private static string IrishRule(int count)
    {
        if (count == 1) return "one";
        if (count == 2) return "two";
        if (count >= 3 && count <= 6) return "few";
        if (count >= 7 && count <= 10) return "many";
        return "other";
    }

    private static string HebrewRule(int count)
    {
        if (count == 1) return "one";
        if (count == 2) return "two";
        return "other";
    }
}
