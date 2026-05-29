using Avalonia;
using Avalonia.Controls;

namespace AuraUI.Core.Helpers;

/// <summary>
/// Attached properties for localizing the text content of common controls.
/// Works on <see cref="TextBlock"/>, <see cref="Button"/>, and
/// <see cref="ContentControl"/>.
/// Text auto-updates when the active culture changes.
/// </summary>
/// <example>
/// <code>
/// &lt;TextBlock LocalizationHelper.LocalizeKey="welcome_message"/&gt;
/// &lt;Button LocalizationHelper.LocalizeKey="save"/&gt;
/// &lt;TextBlock LocalizationHelper.LocalizeKey="page_of"
///            LocalizationHelper.LocalizeFormat="1,5"/&gt;
/// </code>
/// </example>
public static class LocalizationHelper
{
    #region LocalizeKey

    /// <summary>
    /// The resource key whose localized value should be applied to the control's text property.
    /// </summary>
    public static readonly AttachedProperty<string?> LocalizeKeyProperty =
        AvaloniaProperty.RegisterAttached<Control, string?>("LocalizeKey", typeof(LocalizationHelper));

    public static string? GetLocalizeKey(Control element) => element.GetValue(LocalizeKeyProperty);
    public static void SetLocalizeKey(Control element, string? value) => element.SetValue(LocalizeKeyProperty, value);

    #endregion

    #region LocalizeFormat

    /// <summary>
    /// A comma-separated list of format arguments to pass alongside the key.
    /// </summary>
    public static readonly AttachedProperty<string?> LocalizeFormatProperty =
        AvaloniaProperty.RegisterAttached<Control, string?>("LocalizeFormat", typeof(LocalizationHelper));

    public static string? GetLocalizeFormat(Control element) => element.GetValue(LocalizeFormatProperty);
    public static void SetLocalizeFormat(Control element, string? value) => element.SetValue(LocalizeFormatProperty, value);

    #endregion

    // Weak references to controls using localization so we can update on culture change.
    private static readonly HashSet<WeakReference<Control>> _tracked = new();

    static LocalizationHelper()
    {
        LocalizeKeyProperty.Changed.AddClassHandler<Control>(OnKeyChanged);
        LocalizeFormatProperty.Changed.AddClassHandler<Control>(OnFormatChanged);
        Localization.LocalizationManager.Instance.CultureChanged += (_, _) => RefreshAll();
    }

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
        _tracked.RemoveWhere(w => !w.TryGetTarget(out _));

        foreach (var wr in _tracked)
        {
            if (wr.TryGetTarget(out var existing) && existing == control)
                return;
        }

        _tracked.Add(new WeakReference<Control>(control));
    }

    private static void RefreshAll()
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
        var key = GetLocalizeKey(control);
        if (string.IsNullOrEmpty(key))
            return;

        var formatStr = GetLocalizeFormat(control);
        object[] args = Array.Empty<object>();

        if (!string.IsNullOrEmpty(formatStr))
        {
            args = formatStr.Split(',')
                .Select(s => (object)s.Trim())
                .ToArray();
        }

        var text = Localization.LocalizationManager.Instance.GetString(key, args);

        switch (control)
        {
            case TextBlock tb:
                tb.Text = text;
                break;
            case Button btn:
                btn.Content = text;
                break;
            case ContentControl cc:
                cc.Content = text;
                break;
        }
    }
}
