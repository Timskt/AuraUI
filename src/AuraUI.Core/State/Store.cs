namespace AuraUI.Core.State;

/// <summary>
/// A generic state management store inspired by Redux patterns.
/// Holds immutable state, processes actions through a reducer function,
/// and notifies subscribers of state changes.
/// </summary>
/// <typeparam name="TState">The type of state managed by this store.</typeparam>
/// <example>
/// <code>
/// // Define state
/// public record AppState(int Count, string Name);
///
/// // Define reducer
/// static AppState Reducer(AppState state, IAction action) => action switch
/// {
///     IncrementAction a => state with { Count = state.Count + a.Amount },
///     SetNameAction a => state with { Name = a.Name },
///     _ => state
/// };
///
/// // Create store
/// var store = new Store&lt;AppState&gt;(new AppState(0, ""), Reducer);
///
/// // Subscribe
/// store.Subscribe(state => Console.WriteLine($"Count: {state.Count}"));
///
/// // Dispatch
/// store.Dispatch(new IncrementAction(5));
/// </code>
/// </example>
public class Store<TState> : IStateManager<TState>
{
    private readonly Func<TState, IAction, TState> _reducer;
    private readonly List<IMiddleware<TState>> _middleware;
    private TState _state;
    private readonly List<Action<TState>> _listeners = new();
    private bool _isDispatching;

    /// <summary>
    /// Creates a new store with the given initial state and reducer function.
    /// </summary>
    /// <param name="initialState">The initial state value.</param>
    /// <param name="reducer">A pure function that takes the current state and an action, and returns the new state.</param>
    /// <param name="middleware">Optional middleware to intercept actions before and after reduction.</param>
    public Store(TState initialState, Func<TState, IAction, TState> reducer, IEnumerable<IMiddleware<TState>>? middleware = null)
    {
        _state = initialState ?? throw new ArgumentNullException(nameof(initialState));
        _reducer = reducer ?? throw new ArgumentNullException(nameof(reducer));
        _middleware = middleware?.ToList() ?? new List<IMiddleware<TState>>();
    }

    /// <inheritdoc/>
    public TState CurrentState => _state;

    /// <summary>
    /// Gets whether the store is currently in the process of dispatching an action.
    /// </summary>
    public bool IsDispatching => _isDispatching;

    /// <inheritdoc/>
    public void Dispatch(IAction action)
    {
        if (action is null) throw new ArgumentNullException(nameof(action));

        if (_isDispatching)
            throw new InvalidOperationException("Cannot dispatch an action while another action is being processed.");

        _isDispatching = true;
        try
        {
            // Run before middleware
            foreach (var mw in _middleware)
            {
                mw.Before(action, _state);
            }

            var previousState = _state;
            _state = _reducer(_state, action);

            // Run after middleware
            foreach (var mw in _middleware)
            {
                mw.After(action, previousState, _state);
            }

            // Notify listeners
            foreach (var listener in _listeners.ToArray())
            {
                listener(_state);
            }
        }
        finally
        {
            _isDispatching = false;
        }
    }

    /// <inheritdoc/>
    public void Subscribe(Action<TState> listener)
    {
        if (listener is null) throw new ArgumentNullException(nameof(listener));

        if (!_listeners.Contains(listener))
        {
            _listeners.Add(listener);
            // Immediately invoke with current state
            listener(_state);
        }
    }

    /// <inheritdoc/>
    public void Unsubscribe(Action<TState> listener)
    {
        if (listener is null) throw new ArgumentNullException(nameof(listener));
        _listeners.Remove(listener);
    }

    /// <summary>
    /// Adds middleware to the store. Middleware can intercept actions before and after
    /// they are processed by the reducer.
    /// </summary>
    /// <param name="middleware">The middleware to add.</param>
    public void Use(IMiddleware<TState> middleware)
    {
        if (middleware is null) throw new ArgumentNullException(nameof(middleware));
        _middleware.Add(middleware);
    }

    /// <summary>
    /// Gets the current number of subscribers.
    /// </summary>
    public int SubscriberCount => _listeners.Count;
}
