namespace AuraUI.Core.Services;

/// <summary>
/// Simple service locator for AuraUI services. Can be replaced with DI container.
/// </summary>
public static class ServiceLocator
{
    private static readonly Dictionary<Type, object> _services = new();
    private static readonly Dictionary<Type, Func<object>> _factories = new();

    /// <summary>
    /// Register a singleton service instance.
    /// </summary>
    public static void Register<T>(T service) where T : class
    {
        _services[typeof(T)] = service;
    }

    /// <summary>
    /// Register a factory for creating service instances (transient).
    /// </summary>
    public static void RegisterFactory<T>(Func<T> factory) where T : class
    {
        _factories[typeof(T)] = () => factory();
    }

    /// <summary>
    /// Resolve a service instance.
    /// </summary>
    public static T Resolve<T>() where T : class
    {
        if (_services.TryGetValue(typeof(T), out var service))
            return (T)service;

        if (_factories.TryGetValue(typeof(T), out var factory))
            return (T)factory();

        throw new InvalidOperationException($"Service {typeof(T).Name} is not registered.");
    }

    /// <summary>
    /// Try to resolve a service instance.
    /// </summary>
    public static bool TryResolve<T>(out T? service) where T : class
    {
        if (_services.TryGetValue(typeof(T), out var s))
        {
            service = (T)s;
            return true;
        }

        if (_factories.TryGetValue(typeof(T), out var factory))
        {
            service = (T)factory();
            return true;
        }

        service = null;
        return false;
    }

    /// <summary>
    /// Check if a service is registered.
    /// </summary>
    public static bool IsRegistered<T>() where T : class
    {
        return _services.ContainsKey(typeof(T)) || _factories.ContainsKey(typeof(T));
    }

    /// <summary>
    /// Clear all registrations.
    /// </summary>
    public static void Clear()
    {
        _services.Clear();
        _factories.Clear();
    }
}
