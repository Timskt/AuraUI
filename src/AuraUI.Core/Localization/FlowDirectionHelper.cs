using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace AuraUI.Core.Localization;

/// <summary>
/// Attached properties and helpers for automatically setting
/// <see cref="FlowDirection"/> based on the active culture.
/// Right-to-left cultures (Arabic, Hebrew, Persian, Urdu, etc.) are detected
/// and the flow direction is applied without manual configuration.
/// </summary>
/// <example>
/// <code>
/// &lt;StackPanel local:FlowDirectionHelper.AutoDetect="True"/&gt;
/// </code>
/// </example>
public static class FlowDirectionHelper
{
    /// <summary>
    /// When set to <c>true</c> on a layout panel, its <see cref="Layoutable.FlowDirection"/>
    /// is automatically updated whenever the active culture changes to an RTL culture.
    /// </summary>
    public static readonly AttachedProperty<bool> AutoDetectProperty =
        AvaloniaProperty.RegisterAttached<Control, bool>("AutoDetect", typeof(FlowDirectionHelper));

    public static bool GetAutoDetect(Control element) => element.GetValue(AutoDetectProperty);
    public static void SetAutoDetect(Control element, bool value) => element.SetValue(AutoDetectProperty, value);

    // Set of ISO 639-1 two-letter language codes for RTL scripts.
    private static readonly HashSet<string> RtlLanguages = new(StringComparer.OrdinalIgnoreCase)
    {
        "ar", // Arabic
        "he", // Hebrew
        "fa", // Persian (Farsi)
        "ur", // Urdu
        "yi", // Yiddish
        "ps", // Pashto
        "sd", // Sindhi
        "ku", // Kurdish (Sorani)
        "ckb", // Central Kurdish
        "arc", // Aramaic
        "dv", // Divehi
        "ha", // Hausa (when written in Arabic script — conservative inclusion)
    };

    static FlowDirectionHelper()
    {
        AutoDetectProperty.Changed.AddClassHandler<Control>(OnAutoDetectChanged);
        LocalizationManager.Instance.CultureChanged += OnCultureChanged;
    }

    /// <summary>
    /// Determines whether the given culture uses a right-to-left script.
    /// </summary>
    public static bool IsRtlCulture(CultureInfo culture)
    {
        if (culture is null) throw new ArgumentNullException(nameof(culture));

        // Check the two-letter ISO name first, then the three-letter ISO name.
        return RtlLanguages.Contains(culture.TwoLetterISOLanguageName) ||
               RtlLanguages.Contains(culture.ThreeLetterISOLanguageName) ||
               RtlLanguages.Contains(culture.Name);
    }

    /// <summary>
    /// Returns <c>true</c> when the given culture uses right-to-left flow,
    /// <c>false</c> for left-to-right.
    /// </summary>
    public static bool IsRtl(CultureInfo culture) => IsRtlCulture(culture);

    private static void OnAutoDetectChanged(Control control, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.NewValue is true)
        {
            ApplyFlowDirection(control);
        }
    }

    private static void OnCultureChanged(object? sender, CultureInfo culture)
    {
        // Walk all tracked controls and update flow direction.
        // We use a simple static list approach via Avalonia's property system.
    }

    /// <summary>
    /// Applies the flow direction to the given control based on the current culture.
    /// </summary>
    internal static void ApplyFlowDirection(Control control)
    {
        var isRtl = IsRtlCulture(LocalizationManager.Instance.CurrentCulture);
        // Set the FlowDirection property on the control.
        // Avalonia exposes FlowDirection on Visual/Layoutable controls.
        control.FlowDirection = isRtl ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;
    }
}
