using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;

namespace AuraUI.Controls.Selection;

/// <summary>
/// Represents an item in a Transfer control.
/// </summary>
public class TransferItem : AvaloniaObject
{
    /// <summary>
    /// Defines the <see cref="Key"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> KeyProperty =
        AvaloniaProperty.Register<TransferItem, string?>(nameof(Key));

    /// <summary>
    /// Defines the <see cref="Title"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> TitleProperty =
        AvaloniaProperty.Register<TransferItem, string?>(nameof(Title));

    /// <summary>
    /// Defines the <see cref="Description"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> DescriptionProperty =
        AvaloniaProperty.Register<TransferItem, string?>(nameof(Description));

    /// <summary>
    /// Defines the <see cref="IsDisabled"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsDisabledProperty =
        AvaloniaProperty.Register<TransferItem, bool>(nameof(IsDisabled));

    /// <summary>
    /// Defines the <see cref="IsChecked"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsCheckedProperty =
        AvaloniaProperty.Register<TransferItem, bool>(nameof(IsChecked));

    /// <summary>
    /// Gets or sets the unique key for this item.
    /// </summary>
    public string? Key
    {
        get => GetValue(KeyProperty);
        set => SetValue(KeyProperty, value);
    }

    /// <summary>
    /// Gets or sets the display title.
    /// </summary>
    public string? Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    /// <summary>
    /// Gets or sets the item description.
    /// </summary>
    public string? Description
    {
        get => GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }

    /// <summary>
    /// Gets or sets whether this item is disabled.
    /// </summary>
    public bool IsDisabled
    {
        get => GetValue(IsDisabledProperty);
        set => SetValue(IsDisabledProperty, value);
    }

    /// <summary>
    /// Gets or sets whether this item is checked/selected.
    /// </summary>
    public bool IsChecked
    {
        get => GetValue(IsCheckedProperty);
        set => SetValue(IsCheckedProperty, value);
    }
}

/// <summary>
/// Event arguments for Transfer change events.
/// </summary>
public class TransferChangeEventArgs : RoutedEventArgs
{
    /// <summary>
    /// Gets the direction of the transfer.
    /// </summary>
    public TransferDirection Direction { get; }

    /// <summary>
    /// Gets the keys of the moved items.
    /// </summary>
    public IList<string> MovedKeys { get; }

    public TransferChangeEventArgs(TransferDirection direction, IList<string> movedKeys)
    {
        Direction = direction;
        MovedKeys = movedKeys;
    }
}

/// <summary>
/// Specifies the transfer direction.
/// </summary>
public enum TransferDirection
{
    Left,
    Right
}

/// <summary>
/// A dual-list transfer component allowing users to move items between source and target lists.
/// Supports search filtering, select all, and move operations.
/// Inspired by Ant Design's Transfer component.
/// </summary>
[TemplatePart("PART_SourceList", typeof(ListBox))]
[TemplatePart("PART_TargetList", typeof(ListBox))]
[TemplatePart("PART_MoveRightButton", typeof(Button))]
[TemplatePart("PART_MoveLeftButton", typeof(Button))]
[TemplatePart("PART_SourceSearchBox", typeof(TextBox))]
[TemplatePart("PART_TargetSearchBox", typeof(TextBox))]
[PseudoClasses(":has-source-selection", ":has-target-selection")]
public class Transfer : TemplatedControl
{
    private ListBox? _sourceList;
    private ListBox? _targetList;
    private Button? _moveRightButton;
    private Button? _moveLeftButton;
    private TextBox? _sourceSearchBox;
    private TextBox? _targetSearchBox;

    /// <summary>
    /// Defines the <see cref="SourceItems"/> styled property.
    /// </summary>
    public static readonly StyledProperty<ObservableCollection<TransferItem>?> SourceItemsProperty =
        AvaloniaProperty.Register<Transfer, ObservableCollection<TransferItem>?>(nameof(SourceItems));

    /// <summary>
    /// Defines the <see cref="TargetItems"/> styled property.
    /// </summary>
    public static readonly StyledProperty<ObservableCollection<TransferItem>?> TargetItemsProperty =
        AvaloniaProperty.Register<Transfer, ObservableCollection<TransferItem>?>(nameof(TargetItems));

