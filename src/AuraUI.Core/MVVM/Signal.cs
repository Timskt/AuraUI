using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AuraUI.Core.MVVM;

/// <summary>
/// Solid.js-like Signal for fine-grained reactivity.
/// A Signal holds a value and allows subscribing to changes via effects,
/// as well as deriving new signals that automatically track their dependencies.
/// </summary>
/// <typeparam name="T">The type of the signal value.</typeparam>
/// <example>
/// <code>
/// // Create a signal:
/// var count = new Signal&lt;int&gt;(0);
///
/// // Create an effect that runs whenever count changes:
/// var effect = count.Effect(value => Console.WriteLine($"Count: {value}"));
///
/// // Update the signal:
/// count.Value = 5; // Effect runs: "Count: 5"
///
/// // Derive a new signal:
/// var doubled = count.Derive(v => v * 2);
/// Console.WriteLine(doubled.Value); // 10
/// count.Value = 10;
/// Console.WriteLine(doubled.Value); // 20
///
/// // Cleanup:
/// effect.Dispose();
/// </code>
/// </example>
public class Signal<T> : INotifyPropertyChanged, IDisposable
{
    private T _value;
    private readonly List<WeakReference<SignalEffect<T>>> _effects = new();
    private readonly List<WeakReference<IDisposable>> _derivedChildren = new();
    private readonly object _lock = new();
    private bool _disposed;

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Raised with the new value whenever it changes.
    /// </summary>
    public event Action<T>? Changed;

    /// <summary>
    /// Gets or sets the current value. Setting this property triggers all registered
    /// effects and notifies derived signals.
    /// </summary>
    public T Value
    {
        get
        {
            // Track access for automatic dependency discovery
            SignalTracker.RecordAccess(this);
            return _value;
        }
        set
        {
            if (EqualityComparer<T>.Default.Equals(_value, value))
                return;

            _value = value;
            NotifyEffects();
            OnPropertyChanged();
            Changed?.Invoke(_value);
        }
    }

    /// <summary>
    /// Creates a new signal with the given initial value.
    /// </summary>
    public Signal(T initialValue = default!)
    {
        _value = initialValue;
    }

    /// <summary>
    /// Registers an effect that runs immediately and whenever the signal value changes.
    /// The effect is automatically cleaned up when the returned disposable is disposed.
    /// </summary>
    /// <param name="effect">The effect function to run with the current/new value.</param>
    /// <returns>An <see cref="IDisposable"/> that removes the effect when disposed.</returns>
    public IDisposable Effect(Action<T> effect)
    {
        ArgumentNullException.ThrowIfNull(effect);

        var signalEffect = new SignalEffect<T>(effect, this);

        lock (_lock)
        {
            CleanupDeadReferences();
            _effects.Add(new WeakReference<SignalEffect<T>>(signalEffect));
        }

        // Run immediately with current value
        effect(_value);

        return signalEffect;
    }

    /// <summary>
    /// Derives a new signal whose value is computed from this signal's value.
    /// The derived signal automatically updates when this signal changes.
    /// </summary>
    /// <typeparam name="TResult">The type of the derived signal.</typeparam>
    /// <param name="selector">A function that computes the derived value.</param>
    /// <returns>A new <see cref="Signal{TResult}"/> that updates automatically.</returns>
    public Signal<TResult> Derive<TResult>(Func<T, TResult> selector)
    {
        ArgumentNullException.ThrowIfNull(selector);

        var derived = new Signal<TResult>(selector(_value));

        var effect = Effect(newValue =>
        {
            derived.Value = selector(newValue);
        });

        lock (_lock)
        {
            _derivedChildren.Add(new WeakReference<IDisposable>(effect));
        }

        return derived;
    }

    /// <summary>
    /// Creates a new signal that is the result of combining this signal with another.
    /// </summary>
    /// <typeparam name="T2">The type of the other signal.</typeparam>
    /// <typeparam name="TResult">The type of the combined signal.</typeparam>
    /// <param name="other">The other signal to combine with.</param>
    /// <param name="selector">A function that combines the two values.</param>
    /// <returns>A new <see cref="Signal{TResult}"/>.</returns>
    public Signal<TResult> Combine<T2, TResult>(Signal<T2> other, Func<T, T2, TResult> selector)
    {
        ArgumentNullException.ThrowIfNull(other);
        ArgumentNullException.ThrowIfNull(selector);

        var combined = new Signal<TResult>(selector(_value, other._value));

        var effect1 = Effect(v => combined.Value = selector(v, other.Value));
        var effect2 = other.Effect(v2 => combined.Value = selector(_value, v2));

        lock (_lock)
        {
            _derivedChildren.Add(new WeakReference<IDisposable>(effect1));
            _derivedChildren.Add(new WeakReference<IDisposable>(effect2));
        }

        return combined;
    }

