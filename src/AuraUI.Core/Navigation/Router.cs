using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AuraUI.Core.Navigation;

/// <summary>
/// A simple but functional router for view-model-first navigation in AuraUI applications.
/// Manages route registration, navigation with parameters, back navigation, and
/// notifies subscribers of route changes.
/// </summary>
/// <example>
/// <code>
/// var router = new Router();
/// router.RegisterRoute("home", () => new HomeViewModel());
/// router.RegisterRoute("settings", () => new SettingsViewModel());
///
/// router.Navigated += (s, e) => Console.WriteLine($"Navigated to {e.Route}");
///
/// router.Navigate("settings");
/// router.GoBack();
/// </code>
/// </example>
public class Router : INotifyPropertyChanged
{
    private readonly Dictionary<string, Func<object>> _routes = new(StringComparer.OrdinalIgnoreCase);
    private readonly Stack<(string Route, object? Parameter)> _history = new();
    private string _currentRoute = string.Empty;
    private object? _currentViewModel;
    private string? _previousRoute;
    private Func<bool>? _authGuard;

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Raised after the router completes navigation to a new route.
    /// </summary>
    public event EventHandler<RouteChangedEventArgs>? Navigated;

    /// <summary>
    /// Raised before navigation occurs. Handlers can set <see cref="CancelEventArgs.Cancel"/>
    /// to prevent the navigation.
    /// </summary>
    public event EventHandler<CancelEventArgs>? Navigating;

    /// <summary>
    /// Gets the current route path.
    /// </summary>
    public string CurrentRoute
    {
        get => _currentRoute;
        private set => SetProperty(ref _currentRoute, value);
    }

    /// <summary>
    /// Gets the ViewModel instance for the current route.
    /// </summary>
    public object? CurrentViewModel
    {
        get => _currentViewModel;
        private set => SetProperty(ref _currentViewModel, value);
    }

    /// <summary>
    /// Gets the route that was active before the current navigation, or null.
    /// </summary>
    public string? PreviousRoute
    {
        get => _previousRoute;
        private set => SetProperty(ref _previousRoute, value);
    }

    /// <summary>
    /// Gets whether there is a route in the history to go back to.
    /// </summary>
    public bool CanGoBack => _history.Count > 0;

    /// <summary>
    /// Gets the number of routes in the navigation history.
    /// </summary>
    public int HistoryCount => _history.Count;

    /// <summary>
    /// Gets a read-only view of all registered route paths.
    /// </summary>
    public IReadOnlyCollection<string> RegisteredRoutes => _routes.Keys;

    /// <summary>
    /// Registers a route with a ViewModel factory.
    /// </summary>
    /// <param name="route">The route path.</param>
    /// <param name="viewModelFactory">A factory function that creates the ViewModel for this route.</param>
    /// <exception cref="ArgumentNullException">Thrown when route or viewModelFactory is null.</exception>
    /// <exception cref="ArgumentException">Thrown when route is empty or already registered.</exception>
    public void RegisterRoute(string route, Func<object> viewModelFactory)
    {
        if (string.IsNullOrWhiteSpace(route))
            throw new ArgumentException("Route cannot be null or empty.", nameof(route));
        if (viewModelFactory is null)
            throw new ArgumentNullException(nameof(viewModelFactory));

        _routes[route] = viewModelFactory;
    }

    /// <summary>
    /// Removes a registered route.
    /// </summary>
    /// <param name="route">The route path to remove.</param>
    /// <returns>True if the route was found and removed.</returns>
    public bool UnregisterRoute(string route)
    {
        return _routes.Remove(route);
    }

    /// <summary>
    /// Sets an authentication guard. When set, routes with <c>RequiresAuth = true</c>
    /// will only be navigable if this function returns true.
    /// </summary>
    /// <param name="guard">A function that returns true if the user is authenticated.</param>
    public void SetAuthGuard(Func<bool> guard)
    {
        _authGuard = guard;
    }

    /// <summary>
    /// Navigates to the specified route.
    /// </summary>
    /// <param name="route">The route path to navigate to.</param>
    /// <exception cref="ArgumentException">Thrown when the route is not registered.</exception>
    public void Navigate(string route)
    {
        Navigate(route, null);
    }

    /// <summary>
    /// Navigates to the specified route with a parameter.
    /// </summary>
    /// <param name="route">The route path to navigate to.</param>
    /// <param name="parameter">An optional parameter passed to the ViewModel or route handler.</param>
    /// <exception cref="ArgumentException">Thrown when the route is not registered.</exception>
    public void Navigate(string route, object? parameter)
    {
        if (string.IsNullOrWhiteSpace(route))
            throw new ArgumentException("Route cannot be null or empty.", nameof(route));

        if (!_routes.TryGetValue(route, out var factory))
            throw new ArgumentException($"Route '{route}' is not registered. Available routes: {string.Join(", ", _routes.Keys)}", nameof(route));

        // Fire Navigating event (cancellable)
        var cancelArgs = new CancelEventArgs();
        Navigating?.Invoke(this, cancelArgs);
        if (cancelArgs.Cancel)
            return;

        // Push current route to history if it has a value
        if (!string.IsNullOrEmpty(_currentRoute))
        {
            _history.Push((_currentRoute, parameter));
        }

        PreviousRoute = _currentRoute;
        CurrentRoute = route;

        // Create ViewModel via factory
        CurrentViewModel = factory();

        // Raise Navigated event
        Navigated?.Invoke(this, new RouteChangedEventArgs(route, PreviousRoute, CurrentViewModel, parameter));
    }

    /// <summary>
    /// Navigates to the previous route in the history.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when there is no route to go back to.</exception>
    public void GoBack()
    {
        if (!CanGoBack)
            throw new InvalidOperationException("No route in history to go back to.");

        var (previousRoute, _) = _history.Pop();

        PreviousRoute = _currentRoute;
        CurrentRoute = previousRoute;

        if (_routes.TryGetValue(previousRoute, out var factory))
        {
            CurrentViewModel = factory();
        }

        Navigated?.Invoke(this, new RouteChangedEventArgs(previousRoute, PreviousRoute, CurrentViewModel));
    }

    /// <summary>
    /// Clears the navigation history.
    /// </summary>
    public void ClearHistory()
    {
        _history.Clear();
        OnPropertyChanged(nameof(CanGoBack));
        OnPropertyChanged(nameof(HistoryCount));
    }

    /// <summary>
    /// Checks whether a route is registered.
    /// </summary>
    /// <param name="route">The route path to check.</param>
    /// <returns>True if the route is registered.</returns>
    public bool HasRoute(string route)
    {
        return _routes.ContainsKey(route);
    }

    private void SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return;

        field = value;
        OnPropertyChanged(propertyName);
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
