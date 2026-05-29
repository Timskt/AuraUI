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
/// Specifies the selection mode for a <see cref="ToggleButtonGroup"/>.
/// </summary>
public enum ToggleSelectionMode
{
    Single,
    Multi
}

/// <summary>
/// A group of toggle buttons with synchronized selection management.
/// Supports single-select (radio-like) and multi-select (checkbox-like) modes.
/// </summary>
[PseudoClasses(":single", ":multi", ":has-selection")]
public class ToggleButtonGroup : ItemsControl
{
    private readonly ObservableCollection<object> _selectedItems = new();
    private bool _isUpdatingSelection;

    /// <summary>
    /// Defines the <see cref="SelectionMode"/> styled property.
    /// </summary>
    public static readonly StyledProperty<ToggleSelectionMode> SelectionModeProperty =
        AvaloniaProperty.Register<ToggleButtonGroup, ToggleSelectionMode>(
            nameof(SelectionMode),
            ToggleSelectionMode.Single);

    /// <summary>
    /// Defines the <see cref="SelectedItem"/> styled property.
    /// </summary>
    public static readonly StyledProperty<object?> SelectedItemProperty =
        AvaloniaProperty.Register<ToggleButtonGroup, object?>(nameof(SelectedItem));

    /// <summary>
    /// Defines the <see cref="SelectedIndex"/> styled property.
    /// </summary>
    public static readonly StyledProperty<int> SelectedIndexProperty =
        AvaloniaProperty.Register<ToggleButtonGroup, int>(nameof(SelectedIndex), -1);

    static ToggleButtonGroup()
    {
        SelectionModeProperty.Changed.AddClassHandler<ToggleButtonGroup>((x, _) => x.OnSelectionModeChanged());
        SelectedItemProperty.Changed.AddClassHandler<ToggleButtonGroup>((x, e) => x.OnSelectedItemChanged(e));
        SelectedIndexProperty.Changed.AddClassHandler<ToggleButtonGroup>((x, e) => x.OnSelectedIndexChanged(e));
    }

    /// <summary>
    /// Gets or sets the selection mode (Single or Multi).
    /// </summary>
    public ToggleSelectionMode SelectionMode
    {
        get => GetValue(SelectionModeProperty);
        set => SetValue(SelectionModeProperty, value);
    }

    /// <summary>
    /// Gets or sets the currently selected item (Single mode).
    /// </summary>
    public object? SelectedItem
    {
        get => GetValue(SelectedItemProperty);
        set => SetValue(SelectedItemProperty, value);
    }

    /// <summary>
    /// Gets or sets the currently selected index (Single mode).
    /// </summary>
    public int SelectedIndex
    {
        get => GetValue(SelectedIndexProperty);
        set => SetValue(SelectedIndexProperty, value);
    }

    /// <summary>
    /// Gets the collection of selected items (Multi mode).
    /// </summary>
    public IReadOnlyList<object> SelectedItems => _selectedItems;

    /// <summary>
    /// Defines the routed event for selection changes.
    /// </summary>
    public static readonly RoutedEvent<SelectionChangedEventArgs> SelectionChangedEvent =
        RoutedEvent.Register<ToggleButtonGroup, SelectionChangedEventArgs>(
            nameof(SelectionChanged), RoutingStrategies.Bubble);

    /// <summary>
    /// Occurs when the selection changes.
    /// </summary>
    public event EventHandler<SelectionChangedEventArgs>? SelectionChanged
    {
        add => AddHandler(SelectionChangedEvent, value);
        remove => RemoveHandler(SelectionChangedEvent, value);
    }

    protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
    {
        return new ToggleButton();
    }

    protected override bool NeedsContainerOverride(object? item, int index, out object? recycleKey)
    {
        return NeedsContainer<ToggleButton>(item, out recycleKey);
    }

    protected override void PrepareContainerForItemOverride(Control container, object? item, int index)
    {
        base.PrepareContainerForItemOverride(container, item, index);

        if (container is ToggleButton toggleButton)
        {
            toggleButton.IsChecked = GetIsItemSelected(item);
            toggleButton.IsCheckedChanged += OnToggleIsCheckedChanged;
        }
    }

