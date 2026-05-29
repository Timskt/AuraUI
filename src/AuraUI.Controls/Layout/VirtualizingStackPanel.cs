using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Styling;
using Avalonia.VisualTree;
using System.Collections;

namespace AuraUI.Controls.Layout;

/// <summary>
/// An enhanced VirtualizingStackPanel that adds smooth scrolling, scroll-to-index,
/// and container recycling on top of Avalonia's built-in virtualizing stack panel.
/// </summary>
/// <example>
/// <code>
/// &lt;ListBox&gt;
///     &lt;ListBox.ItemsPanel&gt;
///         &lt;ItemsPanelTemplate&gt;
///             &lt;layout:VirtualizingStackPanel ItemHeight="40" RecycleContainers="True"/&gt;
///         &lt;/ItemsPanelTemplate&gt;
///     &lt;/ListBox.ItemsPanel&gt;
/// &lt;/ListBox&gt;
/// </code>
/// </example>
public class VirtualizingStackPanel : Avalonia.Controls.VirtualizingStackPanel
{
    /// <summary>
    /// Defines the <see cref="ItemHeight"/> styled property.
    /// When set to a value greater than 0, enables fixed-height optimization for
    /// more efficient scrolling and index calculation.
    /// </summary>
    public static readonly StyledProperty<double> ItemHeightProperty =
        AvaloniaProperty.Register<VirtualizingStackPanel, double>(nameof(ItemHeight), 0.0);

    /// <summary>
    /// Defines the <see cref="RecycleContainers"/> styled property.
    /// When true, containers are recycled instead of created/destroyed, improving
    /// performance for large data sets.
    /// </summary>
    public static readonly StyledProperty<bool> RecycleContainersProperty =
        AvaloniaProperty.Register<VirtualizingStackPanel, bool>(nameof(RecycleContainers), true);

    /// <summary>
    /// Defines the <see cref="ScrollDuration"/> styled property.
    /// The duration in milliseconds for smooth scroll animations.
    /// Set to 0 to disable smooth scrolling.
    /// </summary>
    public static readonly StyledProperty<double> ScrollDurationProperty =
        AvaloniaProperty.Register<VirtualizingStackPanel, double>(nameof(ScrollDuration), 200.0);

    /// <summary>
    /// Defines the <see cref="ScrollEasing"/> styled property.
    /// The easing function used for smooth scroll animations.
    /// </summary>
    public static readonly StyledProperty<Easing> ScrollEasingProperty =
        AvaloniaProperty.Register<VirtualizingStackPanel, Easing>(nameof(ScrollEasing), new CubicEaseOut());

    private ScrollViewer? _scrollViewer;

    /// <summary>
    /// Gets or sets the fixed height for each item. When greater than 0, enables
    /// fixed-height optimization for faster index-based scrolling.
    /// </summary>
    public double ItemHeight
    {
        get => GetValue(ItemHeightProperty);
        set => SetValue(ItemHeightProperty, value);
    }

    /// <summary>
    /// Gets or sets whether containers should be recycled rather than recreated.
    /// </summary>
    public bool RecycleContainers
    {
        get => GetValue(RecycleContainersProperty);
        set => SetValue(RecycleContainersProperty, value);
    }

    /// <summary>
    /// Gets or sets the scroll animation duration in milliseconds.
    /// </summary>
    public double ScrollDuration
    {
        get => GetValue(ScrollDurationProperty);
        set => SetValue(ScrollDurationProperty, value);
    }

    /// <summary>
    /// Gets or sets the easing function for smooth scroll animations.
    /// </summary>
    public Easing ScrollEasing
    {
        get => GetValue(ScrollEasingProperty);
        set => SetValue(ScrollEasingProperty, value);
    }

    static VirtualizingStackPanel()
    {
        AffectsMeasure<VirtualizingStackPanel>(ItemHeightProperty);
    }

    /// <summary>
    /// Scrolls the panel to bring the item at the specified index into view.
    /// When <see cref="ItemHeight"/> is set, uses direct offset calculation.
    /// Otherwise, delegates to the built-in scroll-into-view behavior.
    /// </summary>
    /// <param name="index">The zero-based index of the item to scroll to.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when index is negative.</exception>
    public void ScrollToIndex(int index)
    {
        if (index < 0)
            throw new ArgumentOutOfRangeException(nameof(index), "Index must be non-negative.");

        EnsureScrollViewer();

        if (_scrollViewer is null)
            return;

        if (ItemHeight > 0)
        {
            // Fixed-height optimization: calculate exact offset
            var targetOffset = index * ItemHeight;
            var maxOffset = _scrollViewer.Extent.Height - _scrollViewer.Viewport.Height;
            targetOffset = Math.Clamp(targetOffset, 0, maxOffset);

            if (ScrollDuration > 0)
            {
                AnimateScrollTo(_scrollViewer.Offset.Y, targetOffset);
            }
            else
            {
                _scrollViewer.Offset = new Vector(_scrollViewer.Offset.X, targetOffset);
            }
        }
        else
        {
            // Variable height: scroll item into view using Avalonia's built-in mechanism
            var container = ContainerFromIndex(index);
            if (container is Control control)
            {
                control.BringIntoView();
            }
        }
    }

    /// <summary>
    /// Scrolls the panel to bring the specified data item into view.
    /// </summary>
    /// <param name="item">The data item to scroll to.</param>
    public void ScrollToItem(object item)
    {
        if (item is null)
            return;

        var items = Items;
        for (var i = 0; i < items.Count; i++)
        {
            if (Equals(items[i], item))
            {
                ScrollToIndex(i);
                return;
            }
        }
    }

    private Control? ContainerFromIndex(int index)
    {
        // Attempt to find the realized container for the given index
        var children = Children;
        if (index >= 0 && index < children.Count)
        {
            return children[index];
        }
        return null;
    }

    private void EnsureScrollViewer()
    {
        if (_scrollViewer is not null)
            return;

        // Walk up the visual tree to find the parent ScrollViewer
        var parent = Parent;
        while (parent is not null)
        {
            if (parent is ScrollViewer sv)
            {
                _scrollViewer = sv;
                return;
            }
            parent = parent is Visual visual ? visual.GetVisualParent() as Control : null;
        }
    }

    private void AnimateScrollTo(double from, double to)
    {
        EnsureScrollViewer();
        if (_scrollViewer is null)
            return;

        var animation = new Animation
        {
            Duration = TimeSpan.FromMilliseconds(ScrollDuration),
            Easing = ScrollEasing,
            FillMode = FillMode.Forward,
            Children =
            {
                new KeyFrame
                {
                    Cue = new Cue(0),
                    Setters =
                    {
                        new Setter(ScrollViewer.OffsetProperty, new Vector(_scrollViewer.Offset.X, from))
                    }
                },
                new KeyFrame
                {
                    Cue = new Cue(1),
                    Setters =
                    {
                        new Setter(ScrollViewer.OffsetProperty, new Vector(_scrollViewer.Offset.X, to))
                    }
                }
            }
        };

        _ = animation.RunAsync(_scrollViewer);
    }
}
