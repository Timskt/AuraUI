using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Media;

namespace AuraUI.Controls.Layout;

/// <summary>
/// Specifies the transform origin anchor.
/// </summary>
public enum TransformOriginAnchor
{
    Center,
    TopLeft,
    TopRight,
    BottomLeft,
    BottomRight,
    Top,
    Bottom,
    Left,
    Right
}

/// <summary>
/// A content control that wraps its content with animated scale, rotation,
/// and translation transforms using <see cref="RenderTransform"/>.
/// </summary>
[PseudoClasses(":scaled", ":rotated", ":translated")]
public class TransformControl : ContentControl
{
    /// <summary>
    /// Defines the <see cref="ScaleX"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> ScaleXProperty =
        AvaloniaProperty.Register<TransformControl, double>(
            nameof(ScaleX),
            1.0);

    /// <summary>
    /// Defines the <see cref="ScaleY"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> ScaleYProperty =
        AvaloniaProperty.Register<TransformControl, double>(
            nameof(ScaleY),
            1.0);

    /// <summary>
    /// Defines the <see cref="RotateAngle"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> RotateAngleProperty =
        AvaloniaProperty.Register<TransformControl, double>(
            nameof(RotateAngle),
            0.0);

    /// <summary>
    /// Defines the <see cref="TranslateX"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> TranslateXProperty =
        AvaloniaProperty.Register<TransformControl, double>(
            nameof(TranslateX),
            0.0);

    /// <summary>
    /// Defines the <see cref="TranslateY"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> TranslateYProperty =
        AvaloniaProperty.Register<TransformControl, double>(
            nameof(TranslateY),
            0.0);

    /// <summary>
    /// Defines the <see cref="TransformOrigin"/> styled property.
    /// </summary>
    public static readonly StyledProperty<TransformOriginAnchor> TransformOriginProperty =
        AvaloniaProperty.Register<TransformControl, TransformOriginAnchor>(
            nameof(TransformOrigin),
            TransformOriginAnchor.Center);

    /// <summary>
    /// Defines the <see cref="AnimationDuration"/> styled property.
    /// </summary>
    public static readonly StyledProperty<TimeSpan> AnimationDurationProperty =
        AvaloniaProperty.Register<TransformControl, TimeSpan>(
            nameof(AnimationDuration),
            TimeSpan.FromMilliseconds(200));

    /// <summary>
    /// Defines the <see cref="IsAnimationEnabled"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsAnimationEnabledProperty =
        AvaloniaProperty.Register<TransformControl, bool>(
            nameof(IsAnimationEnabled),
            true);

    private TransformGroup? _transformGroup;
    private ScaleTransform? _scaleTransform;
    private RotateTransform? _rotateTransform;
    private TranslateTransform? _translateTransform;
    private ContentPresenter? _contentPresenter;

    static TransformControl()
    {
        ScaleXProperty.Changed.AddClassHandler<TransformControl>((x, _) => x.ApplyTransform());
        ScaleYProperty.Changed.AddClassHandler<TransformControl>((x, _) => x.ApplyTransform());
        RotateAngleProperty.Changed.AddClassHandler<TransformControl>((x, _) => x.ApplyTransform());
        TranslateXProperty.Changed.AddClassHandler<TransformControl>((x, _) => x.ApplyTransform());
        TranslateYProperty.Changed.AddClassHandler<TransformControl>((x, _) => x.ApplyTransform());
        TransformOriginProperty.Changed.AddClassHandler<TransformControl>((x, _) => x.ApplyTransformOrigin());
    }

    /// <summary>
    /// Gets or sets the horizontal scale factor.
    /// </summary>
    public double ScaleX
    {
        get => GetValue(ScaleXProperty);
        set => SetValue(ScaleXProperty, value);
    }

    /// <summary>
    /// Gets or sets the vertical scale factor.
    /// </summary>
    public double ScaleY
    {
        get => GetValue(ScaleYProperty);
        set => SetValue(ScaleYProperty, value);
    }

