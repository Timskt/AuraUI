using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Layout;

namespace AuraUI.Controls.Display;

/// <summary>
/// An empty state placeholder control that displays when there is no data,
/// inspired by Ant Design's Empty component. Shows an icon, description text,
/// and an optional action area.
/// </summary>
public class Empty : TemplatedControl
{
    /// <summary>
    /// Defines the <see cref="Description"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> DescriptionProperty =
        AvaloniaProperty.Register<Empty, string?>(nameof(Description), "No Data");

    /// <summary>
    /// Defines the <see cref="ImageSource"/> styled property.
    /// Custom image to display instead of the default empty icon.
    /// </summary>
    public static readonly StyledProperty<Avalonia.Media.IImage?> ImageSourceProperty =
        AvaloniaProperty.Register<Empty, Avalonia.Media.IImage?>(nameof(ImageSource));

    /// <summary>
    /// Defines the <see cref="ImageTemplate"/> styled property.
    /// A custom template for the image/icon area.
    /// </summary>
    public static readonly StyledProperty<IDataTemplate?> ImageTemplateProperty =
        AvaloniaProperty.Register<Empty, IDataTemplate?>(nameof(ImageTemplate));

    /// <summary>
    /// Defines the <see cref="ActionContent"/> styled property.
    /// Content for an action area below the description (e.g., a button).
    /// </summary>
    public static readonly StyledProperty<object?> ActionContentProperty =
        AvaloniaProperty.Register<Empty, object?>(nameof(ActionContent));

    /// <summary>
    /// Defines the <see cref="ImageSize"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> ImageSizeProperty =
        AvaloniaProperty.Register<Empty, double>(nameof(ImageSize), 64);

    /// <summary>
    /// Gets or sets the description text.
    /// </summary>
    public string? Description
    {
        get => GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }

    /// <summary>
    /// Gets or sets a custom image source.
    /// </summary>
    public Avalonia.Media.IImage? ImageSource
    {
        get => GetValue(ImageSourceProperty);
        set => SetValue(ImageSourceProperty, value);
    }

    /// <summary>
    /// Gets or sets a custom image template.
    /// </summary>
    public IDataTemplate? ImageTemplate
    {
        get => GetValue(ImageTemplateProperty);
        set => SetValue(ImageTemplateProperty, value);
    }

    /// <summary>
    /// Gets or sets the action content.
    /// </summary>
    public object? ActionContent
    {
        get => GetValue(ActionContentProperty);
        set => SetValue(ActionContentProperty, value);
    }

    /// <summary>
    /// Gets or sets the size of the image/icon.
    /// </summary>
    public double ImageSize
    {
        get => GetValue(ImageSizeProperty);
        set => SetValue(ImageSizeProperty, value);
    }
}
