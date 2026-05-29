using Avalonia;
using Avalonia.Controls;

namespace AuraUI.Controls.Layout;

/// <summary>
/// A row in the 24-column grid system, inspired by Ant Design's Row.
/// Works with <see cref="AuraCol"/> children to create responsive layouts.
/// </summary>
public class AuraRow : StackPanel
{
    /// <summary>
    /// Defines the <see cref="Gutter"/> styled property.
    /// Horizontal gutter between columns (applied as half-margin on each side).
    /// </summary>
    public static readonly StyledProperty<double> GutterProperty =
        AvaloniaProperty.Register<AuraRow, double>(nameof(Gutter));

    /// <summary>
    /// Defines the <see cref="VerticalGutter"/> styled property.
    /// Vertical gutter between rows when wrapping.
    /// </summary>
    public static readonly StyledProperty<double> VerticalGutterProperty =
        AvaloniaProperty.Register<AuraRow, double>(nameof(VerticalGutter));

    /// <summary>
    /// Defines the <see cref="Justify"/> styled property.
    /// Controls horizontal distribution of columns.
    /// </summary>
    public static readonly StyledProperty<RowJustify> JustifyProperty =
        AvaloniaProperty.Register<AuraRow, RowJustify>(nameof(Justify), RowJustify.Start);

    /// <summary>
    /// Defines the <see cref="RowAlign"/> styled property.
    /// Controls vertical alignment of columns within the row.
    /// </summary>
    public static readonly StyledProperty<RowAlign> RowAlignProperty =
        AvaloniaProperty.Register<AuraRow, RowAlign>(nameof(RowAlign), RowAlign.Top);

    /// <summary>
    /// Defines the <see cref="Wrap"/> styled property.
    /// Whether columns wrap to the next line.
    /// </summary>
    public static readonly StyledProperty<bool> WrapProperty =
        AvaloniaProperty.Register<AuraRow, bool>(nameof(Wrap), true);

    static AuraRow()
    {
        GutterProperty.Changed.AddClassHandler<AuraRow>((x, _) => x.ApplyGutter());
    }

    /// <summary>
    /// Gets or sets the horizontal gutter (spacing) between columns in pixels.
    /// </summary>
    public double Gutter
    {
        get => GetValue(GutterProperty);
        set => SetValue(GutterProperty, value);
    }

    /// <summary>
    /// Gets or sets the vertical gutter between wrapped rows in pixels.
    /// </summary>
    public double VerticalGutter
    {
        get => GetValue(VerticalGutterProperty);
        set => SetValue(VerticalGutterProperty, value);
    }

    /// <summary>
    /// Gets or sets the horizontal distribution of columns.
    /// </summary>
    public RowJustify Justify
    {
        get => GetValue(JustifyProperty);
        set => SetValue(JustifyProperty, value);
    }

    /// <summary>
    /// Gets or sets the vertical alignment of columns.
    /// </summary>
    public RowAlign RowAlign
    {
        get => GetValue(RowAlignProperty);
        set => SetValue(RowAlignProperty, value);
    }

    /// <summary>
    /// Gets or sets whether columns wrap to the next line.
    /// </summary>
    public new bool Wrap
    {
        get => GetValue(WrapProperty);
        set => SetValue(WrapProperty, value);
    }

    protected override void ChildrenChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        base.ChildrenChanged(sender, e);
        ApplyGutter();
    }

    private void ApplyGutter()
    {
        var gutter = Gutter;
        var halfGutter = gutter / 2;

        foreach (var child in Children)
        {
            if (child is AuraCol col)
            {
                col.Margin = new Thickness(halfGutter, 0, halfGutter, 0);
                col.VerticalAlignment = RowAlign switch
                {
                    RowAlign.Top => Avalonia.Layout.VerticalAlignment.Top,
                    RowAlign.Middle => Avalonia.Layout.VerticalAlignment.Center,
                    RowAlign.Bottom => Avalonia.Layout.VerticalAlignment.Bottom,
                    RowAlign.Stretch => Avalonia.Layout.VerticalAlignment.Stretch,
                    _ => Avalonia.Layout.VerticalAlignment.Top
                };
            }
        }

        // Apply negative margin on the row to compensate for the half-gutter
        Margin = new Thickness(-halfGutter, 0, -halfGutter, 0);
    }
}

/// <summary>
/// Horizontal distribution of columns in a row.
/// </summary>
public enum RowJustify
{
    /// <summary>Columns are packed at the start.</summary>
    Start,
    /// <summary>Columns are centered.</summary>
    Center,
    /// <summary>Columns are packed at the end.</summary>
    End,
    /// <summary>Space is evenly distributed between columns.</summary>
    SpaceBetween,
    /// <summary>Space is evenly distributed around columns.</summary>
    SpaceAround,
    /// <summary>Space is evenly distributed including before the first and after the last column.</summary>
    SpaceEvenly
}

/// <summary>
/// Vertical alignment of columns within a row.
/// </summary>
public enum RowAlign
{
    /// <summary>Columns are aligned to the top.</summary>
    Top,
    /// <summary>Columns are vertically centered.</summary>
    Middle,
    /// <summary>Columns are aligned to the bottom.</summary>
    Bottom,
    /// <summary>Columns are stretched to fill the row height.</summary>
    Stretch
}
