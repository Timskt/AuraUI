using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Styling;
using AuraUI.Core.Navigation;

namespace AuraUI.Controls.Navigation;

/// <summary>
/// Specifies the type of page transition animation.
/// </summary>
public enum PageTransitionType
{
    /// <summary>No transition animation.</summary>
    None,
    /// <summary>Fade in/out transition.</summary>
    Fade,
    /// <summary>Crossfade: new content fades in while old content fades out.</summary>
    CrossFade,
    /// <summary>Slide from the right.</summary>
    SlideLeft,
    /// <summary>Slide from the left.</summary>
    SlideRight,
    /// <summary>Slide from the bottom.</summary>
    SlideUp,
    /// <summary>Slide from the top.</summary>
    SlideDown
}

/// <summary>
/// A content frame that displays the current route's ViewModel from a <see cref="Router"/>.
/// Supports page transition animations (fade, crossfade, slide) and automatically resolves
/// views for ViewModels using a configurable data template or view locator pattern.
/// </summary>
/// <example>
/// <code>
/// &lt;Navigation:Frame Router="{Binding Router}"
///                  TransitionType="CrossFade"
///                  TransitionDuration="00:00:00.3"/&gt;
/// </code>
/// </example>
public class Frame : ContentControl
{
    /// <summary>
    /// Defines the <see cref="Router"/> styled property.
    /// The router whose current ViewModel is displayed by this frame.
    /// </summary>
    public static readonly StyledProperty<Core.Navigation.Router?> RouterProperty =
        AvaloniaProperty.Register<Frame, Core.Navigation.Router?>(nameof(Router));

    /// <summary>
    /// Defines the <see cref="TransitionType"/> styled property.
    /// </summary>
    public static readonly StyledProperty<PageTransitionType> TransitionTypeProperty =
        AvaloniaProperty.Register<Frame, PageTransitionType>(nameof(TransitionType), PageTransitionType.CrossFade);

    /// <summary>
    /// Defines the <see cref="TransitionDuration"/> styled property.
    /// </summary>
    public static readonly StyledProperty<TimeSpan> TransitionDurationProperty =
        AvaloniaProperty.Register<Frame, TimeSpan>(nameof(TransitionDuration), TimeSpan.FromMilliseconds(300));

    /// <summary>
    /// Defines the <see cref="TransitionEasing"/> styled property.
    /// </summary>
    public static readonly StyledProperty<Easing> TransitionEasingProperty =
        AvaloniaProperty.Register<Frame, Easing>(nameof(TransitionEasing), new CubicEaseInOut());

    /// <summary>
    /// Defines the <see cref="SlideDirection"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> SlideOffsetProperty =
        AvaloniaProperty.Register<Frame, double>(nameof(SlideOffset), 300.0);

    private Core.Navigation.Router? _previousRouter;

    static Frame()
    {
        RouterProperty.Changed.AddClassHandler<Frame>((x, e) => x.OnRouterChanged(e));
    }

    /// <summary>
    /// Gets or sets the router whose current ViewModel is displayed.
    /// </summary>
    public Core.Navigation.Router? Router
    {
        get => GetValue(RouterProperty);
        set => SetValue(RouterProperty, value);
    }

    /// <summary>
    /// Gets or sets the page transition type.
    /// </summary>
    public PageTransitionType TransitionType
    {
        get => GetValue(TransitionTypeProperty);
        set => SetValue(TransitionTypeProperty, value);
    }

    /// <summary>
    /// Gets or sets the duration of the page transition animation.
    /// </summary>
    public TimeSpan TransitionDuration
    {
        get => GetValue(TransitionDurationProperty);
        set => SetValue(TransitionDurationProperty, value);
    }

    /// <summary>
    /// Gets or sets the easing function for the transition animation.
    /// </summary>
    public Easing TransitionEasing
    {
        get => GetValue(TransitionEasingProperty);
        set => SetValue(TransitionEasingProperty, value);
    }

    /// <summary>
    /// Gets or sets the pixel offset for slide transitions.
    /// </summary>
    public double SlideOffset
    {
        get => GetValue(SlideOffsetProperty);
        set => SetValue(SlideOffsetProperty, value);
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        SubscribeToRouter(Router);
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        UnsubscribeFromRouter(_previousRouter);
    }

