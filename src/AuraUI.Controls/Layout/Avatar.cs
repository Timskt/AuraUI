using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.Media.Imaging;

namespace AuraUI.Controls.Layout;

/// <summary>
/// A versatile avatar control that can display an image, icon, or text fallback.
/// </summary>
[TemplatePart("PART_Image", typeof(Image))]
[TemplatePart("PART_Fallback", typeof(TextBlock))]
[PseudoClasses(":small", ":medium", ":large", ":xl", ":circle", ":square", ":has-image", ":no-image")]
public class Avatar : ContentControl
{
    /// <summary>
    /// Defines the <see cref="Source"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IImage?> SourceProperty =
        AvaloniaProperty.Register<Avatar, IImage?>(nameof(Source));

    /// <summary>
    /// Defines the <see cref="Size"/> styled property.
    /// </summary>
    public static readonly StyledProperty<AvatarSize> SizeProperty =
        AvaloniaProperty.Register<Avatar, AvatarSize>(
            nameof(Size),
            AvatarSize.Medium);

    /// <summary>
    /// Defines the <see cref="Shape"/> styled property.
    /// </summary>
    public static readonly StyledProperty<AvatarShape> ShapeProperty =
        AvaloniaProperty.Register<Avatar, AvatarShape>(
            nameof(Shape),
            AvatarShape.Circle);

    /// <summary>
    /// Defines the <see cref="FallbackText"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> FallbackTextProperty =
        AvaloniaProperty.Register<Avatar, string?>(nameof(FallbackText));

    /// <summary>
    /// Defines the <see cref="FallbackBackground"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> FallbackBackgroundProperty =
        AvaloniaProperty.Register<Avatar, IBrush?>(nameof(FallbackBackground));

    private Image? _image;
    private TextBlock? _fallback;

    static Avatar()
    {
        SourceProperty.Changed.AddClassHandler<Avatar>((x, _) => x.UpdateVisualState());
        SizeProperty.Changed.AddClassHandler<Avatar>((x, _) => x.UpdatePseudoClasses());
        ShapeProperty.Changed.AddClassHandler<Avatar>((x, _) => x.UpdatePseudoClasses());
        FallbackTextProperty.Changed.AddClassHandler<Avatar>((x, _) => x.UpdateVisualState());
    }

    /// <summary>
    /// Gets or sets the image source for the avatar.
    /// </summary>
    public IImage? Source
    {
        get => GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }

    /// <summary>
    /// Gets or sets the size of the avatar.
    /// </summary>
    public AvatarSize Size
    {
        get => GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }

    /// <summary>
    /// Gets or sets the shape of the avatar.
    /// </summary>
    public AvatarShape Shape
    {
        get => GetValue(ShapeProperty);
        set => SetValue(ShapeProperty, value);
    }

    /// <summary>
    /// Gets or sets the fallback text (typically 1-2 initials) shown when no image is available.
    /// </summary>
    public string? FallbackText
    {
        get => GetValue(FallbackTextProperty);
        set => SetValue(FallbackTextProperty, value);
    }

    /// <summary>
    /// Gets or sets the background brush used for the fallback text area.
    /// </summary>
    public IBrush? FallbackBackground
    {
        get => GetValue(FallbackBackgroundProperty);
        set => SetValue(FallbackBackgroundProperty, value);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        _image = e.NameScope.Find<Image>("PART_Image");
        _fallback = e.NameScope.Find<TextBlock>("PART_Fallback");

        UpdatePseudoClasses();
        UpdateVisualState();
    }

    private void UpdatePseudoClasses()
    {
        // Size classes
        PseudoClasses.Set(":small", Size == AvatarSize.Small);
        PseudoClasses.Set(":medium", Size == AvatarSize.Medium);
        PseudoClasses.Set(":large", Size == AvatarSize.Large);
        PseudoClasses.Set(":xl", Size == AvatarSize.XL);

        // Shape classes
        PseudoClasses.Set(":circle", Shape == AvatarShape.Circle);
        PseudoClasses.Set(":square", Shape == AvatarShape.Square);
    }

    private void UpdateVisualState()
    {
        var hasImage = Source != null;

        PseudoClasses.Set(":has-image", hasImage);
        PseudoClasses.Set(":no-image", !hasImage);

        if (_image != null)
        {
            _image.Source = Source;
            _image.IsVisible = hasImage;
        }

        if (_fallback != null)
        {
            _fallback.IsVisible = !hasImage;

            if (!hasImage)
            {
                // Derive fallback text from FallbackText or Content if it is a string.
                var text = FallbackText;
                if (string.IsNullOrEmpty(text) && Content is string contentStr)
                {
                    text = GetInitials(contentStr);
                }

                _fallback.Text = text;
            }
        }
    }

    /// <summary>
    /// Extracts up to 2 initials from a name string.
    /// </summary>
    private static string GetInitials(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return string.Empty;

        var parts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0)
            return string.Empty;

        if (parts.Length == 1)
        {
            return parts[0].Length >= 2
                ? parts[0].Substring(0, 2).ToUpperInvariant()
                : parts[0].ToUpperInvariant();
        }

        // First letter of first word + first letter of last word.
        var first = parts[0][0];
        var last = parts[^1][0];
        return $"{first}{last}".ToUpperInvariant();
    }
}
