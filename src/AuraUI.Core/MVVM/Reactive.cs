using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AuraUI.Core.MVVM;

/// <summary>
/// Svelte-like reactive wrapper that holds a value and notifies listeners when it changes.
/// Unlike a plain field, <see cref="Reactive{T}"/> integrates with
/// <see cref="INotifyPropertyChanged"/> and supports derived/computed chains.
/// </summary>
/// <typeparam name="T">The type of the reactive value.</typeparam>
/// <example>
/// <code>
/// // Create a reactive value:
/// var count = new Reactive&lt;int&gt;(0);
/// count.Changed += newValue => Console.WriteLine($"Count: {newValue}");
/// count.Value = 5; // Prints "Count: 5"
///
/// // Derive a computed reactive:
/// var doubled = count.Derive(c => c * 2);
/// count.Value = 10; // doubled.Value becomes 20
///
/// // Use in a ViewModel:
/// public Reactive&lt;string&gt; Name { get; } = new("World");
/// </code>
/// </example>
public class Reactive<T> : INotifyPropertyChanged, IDisposable
{
    private T _value;
    private bool _disposed;
    private readonly List<IDisposable> _derivedSubscriptions = new();

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Raised with the new value whenever it changes.
    /// </summary>
    public event Action<T>? Changed;

    /// <summary>
    /// Gets or sets the current value. Setting this property triggers
    /// <see cref="PropertyChanged"/> and <see cref="Changed"/> events.
    /// </summary>
    public T Value
    {
        get => _value;
        set
        {
            if (EqualityComparer<T>.Default.Equals(_value, value))
                return;

            _value = value;
            OnPropertyChanged();
            Changed?.Invoke(_value);
        }
    }

    /// <summary>
    /// Creates a new reactive value with the given initial value.
    /// </summary>
    public Reactive(T initialValue = default!)
    {
        _value = initialValue;
    }

    /// <summary>
    /// Creates a derived reactive value that automatically updates when this value changes.
    /// </summary>
    /// <typeparam name="TResult">The type of the derived value.</typeparam>
    /// <param name="selector">A function that computes the derived value from this reactive's value.</param>
    /// <returns>A new <see cref="Reactive{TResult}"/> that updates when this reactive changes.</returns>
    public Reactive<TResult> Derive<TResult>(Func<T, TResult> selector)
    {
        ArgumentNullException.ThrowIfNull(selector);

        var derived = new Reactive<TResult>(selector(_value));

        var subscription = Watch.Property<T>(this, nameof(Value), newValue =>
        {
            derived.Value = selector(newValue);
        });

        _derivedSubscriptions.Add(subscription);
        return derived;
    }

    /// <summary>
    /// Creates a derived reactive value that depends on multiple reactives.
    /// </summary>
    /// <typeparam name="T2">The type of the second reactive.</typeparam>
    /// <typeparam name="TResult">The type of the derived value.</typeparam>
    /// <param name="other">The second reactive dependency.</param>
    /// <param name="selector">A function that computes the derived value.</param>
    /// <returns>A new <see cref="Reactive{TResult}"/>.</returns>
    public Reactive<TResult> Derive<T2, TResult>(Reactive<T2> other, Func<T, T2, TResult> selector)
    {
        ArgumentNullException.ThrowIfNull(other);
        ArgumentNullException.ThrowIfNull(selector);

        var derived = new Reactive<TResult>(selector(_value, other._value));

        var sub1 = Watch.Property<T>(this, nameof(Value), _ =>
        {
            derived.Value = selector(_value, other._value);
        });

        var sub2 = Watch.Property<T2>(other, nameof(Value), _ =>
        {
            derived.Value = selector(_value, other._value);
        });

        _derivedSubscriptions.Add(sub1);
        _derivedSubscriptions.Add(sub2);
        return derived;
    }

    /// <summary>
    /// Updates the value only if the predicate returns true for the new value.
    /// Useful for validation before setting.
    /// </summary>
    /// <param name="newValue">The candidate new value.</param>
    /// <param name="predicate">A predicate that must return true for the update to occur.</param>
    /// <returns><c>true</c> if the value was updated.</returns>
    public bool TrySet(T newValue, Func<T, bool> predicate)
    {
        if (predicate(newValue))
        {
            Value = newValue;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Updates the value using an updater function that receives the current value.
    /// </summary>
    /// <param name="updater">A function that transforms the current value into the new value.</param>
    public void Update(Func<T, T> updater)
    {
        Value = updater(_value);
    }

    /// <inheritdoc/>
    public override string ToString() => _value?.ToString() ?? string.Empty;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;

        foreach (var subscription in _derivedSubscriptions)
        {
            subscription.Dispose();
        }
        _derivedSubscriptions.Clear();

        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Implicit conversion from <see cref="Reactive{T}"/> to <typeparamref name="T"/>.
    /// </summary>
    public static implicit operator T(Reactive<T> reactive) => reactive._value;

    /// <summary>
    /// Implicit conversion from <typeparamref name="T"/> to <see cref="Reactive{T}"/>.
    /// </summary>
    public static implicit operator Reactive<T>(T value) => new(value);
}
