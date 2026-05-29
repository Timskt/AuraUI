using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Threading;

namespace AuraUI.Controls.Layout;

/// <summary>
/// A placeholder loading indicator that displays a shimmer animation
/// while content is being loaded. Supports text, circle, rect, and image shapes.
/// </summary>
[PseudoClasses(":text", ":circle", ":rect", ":image", ":animated")]
public class Skeleton : Control
{
    private readonly LinearGradientBrush _shimmerBrush;
    private DispatcherTimer? _animationTimer;
    private static readonly SolidColorBrush s_defaultBaseBrush = new(Color.FromArgb(40, 128, 128, 128));
    private double _animationOffset;

    /// <summary>
    /// Defines the <see cref="Variant"/> styled property.
    /// </summary>
    public static readonly StyledProperty<SkeletonVariant> VariantProperty =
        AvaloniaProperty.Register<Skeleton, SkeletonVariant>(
            nameof(Variant),
            SkeletonVariant.Text);

    /// <summary>
    /// Defines the <see cref="IsAnimated"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsAnimatedProperty =
        AvaloniaProperty.Register<Skeleton, bool>(
            nameof(IsAnimated),
            true);

    /// <summary>
    /// Defines the <see cref="Rows"/> styled property.
    /// </summary>
    public static readonly StyledProperty<int> RowsProperty =
        AvaloniaProperty.Register<Skeleton, int>(
            nameof(Rows),
            3);

    /// <summary>
    /// Defines the <see cref="RowSpacing"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> RowSpacingProperty =
        AvaloniaProperty.Register<Skeleton, double>(
            nameof(RowSpacing),
            8.0);

    /// <summary>
    /// Defines the <see cref="BaseColor"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> BaseColorProperty =
        AvaloniaProperty.Register<Skeleton, IBrush?>(nameof(BaseColor));

    /// <summary>
    /// Defines the <see cref="ShimmerColor"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> ShimmerColorProperty =
        AvaloniaProperty.Register<Skeleton, IBrush?>(nameof(ShimmerColor));

    /// <summary>
    /// Defines the <see cref="CornerRadius"/> styled property.
    /// </summary>
    public static readonly StyledProperty<CornerRadius> CornerRadiusProperty =
        AvaloniaProperty.Register<Skeleton, CornerRadius>(
            nameof(CornerRadius),
            new CornerRadius(4));

    static Skeleton()
    {
        AffectsRender<Skeleton>(
            VariantProperty,
            IsAnimatedProperty,
            RowsProperty,
            RowSpacingProperty,
            BaseColorProperty,
            ShimmerColorProperty,
            CornerRadiusProperty);
        AffectsMeasure<Skeleton>(
            VariantProperty,
            RowsProperty,
            RowSpacingProperty);
    }

    public Skeleton()
    {
        _shimmerBrush = new LinearGradientBrush
        {
            StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
            EndPoint = new RelativePoint(1, 0, RelativeUnit.Relative),
            GradientStops =
            {
                new GradientStop(Color.FromArgb(30, 128, 128, 128), 0.0),
                new GradientStop(Color.FromArgb(60, 128, 128, 128), 0.5),
                new GradientStop(Color.FromArgb(30, 128, 128, 128), 1.0)
            }
        };

        UpdatePseudoClasses();
    }

    /// <summary>
    /// Gets or sets the skeleton shape variant.
    /// </summary>
    public SkeletonVariant Variant
    {
        get => GetValue(VariantProperty);
        set => SetValue(VariantProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the shimmer animation is active.
    /// </summary>
    public bool IsAnimated
    {
        get => GetValue(IsAnimatedProperty);
        set => SetValue(IsAnimatedProperty, value);
    }

    /// <summary>
    /// Gets or sets the number of text rows (only used when <see cref="Variant"/> is <see cref="SkeletonVariant.Text"/>).
    /// </summary>
    public int Rows
    {
        get => GetValue(RowsProperty);
        set => SetValue(RowsProperty, value);
    }

    /// <summary>
    /// Gets or sets the spacing between text rows.
    /// </summary>
    public double RowSpacing
    {
        get => GetValue(RowSpacingProperty);
        set => SetValue(RowSpacingProperty, value);
    }

    /// <summary>
    /// Gets or sets the base fill color of the skeleton. Defaults to a system-appropriate gray.
    /// </summary>
    public IBrush? BaseColor
    {
        get => GetValue(BaseColorProperty);
        set => SetValue(BaseColorProperty, value);
    }

    /// <summary>
    /// Gets or sets the shimmer highlight color.
    /// </summary>
    public IBrush? ShimmerColor
    {
        get => GetValue(ShimmerColorProperty);
        set => SetValue(ShimmerColorProperty, value);
    }

    /// <summary>
    /// Gets or sets the corner radius of the skeleton shape.
    /// </summary>
    public CornerRadius CornerRadius
    {
        get => GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        StartAnimation();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        StopAnimation();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == VariantProperty)
        {
            UpdatePseudoClasses();
        }
        else if (change.Property == IsAnimatedProperty)
        {
            UpdatePseudoClasses();
            if (IsAnimated)
            {
                StartAnimation();
            }
            else
            {
                StopAnimation();
            }
        }
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(":text", Variant == SkeletonVariant.Text);
        PseudoClasses.Set(":circle", Variant == SkeletonVariant.Circle);
        PseudoClasses.Set(":rect", Variant == SkeletonVariant.Rect);
        PseudoClasses.Set(":image", Variant == SkeletonVariant.Image);
        PseudoClasses.Set(":animated", IsAnimated);
    }

    private void StartAnimation()
    {
        if (_animationTimer != null || !IsAnimated) return;

        _animationTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(16) // ~60fps
        };
        _animationTimer.Tick += OnAnimationTick;
        _animationTimer.Start();
    }

