namespace AuraUI.Core.State;

/// <summary>
/// Defines the contract for a state management container that holds application state,
/// dispatches actions, and notifies subscribers of state changes.
/// </summary>
/// <typeparam name="TState">The type of state managed by this container.</typeparam>
public interface IStateManager<TState>
{
    /// <summary>
    /// Gets the current state value.
    /// </summary>
    TState CurrentState { get; }

    /// <summary>
    /// Dispatches an action to the state manager, which applies it through the reducer
    /// and notifies all subscribers of the resulting state change.
    /// </summary>
    /// <param name="action">The action to dispatch.</param>
    void Dispatch(IAction action);

    /// <summary>
    /// Subscribes a listener to state changes. The listener is immediately invoked with the current state.
    /// </summary>
    /// <param name="listener">The callback invoked on each state change.</param>
    void Subscribe(Action<TState> listener);

    /// <summary>
    /// Removes a previously subscribed listener.
    /// </summary>
    /// <param name="listener">The callback to remove.</param>
    void Unsubscribe(Action<TState> listener);
}
