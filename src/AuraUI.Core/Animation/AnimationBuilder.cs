using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Styling;

namespace AuraUI.Core.Animation;

/// <summary>
/// Fluent builder for constructing and running Avalonia animations.
/// </summary>
/// <example>
/// <code>
/// var animation = AnimationBuilder.Create()
///     .Property(OpacityProperty)
///     .From(0.0)
///     .To(1.0)
///     .Duration(300)
///     .Easing(new CubicEaseOut())
///     .Delay(100)
///     .Build();
///
/// await animation.RunAsync(control);
/// </code>
/// </example>
public class AnimationBuilder
{
    private AvaloniaProperty? _property;
    private object? _from;
    private object? _to;
    private TimeSpan _duration = TimeSpan.FromMilliseconds(300);
    private Easing _easing = new CubicEaseOut();
    private TimeSpan _delay = TimeSpan.Zero;
    private Avalonia.Animation.FillMode _fillMode = Avalonia.Animation.FillMode.Forward;
    private IterationCount _iterationCount = new(1);

    private AnimationBuilder() { }

    /// <summary>
    /// Creates a new <see cref="AnimationBuilder"/> instance.
    /// </summary>
    public static AnimationBuilder Create() => new();

    /// <summary>
    /// Sets the property to animate.
    /// </summary>
    public AnimationBuilder Property(AvaloniaProperty property)
    {
        _property = property ?? throw new ArgumentNullException(nameof(property));
        return this;
    }

    /// <summary>
    /// Sets the starting value for the animation.
    /// </summary>
    public AnimationBuilder From(object? value)
    {
        _from = value;
        return this;
    }

    /// <summary>
    /// Sets the ending value for the animation.
    /// </summary>
    public AnimationBuilder To(object? value)
    {
        _to = value ?? throw new ArgumentNullException(nameof(value));
        return this;
    }

    /// <summary>
    /// Sets the animation duration in milliseconds.
    /// </summary>
    public AnimationBuilder Duration(double milliseconds)
    {
        _duration = TimeSpan.FromMilliseconds(milliseconds);
        return this;
    }

    /// <summary>
    /// Sets the animation duration.
    /// </summary>
    public AnimationBuilder Duration(TimeSpan duration)
    {
        _duration = duration;
        return this;
    }

    /// <summary>
    /// Sets the easing function for the animation.
    /// </summary>
    public AnimationBuilder Easing(Easing easing)
    {
        _easing = easing ?? throw new ArgumentNullException(nameof(easing));
        return this;
    }

    /// <summary>
    /// Sets the delay before the animation starts, in milliseconds.
    /// </summary>
    public AnimationBuilder Delay(double milliseconds)
    {
        _delay = TimeSpan.FromMilliseconds(milliseconds);
        return this;
    }

    /// <summary>
    /// Sets the delay before the animation starts.
    /// </summary>
    public AnimationBuilder Delay(TimeSpan delay)
    {
        _delay = delay;
        return this;
    }

    /// <summary>
    /// Sets the fill mode (how the animation behaves before and after execution).
    /// </summary>
    public AnimationBuilder SetFillMode(Avalonia.Animation.FillMode fillMode)
    {
        _fillMode = fillMode;
        return this;
    }

    /// <summary>
    /// Sets the animation to run infinitely.
    /// </summary>
    public AnimationBuilder RepeatForever()
    {
        _iterationCount = IterationCount.Infinite;
        return this;
    }

    /// <summary>
    /// Sets a specific iteration count for the animation.
    /// </summary>
    public AnimationBuilder Repeat(int count)
    {
        _iterationCount = new IterationCount((ulong)count);
        return this;
    }

    /// <summary>
    /// Adds an intermediate keyframe at the specified cue (0.0 to 1.0).
    /// </summary>
    public AnimationBuilder KeyFrame(double cue, Action<KeyFrameBuilder> configure)
    {
        var builder = new KeyFrameBuilder(cue);
        configure(builder);
        _extraKeyFrames ??= new();
        _extraKeyFrames.Add(builder.Build());
        return this;
    }

    private List<KeyFrame>? _extraKeyFrames;

    /// <summary>
    /// Builds the <see cref="Avalonia.Animation.Animation"/> object.
    /// </summary>
    public Avalonia.Animation.Animation Build()
    {
        if (_property is null)
            throw new InvalidOperationException("Property must be set before building the animation.");
        if (_to is null)
            throw new InvalidOperationException("'To' value must be set before building the animation.");

        var animation = new Avalonia.Animation.Animation
        {
            Duration = _duration,
            Easing = _easing,
            FillMode = _fillMode,
            IterationCount = _iterationCount,
            Delay = _delay,
        };

        // "From" keyframe at 0%
        if (_from is not null)
        {
            animation.Children.Add(new KeyFrame
            {
                Cue = new Cue(0.0),
                Setters = { new Setter(_property, _from) }
            });
        }

        // Extra intermediate keyframes
        if (_extraKeyFrames is not null)
        {
            foreach (var kf in _extraKeyFrames)
                animation.Children.Add(kf);
        }

        // "To" keyframe at 100%
        animation.Children.Add(new KeyFrame
        {
            Cue = new Cue(1.0),
            Setters = { new Setter(_property, _to) }
        });

        return animation;
    }

    /// <summary>
    /// Builds and runs the animation on the specified control.
    /// </summary>
    public Task RunAsync(Control control)
    {
        return Build().RunAsync(control);
    }
}

/// <summary>
/// Fluent builder for individual keyframes within an <see cref="AnimationBuilder"/>.
/// </summary>
public class KeyFrameBuilder
{
    private readonly double _cue;
    private readonly List<Setter> _setters = new();

    internal KeyFrameBuilder(double cue)
    {
        _cue = cue;
    }

    /// <summary>
    /// Adds a setter to this keyframe.
    /// </summary>
    public KeyFrameBuilder Set(AvaloniaProperty property, object value)
    {
        _setters.Add(new Setter(property, value));
        return this;
    }

    internal KeyFrame Build()
    {
        var kf = new KeyFrame
        {
            Cue = new Cue(_cue)
        };
        foreach (var setter in _setters)
            kf.Setters.Add(setter);
        return kf;
    }
}