    /// <summary>
    /// Gets or sets the rotation angle in degrees.
    /// </summary>
    public double RotateAngle
    {
        get => GetValue(RotateAngleProperty);
        set => SetValue(RotateAngleProperty, value);
    }

    /// <summary>
    /// Gets or sets the horizontal translation offset.
    /// </summary>
    public double TranslateX
    {
        get => GetValue(TranslateXProperty);
        set => SetValue(TranslateXProperty, value);
    }

    /// <summary>
    /// Gets or sets the vertical translation offset.
    /// </summary>
    public double TranslateY
    {
        get => GetValue(TranslateYProperty);
        set => SetValue(TranslateYProperty, value);
    }

    /// <summary>
    /// Gets or sets the transform origin anchor point.
    /// </summary>
    public TransformOriginAnchor TransformOrigin
    {
        get => GetValue(TransformOriginProperty);
        set => SetValue(TransformOriginProperty, value);
    }

    /// <summary>
    /// Gets or sets the animation duration for transform changes.
    /// </summary>
    public TimeSpan AnimationDuration
    {
        get => GetValue(AnimationDurationProperty);
        set => SetValue(AnimationDurationProperty, value);
    }

    /// <summary>
    /// Gets or sets whether transform transitions are animated.
    /// </summary>
    public bool IsAnimationEnabled
    {
        get => GetValue(IsAnimationEnabledProperty);
        set => SetValue(IsAnimationEnabledProperty, value);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        _scaleTransform = new ScaleTransform();
        _rotateTransform = new RotateTransform();
        _translateTransform = new TranslateTransform();

        _transformGroup = new TransformGroup
        {
            Children = new Transforms
            {
                _scaleTransform,
                _rotateTransform,
                _translateTransform
            }
        };

        _contentPresenter = e.NameScope.Find<ContentPresenter>("PART_ContentPresenter");
        if (_contentPresenter != null)
        {
            _contentPresenter.RenderTransform = _transformGroup;
            ApplyTransformOrigin();
        }

        ApplyTransform();
    }

    private void ApplyTransform()
    {
        if (_scaleTransform == null || _rotateTransform == null || _translateTransform == null)
            return;

        _scaleTransform.ScaleX = ScaleX;
        _scaleTransform.ScaleY = ScaleY;
        _rotateTransform.Angle = RotateAngle;
        _translateTransform.X = TranslateX;
        _translateTransform.Y = TranslateY;

        // Update pseudo classes
        var isScaled = Math.Abs(ScaleX - 1.0) > 0.001 || Math.Abs(ScaleY - 1.0) > 0.001;
        var isRotated = Math.Abs(RotateAngle) > 0.001;
        var isTranslated = Math.Abs(TranslateX) > 0.001 || Math.Abs(TranslateY) > 0.001;

        PseudoClasses.Set(":scaled", isScaled);
        PseudoClasses.Set(":rotated", isRotated);
        PseudoClasses.Set(":translated", isTranslated);
    }

    private void ApplyTransformOrigin()
    {
        if (_contentPresenter == null) return;

        _contentPresenter.RenderTransformOrigin = TransformOrigin switch
        {
            TransformOriginAnchor.TopLeft => new RelativePoint(0, 0, RelativeUnit.Relative),
            TransformOriginAnchor.TopRight => new RelativePoint(1, 0, RelativeUnit.Relative),
            TransformOriginAnchor.BottomLeft => new RelativePoint(0, 1, RelativeUnit.Relative),
            TransformOriginAnchor.BottomRight => new RelativePoint(1, 1, RelativeUnit.Relative),
            TransformOriginAnchor.Top => new RelativePoint(0.5, 0, RelativeUnit.Relative),
            TransformOriginAnchor.Bottom => new RelativePoint(0.5, 1, RelativeUnit.Relative),
            TransformOriginAnchor.Left => new RelativePoint(0, 0.5, RelativeUnit.Relative),
            TransformOriginAnchor.Right => new RelativePoint(1, 0.5, RelativeUnit.Relative),
            _ => new RelativePoint(0.5, 0.5, RelativeUnit.Relative),
        };
    }
}
