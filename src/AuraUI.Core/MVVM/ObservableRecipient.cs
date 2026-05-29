namespace AuraUI.Core.MVVM;

/// <summary>
/// Base class for ViewModels that participate in the messaging system.
/// Provides messenger access and activate/deactivate lifecycle for automatic registration cleanup.
/// </summary>
public abstract class ObservableRecipient : ViewModelBase
{
    /// <summary>
    /// Gets the messenger instance used by this recipient.
    /// </summary>
    protected IMessenger Messenger { get; }

    /// <summary>
    /// Gets whether this recipient is currently active and receiving messages.
    /// </summary>
    protected bool IsActive { get; private set; }

    /// <summary>
    /// Initializes a new instance of <see cref="ObservableRecipient"/> using the default messenger.
    /// </summary>
    protected ObservableRecipient() : this(global::AuraUI.Core.MVVM.Messenger.Default)
    {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="ObservableRecipient"/> with a specific messenger.
    /// </summary>
    /// <param name="messenger">The messenger instance to use.</param>
    protected ObservableRecipient(IMessenger messenger)
    {
        Messenger = messenger ?? throw new ArgumentNullException(nameof(messenger));
    }

    /// <summary>
    /// Activates this recipient, allowing it to receive messages.
    /// </summary>
    public void Activate()
    {
        if (IsActive) return;

        IsActive = true;
        OnActivated();
    }

    /// <summary>
    /// Deactivates this recipient and unregisters it from all messages.
    /// </summary>
    public void Deactivate()
    {
        if (!IsActive) return;

        IsActive = false;
        OnDeactivated();
        Messenger.UnregisterAll(this);
    }

    /// <summary>
    /// Called when the recipient is activated. Override to register message handlers.
    /// </summary>
    protected virtual void OnActivated()
    {
    }

    /// <summary>
    /// Called when the recipient is deactivated. Override for additional cleanup.
    /// </summary>
    protected virtual void OnDeactivated()
    {
    }

    /// <summary>
    /// Registers a message handler for this recipient.
    /// </summary>
    /// <typeparam name="TMessage">The type of message to receive.</typeparam>
    /// <param name="handler">The handler to invoke when the message is received.</param>
    protected void Receive<TMessage>(Action<TMessage> handler) where TMessage : class
        => Messenger.Register(this, handler);
}
