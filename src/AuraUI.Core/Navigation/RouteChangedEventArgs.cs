namespace AuraUI.Core.Navigation;

/// <summary>
/// Event arguments raised when the router navigates to a new route.
/// </summary>
public sealed class RouteChangedEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RouteChangedEventArgs"/> class.
    /// </summary>
    /// <param name="route">The route that was navigated to.</param>
    /// <param name="previousRoute">The route that was navigated away from, or null.</param>
    /// <param name="viewModel">The ViewModel for the new route.</param>
    /// <param name="parameter">An optional navigation parameter.</param>
    public RouteChangedEventArgs(string route, string? previousRoute, object? viewModel, object? parameter = null)
    {
        Route = route;
        PreviousRoute = previousRoute;
        ViewModel = viewModel;
        Parameter = parameter;
    }

    /// <summary>
    /// Gets the route that was navigated to.
    /// </summary>
    public string Route { get; }

    /// <summary>
    /// Gets the route that was navigated away from, or null if this is the first navigation.
    /// </summary>
    public string? PreviousRoute { get; }

    /// <summary>
    /// Gets the ViewModel instance for the new route.
    /// </summary>
    public object? ViewModel { get; }

    /// <summary>
    /// Gets an optional navigation parameter.
    /// </summary>
    public object? Parameter { get; }
}
