using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Layout;

namespace AuraUI.Controls.Display;

/// <summary>
/// Specifies the layout mode for <see cref="Descriptions"/>.
/// </summary>
public enum DescriptionsLayout
{
    /// <summary>Label and value are displayed horizontally.</summary>
    Horizontal,
    /// <summary>Label is above the value.</summary>
    Vertical
}

/// <summary>
/// A single item in a <see cref="Descriptions"/> control.
/// </summary>
public class DescriptionsItem : AvaloniaObject
{
    /// <summary>
    /// Defines the <see cref="Label"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> LabelProperty =
        AvaloniaProperty.Register<DescriptionsItem, string?>(nameof(Label));

    /// <summary>
    /// Defines the <see cref="Value"/> styled property.
    /// </summary>
    public static readonly StyledProperty<object?> ValueProperty =
        AvaloniaProperty.Register<DescriptionsItem, object?>(nameof(Value));

    /// <summary>
    /// Defines the <see cref="Span"/> styled property.
    /// How many columns this item spans.
    /// </summary>
    public static readonly StyledProperty<int> SpanProperty =
        AvaloniaProperty.Register<DescriptionsItem, int>(nameof(Span), 1);

    /// <summary>
    /// Defines the <see cref="ValueTemplate"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IDataTemplate?> ValueTemplateProperty =
        AvaloniaProperty.Register<DescriptionsItem, IDataTemplate?>(nameof(ValueTemplate));

    /// <summary>
    /// Gets or sets the label text.
    /// </summary>
    public string? Label
    {
        get => GetValue(LabelProperty);
        set => SetValue(LabelProperty, value);
    }

    /// <summary>
    /// Gets or sets the value content.
    /// </summary>
    public object? Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    /// <summary>
    /// Gets or sets the number of columns this item spans.
    /// </summary>
    public int Span
    {
        get => GetValue(SpanProperty);
        set => SetValue(SpanProperty, value);
    }

    /// <summary>
    /// Gets or sets the data template for the value.
    /// </summary>
    public IDataTemplate? ValueTemplate
    {
        get => GetValue(ValueTemplateProperty);
        set => SetValue(ValueTemplateProperty, value);
    }
}

/// <summary>
/// Displays a list of key-value pairs in a structured grid layout,
/// inspired by Ant Design's Descriptions component.
/// </summary>
public class Descriptions : ItemsControl
{
    /// <summary>
    /// Defines the <see cref="Title"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> TitleProperty =
        AvaloniaProperty.Register<Descriptions, string?>(nameof(Title));

    /// <summary>
    /// Defines the <see cref="ColumnCount"/> styled property.
    /// The number of columns in the grid.
    /// </summary>
    public static readonly StyledProperty<int> ColumnCountProperty =
        AvaloniaProperty.Register<Descriptions, int>(nameof(ColumnCount), 3);

    /// <summary>
    /// Defines the <see cref="Layout"/> styled property.
    /// </summary>
    public static readonly StyledProperty<DescriptionsLayout> LayoutProperty =
        AvaloniaProperty.Register<Descriptions, DescriptionsLayout>(nameof(Layout), DescriptionsLayout.Horizontal);

    /// <summary>
    /// Defines the <see cref="Bordered"/> styled property.
    /// Whether to show borders around items.
    /// </summary>
    public static readonly StyledProperty<bool> BorderedProperty =
        AvaloniaProperty.Register<Descriptions, bool>(nameof(Bordered), true);

    /// <summary>
    /// Defines the <see cref="LabelWidth"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> LabelWidthProperty =
        AvaloniaProperty.Register<Descriptions, double>(nameof(LabelWidth), 120);

    /// <summary>
    /// Defines the <see cref="Compact"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> CompactProperty =
        AvaloniaProperty.Register<Descriptions, bool>(nameof(Compact));

    /// <summary>
    /// Gets or sets the title displayed at the top.
    /// </summary>
    public string? Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    /// <summary>
    /// Gets or sets the number of columns.
    /// </summary>
    public int ColumnCount
    {
        get => GetValue(ColumnCountProperty);
        set => SetValue(ColumnCountProperty, value);
    }

    /// <summary>
    /// Gets or sets the layout direction for label/value pairs.
    /// </summary>
    public DescriptionsLayout Layout
    {
        get => GetValue(LayoutProperty);
        set => SetValue(LayoutProperty, value);
    }

    /// <summary>
    /// Gets or sets whether borders are shown around items.
    /// </summary>
    public bool Bordered
    {
        get => GetValue(BorderedProperty);
        set => SetValue(BorderedProperty, value);
    }

    /// <summary>
    /// Gets or sets the label column width.
    /// </summary>
    public double LabelWidth
    {
        get => GetValue(LabelWidthProperty);
        set => SetValue(LabelWidthProperty, value);
    }

    /// <summary>
    /// Gets or sets whether to use compact spacing.
    /// </summary>
    public bool Compact
    {
        get => GetValue(CompactProperty);
        set => SetValue(CompactProperty, value);
    }
}
