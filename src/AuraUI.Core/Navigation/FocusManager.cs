using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;

namespace AuraUI.Core.Navigation;

/// <summary>
/// Enhanced focus management for AuraUI applications.
/// Provides focus traversal (next/previous/first/last), focus groups for logical
/// navigation, and attached properties to control focus behavior declaratively.
/// </summary>
/// <example>
/// <code>
/// &lt;StackPanel&gt;
///     &lt;TextBox navigation:FocusManager.FocusGroup="Form1" navigation:FocusManager.TabIndex="0"/&gt;
///     &lt;Button navigation:FocusManager.FocusGroup="Form1" navigation:FocusManager.TabIndex="1"/&gt;
/// &lt;/StackPanel&gt;
/// </code>
/// </example>
public static class FocusManager
{
    #region FocusGroup attached property

    /// <summary>
    /// Defines the FocusGroup attached property.
    /// Controls with the same FocusGroup value are logically grouped for tab navigation.
    /// </summary>
    public static readonly AttachedProperty<string?> FocusGroupProperty =
        AvaloniaProperty.RegisterAttached<Control, string?>("FocusGroup", typeof(FocusManager));

    /// <summary>
    /// Gets the focus group for the specified control.
    /// </summary>
    public static string? GetFocusGroup(Control element) => element.GetValue(FocusGroupProperty);

    /// <summary>
    /// Sets the focus group for the specified control.
    /// </summary>
    public static void SetFocusGroup(Control element, string? value) => element.SetValue(FocusGroupProperty, value);

    #endregion

    #region TabIndex attached property

    /// <summary>
    /// Defines the TabIndex attached property.
    /// Controls the order in which controls receive focus during tab traversal
    /// within their focus group.
    /// </summary>
    public static readonly AttachedProperty<int> TabIndexProperty =
        AvaloniaProperty.RegisterAttached<Control, int>("TabIndex", typeof(FocusManager), int.MaxValue);

    /// <summary>
    /// Gets the tab index for the specified control.
    /// </summary>
    public static int GetTabIndex(Control element) => element.GetValue(TabIndexProperty);

    /// <summary>
    /// Sets the tab index for the specified control.
    /// </summary>
    public static void SetTabIndex(Control element, int value) => element.SetValue(TabIndexProperty, value);

    #endregion

    #region IsFocusable attached property

    /// <summary>
    /// Defines the IsFocusable attached property.
    /// When false, the control is excluded from focus traversal even if it is
    /// otherwise focusable.
    /// </summary>
    public static readonly AttachedProperty<bool> IsFocusableProperty =
        AvaloniaProperty.RegisterAttached<Control, bool>("IsFocusable", typeof(FocusManager), true);

    /// <summary>
    /// Gets whether the control is focusable.
    /// </summary>
    public static bool GetIsFocusable(Control element) => element.GetValue(IsFocusableProperty);

    /// <summary>
    /// Sets whether the control is focusable.
    /// </summary>
    public static void SetIsFocusable(Control element, bool value) => element.SetValue(IsFocusableProperty, value);

    #endregion

    /// <summary>
    /// Moves focus to the next focusable control after the current one.
    /// Respects focus groups and tab index ordering.
    /// </summary>
    /// <param name="current">The currently focused control.</param>
    public static void FocusNext(Control current)
    {
        var candidates = GetFocusableCandidates(current);
        if (candidates.Count == 0)
            return;

        var currentIndex = candidates.IndexOf(current);
        var nextIndex = (currentIndex + 1) % candidates.Count;

        candidates[nextIndex].Focus();
    }

    /// <summary>
    /// Moves focus to the previous focusable control before the current one.
    /// Respects focus groups and tab index ordering.
    /// </summary>
    /// <param name="current">The currently focused control.</param>
    public static void FocusPrevious(Control current)
    {
        var candidates = GetFocusableCandidates(current);
        if (candidates.Count == 0)
            return;

        var currentIndex = candidates.IndexOf(current);
        var previousIndex = currentIndex <= 0 ? candidates.Count - 1 : currentIndex - 1;

        candidates[previousIndex].Focus();
    }

    /// <summary>
    /// Moves focus to the first focusable control within the specified container.
    /// </summary>
    /// <param name="container">The container whose children to search.</param>
    public static void FocusFirst(Control container)
    {
        var candidates = GetFocusableChildren(container);
        if (candidates.Count > 0)
        {
            candidates[0].Focus();
        }
    }

    /// <summary>
    /// Moves focus to the last focusable control within the specified container.
    /// </summary>
    /// <param name="container">The container whose children to search.</param>
    public static void FocusLast(Control container)
    {
        var candidates = GetFocusableChildren(container);
        if (candidates.Count > 0)
        {
            candidates[^1].Focus();
        }
    }

    /// <summary>
    /// Gets all focusable candidates in the same focus group as the given control,
    /// ordered by their TabIndex.
    /// </summary>
    private static List<Control> GetFocusableCandidates(Control current)
    {
        var parent = current.GetVisualParent() as Control;
        if (parent is null)
            return new List<Control>();

        var focusGroup = GetFocusGroup(current);
        var candidates = GetFocusableChildren(parent);

        if (!string.IsNullOrEmpty(focusGroup))
        {
            candidates = candidates
                .Where(c => GetFocusGroup(c) == focusGroup)
                .ToList();
        }

        return candidates;
    }

    /// <summary>
    /// Gets all focusable children of a container, ordered by TabIndex.
    /// </summary>
    private static List<Control> GetFocusableChildren(Control container)
    {
        return container.GetVisualDescendants()
            .OfType<Control>()
            .Where(c => GetIsFocusable(c) && c.Focusable && c.IsVisible && c.IsEnabled)
            .OrderBy(c => GetTabIndex(c))
            .ToList();
    }
}
