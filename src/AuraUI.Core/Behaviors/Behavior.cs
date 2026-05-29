using Avalonia.Controls;

namespace AuraUI.Core.Behaviors;

/// <summary>
/// Base class for attachable behaviors.
/// A behavior encapsulates reusable UI logic that can be attached to a control.
/// </summary>
/// <typeparam name="T">The type of control this behavior can attach to.</typeparam>
public abstract class Behavior<T> where T : Control
{
    /// <summary>
    /// Gets the control this behavior is attached to.
    /// </summary>
    public T? AssociatedObject { get; private set; }

    /// <summary>
    /// Attaches this behavior to the specified control.
    /// </summary>
    /// <param name="obj">The control to attach to.</param>
    public void Attach(T obj)
    {
        if (AssociatedObject != null)
            throw new InvalidOperationException("Behavior is already attached to a control.");

        AssociatedObject = obj;
        OnAttached();
    }

    /// <summary>
    /// Detaches this behavior from the associated control.
    /// </summary>
    public void Detach()
    {
        if (AssociatedObject == null)
            return;

        OnDetaching();
        AssociatedObject = null;
    }

    /// <summary>
    /// Called when the behavior is attached to a control. Override to perform initialization.
    /// </summary>
    protected virtual void OnAttached() { }

    /// <summary>
    /// Called when the behavior is detached from a control. Override to perform cleanup.
    /// </summary>
    protected virtual void OnDetaching() { }
}
