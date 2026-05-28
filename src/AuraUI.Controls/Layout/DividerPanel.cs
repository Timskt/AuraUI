using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;

namespace AuraUI.Controls.Layout;

/// <summary>
/// A panel that automatically inserts <see cref="Divider"/> controls between each child element.
/// Supports both horizontal and vertical orientations.
/// </summary>
public class DividerPanel : Panel
{
    /// <summary>
    /// Defines the <see cref="Orientation"/> styled property.
    /// </summary>
    public static readonly StyledProperty<Orientation> OrientationProperty =
        AvaloniaProperty.Register<DividerPanel, Orientation>(
            nameof(Orientation),
            Orientation.Vertical);

    /// <summary>
    /// Defines the <see cref="DividerThickness"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> DividerThicknessProperty =
        AvaloniaProperty.Register<DividerPanel, double>(
            nameof(DividerThickness),
            1.0);

    /// <summary>
    /// Defines the <see cref="DividerBrush"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> DividerBrushProperty =
        AvaloniaProperty.Register<DividerPanel, IBrush?>(nameof(DividerBrush));

    /// <summary>
    /// Defines the <see cref="DividerMargin"/> styled property.
    /// </summary>
    public static readonly StyledProperty<Thickness> DividerMarginProperty =
        AvaloniaProperty.Register<DividerPanel, Thickness>(
            nameof(DividerMargin));

    /// <summary>
    /// Defines the <see cref="DividerDashArray"/> styled property.
    /// </summary>
    public static readonly StyledProperty<AvaloniaList<double>?> DividerDashArrayProperty =
        AvaloniaProperty.Register<DividerPanel, AvaloniaList<double>?>(nameof(DividerDashArray));

    private readonly List<Divider> _dividers = new();

    static DividerPanel()
    {
        AffectsMeasure<DividerPanel>(
            OrientationProperty,
            DividerThicknessProperty,
            DividerBrushProperty,
            DividerMarginProperty);
        AffectsRender<DividerPanel>(DividerDashArrayProperty);
    }

    /// <summary>
    /// Gets or sets the layout orientation.
    /// </summary>
    public Orientation Orientation
    {
        get => GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }

    /// <summary>
    /// Gets or sets the thickness of the divider line.
    /// </summary>
    public double DividerThickness
    {
        get => GetValue(DividerThicknessProperty);
        set => SetValue(DividerThicknessProperty, value);
    }

    /// <summary>
    /// Gets or sets the brush used to render dividers.
    /// </summary>
    public IBrush? DividerBrush
    {
        get => GetValue(DividerBrushProperty);
        set => SetValue(DividerBrushProperty, value);
    }

    /// <summary>
    /// Gets or sets the margin applied to each divider.
    /// </summary>
    public Thickness DividerMargin
    {
        get => GetValue(DividerMarginProperty);
        set => SetValue(DividerMarginProperty, value);
    }

