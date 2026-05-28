namespace AuraUI.Core.Services;

/// <summary>
/// Service for page navigation. Can be replaced with custom implementations.
/// </summary>
public interface INavigationService
{
    bool CanGoBack { get; }
    bool CanGoForward { get; }

    Task NavigateAsync<TViewModel>() where TViewModel : class;
    Task NavigateAsync(string pageKey);
    Task NavigateAsync(string pageKey, object? parameter);
    Task GoBackAsync();
    Task GoForwardAsync();

    event EventHandler<NavigationEventArgs>? Navigated;
}

public class NavigationEventArgs : EventArgs
{
    public string PageKey { get; init; } = string.Empty;
    public object? Parameter { get; init; }
    public object? ViewModel { get; init; }
}

/// <summary>
/// Registry for mapping page keys to ViewModel types.
/// </summary>
public interface IPageRegistry
{
    void Register<TViewModel>(string key) where TViewModel : class;
    void Register<TViewModel>(string key, Func<TViewModel> factory) where TViewModel : class;
    Type? GetViewModelType(string key);
    object? CreateViewModel(string key);
}
