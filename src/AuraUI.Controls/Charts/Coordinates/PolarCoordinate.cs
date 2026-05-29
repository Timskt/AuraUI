using Avalonia;
using Avalonia.Media;

namespace AuraUI.Controls.Charts.Coordinates;

/// <summary>
/// Polar coordinate system for radial charts. Provides a circular grid with
/// angle and radius axes, supporting polar bar, line, and scatter plots.
///
/// Features:
///   - Circular grid lines (concentric circles)
///   - Angular axis lines (radial spokes)
///   - Angle labels (0, 90, 180, 270 degrees)
///   - Radius labels
///   - Support for polar bar, line, and scatter series
///
/// Usage:
///   var polar = new PolarCoordinate();
///   polar.ShowGrid = true;
///   polar.ShowAngleAxis = true;
///   polar.ShowRadiusAxis = true;
///   chart.PolarCoordinate = polar;
/// </summary>
public class PolarCoordinate : AvaloniaObject
{
    /// <summary>
    /// Defines the <see cref="ShowGrid"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> ShowGridProperty =
        AvaloniaProperty.Register<PolarCoordinate, bool>(nameof(ShowGrid), true);

    /// <summary>
    /// Defines the <see cref="ShowAngleAxis"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> ShowAngleAxisProperty =
        AvaloniaProperty.Register<PolarCoordinate, bool>(nameof(ShowAngleAxis), true);

    /// <summary>
    /// Defines the <see cref="ShowRadiusAxis"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> ShowRadiusAxisProperty =
        AvaloniaProperty.Register<PolarCoordinate, bool>(nameof(ShowRadiusAxis), true);

    /// <summary>
    /// Defines the <see cref="StartAngle"/> styled property.
    /// Starting angle in degrees (0 = top, 90 = right).
    /// </summary>
    public static readonly StyledProperty<double> StartAngleProperty =
        AvaloniaProperty.Register<PolarCoordinate, double>(nameof(StartAngle), 90.0);

    /// <summary>
    /// Defines the <see cref="Clockwise"/> styled property.
    /// Whether angles increase clockwise.
    /// </summary>
    public static readonly StyledProperty<bool> ClockwiseProperty =
        AvaloniaProperty.Register<PolarCoordinate, bool>(nameof(Clockwise), true);

    /// <summary>
    /// Defines the <see cref="GridLevels"/> styled property.
    /// Number of concentric grid circles.
    /// </summary>
    public static readonly StyledProperty<int> GridLevelsProperty =
        AvaloniaProperty.Register<PolarCoordinate, int>(nameof(GridLevels), 5);

    /// <summary>
    /// Defines the <see cref="AngleDivisions"/> styled property.
    /// Number of angular divisions (spokes).
    /// </summary>
    public static readonly StyledProperty<int> AngleDivisionsProperty =
        AvaloniaProperty.Register<PolarCoordinate, int>(nameof(AngleDivisions), 12);

    /// <summary>
    /// Defines the <see cref="GridColor"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> GridColorProperty =
        AvaloniaProperty.Register<PolarCoordinate, IBrush?>(nameof(GridColor));

    /// <summary>
    /// Defines the <see cref="GridThickness"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> GridThicknessProperty =
        AvaloniaProperty.Register<PolarCoordinate, double>(nameof(GridThickness), 0.5);

    /// <summary>
    /// Defines the <see cref="MaxRadius"/> styled property.
    /// Maximum radius value for the polar coordinate system.
    /// </summary>
    public static readonly StyledProperty<double> MaxRadiusProperty =
        AvaloniaProperty.Register<PolarCoordinate, double>(nameof(MaxRadius), 100.0);

    /// <summary>
    /// Defines the <see cref="InnerRadius"/> styled property.
    /// Inner radius as a fraction of the total radius (0 = from center, >0 = donut).
    /// </summary>
    public static readonly StyledProperty<double> InnerRadiusProperty =
        AvaloniaProperty.Register<PolarCoordinate, double>(nameof(InnerRadius), 0.0,
            coerce: (_, v) => Math.Clamp(v, 0.0, 0.9));