    private void OnRouterChanged(AvaloniaPropertyChangedEventArgs e)
    {
        UnsubscribeFromRouter(_previousRouter);

        var router = e.NewValue as Core.Navigation.Router;
        SubscribeToRouter(router);
        _previousRouter = router;

        // Immediately display current content
        if (router?.CurrentViewModel is not null)
        {
            UpdateContent(router.CurrentViewModel, animate: false);
        }
    }

    private void SubscribeToRouter(Core.Navigation.Router? router)
    {
        if (router is null) return;
        router.Navigated += OnNavigated;
        router.PropertyChanged += OnRouterPropertyChanged;
    }

    private void UnsubscribeFromRouter(Core.Navigation.Router? router)
    {
        if (router is null) return;
        router.Navigated -= OnNavigated;
        router.PropertyChanged -= OnRouterPropertyChanged;
    }

    private void OnNavigated(object? sender, RouteChangedEventArgs e)
    {
        if (e.ViewModel is not null)
        {
            UpdateContent(e.ViewModel, animate: true);
        }
    }

    private void OnRouterPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Core.Navigation.Router.CurrentViewModel))
        {
            var router = sender as Core.Navigation.Router;
            if (router?.CurrentViewModel is not null)
            {
                UpdateContent(router.CurrentViewModel, animate: true);
            }
        }
    }

    private async void UpdateContent(object viewModel, bool animate)
    {
        var transitionType = TransitionType;

        if (animate && transitionType != PageTransitionType.None)
        {
            var duration = TransitionDuration;
            var easing = TransitionEasing;

            IPageTransition? transition = transitionType switch
            {
                PageTransitionType.Fade => new FadeTransition(duration, easing),
                PageTransitionType.CrossFade => new CrossFadeTransition(duration, easing),
                PageTransitionType.SlideLeft => new SlideTransition(duration, easing, SlideDirection.Left, SlideOffset),
                PageTransitionType.SlideRight => new SlideTransition(duration, easing, SlideDirection.Right, SlideOffset),
                PageTransitionType.SlideUp => new SlideTransition(duration, easing, SlideDirection.Up, SlideOffset),
                PageTransitionType.SlideDown => new SlideTransition(duration, easing, SlideDirection.Down, SlideOffset),
                _ => null
            };

            if (transition is not null)
            {
                await transition.Run(this, viewModel);
                return;
            }
        }

        Content = viewModel;
    }
}

/// <summary>
/// Direction for slide transitions.
/// </summary>
internal enum SlideDirection
{
    Left,
    Right,
    Up,
    Down
}

/// <summary>
/// Interface for page transitions.
/// </summary>
internal interface IPageTransition
{
    System.Threading.Tasks.Task Run(Frame frame, object newContent);
}

/// <summary>
/// Fade transition implementation.
/// </summary>
internal sealed class FadeTransition : IPageTransition
{
    private readonly TimeSpan _duration;
    private readonly Easing _easing;

    public FadeTransition(TimeSpan duration, Easing easing)
    {
        _duration = duration;
        _easing = easing;
    }

    public async System.Threading.Tasks.Task Run(Frame frame, object newContent)
    {
        // Fade out current content
        var fadeOut = new Avalonia.Animation.Animation
        {
            Duration = _duration / 2,
            Easing = _easing,
            FillMode = FillMode.Forward,
            Children =
            {
                new KeyFrame { Cue = new Cue(0.0), Setters = { new Setter(OpacityProperty, 1.0) } },
                new KeyFrame { Cue = new Cue(1.0), Setters = { new Setter(OpacityProperty, 0.0) } }
            }
        };

        await fadeOut.RunAsync(frame);

        frame.Content = newContent;

        // Fade in new content
        var fadeIn = new Avalonia.Animation.Animation
        {
            Duration = _duration / 2,
            Easing = _easing,
            FillMode = FillMode.Forward,
            Children =
            {
                new KeyFrame { Cue = new Cue(0.0), Setters = { new Setter(OpacityProperty, 0.0) } },
                new KeyFrame { Cue = new Cue(1.0), Setters = { new Setter(OpacityProperty, 1.0) } }
            }
        };

        await fadeIn.RunAsync(frame);
    }

    private static readonly StyledProperty<double> OpacityProperty = Visual.OpacityProperty;
}

