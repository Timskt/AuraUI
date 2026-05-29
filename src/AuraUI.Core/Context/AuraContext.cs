using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;

namespace AuraUI.Core.Context;

/// <summary>
/// React-like Context system that provides values down the Avalonia visual tree.
/// Set a value on a parent element, and descendant elements can read it without
/// explicit prop drilling through constructors or bindings.
/// </summary>
/// <example>
/// <code>
/// // In AXAML, set a context value on a parent:
/// &lt;StackPanel Context.AuraContext.ThemeFontSize="14"
///             Context.AuraContext.ThemeAccentColor="#0078D4"&gt;
///     &lt;TextBlock Text="{Binding $parent[StackPanel].(Context:AuraContext.ThemeFontSize)}" /&gt;
/// &lt;/StackPanel&gt;
///
/// // In code-behind:
/// AuraContext.SetValue(myPanel, "ThemeFontSize", 14);
/// var fontSize = AuraContext.GetValue&lt;int&gt;(myPanel, "ThemeFontSize");
/// </code>
/// </example>
public static class AuraContext
{
    private static readonly AttachedProperty<string?> ContextKeyProperty =
        AvaloniaProperty.RegisterAttached<Control, string?>("ContextKey", typeof(AuraContext));

    // Stores a dictionary of key-value pairs per control
    private static readonly AttachedProperty<ContextStore?> StoreProperty =
        AvaloniaProperty.RegisterAttached<Control, ContextStore?>("Store", typeof(AuraContext));

    /// <summary>
    /// A generic context value attached property. Set via <see cref="SetValue"/> and read
    /// via <see cref="GetValue{T}"/>. Values propagate down the visual tree: if a key
    /// is not found on the target control, the tree is walked upward until a value is found.
    /// </summary>
    public static readonly AttachedProperty<object?> ValueProperty =
        AvaloniaProperty.RegisterAttached<Control, object?>("Value", typeof(AuraContext));

    /// <summary>
    /// Sets a named context value on the specified control.
    /// </summary>
    /// <param name="element">The control to set the value on.</param>
    /// <param name="key">A string key identifying the context value.</param>
    /// <param name="value">The value to store.</param>
    public static void SetValue(Control element, string key, object? value)
    {
        if (element is null) throw new ArgumentNullException(nameof(element));
        if (key is null) throw new ArgumentNullException(nameof(key));

        var store = element.GetValue(StoreProperty);
        if (store is null)
        {
            store = new ContextStore();
            element.SetValue(StoreProperty, store);
        }

        store.Set(key, value);
    }

    /// <summary>
    /// Gets a named context value from the control or its ancestors.
    /// Walks up the visual tree until a value for the given key is found.
    /// Returns <c>default</c> if no value is found.
    /// </summary>
    /// <typeparam name="T">The expected type of the value.</typeparam>
    /// <param name="element">The control to start searching from.</param>
    /// <param name="key">The context key to look up.</param>
    /// <returns>The value cast to <typeparamref name="T"/>, or <c>default</c>.</returns>
    public static T? GetValue<T>(Control element, string key)
    {
        if (element is null) throw new ArgumentNullException(nameof(element));
        if (key is null) throw new ArgumentNullException(nameof(key));

        var current = element;
        while (current is not null)
        {
            var store = current.GetValue(StoreProperty);
            if (store is not null && store.TryGet(key, out var raw) && raw is T typed)
            {
                return typed;
            }

            current = current.Parent as Control;
        }

        return default;
    }

    /// <summary>
    /// Gets a named context value from the control or its ancestors.
    /// Walks up the visual tree until a value for the given key is found.
    /// Returns <c>null</c> if no value is found.
    /// </summary>
    /// <param name="element">The control to start searching from.</param>
    /// <param name="key">The context key to look up.</param>
    /// <returns>The value, or <c>null</c>.</returns>
    public static object? GetValue(Control element, string key)
    {
        return GetValue<object>(element, key);
    }

    /// <summary>
    /// Checks whether a context value exists on the control or any ancestor.
    /// </summary>
    /// <param name="element">The control to start searching from.</param>
    /// <param name="key">The context key to check.</param>
    /// <returns><c>true</c> if a value for the key is found in the tree.</returns>
    public static bool HasValue(Control element, string key)
    {
        if (element is null) throw new ArgumentNullException(nameof(element));
        if (key is null) throw new ArgumentNullException(nameof(key));

        var current = element;
        while (current is not null)
        {
            var store = current.GetValue(StoreProperty);
            if (store is not null && store.ContainsKey(key))
                return true;

            current = current.Parent as Control;
        }

        return false;
    }

    /// <summary>
    /// Removes a named context value from the specified control.
    /// Does not walk the tree; only removes from the given element.
    /// </summary>
    /// <param name="element">The control to remove the value from.</param>
    /// <param name="key">The context key to remove.</param>
    /// <returns><c>true</c> if the key was found and removed.</returns>
    public static bool RemoveValue(Control element, string key)
    {
        if (element is null) throw new ArgumentNullException(nameof(element));
        if (key is null) throw new ArgumentNullException(nameof(key));

        var store = element.GetValue(StoreProperty);
        return store is not null && store.Remove(key);
    }

    /// <summary>
    /// Gets the context store for a control (creates one if needed).
    /// Useful for binding scenarios.
    /// </summary>
    public static ContextStore GetStore(Control element)
    {
        var store = element.GetValue(StoreProperty);
        if (store is null)
        {
            store = new ContextStore();
            element.SetValue(StoreProperty, store);
        }
        return store;
    }
}

/// <summary>
/// A thread-safe key-value store attached to a control for context data.
/// Implements <see cref="AvaloniaObject"/> so it can participate in the Avalonia
/// property system if needed.
/// </summary>
public class ContextStore
{
    private readonly Dictionary<string, object?> _values = new(StringComparer.OrdinalIgnoreCase);
    private readonly object _lock = new();

    /// <summary>
    /// Sets a value for the given key.
    /// </summary>
    public void Set(string key, object? value)
    {
        lock (_lock)
        {
            _values[key] = value;
        }
    }

    /// <summary>
    /// Tries to get a value for the given key.
    /// </summary>
    public bool TryGet(string key, out object? value)
    {
        lock (_lock)
        {
            return _values.TryGetValue(key, out value);
        }
    }

    /// <summary>
    /// Checks whether the store contains the given key.
    /// </summary>
    public bool ContainsKey(string key)
    {
        lock (_lock)
        {
            return _values.ContainsKey(key);
        }
    }

    /// <summary>
    /// Removes a key from the store.
    /// </summary>
    public bool Remove(string key)
    {
        lock (_lock)
        {
            return _values.Remove(key);
        }
    }

    /// <summary>
    /// Gets all keys in the store.
    /// </summary>
    public IReadOnlyCollection<string> Keys
    {
        get
        {
            lock (_lock)
            {
                return _values.Keys.ToList();
            }
        }
    }

    /// <summary>
    /// Clears all values in the store.
    /// </summary>
    public void Clear()
    {
        lock (_lock)
        {
            _values.Clear();
        }
    }
}
