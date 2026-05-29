namespace AuraUI.Core.State;

/// <summary>
/// Defines middleware that can intercept actions before and after they are processed
/// by the store's reducer. Useful for logging, validation, analytics, or async side effects.
/// </summary>
/// <typeparam name="TState">The type of state managed by the store.</typeparam>
/// <example>
/// <code>
/// public class LoggingMiddleware&lt;TState&gt; : IMiddleware&lt;TState&gt;
/// {
///     public void Before(IAction action, TState state)
///         => Console.WriteLine($"Dispatching {action.GetType().Name}");
///
///     public void After(IAction action, TState state, TState newState)
///         => Console.WriteLine($"State updated");
/// }
/// </code>
/// </example>
public interface IMiddleware<TState>
{
    /// <summary>
    /// Called before the action is passed to the reducer.
    /// </summary>
    /// <param name="action">The action being dispatched.</param>
    /// <param name="state">The current state before the action.</param>
    void Before(IAction action, TState state);

    /// <summary>
    /// Called after the reducer has produced the new state.
    /// </summary>
    /// <param name="action">The action that was dispatched.</param>
    /// <param name="state">The state before the action.</param>
    /// <param name="newState">The state after the action.</param>
    void After(IAction action, TState state, TState newState);
}

/// <summary>
/// A convenience base class for middleware that provides empty default implementations.
/// Override only the methods you need.
/// </summary>
/// <typeparam name="TState">The type of state managed by the store.</typeparam>
public abstract class MiddlewareBase<TState> : IMiddleware<TState>
{
    /// <inheritdoc/>
    public virtual void Before(IAction action, TState state) { }

    /// <inheritdoc/>
    public virtual void After(IAction action, TState state, TState newState) { }
}

/// <summary>
/// A logging middleware that outputs action types and state transitions to the console.
/// </summary>
/// <typeparam name="TState">The type of state managed by the store.</typeparam>
public class LoggingMiddleware<TState> : MiddlewareBase<TState>
{
    /// <inheritdoc/>
    public override void Before(IAction action, TState state)
    {
        Console.WriteLine($"[Store] Dispatching {action.GetType().Name}");
    }

    /// <inheritdoc/>
    public override void After(IAction action, TState state, TState newState)
    {
        Console.WriteLine($"[Store] State updated by {action.GetType().Name}");
    }
}
