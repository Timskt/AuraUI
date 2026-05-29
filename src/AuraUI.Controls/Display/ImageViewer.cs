using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;

namespace AuraUI.Controls.Display;

/// <summary>
/// Specifies the pseudo-classes for the image viewer.
/// </summary>
[PseudoClasses(":fullscreen", ":loading", ":error")]
[TemplatePart("PART_Image", typeof(Image))]
[TemplatePart("PART_ThumbnailStrip", typeof(ItemsControl))]
[TemplatePart("PART_Toolbar", typeof(Panel))]
/// <summary>
/// An image viewer/gallery control with zoom, rotate, fullscreen, thumbnail strip,
/// and keyboard navigation. Inspired by Element Plus's image viewer.
/// </summary>
public class ImageViewer : TemplatedControl
{
    private Image? _image;
    private ItemsControl? _thumbnailStrip;
    private Panel? _toolbar;

    /// <summary>
    /// Defines the <see cref="Source"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IImage?> SourceProperty =
        AvaloniaProperty.Register<ImageViewer, IImage?>(nameof(Source));

    /// <summary>
    /// Defines the <see cref="Sources"/> styled property.
    /// Collection of images for gallery mode.
    /// </summary>
    public static readonly StyledProperty<IList<IImage>?> SourcesProperty =
        AvaloniaProperty.Register<ImageViewer, IList<IImage>?>(nameof(Sources));

    /// <summary>
    /// Defines the <see cref="CurrentIndex"/> styled property.
    /// </summary>
    public static readonly StyledProperty<int> CurrentIndexProperty =
        AvaloniaProperty.Register<ImageViewer, int>(nameof(CurrentIndex));

    /// <summary>
    /// Defines the <see cref="ZoomLevel"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> ZoomLevelProperty =
        AvaloniaProperty.Register<ImageViewer, double>(nameof(ZoomLevel), 1.0);

    /// <summary>
    /// Defines the <see cref="MinZoom"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> MinZoomProperty =
        AvaloniaProperty.Register<ImageViewer, double>(nameof(MinZoom), 0.1);

    /// <summary>
    /// Defines the <see cref="MaxZoom"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> MaxZoomProperty =
        AvaloniaProperty.Register<ImageViewer, double>(nameof(MaxZoom), 10.0);

    /// <summary>
    /// Defines the <see cref="Rotation"/> styled property.
    /// Rotation angle in degrees.
    /// </summary>
    public static readonly StyledProperty<double> RotationProperty =
        AvaloniaProperty.Register<ImageViewer, double>(nameof(Rotation));

    /// <summary>
    /// Defines the <see cref="IsFullscreen"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsFullscreenProperty =
        AvaloniaProperty.Register<ImageViewer, bool>(nameof(IsFullscreen));

    /// <summary>
    /// Defines the <see cref="ShowToolbar"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> ShowToolbarProperty =
        AvaloniaProperty.Register<ImageViewer, bool>(nameof(ShowToolbar), true);

    /// <summary>
    /// Defines the <see cref="ShowThumbnails"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> ShowThumbnailsProperty =
        AvaloniaProperty.Register<ImageViewer, bool>(nameof(ShowThumbnails), true);

    /// <summary>
    /// Defines the <see cref="IsLoading"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsLoadingProperty =
        AvaloniaProperty.Register<ImageViewer, bool>(nameof(IsLoading));

    /// <summary>
    /// Defines the <see cref="HasError"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> HasErrorProperty =
        AvaloniaProperty.Register<ImageViewer, bool>(nameof(HasError));

    static ImageViewer()
    {
        IsFullscreenProperty.Changed.AddClassHandler<ImageViewer>((x, _) => x.UpdatePseudoClasses());
        IsLoadingProperty.Changed.AddClassHandler<ImageViewer>((x, _) => x.UpdatePseudoClasses());
        HasErrorProperty.Changed.AddClassHandler<ImageViewer>((x, _) => x.UpdatePseudoClasses());
        CurrentIndexProperty.Changed.AddClassHandler<ImageViewer>((x, _) => x.OnCurrentIndexChanged());
    }

    /// <summary>
    /// Gets or sets the current image source.
    /// </summary>
    public IImage? Source
    {
        get => GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }

    /// <summary>
    /// Gets or sets the collection of images for gallery mode.
    /// </summary>
    public IList<IImage>? Sources
    {
        get => GetValue(SourcesProperty);
        set => SetValue(SourcesProperty, value);
    }

    /// <summary>
    /// Gets or sets the current image index in gallery mode.
    /// </summary>
    public int CurrentIndex
    {
        get => GetValue(CurrentIndexProperty);
        set => SetValue(CurrentIndexProperty, value);
    }

    /// <summary>
    /// Gets or sets the zoom level (1.0 = 100%).
    /// </summary>
    public double ZoomLevel
    {
        get => GetValue(ZoomLevelProperty);
        set => SetValue(ZoomLevelProperty, Math.Clamp(value, MinZoom, MaxZoom));
    }

