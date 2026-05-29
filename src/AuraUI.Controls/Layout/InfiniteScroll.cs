using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.VisualTree;

namespace AuraUI.Controls.Layout;

/// <summary>
/// An attached behavior that triggers loading more items when the user scrolls
/// near the bottom of a scrollable container, inspired by Element Plus's v-infinite-scroll directive.
/// </summary>
public class InfiniteScroll : AvaloniaObject
{
    /// <summary>
    /// Defines the IsEnabled attached property.
    /// </summary>
    public static readonly AttachedProperty<bool> IsEnabledProperty =
        AvaloniaProperty.RegisterAttached<InfiniteScroll, Control, bool>("IsEnabled");

    /// <summary>
    /// Defines the LoadMoreCommand attached property.
    /// The command to execute when more items should be loaded.
    /// </summary>
    public static readonly AttachedProperty<System.Windows.Input.ICommand?> LoadMoreCommandProperty =
        AvaloniaProperty.RegisterAttached<InfiniteScroll, Control, System.Windows.Input.ICommand?>("LoadMoreCommand");

    /// <summary>
    /// Defines the DistanceThreshold attached property.
    /// The distance from the bottom (in pixels) at which loading is triggered.
    /// </summary>
    public static readonly AttachedProperty<double> DistanceThresholdProperty =
        AvaloniaProperty.RegisterAttached<InfiniteScroll, Control, double>("DistanceThreshold", 100.0);

    /// <summary>
    /// Defines the IsLoading attached property.
    /// When true, the load-more command will not fire (prevents duplicate loads).
    /// </summary>
    public static readonly AttachedProperty<bool> IsLoadingProperty =
        AvaloniaProperty.RegisterAttached<InfiniteScroll, Control, bool>("IsLoading");

    /// <summary>
    /// Defines the MaxItems attached property.
    /// When set to a positive value, disables loading after reaching this count.
    /// 0 means no limit.
    /// </summary>
    public static readonly AttachedProperty<int> MaxItemsProperty =
        AvaloniaProperty.RegisterAttached<InfiniteScroll, Control, int>("MaxItems");

    /// <summary>
    /// Defines the ItemCount attached property.
    /// The current number of items in the list.
    /// </summary>
    public static readonly AttachedProperty<int> ItemCountProperty =
        AvaloniaProperty.RegisterAttached<InfiniteScroll, Control, int>("ItemCount");

    private static readonly AttachedProperty<ScrollViewer?> TrackedScrollViewerProperty =
        AvaloniaProperty.RegisterAttached<InfiniteScroll, Control, ScrollViewer?>("TrackedScrollViewer");

    static InfiniteScroll()
    {
        IsEnabledProperty.Changed.AddClassHandler<Control>(OnIsEnabledChanged);
    }

    public static bool GetIsEnabled(Control element) => element.GetValue(IsEnabledProperty);
    public static void SetIsEnabled(Control element, bool value) => element.SetValue(IsEnabledProperty, value);

    public static System.Windows.Input.ICommand? GetLoadMoreCommand(Control element) => element.GetValue(LoadMoreCommandProperty);
    public static void SetLoadMoreCommand(Control element, System.Windows.Input.ICommand? value) => element.SetValue(LoadMoreCommandProperty, value);

    public static double GetDistanceThreshold(Control element) => element.GetValue(DistanceThresholdProperty);
    public static void SetDistanceThreshold(Control element, double value) => element.SetValue(DistanceThresholdProperty, value);

    public static bool GetIsLoading(Control element) => element.GetValue(IsLoadingProperty);
    public static void SetIsLoading(Control element, bool value) => element.SetValue(IsLoadingProperty, value);

    public static int GetMaxItems(Control element) => element.GetValue(MaxItemsProperty);
    public static void SetMaxItems(Control element, int value) => element.SetValue(MaxItemsProperty, value);

    public static int GetItemCount(Control element) => element.GetValue(ItemCountProperty);
    public static void SetItemCount(Control element, int value) => element.SetValue(ItemCountProperty, value);

    private static void OnIsEnabledChanged(Control control, AvaloniaPropertyChangedEventArgs e)
    {
        var isEnabled = (bool)e.NewValue!;
        if (isEnabled)
        {
            AttachScrollWatcher(control);
        }
        else
        {
            DetachScrollWatcher(control);
        }
    }

    private static void AttachScrollWatcher(Control control)
    {
        // Find the nearest ScrollViewer
        control.AttachedToVisualTree += (_, _) =>
        {
            var scrollViewer = FindScrollViewer(control);
            if (scrollViewer != null)
            {
                control.SetValue(TrackedScrollViewerProperty, scrollViewer);
                scrollViewer.ScrollChanged += (s, args) => OnScrollChanged(control, scrollViewer);
            }
        };
    }

    private static void DetachScrollWatcher(Control control)
    {
        var scrollViewer = control.GetValue(TrackedScrollViewerProperty);
        if (scrollViewer != null)
        {
            control.ClearValue(TrackedScrollViewerProperty);
        }
    }

    private static void OnScrollChanged(Control control, ScrollViewer scrollViewer)
    {
        var isLoading = GetIsLoading(control);
        if (isLoading) return;

        var maxItems = GetMaxItems(control);
        var itemCount = GetItemCount(control);
        if (maxItems > 0 && itemCount >= maxItems) return;

        var threshold = GetDistanceThreshold(control);
        var distanceFromBottom = scrollViewer.Extent.Height - scrollViewer.Viewport.Height - scrollViewer.Offset.Y;

        if (distanceFromBottom <= threshold)
        {
            var command = GetLoadMoreCommand(control);
            if (command?.CanExecute(null) == true)
            {
                command.Execute(null);
            }
        }
    }

    private static ScrollViewer? FindScrollViewer(Control control)
    {
        return control.FindAncestorOfType<ScrollViewer>();
    }
}
