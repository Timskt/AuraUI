using System;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Automation;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Threading;

namespace AuraUI.Controls.Selection;

/// <summary>
/// A toggle switch control with smooth thumb transition animation.
/// Supports small, medium, and large size variants.
/// </summary>
/// <remarks>
/// <para>
/// The switch renders as a track with a sliding thumb. When toggled, the thumb
/// translates smoothly using a <see cref="DoubleTransition"/> on the translate transform.
/// </para>
/// <para>
/// Template parts:
/// <list type="bullet">
///   <item><c>PART_Track</c>: The track border that responds to clicks.</item>
///   <item><c>PART_Thumb</c>: The thumb border that slides between positions.</item>
/// </list>
/// </para>
/// </remarks>
[TemplatePart("PART_Track", typeof(Border))]
[TemplatePart("PART_Thumb", typeof(Border))]
[PseudoClasses(":checked", ":unchecked", ":small", ":medium", ":large", ":pointerover", ":pressed")]
public class Switch : ToggleButton
{
    private Border? _track;
    private Border? _thumb;
    private TranslateTransform? _thumbTranslate;

    /// <summary>
    /// Defines the <see cref="OnContent"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> OnContentProperty =
        AvaloniaProperty.Register<Switch, string?>(nameof(OnContent));

    /// <summary>
    /// Defines the <see cref="OffContent"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> OffContentProperty =
        AvaloniaProperty.Register<Switch, string?>(nameof(OffContent));

    /// <summary>
    /// Defines the <see cref="ThumbContent"/> styled property.
    /// </summary>
    public static readonly StyledProperty<object?> ThumbContentProperty =
        AvaloniaProperty.Register<Switch, object?>(nameof(ThumbContent));

    /// <summary>
    /// Defines the <see cref="SwitchSize"/> styled property.
    /// </summary>
    public static readonly StyledProperty<SwitchSize> SizeProperty =
        AvaloniaProperty.Register<Switch, SwitchSize>(
            nameof(Size),
            SwitchSize.Medium);

    /// <summary>
    /// Defines the <see cref="TransitionDuration"/> styled property.
    /// </summary>
    public static readonly StyledProperty<TimeSpan> TransitionDurationProperty =
        AvaloniaProperty.Register<Switch, TimeSpan>(
            nameof(TransitionDuration),
            TimeSpan.FromMilliseconds(200));

    /// <summary>
    /// Defines the <see cref="TransitionEasing"/> styled property.
    /// </summary>
    public static readonly StyledProperty<Easing?> TransitionEasingProperty =
        AvaloniaProperty.Register<Switch, Easing?>(
            nameof(TransitionEasing),
            new CubicEaseOut());

    static Switch()
    {
        IsCheckedProperty.Changed.AddClassHandler<Switch>((x, _) => x.OnIsCheckedChanged());
        SizeProperty.Changed.AddClassHandler<Switch>((x, _) => x.UpdatePseudoClasses());
    }

    /// <summary>
    /// Gets or sets the text displayed when the switch is on.
    /// </summary>
    public string? OnContent
    {
        get => GetValue(OnContentProperty);
        set => SetValue(OnContentProperty, value);
    }

    /// <summary>
    /// Gets or sets the text displayed when the switch is off.
    /// </summary>
    public string? OffContent
    {
        get => GetValue(OffContentProperty);
        set => SetValue(OffContentProperty, value);
    }

    /// <summary>
    /// Gets or sets custom content displayed inside the thumb.
    /// </summary>
    public object? ThumbContent
    {
        get => GetValue(ThumbContentProperty);
        set => SetValue(ThumbContentProperty, value);
    }

    /// <summary>
    /// Gets or sets the size variant of the switch.
    /// </summary>
    public SwitchSize Size
    {
        get => GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }

    /// <summary>
    /// Gets or sets the duration of the thumb transition animation.
    /// </summary>
    public TimeSpan TransitionDuration
    {
        get => GetValue(TransitionDurationProperty);
        set => SetValue(TransitionDurationProperty, value);
    }