    /// <summary>
    /// Defines the <see cref="LeftTitle"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> LeftTitleProperty =
        AvaloniaProperty.Register<Transfer, string?>(nameof(LeftTitle), "Source");

    /// <summary>
    /// Defines the <see cref="RightTitle"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> RightTitleProperty =
        AvaloniaProperty.Register<Transfer, string?>(nameof(RightTitle), "Target");

    /// <summary>
    /// Defines the <see cref="ShowSearch"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> ShowSearchProperty =
        AvaloniaProperty.Register<Transfer, bool>(nameof(ShowSearch));

    /// <summary>
    /// Defines the <see cref="SourceSearchText"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> SourceSearchTextProperty =
        AvaloniaProperty.Register<Transfer, string?>(nameof(SourceSearchText));

    /// <summary>
    /// Defines the <see cref="TargetSearchText"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> TargetSearchTextProperty =
        AvaloniaProperty.Register<Transfer, string?>(nameof(TargetSearchText));

    /// <summary>
    /// Defines the <see cref="Operations"/> styled property.
    /// Text for the move buttons [moveRight, moveLeft].
    /// </summary>
    public static readonly StyledProperty<IList<string>?> OperationsProperty =
        AvaloniaProperty.Register<Transfer, IList<string>?>(nameof(Operations));

    /// <summary>
    /// Defines the <see cref="IsSourceSelectAll"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsSourceSelectAllProperty =
        AvaloniaProperty.Register<Transfer, bool>(nameof(IsSourceSelectAll));

    /// <summary>
    /// Defines the <see cref="IsTargetSelectAll"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsTargetSelectAllProperty =
        AvaloniaProperty.Register<Transfer, bool>(nameof(IsTargetSelectAll));

    static Transfer()
    {
        SourceItemsProperty.Changed.AddClassHandler<Transfer>((x, _) => x.UpdateSelectionPseudoClasses());
        TargetItemsProperty.Changed.AddClassHandler<Transfer>((x, _) => x.UpdateSelectionPseudoClasses());
    }

    /// <summary>
    /// Gets or sets the source (left) items.
    /// </summary>
    public ObservableCollection<TransferItem>? SourceItems
    {
        get => GetValue(SourceItemsProperty);
        set => SetValue(SourceItemsProperty, value);
    }

    /// <summary>
    /// Gets or sets the target (right) items.
    /// </summary>
    public ObservableCollection<TransferItem>? TargetItems
    {
        get => GetValue(TargetItemsProperty);
        set => SetValue(TargetItemsProperty, value);
    }

    /// <summary>
    /// Gets or sets the title for the source list.
    /// </summary>
    public string? LeftTitle
    {
        get => GetValue(LeftTitleProperty);
        set => SetValue(LeftTitleProperty, value);
    }

    /// <summary>
    /// Gets or sets the title for the target list.
    /// </summary>
    public string? RightTitle
    {
        get => GetValue(RightTitleProperty);
        set => SetValue(RightTitleProperty, value);
    }

    /// <summary>
    /// Gets or sets whether search boxes are shown.
    /// </summary>
    public bool ShowSearch
    {
        get => GetValue(ShowSearchProperty);
        set => SetValue(ShowSearchProperty, value);
    }

    /// <summary>
    /// Gets or sets the source list search text.
    /// </summary>
    public string? SourceSearchText
    {
        get => GetValue(SourceSearchTextProperty);
        set => SetValue(SourceSearchTextProperty, value);
    }

    /// <summary>
    /// Gets or sets the target list search text.
    /// </summary>
    public string? TargetSearchText
    {
        get => GetValue(TargetSearchTextProperty);
        set => SetValue(TargetSearchTextProperty, value);
    }

    /// <summary>
    /// Gets or sets the operation button labels [right, left].
    /// </summary>
    public IList<string>? Operations
    {
        get => GetValue(OperationsProperty);
        set => SetValue(OperationsProperty, value);
    }

    /// <summary>
    /// Gets or sets whether all source items are selected.
    /// </summary>
    public bool IsSourceSelectAll
    {
        get => GetValue(IsSourceSelectAllProperty);
        set => SetValue(IsSourceSelectAllProperty, value);
    }

