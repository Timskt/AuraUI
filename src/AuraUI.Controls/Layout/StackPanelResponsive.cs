using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;

namespace AuraUI.Controls.Layout;

/// <summary>
/// A <see cref="StackPanel"/> that switches its orientation when the available width
/// crosses a configurable breakpoint. Useful for layouts that should be horizontal on
/// wide screens and vertical on narrow screens.
/// </summary>
/// <example>
/// <code>
/// &lt;layout:StackPanelResponsive BreakpointWidth="600"
///     DefaultOrientation="Horizontal"
///     OrientationAtBreakpoint="Vertical"
///     Spacing="8"&gt;
///     &lt;Button Content="First"/&gt;
///     &lt;Button Content="Second"/&gt;
///     &lt;Button Content="Third"/&gt;
/// &lt;/layout:StackPanelResponsive&gt;
/// </code>
/// </example>
public class StackPanelResponsive : StackPanel
{
    /// <summary>
    /// Defines the <see cref="BreakpointWidth"/> styled property.
    /// When the available width is below this value, the panel switches to
    /// <see cref="OrientationAtBreakpoint"/>.
    /// </summary>
    public static readonly StyledProperty<double> BreakpointWidthProperty =
        AvaloniaProperty.Register<StackPanelResponsive, double>(
            nameof(BreakpointWidth),
            600.0);

    /// <summary>
    /// Defines the <see cref="OrientationAtBreakpoint"/> styled property.
    /// The orientation to use when the available width is below <see cref="BreakpointWidth"/>.
    /// </summary>
    public static readonly StyledProperty<Orientation> OrientationAtBreakpointProperty =
        AvaloniaProperty.Register<StackPanelResponsive, Orientation>(
            nameof(OrientationAtBreakpoint),
            Orientation.Vertical);

    /// <summary>
    /// Defines the <see cref="DefaultOrientation"/> styled property.
    /// The orientation to use when the available width is at or above <see cref="BreakpointWidth"/>.
    /// </summary>
    public static readonly StyledProperty<Orientation> DefaultOrientationProperty =
        AvaloniaProperty.Register<StackPanelResponsive, Orientation>(
            nameof(DefaultOrientation),
            Orientation.Horizontal);

    /// <summary>
    /// Defines the <see cref="TransitionDuration"/> styled property.
    /// Reserved for future animation support.
    /// </summary>
    public static readonly StyledProperty<TimeSpan> TransitionDurationProperty =
        AvaloniaProperty.Register<StackPanelResponsive, TimeSpan>(
            nameof(TransitionDuration),
            TimeSpan.FromMilliseconds(200));

    private bool _isAtBreakpoint;

    static StackPanelResponsive()
    {
        BreakpointWidthProperty.Changed.AddClassHandler<StackPanelResponsive>((x, _) => x.UpdateOrientation());
        OrientationAtBreakpointProperty.Changed.AddClassHandler<StackPanelResponsive>((x, _) => x.UpdateOrientation());
        DefaultOrientationProperty.Changed.AddClassHandler<StackPanelResponsive>((x, _) => x.UpdateOrientation());
    }

    /// <summary>
    /// Gets or sets the width threshold below which the panel switches orientation.
    /// </summary>
    public double BreakpointWidth
    {
        get => GetValue(BreakpointWidthProperty);
        set => SetValue(BreakpointWidthProperty, value);
    }

    /// <summary>
    /// Gets or sets the orientation used when the width is below <see cref="BreakpointWidth"/>.
    /// </summary>
    public Orientation OrientationAtBreakpoint
    {
        get => GetValue(OrientationAtBreakpointProperty);
        set => SetValue(OrientationAtBreakpointProperty, value);
    }

    /// <summary>
    /// Gets or sets the orientation used when the width is at or above <see cref="BreakpointWidth"/>.
    /// </summary>
    public Orientation DefaultOrientation
    {
        get => GetValue(DefaultOrientationProperty);
        set => SetValue(DefaultOrientationProperty, value);
    }

    /// <summary>
    /// Gets or sets the transition duration for orientation changes. Reserved for future use.
    /// </summary>
    public TimeSpan TransitionDuration
    {
        get => GetValue(TransitionDurationProperty);
        set => SetValue(TransitionDurationProperty, value);
    }

    /// <summary>
    /// Gets whether the panel is currently in its breakpoint orientation.
    /// </summary>
    public bool IsAtBreakpoint => _isAtBreakpoint;

    protected override Size MeasureOverride(Size availableSize)
    {
        UpdateOrientationFromWidth(availableSize.Width);
        return base.MeasureOverride(availableSize);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        UpdateOrientationFromWidth(finalSize.Width);
        return base.ArrangeOverride(finalSize);
    }

    private void UpdateOrientationFromWidth(double width)
    {
        if (double.IsInfinity(width))
            return;

        var breakpoint = BreakpointWidth;
        var wasAtBreakpoint = _isAtBreakpoint;

        _isAtBreakpoint = width < breakpoint;

        if (_isAtBreakpoint)
        {
            if (Orientation != OrientationAtBreakpoint)
                Orientation = OrientationAtBreakpoint;
        }
        else
        {
            if (Orientation != DefaultOrientation)
                Orientation = DefaultOrientation;
        }
    }

    private void UpdateOrientation()
    {
        InvalidateMeasure();
    }
}
