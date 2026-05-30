using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;

namespace AuraUI.Core.Animation;

/// <summary>
/// Helper for adding CSS-like transitions to Avalonia controls.
/// Transitions allow property changes to animate smoothly without explicit animation code.
/// </summary>
/// <example>
/// <code>
/// // Add opacity and transform transitions
/// TransitionHelper.AddTransitions(myButton, OpacityProperty, ScaleTransform.ScaleXProperty);
///
/// // Add brush transitions for color changes
/// TransitionHelper.AddBrushTransitions(myBorder, Border.BackgroundProperty);
///
/// // Add all common transitions at once
/// TransitionHelper.AddAllTransitions(myCard);
/// </code>
/// </example>
public static class TransitionHelper
{
    private static readonly TimeSpan DefaultDuration = TimeSpan.FromMilliseconds(200);
    private static readonly Easing DefaultEasing = new CubicEaseOut();

    /// <summary>
    /// Default transition properties that respond to numeric changes (opacity, translate, scale, etc.).
    /// </summary>
    private static readonly AvaloniaProperty[] DefaultTransitionProperties =
    {
        Visual.OpacityProperty,
    };

    /// <summary>
    /// Default transition properties for transform-related values.
    /// </summary>
    private static readonly AvaloniaProperty[] TransformTransitionProperties =
    {
        TranslateTransform.XProperty,
        TranslateTransform.YProperty,
        ScaleTransform.ScaleXProperty,
        ScaleTransform.ScaleYProperty,
        RotateTransform.AngleProperty,
    };

    /// <summary>
    /// Adds transitions for the specified properties using default duration and easing.
    /// </summary>
    /// <param name="control">The control to add transitions to.</param>
    /// <param name="properties">The properties to animate.</param>
    public static void AddTransitions(Control control, params AvaloniaProperty[] properties)
    {
        AddTransitions(control, DefaultDuration, DefaultEasing, properties);
    }

    /// <summary>
    /// Adds transitions for the specified properties using custom duration and easing.
    /// </summary>
    /// <param name="control">The control to add transitions to.</param>
    /// <param name="duration">The transition duration.</param>
    /// <param name="easing">The easing function.</param>
    /// <param name="properties">The properties to animate.</param>
    public static void AddTransitions(Control control, TimeSpan duration, Easing easing, params AvaloniaProperty[] properties)
    {
        if (control is null) throw new ArgumentNullException(nameof(control));
        if (properties is null) throw new ArgumentNullException(nameof(properties));

        var transitions = control.Transitions ?? new Transitions();

        foreach (var property in properties)
        {
            // Avoid duplicate transitions for the same property
            if (HasTransitionFor(transitions, property))
                continue;

            transitions.Add(CreateTransition(property, duration, easing));
        }

        control.Transitions = transitions;
    }

    /// <summary>
    /// Adds brush-typed transitions for the specified properties.
    /// Brush transitions use <see cref="BrushTransition"/> for smooth color interpolation.
    /// </summary>
    /// <param name="control">The control to add brush transitions to.</param>
    /// <param name="properties">The brush-typed properties to animate.</param>
    public static void AddBrushTransitions(Control control, params AvaloniaProperty[] properties)
    {
        AddBrushTransitions(control, DefaultDuration, DefaultEasing, properties);
    }

    /// <summary>
    /// Adds brush-typed transitions with custom duration and easing.
    /// </summary>
    /// <param name="control">The control to add brush transitions to.</param>
    /// <param name="duration">The transition duration.</param>
    /// <param name="easing">The easing function.</param>
    /// <param name="properties">The brush-typed properties to animate.</param>
    public static void AddBrushTransitions(Control control, TimeSpan duration, Easing easing, params AvaloniaProperty[] properties)
    {
        if (control is null) throw new ArgumentNullException(nameof(control));
        if (properties is null) throw new ArgumentNullException(nameof(properties));

        var transitions = control.Transitions ?? new Transitions();

        foreach (var property in properties)
        {
            if (HasTransitionFor(transitions, property))
                continue;

            transitions.Add(new BrushTransition
            {
                Property = property,
                Duration = duration,
                Easing = easing
            });
        }

        control.Transitions = transitions;
    }