    private void StopAnimation()
    {
        if (_animationTimer == null) return;

        _animationTimer.Stop();
        _animationTimer.Tick -= OnAnimationTick;
        _animationTimer = null;
    }

    private void OnAnimationTick(object? sender, EventArgs e)
    {
        _animationOffset += 0.01;
        if (_animationOffset > 1.0)
        {
            _animationOffset = -0.5; // Start offscreen to the left
        }

        // Shift the gradient stops to simulate shimmer movement.
        var stops = _shimmerBrush.GradientStops;
        if (stops.Count >= 3)
        {
            stops[0].Offset = Math.Clamp(_animationOffset - 0.3, 0, 1);
            stops[1].Offset = Math.Clamp(_animationOffset, 0, 1);
            stops[2].Offset = Math.Clamp(_animationOffset + 0.3, 0, 1);
        }

        InvalidateVisual();
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        var bounds = Bounds;

        return Variant switch
        {
            SkeletonVariant.Text => MeasureText(availableSize),
            SkeletonVariant.Circle => MeasureCircle(availableSize),
            SkeletonVariant.Rect => MeasureRect(availableSize),
            SkeletonVariant.Image => MeasureImage(availableSize),
            _ => base.MeasureOverride(availableSize)
        };
    }

    private Size MeasureText(Size availableSize)
    {
        var rows = Math.Max(1, Rows);
        var rowHeight = 14.0;
        var spacing = RowSpacing;
        var totalHeight = (rows * rowHeight) + ((rows - 1) * spacing);
        var width = double.IsInfinity(availableSize.Width) ? 200 : availableSize.Width;
        return new Size(width, totalHeight);
    }

    private Size MeasureCircle(Size availableSize)
    {
        var size = Math.Min(
            double.IsInfinity(availableSize.Width) ? 40 : availableSize.Width,
            double.IsInfinity(availableSize.Height) ? 40 : availableSize.Height);
        return new Size(size, size);
    }

    private Size MeasureRect(Size availableSize)
    {
        var width = double.IsInfinity(availableSize.Width) ? 200 : availableSize.Width;
        var height = double.IsInfinity(availableSize.Height) ? 100 : availableSize.Height;
        return new Size(width, height);
    }

    private Size MeasureImage(Size availableSize)
    {
        var width = double.IsInfinity(availableSize.Width) ? 300 : availableSize.Width;
        var height = width * 0.5625; // 16:9 aspect ratio
        return new Size(width, height);
    }

    public override void Render(DrawingContext context)
    {
        var bounds = Bounds;
        if (bounds.Width <= 0 || bounds.Height <= 0) return;

        var baseBrush = BaseColor ?? s_defaultBaseBrush;
        var cornerRadius = CornerRadius;

        switch (Variant)
        {
            case SkeletonVariant.Text:
                RenderTextSkeleton(context, bounds, baseBrush, cornerRadius);
                break;

            case SkeletonVariant.Circle:
                RenderCircleSkeleton(context, bounds, baseBrush);
                break;

            case SkeletonVariant.Rect:
                RenderRectSkeleton(context, bounds, baseBrush, cornerRadius);
                break;

            case SkeletonVariant.Image:
                RenderRectSkeleton(context, bounds, baseBrush, cornerRadius);
                break;
        }
    }

    private void RenderTextSkeleton(DrawingContext context, Rect bounds, IBrush baseBrush, CornerRadius cornerRadius)
    {
        var rows = Math.Max(1, Rows);
        var rowHeight = 14.0;
        var spacing = RowSpacing;
        var y = 0.0;
        var radiusX = cornerRadius.TopLeft;
        var radiusY = cornerRadius.TopLeft;

        for (var i = 0; i < rows; i++)
        {
            // Last row is typically shorter.
            var rowWidth = (i == rows - 1) ? bounds.Width * 0.6 : bounds.Width;
            var rect = new Rect(0, y, rowWidth, rowHeight);

            // Draw base fill.
            context.DrawRectangle(baseBrush, null, rect, radiusX, radiusY, default);

            // Draw shimmer overlay.
            if (IsAnimated)
            {
                context.DrawRectangle(_shimmerBrush, null, rect, radiusX, radiusY, default);
            }

            y += rowHeight + spacing;
        }
    }

    private void RenderCircleSkeleton(DrawingContext context, Rect bounds, IBrush baseBrush)
    {
        var radius = Math.Min(bounds.Width, bounds.Height) / 2.0;
        var center = new Point(bounds.Width / 2.0, bounds.Height / 2.0);

        // Draw base fill.
        context.DrawEllipse(baseBrush, null, center, radius, radius);

        // Draw shimmer overlay.
        if (IsAnimated)
        {
            context.DrawEllipse(_shimmerBrush, null, center, radius, radius);
        }
    }

    private void RenderRectSkeleton(DrawingContext context, Rect bounds, IBrush baseBrush, CornerRadius cornerRadius)
    {
        var radiusX = cornerRadius.TopLeft;
        var radiusY = cornerRadius.TopLeft;

        // Draw base fill.
        context.DrawRectangle(baseBrush, null, bounds, radiusX, radiusY, default);

        // Draw shimmer overlay.
        if (IsAnimated)
        {
            context.DrawRectangle(_shimmerBrush, null, bounds, radiusX, radiusY, default);
        }
    }
}
