using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Media;

namespace AuraUI.Controls.Display;

/// <summary>
/// Specifies the type of a tree view node.
/// </summary>
public enum TreeNodeType
{
    Leaf,
    Branch
}

/// <summary>
/// A customized TreeViewItem that supports icon display, expand/collapse icons,
/// leaf icons, and tree lines.
/// </summary>
[PseudoClasses(":selected", ":expanded", ":leaf", ":branch")]
public class AuraTreeViewItem : TreeViewItem
{
    /// <summary>
    /// Defines the <see cref="Icon"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IImage?> IconProperty =
        AvaloniaProperty.Register<AuraTreeViewItem, IImage?>(nameof(Icon));

    /// <summary>
    /// Defines the <see cref="NodeType"/> styled property.
    /// </summary>
    public static readonly StyledProperty<TreeNodeType> NodeTypeProperty =
        AvaloniaProperty.Register<AuraTreeViewItem, TreeNodeType>(nameof(NodeType));

    static AuraTreeViewItem()
    {
        IsSelectedProperty.Changed.AddClassHandler<AuraTreeViewItem>((x, _) => x.UpdatePseudoClasses());
        IsExpandedProperty.Changed.AddClassHandler<AuraTreeViewItem>((x, _) => x.UpdatePseudoClasses());
        NodeTypeProperty.Changed.AddClassHandler<AuraTreeViewItem>((x, _) => x.UpdatePseudoClasses());
    }

    /// <summary>
    /// Gets or sets the icon displayed for this tree node.
    /// </summary>
    public IImage? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    /// <summary>
    /// Gets or sets whether this node is a leaf or branch.
    /// </summary>
    public TreeNodeType NodeType
    {
        get => GetValue(NodeTypeProperty);
        set => SetValue(NodeTypeProperty, value);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        UpdatePseudoClasses();
    }

    protected override void PrepareContainerForItemOverride(Control container, object? item, int index)
    {
        base.PrepareContainerForItemOverride(container, item, index);
        if (container is AuraTreeViewItem treeItem)
        {
            treeItem.Classes.Add("auratreeitem");
        }
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(":selected", IsSelected);
        PseudoClasses.Set(":expanded", IsExpanded);
        PseudoClasses.Set(":leaf", NodeType == TreeNodeType.Leaf);
        PseudoClasses.Set(":branch", NodeType == TreeNodeType.Branch);
    }
}

/// <summary>
/// An enhanced TreeView with customizable item spacing, corner radius, hover/selected backgrounds,
/// indentation, icon support for expand/collapse/leaf nodes, and optional tree lines.
/// </summary>
[TemplatePart("PART_ScrollViewer", typeof(ScrollViewer))]
public class AuraTreeView : TreeView
{
    /// <summary>
    /// Defines the <see cref="ItemSpacing"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> ItemSpacingProperty =
        AvaloniaProperty.Register<AuraTreeView, double>(nameof(ItemSpacing), 2);

    /// <summary>
    /// Defines the <see cref="ItemCornerRadius"/> styled property.
    /// </summary>
    public static readonly StyledProperty<CornerRadius> ItemCornerRadiusProperty =
        AvaloniaProperty.Register<AuraTreeView, CornerRadius>(nameof(ItemCornerRadius), new CornerRadius(4));

    /// <summary>
    /// Defines the <see cref="ItemHoverBackground"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> ItemHoverBackgroundProperty =
        AvaloniaProperty.Register<AuraTreeView, IBrush?>(nameof(ItemHoverBackground));

    /// <summary>
    /// Defines the <see cref="ItemSelectedBackground"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> ItemSelectedBackgroundProperty =
        AvaloniaProperty.Register<AuraTreeView, IBrush?>(nameof(ItemSelectedBackground));

    /// <summary>
    /// Defines the <see cref="IndentSize"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> IndentSizeProperty =
        AvaloniaProperty.Register<AuraTreeView, double>(nameof(IndentSize), 20);

    /// <summary>
    /// Defines the <see cref="ExpandIcon"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> ExpandIconProperty =
        AvaloniaProperty.Register<AuraTreeView, string?>(nameof(ExpandIcon));

