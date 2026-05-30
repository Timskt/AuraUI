using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Layout;
using Avalonia.Threading;
using Avalonia.VisualTree;

namespace AuraUI.Controls.Layout;

/// <summary>
/// Specifies the position where the <see cref="Affix"/> control sticks.
/// </summary>
public enum AffixPosition
{
    /// <summary>Sticks to the top of the scroll container.</summary>
    Top,
    /// <summary>Sticks to the bottom of the scroll container.</summary>
    Bottom
}

/// <summary>
/// A sticky positioning control that pins its content to a specified position
/// within a scrollable container, inspired by Ant Design's Affix component.
/// </summary>
[PseudoClasses(":affixed-top", ":affixed-bottom")]
public class Affix : ContentControl
{
    private ScrollViewer? _scrollViewer;
    private bool _isAffixed;

    /// <summary>
    /// Defines the <see cref="OffsetTop"/> styled property.
    /// The pixel offset from the top of the scroll container.
    /// </summary>
    public static readonly StyledProperty<double> OffsetTopProperty =
        AvaloniaProperty.Register<Affix, double>(nameof(OffsetTop));

    /// <summary>
    /// Defines the <see cref="OffsetBottom"/> styled property.
    /// The pixel offset from the bottom of the scroll container.
    /// </summary>
    public static readonly StyledProperty<double> OffsetBottomProperty =
        AvaloniaProperty.Register<Affix, double>(nameof(OffsetBottom));

    /// <summary>
    /// Defines the <see cref="Position"/> styled property.
    /// </summary>
    public static readonly StyledProperty<AffixPosition> PositionProperty =
        AvaloniaProperty.Register<Affix, AffixPosition>(nameof(Position), AffixPosition.Top);

    /// <summary>
    /// Defines the <see cref="Target"/> styled property.
    /// The scroll container to listen to. When null, the nearest ScrollViewer ancestor is used.
    /// </summary>
    public static readonly StyledProperty<ScrollViewer?> TargetProperty =
        AvaloniaProperty.Register<Affix, ScrollViewer?>(nameof(Target));

    /// <summary>
    /// Defines the <see cref="ZIndex"/> styled property.
    /// </summary>
    public static new readonly StyledProperty<int> ZIndexProperty =
        AvaloniaProperty.Register<Affix, int>(nameof(ZIndex), 100);

    /// <summary>
    /// Occurs when the affix state changes.
    /// </summary>
    public event EventHandler<AffixChangedEventArgs>? AffixChanged;

    static Affix()
    {
        PositionProperty.Changed.AddClassHandler<Affix>((x, _) => x.UpdateAffix());
    }

    /// <summary>
    /// Gets or sets the offset from the top in pixels.
    /// </summary>
    public double OffsetTop
    {
        get => GetValue(OffsetTopProperty);
        set => SetValue(OffsetTopProperty, value);
    }

    /// <summary>
    /// Gets or sets the offset from the bottom in pixels.
    /// </summary>
    public double OffsetBottom
    {
        get => GetValue(OffsetBottomProperty);
        set => SetValue(OffsetBottomProperty, value);
    }

    /// <summary>
    /// Gets or sets which edge to stick to.
    /// </summary>
    public AffixPosition Position
    {
        get => GetValue(PositionProperty);
        set => SetValue(PositionProperty, value);
    }

    /// <summary>
    /// Gets or sets the target scroll container.
    /// </summary>
    public ScrollViewer? Target
    {
        get => GetValue(TargetProperty);
        set => SetValue(TargetProperty, value);
    }

    /// <summary>
    /// Gets or sets the z-index when affixed.
    /// </summary>
    public new int ZIndex
    {
        get => GetValue(ZIndexProperty);
        set => SetValue(ZIndexProperty, value);
    }

    /// <summary>
    /// Gets whether the control is currently affixed (sticky).
    /// </summary>
    public bool IsAffixed => _isAffixed;

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        FindAndSubscribeScrollViewer();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        UnsubscribeScrollViewer();
    }

    private void FindAndSubscribeScrollViewer()
    {
        UnsubscribeScrollViewer();

        _scrollViewer = Target ?? this.FindAncestorOfType<ScrollViewer>();
        if (_scrollViewer != null)
        {
            _scrollViewer.ScrollChanged += OnScrollChanged;
        }
    }

    private void UnsubscribeScrollViewer()
    {
        if (_scrollViewer != null)
        {
            _scrollViewer.ScrollChanged -= OnScrollChanged;
            _scrollViewer = null;
        }
    }

    private void OnScrollChanged(object? sender, ScrollChangedEventArgs e)
    {
        UpdateAffix();
    }

    private void UpdateAffix()
    {
        if (_scrollViewer == null) return;

        var wasAffixed = _isAffixed;

        if (Position == AffixPosition.Top)
        {
            var scrollTop = _scrollViewer.Offset.Y;
            _isAffixed = scrollTop >= OffsetTop;
        }
        else
        {
            var scrollBottom = _scrollViewer.Extent.Height - _scrollViewer.Viewport.Height - _scrollViewer.Offset.Y;
            _isAffixed = scrollBottom >= OffsetBottom;
        }

        PseudoClasses.Set(":affixed-top", _isAffixed && Position == AffixPosition.Top);
        PseudoClasses.Set(":affixed-bottom", _isAffixed && Position == AffixPosition.Bottom);

        if (wasAffixed != _isAffixed)
        {
            AffixChanged?.Invoke(this, new AffixChangedEventArgs(_isAffixed));
        }
    }
}

/// <summary>
/// Event arguments for the <see cref="Affix.AffixChanged"/> event.
/// </summary>
public class AffixChangedEventArgs : EventArgs
{
    /// <summary>
    /// Gets whether the control is now affixed.
    /// </summary>
    public bool IsAffixed { get; }

    public AffixChangedEventArgs(bool isAffixed)
    {
        IsAffixed = isAffixed;
    }
}
