using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Media;

namespace AuraUI.Controls.Display;

/// <summary>
/// Specifies the orientation of a timeline.
/// </summary>
public enum TimelineOrientation
{
    Horizontal,
    Vertical
}

/// <summary>
/// A timeline control that displays events along a vertical or horizontal line,
/// supporting alternating sides, icons, and colored dots.
/// </summary>
[TemplatePart("PART_TimelineLine", typeof(Border))]
[PseudoClasses(":horizontal", ":vertical", ":alternate")]
public class AuraTimeline : ItemsControl
{
    /// <summary>
    /// Defines the <see cref="Orientation"/> styled property.
    /// </summary>
    public static readonly StyledProperty<TimelineOrientation> OrientationProperty =
        AvaloniaProperty.Register<AuraTimeline, TimelineOrientation>(nameof(Orientation), TimelineOrientation.Vertical);

    /// <summary>
    /// Defines the <see cref="Alternate"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> AlternateProperty =
        AvaloniaProperty.Register<AuraTimeline, bool>(nameof(Alternate), true);

    static AuraTimeline()
    {
        OrientationProperty.Changed.AddClassHandler<AuraTimeline>((x, _) => x.UpdatePseudoClasses());
        AlternateProperty.Changed.AddClassHandler<AuraTimeline>((x, _) => x.UpdatePseudoClasses());
    }

    /// <summary>
    /// Gets or sets the orientation of the timeline.
    /// </summary>
    public TimelineOrientation Orientation
    {
        get => GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }

    /// <summary>
    /// Gets or sets whether items alternate sides (vertical) or top/bottom (horizontal).
    /// </summary>
    public bool Alternate
    {
        get => GetValue(AlternateProperty);
        set => SetValue(AlternateProperty, value);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        UpdatePseudoClasses();
    }

    protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
    {
        return new AuraTimelineItem();
    }

    protected override bool NeedsContainerOverride(object? item, int index, out object? recycleKey)
    {
        return NeedsContainer<AuraTimelineItem>(item, out recycleKey);
    }

    protected override void PrepareContainerForItemOverride(Control container, object? item, int index)
    {
        base.PrepareContainerForItemOverride(container, item, index);
        if (container is AuraTimelineItem timelineItem)
        {
            timelineItem.IsAlternate = Alternate && (index % 2 != 0);
            timelineItem.IsLast = index == (ItemCount - 1);
            timelineItem.PositionIndex = index;
        }
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(":horizontal", Orientation == TimelineOrientation.Horizontal);
        PseudoClasses.Set(":vertical", Orientation == TimelineOrientation.Vertical);
        PseudoClasses.Set(":alternate", Alternate);
    }
}

/// <summary>
/// Represents a single item within an <see cref="AuraTimeline"/>.
/// </summary>
[PseudoClasses(":alternate", ":last", ":left", ":right")]
public class AuraTimelineItem : ContentControl
{
    /// <summary>
    /// Defines the <see cref="Timestamp"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> TimestampProperty =
        AvaloniaProperty.Register<AuraTimelineItem, string?>(nameof(Timestamp));

    /// <summary>
    /// Defines the <see cref="Icon"/> styled property.
    /// </summary>
    public static readonly StyledProperty<object?> IconProperty =
        AvaloniaProperty.Register<AuraTimelineItem, object?>(nameof(Icon));

    /// <summary>
    /// Defines the <see cref="DotColor"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> DotColorProperty =
        AvaloniaProperty.Register<AuraTimelineItem, IBrush?>(nameof(DotColor));

    /// <summary>
    /// Defines the <see cref="IsAlternate"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsAlternateProperty =
        AvaloniaProperty.Register<AuraTimelineItem, bool>(nameof(IsAlternate));

    /// <summary>
    /// Defines the <see cref="IsLast"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsLastProperty =
        AvaloniaProperty.Register<AuraTimelineItem, bool>(nameof(IsLast));

    /// <summary>
    /// Defines the <see cref="PositionIndex"/> styled property.
    /// </summary>
    public static readonly StyledProperty<int> PositionIndexProperty =
        AvaloniaProperty.Register<AuraTimelineItem, int>(nameof(PositionIndex));

    static AuraTimelineItem()
    {
        IsAlternateProperty.Changed.AddClassHandler<AuraTimelineItem>((x, _) => x.UpdatePseudoClasses());
        IsLastProperty.Changed.AddClassHandler<AuraTimelineItem>((x, _) => x.UpdatePseudoClasses());
    }

    /// <summary>
    /// Gets or sets the timestamp text displayed on this item.
    /// </summary>
    public string? Timestamp
    {
        get => GetValue(TimestampProperty);
        set => SetValue(TimestampProperty, value);
    }

    /// <summary>
    /// Gets or sets the icon displayed in the timeline dot.
    /// </summary>
    public object? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    /// <summary>
    /// Gets or sets the color of the timeline dot.
    /// </summary>
    public IBrush? DotColor
    {
        get => GetValue(DotColorProperty);
        set => SetValue(DotColorProperty, value);
    }

    /// <summary>
    /// Gets or sets whether this item is on the alternate side.
    /// </summary>
    public bool IsAlternate
    {
        get => GetValue(IsAlternateProperty);
        set => SetValue(IsAlternateProperty, value);
    }

    /// <summary>
    /// Gets or sets whether this is the last item (hides the connecting line).
    /// </summary>
    public bool IsLast
    {
        get => GetValue(IsLastProperty);
        set => SetValue(IsLastProperty, value);
    }

    /// <summary>
    /// Gets or sets the position index within the timeline.
    /// </summary>
    public int PositionIndex
    {
        get => GetValue(PositionIndexProperty);
        set => SetValue(PositionIndexProperty, value);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        UpdatePseudoClasses();
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(":alternate", IsAlternate);
        PseudoClasses.Set(":last", IsLast);
        PseudoClasses.Set(":left", IsAlternate);
        PseudoClasses.Set(":right", !IsAlternate);
    }
}
