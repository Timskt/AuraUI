using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Media;
using Avalonia.Styling;

namespace AuraUI.Core.Animation;

/// <summary>
/// Factory methods that return pre-configured <see cref="Avalonia.Animation.Animation"/> instances
/// for common UI transitions (fade, slide, scale, bounce, shake).
/// </summary>
/// <example>
/// <code>
/// // Fade in a control
/// await AnimationPresets.FadeIn().RunAsync(myControl);
///
/// // Slide in from left with custom duration
/// await AnimationPresets.SlideInLeft(400).RunAsync(myPanel);
///
/// // Bounce-in effect
/// await AnimationPresets.BounceIn().RunAsync(myCard);
/// </code>
/// </example>
public static class AnimationPresets
{
    /// <summary>
    /// Creates a fade-in animation (opacity 0 -> 1).
    /// </summary>
    /// <param name="duration">Duration in milliseconds. Default is 300.</param>
    public static Avalonia.Animation.Animation FadeIn(double duration = 300)
    {
        return AnimationBuilder.Create()
            .Property(Visual.OpacityProperty)
            .From(0.0)
            .To(1.0)
            .Duration(duration)
            .Easing(new CubicEaseOut())
            .SetFillMode(Avalonia.Animation.FillMode.Forward)
            .Build();
    }

    /// <summary>
    /// Creates a fade-out animation (opacity 1 -> 0).
    /// </summary>
    /// <param name="duration">Duration in milliseconds. Default is 300.</param>
    public static Avalonia.Animation.Animation FadeOut(double duration = 300)
    {
        return AnimationBuilder.Create()
            .Property(Visual.OpacityProperty)
            .From(1.0)
            .To(0.0)
            .Duration(duration)
            .Easing(new CubicEaseIn())
            .SetFillMode(Avalonia.Animation.FillMode.Forward)
            .Build();
    }

    /// <summary>
    /// Creates a slide-in-from-left animation using TranslateTransform.
    /// The control starts off-screen to the left and slides to its natural position.
    /// </summary>
    /// <param name="duration">Duration in milliseconds. Default is 300.</param>
    public static Avalonia.Animation.Animation SlideInLeft(double duration = 300)
    {
        return AnimationBuilder.Create()
            .Property(TranslateTransform.XProperty)
            .From(-100.0)
            .To(0.0)
            .Duration(duration)
            .Easing(new CubicEaseOut())
            .SetFillMode(Avalonia.Animation.FillMode.Forward)
            .Build();
    }

    /// <summary>
    /// Creates a slide-in-from-right animation using TranslateTransform.
    /// The control starts off-screen to the right and slides to its natural position.
    /// </summary>
    /// <param name="duration">Duration in milliseconds. Default is 300.</param>
    public static Avalonia.Animation.Animation SlideInRight(double duration = 300)
    {
        return AnimationBuilder.Create()
            .Property(TranslateTransform.XProperty)
            .From(100.0)
            .To(0.0)
            .Duration(duration)
            .Easing(new CubicEaseOut())
            .SetFillMode(Avalonia.Animation.FillMode.Forward)
            .Build();
    }

    /// <summary>
    /// Creates a slide-in-from-bottom animation using TranslateTransform.
    /// The control starts below its final position and slides upward.
    /// </summary>
    /// <param name="duration">Duration in milliseconds. Default is 300.</param>
    public static Avalonia.Animation.Animation SlideInUp(double duration = 300)
    {
        return AnimationBuilder.Create()
            .Property(TranslateTransform.YProperty)
            .From(100.0)
            .To(0.0)
            .Duration(duration)
            .Easing(new CubicEaseOut())
            .SetFillMode(Avalonia.Animation.FillMode.Forward)
            .Build();
    }

    /// <summary>
    /// Creates a slide-in-from-top animation using TranslateTransform.
    /// The control starts above its final position and slides downward.
    /// </summary>
    /// <param name="duration">Duration in milliseconds. Default is 300.</param>
    public static Avalonia.Animation.Animation SlideInDown(double duration = 300)
    {
        return AnimationBuilder.Create()
            .Property(TranslateTransform.YProperty)
            .From(-100.0)
            .To(0.0)
            .Duration(duration)
            .Easing(new CubicEaseOut())
            .SetFillMode(Avalonia.Animation.FillMode.Forward)
            .Build();
    }

    /// <summary>
    /// Creates a scale-in animation (scale 0 -> 1 on both axes with fade-in).
    /// Requires the control to have a RenderTransform that includes ScaleTransform.
    /// </summary>
    /// <param name="duration">Duration in milliseconds. Default is 300.</param>
    public static Avalonia.Animation.Animation ScaleIn(double duration = 300)
    {
        var animation = new Avalonia.Animation.Animation
        {
            Duration = TimeSpan.FromMilliseconds(duration),
            Easing = new CubicEaseOut(),
            FillMode = Avalonia.Animation.FillMode.Forward,
            Children =
            {
                new KeyFrame
                {
                    Cue = new Cue(0.0),
                    Setters =
                    {
                        new Setter(ScaleTransform.ScaleXProperty, 0.0),
                        new Setter(ScaleTransform.ScaleYProperty, 0.0),
                        new Setter(Visual.OpacityProperty, 0.0),
                    }
                },
                new KeyFrame
                {
                    Cue = new Cue(1.0),
                    Setters =
                    {
                        new Setter(ScaleTransform.ScaleXProperty, 1.0),
                        new Setter(ScaleTransform.ScaleYProperty, 1.0),
                        new Setter(Visual.OpacityProperty, 1.0),
                    }
                }
            }
        };

        return animation;
    }

