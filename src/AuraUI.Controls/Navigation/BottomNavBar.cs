using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Media;

namespace AuraUI.Controls.Navigation;

/// <summary>
/// A mobile-style bottom navigation bar with icon + label per item,
/// smooth active indicator animation, and support for 3-5 navigation items.
/// </summary>
[PseudoClasses(":no-labels", ":with-labels")]
public class BottomNavBar : ItemsControl
{
    /// <summary>
    /// Defines the <see cref="SelectedIndex"/> styled property.
    /// </summary>
    public static readonly StyledProperty<int> SelectedIndexProperty =
        AvaloniaProperty.Register<BottomNavBar, int>(nameof(SelectedIndex));

    /// <summary>
    /// Defines the <see cref="BarBackground"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> BarBackgroundProperty =
        AvaloniaProperty.Register<BottomNavBar, IBrush?>(nameof(BarBackground));

    /// <summary>
    /// Defines the <see cref="ActiveColor"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> ActiveColorProperty =
        AvaloniaProperty.Register<BottomNavBar, IBrush?>(nameof(ActiveColor));

    /// <summary>
    /// Defines the <see cref="InactiveColor"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> InactiveColorProperty =
        AvaloniaProperty.Register<BottomNavBar, IBrush?>(nameof(InactiveColor));

    /// <summary>
    /// Defines the <see cref="ShowLabels"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> ShowLabelsProperty =
        AvaloniaProperty.Register<BottomNavBar, bool>(nameof(ShowLabels), true);

    /// <summary>
    /// Defines the <see cref="IconSize"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> IconSizeProperty =
        AvaloniaProperty.Register<BottomNavBar, double>(nameof(IconSize), 24);

    /// <summary>
    /// Defines the <see cref="BarPadding"/> styled property.
    /// </summary>
    public static readonly StyledProperty<Thickness> BarPaddingProperty =
        AvaloniaProperty.Register<BottomNavBar, Thickness>(nameof(BarPadding), new Thickness(0, 4));

    /// <summary>
    /// Defines the <see cref="ItemSpacing"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> ItemSpacingProperty =
        AvaloniaProperty.Register<BottomNavBar, double>(nameof(ItemSpacing));

    /// <summary>
    /// Defines the <see cref="Elevation"/> styled property.
    /// Shadow/elevation height in device-independent pixels.
    /// </summary>
    public static readonly StyledProperty<double> ElevationProperty =
        AvaloniaProperty.Register<BottomNavBar, double>(nameof(Elevation), 4);

    static BottomNavBar()
    {
        SelectedIndexProperty.Changed.AddClassHandler<BottomNavBar>((x, _) => x.UpdatePseudoClasses());
        ShowLabelsProperty.Changed.AddClassHandler<BottomNavBar>((x, _) => x.UpdatePseudoClasses());
    }

    /// <summary>
    /// Gets or sets the index of the currently selected navigation item.
    /// </summary>
    public int SelectedIndex
    {
        get => GetValue(SelectedIndexProperty);
        set => SetValue(SelectedIndexProperty, value);
    }

    /// <summary>
    /// Gets or sets the background brush of the navigation bar.
    /// </summary>
    public IBrush? BarBackground
    {
        get => GetValue(BarBackgroundProperty);
        set => SetValue(BarBackgroundProperty, value);
    }

    /// <summary>
    /// Gets or sets the color for the active/selected item.
    /// </summary>
    public IBrush? ActiveColor
    {
        get => GetValue(ActiveColorProperty);
        set => SetValue(ActiveColorProperty, value);
    }

    /// <summary>
    /// Gets or sets the color for inactive items.
    /// </summary>
    public IBrush? InactiveColor
    {
        get => GetValue(InactiveColorProperty);
        set => SetValue(InactiveColorProperty, value);
    }

    /// <summary>
    /// Gets or sets whether text labels are shown below icons.
    /// </summary>
    public bool ShowLabels
    {
        get => GetValue(ShowLabelsProperty);
        set => SetValue(ShowLabelsProperty, value);
    }

    /// <summary>
    /// Gets or sets the icon size in pixels.
    /// </summary>
    public double IconSize
    {
        get => GetValue(IconSizeProperty);
        set => SetValue(IconSizeProperty, value);
    }

    /// <summary>
    /// Gets or sets the padding inside the bar.
    /// </summary>
    public Thickness BarPadding
    {
        get => GetValue(BarPaddingProperty);
        set => SetValue(BarPaddingProperty, value);
    }

