using System.Globalization;
using System.Reflection;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Threading;

namespace AuraUI.Core.Behaviors;

/// <summary>
/// Behavior that binds a control event to an ICommand.
/// Allows invoking a command when any named event fires on the associated control.
/// </summary>
/// <example>
/// Usage in AXAML:
/// <code>
/// &lt;Button&gt;
///     &lt;Interaction.Behaviors&gt;
///         &lt;behaviors:EventToCommandBehavior EventName="Click"
///                                           Command="{Binding MyCommand}"
///                                           CommandParameter="Hello" /&gt;
///     &lt;/Interaction.Behaviors&gt;
/// &lt;/Button&gt;
/// </code>
/// </example>
public class EventToCommandBehavior : Behavior<Control>
{
    private Delegate? _eventHandler;
    private EventInfo? _eventInfo;

    /// <summary>
    /// The name of the event to subscribe to (e.g. "Click", "TextChanged", "SelectionChanged").
    /// </summary>
    public string? EventName { get; set; }

    /// <summary>
    /// The command to execute when the event fires.
    /// </summary>
    public ICommand? Command { get; set; }

    /// <summary>
    /// An optional parameter to pass to the command.
    /// </summary>
    public object? CommandParameter { get; set; }

    /// <summary>
    /// An optional value converter that converts the EventArgs into a command parameter.
    /// When set, the converted value is used instead of CommandParameter.
    /// </summary>
    public IValueConverter? EventArgsConverter { get; set; }

    /// <summary>
    /// An optional parameter to pass to the EventArgsConverter.
    /// </summary>
    public object? EventArgsConverterParameter { get; set; }

    /// <summary>
    /// When true and no CommandParameter or Converter is set, passes the raw EventArgs to the command.
    /// </summary>
    public bool PassEventArgsToCommand { get; set; }

    protected override void OnAttached()
    {
        base.OnAttached();
        SubscribeToEvent();
    }

    protected override void OnDetaching()
    {
        UnsubscribeFromEvent();
        base.OnDetaching();
    }

    private void SubscribeToEvent()
    {
        if (AssociatedObject == null || string.IsNullOrEmpty(EventName))
            return;

        _eventInfo = AssociatedObject.GetType().GetEvent(EventName,
            BindingFlags.Public | BindingFlags.Instance);

        if (_eventInfo == null)
            throw new InvalidOperationException(
                $"Event '{EventName}' not found on type '{AssociatedObject.GetType().Name}'.");

        // Build a delegate matching the event's handler signature
        var handlerType = _eventInfo.EventHandlerType;
        if (handlerType == null)
            return;

        var invokeMethod = handlerType.GetMethod("Invoke");
        if (invokeMethod == null)
            return;

        var parameters = invokeMethod.GetParameters();

        var delegateParams = new Type[parameters.Length];
        for (int i = 0; i < parameters.Length; i++)
            delegateParams[i] = parameters[i].ParameterType;

        // For standard EventHandler<TEventArgs> and EventHandler patterns
        _eventInfo.AddEventHandler(AssociatedObject,
            handlerType == typeof(EventHandler)
                ? new EventHandler(OnEventFired)
                : CreateCompatibleDelegate(handlerType));
    }

    private Delegate CreateCompatibleDelegate(Type handlerType)
    {
        // For EventHandler<TEventArgs> pattern
        var invokeMethod = handlerType.GetMethod("Invoke");
        if (invokeMethod == null)
            throw new InvalidOperationException("Cannot resolve delegate Invoke method.");

        var parameters = invokeMethod.GetParameters();
        if (parameters.Length == 2)
        {
            // Create a delegate that accepts (sender, args) and calls our handler
            return Delegate.CreateDelegate(handlerType, this,
                typeof(EventToCommandBehavior).GetMethod(nameof(OnGenericEventFired),
                    BindingFlags.NonPublic | BindingFlags.Instance)
                ?? throw new InvalidOperationException("Internal handler not found."));
        }

        throw new NotSupportedException(
            $"Event handler with {parameters.Length} parameters is not supported.");
    }

    /// <summary>
    /// Generic event handler for EventHandler&lt;T&gt; patterns.
    /// </summary>
    private void OnGenericEventFired(object? sender, object? args)
    {
        ExecuteCommand(args);
    }

    private void OnEventFired(object? sender, EventArgs e)
    {
        ExecuteCommand(e);
    }

    private void ExecuteCommand(object? eventArgs)
    {
        var command = Command;
        if (command == null)
            return;

        object? parameter = ResolveParameter(eventArgs);

        if (command.CanExecute(parameter))
        {
            if (Dispatcher.UIThread.CheckAccess())
            {
                command.Execute(parameter);
            }
            else
            {
                Dispatcher.UIThread.Post(() =>
                {
                    if (command.CanExecute(parameter))
                        command.Execute(parameter);
                });
            }
        }
    }

    private object? ResolveParameter(object? eventArgs)
    {
        // Priority: Converter > CommandParameter > PassEventArgs > null
        var converter = EventArgsConverter;
        if (converter != null && eventArgs != null)
        {
            return converter.Convert(eventArgs, typeof(object),
                EventArgsConverterParameter, CultureInfo.CurrentCulture);
        }

        var commandParam = CommandParameter;
        if (commandParam != null)
            return commandParam;

        if (PassEventArgsToCommand)
            return eventArgs;

        return null;
    }

    private void UnsubscribeFromEvent()
    {
        if (_eventInfo == null || AssociatedObject == null)
            return;

        _eventInfo.RemoveEventHandler(AssociatedObject, _eventHandler);
        _eventInfo = null;
        _eventHandler = null;
    }
}
