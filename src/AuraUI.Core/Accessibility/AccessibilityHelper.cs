using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;
using Avalonia.Automation;

namespace AuraUI.Core.Accessibility;

/// <summary>
/// Provides attached properties for declarative accessibility support and
/// helper methods for focus management and screen reader announcements.
/// </summary>
public static class AccessibilityHelper
{
    #region Attached Properties

    /// <summary>
    /// Attached property for setting the automation identifier on any control.
    /// Maps to <c>AutomationProperties.AutomationId</c> on the platform.
    /// </summary>
    public static readonly AttachedProperty<string?> AutomationIdProperty =
        AvaloniaProperty.RegisterAttached<Control, string?>("AutomationId", typeof(AccessibilityHelper));

    /// <summary>
    /// Attached property for setting the human-readable automation name.
    /// Announced by screen readers when the control receives focus.
    /// </summary>
    public static readonly AttachedProperty<string?> AutomationNameProperty =
        AvaloniaProperty.RegisterAttached<Control, string?>("AutomationName", typeof(AccessibilityHelper));

    /// <summary>
    /// Attached property for setting help text that provides additional context
    /// about the control for assistive technologies.
    /// </summary>
    public static readonly AttachedProperty<string?> AutomationHelpTextProperty =
        AvaloniaProperty.RegisterAttached<Control, string?>("AutomationHelpText", typeof(AccessibilityHelper));

    /// <summary>
    /// Attached property for specifying the type of item in a collection,
    /// useful for data-bound and virtualized controls.
    /// </summary>
    public static readonly AttachedProperty<string?> AutomationItemTypeProperty =
        AvaloniaProperty.RegisterAttached<Control, string?>("AutomationItemType", typeof(AccessibilityHelper));

    /// <summary>
    /// Attached property for specifying the semantic accessibility role of a control.
    /// </summary>
    public static readonly AttachedProperty<AccessibilityRole> AutomationRoleProperty =
        AvaloniaProperty.RegisterAttached<Control, AccessibilityRole>("AutomationRole", typeof(AccessibilityHelper));

    /// <summary>
    /// Attached property indicating whether the control should be included in
    /// keyboard tab navigation.
    /// </summary>
    public static readonly AttachedProperty<bool> IsKeyboardFocusableProperty =
        AvaloniaProperty.RegisterAttached<Control, bool>("IsKeyboardFocusable", typeof(AccessibilityHelper), true);

    #endregion

    #region Static Constructor

    static AccessibilityHelper()
    {
        AutomationIdProperty.Changed.AddClassHandler<Control>(OnAutomationIdChanged);
        AutomationNameProperty.Changed.AddClassHandler<Control>(OnAutomationNameChanged);
        AutomationHelpTextProperty.Changed.AddClassHandler<Control>(OnAutomationHelpTextChanged);
    }

    #endregion

    #region Property Accessors

    /// <summary>Gets the <see cref="AutomationIdProperty"/> value for the specified element.</summary>
    public static string? GetAutomationId(Control element) => element.GetValue(AutomationIdProperty);

    /// <summary>Sets the <see cref="AutomationIdProperty"/> value for the specified element.</summary>
    public static void SetAutomationId(Control element, string? value) => element.SetValue(AutomationIdProperty, value);

    /// <summary>Gets the <see cref="AutomationNameProperty"/> value for the specified element.</summary>
    public static string? GetAutomationName(Control element) => element.GetValue(AutomationNameProperty);

    /// <summary>Sets the <see cref="AutomationNameProperty"/> value for the specified element.</summary>
    public static void SetAutomationName(Control element, string? value) => element.SetValue(AutomationNameProperty, value);

    /// <summary>Gets the <see cref="AutomationHelpTextProperty"/> value for the specified element.</summary>
    public static string? GetAutomationHelpText(Control element) => element.GetValue(AutomationHelpTextProperty);

    /// <summary>Sets the <see cref="AutomationHelpTextProperty"/> value for the specified element.</summary>
    public static void SetAutomationHelpText(Control element, string? value) => element.SetValue(AutomationHelpTextProperty, value);

    /// <summary>Gets the <see cref="AutomationItemTypeProperty"/> value for the specified element.</summary>
    public static string? GetAutomationItemType(Control element) => element.GetValue(AutomationItemTypeProperty);

    /// <summary>Sets the <see cref="AutomationItemTypeProperty"/> value for the specified element.</summary>
    public static void SetAutomationItemType(Control element, string? value) => element.SetValue(AutomationItemTypeProperty, value);

    /// <summary>Gets the <see cref="AutomationRoleProperty"/> value for the specified element.</summary>
    public static AccessibilityRole GetAutomationRole(Control element) => element.GetValue(AutomationRoleProperty);

    /// <summary>Sets the <see cref="AutomationRoleProperty"/> value for the specified element.</summary>
    public static void SetAutomationRole(Control element, AccessibilityRole value) => element.SetValue(AutomationRoleProperty, value);