    /// <summary>
    /// Gets or sets the easing function for the thumb transition.
    /// </summary>
    public Easing? TransitionEasing
    {
        get => GetValue(TransitionEasingProperty);
        set => SetValue(TransitionEasingProperty, value);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        if (_track != null)
        {
            _track.PointerEntered -= OnTrackPointerEntered;
            _track.PointerExited -= OnTrackPointerExited;
        }

        _track = e.NameScope.Find<Border>("PART_Track");
        _thumb = e.NameScope.Find<Border>("PART_Thumb");

        if (_track != null)
        {
            _track.PointerEntered += OnTrackPointerEntered;
            _track.PointerExited += OnTrackPointerExited;
        }

        // Set up the thumb translate transform with a transition for smooth animation.
        if (_thumb != null)
        {
            _thumbTranslate = new TranslateTransform();

            // Add a double transition so that changing TranslateTransform.X animates smoothly.
            _thumbTranslate.Transitions = new Avalonia.Animation.Transitions
            {
                new DoubleTransition
                {
                    Property = TranslateTransform.XProperty,
                    Duration = TransitionDuration,
                    Easing = TransitionEasing ?? new CubicEaseOut()
                }
            };

            _thumb.RenderTransform = _thumbTranslate;
        }

        UpdatePseudoClasses();
        UpdateThumbPosition(false);
        SetValue(AutomationProperties.NameProperty, "Toggle Switch");
    }

    protected override void OnPointerEntered(PointerEventArgs e)
    {
        base.OnPointerEntered(e);
        PseudoClasses.Set(":pointerover", true);
    }

    protected override void OnPointerExited(PointerEventArgs e)
    {
        base.OnPointerExited(e);
        PseudoClasses.Set(":pointerover", false);
        PseudoClasses.Set(":pressed", false);
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            PseudoClasses.Set(":pressed", true);
        }
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);
        PseudoClasses.Set(":pressed", false);
    }

    private void OnIsCheckedChanged()
    {
        UpdatePseudoClasses();
        UpdateThumbPosition(true);
    }

    private void UpdateThumbPosition(bool animate)
    {
        if (_thumbTranslate == null || _track == null || _thumb == null)
            return;

        // Calculate the target X based on track and thumb widths.
        var trackWidth = _track.Bounds.Width;
        var thumbWidth = _thumb.Bounds.Width;

        if (trackWidth <= 0 || thumbWidth <= 0)
        {
            // If layout hasn't occurred yet, defer.
            _thumb.LayoutUpdated += OnThumbLayoutUpdated;
            return;
        }

        var travelDistance = trackWidth - thumbWidth;

        if (!animate)
        {
            // Temporarily disable transitions for instant positioning (e.g., initial load).
            _thumbTranslate.Transitions = null;
        }

        _thumbTranslate.X = IsChecked == true ? travelDistance : 0;

        if (!animate)
        {
            // Re-enable transitions after instant positioning.
            Dispatcher.UIThread.Post(() =>
            {
                if (_thumbTranslate != null)
                {
                    _thumbTranslate.Transitions = new Avalonia.Animation.Transitions
                    {
                        new DoubleTransition
                        {
                            Property = TranslateTransform.XProperty,
                            Duration = TransitionDuration,
                            Easing = TransitionEasing ?? new CubicEaseOut()
                        }
                    };
                }
            }, DispatcherPriority.Render);
        }
    }

    private void OnThumbLayoutUpdated(object? sender, EventArgs e)
    {
        if (_thumb != null)
        {
            _thumb.LayoutUpdated -= OnThumbLayoutUpdated;
        }
        UpdateThumbPosition(false);
    }

    private void OnTrackPointerEntered(object? sender, PointerEventArgs e)
    {
        PseudoClasses.Set(":pointerover", true);
    }

    private void OnTrackPointerExited(object? sender, PointerEventArgs e)
    {
        PseudoClasses.Set(":pointerover", false);
    }

    private void UpdatePseudoClasses()
    {
        var isChecked = IsChecked == true;

        PseudoClasses.Set(":checked", isChecked);
        PseudoClasses.Set(":unchecked", !isChecked);

        PseudoClasses.Set(":small", Size == SwitchSize.Small);
        PseudoClasses.Set(":medium", Size == SwitchSize.Medium);
        PseudoClasses.Set(":large", Size == SwitchSize.Large);
    }
}
