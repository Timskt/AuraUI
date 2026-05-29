using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;

namespace AuraUI.Controls.Selection;

/// <summary>
/// Specifies the size of the segmented control.
/// </summary>
public enum SegmentedSize
{
    Small,
    Medium,
    Large
}

/// <summary>
/// A segmented button group control similar to iOS UISegmentedControl.
/// Displays a row of options with a sliding selection indicator.
/// Inspired by Ant Design's Segmented component.
/// </summary>
[TemplatePart("PART_Indicator", typeof(Border))]
[PseudoClasses(":small", ":medium", ":large", ":has-selection")]
public class Segmented : ItemsControl
{
    private Border? _indicator;

    /// <summary>
    /// Defines the <see cref="SelectedIndex"/> styled property.
    /// </summary>
    public static readonly StyledProperty<int> SelectedIndexProperty =
        AvaloniaProperty.Register<Segmented, int>(nameof(SelectedIndex));

    /// <summary>
    /// Defines the <see cref="SegmentedSize"/> styled property.
    /// </summary>
    public static readonly StyledProperty<SegmentedSize> SizeProperty =
        AvaloniaProperty.Register<Segmented, SegmentedSize>(nameof(Size), SegmentedSize.Medium);

    /// <summary>
    /// Defines the <see cref="IsAnimated"/> styled property.
    /// Whether the indicator animates when switching segments.
    /// </summary>
    public static readonly StyledProperty<bool> IsAnimatedProperty =
        AvaloniaProperty.Register<Segmented, bool>(nameof(IsAnimated), true);

    /// <summary>
    /// Defines the <see cref="IsBlock"/> styled property.
    /// Whether the segmented control fills its container width.
    /// </summary>
    public static readonly StyledProperty<bool> IsBlockProperty =
        AvaloniaProperty.Register<Segmented, bool>(nameof(IsBlock));

    /// <summary>
    /// Defines the <see cref="IsReadOnly"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsReadOnlyProperty =
        AvaloniaProperty.Register<Segmented, bool>(nameof(IsReadOnly));

    static Segmented()
    {
        SelectedIndexProperty.Changed.AddClassHandler<Segmented>((x, _) => x.OnSelectedIndexChanged());
        SizeProperty.Changed.AddClassHandler<Segmented>((x, _) => x.UpdatePseudoClasses());
    }

    /// <summary>
    /// Gets or sets the selected segment index.
    /// </summary>
    public int SelectedIndex
    {
        get => GetValue(SelectedIndexProperty);
        set => SetValue(SelectedIndexProperty, value);
    }

    /// <summary>
    /// Gets or sets the size of the segmented control.
    /// </summary>
    public SegmentedSize Size
    {
        get => GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the selection indicator animates.
    /// </summary>
    public bool IsAnimated
    {
        get => GetValue(IsAnimatedProperty);
        set => SetValue(IsAnimatedProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the control fills container width.
    /// </summary>
    public bool IsBlock
    {
        get => GetValue(IsBlockProperty);
        set => SetValue(IsBlockProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the control is read-only.
    /// </summary>
    public bool IsReadOnly
    {
        get => GetValue(IsReadOnlyProperty);
        set => SetValue(IsReadOnlyProperty, value);
    }

    /// <summary>
    /// Gets the currently selected item.
    /// </summary>
    public object? SelectedItem
    {
        get
        {
            var index = SelectedIndex;
            var items = Items;
            if (index >= 0 && index < items.Count)
                return items[index];
            return null;
        }
    }

    /// <summary>
    /// Occurs when the selected segment changes.
    /// </summary>
    public event EventHandler<SelectionChangedEventArgs>? SelectionChanged;

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _indicator = e.NameScope.Find<Border>("PART_Indicator");
        UpdatePseudoClasses();
        UpdateIndicatorPosition();
    }

    protected override void ContainerForItemPreparedOverride(Control container, object? item, int index)
    {
        base.ContainerForItemPreparedOverride(container, item, index);

        // Add click handler to segment items
        if (container is Control control)
        {
            control.Tapped += (_, _) =>
            {
                if (!IsReadOnly)
                {
                    SelectedIndex = index;
                }
            };
        }
    }

    private void OnSelectedIndexChanged()
    {
        UpdateIndicatorPosition();
        UpdatePseudoClasses();
        SelectionChanged?.Invoke(this, new SelectionChangedEventArgs(
            Avalonia.Controls.Primitives.SelectingItemsControl.SelectionChangedEvent,
            new System.Collections.ArrayList(),
            new System.Collections.ArrayList { SelectedItem }));
    }

    private void UpdateIndicatorPosition()
    {
        // The actual positioning would be handled by the template/binding.
        // This provides the index for template use.
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(":small", Size == SegmentedSize.Small);
        PseudoClasses.Set(":medium", Size == SegmentedSize.Medium);
        PseudoClasses.Set(":large", Size == SegmentedSize.Large);
        PseudoClasses.Set(":has-selection", SelectedIndex >= 0);
    }
}
