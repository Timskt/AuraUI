using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace AuraUI.Controls.Layout;

/// <summary>
/// A custom-styled scrollbar wrapper, inspired by Element Plus's ElScrollbar.
/// Wraps any content in a scrollable container with custom scroll bar styling
/// and additional features like always-visible thumb and native scrolling option.
/// </summary>
public class AuraScrollbar : ContentControl
{
    /// <summary>
    /// Defines the <see cref="AlwaysShowThumb"/> styled property.
    /// When true, the scrollbar thumb is always visible (not just on hover/scroll).
    /// </summary>
    public static readonly StyledProperty<bool> AlwaysShowThumbProperty =
        AvaloniaProperty.Register<AuraScrollbar, bool>(nameof(AlwaysShowThumb));

    /// <summary>
    /// Defines the <see cref="ThumbMinLength"/> styled property.
    /// The minimum length of the scrollbar thumb in pixels.
    /// </summary>
    public static readonly StyledProperty<double> ThumbMinLengthProperty =
        AvaloniaProperty.Register<AuraScrollbar, double>(nameof(ThumbMinLength), 40);

    /// <summary>
    /// Defines the <see cref="ThumbThickness"/> styled property.
    /// The thickness (width/height) of the scrollbar thumb.
    /// </summary>
    public static readonly StyledProperty<double> ThumbThicknessProperty =
        AvaloniaProperty.Register<AuraScrollbar, double>(nameof(ThumbThickness), 6);

    /// <summary>
    /// Defines the <see cref="NativeScrolling"/> styled property.
    /// When true, uses native OS scrolling behavior.
    /// </summary>
    public static readonly StyledProperty<bool> NativeScrollingProperty =
        AvaloniaProperty.Register<AuraScrollbar, bool>(nameof(NativeScrolling));

    /// <summary>
    /// Defines the <see cref="HorizontalScrollBarVisibility"/> styled property.
    /// </summary>
    public static readonly StyledProperty<ScrollBarVisibility> HorizontalScrollBarVisibilityProperty =
        AvaloniaProperty.Register<AuraScrollbar, ScrollBarVisibility>(nameof(HorizontalScrollBarVisibility), ScrollBarVisibility.Disabled);

    /// <summary>
    /// Defines the <see cref="VerticalScrollBarVisibility"/> styled property.
    /// </summary>
    public static readonly StyledProperty<ScrollBarVisibility> VerticalScrollBarVisibilityProperty =
        AvaloniaProperty.Register<AuraScrollbar, ScrollBarVisibility>(nameof(VerticalScrollBarVisibility), ScrollBarVisibility.Auto);

    /// <summary>
    /// Gets or sets whether the thumb is always visible.
    /// </summary>
    public bool AlwaysShowThumb
    {
        get => GetValue(AlwaysShowThumbProperty);
        set => SetValue(AlwaysShowThumbProperty, value);
    }

    /// <summary>
    /// Gets or sets the minimum thumb length.
    /// </summary>
    public double ThumbMinLength
    {
        get => GetValue(ThumbMinLengthProperty);
        set => SetValue(ThumbMinLengthProperty, value);
    }

    /// <summary>
    /// Gets or sets the thumb thickness.
    /// </summary>
    public double ThumbThickness
    {
        get => GetValue(ThumbThicknessProperty);
        set => SetValue(ThumbThicknessProperty, value);
    }

    /// <summary>
    /// Gets or sets whether to use native scrolling.
    /// </summary>
    public bool NativeScrolling
    {
        get => GetValue(NativeScrollingProperty);
        set => SetValue(NativeScrollingProperty, value);
    }

    /// <summary>
    /// Gets or sets the horizontal scrollbar visibility.
    /// </summary>
    public ScrollBarVisibility HorizontalScrollBarVisibility
    {
        get => GetValue(HorizontalScrollBarVisibilityProperty);
        set => SetValue(HorizontalScrollBarVisibilityProperty, value);
    }

    /// <summary>
    /// Gets or sets the vertical scrollbar visibility.
    /// </summary>
    public ScrollBarVisibility VerticalScrollBarVisibility
    {
        get => GetValue(VerticalScrollBarVisibilityProperty);
        set => SetValue(VerticalScrollBarVisibilityProperty, value);
    }
}
