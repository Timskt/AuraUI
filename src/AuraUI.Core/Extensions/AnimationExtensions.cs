using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Media;

namespace AuraUI.Core.Extensions;

/// <summary>
/// Extension methods for animations and transitions.
/// </summary>
public static class AnimationExtensions
{
    /// <summary>
    /// Add a brush transition to the control.
    /// </summary>
    public static T WithBrushTransition<T>(this T control, params AvaloniaProperty[] properties) where T : Control
    {
        var transitions = control.Transitions ?? new Transitions();
        foreach (var prop in properties)
        {
            transitions.Add(new BrushTransition
            {
                Property = prop,
                Duration = TimeSpan.FromMilliseconds(200),
                Easing = new CubicEaseOut()
            });
        }
        control.Transitions = transitions;
        return control;
    }

    /// <summary>
    /// Add a double transition to the control.
    /// </summary>
    public static T WithDoubleTransition<T>(this T control, params AvaloniaProperty[] properties) where T : Control
    {
        var transitions = control.Transitions ?? new Transitions();
        foreach (var prop in properties)
        {
            transitions.Add(new DoubleTransition
            {
                Property = prop,
                Duration = TimeSpan.FromMilliseconds(200),
                Easing = new CubicEaseOut()
            });
        }
        control.Transitions = transitions;
        return control;
    }

    /// <summary>
    /// Add a corner radius transition to the control.
    /// </summary>
    public static T WithCornerRadiusTransition<T>(this T control) where T : Control
    {
        var transitions = control.Transitions ?? new Transitions();
        transitions.Add(new CornerRadiusTransition
        {
            Property = Border.CornerRadiusProperty,
            Duration = TimeSpan.FromMilliseconds(200),
            Easing = new CubicEaseOut()
        });
        control.Transitions = transitions;
        return control;
    }

    /// <summary>
    /// Add a thickness transition to the control.
    /// </summary>
    public static T WithThicknessTransition<T>(this T control, params AvaloniaProperty[] properties) where T : Control
    {
        var transitions = control.Transitions ?? new Transitions();
        foreach (var prop in properties)
        {
            transitions.Add(new ThicknessTransition
            {
                Property = prop,
                Duration = TimeSpan.FromMilliseconds(200),
                Easing = new CubicEaseOut()
            });
        }
        control.Transitions = transitions;
        return control;
    }

    /// <summary>
    /// Add a box shadow transition to the control.
    /// </summary>
    public static T WithBoxShadowTransition<T>(this T control) where T : Control
    {
        var transitions = control.Transitions ?? new Transitions();
        transitions.Add(new BoxShadowsTransition
        {
            Property = Border.BoxShadowProperty,
            Duration = TimeSpan.FromMilliseconds(200),
            Easing = new CubicEaseOut()
        });
        control.Transitions = transitions;
        return control;
    }

    /// <summary>
    /// Add common transitions (brush, double, corner radius) to a control.
    /// </summary>
    public static T WithStandardTransitions<T>(this T control) where T : Control
    {
        return control
            .WithBrushTransition(Border.BackgroundProperty, Border.BorderBrushProperty)
            .WithDoubleTransition(OpacityProperty)
            .WithCornerRadiusTransition();
    }

    private static readonly AvaloniaProperty OpacityProperty = Visual.OpacityProperty;
}
