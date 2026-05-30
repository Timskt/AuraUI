using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Avalonia.VisualTree;
using AuraUI.Controls.Layout;

namespace AuraUI.Controls.Input;

/// <summary>
/// Specifies the alignment of an input group addon.
/// </summary>
public enum InputGroupAddonAlign
{
    InlineStart,
    InlineEnd,
    BlockStart,
    BlockEnd
}

/// <summary>
/// A container that groups input controls with attached elements such as
/// prefix text, suffix text, or buttons. Supports automatic border merging
/// between adjacent items.
///
/// Supports size classes: .sm, .md, .lg
/// </summary>
public class InputGroup : ItemsControl
{
    /// <summary>
    /// Defines the <see cref="Size"/> styled property.
    /// </summary>
    public static readonly StyledProperty<ControlSize> SizeProperty =
        AvaloniaProperty.Register<InputGroup, ControlSize>(nameof(Size), ControlSize.Medium);

    static InputGroup()
    {
        SizeProperty.Changed.AddClassHandler<InputGroup>((x, _) =>
        {
            x.SyncClasses();
            x.SyncItemStates();
        });
    }

    public InputGroup()
    {
        Focusable = false;
        ItemsView.CollectionChanged += (_, _) => SyncItemStates();
        AddHandler(InputElement.GotFocusEvent, OnDescendantGotFocus, RoutingStrategies.Bubble, handledEventsToo: true);
        AddHandler(InputElement.LostFocusEvent, OnDescendantLostFocus, RoutingStrategies.Bubble, handledEventsToo: true);
        SyncClasses();
    }

    /// <summary>
    /// Gets or sets the size of the input group.
    /// </summary>
    public ControlSize Size
    {
        get => GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }

    protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
    {
        return new InputGroupText();
    }

    protected override bool NeedsContainerOverride(object? item, int index, out object? recycleKey)
    {
        recycleKey = null;
        return item is not Control;
    }

    protected override void PrepareContainerForItemOverride(Control container, object? item, int index)
    {
        base.PrepareContainerForItemOverride(container, item, index);

        if (container is InputGroupText text && item is not Control)
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

    private void OnDescendantGotFocus(object? sender, FocusChangedEventArgs e)
    {
        Classes.Set("has-focus-within", true);
    }

    private void OnDescendantLostFocus(object? sender, RoutedEventArgs e)
    {
        Dispatcher.UIThread.Post(() =>
        {
            if (this.IsAttachedToVisualTree())
            {
                Classes.Set("has-focus-within", false);
            }
        });
    }

    internal void SyncItemStates()
    {
        var items = GetGroupControls().ToArray();
        var count = items.Length;
        var hasBlockAddon = false;

        for (var index = 0; index < count; index++)
        {
            SyncItemState(items[index], index, count);
            hasBlockAddon |= items[index] is InputGroupAddon
            {
                Align: InputGroupAddonAlign.BlockStart or InputGroupAddonAlign.BlockEnd
            };
        }

        Classes.Set("empty", count == 0);
        Classes.Set("has-items", count > 0);
        Classes.Set("inline", !hasBlockAddon);
        Classes.Set("block", hasBlockAddon);
    }

    private void SyncClasses()
    {
        Classes.Set("small", Size == ControlSize.Small);
        Classes.Set("medium", Size == ControlSize.Medium);
        Classes.Set("large", Size == ControlSize.Large);
        Classes.Set("input-group", true);
    }

    private void SyncItemState(Control item, int index, int count)
    {
        var single = count == 1;
        var first = index == 0 && !single;
        var last = index == count - 1 && !single;
        var middle = index > 0 && index < count - 1;

        item.Classes.Set("group-item", true);
        item.Classes.Set("input-group-item", true);
        item.Classes.Set("group-single", single);
        item.Classes.Set("group-first", first);
        item.Classes.Set("group-middle", middle);
        item.Classes.Set("group-last", last);

        if (item is InputGroupAddon addon)
        {
            addon.SetCurrentValue(InputGroupAddon.SizeProperty, Size);
        }

        if (item is InputGroupText text)
        {
            text.SetCurrentValue(InputGroupText.SizeProperty, Size);
        }
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
        item.Classes.Set("input-group-item", false);
        item.Classes.Set("group-single", false);
        item.Classes.Set("group-first", false);
        item.Classes.Set("group-middle", false);
        item.Classes.Set("group-last", false);
    }
}

/// <summary>
/// An addon that can be placed at the start, end, top, or bottom of an input group.
/// </summary>
public class InputGroupAddon : ContentControl
{
    /// <summary>
    /// Defines the <see cref="Align"/> styled property.
    /// </summary>
    public static readonly StyledProperty<InputGroupAddonAlign> AlignProperty =
        AvaloniaProperty.Register<InputGroupAddon, InputGroupAddonAlign>(nameof(Align), InputGroupAddonAlign.InlineStart);

    /// <summary>
    /// Defines the <see cref="Size"/> styled property.
    /// </summary>
    public static readonly StyledProperty<ControlSize> SizeProperty =
        AvaloniaProperty.Register<InputGroupAddon, ControlSize>(nameof(Size), ControlSize.Medium);

    static InputGroupAddon()
    {
        AlignProperty.Changed.AddClassHandler<InputGroupAddon>((x, _) =>
        {
            x.SyncClasses();
            x.FindAncestorOfType<InputGroup>()?.SyncItemStates();
        });
        SizeProperty.Changed.AddClassHandler<InputGroupAddon>((x, _) => x.SyncClasses());
    }

    public InputGroupAddon()
    {
        Focusable = false;
        SyncClasses();
    }

    /// <summary>
    /// Gets or sets the alignment of the addon within the group.
    /// </summary>
    public InputGroupAddonAlign Align
    {
        get => GetValue(AlignProperty);
        set => SetValue(AlignProperty, value);
    }

    /// <summary>
    /// Gets or sets the size of the addon.
    /// </summary>
    public ControlSize Size
    {
        get => GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }

    private void SyncClasses()
    {
        Classes.Set("input-group-addon", true);
        Classes.Set("align-inline-start", Align == InputGroupAddonAlign.InlineStart);
        Classes.Set("align-inline-end", Align == InputGroupAddonAlign.InlineEnd);
        Classes.Set("align-block-start", Align == InputGroupAddonAlign.BlockStart);
        Classes.Set("align-block-end", Align == InputGroupAddonAlign.BlockEnd);
        Classes.Set("small", Size == ControlSize.Small);
        Classes.Set("medium", Size == ControlSize.Medium);
        Classes.Set("large", Size == ControlSize.Large);
    }
}

/// <summary>
/// A non-interactive text element within an input group (prefix/suffix labels).
/// </summary>
public class InputGroupText : ContentControl
{
    /// <summary>
    /// Defines the <see cref="Size"/> styled property.
    /// </summary>
    public static readonly StyledProperty<ControlSize> SizeProperty =
        AvaloniaProperty.Register<InputGroupText, ControlSize>(nameof(Size), ControlSize.Medium);

    static InputGroupText()
    {
        SizeProperty.Changed.AddClassHandler<InputGroupText>((x, _) => x.SyncClasses());
    }

    public InputGroupText()
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
        Classes.Set("input-group-text", true);
        Classes.Set("small", Size == ControlSize.Small);
        Classes.Set("medium", Size == ControlSize.Medium);
        Classes.Set("large", Size == ControlSize.Large);
    }
}