    /// <summary>Gets the <see cref="IsKeyboardFocusableProperty"/> value for the specified element.</summary>
    public static bool GetIsKeyboardFocusable(Control element) => element.GetValue(IsKeyboardFocusableProperty);

    /// <summary>Sets the <see cref="IsKeyboardFocusableProperty"/> value for the specified element.</summary>
    public static void SetIsKeyboardFocusable(Control element, bool value) => element.SetValue(IsKeyboardFocusableProperty, value);

    #endregion

    #region Screen Reader Announcements

    /// <summary>
    /// Announces a message to screen readers without moving focus.
    /// Uses a hidden live region to trigger the announcement.
    /// </summary>
    /// <param name="message">The message to announce.</param>
    public static void Announce(string message)
    {
        if (string.IsNullOrEmpty(message))
            return;

        var topLevel = Avalonia.Application.Current?.ApplicationLifetime is
            Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop
                ? desktop.MainWindow
                : null;

        if (topLevel is null)
            return;

        var liveRegion = new TextBlock
        {
            Text = message,
            Width = 1,
            Height = 1,
            Opacity = 0,
            IsHitTestVisible = false
        };

        AutomationProperties.SetLiveSetting(liveRegion, AutomationLiveSetting.Assertive);

        var adornerLayer = topLevel.FindDescendantOfType<Panel>();
        adornerLayer?.Children.Add(liveRegion);

        // Remove the live region element after a brief delay to allow the screen reader to process it
        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            adornerLayer?.Children.Remove(liveRegion);
        }, Avalonia.Threading.DispatcherPriority.Default);
    }

    #endregion

    #region Focus Management

    /// <summary>
    /// Moves keyboard focus to the next focusable control in tab order after the specified control.
    /// </summary>
    /// <param name="current">The currently focused control.</param>
    public static void FocusNext(Control current)
    {
        if (current is null)
            return;

        var root = current.GetVisualRoot() as InputElement;
        if (root is null)
            return;

        var focusableElements = GetFocusableDescendants(root);
        var currentIndex = focusableElements.IndexOf(current);

        if (currentIndex >= 0 && currentIndex < focusableElements.Count - 1)
        {
            focusableElements[currentIndex + 1].Focus();
        }
        else if (focusableElements.Count > 0)
        {
            // Wrap around to the first element
            focusableElements[0].Focus();
        }
    }

    /// <summary>
    /// Moves keyboard focus to the previous focusable control in tab order before the specified control.
    /// </summary>
    /// <param name="current">The currently focused control.</param>
    public static void FocusPrevious(Control current)
    {
        if (current is null)
            return;

        var root = current.GetVisualRoot() as InputElement;
        if (root is null)
            return;

        var focusableElements = GetFocusableDescendants(root);
        var currentIndex = focusableElements.IndexOf(current);

        if (currentIndex > 0)
        {
            focusableElements[currentIndex - 1].Focus();
        }
        else if (focusableElements.Count > 0)
        {
            // Wrap around to the last element
            focusableElements[^1].Focus();
        }
    }

    /// <summary>
    /// Collects all keyboard-focusable descendant controls in tab order.
    /// </summary>
    private static List<Control> GetFocusableDescendants(InputElement root)
    {
        var result = new List<Control>();
        CollectFocusableChildren(root, result);
        return result;
    }

    private static void CollectFocusableChildren(InputElement element, List<Control> result)
    {
        if (element is Control control
            && element.Focusable
            && element.IsVisible
            && element.IsEnabled
            && GetIsKeyboardFocusable(control))
        {
            result.Add(control);
        }

        if (element is Panel panel)
        {
            foreach (var child in panel.Children)
            {
                CollectFocusableChildren(child, result);
            }
        }
        else if (element is ContentControl contentControl && contentControl.Content is InputElement content)
        {
            CollectFocusableChildren(content, result);
        }
        else if (element is Border border && border.Child is InputElement borderChild)
        {
            CollectFocusableChildren(borderChild, result);
        }
        else if (element is Decorator decorator && decorator.Child is InputElement decoratorChild)
        {
            CollectFocusableChildren(decoratorChild, result);
        }
        else if (element is ItemsControl itemsControl)
        {
            foreach (var item in itemsControl.Items)
            {
                if (item is InputElement itemElement)
                {
                    CollectFocusableChildren(itemElement, result);
                }
                else
                {
                    var container = itemsControl.ContainerFromItem(item) as InputElement;
                    if (container is not null)
                        CollectFocusableChildren(container, result);
                }
            }
        }
    }

    #endregion

    #region Property Changed Handlers

    private static void OnAutomationIdChanged(Control control, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.NewValue is string id)
        {
            AutomationProperties.SetAutomationId(control, id);
        }
    }

    private static void OnAutomationNameChanged(Control control, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.NewValue is string name)
        {
            AutomationProperties.SetName(control, name);
        }
    }

    private static void OnAutomationHelpTextChanged(Control control, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.NewValue is string helpText)
        {
            AutomationProperties.SetHelpText(control, helpText);
        }
    }

    #endregion
}
