using System.Collections.ObjectModel;

namespace AuraUI.Core.MVVM;

/// <summary>
/// Extension methods for ObservableCollection.
/// </summary>
public static class ObservableCollectionExtensions
{
    /// <summary>
    /// Replaces all items in the collection with the new items.
    /// </summary>
    public static void ReplaceAll<T>(this ObservableCollection<T> collection, IEnumerable<T> newItems)
    {
        collection.Clear();
        foreach (var item in newItems)
            collection.Add(item);
    }

    /// <summary>
    /// Adds a range of items to the collection.
    /// </summary>
    public static void AddRange<T>(this ObservableCollection<T> collection, IEnumerable<T> items)
    {
        foreach (var item in items)
            collection.Add(item);
    }

    /// <summary>
    /// Removes all items matching the predicate.
    /// </summary>
    public static int RemoveAll<T>(this ObservableCollection<T> collection, Predicate<T> match)
    {
        var toRemove = collection.Where(x => match(x)).ToList();
        foreach (var item in toRemove)
            collection.Remove(item);
        return toRemove.Count;
    }

    /// <summary>
    /// Sorts the collection in place.
    /// </summary>
    public static void Sort<T>(this ObservableCollection<T> collection) where T : IComparable<T>
    {
        var sorted = collection.OrderBy(x => x).ToList();
        collection.Clear();
        foreach (var item in sorted)
            collection.Add(item);
    }

    /// <summary>
    /// Sorts the collection in place with a comparison.
    /// </summary>
    public static void Sort<T>(this ObservableCollection<T> collection, Comparison<T> comparison)
    {
        var sorted = collection.ToList();
        sorted.Sort(comparison);
        collection.Clear();
        foreach (var item in sorted)
            collection.Add(item);
    }

    /// <summary>
    /// Moves an item from one index to another.
    /// </summary>
    public static void MoveTo<T>(this ObservableCollection<T> collection, int oldIndex, int newIndex)
    {
        if (oldIndex < 0 || oldIndex >= collection.Count || newIndex < 0 || newIndex >= collection.Count)
            return;
        collection.Move(oldIndex, newIndex);
    }
}