    /// <summary>
    /// Defines the <see cref="CollapseIcon"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> CollapseIconProperty =
        AvaloniaProperty.Register<AuraTreeView, string?>(nameof(CollapseIcon));

    /// <summary>
    /// Defines the <see cref="LeafIcon"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> LeafIconProperty =
        AvaloniaProperty.Register<AuraTreeView, string?>(nameof(LeafIcon));

    /// <summary>
    /// Defines the <see cref="ShowLines"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> ShowLinesProperty =
        AvaloniaProperty.Register<AuraTreeView, bool>(nameof(ShowLines));

    /// <summary>
    /// Defines the <see cref="LineBrush"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> LineBrushProperty =
        AvaloniaProperty.Register<AuraTreeView, IBrush?>(nameof(LineBrush));

    static AuraTreeView()
    {
        ItemSpacingProperty.Changed.AddClassHandler<AuraTreeView>((x, _) => x.InvalidateMeasure());
        IndentSizeProperty.Changed.AddClassHandler<AuraTreeView>((x, _) => x.InvalidateMeasure());
    }

    /// <summary>
    /// Gets or sets the vertical spacing between tree items.
    /// </summary>
    public double ItemSpacing
    {
        get => GetValue(ItemSpacingProperty);
        set => SetValue(ItemSpacingProperty, value);
    }

    /// <summary>
    /// Gets or sets the corner radius for tree items.
    /// </summary>
    public CornerRadius ItemCornerRadius
    {
        get => GetValue(ItemCornerRadiusProperty);
        set => SetValue(ItemCornerRadiusProperty, value);
    }

    /// <summary>
    /// Gets or sets the background brush when hovering over a tree item.
    /// </summary>
    public IBrush? ItemHoverBackground
    {
        get => GetValue(ItemHoverBackgroundProperty);
        set => SetValue(ItemHoverBackgroundProperty, value);
    }

    /// <summary>
    /// Gets or sets the background brush for selected tree items.
    /// </summary>
    public IBrush? ItemSelectedBackground
    {
        get => GetValue(ItemSelectedBackgroundProperty);
        set => SetValue(ItemSelectedBackgroundProperty, value);
    }

    /// <summary>
    /// Gets or sets the indentation size for nested tree levels in pixels.
    /// </summary>
    public double IndentSize
    {
        get => GetValue(IndentSizeProperty);
        set => SetValue(IndentSizeProperty, value);
    }

    /// <summary>
    /// Gets or sets the icon geometry or path data for expanded branch nodes.
    /// </summary>
    public string? ExpandIcon
    {
        get => GetValue(ExpandIconProperty);
        set => SetValue(ExpandIconProperty, value);
    }

    /// <summary>
    /// Gets or sets the icon geometry or path data for collapsed branch nodes.
    /// </summary>
    public string? CollapseIcon
    {
        get => GetValue(CollapseIconProperty);
        set => SetValue(CollapseIconProperty, value);
    }

    /// <summary>
    /// Gets or sets the icon geometry or path data for leaf nodes.
    /// </summary>
    public string? LeafIcon
    {
        get => GetValue(LeafIconProperty);
        set => SetValue(LeafIconProperty, value);
    }

    /// <summary>
    /// Gets or sets whether tree lines are shown to indicate hierarchy.
    /// </summary>
    public bool ShowLines
    {
        get => GetValue(ShowLinesProperty);
        set => SetValue(ShowLinesProperty, value);
    }

    /// <summary>
    /// Gets or sets the brush used to draw tree lines.
    /// </summary>
    public IBrush? LineBrush
    {
        get => GetValue(LineBrushProperty);
        set => SetValue(LineBrushProperty, value);
    }

    protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
    {
        return new AuraTreeViewItem();
    }

    protected override bool NeedsContainerOverride(object? item, int index, out object? recycleKey)
    {
        return NeedsContainer<AuraTreeViewItem>(item, out recycleKey);
    }

    protected override void PrepareContainerForItemOverride(Control container, object? item, int index)
    {
        base.PrepareContainerForItemOverride(container, item, index);
        if (container is AuraTreeViewItem treeItem)
        {
            treeItem.Classes.Add("auratreeitem");
        }
    }
}
