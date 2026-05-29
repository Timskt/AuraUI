namespace AuraUI.Core.MVVM;

/// <summary>
/// Base class for ViewModels that participate in navigation lifecycle.
/// Override the navigation methods to handle setup, teardown, and navigation guards.
/// </summary>
public abstract class NavigableViewModelBase : ViewModelBase
{
    /// <summary>
    /// Called when the ViewModel is navigated to. Use for initialization and loading data.
    /// </summary>
    /// <param name="parameter">Optional navigation parameter passed during navigation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public virtual Task OnNavigatedToAsync(object? parameter) => Task.CompletedTask;

    /// <summary>
    /// Called when navigating away from the ViewModel. Use for cleanup or saving state.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public virtual Task OnNavigatedFromAsync() => Task.CompletedTask;

    /// <summary>
    /// Determines whether navigation away from this ViewModel is allowed.
    /// Return false to cancel navigation (e.g., if there are unsaved changes).
    /// </summary>
    /// <returns>True if navigation is allowed; otherwise false.</returns>
    public virtual bool CanNavigateFrom() => true;
}