    /// <summary>
    /// Creates a scale-out animation (scale 1 -> 0 on both axes with fade-out).
    /// Requires the control to have a RenderTransform that includes ScaleTransform.
    /// </summary>
    /// <param name="duration">Duration in milliseconds. Default is 300.</param>
    public static Avalonia.Animation.Animation ScaleOut(double duration = 300)
    {
        var animation = new Avalonia.Animation.Animation
        {
            Duration = TimeSpan.FromMilliseconds(duration),
            Easing = new CubicEaseIn(),
            FillMode = Avalonia.Animation.FillMode.Forward,
            Children =
            {
                new KeyFrame
                {
                    Cue = new Cue(0.0),
                    Setters =
                    {
                        new Setter(ScaleTransform.ScaleXProperty, 1.0),
                        new Setter(ScaleTransform.ScaleYProperty, 1.0),
                        new Setter(Visual.OpacityProperty, 1.0),
                    }
                },
                new KeyFrame
                {
                    Cue = new Cue(1.0),
                    Setters =
                    {
                        new Setter(ScaleTransform.ScaleXProperty, 0.0),
                        new Setter(ScaleTransform.ScaleYProperty, 0.0),
                        new Setter(Visual.OpacityProperty, 0.0),
                    }
                }
            }
        };

        return animation;
    }

    /// <summary>
    /// Creates a bounce-in animation with overshoot easing.
    /// Scales from 0 to 1 with a slight overshoot effect.
    /// </summary>
    /// <param name="duration">Duration in milliseconds. Default is 500.</param>
    public static Avalonia.Animation.Animation BounceIn(double duration = 500)
    {
        var animation = new Avalonia.Animation.Animation
        {
            Duration = TimeSpan.FromMilliseconds(duration),
            Easing = new BackEaseOut(),
            FillMode = Avalonia.Animation.FillMode.Forward,
            Children =
            {
                new KeyFrame
                {
                    Cue = new Cue(0.0),
                    Setters =
                    {
                        new Setter(ScaleTransform.ScaleXProperty, 0.0),
                        new Setter(ScaleTransform.ScaleYProperty, 0.0),
                        new Setter(Visual.OpacityProperty, 0.0),
                    }
                },
                new KeyFrame
                {
                    Cue = new Cue(1.0),
                    Setters =
                    {
                        new Setter(ScaleTransform.ScaleXProperty, 1.0),
                        new Setter(ScaleTransform.ScaleYProperty, 1.0),
                        new Setter(Visual.OpacityProperty, 1.0),
                    }
                }
            }
        };

        return animation;
    }

    /// <summary>
    /// Creates a shake animation that oscillates the control horizontally.
    /// Translates the control left and right, then returns to center.
    /// </summary>
    /// <param name="duration">Duration in milliseconds. Default is 500.</param>
    public static Avalonia.Animation.Animation Shake(double duration = 500)
    {
        var animation = new Avalonia.Animation.Animation
        {
            Duration = TimeSpan.FromMilliseconds(duration),
            Easing = new LinearEasing(),
            FillMode = Avalonia.Animation.FillMode.Forward,
            Children =
            {
                new KeyFrame
                {
                    Cue = new Cue(0.0),
                    Setters = { new Setter(TranslateTransform.XProperty, 0.0) }
                },
                new KeyFrame
                {
                    Cue = new Cue(0.1),
                    Setters = { new Setter(TranslateTransform.XProperty, -10.0) }
                },
                new KeyFrame
                {
                    Cue = new Cue(0.2),
                    Setters = { new Setter(TranslateTransform.XProperty, 10.0) }
                },
                new KeyFrame
                {
                    Cue = new Cue(0.3),
                    Setters = { new Setter(TranslateTransform.XProperty, -10.0) }
                },
                new KeyFrame
                {
                    Cue = new Cue(0.4),
                    Setters = { new Setter(TranslateTransform.XProperty, 10.0) }
                },
                new KeyFrame
                {
                    Cue = new Cue(0.5),
                    Setters = { new Setter(TranslateTransform.XProperty, -5.0) }
                },
                new KeyFrame
                {
                    Cue = new Cue(0.6),
                    Setters = { new Setter(TranslateTransform.XProperty, 5.0) }
                },
                new KeyFrame
                {
                    Cue = new Cue(0.7),
                    Setters = { new Setter(TranslateTransform.XProperty, -3.0) }
                },
                new KeyFrame
                {
                    Cue = new Cue(0.8),
                    Setters = { new Setter(TranslateTransform.XProperty, 3.0) }
                },
                new KeyFrame
                {
                    Cue = new Cue(0.9),
                    Setters = { new Setter(TranslateTransform.XProperty, -1.0) }
                },
                new KeyFrame
                {
                    Cue = new Cue(1.0),
                    Setters = { new Setter(TranslateTransform.XProperty, 0.0) }
                }
            }
        };

        return animation;
    }
}
