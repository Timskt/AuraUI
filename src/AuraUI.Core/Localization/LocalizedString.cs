using System.ComponentModel;
using System.Globalization;

namespace AuraUI.Core.Localization;

/// <summary>
/// A bindable localized string that automatically raises
/// <see cref="INotifyPropertyChanged.PropertyChanged"/> when the active culture changes.
/// Bind this in XAML to keep UI text up-to-date without code-behind.
/// </summary>
/// <example>
/// <code>
/// // In a ViewModel:
/// public LocalizedString Greeting { get; } = new("welcome_message");
///
/// // In XAML:
/// &lt;TextBlock Text="{Binding Greeting.Value}"/&gt;
/// </code>
/// </example>
public class LocalizedString : INotifyPropertyChanged
{
    private readonly object[] _args;

    /// <summary>
    /// Creates a new <see cref="LocalizedString"/> for the given resource key.
    /// </summary>
    /// <param name="key">The resource key.</param>
    /// <param name="args">Optional format arguments.</param>
    public LocalizedString(string key, params object[] args)
    {
        Key = key ?? throw new ArgumentNullException(nameof(key));
        _args = args;

        LocalizationManager.Instance.CultureChanged += OnCultureChanged;
    }

    /// <summary>
    /// The resource key this instance looks up.
    /// </summary>
    public string Key { get; }

    /// <summary>
    /// The current localized, formatted value. Re-evaluated every time the culture changes.
    /// </summary>
    public string Value => LocalizationManager.Instance.GetString(Key, _args);

    /// <inheritdoc />
    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnCultureChanged(object? sender, CultureInfo e)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
    }
}
