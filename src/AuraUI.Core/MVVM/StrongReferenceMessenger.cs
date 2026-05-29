using System.Collections.Concurrent;

namespace AuraUI.Core.MVVM;

/// <summary>
/// Messenger implementation using strong references for maximum performance.
/// Recipients must be manually unregistered to avoid memory leaks.
/// All operations are thread-safe.
/// </summary>
public sealed class StrongReferenceMessenger : IMessenger
{
    private readonly ConcurrentDictionary<Type, ConcurrentDictionary<object, Delegate>> _subscriptions = new();

    public void Send<TMessage>(TMessage message) where TMessage : class
    {
        ArgumentNullException.ThrowIfNull(message);

        if (!_subscriptions.TryGetValue(typeof(TMessage), out var handlers))
            return;

        foreach (var kvp in handlers.ToArray())
        {
            try
            {
                ((Action<TMessage>)kvp.Value)(message);
            }
            catch
            {
                // Swallow handler exceptions to avoid breaking other recipients
            }
        }
    }

    public void Register<TMessage>(object recipient, Action<TMessage> handler) where TMessage : class
    {
        ArgumentNullException.ThrowIfNull(recipient);
        ArgumentNullException.ThrowIfNull(handler);

        var handlers = _subscriptions.GetOrAdd(typeof(TMessage), _ => new ConcurrentDictionary<object, Delegate>());
        handlers[recipient] = handler;
    }

    public void Unregister<TMessage>(object recipient) where TMessage : class
    {
        ArgumentNullException.ThrowIfNull(recipient);

        if (_subscriptions.TryGetValue(typeof(TMessage), out var handlers))
            handlers.TryRemove(recipient, out _);
    }

    public void UnregisterAll(object recipient)
    {
        ArgumentNullException.ThrowIfNull(recipient);

        foreach (var kvp in _subscriptions)
            kvp.Value.TryRemove(recipient, out _);
    }
}
