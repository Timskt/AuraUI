using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;

namespace AuraUI.Controls.Layout;

/// <summary>
/// A collapsible container that shows a header and optionally expanded content
/// beneath (or beside) the header.
/// </summary>
[TemplatePart("PART_HeaderSite", typeof(ContentControl))]
[TemplatePart("PART_ExpandSite", typeof(ContentControl))]
[PseudoClasses(":expanded", ":collapsed", ":up", ":down", ":left", ":right")]
public class Expander : HeaderedContentControl
{
    private ContentControl? _expandSite;
    private bool _isAnimating;

    /// <summary>
    /// Defines the <see cref="IsExpanded"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsExpandedProperty =
        AvaloniaProperty.Register<Expander, bool>(nameof(IsExpanded));

    /// <summary>
    /// Defines the <see cref="ExpandDirection"/> styled property.
    /// </summary>
    public static readonly StyledProperty<ExpandDirection> ExpandDirectionProperty =
        AvaloniaProperty.Register<Expander, ExpandDirection>(
            nameof(ExpandDirection),
            ExpandDirection.Down);

    /// <summary>
    /// Defines the <see cref="ExpandAnimationDuration"/> styled property.
    /// </summary>
    public static readonly StyledProperty<TimeSpan> ExpandAnimationDurationProperty =
        AvaloniaProperty.Register<Expander, TimeSpan>(
            nameof(ExpandAnimationDuration),
            TimeSpan.FromMilliseconds(250));

    static Expander()
    {
        IsExpandedProperty.Changed.AddClassHandler<Expander>((x, e) => x.OnIsExpandedChanged(e));
        ExpandDirectionProperty.Changed.AddClassHandler<Expander>((x, e) => x.OnExpandDirectionChanged(e));
    }

    /// <summary>
    /// Gets or sets whether the expander content is currently visible.
    /// </summary>
    public bool IsExpanded
    {
        get => GetValue(IsExpandedProperty);
        set => SetValue(IsExpandedProperty, value);
    }

    /// <summary>
    /// Gets or sets the direction in which the expander opens.
    /// </summary>
    public ExpandDirection ExpandDirection
    {
        get => GetValue(ExpandDirectionProperty);
        set => SetValue(ExpandDirectionProperty, value);
    }

    /// <summary>
    /// Gets or sets the duration of the expand/collapse animation.
    /// </summary>
    public TimeSpan ExpandAnimationDuration
    {
        get => GetValue(ExpandAnimationDurationProperty);
        set => SetValue(ExpandAnimationDurationProperty, value);
    }

    /// <summary>
    /// Occurs when the expander is opened.
    /// </summary>
    public event EventHandler? Expanded;

    /// <summary>
    /// Occurs when the expander is collapsed.
    /// </summary>
    public event EventHandler? Collapsed;

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _expandSite = e.NameScope.Find<ContentControl>("PART_ExpandSite");
        UpdateExpandSiteVisibility(false);
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);

        // Toggle expansion when the header area is clicked.
        if (e.Source is Control source && IsHeaderArea(source))
        {
            SetCurrentValue(IsExpandedProperty, !IsExpanded);
        }
    }

    private bool IsHeaderArea(Control source)
    {
        // Walk up the visual tree to see if the click was on the header.
        var current = source;
        while (current != null)
        {
            if (current is ContentPresenter presenter &&
                presenter.Name == "PART_HeaderSite")
            {
                return true;
            }

            // Also check if the click is on the header content area (the part before the expand site).
            if (current == this)
            {
                // If we reach the Expander itself without finding the expand site,
                // assume click was in header area.
                return false;
            }

            if (current.TemplatedParent == this)
            {
                // Direct template child; check if this is NOT the expand site.
                if (current != _expandSite &&
                    current.Name != "PART_ExpandSite")
                {
                    return true;
                }
            }

            current = current.GetVisualParent() as Control;
        }

        return false;
    }

    private void OnIsExpandedChanged(AvaloniaPropertyChangedEventArgs e)
    {
        var isExpanded = (bool)e.NewValue!;

        UpdatePseudoClasses();

        if (isExpanded)
        {
            Expanded?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            Collapsed?.Invoke(this, EventArgs.Empty);
        }

        UpdateExpandSiteVisibility(true);
    }

    private void OnExpandDirectionChanged(AvaloniaPropertyChangedEventArgs e)
    {
        UpdateDirectionPseudoClasses();
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(":expanded", IsExpanded);
        PseudoClasses.Set(":collapsed", !IsExpanded);
    }

    private void UpdateDirectionPseudoClasses()
    {
        PseudoClasses.Set(":up", ExpandDirection == ExpandDirection.Up);
        PseudoClasses.Set(":down", ExpandDirection == ExpandDirection.Down);
        PseudoClasses.Set(":left", ExpandDirection == ExpandDirection.Left);
        PseudoClasses.Set(":right", ExpandDirection == ExpandDirection.Right);
    }

    private void UpdateExpandSiteVisibility(bool animate)
    {
        if (_expandSite == null) return;

        if (IsExpanded)
        {
            _expandSite.IsVisible = true;

            if (animate)
            {
                AnimateExpand();
            }
        }
        else
        {
            if (animate)
            {
                AnimateCollapse(() =>
                {
                    if (_expandSite != null)
                    {
                        _expandSite.IsVisible = false;
                    }
                });
            }
            else
            {
                _expandSite.IsVisible = false;
            }
        }
    }

    private async void AnimateExpand()
    {
        if (_expandSite == null || _isAnimating) return;
        _isAnimating = true;

        try
        {
            var duration = ExpandAnimationDuration;
            var isHorizontal = ExpandDirection is ExpandDirection.Left or ExpandDirection.Right;

            var animation = new Animation
            {
                Duration = duration,
                Easing = new CubicEaseOut(),
                Children =
                {
                    new KeyFrame
                    {
                        Cue = new Cue(0d),
                        Setters =
                        {
                            new Setter(
                                isHorizontal ? WidthProperty : HeightProperty,
                                0d)
                        }
                    },
                    new KeyFrame
                    {
                        Cue = new Cue(1d),
                        Setters =
                        {
                            new Setter(
                                isHorizontal ? WidthProperty : HeightProperty,
                                isHorizontal ? _expandSite.DesiredSize.Width : _expandSite.DesiredSize.Height)
                        }
                    }
                }
            };

            await animation.RunAsync(_expandSite);
        }
        finally
        {
            _isAnimating = false;
        }
    }

    private async void AnimateCollapse(Action onComplete)
    {
        if (_expandSite == null || _isAnimating)
        {
            onComplete();
            return;
        }
        _isAnimating = true;

        try
        {
            var duration = ExpandAnimationDuration;
            var isHorizontal = ExpandDirection is ExpandDirection.Left or ExpandDirection.Right;
            var currentSize = isHorizontal ? _expandSite.Bounds.Width : _expandSite.Bounds.Height;

            var animation = new Animation
            {
                Duration = duration,
                Easing = new CubicEaseIn(),
                Children =
                {
                    new KeyFrame
                    {
                        Cue = new Cue(0d),
                        Setters =
                        {
                            new Setter(
                                isHorizontal ? WidthProperty : HeightProperty,
                                currentSize)
                        }
                    },
                    new KeyFrame
                    {
                        Cue = new Cue(1d),
                        Setters =
                        {
                            new Setter(
                                isHorizontal ? WidthProperty : HeightProperty,
                                0d)
                        }
                    }
                }
            };

            await animation.RunAsync(_expandSite);
        }
        finally
        {
            _isAnimating = false;
            onComplete();
        }
    }
}