    /// <summary>
    /// Gets or sets whether all target items are selected.
    /// </summary>
    public bool IsTargetSelectAll
    {
        get => GetValue(IsTargetSelectAllProperty);
        set => SetValue(IsTargetSelectAllProperty, value);
    }

    /// <summary>
    /// Occurs when items are moved between lists.
    /// </summary>
    public event EventHandler<TransferChangeEventArgs>? Change;

    /// <summary>
    /// Occurs when item selection changes.
    /// </summary>
    public event EventHandler<RoutedEventArgs>? SelectChange;

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        if (_moveRightButton != null) _moveRightButton.Click -= OnMoveRightClick;
        if (_moveLeftButton != null) _moveLeftButton.Click -= OnMoveLeftClick;

        _sourceList = e.NameScope.Find<ListBox>("PART_SourceList");
        _targetList = e.NameScope.Find<ListBox>("PART_TargetList");
        _moveRightButton = e.NameScope.Find<Button>("PART_MoveRightButton");
        _moveLeftButton = e.NameScope.Find<Button>("PART_MoveLeftButton");
        _sourceSearchBox = e.NameScope.Find<TextBox>("PART_SourceSearchBox");
        _targetSearchBox = e.NameScope.Find<TextBox>("PART_TargetSearchBox");

        if (_moveRightButton != null) _moveRightButton.Click += OnMoveRightClick;
        if (_moveLeftButton != null) _moveLeftButton.Click += OnMoveLeftClick;

        UpdateSelectionPseudoClasses();
    }

    /// <summary>
    /// Moves selected items from source to target.
    /// </summary>
    public void MoveToTarget()
    {
        var source = SourceItems;
        var target = TargetItems;
        if (source == null || target == null) return;

        var selected = source.Where(i => i.IsChecked && !i.IsDisabled).ToList();
        var movedKeys = new List<string>();

        foreach (var item in selected)
        {
            source.Remove(item);
            item.IsChecked = false;
            target.Add(item);
            if (item.Key != null) movedKeys.Add(item.Key);
        }

        if (movedKeys.Count > 0)
        {
            Change?.Invoke(this, new TransferChangeEventArgs(TransferDirection.Right, movedKeys));
        }
    }

    /// <summary>
    /// Moves selected items from target to source.
    /// </summary>
    public void MoveToSource()
    {
        var source = SourceItems;
        var target = TargetItems;
        if (source == null || target == null) return;

        var selected = target.Where(i => i.IsChecked && !i.IsDisabled).ToList();
        var movedKeys = new List<string>();

        foreach (var item in selected)
        {
            target.Remove(item);
            item.IsChecked = false;
            source.Add(item);
            if (item.Key != null) movedKeys.Add(item.Key);
        }

        if (movedKeys.Count > 0)
        {
            Change?.Invoke(this, new TransferChangeEventArgs(TransferDirection.Left, movedKeys));
        }
    }

    /// <summary>
    /// Selects or deselects all items in the source list.
    /// </summary>
    public void SelectAllSource(bool select)
    {
        var source = SourceItems;
        if (source == null) return;
        foreach (var item in source.Where(i => !i.IsDisabled))
        {
            item.IsChecked = select;
        }
        IsSourceSelectAll = select;
        SelectChange?.Invoke(this, new RoutedEventArgs());
    }

    /// <summary>
    /// Selects or deselects all items in the target list.
    /// </summary>
    public void SelectAllTarget(bool select)
    {
        var target = TargetItems;
        if (target == null) return;
        foreach (var item in target.Where(i => !i.IsDisabled))
        {
            item.IsChecked = select;
        }
        IsTargetSelectAll = select;
        SelectChange?.Invoke(this, new RoutedEventArgs());
    }

    private void OnMoveRightClick(object? sender, RoutedEventArgs e) => MoveToTarget();
    private void OnMoveLeftClick(object? sender, RoutedEventArgs e) => MoveToSource();

    private void UpdateSelectionPseudoClasses()
    {
        var hasSourceSelection = SourceItems?.Any(i => i.IsChecked) == true;
        var hasTargetSelection = TargetItems?.Any(i => i.IsChecked) == true;
        PseudoClasses.Set(":has-source-selection", hasSourceSelection);
        PseudoClasses.Set(":has-target-selection", hasTargetSelection);
    }
}
