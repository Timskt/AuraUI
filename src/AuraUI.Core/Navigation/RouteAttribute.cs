namespace AuraUI.Core.Navigation;

/// <summary>
/// Attribute used to declaratively associate a ViewModel or View class with a route path.
/// The <see cref="Router"/> can scan assemblies for classes decorated with this attribute
/// to auto-register routes.
/// </summary>
/// <example>
/// <code>
/// [Route("settings", Title = "Settings", RequiresAuth = true)]
/// public class SettingsViewModel : ViewModelBase { }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class RouteAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RouteAttribute"/> class.
    /// </summary>
    /// <param name="path">The route path (e.g., "home", "settings", "users/{id}").</param>
    public RouteAttribute(string path)
    {
        Path = path ?? throw new ArgumentNullException(nameof(path));
    }

    /// <summary>
    /// Gets the route path.
    /// </summary>
    public string Path { get; }

    /// <summary>
    /// Gets or sets an optional display title for the route.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets whether this route requires authentication.
    /// When true, the router's navigation guard will check authentication before allowing navigation.
    /// </summary>
    public bool RequiresAuth { get; set; }

    /// <summary>
    /// Gets or sets an optional sort order for route registration.
    /// Lower values are registered first.
    /// </summary>
    public int Order { get; set; }
}
