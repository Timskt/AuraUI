using Avalonia;
using Avalonia.Controls;

namespace AuraUI.Controls.Layout;

/// <summary>
/// A panel that displays a group of avatars with configurable overlap,
/// used for showing multiple users. Supports a "+N" overflow count.
///
/// Supports size classes: .sm, .md, .lg
/// </summary>
public class AvatarGroup : Panel
{
    /// <summary>
    /// Defines the <see cref="Size"/> styled property.
    /// </summary>
    public static readonly StyledProperty<ControlSize> SizeProperty =
        AvaloniaProperty.Register<AvatarGroup, ControlSize>(nameof(Size), ControlSize.Medium);

    /// <summary>
    /// Defines the <see cref="Overlap"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> OverlapProperty =
        AvaloniaProperty.Register<AvatarGroup, double>(nameof(Overlap), 10d);

    /// <summary>
    /// Defines the <see cref="IsStacked"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsStackedProperty =
        AvaloniaProperty.Register<AvatarGroup, bool>(nameof(IsStacked), true);

    /// <summary>
    /// Defines the <see cref="Max"/> styled property.
    /// </summary>
    public static readonly StyledProperty<int> MaxProperty =
        AvaloniaProperty.Register<AvatarGroup, int>(nameof(Max), 0);

    /// <summary>
    /// Defines the <see cref="ItemCount"/> readonly property.
    /// </summary>
    public static readonly StyledProperty<int> ItemCountProperty =
        AvaloniaProperty.Register<AvatarGroup, int>(nameof(ItemCount));

    static AvatarGroup()
    {
        SizeProperty.Changed.AddClassHandler<AvatarGroup>((x, _) =>
        {
            x.SyncClasses();
            x.InvalidateMeasure();
        });
        OverlapProperty.Changed.AddClassHandler<AvatarGroup>((x, _) => x.InvalidateMeasure());
        IsStackedProperty.Changed.AddClassHandler<AvatarGroup>((x, _) =>
        {
            x.SyncClasses();
            x.InvalidateMeasure();
        });
        MaxProperty.Changed.AddClassHandler<AvatarGroup>((x, _) =>
        {
            x.SyncClasses();
            x.InvalidateMeasure();
        });
        AffectsMeasure<AvatarGroup>(SizeProperty, OverlapProperty, IsStackedProperty, MaxProperty);
    }

    public AvatarGroup()
    {
        SyncClasses();
    }

    /// <summary>
    /// Gets or sets the size of the avatars in the group.
    /// </summary>
    public ControlSize Size
    {
        get => GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }

    /// <summary>
    /// Gets or sets the overlap amount in pixels.
    /// </summary>
    public double Overlap
    {
        get => GetValue(OverlapProperty);
        set => SetValue(OverlapProperty, value);
    }

    /// <summary>
    /// Gets or sets whether avatars are stacked (overlapping).
    /// </summary>
    public bool IsStacked
    {
        get => GetValue(IsStackedProperty);
        set => SetValue(IsStackedProperty, value);
    }

    /// <summary>
    /// Gets or sets the maximum number of avatars to display (0 = unlimited).
    /// </summary>
    public int Max
    {
        get => GetValue(MaxProperty);
        set => SetValue(MaxProperty, value);
    }

    /// <summary>
    /// Gets the number of visible items.
    /// </summary>
    public int ItemCount => GetValue(ItemCountProperty);

    protected override Size MeasureOverride(Size availableSize)
    {
        SyncChildren();

        var width = 0d;
        var height = 0d;
        var index = 0;
        var overlap = EffectiveOverlap();

        foreach (var child in VisibleChildren())
        {
            child.Measure(availableSize);
            var desired = child.DesiredSize;
            width += index == 0 || !IsStacked ? desired.Width : Math.Max(0, desired.Width - overlap);
            height = Math.Max(height, desired.Height);
            index++;
        }

        return new Size(width, height);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        SyncChildren();

        var x = 0d;
        var overlap = EffectiveOverlap();

        foreach (var child in VisibleChildren())
        {
            var desired = child.DesiredSize;
            var height = double.IsFinite(finalSize.Height) && finalSize.Height > 0
                ? finalSize.Height
                : desired.Height;
            child.Arrange(new Rect(x, Math.Max(0, (height - desired.Height) / 2d), desired.Width, desired.Height));
            x += IsStacked ? Math.Max(0, desired.Width - overlap) : desired.Width;
        }

        return finalSize;
    }

    private IEnumerable<Control> VisibleChildren()
    {
        var children = Children.Where(child => child.IsVisible);
        if (Max > 0)
        {
            children = children.Take(Max);
        }
        return children;
    }

    private double EffectiveOverlap()
    {
        return Math.Max(0, Overlap);
    }

    private void SyncChildren()
    {
        var visible = VisibleChildren().ToArray();
        SetValue(ItemCountProperty, visible.Length);
        SyncClasses();

        for (var index = 0; index < visible.Length; index++)
        {
            var child = visible[index];
            child.Classes.Set("avatar-group-item", true);
            child.Classes.Set("group-first", index == 0);
            child.Classes.Set("group-middle", index > 0 && index < visible.Length - 1);
            child.Classes.Set("group-last", index == visible.Length - 1);
            child.SetValue(Panel.ZIndexProperty, index);
        }
    }

    private void SyncClasses()
    {
        Classes.Set("small", Size == ControlSize.Small);
        Classes.Set("medium", Size == ControlSize.Medium);
        Classes.Set("large", Size == ControlSize.Large);
        Classes.Set("stacked", IsStacked);
        Classes.Set("inline", !IsStacked);
        Classes.Set("empty", ItemCount == 0);
        Classes.Set("has-items", ItemCount > 0);
        Classes.Set("has-max", Max > 0);
    }
}
