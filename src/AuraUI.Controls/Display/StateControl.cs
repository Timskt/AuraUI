using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;

namespace AuraUI.Controls.Display;

/// <summary>
/// Represents a named content item within a <see cref="StateControl"/>.
/// </summary>
public class StateItem : AvaloniaObject
{
    /// <summary>
    /// Defines the <see cref="StateName"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> StateNameProperty =
        AvaloniaProperty.Register<StateItem, string?>(nameof(StateName));

    /// <summary>
    /// Defines the <see cref="Content"/> styled property.
    /// </summary>
    public static readonly StyledProperty<object?> ContentProperty =
        AvaloniaProperty.Register<StateItem, object?>(nameof(Content));

    /// <summary>
    /// Gets or sets the name of this state (e.g. "Loading", "Error", "Empty", "Content").
    /// </summary>
    public string? StateName
    {
        get => GetValue(StateNameProperty);
        set => SetValue(StateNameProperty, value);
    }

    /// <summary>
    /// Gets or sets the content to display when this state is active.
    /// </summary>
    public object? Content
    {
        get => GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }
}

/// <summary>
/// A state-based content switcher that displays the content of the <see cref="StateItem"/>
/// whose <see cref="StateItem.StateName"/> matches the <see cref="CurrentState"/> value.
/// Useful for Loading / Error / Empty / Content state management.
/// </summary>
[PseudoClasses(":loading", ":error", ":empty", ":content")]
public class StateControl : ContentControl
{
    /// <summary>
    /// Defines the <see cref="CurrentState"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> CurrentStateProperty =
        AvaloniaProperty.Register<StateControl, string?>(nameof(CurrentState));

    /// <summary>
    /// Defines the <see cref="States"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IList<StateItem>?> StatesProperty =
        AvaloniaProperty.Register<StateControl, IList<StateItem>?>(nameof(States));

    /// <summary>
    /// Defines the <see cref="TransitionDuration"/> styled property.
    /// </summary>
    public static readonly StyledProperty<TimeSpan> TransitionDurationProperty =
        AvaloniaProperty.Register<StateControl, TimeSpan>(
            nameof(TransitionDuration),
            TimeSpan.FromMilliseconds(200));

    private readonly ObservableCollection<StateItem> _defaultStates = new();

    static StateControl()
    {
        CurrentStateProperty.Changed.AddClassHandler<StateControl>((x, e) => x.OnCurrentStateChanged(e));
        StatesProperty.Changed.AddClassHandler<StateControl>((x, _) => x.UpdateDisplayedContent());
    }

    public StateControl()
    {
        States = _defaultStates;
    }

    /// <summary>
    /// Gets or sets the current state name. The matching <see cref="StateItem"/> content will be displayed.
    /// </summary>
    public string? CurrentState
    {
        get => GetValue(CurrentStateProperty);
        set => SetValue(CurrentStateProperty, value);
    }

    /// <summary>
    /// Gets or sets the collection of state items.
    /// </summary>
    public IList<StateItem>? States
    {
        get => GetValue(StatesProperty);
        set => SetValue(StatesProperty, value);
    }

    /// <summary>
    /// Gets or sets the cross-fade duration when switching states.
    /// </summary>
    public TimeSpan TransitionDuration
    {
        get => GetValue(TransitionDurationProperty);
        set => SetValue(TransitionDurationProperty, value);
    }

    /// <summary>
    /// Adds a state item to the default states collection.
    /// </summary>
    public void AddState(string name, object? content)
    {
        _defaultStates.Add(new StateItem { StateName = name, Content = content });
        UpdateDisplayedContent();
    }

    /// <summary>
    /// Removes a state item by name.
    /// </summary>
    public bool RemoveState(string name)
    {
        var item = _defaultStates.FirstOrDefault(s =>
            string.Equals(s.StateName, name, StringComparison.OrdinalIgnoreCase));
        if (item != null)
        {
            var result = _defaultStates.Remove(item);
            UpdateDisplayedContent();
            return result;
        }
        return false;
    }

    private void OnCurrentStateChanged(AvaloniaPropertyChangedEventArgs e)
    {
        UpdatePseudoClasses();
        UpdateDisplayedContent();
    }

    private void UpdatePseudoClasses()
    {
        var state = CurrentState;
        PseudoClasses.Set(":loading", string.Equals(state, "Loading", StringComparison.OrdinalIgnoreCase));
        PseudoClasses.Set(":error", string.Equals(state, "Error", StringComparison.OrdinalIgnoreCase));
        PseudoClasses.Set(":empty", string.Equals(state, "Empty", StringComparison.OrdinalIgnoreCase));
        PseudoClasses.Set(":content", string.Equals(state, "Content", StringComparison.OrdinalIgnoreCase));
    }

    private void UpdateDisplayedContent()
    {
        var states = States;
        if (states == null || string.IsNullOrEmpty(CurrentState))
        {
            SetCurrentValue(ContentProperty, null);
            return;
        }

        foreach (var state in states)
        {
            if (string.Equals(state.StateName, CurrentState, StringComparison.OrdinalIgnoreCase))
            {
                SetCurrentValue(ContentProperty, state.Content);
                return;
            }
        }

        // No matching state found; clear content.
        SetCurrentValue(ContentProperty, null);
    }
}
