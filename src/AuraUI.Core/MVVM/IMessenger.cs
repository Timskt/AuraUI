namespace AuraUI.Core.MVVM;

/// <summary>
/// Defines a pub/sub messenger for loosely-coupled communication between components.
/// </summary>
public interface IMessenger
{
    /// <summary>
    /// Sends a message to all registered recipients.
    /// </summary>
    void Send<TMessage>(TMessage message) where TMessage : class;

    /// <summary>
    /// Registers a recipient for a specific message type.
    /// </summary>
    void Register<TMessage>(object recipient, Action<TMessage> handler) where TMessage : class;

    /// <summary>
    /// Unregisters a recipient from a specific message type.
    /// </summary>
    void Unregister<TMessage>(object recipient) where TMessage : class;

    /// <summary>
    /// Unregisters a recipient from all message types.
    /// </summary>
    void UnregisterAll(object recipient);
}
