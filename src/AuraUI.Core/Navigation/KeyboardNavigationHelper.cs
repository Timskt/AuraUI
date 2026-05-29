using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Interactivity;

namespace AuraUI.Core.Navigation;

/// <summary>
/// Provides attached properties for declarative keyboard navigation behavior on controls.
/// Supports arrow key navigation within containers, Enter-to-activate, Escape-to-dismiss,
/// and configurable tab navigation modes.
/// </summary>
/// <example>
/// <code>
/// &lt;StackPanel navigation:KeyboardNavigationHelper.ArrowKeyNavigation="True"&gt;
///     &lt;Button Content="First"/&gt;
///     &lt;Button Content="Second"/&gt;
///     &lt;Button Content="Third"/&gt;
/// &lt;/StackPanel&gt;
///
/// &lt;Button Content="OK"
///     navigation:KeyboardNavigationHelper.EnterKeyActivates="True"/&gt;
/// </code>
/// </example>
public static class KeyboardNavigationHelper
{
    #region ArrowKeyNavigation

    /// <summary>
    /// Defines the ArrowKeyNavigation attached property.
    /// When true, arrow keys move focus between child controls within the container.
    /// </summary>
    public static readonly AttachedProperty<bool> ArrowKeyNavigationProperty =
        AvaloniaProperty.RegisterAttached<Control, bool>("ArrowKeyNavigation", typeof(KeyboardNavigationHelper));

    /// <summary>
    /// Gets whether arrow key navigation is enabled.
    /// </summary>
    public static bool GetArrowKeyNavigation(Control element) => element.GetValue(ArrowKeyNavigationProperty);

    /// <summary>
    /// Sets whether arrow key navigation is enabled.
    /// </summary>
    public static void SetArrowKeyNavigation(Control element, bool value) => element.SetValue(ArrowKeyNavigationProperty, value);

    #endregion

    #region EnterKeyActivates

    /// <summary>
    /// Defines the EnterKeyActivates attached property.
    /// When true, pressing Enter while the control has focus activates it
    /// (e.g., clicks a button, toggles a checkbox).
    /// </summary>
    public static readonly AttachedProperty<bool> EnterKeyActivatesProperty =
        AvaloniaProperty.RegisterAttached<Control, bool>("EnterKeyActivates", typeof(KeyboardNavigationHelper));

    /// <summary>
    /// Gets whether Enter key activates the control.
    /// </summary>
    public static bool GetEnterKeyActivates(Control element) => element.GetValue(EnterKeyActivatesProperty);

    /// <summary>
    /// Sets whether Enter key activates the control.
    /// </summary>
    public static void SetEnterKeyActivates(Control element, bool value) => element.SetValue(EnterKeyActivatesProperty, value);

    #endregion

    #region EscapeKeyDismisses

    /// <summary>
    /// Defines the EscapeKeyDismisses attached property.
    /// When true, pressing Escape while the control has focus dismisses it
    /// (e.g., closes a popup, cancels a dialog).
    /// </summary>
    public static readonly AttachedProperty<bool> EscapeKeyDismissesProperty =
        AvaloniaProperty.RegisterAttached<Control, bool>("EscapeKeyDismisses", typeof(KeyboardNavigationHelper));

    /// <summary>
    /// Gets whether Escape key dismisses the control.
    /// </summary>
    public static bool GetEscapeKeyDismisses(Control element) => element.GetValue(EscapeKeyDismissesProperty);

    /// <summary>
    /// Sets whether Escape key dismisses the control.
    /// </summary>
    public static void SetEscapeKeyDismisses(Control element, bool value) => element.SetValue(EscapeKeyDismissesProperty, value);

    #endregion

    #region TabNavigation

    /// <summary>
    /// Defines the TabNavigation attached property.
    /// Controls how Tab and Shift+Tab navigate within the container.
    /// </summary>
    public static readonly AttachedProperty<TabNavigationMode> TabNavigationProperty =
        AvaloniaProperty.RegisterAttached<Control, TabNavigationMode>("TabNavigation", typeof(KeyboardNavigationHelper), TabNavigationMode.Continue);

