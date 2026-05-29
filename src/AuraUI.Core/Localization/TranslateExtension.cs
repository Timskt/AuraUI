using System.Globalization;
using Avalonia.Data;
using Avalonia.Markup.Xaml;

namespace AuraUI.Core.Localization;

/// <summary>
/// XAML markup extension for localized strings that auto-update when the culture changes.
/// </summary>
/// <example>
/// <code>
/// &lt;!-- Simple key lookup --&gt;
/// &lt;TextBlock Text="{local:Translate welcome_message}"/&gt;
///
/// &lt;!-- With format arguments --&gt;
/// &lt;TextBlock Text="{local:Translate greeting, 'John'}"/&gt;
///
/// &lt;!-- Binding to a LocalizedString property --&gt;
/// &lt;TextBlock Text="{local:Translate page_of}" /&gt;
/// </code>
/// </example>
public class TranslateExtension : MarkupExtension
{
    /// <summary>
    /// The resource key to look up.
    /// </summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>
    /// Optional format arguments passed to
    /// <see cref="ILocalizationService.GetString(string, object[])"/>.
    /// </summary>
    public object[] Args { get; set; } = Array.Empty<object>();

    /// <summary>
    /// Creates a new <see cref="TranslateExtension"/> for the given key.
    /// </summary>
    public TranslateExtension() { }

    /// <summary>
    /// Creates a new <see cref="TranslateExtension"/> for the given key.
    /// </summary>
    public TranslateExtension(string key)
    {
        Key = key;
    }

    /// <inheritdoc />
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        // Return a binding to LocalizedString.Value so it auto-updates.
        var loc = new LocalizedString(Key, Args);
        return loc;
    }
}
