using System.Collections.Concurrent;

namespace AuraUI.Core.MVVM;

/// <summary>
/// Messenger implementation using WeakReferences to avoid memory leaks.
/// Dead references are automatically cleaned up during message delivery.
/// All operations are thread-safe.
/// </summary>
public sealed class WeakReferenceMessenger : IMessenger
{
    private readonly ConcurrentDictionary<Type, ConcurrentBag<(WeakReference Reference, Delegate Handler)>> _subscriptions = new();
    private readonly object _cleanupLock = new();

    public void Send<TMessage>(TMessage message) where TMessage : class
    {
        ArgumentNullException.ThrowIfNull(message);

        if (!_subscriptions.TryGetValue(typeof(TMessage), out var handlers))
            return;

        var dead = new List<(WeakReference, Delegate)>();

        // Snapshot to avoid issues if collection changes during iteration
        var snapshot = handlers.ToArray();

        foreach (var (reference, handler) in snapshot)
        {
            if (!reference.IsAlive)
            {
                dead.Add((reference, handler));
                continue;
            }

            try
            {
                ((Action<TMessage>)handler)(message);
            }
            catch
            {
                // Swallow handler exceptions to avoid breaking other recipients
            }
        }

        if (dead.Count > 0)
            CleanupDeadReferences(typeof(TMessage), dead);
    }

    public void Register<TMessage>(object recipient, Action<TMessage> handler) where TMessage : class
    {
        ArgumentNullException.ThrowIfNull(recipient);
        ArgumentNullException.ThrowIfNull(handler);

        var handlers = _subscriptions.GetOrAdd(typeof(TMessage), _ => new ConcurrentBag<(WeakReference, Delegate)>());
        handlers.Add((new WeakReference(recipient), handler));
    }

    public void Unregister<TMessage>(object recipient) where TMessage : class
    {
        ArgumentNullException.ThrowIfNull(recipient);

        if (!_subscriptions.TryGetValue(typeof(TMessage), out var handlers))
            return;

        lock (_cleanupLock)
        {
            var remaining = new ConcurrentBag<(WeakReference, Delegate)>();

            foreach (var entry in handlers.ToArray())
            {
                if (entry.Reference.IsAlive && entry.Reference.Target != recipient)
                    remaining.Add(entry);
            }

            _subscriptions[typeof(TMessage)] = remaining;
        }
    }

    public void UnregisterAll(object recipient)
    {
        ArgumentNullException.ThrowIfNull(recipient);

        lock (_cleanupLock)
        {
            foreach (var kvp in _subscriptions)
            {
                var remaining = new ConcurrentBag<(WeakReference, Delegate)>();

                foreach (var entry in kvp.Value.ToArray())
                {
                    if (entry.Reference.IsAlive && entry.Reference.Target != recipient)
                        remaining.Add(entry);
                }

                _subscriptions[kvp.Key] = remaining;
            }
        }
    }

    private void CleanupDeadReferences(Type messageType, List<(WeakReference, Delegate)> dead)
    {
        lock (_cleanupLock)
        {
            if (!_subscriptions.TryGetValue(messageType, out var handlers))
                return;

            var remaining = new ConcurrentBag<(WeakReference, Delegate)>();

            foreach (var entry in handlers.ToArray())
            {
                if (entry.Reference.IsAlive)
                    remaining.Add(entry);
            }

            _subscriptions[messageType] = remaining;
        }
    }
}
