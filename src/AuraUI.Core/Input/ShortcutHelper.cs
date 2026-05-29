using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using System.Windows.Input;

namespace AuraUI.Core.Input;

/// <summary>
/// Attached properties for declarative keyboard shortcut binding on controls.
/// When a shortcut string is set on a control (e.g., a Button), the shortcut is
/// automatically registered with the <see cref="ShortcutManager"/> and bound to
/// the control's Command property.
/// </summary>
/// <example>
/// <code>
/// &lt;Button Content="Save (Ctrl+S)"
///     input:ShortcutHelper.Shortcut="Ctrl+S"
///     input:ShortcutHelper.ShortcutDescription="Save the current document"
///     Command="{Binding SaveCommand}"/&gt;
/// </code>
/// </example>
public static class ShortcutHelper
{
    #region Shortcut attached property

    /// <summary>
    /// Defines the Shortcut attached property.
    /// A string representation of the keyboard shortcut (e.g., "Ctrl+S", "Ctrl+Shift+Z").
    /// When set on a control with a Command, the shortcut is auto-registered.
    /// </summary>
    public static readonly AttachedProperty<string?> ShortcutProperty =
        AvaloniaProperty.RegisterAttached<Control, string?>("Shortcut", typeof(ShortcutHelper));

    /// <summary>
    /// Gets the shortcut string for the specified control.
    /// </summary>
    public static string? GetShortcut(Control element) => element.GetValue(ShortcutProperty);

    /// <summary>
    /// Sets the shortcut string for the specified control.
    /// </summary>
    public static void SetShortcut(Control element, string? value) => element.SetValue(ShortcutProperty, value);

    #endregion

    #region ShortcutDescription attached property

    /// <summary>
    /// Defines the ShortcutDescription attached property.
    /// A human-readable description of what the shortcut does.
    /// </summary>
    public static readonly AttachedProperty<string?> ShortcutDescriptionProperty =
        AvaloniaProperty.RegisterAttached<Control, string?>("ShortcutDescription", typeof(ShortcutHelper));

    /// <summary>
    /// Gets the shortcut description for the specified control.
    /// </summary>
    public static string? GetShortcutDescription(Control element) => element.GetValue(ShortcutDescriptionProperty);

    /// <summary>
    /// Sets the shortcut description for the specified control.
    /// </summary>
    public static void SetShortcutDescription(Control element, string? value) => element.SetValue(ShortcutDescriptionProperty, value);

    #endregion

    #region IsShortcutEnabled attached property

    /// <summary>
    /// Defines the IsShortcutEnabled attached property.
    /// When false, the shortcut is registered but not active.
    /// </summary>
    public static readonly AttachedProperty<bool> IsShortcutEnabledProperty =
        AvaloniaProperty.RegisterAttached<Control, bool>("IsShortcutEnabled", typeof(ShortcutHelper), true);

    /// <summary>
    /// Gets whether the shortcut is enabled.
    /// </summary>
    public static bool GetIsShortcutEnabled(Control element) => element.GetValue(IsShortcutEnabledProperty);

    /// <summary>
    /// Sets whether the shortcut is enabled.
    /// </summary>
    public static void SetIsShortcutEnabled(Control element, bool value) => element.SetValue(IsShortcutEnabledProperty, value);

    #endregion

    private static KeyboardShortcut? _currentShortcut;

    static ShortcutHelper()
    {
        ShortcutProperty.Changed.AddClassHandler<Control>(OnShortcutChanged);
        ShortcutDescriptionProperty.Changed.AddClassHandler<Control>(OnDescriptionChanged);
        IsShortcutEnabledProperty.Changed.AddClassHandler<Control>(OnEnabledChanged);
    }

    private static void OnShortcutChanged(Control control, AvaloniaPropertyChangedEventArgs e)
    {
        UnregisterCurrentShortcut();

        if (e.NewValue is not string shortcutString || string.IsNullOrWhiteSpace(shortcutString))
            return;

        var command = ResolveCommand(control);
        if (command is null)
            return;

        var description = GetShortcutDescription(control) ?? string.Empty;

        try
        {
            var shortcut = KeyboardShortcut.Parse(shortcutString, command, description);
            shortcut.IsEnabled = GetIsShortcutEnabled(control);
            ShortcutManager.Instance.Register(shortcut);
            _currentShortcut = shortcut;
        }
        catch
        {
            // Invalid shortcut string; ignore silently
        }
    }

    private static void OnDescriptionChanged(Control control, AvaloniaPropertyChangedEventArgs e)
    {
        if (_currentShortcut is not null)
        {
            _currentShortcut.Description = e.NewValue as string ?? string.Empty;
        }
    }

    private static void OnEnabledChanged(Control control, AvaloniaPropertyChangedEventArgs e)
    {
        if (_currentShortcut is not null)
        {
            _currentShortcut.IsEnabled = e.NewValue is true;
        }
    }

    private static ICommand? ResolveCommand(Control control)
    {
        // Try to get Command from common control types
        // ToggleButton inherits from Button, so check it first
        if (control is ToggleButton toggleButton)
            return toggleButton.Command;
        if (control is Button button)
            return button.Command;
        if (control is MenuItem menuItem)
            return menuItem.Command;
        return null;
    }

    private static void UnregisterCurrentShortcut()
    {
        if (_currentShortcut is not null)
        {
            ShortcutManager.Instance.Unregister(_currentShortcut.Key, _currentShortcut.Modifiers);
            _currentShortcut = null;
        }
    }
}