    /// <summary>
    /// Gets or sets the minimum zoom level.
    /// </summary>
    public double MinZoom
    {
        get => GetValue(MinZoomProperty);
        set => SetValue(MinZoomProperty, value);
    }

    /// <summary>
    /// Gets or sets the maximum zoom level.
    /// </summary>
    public double MaxZoom
    {
        get => GetValue(MaxZoomProperty);
        set => SetValue(MaxZoomProperty, value);
    }

    /// <summary>
    /// Gets or sets the rotation angle in degrees.
    /// </summary>
    public double Rotation
    {
        get => GetValue(RotationProperty);
        set => SetValue(RotationProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the viewer is in fullscreen mode.
    /// </summary>
    public bool IsFullscreen
    {
        get => GetValue(IsFullscreenProperty);
        set => SetValue(IsFullscreenProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the toolbar is visible.
    /// </summary>
    public bool ShowToolbar
    {
        get => GetValue(ShowToolbarProperty);
        set => SetValue(ShowToolbarProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the thumbnail strip is visible.
    /// </summary>
    public bool ShowThumbnails
    {
        get => GetValue(ShowThumbnailsProperty);
        set => SetValue(ShowThumbnailsProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the image is currently loading.
    /// </summary>
    public bool IsLoading
    {
        get => GetValue(IsLoadingProperty);
        set => SetValue(IsLoadingProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the image failed to load.
    /// </summary>
    public bool HasError
    {
        get => GetValue(HasErrorProperty);
        set => SetValue(HasErrorProperty, value);
    }

    /// <summary>
    /// Occurs when the image changes (navigating in gallery mode).
    /// </summary>
    public event EventHandler<RoutedEventArgs>? ImageChanged;

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _image = e.NameScope.Find<Image>("PART_Image");
        _thumbnailStrip = e.NameScope.Find<ItemsControl>("PART_ThumbnailStrip");
        _toolbar = e.NameScope.Find<Panel>("PART_Toolbar");
        UpdatePseudoClasses();
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        switch (e.Key)
        {
            case Key.Left:
                NavigatePrevious();
                e.Handled = true;
                break;
            case Key.Right:
                NavigateNext();
                e.Handled = true;
                break;
            case Key.Add:
            case Key.OemPlus:
                ZoomIn();
                e.Handled = true;
                break;
            case Key.Subtract:
            case Key.OemMinus:
                ZoomOut();
                e.Handled = true;
                break;
            case Key.R:
                RotateRight();
                e.Handled = true;
                break;
            case Key.F:
                ToggleFullscreen();
                e.Handled = true;
                break;
            case Key.Escape:
                if (IsFullscreen)
                {
                    IsFullscreen = false;
                    e.Handled = true;
                }
                break;
        }
    }

    /// <summary>
    /// Navigates to the previous image in gallery mode.
    /// </summary>
    public void NavigatePrevious()
    {
        var sources = Sources;
        if (sources == null || sources.Count == 0) return;
        CurrentIndex = (CurrentIndex - 1 + sources.Count) % sources.Count;
    }

    /// <summary>
    /// Navigates to the next image in gallery mode.
    /// </summary>
    public void NavigateNext()
    {
        var sources = Sources;
        if (sources == null || sources.Count == 0) return;
        CurrentIndex = (CurrentIndex + 1) % sources.Count;
    }

    /// <summary>
    /// Zooms in by one step.
    /// </summary>
    public void ZoomIn()
    {
        ZoomLevel = Math.Min(ZoomLevel + 0.25, MaxZoom);
    }

    /// <summary>
    /// Zooms out by one step.
    /// </summary>
    public void ZoomOut()
    {
        ZoomLevel = Math.Max(ZoomLevel - 0.25, MinZoom);
    }

    /// <summary>
    /// Rotates the image 90 degrees to the left.
    /// </summary>
    public void RotateLeft()
    {
        Rotation = (Rotation - 90) % 360;
    }

    /// <summary>
    /// Rotates the image 90 degrees to the right.
    /// </summary>
    public void RotateRight()
    {
        Rotation = (Rotation + 90) % 360;
    }

    /// <summary>
    /// Resets zoom and rotation to defaults.
    /// </summary>
    public void Reset()
    {
        ZoomLevel = 1.0;
        Rotation = 0;
    }

    /// <summary>
    /// Toggles fullscreen mode.
    /// </summary>
    public void ToggleFullscreen()
    {
        IsFullscreen = !IsFullscreen;
    }

    private void OnCurrentIndexChanged()
    {
        var sources = Sources;
        if (sources != null && CurrentIndex >= 0 && CurrentIndex < sources.Count)
        {
            Source = sources[CurrentIndex];
            Reset();
        }
        ImageChanged?.Invoke(this, new RoutedEventArgs());
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(":fullscreen", IsFullscreen);
        PseudoClasses.Set(":loading", IsLoading);
        PseudoClasses.Set(":error", HasError);
    }
}