    /// <summary>
    /// Gets or sets the spacing between items.
    /// </summary>
    public double ItemSpacing
    {
        get => GetValue(ItemSpacingProperty);
        set => SetValue(ItemSpacingProperty, value);
    }

    /// <summary>
    /// Gets or sets the elevation/shadow height.
    /// </summary>
    public double Elevation
    {
        get => GetValue(ElevationProperty);
        set => SetValue(ElevationProperty, value);
    }

    protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
    {
        return new BottomNavBarItem();
    }

    protected override bool NeedsContainerOverride(object? item, int index, out object? recycleKey)
    {
        return NeedsContainer<BottomNavBarItem>(item, out recycleKey);
    }

    protected override void PrepareContainerForItemOverride(Control container, object? item, int index)
    {
        base.PrepareContainerForItemOverride(container, item, index);
        if (container is BottomNavBarItem navItem)
        {
            navItem.IsActive = index == SelectedIndex;
            navItem.IconSize = IconSize;
            navItem.ShowLabel = ShowLabels;
        }
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        UpdatePseudoClasses();
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(":no-labels", !ShowLabels);
        PseudoClasses.Set(":with-labels", ShowLabels);

        // Update active state on existing containers
        var items = Items;
        if (items == null) return;
        for (int i = 0; i < items.Count; i++)
        {
            var container = ContainerFromIndex(i);
            if (container is BottomNavBarItem navItem)
            {
                navItem.IsActive = i == SelectedIndex;
            }
        }
    }
}

/// <summary>
/// Represents a single item in the bottom navigation bar.
/// </summary>
[PseudoClasses(":active", ":inactive")]
public class BottomNavBarItem : ContentControl
{
    /// <summary>
    /// Defines the <see cref="Icon"/> styled property.
    /// </summary>
    public static readonly StyledProperty<object?> IconProperty =
        AvaloniaProperty.Register<BottomNavBarItem, object?>(nameof(Icon));

    /// <summary>
    /// Defines the <see cref="Label"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> LabelProperty =
        AvaloniaProperty.Register<BottomNavBarItem, string?>(nameof(Label));

    /// <summary>
    /// Defines the <see cref="IsActive"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsActiveProperty =
        AvaloniaProperty.Register<BottomNavBarItem, bool>(nameof(IsActive));

    /// <summary>
    /// Defines the <see cref="IconSize"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> IconSizeProperty =
        AvaloniaProperty.Register<BottomNavBarItem, double>(nameof(IconSize), 24);

    /// <summary>
    /// Defines the <see cref="ShowLabel"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> ShowLabelProperty =
        AvaloniaProperty.Register<BottomNavBarItem, bool>(nameof(ShowLabel), true);

    /// <summary>
    /// Defines the <see cref="BadgeCount"/> styled property.
    /// </summary>
    public static readonly StyledProperty<int?> BadgeCountProperty =
        AvaloniaProperty.Register<BottomNavBarItem, int?>(nameof(BadgeCount));

    static BottomNavBarItem()
    {
        IsActiveProperty.Changed.AddClassHandler<BottomNavBarItem>((x, _) => x.UpdatePseudoClasses());
    }

    /// <summary>
    /// Gets or sets the icon content.
    /// </summary>
    public object? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    /// <summary>
    /// Gets or sets the label text.
    /// </summary>
    public string? Label
    {
        get => GetValue(LabelProperty);
        set => SetValue(LabelProperty, value);
    }

    /// <summary>
    /// Gets or sets whether this item is currently active/selected.
    /// </summary>
    public bool IsActive
    {
        get => GetValue(IsActiveProperty);
        set => SetValue(IsActiveProperty, value);
    }

    /// <summary>
    /// Gets or sets the icon size.
    /// </summary>
    public double IconSize
    {
        get => GetValue(IconSizeProperty);
        set => SetValue(IconSizeProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the label is shown.
    /// </summary>
    public bool ShowLabel
    {
        get => GetValue(ShowLabelProperty);
        set => SetValue(ShowLabelProperty, value);
    }

    /// <summary>
    /// Gets or sets the badge count. Null means no badge.
    /// </summary>
    public int? BadgeCount
    {
        get => GetValue(BadgeCountProperty);
        set => SetValue(BadgeCountProperty, value);
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        UpdatePseudoClasses();
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(":active", IsActive);
        PseudoClasses.Set(":inactive", !IsActive);
    }
}