    protected override void ClearContainerForItemOverride(Control container)
    {
        base.ClearContainerForItemOverride(container);

        if (container is ToggleButton toggleButton)
        {
            toggleButton.IsCheckedChanged -= OnToggleIsCheckedChanged;
        }
    }

    private void OnToggleIsCheckedChanged(object? sender, RoutedEventArgs e)
    {
        if (_isUpdatingSelection) return;

        if (sender is ToggleButton toggleButton)
        {
            var item = ItemFromContainer(toggleButton);
            if (item == null) return;

            _isUpdatingSelection = true;
            try
            {
                var isChecked = toggleButton.IsChecked == true;
                if (isChecked)
                {
                    if (SelectionMode == ToggleSelectionMode.Single)
                    {
                        // Uncheck all other toggles.
                        UncheckAllExcept(toggleButton);
                        SetCurrentValue(SelectedItemProperty, item);
                        SetCurrentValue(SelectedIndexProperty, IndexFromContainer(toggleButton));
                    }
                    else
                    {
                        if (!_selectedItems.Contains(item))
                        {
                            _selectedItems.Add(item);
                        }
                    }
                }
                else
                {
                    if (SelectionMode == ToggleSelectionMode.Single)
                    {
                        // In single mode, prevent unchecking (radio behavior).
                        toggleButton.IsChecked = true;
                    }
                    else
                    {
                        _selectedItems.Remove(item);
                    }
                }

                UpdatePseudoClasses();
                RaiseSelectionChanged(item, isChecked);
            }
            finally
            {
                _isUpdatingSelection = false;
            }
        }
    }

    private void OnSelectionModeChanged()
    {
        // Clear multi-select when switching to single.
        if (SelectionMode == ToggleSelectionMode.Single && _selectedItems.Count > 1)
        {
            var first = _selectedItems.FirstOrDefault();
            _selectedItems.Clear();
            if (first != null) _selectedItems.Add(first);
        }

        UpdatePseudoClasses();
        SyncToggleStates();
    }

    private void OnSelectedItemChanged(AvaloniaPropertyChangedEventArgs e)
    {
        if (_isUpdatingSelection) return;

        _isUpdatingSelection = true;
        try
        {
            _selectedItems.Clear();
            if (e.NewValue != null)
            {
                _selectedItems.Add(e.NewValue);
            }

            SyncToggleStates();
            UpdatePseudoClasses();
        }
        finally
        {
            _isUpdatingSelection = false;
        }
    }

    private void OnSelectedIndexChanged(AvaloniaPropertyChangedEventArgs e)
    {
        if (_isUpdatingSelection) return;

        var index = (int)e.NewValue!;
        if (index >= 0 && index < ItemCount)
        {
            var item = Items[index];
            SetCurrentValue(SelectedItemProperty, item);
        }
    }

    private bool GetIsItemSelected(object? item)
    {
        if (item == null) return false;

        if (SelectionMode == ToggleSelectionMode.Single)
        {
            return Equals(item, SelectedItem);
        }

        return _selectedItems.Contains(item);
    }

    private void UncheckAllExcept(ToggleButton except)
    {
        var count = ItemCount;
        for (var i = 0; i < count; i++)
        {
            if (ContainerFromIndex(i) is ToggleButton toggle && toggle != except)
            {
                toggle.IsChecked = false;
            }
        }
    }

    private void SyncToggleStates()
    {
        var count = ItemCount;
        for (var i = 0; i < count; i++)
        {
            if (ContainerFromIndex(i) is ToggleButton toggle)
            {
                var item = Items[i];
                toggle.IsChecked = GetIsItemSelected(item);
            }
        }
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(":single", SelectionMode == ToggleSelectionMode.Single);
        PseudoClasses.Set(":multi", SelectionMode == ToggleSelectionMode.Multi);
        PseudoClasses.Set(":has-selection", _selectedItems.Count > 0);
    }

    private void RaiseSelectionChanged(object? item, bool isSelected)
    {
        var added = isSelected ? new[] { item! } : Array.Empty<object>();
        var removed = isSelected ? Array.Empty<object>() : new[] { item! };
        var args = new SelectionChangedEventArgs(SelectionChangedEvent, removed, added);
        RaiseEvent(args);
    }
}