    /// <summary>
    /// Defines the <see cref="ShowAngleLabels"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> ShowAngleLabelsProperty =
        AvaloniaProperty.Register<PolarCoordinate, bool>(nameof(ShowAngleLabels), true);

    /// <summary>
    /// Defines the <see cref="ShowRadiusLabels"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> ShowRadiusLabelsProperty =
        AvaloniaProperty.Register<PolarCoordinate, bool>(nameof(ShowRadiusLabels), true);

    /// <summary>
    /// Defines the <see cref="LabelFontSize"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> LabelFontSizeProperty =
        AvaloniaProperty.Register<PolarCoordinate, double>(nameof(LabelFontSize), 10.0);

    public bool ShowGrid { get => GetValue(ShowGridProperty); set => SetValue(ShowGridProperty, value); }
    public bool ShowAngleAxis { get => GetValue(ShowAngleAxisProperty); set => SetValue(ShowAngleAxisProperty, value); }
    public bool ShowRadiusAxis { get => GetValue(ShowRadiusAxisProperty); set => SetValue(ShowRadiusAxisProperty, value); }
    public double StartAngle { get => GetValue(StartAngleProperty); set => SetValue(StartAngleProperty, value); }
    public bool Clockwise { get => GetValue(ClockwiseProperty); set => SetValue(ClockwiseProperty, value); }
    public int GridLevels { get => GetValue(GridLevelsProperty); set => SetValue(GridLevelsProperty, value); }
    public int AngleDivisions { get => GetValue(AngleDivisionsProperty); set => SetValue(AngleDivisionsProperty, value); }
    public IBrush? GridColor { get => GetValue(GridColorProperty); set => SetValue(GridColorProperty, value); }
    public double GridThickness { get => GetValue(GridThicknessProperty); set => SetValue(GridThicknessProperty, value); }
    public double MaxRadius { get => GetValue(MaxRadiusProperty); set => SetValue(MaxRadiusProperty, value); }
    public double InnerRadius { get => GetValue(InnerRadiusProperty); set => SetValue(InnerRadiusProperty, value); }
    public bool ShowAngleLabels { get => GetValue(ShowAngleLabelsProperty); set => SetValue(ShowAngleLabelsProperty, value); }
    public bool ShowRadiusLabels { get => GetValue(ShowRadiusLabelsProperty); set => SetValue(ShowRadiusLabelsProperty, value); }
    public double LabelFontSize { get => GetValue(LabelFontSizeProperty); set => SetValue(LabelFontSizeProperty, value); }

    /// <summary>
    /// Render the polar grid (concentric circles and radial spokes).
    /// </summary>
    public void RenderGrid(DrawingContext context, Rect plotArea)
    {
        var center = new Point(plotArea.Center.X, plotArea.Center.Y);
        var outerRadius = Math.Min(plotArea.Width, plotArea.Height) / 2 - 10;
        var innerRadius = outerRadius * InnerRadius;
        var gridColor = GridColor ?? Brushes.LightGray;
        var gridPen = new Pen(gridColor, GridThickness);

        // Draw concentric circles
        if (ShowGrid)
        {
            for (int level = 1; level <= GridLevels; level++)
            {
                var radius = innerRadius + (outerRadius - innerRadius) * level / GridLevels;
                context.DrawEllipse(null, gridPen, center, radius, radius);
            }
        }

        // Draw radial spokes
        if (ShowAngleAxis)
        {
            for (int i = 0; i < AngleDivisions; i++)
            {
                var angle = StartAngle + (360.0 / AngleDivisions) * i;
                var rad = angle * Math.PI / 180;
                var direction = Clockwise ? 1 : -1;
                var adjustedRad = rad * direction;

                var endPoint = new Point(
                    center.X + outerRadius * Math.Cos(adjustedRad),
                    center.Y + outerRadius * Math.Sin(adjustedRad));

                context.DrawLine(gridPen, center, endPoint);
            }
        }

        // Draw angle labels
        if (ShowAngleLabels && ShowAngleAxis)
        {
            DrawAngleLabels(context, center, outerRadius);
        }

        // Draw radius labels
        if (ShowRadiusLabels && ShowRadiusAxis)
        {
            DrawRadiusLabels(context, center, innerRadius, outerRadius);
        }
    }

