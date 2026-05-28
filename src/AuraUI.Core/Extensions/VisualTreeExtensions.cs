using Avalonia;
using Avalonia.VisualTree;

namespace AuraUI.Core.Extensions;

/// <summary>
/// Extension methods for traversing the Avalonia visual tree.
/// </summary>
public static class VisualTreeExtensions
{
    /// <summary>
    /// Find the first ancestor of type T.
    /// </summary>
    public static T? FindAncestor<T>(this Visual visual) where T : Visual
    {
        var current = visual.GetVisualParent();
        while (current != null)
        {
            if (current is T match)
                return match;
            current = current.GetVisualParent();
        }
        return null;
    }

    /// <summary>
    /// Find the first descendant of type T.
    /// </summary>
    public static T? FindDescendant<T>(this Visual visual) where T : Visual
    {
        foreach (var child in visual.GetVisualChildren())
        {
            if (child is T match)
                return match;
            var result = child.FindDescendant<T>();
            if (result != null)
                return result;
        }
        return null;
    }

    /// <summary>
    /// Find all descendants of type T.
    /// </summary>
    public static IEnumerable<T> FindDescendants<T>(this Visual visual) where T : Visual
    {
        foreach (var child in visual.GetVisualChildren())
        {
            if (child is T match)
                yield return match;
            foreach (var descendant in child.FindDescendants<T>())
                yield return descendant;
        }
    }

    /// <summary>
    /// Find a descendant by name.
    /// </summary>
    public static T? FindDescendantByName<T>(this Visual visual, string name) where T : Visual
    {
        foreach (var child in visual.GetVisualChildren())
        {
            if (child is T match && match.Name == name)
                return match;
            var result = child.FindDescendantByName<T>(name);
            if (result != null)
                return result;
        }
        return null;
    }

    /// <summary>
    /// Get the visual root.
    /// </summary>
    public static Visual GetVisualRoot(this Visual visual)
    {
        var current = visual;
        while (current.GetVisualParent() != null)
            current = current.GetVisualParent();
        return current;
    }

    /// <summary>
    /// Check if the visual is a descendant of the specified ancestor.
    /// </summary>
    public static bool IsDescendantOf(this Visual visual, Visual ancestor)
    {
        var current = visual.GetVisualParent();
        while (current != null)
        {
            if (current == ancestor)
                return true;
            current = current.GetVisualParent();
        }
        return false;
    }

    /// <summary>
    /// Get all ancestors of the visual.
    /// </summary>
    public static IEnumerable<Visual> GetAncestors(this Visual visual)
    {
        var current = visual.GetVisualParent();
        while (current != null)
        {
            yield return current;
            current = current.GetVisualParent();
        }
    }

    /// <summary>
    /// Get the first ancestor of type T, or throw if not found.
    /// </summary>
    public static T FindRequiredAncestor<T>(this Visual visual) where T : Visual
    {
        return visual.FindAncestor<T>() ?? throw new InvalidOperationException($"Required ancestor of type {typeof(T).Name} not found.");
    }
}
