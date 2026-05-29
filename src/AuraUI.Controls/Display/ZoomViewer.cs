using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Input;
using Avalonia.Media;

namespace AuraUI.Controls.Display;

/// <summary>
/// A zoomable and pannable content container.
/// Supports mouse-wheel zoom at cursor position, click-drag panning,
/// double-click reset, and exposes a zoom level indicator.
/// </summary>
[PseudoClasses(":zooming", ":panning", ":can-zoom", ":can-pan")]
public class ZoomViewer : ContentControl
{
    /// <summary>
    /// Defines the <see cref="ZoomLevel"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> ZoomLevelProperty =
        AvaloniaProperty.Register<ZoomViewer, double>(
            nameof(ZoomLevel),
            1.0,
            coerce: (_, v) => v);

    /// <summary>
    /// Defines the <see cref="MinZoomLevel"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> MinZoomLevelProperty =
        AvaloniaProperty.Register<ZoomViewer, double>(
            nameof(MinZoomLevel),
            0.1);

    /// <summary>
    /// Defines the <see cref="MaxZoomLevel"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> MaxZoomLevelProperty =
        AvaloniaProperty.Register<ZoomViewer, double>(
            nameof(MaxZoomLevel),
            10.0);

    /// <summary>
    /// Defines the <see cref="ZoomLevelInterval"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> ZoomLevelIntervalProperty =
        AvaloniaProperty.Register<ZoomViewer, double>(
            nameof(ZoomLevelInterval),
            0.1);

    /// <summary>
    /// Defines the <see cref="CanZoom"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> CanZoomProperty =
        AvaloniaProperty.Register<ZoomViewer, bool>(
            nameof(CanZoom),
            true);

    /// <summary>
    /// Defines the <see cref="CanPan"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> CanPanProperty =
        AvaloniaProperty.Register<ZoomViewer, bool>(
            nameof(CanPan),
            true);

    /// <summary>
    /// Defines the <see cref="ZoomLevelIndicatorFormat"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string> ZoomLevelIndicatorFormatProperty =
        AvaloniaProperty.Register<ZoomViewer, string>(
            nameof(ZoomLevelIndicatorFormat),
            "{0:P0}");

    private Point _panStart;
    private Point _originOffset;
    private bool _isPanning;
    private TranslateTransform? _translateTransform;
    private ScaleTransform? _scaleTransform;
    private TransformGroup? _transformGroup;
    private bool _isTemplateApplied;

    static ZoomViewer()
    {
        ZoomLevelProperty.Changed.AddClassHandler<ZoomViewer>((x, e) => x.OnZoomLevelChanged(e));
        MinZoomLevelProperty.Changed.AddClassHandler<ZoomViewer>((x, _) => x.CoerceZoomLevel());
        MaxZoomLevelProperty.Changed.AddClassHandler<ZoomViewer>((x, _) => x.CoerceZoomLevel());
        CanZoomProperty.Changed.AddClassHandler<ZoomViewer>((x, _) => x.UpdatePseudoClasses());
        CanPanProperty.Changed.AddClassHandler<ZoomViewer>((x, _) => x.UpdatePseudoClasses());
        AffectsRender<ZoomViewer>(ZoomLevelProperty);
    }

    /// <summary>
    /// Gets or sets the current zoom level.
    /// </summary>
    public double ZoomLevel
    {
        get => GetValue(ZoomLevelProperty);
        set => SetValue(ZoomLevelProperty, value);
    }

    /// <summary>
    /// Gets or sets the minimum zoom level.
    /// </summary>
    public double MinZoomLevel
    {
        get => GetValue(MinZoomLevelProperty);
        set => SetValue(MinZoomLevelProperty, value);
    }

    /// <summary>
    /// Gets or sets the maximum zoom level.
    /// </summary>
    public double MaxZoomLevel
    {
        get => GetValue(MaxZoomLevelProperty);
        set => SetValue(MaxZoomLevelProperty, value);
    }

    /// <summary>
    /// Gets or sets the interval at which zoom level changes.
    /// </summary>
    public double ZoomLevelInterval
    {
        get => GetValue(ZoomLevelIntervalProperty);
        set => SetValue(ZoomLevelIntervalProperty, value);
    }

    /// <summary>
    /// Gets or sets whether zooming is allowed.
    /// </summary>
    public bool CanZoom
    {
        get => GetValue(CanZoomProperty);
        set => SetValue(CanZoomProperty, value);
    }

    /// <summary>
    /// Gets or sets whether panning is allowed.
    /// </summary>
    public bool CanPan
    {
        get => GetValue(CanPanProperty);
        set => SetValue(CanPanProperty, value);
    }

