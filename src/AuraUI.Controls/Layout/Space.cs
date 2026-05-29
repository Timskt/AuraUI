using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;

namespace AuraUI.Controls.Layout;

/// <summary>
/// Specifies the direction of the <see cref="Space"/> layout.
/// </summary>
public enum SpaceDirection
{
    /// <summary>Items are arranged horizontally.</summary>
    Horizontal,
    /// <summary>Items are arranged vertically.</summary>
    Vertical
}

/// <summary>
/// Specifies the alignment of items within the <see cref="Space"/> container.
/// </summary>
public enum SpaceAlign
{
    /// <summary>Items are stretched to fill the cross axis.</summary>
    Stretch,
    /// <summary>Items are aligned to the start of the cross axis.</summary>
    Start,
    /// <summary>Items are centered on the cross axis.</summary>
    Center,
    /// <summary>Items are aligned to the end of the cross axis.</summary>
    End
}

/// <summary>
/// A flex spacing container inspired by Ant Design's Space component.
/// Arranges child elements in a single direction with uniform spacing,
/// optional wrapping, and configurable alignment.
/// </summary>
public class Space : Panel
{
    /// <summary>
    /// Defines the <see cref="Spacing"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> SpacingProperty =
        AvaloniaProperty.Register<Space, double>(nameof(Spacing), 8.0);

    /// <summary>
    /// Defines the <see cref="Direction"/> styled property.
    /// </summary>
    public static readonly StyledProperty<SpaceDirection> DirectionProperty =
        AvaloniaProperty.Register<Space, SpaceDirection>(nameof(Direction), SpaceDirection.Horizontal);

    /// <summary>
    /// Defines the <see cref="Alignment"/> styled property.
    /// </summary>
    public static readonly StyledProperty<SpaceAlign> AlignmentProperty =
        AvaloniaProperty.Register<Space, SpaceAlign>(nameof(Alignment), SpaceAlign.Center);

    /// <summary>
    /// Defines the <see cref="Wrap"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> WrapProperty =
        AvaloniaProperty.Register<Space, bool>(nameof(Wrap));

    /// <summary>
    /// Defines the <see cref="Separator"/> styled property.
    /// An optional visual element inserted between each child.
    /// </summary>
    public static readonly StyledProperty<Control?> SeparatorProperty =
        AvaloniaProperty.Register<Space, Control?>(nameof(Separator));

    static Space()
    {
        SpacingProperty.Changed.AddClassHandler<Space>((x, _) => x.InvalidateMeasure());
        DirectionProperty.Changed.AddClassHandler<Space>((x, _) => x.InvalidateMeasure());
        WrapProperty.Changed.AddClassHandler<Space>((x, _) => x.InvalidateMeasure());
        AffectsMeasure<Space>(SpacingProperty, DirectionProperty, WrapProperty, AlignmentProperty);
    }

    /// <summary>
    /// Gets or sets the spacing between child elements.
    /// </summary>
    public double Spacing
    {
        get => GetValue(SpacingProperty);
        set => SetValue(SpacingProperty, value);
    }

    /// <summary>
    /// Gets or sets the layout direction.
    /// </summary>
    public SpaceDirection Direction
    {
        get => GetValue(DirectionProperty);
        set => SetValue(DirectionProperty, value);
    }

    /// <summary>
    /// Gets or sets the cross-axis alignment of children.
    /// </summary>
    public SpaceAlign Alignment
    {
        get => GetValue(AlignmentProperty);
        set => SetValue(AlignmentProperty, value);
    }

    /// <summary>
    /// Gets or sets whether children wrap to the next line when space is exhausted.
    /// </summary>
    public bool Wrap
    {
        get => GetValue(WrapProperty);
        set => SetValue(WrapProperty, value);
    }

    /// <summary>
    /// Gets or sets an optional separator control rendered between each child.
    /// </summary>
    public Control? Separator
    {
        get => GetValue(SeparatorProperty);
        set => SetValue(SeparatorProperty, value);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        var isHorizontal = Direction == SpaceDirection.Horizontal;
        var spacing = Spacing;
        var children = GetVisibleChildren();
        if (children.Count == 0)
            return new Size(0, 0);

        double totalMain = 0;
        double maxCross = 0;
        var desiredSizes = new List<Size>();

        foreach (var child in children)
        {
            child.Measure(availableSize);
            var desired = child.DesiredSize;
            desiredSizes.Add(desired);

            if (isHorizontal)
            {
                totalMain += desired.Width;
                maxCross = Math.Max(maxCross, desired.Height);
            }
            else
            {
                totalMain += desired.Height;
                maxCross = Math.Max(maxCross, desired.Width);
            }
        }

        // Add spacing (including separator widths)
        var separatorSize = Separator?.DesiredSize ?? default;
        var separatorMain = isHorizontal ? separatorSize.Width : separatorSize.Height;

        var spacingCount = children.Count - 1;
        if (spacingCount > 0)
        {
            totalMain += spacing * spacingCount;
            if (Separator != null)
                totalMain += separatorMain * spacingCount;
        }

        if (isHorizontal)
            return new Size(totalMain, maxCross);
        else
            return new Size(maxCross, totalMain);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        var isHorizontal = Direction == SpaceDirection.Horizontal;
        var spacing = Spacing;
        var children = GetVisibleChildren();
        if (children.Count == 0)
            return finalSize;

        var separatorSize = Separator?.DesiredSize ?? default;
        var separatorMain = isHorizontal ? separatorSize.Width : separatorSize.Height;

        double offset = 0;

        for (var i = 0; i < children.Count; i++)
        {
            var child = children[i];
            var desired = child.DesiredSize;

            double x, y, w, h;

            if (isHorizontal)
            {
                w = desired.Width;
                h = Wrap ? desired.Height : finalSize.Height;
                x = offset;
                y = Alignment switch
                {
                    SpaceAlign.Start => 0,
                    SpaceAlign.Center => (finalSize.Height - desired.Height) / 2,
                    SpaceAlign.End => finalSize.Height - desired.Height,
                    _ => 0 // Stretch handled by child alignment
                };
                offset += w;
            }
            else
            {
                w = Wrap ? desired.Width : finalSize.Width;
                h = desired.Height;
                y = offset;
                x = Alignment switch
                {
                    SpaceAlign.Start => 0,
                    SpaceAlign.Center => (finalSize.Width - desired.Width) / 2,
                    SpaceAlign.End => finalSize.Width - desired.Width,
                    _ => 0
                };
                offset += h;
            }

            child.Arrange(new Rect(x, y, w, h));

            // Add spacing after each child except the last
            if (i < children.Count - 1)
            {
                offset += spacing;
                if (Separator != null)
                    offset += separatorMain;
            }
        }

        return finalSize;
    }

    private List<Control> GetVisibleChildren()
    {
        var result = new List<Control>();
        foreach (var child in Children)
        {
            if (child.IsVisible)
                result.Add(child);
        }
        return result;
    }
}