    /// <summary>
    /// Gets or sets the dash pattern for dividers. Null for solid lines.
    /// </summary>
    public AvaloniaList<double>? DividerDashArray
    {
        get => GetValue(DividerDashArrayProperty);
        set => SetValue(DividerDashArrayProperty, value);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        SyncDividers();

        var isHorizontal = Orientation == Orientation.Horizontal;
        var children = GetRealChildren();
        var childCount = children.Count;
        var dividerCount = childCount > 0 ? childCount - 1 : 0;

        var dividerThickness = DividerThickness;
        var dividerMargin = DividerMargin;
        var totalDividerSize = dividerCount > 0
            ? dividerCount * (dividerThickness + dividerMargin.Top + dividerMargin.Bottom)
            : 0;

        double childAvailableWidth = availableSize.Width;
        double childAvailableHeight = availableSize.Height;

        if (isHorizontal)
        {
            childAvailableWidth -= totalDividerSize;
            if (childAvailableWidth < 0) childAvailableWidth = 0;
        }
        else
        {
            childAvailableHeight -= totalDividerSize;
            if (childAvailableHeight < 0) childAvailableHeight = 0;
        }

        var childConstraint = new Size(childAvailableWidth, childAvailableHeight);

        double maxChildWidth = 0;
        double maxChildHeight = 0;
        double totalChildWidth = 0;
        double totalChildHeight = 0;

        foreach (var child in children)
        {
            child.Measure(childConstraint);

            if (isHorizontal)
            {
                totalChildWidth += child.DesiredSize.Width;
                maxChildHeight = Math.Max(maxChildHeight, child.DesiredSize.Height);
            }
            else
            {
                totalChildHeight += child.DesiredSize.Height;
                maxChildWidth = Math.Max(maxChildWidth, child.DesiredSize.Width);
            }
        }

        if (isHorizontal)
        {
            return new Size(
                totalChildWidth + totalDividerSize,
                maxChildHeight);
        }
        else
        {
            return new Size(
                maxChildWidth,
                totalChildHeight + totalDividerSize);
        }
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        SyncDividers();

        var isHorizontal = Orientation == Orientation.Horizontal;
        var children = GetRealChildren();
        var childCount = children.Count;
        var dividerThickness = DividerThickness;
        var dividerMargin = DividerMargin;
        var dividerExtra = dividerThickness + dividerMargin.Top + dividerMargin.Bottom;

        var offset = 0.0;
        var dividerIndex = 0;

        for (var i = 0; i < childCount; i++)
        {
            var child = children[i];
            var childSize = isHorizontal ? child.DesiredSize.Width : child.DesiredSize.Height;

            if (isHorizontal)
            {
                child.Arrange(new Rect(offset, 0, childSize, finalSize.Height));
                offset += childSize;

                // Insert divider after non-last child.
                if (i < childCount - 1 && dividerIndex < _dividers.Count)
                {
                    var div = _dividers[dividerIndex];
                    var divX = offset + dividerMargin.Left;
                    var divY = dividerMargin.Top;
                    var divWidth = dividerThickness;
                    var divHeight = Math.Max(0, finalSize.Height - dividerMargin.Top - dividerMargin.Bottom);
                    div.Arrange(new Rect(divX, divY, divWidth, divHeight));
                    offset += dividerExtra;
                    dividerIndex++;
                }
            }
            else
            {
                child.Arrange(new Rect(0, offset, finalSize.Width, childSize));
                offset += childSize;

                // Insert divider after non-last child.
                if (i < childCount - 1 && dividerIndex < _dividers.Count)
                {
                    var div = _dividers[dividerIndex];
                    var divX = dividerMargin.Left;
                    var divY = offset + dividerMargin.Top;
                    var divWidth = Math.Max(0, finalSize.Width - dividerMargin.Left - dividerMargin.Right);
                    var divHeight = dividerThickness;
                    div.Arrange(new Rect(divX, divY, divWidth, divHeight));
                    offset += dividerExtra;
                    dividerIndex++;
                }
            }
        }

        return finalSize;
    }

    protected override void ChildrenChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        base.ChildrenChanged(sender, e);
        SyncDividers();
        InvalidateMeasure();
    }

    /// <summary>
    /// Ensures the correct number of <see cref="Divider"/> instances are present
    /// as visual children between real content children.
    /// </summary>
    private void SyncDividers()
    {
        var childCount = Children.Count(c => c is not Divider);
        var desiredDividerCount = Math.Max(0, childCount - 1);

        // Add dividers if needed.
        while (_dividers.Count < desiredDividerCount)
        {
            var divider = CreateDivider();
            _dividers.Add(divider);
            Children.Add(divider);
        }

        // Remove excess dividers.
        while (_dividers.Count > desiredDividerCount)
        {
            var last = _dividers[^1];
            _dividers.RemoveAt(_dividers.Count - 1);
            Children.Remove(last);
        }

        // Update existing divider properties.
        foreach (var divider in _dividers)
        {
            ApplyDividerProperties(divider);
        }
    }

    private Divider CreateDivider()
    {
        var divider = new Divider();
        ApplyDividerProperties(divider);
        return divider;
    }

    private void ApplyDividerProperties(Divider divider)
    {
        divider.Orientation = Orientation;
        divider.StrokeThickness = DividerThickness;

        if (DividerBrush != null)
        {
            divider.Foreground = DividerBrush;
        }

        if (DividerDashArray != null)
        {
            divider.StrokeDashArray = new AvaloniaList<double>(DividerDashArray);
        }
        else
        {
            divider.StrokeDashArray = null;
        }
    }

    /// <summary>
    /// Gets only the non-Divider children (the user's content children).
    /// </summary>
    private IReadOnlyList<Control> GetRealChildren()
    {
        return Children.Where(c => c is not Divider).ToList();
    }
}
