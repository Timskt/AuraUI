using Avalonia;
using Avalonia.Controls;

namespace AuraUI.Core.Localization;

/// <summary>
/// Attached properties that allow localizing the text content of any
/// <see cref="TextBlock"/> or <see cref="ContentControl"/> directly from XAML.
/// </summary>
/// <example>
/// <code>
/// &lt;TextBlock local:StringResource.Key="welcome_message"/&gt;
/// &lt;TextBlock local:StringResource.Key="page_of" local:StringResource.Format="1,5"/&gt;
/// </code>
/// </example>
public static class StringResource
{
    #region Key

    /// <summary>
    /// The localization resource key to look up.
    /// </summary>
    public static readonly AttachedProperty<string?> KeyProperty =
        AvaloniaProperty.RegisterAttached<Control, string?>("Key", typeof(StringResource));

    public static string? GetKey(Control element) => element.GetValue(KeyProperty);
    public static void SetKey(Control element, string? value) => element.SetValue(KeyProperty, value);

    #endregion

    #region Format

    /// <summary>
    /// A comma-separated list of format arguments to pass to
    /// <see cref="ILocalizationService.GetString(string, object[])"/>.
    /// </summary>
    public static readonly AttachedProperty<string?> FormatProperty =
        AvaloniaProperty.RegisterAttached<Control, string?>("Format", typeof(StringResource));

    public static string? GetFormat(Control element) => element.GetValue(FormatProperty);
    public static void SetFormat(Control element, string? value) => element.SetValue(FormatProperty, value);

    #endregion

    static StringResource()
    {
        KeyProperty.Changed.AddClassHandler<Control>(OnKeyChanged);
        FormatProperty.Changed.AddClassHandler<Control>(OnFormatChanged);
        LocalizationManager.Instance.CultureChanged += (_, _) => UpdateAllPending();
    }

    // WeakHashSet to track controls using StringResource so we can update them on culture change.
    private static readonly HashSet<WeakReference<Control>> _tracked = new();

    private static void OnKeyChanged(Control control, AvaloniaPropertyChangedEventArgs e)
    {
        TrackControl(control);
        ApplyLocalization(control);
    }

    private static void OnFormatChanged(Control control, AvaloniaPropertyChangedEventArgs e)
    {
        ApplyLocalization(control);
    }

    private static void TrackControl(Control control)
    {
        // Remove stale references
        _tracked.RemoveWhere(w => !w.TryGetTarget(out _));

        // Add if not already tracked
        var exists = false;
        foreach (var wr in _tracked)
        {
            if (wr.TryGetTarget(out var existing) && existing == control)
            {
                exists = true;
                break;
            }
        }

        if (!exists)
            _tracked.Add(new WeakReference<Control>(control));
    }

    private static void UpdateAllPending()
    {
        _tracked.RemoveWhere(w => !w.TryGetTarget(out _));
        foreach (var wr in _tracked)
        {
            if (wr.TryGetTarget(out var control))
                ApplyLocalization(control);
        }
    }

    private static void ApplyLocalization(Control control)
    {
        var key = GetKey(control);
        if (string.IsNullOrEmpty(key))
            return;

        var formatStr = GetFormat(control);
        object[] args = Array.Empty<object>();

        if (!string.IsNullOrEmpty(formatStr))
        {
            args = formatStr.Split(',')
                .Select(s => (object)s.Trim())
                .ToArray();
        }

        var text = LocalizationManager.Instance.GetString(key, args);

        switch (control)
        {
            case TextBlock tb:
                tb.Text = text;
                break;
            case ContentControl cc:
                cc.Content = text;
                break;
        }
    }
}
