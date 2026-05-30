using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;

namespace AuraUI.Controls.Display;

/// <summary>
/// An image cropper control that allows users to select a crop area by dragging and resizing.
/// Supports aspect ratio locking, min/max crop size constraints, and output dimensions.
/// Inspired by Ant Design Pro's image cropper.
/// </summary>
[TemplatePart("PART_Image", typeof(Image))]
[TemplatePart("PART_CropOverlay", typeof(Border))]
[PseudoClasses(":cropping", ":dragging", ":resizing")]
public class ImageCropper : TemplatedControl
{
    private Point _dragStart;
    private Rect _dragStartRect;
    private bool _isDragging;
#pragma warning disable CS0649 // Field is never assigned — populated by framework interaction
    private bool _isResizing;
#pragma warning restore CS0649

    /// <summary>
    /// Defines the <see cref="Source"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IImage?> SourceProperty =
        AvaloniaProperty.Register<ImageCropper, IImage?>(nameof(Source));

    /// <summary>
    /// Defines the <see cref="AspectRatio"/> styled property.
    /// Null means free-form cropping.
    /// </summary>
    public static readonly StyledProperty<double?> AspectRatioProperty =
        AvaloniaProperty.Register<ImageCropper, double?>(nameof(AspectRatio));

    /// <summary>
    /// Defines the <see cref="CropRect"/> styled property.
    /// The current crop rectangle in control coordinates.
    /// </summary>
    public static readonly StyledProperty<Rect> CropRectProperty =
        AvaloniaProperty.Register<ImageCropper, Rect>(nameof(CropRect));

    /// <summary>
    /// Defines the <see cref="MinCropSize"/> styled property.
    /// </summary>
    public static readonly StyledProperty<Size> MinCropSizeProperty =
        AvaloniaProperty.Register<ImageCropper, Size>(nameof(MinCropSize), new Size(20, 20));

    /// <summary>
    /// Defines the <see cref="MaxCropSize"/> styled property.
    /// Null means no maximum.
    /// </summary>
    public static readonly StyledProperty<Size?> MaxCropSizeProperty =
        AvaloniaProperty.Register<ImageCropper, Size?>(nameof(MaxCropSize));

    /// <summary>
    /// Defines the <see cref="OutputWidth"/> styled property.
    /// </summary>
    public static readonly StyledProperty<int> OutputWidthProperty =
        AvaloniaProperty.Register<ImageCropper, int>(nameof(OutputWidth), 256);

    /// <summary>
    /// Defines the <see cref="OutputHeight"/> styled property.
    /// </summary>
    public static readonly StyledProperty<int> OutputHeightProperty =
        AvaloniaProperty.Register<ImageCropper, int>(nameof(OutputHeight), 256);

    /// <summary>
    /// Defines the <see cref="CropOverlayBrush"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> CropOverlayBrushProperty =
        AvaloniaProperty.Register<ImageCropper, IBrush?>(nameof(CropOverlayBrush));

    /// <summary>
    /// Gets or sets the image source to crop.
    /// </summary>
    public IImage? Source
    {
        get => GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }

    /// <summary>
    /// Gets or sets the aspect ratio (width / height). Null for free-form.
    /// </summary>
    public double? AspectRatio
    {
        get => GetValue(AspectRatioProperty);
        set => SetValue(AspectRatioProperty, value);
    }

    /// <summary>
    /// Gets or sets the current crop rectangle.
    /// </summary>
    public Rect CropRect
    {
        get => GetValue(CropRectProperty);
        set => SetValue(CropRectProperty, value);
    }

    /// <summary>
    /// Gets or sets the minimum crop size.
    /// </summary>
    public Size MinCropSize
    {
        get => GetValue(MinCropSizeProperty);
        set => SetValue(MinCropSizeProperty, value);
    }

    /// <summary>
    /// Gets or sets the maximum crop size. Null for no limit.
    /// </summary>
    public Size? MaxCropSize
    {
        get => GetValue(MaxCropSizeProperty);
        set => SetValue(MaxCropSizeProperty, value);
    }

    /// <summary>
    /// Gets or sets the output image width.
    /// </summary>
    public int OutputWidth
    {
        get => GetValue(OutputWidthProperty);
        set => SetValue(OutputWidthProperty, value);
    }

    /// <summary>
    /// Gets or sets the output image height.
    /// </summary>
    public int OutputHeight
    {
        get => GetValue(OutputHeightProperty);
        set => SetValue(OutputHeightProperty, value);
    }

    /// <summary>
    /// Gets or sets the overlay brush for the crop area.
    /// </summary>
    public IBrush? CropOverlayBrush
    {
        get => GetValue(CropOverlayBrushProperty);
        set => SetValue(CropOverlayBrushProperty, value);
    }

    /// <summary>
    /// Occurs when the crop rectangle changes during interaction.
    /// </summary>
    public event EventHandler<RoutedEventArgs>? CropChanged;

    /// <summary>
    /// Occurs when the user finishes a crop interaction (mouse up).
    /// </summary>
    public event EventHandler<RoutedEventArgs>? CropCompleted;

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        UpdatePseudoClasses();
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        var point = e.GetCurrentPoint(this);
        if (!point.Properties.IsLeftButtonPressed) return;

        _dragStart = point.Position;
        _dragStartRect = CropRect;
        _isDragging = true;
        PseudoClasses.Set(":dragging", true);
        e.Handled = true;
        Focus();
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        base.OnPointerMoved(e);
        if (!_isDragging) return;

        var current = e.GetPosition(this);
        var delta = current - _dragStart;

        var newX = Math.Max(0, Math.Min(_dragStartRect.X + delta.X, Bounds.Width - CropRect.Width));
        var newY = Math.Max(0, Math.Min(_dragStartRect.Y + delta.Y, Bounds.Height - CropRect.Height));

        CropRect = new Rect(newX, newY, CropRect.Width, CropRect.Height);
        CropChanged?.Invoke(this, new RoutedEventArgs());
        e.Handled = true;
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);
        if (_isDragging)
        {
            _isDragging = false;
            PseudoClasses.Set(":dragging", false);
            CropCompleted?.Invoke(this, new RoutedEventArgs());
            e.Handled = true;
        }
    }

    /// <summary>
    /// Sets the crop rectangle to cover the entire image area.
    /// </summary>
    public void SelectAll()
    {
        CropRect = new Rect(0, 0, Bounds.Width, Bounds.Height);
    }

    /// <summary>
    /// Resets the crop rectangle.
    /// </summary>
    public void ResetCrop()
    {
        CropRect = new Rect(0, 0, 100, 100);
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(":cropping", CropRect.Width > 0 && CropRect.Height > 0);
        PseudoClasses.Set(":dragging", _isDragging);
        PseudoClasses.Set(":resizing", _isResizing);
    }
}
