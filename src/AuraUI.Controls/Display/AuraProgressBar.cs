using Avalonia;
using Avalonia.Automation;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Media;

namespace AuraUI.Controls.Display;

/// <summary>
/// Specifies the visual variant / color scheme for a progress bar.
/// </summary>
public enum ProgressVariant
{
    Primary,
    Success,
    Warning,
    Error
}

/// <summary>
/// A progress bar with support for linear and circular modes, label display,
/// and color variants (primary, success, warning, error).
/// </summary>
[TemplatePart("PART_Indicator", typeof(Border))]
[TemplatePart("PART_Label", typeof(TextBlock))]
[PseudoClasses(":linear", ":circular", ":primary", ":success", ":warning", ":error", ":indeterminate")]
public class AuraProgressBar : ProgressBar
{
    /// <summary>
    /// Defines the <see cref="ShowLabel"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> ShowLabelProperty =
        AvaloniaProperty.Register<AuraProgressBar, bool>(nameof(ShowLabel));

    /// <summary>
    /// Defines the <see cref="Variant"/> styled property.
    /// </summary>
    public static readonly StyledProperty<ProgressVariant> VariantProperty =
        AvaloniaProperty.Register<AuraProgressBar, ProgressVariant>(nameof(Variant), ProgressVariant.Primary);

    /// <summary>
    /// Defines the <see cref="IsCircular"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsCircularProperty =
        AvaloniaProperty.Register<AuraProgressBar, bool>(nameof(IsCircular));

    private TextBlock? _label;

    // Cached brushes to avoid per-render allocations
    private static readonly SolidColorBrush s_successBrush = new(Color.Parse("#4CAF50"));
    private static readonly SolidColorBrush s_warningBrush = new(Color.Parse("#FF9800"));
    private static readonly SolidColorBrush s_errorBrush = new(Color.Parse("#F44336"));
    private static readonly SolidColorBrush s_primaryBrush = new(Color.Parse("#2196F3"));

    static AuraProgressBar()
    {
        ShowLabelProperty.Changed.AddClassHandler<AuraProgressBar>((x, _) => x.UpdateLabel());
        VariantProperty.Changed.AddClassHandler<AuraProgressBar>((x, _) => x.UpdatePseudoClasses());
        IsCircularProperty.Changed.AddClassHandler<AuraProgressBar>((x, _) => x.UpdatePseudoClasses());
        ValueProperty.Changed.AddClassHandler<AuraProgressBar>((x, _) => { x.UpdateLabel(); x.UpdateAutomationHelpText(); });
        IsIndeterminateProperty.Changed.AddClassHandler<AuraProgressBar>((x, _) => x.UpdatePseudoClasses());
    }

    /// <summary>
    /// Gets or sets whether to show a percentage label.
    /// </summary>
    public bool ShowLabel
    {
        get => GetValue(ShowLabelProperty);
        set => SetValue(ShowLabelProperty, value);
    }

