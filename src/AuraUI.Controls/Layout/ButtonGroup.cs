using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using AuraUI.Controls.Layout;

namespace AuraUI.Controls.Layout;

/// <summary>
/// A container that groups multiple buttons together with connected borders
/// and consistent sizing. Supports horizontal and vertical orientations.
///
/// Supports size classes: .sm, .md, .lg
/// Supports orientation classes: .horizontal, .vertical
/// </summary>
public class ButtonGroup : ItemsControl
{
    /// <summary>
    /// Defines the <see cref="Orientation"/> styled property.
    /// </summary>
    public static readonly StyledProperty<Orientation> OrientationProperty =
        AvaloniaProperty.Register<ButtonGroup, Orientation>(nameof(Orientation), Orientation.Horizontal);

    /// <summary>
    /// Defines the <see cref="IsAttached"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsAttachedProperty =
        AvaloniaProperty.Register<ButtonGroup, bool>(nameof(IsAttached), true);

    /// <summary>
    /// Defines the <see cref="Size"/> styled property.
    /// </summary>
    public static readonly StyledProperty<ControlSize> SizeProperty =
        AvaloniaProperty.Register<ButtonGroup, ControlSize>(nameof(Size), ControlSize.Medium);

    static ButtonGroup()
    {
        OrientationProperty.Changed.AddClassHandler<ButtonGroup>((x, _) =>
        {
            x.SyncClasses();
            x.SyncItemStates();
        });
        IsAttachedProperty.Changed.AddClassHandler<ButtonGroup>((x, _) =>
        {
            x.SyncClasses();
            x.SyncItemStates();
        });
        SizeProperty.Changed.AddClassHandler<ButtonGroup>((x, _) =>
        {
            x.SyncClasses();
            x.SyncItemStates();
        });
    }

    public ButtonGroup()
    {
        Focusable = false;
        ItemsView.CollectionChanged += (_, _) => SyncItemStates();
        SyncClasses();
    }

    /// <summary>
    /// Gets or sets the orientation of the button group.
    /// </summary>
    public Orientation Orientation
    {
        get => GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }

    /// <summary>
    /// Gets or sets whether buttons are visually attached (no gap between them).
    /// </summary>
    public bool IsAttached
    {
        get => GetValue(IsAttachedProperty);
        set => SetValue(IsAttachedProperty, value);
    }

    /// <summary>
    /// Gets or sets the size of buttons in the group.
    /// </summary>
    public ControlSize Size
    {
        get => GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }

    protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
    {
        return new ButtonGroupText();
    }

    protected override bool NeedsContainerOverride(object? item, int index, out object? recycleKey)
    {
        recycleKey = null;
        return item is not Control;
    }

    protected override void PrepareContainerForItemOverride(Control container, object? item, int index)
    {
        base.PrepareContainerForItemOverride(container, item, index);

        if (container is ButtonGroupText text && item is not Control)
        {
            text.SetCurrentValue(ContentControl.ContentProperty, item);
        }

        SyncItemStates();
    }

    protected override void ClearContainerForItemOverride(Control element)
    {
        ClearItemState(element);
        base.ClearContainerForItemOverride(element);
        SyncItemStates();
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        SyncItemStates();
    }

    internal void SyncItemStates()
    {
        var items = GetGroupControls().ToArray();
        var count = items.Length;

        for (var index = 0; index < count; index++)
        {
            SyncItemState(items[index], index, count);
        }

        Classes.Set("empty", count == 0);
        Classes.Set("has-items", count > 0);
    }

    private void SyncClasses()
    {
        Classes.Set("button-group", true);
        Classes.Set("horizontal", Orientation == Orientation.Horizontal);
        Classes.Set("vertical", Orientation == Orientation.Vertical);
        Classes.Set("attached", IsAttached);
        Classes.Set("separated", !IsAttached);
        Classes.Set("small", Size == ControlSize.Small);
        Classes.Set("medium", Size == ControlSize.Medium);
        Classes.Set("large", Size == ControlSize.Large);
    }

    private void SyncItemState(Control item, int index, int count)
    {
        var single = count == 1;
        var first = index == 0 && !single;
        var last = index == count - 1 && !single;
        var middle = index > 0 && index < count - 1;

        item.Classes.Set("group-item", true);
        item.Classes.Set("button-group-item", true);
        item.Classes.Set("group-single", single);
        item.Classes.Set("group-first", first);
        item.Classes.Set("group-middle", middle);
        item.Classes.Set("group-last", last);
        item.Classes.Set("horizontal", Orientation == Orientation.Horizontal);
        item.Classes.Set("vertical", Orientation == Orientation.Vertical);
    }

    private IEnumerable<Control> GetGroupControls()
    {
        for (var index = 0; index < ItemsView.Count; index++)
        {
            if (GetGroupControlAt(index) is { } item)
            {
                yield return item;
            }
        }
    }

    private Control? GetGroupControlAt(int index)
    {
        if (index < 0 || index >= ItemsView.Count)
        {
            return null;
        }

        return ItemsView[index] as Control ?? ContainerFromIndex(index) as Control;
    }

    private static void ClearItemState(Control item)
    {
        item.Classes.Set("group-item", false);
        item.Classes.Set("button-group-item", false);
        item.Classes.Set("group-single", false);
        item.Classes.Set("group-first", false);
        item.Classes.Set("group-middle", false);
        item.Classes.Set("group-last", false);
        item.Classes.Set("horizontal", false);
        item.Classes.Set("vertical", false);
    }
}

/// <summary>
/// A non-interactive text element within a <see cref="ButtonGroup"/>.
/// </summary>
public class ButtonGroupText : ContentControl
{
    /// <summary>
    /// Defines the <see cref="Size"/> styled property.
    /// </summary>
    public static readonly StyledProperty<ControlSize> SizeProperty =
        AvaloniaProperty.Register<ButtonGroupText, ControlSize>(nameof(Size), ControlSize.Medium);

    static ButtonGroupText()
    {
        SizeProperty.Changed.AddClassHandler<ButtonGroupText>((x, _) => x.SyncClasses());
    }

    public ButtonGroupText()
    {
        IsHitTestVisible = false;
        Focusable = false;
        SyncClasses();
    }

    /// <summary>
    /// Gets or sets the size of the text element.
    /// </summary>
    public ControlSize Size
    {
        get => GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }

    private void SyncClasses()
    {
        Classes.Set("button-group-text", true);
        Classes.Set("small", Size == ControlSize.Small);
        Classes.Set("medium", Size == ControlSize.Medium);
        Classes.Set("large", Size == ControlSize.Large);
    }
}
