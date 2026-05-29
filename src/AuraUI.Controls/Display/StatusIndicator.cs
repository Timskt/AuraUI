using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Media;

namespace AuraUI.Controls.Display;

/// <summary>
/// Specifies the status of a service or resource.
/// </summary>
public enum ServiceStatus
{
    Online,
    Offline,
    Warning,
    Error,
    Maintenance
}

/// <summary>
/// A colored status dot with optional pulse animation and label text.
/// Used to display the status of services, servers, or resources.
/// </summary>
[PseudoClasses(":online", ":offline", ":warning", ":error", ":maintenance", ":pulsing")]
public class StatusIndicator : Control
{
    /// <summary>
    /// Defines the <see cref="Status"/> styled property.
    /// </summary>
    public static readonly StyledProperty<ServiceStatus> StatusProperty =
        AvaloniaProperty.Register<StatusIndicator, ServiceStatus>(nameof(Status), ServiceStatus.Online);

    /// <summary>
    /// Defines the <see cref="IndicatorSize"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> IndicatorSizeProperty =
        AvaloniaProperty.Register<StatusIndicator, double>(nameof(IndicatorSize), 10);

    /// <summary>
    /// Defines the <see cref="ShowLabel"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> ShowLabelProperty =
        AvaloniaProperty.Register<StatusIndicator, bool>(nameof(ShowLabel));

    /// <summary>
    /// Defines the <see cref="Label"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> LabelProperty =
        AvaloniaProperty.Register<StatusIndicator, string?>(nameof(Label));

    /// <summary>
    /// Defines the <see cref="PulseAnimation"/> styled property.
    /// Whether the dot should pulse (animate opacity) when in certain states.
    /// </summary>
    public static readonly StyledProperty<bool> PulseAnimationProperty =
        AvaloniaProperty.Register<StatusIndicator, bool>(nameof(PulseAnimation));

    static StatusIndicator()
    {
        StatusProperty.Changed.AddClassHandler<StatusIndicator>((x, _) => x.UpdatePseudoClasses());
        PulseAnimationProperty.Changed.AddClassHandler<StatusIndicator>((x, _) => x.UpdatePseudoClasses());
    }

    /// <summary>
    /// Gets or sets the status to display.
    /// </summary>
    public ServiceStatus Status
    {
        get => GetValue(StatusProperty);
        set => SetValue(StatusProperty, value);
    }

    /// <summary>
    /// Gets or sets the diameter of the status dot in pixels.
    /// </summary>
    public double IndicatorSize
    {
        get => GetValue(IndicatorSizeProperty);
        set => SetValue(IndicatorSizeProperty, value);
    }

    /// <summary>
    /// Gets or sets whether to display a text label alongside the dot.
    /// </summary>
    public bool ShowLabel
    {
        get => GetValue(ShowLabelProperty);
        set => SetValue(ShowLabelProperty, value);
    }

    /// <summary>
    /// Gets or sets the label text. Defaults to the status name when null.
    /// </summary>
    public string? Label
    {
        get => GetValue(LabelProperty);
        set => SetValue(LabelProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the dot should pulse for active statuses (Online, Warning, Error).
    /// </summary>
    public bool PulseAnimation
    {
        get => GetValue(PulseAnimationProperty);
        set => SetValue(PulseAnimationProperty, value);
    }

    /// <summary>
    /// Gets the display label, falling back to the status name.
    /// </summary>
    public string DisplayLabel => Label ?? Status.ToString();

    // Cached brushes to avoid per-access allocations
    private static readonly SolidColorBrush s_onlineBrush = new(Color.Parse("#4CAF50"));
    private static readonly SolidColorBrush s_offlineBrush = new(Color.Parse("#9E9E9E"));
    private static readonly SolidColorBrush s_warningBrush = new(Color.Parse("#FF9800"));
    private static readonly SolidColorBrush s_errorBrush = new(Color.Parse("#F44336"));
    private static readonly SolidColorBrush s_maintenanceBrush = new(Color.Parse("#2196F3"));

    /// <summary>
    /// Gets the color brush associated with the current status.
    /// </summary>
    public IBrush StatusBrush => Status switch
    {
        ServiceStatus.Online => s_onlineBrush,
        ServiceStatus.Offline => s_offlineBrush,
        ServiceStatus.Warning => s_warningBrush,
        ServiceStatus.Error => s_errorBrush,
        ServiceStatus.Maintenance => s_maintenanceBrush,
        _ => Brushes.Gray
    };

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        UpdatePseudoClasses();
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(":online", Status == ServiceStatus.Online);
        PseudoClasses.Set(":offline", Status == ServiceStatus.Offline);
        PseudoClasses.Set(":warning", Status == ServiceStatus.Warning);
        PseudoClasses.Set(":error", Status == ServiceStatus.Error);
        PseudoClasses.Set(":maintenance", Status == ServiceStatus.Maintenance);

        var shouldPulse = PulseAnimation && (Status == ServiceStatus.Online || Status == ServiceStatus.Warning || Status == ServiceStatus.Error);
        PseudoClasses.Set(":pulsing", shouldPulse);

        _cachedFormattedText = null; // invalidate text cache
    }

    // Cached FormattedText to avoid per-render allocations
    private FormattedText? _cachedFormattedText;
    private string? _cachedLabelText;
    private IBrush? _cachedTextBrush;
    private static readonly Typeface s_labelTypeface = new("Segoe UI");

    private FormattedText? GetOrCreateFormattedText()
    {
        if (!ShowLabel) return null;

        var text = DisplayLabel;
        if (string.IsNullOrEmpty(text)) return null;

        var textBrush = Foreground ?? Brushes.Black;
        if (_cachedFormattedText != null && _cachedLabelText == text && _cachedTextBrush == textBrush)
            return _cachedFormattedText;

        _cachedLabelText = text;
        _cachedTextBrush = textBrush;
        _cachedFormattedText = new FormattedText(
            text,
            System.Globalization.CultureInfo.CurrentCulture,
            FlowDirection.LeftToRight,
            s_labelTypeface,
            12,
            textBrush);
        return _cachedFormattedText;
    }

    public override void Render(DrawingContext context)
    {
        var bounds = new Rect(Bounds.Size);
        if (bounds.Width <= 0 || bounds.Height <= 0) return;

        var size = IndicatorSize;
        var center = new Point(size / 2, bounds.Height / 2);

        // Draw the status dot
        context.DrawEllipse(StatusBrush, null, center, size / 2, size / 2);

        // Draw label text if enabled
        var formattedText = GetOrCreateFormattedText();
        if (formattedText != null)
        {
            var textX = size + 6;
            var textY = (bounds.Height - formattedText.Height) / 2;
            context.DrawText(formattedText, new Point(textX, textY));
        }
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        var size = IndicatorSize;

        if (!ShowLabel)
        {
            return new Size(size, size);
        }

        var formattedText = GetOrCreateFormattedText();
        if (formattedText != null)
        {
            return new Size(size + 6 + formattedText.Width, Math.Max(size, formattedText.Height));
        }

        return new Size(size, size);
    }

    /// <summary>
    /// The Foreground property for label text color.
    /// </summary>
    public static readonly StyledProperty<IBrush?> ForegroundProperty =
        AvaloniaProperty.Register<StatusIndicator, IBrush?>(nameof(Foreground));

    /// <summary>
    /// Gets or sets the foreground brush for label text.
    /// </summary>
    public IBrush? Foreground
    {
        get => GetValue(ForegroundProperty);
        set => SetValue(ForegroundProperty, value);
    }
}