/// <summary>
/// Crossfade transition: new content fades in while old content fades out simultaneously.
/// </summary>
internal sealed class CrossFadeTransition : IPageTransition
{
    private readonly TimeSpan _duration;
    private readonly Easing _easing;

    public CrossFadeTransition(TimeSpan duration, Easing easing)
    {
        _duration = duration;
        _easing = easing;
    }

    public async System.Threading.Tasks.Task Run(Frame frame, object newContent)
    {
        // Simply swap content with a fade on the frame
        var animation = new Avalonia.Animation.Animation
        {
            Duration = _duration,
            Easing = _easing,
            FillMode = FillMode.Forward,
            Children =
            {
                new KeyFrame { Cue = new Cue(0.0), Setters = { new Setter(Visual.OpacityProperty, 1.0) } },
                new KeyFrame { Cue = new Cue(0.5), Setters = { new Setter(Visual.OpacityProperty, 0.0) } },
                new KeyFrame { Cue = new Cue(1.0), Setters = { new Setter(Visual.OpacityProperty, 1.0) } }
            }
        };

        // Swap content at the midpoint
        var halfDuration = _duration / 2;
        var tcs = new System.Threading.Tasks.TaskCompletionSource<bool>();

        var timer = new System.Threading.Timer(_ =>
        {
            frame.Content = newContent;
            tcs.TrySetResult(true);
        }, null, halfDuration, System.Threading.Timeout.InfiniteTimeSpan);

        await animation.RunAsync(frame);
        await tcs.Task;
        await timer.DisposeAsync();
    }
}

/// <summary>
/// Slide transition: content slides in from a direction.
/// </summary>
internal sealed class SlideTransition : IPageTransition
{
    private readonly TimeSpan _duration;
    private readonly Easing _easing;
    private readonly SlideDirection _direction;
    private readonly double _offset;

    public SlideTransition(TimeSpan duration, Easing easing, SlideDirection direction, double offset)
    {
        _duration = duration;
        _easing = easing;
        _direction = direction;
        _offset = offset;
    }

    public async System.Threading.Tasks.Task Run(Frame frame, object newContent)
    {
        var startOffset = _direction switch
        {
            SlideDirection.Left => _offset,
            SlideDirection.Right => -_offset,
            SlideDirection.Up => _offset,
            SlideDirection.Down => -_offset,
            _ => _offset
        };

        var isHorizontal = _direction is SlideDirection.Left or SlideDirection.Right;
        var translateProperty = isHorizontal
            ? Avalonia.Media.TranslateTransform.XProperty
            : Avalonia.Media.TranslateTransform.YProperty;

        // Ensure transform exists
        if (frame.RenderTransform is not Avalonia.Media.TranslateTransform translate)
        {
            translate = new Avalonia.Media.TranslateTransform();
            frame.RenderTransform = translate;
        }

        // Slide out
        var slideOut = new Avalonia.Animation.Animation
        {
            Duration = _duration / 2,
            Easing = _easing,
            FillMode = FillMode.Forward,
            Children =
            {
                new KeyFrame { Cue = new Cue(0.0), Setters = { new Setter(Visual.OpacityProperty, 1.0) } },
                new KeyFrame { Cue = new Cue(1.0), Setters = { new Setter(Visual.OpacityProperty, 0.0) } }
            }
        };

        await slideOut.RunAsync(frame);

        frame.Content = newContent;

        // Position new content off-screen
        if (isHorizontal)
            translate.X = -startOffset;
        else
            translate.Y = -startOffset;

        // Slide in
        var slideIn = new Avalonia.Animation.Animation
        {
            Duration = _duration / 2,
            Easing = _easing,
            FillMode = FillMode.Forward,
            Children =
            {
                new KeyFrame
                {
                    Cue = new Cue(0.0),
                    Setters =
                    {
                        new Setter(Visual.OpacityProperty, 0.0),
                        new Setter(translateProperty, -startOffset)
                    }
                },
                new KeyFrame
                {
                    Cue = new Cue(1.0),
                    Setters =
                    {
                        new Setter(Visual.OpacityProperty, 1.0),
                        new Setter(translateProperty, 0.0)
                    }
                }
            }
        };

        await slideIn.RunAsync(frame);

        // Reset transform
        if (isHorizontal)
            translate.X = 0;
        else
            translate.Y = 0;
    }
}