    /// <summary>
    /// Convert polar coordinates (angle in degrees, radius) to pixel position.
    /// </summary>
    public Point PolarToPixel(double angleDeg, double radius, Rect plotArea)
    {
        var center = new Point(plotArea.Center.X, plotArea.Center.Y);
        var outerRadius = Math.Min(plotArea.Width, plotArea.Height) / 2 - 10;
        var innerRadius = outerRadius * InnerRadius;

        var normalizedRadius = radius / MaxRadius;
        var pixelRadius = innerRadius + normalizedRadius * (outerRadius - innerRadius);

        var direction = Clockwise ? 1 : -1;
        var adjustedAngle = (StartAngle + angleDeg * direction) * Math.PI / 180;

        return new Point(
            center.X + pixelRadius * Math.Cos(adjustedAngle),
            center.Y + pixelRadius * Math.Sin(adjustedAngle));
    }

    /// <summary>
    /// Convert pixel position back to polar coordinates (angle in degrees, radius).
    /// </summary>
    public (double angleDeg, double radius) PixelToPolar(Point pixel, Rect plotArea)
    {
        var center = new Point(plotArea.Center.X, plotArea.Center.Y);
        var outerRadius = Math.Min(plotArea.Width, plotArea.Height) / 2 - 10;
        var innerRadius = outerRadius * InnerRadius;

        var dx = pixel.X - center.X;
        var dy = pixel.Y - center.Y;
        var pixelRadius = Math.Sqrt(dx * dx + dy * dy);

        var angleRad = Math.Atan2(dy, dx);
        var angleDeg = angleRad * 180 / Math.PI;

        // Adjust for start angle and direction
        var direction = Clockwise ? 1 : -1;
        angleDeg = (angleDeg - StartAngle * direction) / direction;

        // Normalize to 0-360
        while (angleDeg < 0) angleDeg += 360;
        while (angleDeg >= 360) angleDeg -= 360;

        // Convert pixel radius to data radius
        var normalizedRadius = (pixelRadius - innerRadius) / (outerRadius - innerRadius);
        var dataRadius = normalizedRadius * MaxRadius;

        return (angleDeg, Math.Max(0, dataRadius));
    }

    private void DrawAngleLabels(DrawingContext context, Point center, double radius)
    {
        var labelRadius = radius + 12;

        for (int i = 0; i < AngleDivisions; i++)
        {
            var angle = StartAngle + (360.0 / AngleDivisions) * i;
            var direction = Clockwise ? 1 : -1;
            var rad = angle * Math.PI / 180 * direction;

            var labelPos = new Point(
                center.X + labelRadius * Math.Cos(rad),
                center.Y + labelRadius * Math.Sin(rad));

            var labelText = $"{angle:F0}°";
            var formattedText = new FormattedText(labelText,
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface("Segoe UI", FontStyle.Normal, FontWeight.Normal),
                LabelFontSize,
                Brushes.Gray);

            context.DrawText(formattedText,
                new Point(labelPos.X - formattedText.Width / 2, labelPos.Y - formattedText.Height / 2));
        }
    }

    private void DrawRadiusLabels(DrawingContext context, Point center, double innerRadius, double outerRadius)
    {
        for (int level = 1; level <= GridLevels; level++)
        {
            var radius = innerRadius + (outerRadius - innerRadius) * level / GridLevels;
            var dataValue = MaxRadius * level / GridLevels;

            var labelText = dataValue.ToString("F0");
            var formattedText = new FormattedText(labelText,
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface("Segoe UI", FontStyle.Normal, FontWeight.Normal),
                LabelFontSize - 1,
                Brushes.LightGray);

            // Place label at 45 degrees
            var labelAngle = 45 * Math.PI / 180;
            var labelPos = new Point(
                center.X + radius * Math.Cos(labelAngle),
                center.Y + radius * Math.Sin(labelAngle));

            context.DrawText(formattedText,
                new Point(labelPos.X + 2, labelPos.Y - formattedText.Height));
        }
    }
}
