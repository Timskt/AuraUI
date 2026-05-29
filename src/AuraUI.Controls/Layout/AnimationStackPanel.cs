using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Styling;

namespace AuraUI.Controls.Layout;

/// <summary>
/// A panel that animates child position changes when children are added,
/// removed, or reordered. Uses animated offset transitions for smooth layout changes.
/// </summary>
public class AnimationStackPanel : Panel
{
    /// <summary>
    /// Defines the <see cref="Orientation"/> styled property.
    /// </summary>
    public static readonly StyledProperty<Orientation> OrientationProperty =
        AvaloniaProperty.Register<AnimationStackPanel, Orientation>(
            nameof(Orientation),
            Orientation.Vertical);

    /// <summary>
    /// Defines the <see cref="Spacing"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> SpacingProperty =
        AvaloniaProperty.Register<AnimationStackPanel, double>(
            nameof(Spacing),
            0.0);

    /// <summary>
    /// Defines the <see cref="AnimationDuration"/> styled property.
    /// </summary>
    public static readonly StyledProperty<TimeSpan> AnimationDurationProperty =
        AvaloniaProperty.Register<AnimationStackPanel, TimeSpan>(
            nameof(AnimationDuration),
            TimeSpan.FromMilliseconds(300));

    /// <summary>
    /// Defines the <see cref="AnimationEasing"/> styled property.
    /// </summary>
    public static readonly StyledProperty<Easing> AnimationEasingProperty =
        AvaloniaProperty.Register<AnimationStackPanel, Easing>(
            nameof(AnimationEasing),
            new CubicEaseOut());

    private readonly Dictionary<Control, TranslateTransform> _transforms = new();
    private readonly Dictionary<Control, double> _previousPositions = new();
    private readonly HashSet<Control> _currentChildrenSet = new();

    static AnimationStackPanel()
    {
        AffectsMeasure<AnimationStackPanel>(OrientationProperty, SpacingProperty);
        AffectsArrange<AnimationStackPanel>(OrientationProperty, SpacingProperty);
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
    /// Gets or sets the spacing between children.
    /// </summary>
    public double Spacing
    {
        get => GetValue(SpacingProperty);
        set => SetValue(SpacingProperty, value);
    }

    /// <summary>
    /// Gets or sets the animation duration for position changes.
    /// </summary>
    public TimeSpan AnimationDuration
    {
        get => GetValue(AnimationDurationProperty);
        set => SetValue(AnimationDurationProperty, value);
    }

    /// <summary>
    /// Gets or sets the easing function for animations.
    /// </summary>
    public Easing AnimationEasing
    {
        get => GetValue(AnimationEasingProperty);
        set => SetValue(AnimationEasingProperty, value);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        var isHorizontal = Orientation == Orientation.Horizontal;
        var spacing = Spacing;
        var totalMainAxis = 0.0;
        var maxCrossAxis = 0.0;
        var isFirst = true;

        foreach (var child in Children)
        {
            child.Measure(availableSize);

            if (isHorizontal)
            {
                if (!isFirst) totalMainAxis += spacing;
                totalMainAxis += child.DesiredSize.Width;
                maxCrossAxis = Math.Max(maxCrossAxis, child.DesiredSize.Height);
            }
            else
            {
                if (!isFirst) totalMainAxis += spacing;
                totalMainAxis += child.DesiredSize.Height;
                maxCrossAxis = Math.Max(maxCrossAxis, child.DesiredSize.Width);
            }

            isFirst = false;
        }

        if (isHorizontal)
        {
            return new Size(totalMainAxis, maxCrossAxis);
        }
        else
        {
            return new Size(maxCrossAxis, totalMainAxis);
        }
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        var isHorizontal = Orientation == Orientation.Horizontal;
        var spacing = Spacing;
        var offset = 0.0;

        foreach (var child in Children)
        {
            var childSize = isHorizontal ? child.DesiredSize.Width : child.DesiredSize.Height;
            var crossSize = isHorizontal ? finalSize.Height : finalSize.Width;

            double previousOffset;
            _previousPositions.TryGetValue(child, out previousOffset);

            Rect arrangeRect;
            if (isHorizontal)
            {
                arrangeRect = new Rect(offset, 0, childSize, crossSize);
            }
            else
            {
                arrangeRect = new Rect(0, offset, crossSize, childSize);
            }

            child.Arrange(arrangeRect);

            // Animate if position changed
            if (Math.Abs(previousOffset - offset) > 0.5 && previousOffset != 0)
            {
                AnimateChildPosition(child, previousOffset - offset, isHorizontal);
            }

            _previousPositions[child] = offset;
            offset += childSize + spacing;
        }

        // Cleanup removed children (avoid LINQ allocation)
        _currentChildrenSet.Clear();
        foreach (var c in Children)
            _currentChildrenSet.Add(c);

        // Remove stale entries without LINQ
        foreach (var key in _previousPositions.Keys.ToList())
        {
            if (!_currentChildrenSet.Contains(key))
            {
                _previousPositions.Remove(key);
                _transforms.Remove(key);
            }
        }

        return finalSize;
    }

    private void AnimateChildPosition(Control child, double delta, bool isHorizontal)
    {
        if (!_transforms.TryGetValue(child, out var transform))
        {
            transform = new TranslateTransform();
            _transforms[child] = transform;
            child.RenderTransform = transform;
        }

        // Set initial offset
        if (isHorizontal)
        {
            transform.X = delta;
            transform.Y = 0;
        }
        else
        {
            transform.X = 0;
            transform.Y = delta;
        }

        // Animate back to zero
        var animation = new Avalonia.Animation.Animation
        {
            Duration = AnimationDuration,
            Easing = AnimationEasing,
            FillMode = FillMode.Forward,
            Children =
            {
                new KeyFrame
                {
                    Cue = new Cue(0.0),
                    Setters =
                    {
                        new Setter(isHorizontal ? TranslateTransform.XProperty : TranslateTransform.YProperty,
                            isHorizontal ? (object)delta : (object)delta)
                    }
                },
                new KeyFrame
                {
                    Cue = new Cue(1.0),
                    Setters =
                    {
                        new Setter(isHorizontal ? TranslateTransform.XProperty : TranslateTransform.YProperty,
                            0.0)
                    }
                }
            }
        };

        _ = animation.RunAsync(child);
    }
}