    /// <summary>
    /// Gets or sets the color variant.
    /// </summary>
    public ProgressVariant Variant
    {
        get => GetValue(VariantProperty);
        set => SetValue(VariantProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the progress bar is rendered as a circle.
    /// </summary>
    public bool IsCircular
    {
        get => GetValue(IsCircularProperty);
        set => SetValue(IsCircularProperty, value);
    }

    /// <summary>
    /// Gets the formatted percentage string.
    /// </summary>
    public string PercentageText
    {
        get
        {
            var min = Minimum;
            var max = Maximum;
            var val = Value;
            var range = max - min;
            if (range <= 0) return "0%";
            var pct = (val - min) / range * 100.0;
            return $"{pct:F0}%";
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _label = e.NameScope.Find<TextBlock>("PART_Label");
        UpdatePseudoClasses();
        UpdateLabel();
        SetValue(AutomationProperties.NameProperty, "Progress");
        UpdateAutomationHelpText();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == ValueProperty || change.Property == MinimumProperty || change.Property == MaximumProperty)
        {
            UpdateLabel();
        }
    }

    private void UpdateLabel()
    {
        if (_label != null)
        {
            _label.Text = PercentageText;
            _label.IsVisible = ShowLabel;
        }
    }

    private void UpdateAutomationHelpText()
    {
        SetValue(AutomationProperties.HelpTextProperty, PercentageText);
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(":linear", !IsCircular);
        PseudoClasses.Set(":circular", IsCircular);
        PseudoClasses.Set(":primary", Variant == ProgressVariant.Primary);
        PseudoClasses.Set(":success", Variant == ProgressVariant.Success);
        PseudoClasses.Set(":warning", Variant == ProgressVariant.Warning);
        PseudoClasses.Set(":error", Variant == ProgressVariant.Error);
        PseudoClasses.Set(":indeterminate", IsIndeterminate);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        if (IsCircular)
        {
            var size = Math.Min(availableSize.Width, availableSize.Height);
            if (double.IsInfinity(size)) size = 48;
            return new Size(size, size);
        }
        return base.MeasureOverride(availableSize);
    }

    // Cached arc geometry for circular mode
    private StreamGeometry? _cachedArcGeometry;
    private double _cachedArcRadius;
    private double _cachedArcSweep;

    public override void Render(DrawingContext context)
    {
        if (!IsCircular)
        {
            base.Render(context);
            return;
        }

        // Circular rendering
        var bounds = new Rect(Bounds.Size);
        if (bounds.Width <= 0 || bounds.Height <= 0) return;

        var strokeWidth = 4.0;
        var radius = Math.Min(bounds.Width, bounds.Height) / 2 - strokeWidth / 2;
        if (radius <= 0) return;

        var center = new Point(bounds.Width / 2, bounds.Height / 2);
        var trackPen = new Pen(Brushes.LightGray, strokeWidth);

        var ringBrush = Variant switch
        {
            ProgressVariant.Success => s_successBrush,
            ProgressVariant.Warning => s_warningBrush,
            ProgressVariant.Error => s_errorBrush,
            _ => s_primaryBrush
        };
        var ringPen = new Pen(ringBrush, strokeWidth);

        // Track
        context.DrawEllipse(null, trackPen, center, radius, radius);

        // Arc
        var range = Maximum - Minimum;
        if (range > 0)
        {
            var normalized = (Value - Minimum) / range;
            var sweepAngle = normalized * 360.0;
            if (sweepAngle > 0.5)
            {
                // Rebuild geometry only if radius or sweep changed significantly
                if (_cachedArcGeometry == null ||
                    Math.Abs(_cachedArcRadius - radius) > 0.001 ||
                    Math.Abs(_cachedArcSweep - sweepAngle) > 0.1)
                {
                    var startAngleRad = -90 * Math.PI / 180.0;
                    var endAngleRad = (-90 + sweepAngle) * Math.PI / 180.0;

                    var startPoint = new Point(center.X + radius * Math.Cos(startAngleRad), center.Y + radius * Math.Sin(startAngleRad));
                    var endPoint = new Point(center.X + radius * Math.Cos(endAngleRad), center.Y + radius * Math.Sin(endAngleRad));

                    var geometry = new StreamGeometry();
                    using (var ctx = geometry.Open())
                    {
                        ctx.BeginFigure(startPoint, false);
                        ctx.ArcTo(endPoint, new Size(radius, radius), 0, sweepAngle > 180, SweepDirection.Clockwise);
                        ctx.EndFigure(false);
                    }

                    // _cachedArcGeometry is replaced (no Dispose needed in Avalonia 11)
                    _cachedArcGeometry = geometry;
                    _cachedArcRadius = radius;
                    _cachedArcSweep = sweepAngle;
                }

                context.DrawGeometry(null, ringPen, _cachedArcGeometry);
            }
        }
    }
}
