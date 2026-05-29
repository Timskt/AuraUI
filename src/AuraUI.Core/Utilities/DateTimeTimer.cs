using System.ComponentModel;
using System.Runtime.CompilerServices;
using Avalonia.Threading;

namespace AuraUI.Core.Utilities;

/// <summary>
/// A singleton timer that exposes the current <see cref="DateTime"/> as bindable properties,
/// updating every second via <see cref="DispatcherTimer"/>. Useful for clock displays in XAML.
/// </summary>
/// <example>
/// Bind to the singleton in XAML:
/// <code>
/// &lt;TextBlock Text="{x:Static u:DateTimeTimer.Instance.TimeString}" /&gt;
/// </code>
/// </example>
public class DateTimeTimer : INotifyPropertyChanged
{
    private static readonly Lazy<DateTimeTimer> _instance = new(() => new DateTimeTimer());

    /// <summary>
    /// Gets the shared singleton instance.
    /// </summary>
    public static DateTimeTimer Instance => _instance.Value;

    private DateTimeTimer()
    {
        var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
        timer.Tick += (_, _) =>
        {
            OnPropertyChanged(nameof(Now));
            OnPropertyChanged(nameof(TimeString));
            OnPropertyChanged(nameof(DateString));
        };
        timer.Start();
    }

    /// <summary>
    /// Gets the current local date and time.
    /// </summary>
    public DateTime Now => DateTime.Now;

    /// <summary>
    /// Gets the current time formatted as <c>HH:mm:ss</c>.
    /// </summary>
    public string TimeString => Now.ToString("HH:mm:ss");

    /// <summary>
    /// Gets the current date formatted as <c>yyyy-MM-dd</c>.
    /// </summary>
    public string DateString => Now.ToString("yyyy-MM-dd");

    /// <inheritdoc />
    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
