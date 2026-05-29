namespace AuraUI.Core.MVVM;

/// <summary>
/// Provides a default singleton instance of <see cref="IMessenger"/>.
/// </summary>
public static class Messenger
{
    /// <summary>
    /// Gets the default <see cref="IMessenger"/> instance using weak references.
    /// </summary>
    public static IMessenger Default { get; } = new WeakReferenceMessenger();
}