    /// <summary>
    /// Adds all commonly-used transitions to the control (opacity, background, foreground, border brush).
    /// This provides a quick way to enable smooth animations across the most commonly changed properties.
    /// </summary>
    /// <param name="control">The control to add transitions to.</param>
    public static void AddAllTransitions(Control control)
    {
        AddAllTransitions(control, DefaultDuration, DefaultEasing);
    }

    /// <summary>
    /// Adds all commonly-used transitions with custom duration and easing.
    /// </summary>
    /// <param name="control">The control to add transitions to.</param>
    /// <param name="duration">The transition duration.</param>
    /// <param name="easing">The easing function.</param>
    public static void AddAllTransitions(Control control, TimeSpan duration, Easing easing)
    {
        if (control is null) throw new ArgumentNullException(nameof(control));

        var transitions = control.Transitions ?? new Transitions();

        // Add numeric transitions (opacity)
        AddTransitionIfMissing(transitions, Visual.OpacityProperty, duration, easing);

        // Add brush transitions for templated controls
        if (control is TemplatedControl tc)
        {
            AddBrushTransitionIfMissing(transitions, TemplatedControl.BackgroundProperty, duration, easing);
            AddBrushTransitionIfMissing(transitions, TemplatedControl.ForegroundProperty, duration, easing);
            AddBrushTransitionIfMissing(transitions, TemplatedControl.BorderBrushProperty, duration, easing);
        }

        // Add brush transitions for border
        if (control is Border border)
        {
            AddBrushTransitionIfMissing(transitions, Border.BackgroundProperty, duration, easing);
            AddBrushTransitionIfMissing(transitions, Border.BorderBrushProperty, duration, easing);
        }

        control.Transitions = transitions;
    }

    /// <summary>
    /// Removes all transitions from the control.
    /// </summary>
    /// <param name="control">The control to clear transitions from.</param>
    public static void ClearTransitions(Control control)
    {
        if (control is null) throw new ArgumentNullException(nameof(control));
        control.Transitions?.Clear();
    }

    /// <summary>
    /// Creates a standard numeric transition for the given property.
    /// </summary>
    public static ITransition CreateTransition(AvaloniaProperty property, TimeSpan duration, Easing easing)
    {
        return new DoubleTransition
        {
            Property = property,
            Duration = duration,
            Easing = easing
        };
    }

    /// <summary>
    /// Creates a brush transition for the given property.
    /// </summary>
    public static ITransition CreateBrushTransition(AvaloniaProperty property, TimeSpan duration, Easing easing)
    {
        return new BrushTransition
        {
            Property = property,
            Duration = duration,
            Easing = easing
        };
    }

    private static bool HasTransitionFor(Transitions transitions, AvaloniaProperty property)
    {
        foreach (var t in transitions)
        {
            if (t is DoubleTransition dt && dt.Property == property)
                return true;
            if (t is BrushTransition bt && bt.Property == property)
                return true;
        }
        return false;
    }

    private static void AddTransitionIfMissing(Transitions transitions, AvaloniaProperty property, TimeSpan duration, Easing easing)
    {
        if (!HasTransitionFor(transitions, property))
        {
            transitions.Add(new DoubleTransition
            {
                Property = property,
                Duration = duration,
                Easing = easing
            });
        }
    }

    private static void AddBrushTransitionIfMissing(Transitions transitions, AvaloniaProperty property, TimeSpan duration, Easing easing)
    {
        if (!HasTransitionFor(transitions, property))
        {
            transitions.Add(new BrushTransition
            {
                Property = property,
                Duration = duration,
                Easing = easing
            });
        }
    }
}