    /// <summary>
    /// Gets the tab navigation mode for the container.
    /// </summary>
    public static TabNavigationMode GetTabNavigation(Control element) => element.GetValue(TabNavigationProperty);

    /// <summary>
    /// Sets the tab navigation mode for the container.
    /// </summary>
    public static void SetTabNavigation(Control element, TabNavigationMode value) => element.SetValue(TabNavigationProperty, value);

    #endregion

    static KeyboardNavigationHelper()
    {
        ArrowKeyNavigationProperty.Changed.AddClassHandler<Control>(OnArrowKeyNavigationChanged);
        EnterKeyActivatesProperty.Changed.AddClassHandler<Control>(OnEnterKeyActivatesChanged);
        EscapeKeyDismissesProperty.Changed.AddClassHandler<Control>(OnEscapeKeyDismissesChanged);
    }

    private static void OnArrowKeyNavigationChanged(Control control, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.NewValue is true)
        {
            control.KeyDown += HandleArrowKeyNavigation;
        }
        else
        {
            control.KeyDown -= HandleArrowKeyNavigation;
        }
    }

    private static void OnEnterKeyActivatesChanged(Control control, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.NewValue is true)
        {
            control.KeyDown += HandleEnterActivation;
        }
        else
        {
            control.KeyDown -= HandleEnterActivation;
        }
    }

    private static void OnEscapeKeyDismissesChanged(Control control, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.NewValue is true)
        {
            control.KeyDown += HandleEscapeDismiss;
        }
        else
        {
            control.KeyDown -= HandleEscapeDismiss;
        }
    }

    private static void HandleArrowKeyNavigation(object? sender, KeyEventArgs e)
    {
        if (sender is not Control container)
            return;

        var focused = GetFocusedElement(container);

        if (focused is not Control current)
            return;

        switch (e.Key)
        {
            case Key.Down:
            case Key.Right:
                FocusManager.FocusNext(current);
                e.Handled = true;
                break;

            case Key.Up:
            case Key.Left:
                FocusManager.FocusPrevious(current);
                e.Handled = true;
                break;

            case Key.Home:
                FocusManager.FocusFirst(container);
                e.Handled = true;
                break;

            case Key.End:
                FocusManager.FocusLast(container);
                e.Handled = true;
                break;
        }
    }

    private static void HandleEnterActivation(object? sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter)
            return;

        if (sender is not Control control)
            return;

        // Simulate activation by raising a click event for buttons
        if (control is Button button)
        {
            button.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            e.Handled = true;
        }
        else if (control is ToggleButton toggleButton)
        {
            toggleButton.IsChecked = !toggleButton.IsChecked;
            e.Handled = true;
        }
        else if (control is CheckBox checkBox)
        {
            checkBox.IsChecked = !checkBox.IsChecked;
            e.Handled = true;
        }
        else if (control is RadioButton radioButton)
        {
            radioButton.IsChecked = true;
            e.Handled = true;
        }
    }

    private static void HandleEscapeDismiss(object? sender, KeyEventArgs e)
    {
        if (e.Key != Key.Escape)
            return;

        if (sender is not Control control)
            return;

        // Remove focus from the control
        control.Focus(NavigationMethod.Unspecified);

        // For popup-like controls, raise a close request
        if (control is Popup popup)
        {
            popup.IsOpen = false;
            e.Handled = true;
        }
    }

    /// <summary>
    /// Gets the currently focused element within the specified container's visual tree.
    /// </summary>
    private static IInputElement? GetFocusedElement(Control container)
    {
        var topLevel = TopLevel.GetTopLevel(container);
        return topLevel?.FocusManager?.GetFocusedElement();
    }
}

/// <summary>
/// Defines how Tab key navigation works within a container.
/// </summary>
public enum TabNavigationMode
{
    /// <summary>
    /// Tab continues through the container and moves to the next control outside.
    /// </summary>
    Continue,

    /// <summary>
    /// Tab cycles only within the container.
    /// </summary>
    Cycle,

    /// <summary>
    /// Tab once into the container focuses the first child; subsequent tabs move
    /// to the next control outside the container.
    /// </summary>
    Once,

    /// <summary>
    /// Tab is disabled within the container; only programmatic focus is allowed.
    /// </summary>
    None
}
