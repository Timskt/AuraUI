using System;
using System.Collections;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;

namespace AuraUI.Controls.Display;

/// <summary>
/// Specifies the preview mode for the image gallery.
/// </summary>
public enum ImageGalleryPreviewMode
{
    /// <summary>Opens a lightbox overlay for preview.</summary>
    Lightbox,
    /// <summary>Shows preview inline.</summary>
    Inline
}

/// <summary>
/// An image gallery control displaying a grid of images with click-to-preview functionality.
/// Supports lightbox and inline preview modes with keyboard navigation.
/// Inspired by Element Plus's image gallery.
/// </summary>
[PseudoClasses(":preview-open")]
public class ImageGallery : ItemsControl
{
    private ImageViewer? _viewer;

    /// <summary>
    /// Defines the <see cref="Columns"/> styled property.
    /// </summary>
    public static readonly StyledProperty<int> ColumnsProperty =
        AvaloniaProperty.Register<ImageGallery, int>(nameof(Columns), 3);

    /// <summary>
    /// Defines the <see cref="Spacing"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> SpacingProperty =
        AvaloniaProperty.Register<ImageGallery, double>(nameof(Spacing), 8);

    /// <summary>
    /// Defines the <see cref="ShowPreview"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> ShowPreviewProperty =
        AvaloniaProperty.Register<ImageGallery, bool>(nameof(ShowPreview), true);

    /// <summary>
    /// Defines the <see cref="PreviewMode"/> styled property.
    /// </summary>
    public static readonly StyledProperty<ImageGalleryPreviewMode> PreviewModeProperty =
        AvaloniaProperty.Register<ImageGallery, ImageGalleryPreviewMode>(
            nameof(PreviewMode), ImageGalleryPreviewMode.Lightbox);

    /// <summary>
    /// Defines the <see cref="ImageHeight"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> ImageHeightProperty =
        AvaloniaProperty.Register<ImageGallery, double>(nameof(ImageHeight), 200);

    /// <summary>
    /// Defines the <see cref="IsPreviewOpen"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsPreviewOpenProperty =
        AvaloniaProperty.Register<ImageGallery, bool>(nameof(IsPreviewOpen));

    static ImageGallery()
    {
        IsPreviewOpenProperty.Changed.AddClassHandler<ImageGallery>((x, _) => x.UpdatePseudoClasses());
    }

    /// <summary>
    /// Gets or sets the number of columns in the gallery grid.
    /// </summary>
    public int Columns
    {
        get => GetValue(ColumnsProperty);
        set => SetValue(ColumnsProperty, value);
    }

    /// <summary>
    /// Gets or sets the spacing between images in pixels.
    /// </summary>
    public double Spacing
    {
        get => GetValue(SpacingProperty);
        set => SetValue(SpacingProperty, value);
    }

    /// <summary>
    /// Gets or sets whether clicking an image opens a preview.
    /// </summary>
    public bool ShowPreview
    {
        get => GetValue(ShowPreviewProperty);
        set => SetValue(ShowPreviewProperty, value);
    }

    /// <summary>
    /// Gets or sets the preview mode (lightbox or inline).
    /// </summary>
    public ImageGalleryPreviewMode PreviewMode
    {
        get => GetValue(PreviewModeProperty);
        set => SetValue(PreviewModeProperty, value);
    }

    /// <summary>
    /// Gets or sets the height of each gallery image.
    /// </summary>
    public double ImageHeight
    {
        get => GetValue(ImageHeightProperty);
        set => SetValue(ImageHeightProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the preview is currently open.
    /// </summary>
    public bool IsPreviewOpen
    {
        get => GetValue(IsPreviewOpenProperty);
        set => SetValue(IsPreviewOpenProperty, value);
    }

    /// <summary>
    /// Occurs when the preview is opened.
    /// </summary>
    public event EventHandler<RoutedEventArgs>? PreviewOpened;

    /// <summary>
    /// Occurs when the preview is closed.
    /// </summary>
    public event EventHandler<RoutedEventArgs>? PreviewClosed;

    /// <summary>
    /// Opens the preview for the image at the specified index.
    /// </summary>
    public void OpenPreview(int index)
    {
        if (!ShowPreview) return;
        IsPreviewOpen = true;
        PreviewOpened?.Invoke(this, new RoutedEventArgs());
    }

    /// <summary>
    /// Closes the image preview.
    /// </summary>
    public void ClosePreview()
    {
        IsPreviewOpen = false;
        PreviewClosed?.Invoke(this, new RoutedEventArgs());
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);

        if (!IsPreviewOpen) return;

        switch (e.Key)
        {
            case Key.Left:
                _viewer?.NavigatePrevious();
                e.Handled = true;
                break;
            case Key.Right:
                _viewer?.NavigateNext();
                e.Handled = true;
                break;
            case Key.Escape:
                ClosePreview();
                e.Handled = true;
                break;
        }
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(":preview-open", IsPreviewOpen);
    }
}
