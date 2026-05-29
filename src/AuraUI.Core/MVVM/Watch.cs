using System.ComponentModel;

namespace AuraUI.Core.MVVM;

/// <summary>
/// Vue-like watch system that reacts to property changes on
/// <see cref="INotifyPropertyChanged"/> sources. Provides static factory methods
/// for watching single or multiple properties with automatic cleanup via
/// <see cref="IDisposable"/>.
/// </summary>
/// <example>
/// <code>
/// // Watch a single property:
/// using var w = Watch.Property&lt;string&gt;(viewModel, nameof(viewModel.Name), newName =>
/// {
///     Console.WriteLine($"Name changed to: {newName}");
/// });
///
/// // Watch multiple properties:
/// using var w = Watch.Properties(viewModel,
///     new[] { nameof(viewModel.Width), nameof(viewModel.Height) },
///     () => Console.WriteLine("Size changed"));
///
/// // Watch with a filter (only notify when condition is met):
/// using var w = Watch.Property&lt;int&gt;(viewModel, nameof(viewModel.Count), count =>
/// {
///     Console.WriteLine($"Count is now {count}");
/// }, count => count > 10);
///
/// // Deep watch - watches all properties:
/// using var w = Watch.AllProperties(viewModel, () => Console.WriteLine("Something changed"));
/// </code>
/// </example>
public static class Watch
{
    /// <summary>
    /// Watches a single property and invokes the callback with the new value when it changes.
    /// </summary>
    /// <typeparam name="T">The property type.</typeparam>
    /// <param name="source">The object to watch.</param>
    /// <param name="propertyName">The name of the property to watch.</param>
    /// <param name="callback">Invoked with the new value when the property changes.</param>
    /// <param name="filter">
    /// An optional predicate. The callback is only invoked when the filter returns <c>true</c>.
    /// </param>
    /// <returns>An <see cref="IDisposable"/> that unsubscribes when disposed.</returns>
    public static IDisposable Property<T>(
        INotifyPropertyChanged source,
        string propertyName,
        Action<T> callback,
        Func<T, bool>? filter = null)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(propertyName);
        ArgumentNullException.ThrowIfNull(callback);

        return new PropertyWatcher<T>(source, propertyName, callback, filter);
    }

    /// <summary>
    /// Watches multiple properties and invokes the callback when any of them change.
    /// </summary>
    /// <param name="source">The object to watch.</param>
    /// <param name="propertyNames">The names of the properties to watch.</param>
    /// <param name="callback">Invoked when any of the watched properties change.</param>
    /// <returns>An <see cref="IDisposable"/> that unsubscribes when disposed.</returns>
    public static IDisposable Properties(
        INotifyPropertyChanged source,
        string[] propertyNames,
        Action callback)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(propertyNames);
        ArgumentNullException.ThrowIfNull(callback);

        return new MultiPropertyWatcher(source, propertyNames, callback);
    }

    /// <summary>
    /// Watches all properties on the source and invokes the callback when any property changes.
    /// </summary>
    /// <param name="source">The object to watch.</param>
    /// <param name="callback">Invoked when any property changes.</param>
    /// <returns>An <see cref="IDisposable"/> that unsubscribes when disposed.</returns>
    public static IDisposable AllProperties(
        INotifyPropertyChanged source,
        Action callback)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(callback);

        return new AllPropertiesWatcher(source, callback);
    }

    /// <summary>
    /// Watches a property and invokes an async callback when it changes.
    /// </summary>
    /// <typeparam name="T">The property type.</typeparam>
    /// <param name="source">The object to watch.</param>
    /// <param name="propertyName">The name of the property to watch.</param>
    /// <param name="callback">An async callback invoked with the new value.</param>
    /// <returns>An <see cref="IDisposable"/> that unsubscribes when disposed.</returns>
    public static IDisposable PropertyAsync<T>(
        INotifyPropertyChanged source,
        string propertyName,
        Func<T, Task> callback)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(propertyName);
        ArgumentNullException.ThrowIfNull(callback);

        return new AsyncPropertyWatcher<T>(source, propertyName, callback);
    }

    #region Private watcher implementations

    private sealed class PropertyWatcher<T> : IDisposable
    {
        private readonly INotifyPropertyChanged _source;
        private readonly string _propertyName;
        private readonly Action<T> _callback;
        private readonly Func<T, bool>? _filter;
        private bool _disposed;

        public PropertyWatcher(INotifyPropertyChanged source, string propertyName,
            Action<T> callback, Func<T, bool>? filter)
        {
            _source = source;
            _propertyName = propertyName;
            _callback = callback;
            _filter = filter;
            _source.PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != _propertyName && e.PropertyName is not null) return;

            // Try to get the property value
            var prop = _source.GetType().GetProperty(_propertyName);
            if (prop is null || !prop.CanRead) return;

            var value = prop.GetValue(_source);
            if (value is T typed)
            {
                if (_filter is null || _filter(typed))
                {
                    _callback(typed);
                }
            }
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            _source.PropertyChanged -= OnPropertyChanged;
        }
    }

    private sealed class MultiPropertyWatcher : IDisposable
    {
        private readonly INotifyPropertyChanged _source;
        private readonly HashSet<string> _propertyNames;
        private readonly Action _callback;
        private bool _disposed;

        public MultiPropertyWatcher(INotifyPropertyChanged source, string[] propertyNames, Action callback)
        {
            _source = source;
            _propertyNames = new HashSet<string>(propertyNames, StringComparer.Ordinal);
            _callback = callback;
            _source.PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName is not null && _propertyNames.Contains(e.PropertyName))
            {
                _callback();
            }
            else if (e.PropertyName is null)
            {
                // PropertyChanged with null name means "all properties changed"
                _callback();
            }
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            _source.PropertyChanged -= OnPropertyChanged;
        }
    }

    private sealed class AllPropertiesWatcher : IDisposable
    {
        private readonly INotifyPropertyChanged _source;
        private readonly Action _callback;
        private bool _disposed;

        public AllPropertiesWatcher(INotifyPropertyChanged source, Action callback)
        {
            _source = source;
            _callback = callback;
            _source.PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            _callback();
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            _source.PropertyChanged -= OnPropertyChanged;
        }
    }

    private sealed class AsyncPropertyWatcher<T> : IDisposable
    {
        private readonly INotifyPropertyChanged _source;
        private readonly string _propertyName;
        private readonly Func<T, Task> _callback;
        private bool _disposed;

        public AsyncPropertyWatcher(INotifyPropertyChanged source, string propertyName,
            Func<T, Task> callback)
        {
            _source = source;
            _propertyName = propertyName;
            _callback = callback;
            _source.PropertyChanged += OnPropertyChanged;
        }

        private async void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != _propertyName && e.PropertyName is not null) return;

            var prop = _source.GetType().GetProperty(_propertyName);
            if (prop is null || !prop.CanRead) return;

            var value = prop.GetValue(_source);
            if (value is T typed)
            {
                await _callback(typed);
            }
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            _source.PropertyChanged -= OnPropertyChanged;
        }
    }

    #endregion
}
