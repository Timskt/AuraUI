using Avalonia;
using Avalonia.Controls;

namespace AuraUI.Controls.Layout;

/// <summary>
/// A layout panel that arranges <see cref="FormField"/> children in a form layout.
/// Supports uniform label width across sibling fields and configurable spacing.
/// </summary>
public class FormPanel : StackPanel
{
    /// <summary>
    /// Defines the <see cref="LabelWidth"/> styled property.
    /// When set, all child <see cref="FormField"/> controls will use this label width.
    /// </summary>
    public static readonly StyledProperty<double> LabelWidthProperty =
        AvaloniaProperty.Register<FormPanel, double>(nameof(LabelWidth), 120);

    /// <summary>
    /// Defines the <see cref="FieldSpacing"/> styled property.
    /// The vertical spacing between form field rows.
    /// </summary>
    public static readonly StyledProperty<double> FieldSpacingProperty =
        AvaloniaProperty.Register<FormPanel, double>(nameof(FieldSpacing), 16);

    /// <summary>
    /// Defines the <see cref="RowSpacing"/> styled property.
    /// Alias for FieldSpacing; spacing between rows of fields.
    /// </summary>
    public static readonly StyledProperty<double> RowSpacingProperty =
        AvaloniaProperty.Register<FormPanel, double>(nameof(RowSpacing), 16);

    static FormPanel()
    {
        LabelWidthProperty.Changed.AddClassHandler<FormPanel>((x, e) => x.OnLabelWidthChanged());
        FieldSpacingProperty.Changed.AddClassHandler<FormPanel>((x, e) => x.OnSpacingChanged());
        RowSpacingProperty.Changed.AddClassHandler<FormPanel>((x, e) => x.OnSpacingChanged());
    }

    /// <summary>
    /// Gets or sets the default label width applied to all child <see cref="FormField"/> controls.
    /// </summary>
    public double LabelWidth
    {
        get => GetValue(LabelWidthProperty);
        set => SetValue(LabelWidthProperty, value);
    }

    /// <summary>
    /// Gets or sets the vertical spacing between form fields.
    /// </summary>
    public double FieldSpacing
    {
        get => GetValue(FieldSpacingProperty);
        set => SetValue(FieldSpacingProperty, value);
    }

    /// <summary>
    /// Gets or sets the row spacing (alias for FieldSpacing).
    /// </summary>
    public double RowSpacing
    {
        get => GetValue(RowSpacingProperty);
        set => SetValue(RowSpacingProperty, value);
    }

    protected override void ChildrenChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        base.ChildrenChanged(sender, e);
        ApplyLabelWidthToChildren();
    }

    private void OnLabelWidthChanged()
    {
        ApplyLabelWidthToChildren();
    }

    private void OnSpacingChanged()
    {
        // Use the larger of FieldSpacing and RowSpacing
        var spacing = Math.Max(FieldSpacing, RowSpacing);
        Spacing = spacing;
    }

    private void ApplyLabelWidthToChildren()
    {
        var width = LabelWidth;
        foreach (var child in Children)
        {
            if (child is FormField formField)
            {
                formField.LabelWidth = width;
                formField.LabelPlacement = FormFieldLabelPlacement.Left;
            }
        }
    }
}
