using Avalonia;
using Avalonia.Controls;

namespace AuraUI.Controls.Layout;

/// <summary>
/// A column in the 24-column grid system, inspired by Ant Design's Col.
/// Use within an <see cref="AuraRow"/> to create responsive grid layouts.
/// The total span of all columns in a row should not exceed 24.
/// </summary>
public class AuraCol : ContentControl
{
    /// <summary>
    /// Defines the <see cref="Span"/> styled property.
    /// The number of columns (out of 24) this column occupies.
    /// </summary>
    public static readonly StyledProperty<int> SpanProperty =
        AvaloniaProperty.Register<AuraCol, int>(nameof(Span), 24);

    /// <summary>
    /// Defines the <see cref="Offset"/> styled property.
    /// The number of columns to offset this column from the left.
    /// </summary>
    public static readonly StyledProperty<int> OffsetProperty =
        AvaloniaProperty.Register<AuraCol, int>(nameof(Offset));

    /// <summary>
    /// Defines the <see cref="Push"/> styled property.
    /// Pushes the column to the right by the specified number of columns.
    /// </summary>
    public static readonly StyledProperty<int> PushProperty =
        AvaloniaProperty.Register<AuraCol, int>(nameof(Push));

    /// <summary>
    /// Defines the <see cref="Pull"/> styled property.
    /// Pulls the column to the left by the specified number of columns.
    /// </summary>
    public static readonly StyledProperty<int> PullProperty =
        AvaloniaProperty.Register<AuraCol, int>(nameof(Pull));

    /// <summary>
    /// Defines the <see cref="ResponsiveSpan"/> styled property.
    /// Span at a specific breakpoint width.
    /// </summary>
    public static readonly StyledProperty<int> ResponsiveSpanProperty =
        AvaloniaProperty.Register<AuraCol, int>(nameof(ResponsiveSpan));

    /// <summary>
    /// Defines the <see cref="ResponsiveOffset"/> styled property.
    /// Offset at a specific breakpoint width.
    /// </summary>
    public static readonly StyledProperty<int> ResponsiveOffsetProperty =
        AvaloniaProperty.Register<AuraCol, int>(nameof(ResponsiveOffset));

    static AuraCol()
    {
        SpanProperty.Changed.AddClassHandler<AuraCol>((x, _) => x.UpdateWidth());
        OffsetProperty.Changed.AddClassHandler<AuraCol>((x, _) => x.UpdateWidth());
    }

    /// <summary>
    /// Gets or sets the number of columns (out of 24) this column occupies.
    /// </summary>
    public int Span
    {
        get => GetValue(SpanProperty);
        set => SetValue(SpanProperty, value);
    }

    /// <summary>
    /// Gets or sets the number of empty columns to the left of this column.
    /// </summary>
    public int Offset
    {
        get => GetValue(OffsetProperty);
        set => SetValue(OffsetProperty, value);
    }

    /// <summary>
    /// Gets or sets the number of columns to push this column to the right.
    /// </summary>
    public int Push
    {
        get => GetValue(PushProperty);
        set => SetValue(PushProperty, value);
    }

    /// <summary>
    /// Gets or sets the number of columns to pull this column to the left.
    /// </summary>
    public int Pull
    {
        get => GetValue(PullProperty);
        set => SetValue(PullProperty, value);
    }

    /// <summary>
    /// Gets or sets the span at a responsive breakpoint.
    /// </summary>
    public int ResponsiveSpan
    {
        get => GetValue(ResponsiveSpanProperty);
        set => SetValue(ResponsiveSpanProperty, value);
    }

    /// <summary>
    /// Gets or sets the offset at a responsive breakpoint.
    /// </summary>
    public int ResponsiveOffset
    {
        get => GetValue(ResponsiveOffsetProperty);
        set => SetValue(ResponsiveOffsetProperty, value);
    }

    private void UpdateWidth()
    {
        var span = Math.Clamp(Span, 0, 24);
        var offset = Math.Clamp(Offset, 0, 24);

        // Calculate percentage width
        var widthPercent = (span / 24.0) * 100;
        var offsetPercent = (offset / 24.0) * 100;

        // Use MaxWidth as a percentage hint (will be set in theme via converter or binding)
        // For simplicity, we store the ratio and let the theme handle it
        Tag = new AuraColLayout(span, offset, Push, Pull);
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        UpdateWidth();
    }
}

/// <summary>
/// Internal record storing the computed layout for a grid column.
/// </summary>
internal record AuraColLayout(int Span, int Offset, int Push, int Pull);