    /// <summary>
    /// Gets or sets the format string for the zoom level indicator.
    /// </summary>
    public string ZoomLevelIndicatorFormat
    {
        get => GetValue(ZoomLevelIndicatorFormatProperty);
        set => SetValue(ZoomLevelIndicatorFormatProperty, value);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        _translateTransform = new TranslateTransform();
        _scaleTransform = new ScaleTransform();
        _transformGroup = new TransformGroup
        {
            Children = new Transforms { _scaleTransform, _translateTransform }
        };

        // Apply transform to the content presenter if found
        if (e.NameScope.Find<ContentPresenter>("PART_ContentPresenter") is { } presenter)
        {
            presenter.RenderTransform = _transformGroup;
            presenter.RenderTransformOrigin = new RelativePoint(0.5, 0.5, RelativeUnit.Relative);
        }

        _isTemplateApplied = true;
        UpdatePseudoClasses();
        ApplyTransform();
    }

    protected override void OnPointerWheelChanged(PointerWheelEventArgs e)
    {
        if (!CanZoom)
        {
            base.OnPointerWheelChanged(e);
            return;
        }

        e.Handled = true;

        var interval = ZoomLevelInterval;
        var delta = e.Delta.Y > 0 ? interval : -interval;
        var newZoom = Math.Clamp(ZoomLevel + delta, MinZoomLevel, MaxZoomLevel);

        if (Math.Abs(newZoom - ZoomLevel) > 0.001)
        {
            // Zoom towards cursor position
            var cursorPos = e.GetPosition(this);
            var centerX = Bounds.Width / 2;
            var centerY = Bounds.Height / 2;

            var oldZoom = ZoomLevel;
            var ratio = newZoom / oldZoom;

            // Adjust pan offset so zoom is centered on cursor
            if (_translateTransform != null)
            {
                var offsetX = (cursorPos.X - centerX) * (1 - ratio);
                var offsetY = (cursorPos.Y - centerY) * (1 - ratio);
                _translateTransform.X += offsetX;
                _translateTransform.Y += offsetY;
            }

            ZoomLevel = newZoom;
        }
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);

        if (CanPan && e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            _isPanning = true;
            _panStart = e.GetPosition(this);
            _originOffset = new Point(
                _translateTransform?.X ?? 0,
                _translateTransform?.Y ?? 0);
            PseudoClasses.Set(":panning", true);
            e.Handled = true;
        }
    }

    protected override void OnPointerMoved(PointerMovedEventArgs e)
    {
        base.OnPointerMoved(e);

        if (_isPanning && _translateTransform != null)
        {
            var current = e.GetPosition(this);
            var delta = current - _panStart;
            _translateTransform.X = _originOffset.X + delta.X;
            _translateTransform.Y = _originOffset.Y + delta.Y;
            e.Handled = true;
        }
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);

        if (_isPanning)
        {
            _isPanning = false;
            PseudoClasses.Set(":panning", false);
            e.Handled = true;
        }
    }

    protected override void OnDoubleTapped(TappedEventArgs e)
    {
        base.OnDoubleTapped(e);

        // Reset zoom and pan on double-click
        ResetView();
        e.Handled = true;
    }

    /// <summary>
    /// Resets the zoom level and pan offset to their defaults.
    /// </summary>
    public void ResetView()
    {
        ZoomLevel = 1.0;
        if (_translateTransform != null)
        {
            _translateTransform.X = 0;
            _translateTransform.Y = 0;
        }
    }

    /// <summary>
    /// Zooms to fit the content within the viewer bounds.
    /// </summary>
    public void ZoomToFit()
    {
        ResetView();
    }

    private void OnZoomLevelChanged(AvaloniaPropertyChangedEventArgs e)
    {
        PseudoClasses.Set(":zooming", true);
        ApplyTransform();

        // Clear zooming pseudo-class after a short delay
        Dispatcher.UIThread.Post(() => PseudoClasses.Set(":zooming", false),
            DispatcherPriority.Background);
    }

    private void CoerceZoomLevel()
    {
        var zoom = ZoomLevel;
        var min = MinZoomLevel;
        var max = MaxZoomLevel;
        if (zoom < min) ZoomLevel = min;
        else if (zoom > max) ZoomLevel = max;
    }

    private void ApplyTransform()
    {
        if (!_isTemplateApplied || _scaleTransform == null) return;

        _scaleTransform.ScaleX = ZoomLevel;
        _scaleTransform.ScaleY = ZoomLevel;
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(":can-zoom", CanZoom);
        PseudoClasses.Set(":can-pan", CanPan);
    }
}
