namespace AuraUI.Core.State;

/// <summary>
/// Marker interface for actions that can be dispatched to a <see cref="Store{TState}"/>.
/// Implement this interface to define discrete state changes.
/// </summary>
/// <example>
/// <code>
/// public record IncrementAction(int Amount) : IAction;
/// public record SetNameAction(string Name) : IAction;
/// </code>
/// </example>
public interface IAction { }
