using System.Globalization;
using Avalonia;
using Avalonia.Controls;

namespace AuraUI.Controls.Layout;

/// <summary>
/// Specifies how the aspect ratio content fits within available space.
/// </summary>
public enum AspectRatioFitMode
{
    Width,
    Height,
    Contain
}

/// <summary>
/// A layout decorator that maintains a specific aspect ratio of its content.
/// Useful for images, videos, and cards that need to maintain proportions.
///
/// Supports size classes: .sm, .md, .lg
/// </summary>
public class AspectRatio : ContentControl
{
    private const double DefaultWidth = 320d;

    /// <summary>
    /// Defines the <see cref="Ratio"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> RatioProperty =
        AvaloniaProperty.Register<AspectRatio, double>(nameof(Ratio), 16d / 9d);

    /// <summary>
    /// Defines the <see cref="FitMode"/> styled property.
    /// </summary>
    public static readonly StyledProperty<AspectRatioFitMode> FitModeProperty =
        AvaloniaProperty.Register<AspectRatio, AspectRatioFitMode>(nameof(FitMode), AspectRatioFitMode.Width);

    /// <summary>
    /// Defines the <see cref="HasContent"/> readonly property.
    /// </summary>
    public static readonly StyledProperty<bool> HasContentProperty =
        AvaloniaProperty.Register<AspectRatio, bool>(nameof(HasContent));

    /// <summary>
    /// Defines the <see cref="RatioText"/> readonly property.
    /// </summary>
    public static readonly StyledProperty<string> RatioTextProperty =
        AvaloniaProperty.Register<AspectRatio, string>(nameof(RatioText), "16:9");

    static AspectRatio()
    {
        RatioProperty.Changed.AddClassHandler<AspectRatio>((x, args) => x.OnRatioChanged(args));
        FitModeProperty.Changed.AddClassHandler<AspectRatio>((x, _) =>
        {
            x.SyncClasses();
            x.InvalidateMeasure();
        });
        ContentProperty.Changed.AddClassHandler<AspectRatio>((x, _) => x.SyncClasses());
        AffectsMeasure<AspectRatio>(RatioProperty, FitModeProperty, PaddingProperty, BorderThicknessProperty);
    }

    public AspectRatio()
    {
        ClipToBounds = true;
        SyncClasses();
    }

    /// <summary>
    /// Gets or sets the aspect ratio (width / height). Default is 16/9.
    /// </summary>
    public double Ratio
    {
        get => GetValue(RatioProperty);
        set => SetValue(RatioProperty, NormalizeRatio(value));
    }

    /// <summary>
    /// Gets or sets how content is fit within the ratio.
    /// </summary>
    public AspectRatioFitMode FitMode
    {
        get => GetValue(FitModeProperty);
        set => SetValue(FitModeProperty, value);
    }

    /// <summary>
    /// Gets whether content is present.
    /// </summary>
    public bool HasContent => GetValue(HasContentProperty);

    /// <summary>
    /// Gets a human-readable ratio string (e.g., "16:9", "4:3", "1:1").
    /// </summary>
    public string RatioText => GetValue(RatioTextProperty);

    /// <summary>
    /// Calculates the desired size for a given ratio and available space.
    /// </summary>
    public static Size CalculateRatioSize(
        double ratio,
        AspectRatioFitMode fitMode,
        Size availableSize,
        double explicitWidth = double.NaN,
        double explicitHeight = double.NaN)
    {
        ratio = NormalizeRatio(ratio);

        var width = PickSize(explicitWidth, availableSize.Width);
        var height = PickSize(explicitHeight, availableSize.Height);

        if (fitMode == AspectRatioFitMode.Contain && width.HasValue && height.HasValue)
        {
            var candidateHeight = width.Value / ratio;
            if (candidateHeight <= height.Value)
            {
                return new Size(width.Value, candidateHeight);
            }
            return new Size(height.Value * ratio, height.Value);
        }

        if (fitMode == AspectRatioFitMode.Height && height.HasValue)
        {
            return new Size(height.Value * ratio, height.Value);
        }

        if (width.HasValue)
        {
            return new Size(width.Value, width.Value / ratio);
        }

        if (height.HasValue)
        {
            return new Size(height.Value * ratio, height.Value);
        }

        return new Size(DefaultWidth, DefaultWidth / ratio);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        var ratioSize = CalculateRatioSize(Ratio, FitMode, availableSize, Width, Height);
        base.MeasureOverride(ratioSize);
        return ratioSize;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        var ratioSize = CalculateRatioSize(Ratio, FitMode, finalSize, Width, Height);
        base.ArrangeOverride(ratioSize);
        return ratioSize;
    }

    private void OnRatioChanged(AvaloniaPropertyChangedEventArgs args)
    {
        var normalized = NormalizeRatio(Ratio);
        if (!IsClose(Ratio, normalized))
        {
            SetCurrentValue(RatioProperty, normalized);
            return;
        }

        var ratioText = FormatRatio(normalized);
        SetValue(RatioTextProperty, ratioText);
        SyncClasses();
        InvalidateMeasure();
    }

    private void SyncClasses()
    {
        var ratio = NormalizeRatio(Ratio);

        SetValue(HasContentProperty, HasValue(Content));
        SetValue(RatioTextProperty, FormatRatio(ratio));

        Classes.Set("aspect-ratio", true);
        Classes.Set("has-content", HasContent);
        Classes.Set("empty", !HasContent);
        Classes.Set("ratio-square", IsClose(ratio, 1d));
        Classes.Set("ratio-video", IsClose(ratio, 16d / 9d));
        Classes.Set("ratio-portrait", ratio < 1d && !IsClose(ratio, 1d));
        Classes.Set("ratio-landscape", ratio > 1d && !IsClose(ratio, 16d / 9d));
        Classes.Set("fit-width", FitMode == AspectRatioFitMode.Width);
        Classes.Set("fit-height", FitMode == AspectRatioFitMode.Height);
        Classes.Set("fit-contain", FitMode == AspectRatioFitMode.Contain);
    }

    private static double? PickSize(double explicitSize, double availableSize)
    {
        var explicitValue = CoerceSize(explicitSize);
        var availableValue = CoerceSize(availableSize);

        return explicitValue.HasValue && availableValue.HasValue
            ? Math.Min(explicitValue.Value, availableValue.Value)
            : explicitValue ?? availableValue;
    }

    private static double? CoerceSize(double value)
    {
        if (double.IsNaN(value) || double.IsInfinity(value))
        {
            return null;
        }
        return Math.Max(0d, value);
    }

    private static double NormalizeRatio(double ratio)
    {
        return double.IsNaN(ratio) || double.IsInfinity(ratio) || ratio <= 0d
            ? 1d
            : ratio;
    }

    private static string FormatRatio(double ratio)
    {
        if (IsClose(ratio, 16d / 9d)) return "16:9";
        if (IsClose(ratio, 9d / 16d)) return "9:16";
        if (IsClose(ratio, 4d / 3d)) return "4:3";
        if (IsClose(ratio, 1d)) return "1:1";
        return ratio.ToString("0.###", CultureInfo.InvariantCulture);
    }

    private static bool HasValue(object? value)
    {
        return value is string text ? !string.IsNullOrWhiteSpace(text) : value is not null;
    }

    private static bool IsClose(double left, double right)
    {
        return Math.Abs(left - right) < 0.001d;
    }
}