    /// <summary>
    /// Batch multiple updates. Effects are only notified once after the batch completes.
    /// </summary>
    /// <param name="action">The action containing multiple updates.</param>
    public void Batch(Action action)
    {
        ArgumentNullException.ThrowIfNull(action);

        // Simple batch: store the value, run action, then notify once
        var previousValue = _value;
        action();

        if (!EqualityComparer<T>.Default.Equals(previousValue, _value))
        {
            NotifyEffects();
            OnPropertyChanged();
            Changed?.Invoke(_value);
        }
    }

    private void NotifyEffects()
    {
        lock (_lock)
        {
            CleanupDeadReferences();
            foreach (var weakRef in _effects)
            {
                if (weakRef.TryGetTarget(out var effect))
                {
                    effect.Invoke(_value);
                }
            }
        }
    }

    private void CleanupDeadReferences()
    {
        _effects.RemoveAll(w => !w.TryGetTarget(out _));
        _derivedChildren.RemoveAll(w => !w.TryGetTarget(out _));
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;

        lock (_lock)
        {
            foreach (var weakRef in _derivedChildren)
            {
                if (weakRef.TryGetTarget(out var child))
                {
                    child.Dispose();
                }
            }
            _derivedChildren.Clear();
            _effects.Clear();
        }

        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Implicit conversion from <see cref="Signal{T}"/> to <typeparamref name="T"/>.
    /// </summary>
    public static implicit operator T(Signal<T> signal) => signal._value;
}

/// <summary>
/// Represents an effect attached to a signal.
/// </summary>
internal class SignalEffect<T> : IDisposable
{
    private readonly Action<T> _effect;
    private readonly Signal<T> _signal;
    private bool _disposed;

    public SignalEffect(Action<T> effect, Signal<T> signal)
    {
        _effect = effect;
        _signal = signal;
    }

    public void Invoke(T value)
    {
        if (!_disposed)
        {
            _effect(value);
        }
    }

    public void Dispose()
    {
        _disposed = true;
    }
}

/// <summary>
/// Static tracker for automatic dependency discovery in signals.
/// When a signal's <c>Value</c> getter is called inside a tracking scope,
/// the signal is recorded as a dependency.
/// </summary>
public static class SignalTracker
{
    [ThreadStatic]
    private static Stack<List<object>>? _trackingScopes;

    /// <summary>
    /// Begins a tracking scope. Any signal accessed within this scope
    /// will be recorded as a dependency.
    /// </summary>
    /// <returns>A disposable that ends the tracking scope and returns the tracked dependencies.</returns>
    public static TrackingScope BeginScope()
    {
        _trackingScopes ??= new Stack<List<object>>();
        _trackingScopes.Push(new List<object>());
        return new TrackingScope();
    }

    internal static void RecordAccess(object signal)
    {
        if (_trackingScopes is { Count: > 0 })
        {
            _trackingScopes.Peek().Add(signal);
        }
    }

    /// <summary>
    /// Gets the dependencies recorded in the current tracking scope.
    /// </summary>
    public static IReadOnlyList<object> GetDependencies()
    {
        if (_trackingScopes is { Count: > 0 })
        {
            return _trackingScopes.Peek().AsReadOnly();
        }
        return Array.Empty<object>();
    }

    /// <summary>
    /// Ends the current tracking scope and returns the recorded dependencies.
    /// </summary>
    public class TrackingScope : IDisposable
    {
        private bool _disposed;

        /// <summary>
        /// Gets the dependencies recorded during this scope.
        /// </summary>
        public IReadOnlyList<object> Dependencies { get; private set; } = Array.Empty<object>();

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            if (_trackingScopes is { Count: > 0 })
            {
                Dependencies = _trackingScopes.Pop().AsReadOnly();
            }
        }
    }
}
