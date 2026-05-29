using Avalonia;
using Avalonia.Controls;

namespace AuraUI.Core.State;

/// <summary>
/// Extension methods for binding Avalonia controls to a <see cref="Store{TState}"/>.
/// Provides a declarative way to keep UI in sync with state changes.
/// </summary>
/// <example>
/// <code>
/// var store = new Store&lt;AppState&gt;(initialState, reducer);
/// myTextBlock.BindState(store, s => s.UserName, TextBlock.TextProperty);
/// myButton.BindState(store, s => s.CanSubmit, Button.IsEnabledProperty);
/// </code>
/// </example>
public static class StateBinding
{
    /// <summary>
    /// Binds a control's Avalonia property to a selector on the store's state.
    /// The property is automatically updated whenever the selected value changes.
    /// </summary>
    /// <typeparam name="TControl">The control type.</typeparam>
    /// <typeparam name="TState">The state type.</typeparam>
    /// <typeparam name="TValue">The selected value type.</typeparam>
    /// <param name="control">The control to bind.</param>
    /// <param name="store">The state store to observe.</param>
    /// <param name="selector">A function that extracts the relevant value from the state.</param>
    /// <param name="property">The Avalonia property to update.</param>
    /// <returns>The control for fluent chaining.</returns>
    public static TControl BindState<TControl, TState, TValue>(
        this TControl control,
        Store<TState> store,
        Func<TState, TValue> selector,
        AvaloniaProperty property)
        where TControl : Control
    {
        if (control is null) throw new ArgumentNullException(nameof(control));
        if (store is null) throw new ArgumentNullException(nameof(store));
        if (selector is null) throw new ArgumentNullException(nameof(selector));
        if (property is null) throw new ArgumentNullException(nameof(property));

        // Track the last value to avoid unnecessary updates
        var lastValue = selector(store.CurrentState);
        control.SetValue(property, lastValue);

        store.Subscribe(state =>
        {
            var newValue = selector(state);
            if (!EqualityComparer<TValue>.Default.Equals(lastValue, newValue))
            {
                lastValue = newValue;
                control.SetValue(property, newValue);
            }
        });

        return control;
    }

    /// <summary>
    /// Binds a control's Avalonia property to a selector, using a custom converter
    /// to transform the state value before applying it.
    /// </summary>
    /// <typeparam name="TControl">The control type.</typeparam>
    /// <typeparam name="TState">The state type.</typeparam>
    /// <typeparam name="TValue">The value type extracted by the selector.</typeparam>
    /// <typeparam name="TTarget">The target property type.</typeparam>
    /// <param name="control">The control to bind.</param>
    /// <param name="store">The state store to observe.</param>
    /// <param name="selector">A function that extracts the relevant value from the state.</param>
    /// <param name="converter">A function to convert the selected value to the target property type.</param>
    /// <param name="property">The Avalonia property to update.</param>
    /// <returns>The control for fluent chaining.</returns>
    public static TControl BindState<TControl, TState, TValue, TTarget>(
        this TControl control,
        Store<TState> store,
        Func<TState, TValue> selector,
        Func<TValue, TTarget> converter,
        AvaloniaProperty property)
        where TControl : Control
    {
        if (control is null) throw new ArgumentNullException(nameof(control));
        if (store is null) throw new ArgumentNullException(nameof(store));
        if (selector is null) throw new ArgumentNullException(nameof(selector));
        if (converter is null) throw new ArgumentNullException(nameof(converter));
        if (property is null) throw new ArgumentNullException(nameof(property));

        var lastValue = selector(store.CurrentState);
        control.SetValue(property, converter(lastValue));

        store.Subscribe(state =>
        {
            var newValue = selector(state);
            if (!EqualityComparer<TValue>.Default.Equals(lastValue, newValue))
            {
                lastValue = newValue;
                control.SetValue(property, converter(newValue));
            }
        });

        return control;
    }

    /// <summary>
    /// Binds a control's visibility to a predicate on the store's state.
    /// The control becomes visible when the predicate returns true.
    /// </summary>
    /// <typeparam name="TControl">The control type.</typeparam>
    /// <typeparam name="TState">The state type.</typeparam>
    /// <param name="control">The control to bind.</param>
    /// <param name="store">The state store to observe.</param>
    /// <param name="predicate">A function that determines visibility from the state.</param>
    /// <returns>The control for fluent chaining.</returns>
    public static TControl BindVisibility<TControl, TState>(
        this TControl control,
        Store<TState> store,
        Func<TState, bool> predicate)
        where TControl : Control
    {
        return BindState(control, store, predicate, Visual.IsVisibleProperty);
    }

    /// <summary>
    /// Creates a one-way binding from store state to multiple properties on a control.
    /// </summary>
    /// <typeparam name="TControl">The control type.</typeparam>
    /// <typeparam name="TState">The state type.</typeparam>
    /// <param name="control">The control to bind.</param>
    /// <param name="store">The state store to observe.</param>
    /// <param name="bindings">Pairs of (selector, property) to bind.</param>
    /// <returns>The control for fluent chaining.</returns>
    public static TControl BindMultiple<TControl, TState>(
        this TControl control,
        Store<TState> store,
        params (Func<TState, object?> Selector, AvaloniaProperty Property)[] bindings)
        where TControl : Control
    {
        foreach (var (selector, property) in bindings)
        {
            var lastValue = selector(store.CurrentState);
            control.SetValue(property, lastValue);

            store.Subscribe(state =>
            {
                var newValue = selector(state);
                if (!Equals(lastValue, newValue))
                {
                    lastValue = newValue;
                    control.SetValue(property, newValue);
                }
            });
        }

        return control;
    }
}
