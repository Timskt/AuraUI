using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace AuraUI.Controls.Input;

/// <summary>
/// A mobile-style chip/tag input control. Users type to add chips, swipe to delete,
/// and can select from a suggestions dropdown. Supports max chip limits and
/// duplicate prevention.
/// </summary>
[PseudoClasses(":empty", ":focused", ":max-reached")]
public class ChipInput : TemplatedControl
{
    /// <summary>
    /// Defines the <see cref="Chips"/> styled property.
    /// </summary>
    public static readonly StyledProperty<ObservableCollection<string>?> ChipsProperty =
        AvaloniaProperty.Register<ChipInput, ObservableCollection<string>?>(nameof(Chips));

    /// <summary>
    /// Defines the <see cref="Placeholder"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> PlaceholderProperty =
        AvaloniaProperty.Register<ChipInput, string?>(nameof(Placeholder));

    /// <summary>
    /// Defines the <see cref="MaxChips"/> styled property.
    /// Maximum number of chips allowed. -1 means unlimited.
    /// </summary>
    public static readonly StyledProperty<int> MaxChipsProperty =
        AvaloniaProperty.Register<ChipInput, int>(nameof(MaxChips), -1);

    /// <summary>
    /// Defines the <see cref="AllowDuplicate"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> AllowDuplicateProperty =
        AvaloniaProperty.Register<ChipInput, bool>(nameof(AllowDuplicate));

    /// <summary>
    /// Defines the <see cref="InputText"/> styled property.
    /// The current text in the input field.
    /// </summary>
    public static readonly StyledProperty<string?> InputTextProperty =
        AvaloniaProperty.Register<ChipInput, string?>(nameof(InputText));

    /// <summary>
    /// Defines the <see cref="Suggestions"/> styled property.
    /// Available suggestions shown in the dropdown.
    /// </summary>
    public static readonly StyledProperty<ObservableCollection<string>?> SuggestionsProperty =
        AvaloniaProperty.Register<ChipInput, ObservableCollection<string>?>(nameof(Suggestions));

    /// <summary>
    /// Defines the <see cref="ShowSuggestions"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> ShowSuggestionsProperty =
        AvaloniaProperty.Register<ChipInput, bool>(nameof(ShowSuggestions));

    /// <summary>
    /// Defines the <see cref="Separator"/> styled property.
    /// Character(s) that trigger chip creation (e.g., comma, enter).
    /// </summary>
    public static readonly StyledProperty<string> SeparatorProperty =
        AvaloniaProperty.Register<ChipInput, string>(nameof(Separator), ",");

    /// <summary>
    /// Defines the <see cref="SelectedSuggestionIndex"/> styled property.
    /// </summary>
    public static readonly StyledProperty<int> SelectedSuggestionIndexProperty =
        AvaloniaProperty.Register<ChipInput, int>(nameof(SelectedSuggestionIndex), -1);

    /// <summary>
    /// Defines the <see cref="IsInputFocused"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsInputFocusedProperty =
        AvaloniaProperty.Register<ChipInput, bool>(nameof(IsInputFocused));

    static ChipInput()
    {
        ChipsProperty.Changed.AddClassHandler<ChipInput>((x, _) => x.UpdatePseudoClasses());
        MaxChipsProperty.Changed.AddClassHandler<ChipInput>((x, _) => x.UpdatePseudoClasses());
        IsInputFocusedProperty.Changed.AddClassHandler<ChipInput>((x, _) => x.UpdatePseudoClasses());
    }

    /// <summary>
    /// Gets or sets the collection of chip strings.
    /// </summary>
    public ObservableCollection<string>? Chips
    {
        get => GetValue(ChipsProperty);
        set => SetValue(ChipsProperty, value);
    }

    /// <summary>
    /// Gets or sets the placeholder text for the input.
    /// </summary>
    public string? Placeholder
    {
        get => GetValue(PlaceholderProperty);
        set => SetValue(PlaceholderProperty, value);
    }

    /// <summary>
    /// Gets or sets the maximum number of chips (-1 for unlimited).
    /// </summary>
    public int MaxChips
    {
        get => GetValue(MaxChipsProperty);
        set => SetValue(MaxChipsProperty, value);
    }

    /// <summary>
    /// Gets or sets whether duplicate chips are allowed.
    /// </summary>
    public bool AllowDuplicate
    {
        get => GetValue(AllowDuplicateProperty);
        set => SetValue(AllowDuplicateProperty, value);
    }

    /// <summary>
    /// Gets or sets the current input text.
    /// </summary>
    public string? InputText
    {
        get => GetValue(InputTextProperty);
        set => SetValue(InputTextProperty, value);
    }

    /// <summary>
    /// Gets or sets the available suggestions.
    /// </summary>
    public ObservableCollection<string>? Suggestions
    {
        get => GetValue(SuggestionsProperty);
        set => SetValue(SuggestionsProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the suggestions dropdown is visible.
    /// </summary>
    public bool ShowSuggestions
    {
        get => GetValue(ShowSuggestionsProperty);
        set => SetValue(ShowSuggestionsProperty, value);
    }

    /// <summary>
    /// Gets or sets the separator character for chip creation.
    /// </summary>
    public string Separator
    {
        get => GetValue(SeparatorProperty);
        set => SetValue(SeparatorProperty, value);
    }

    /// <summary>
    /// Gets or sets the currently highlighted suggestion index.
    /// </summary>
    public int SelectedSuggestionIndex
    {
        get => GetValue(SelectedSuggestionIndexProperty);
        set => SetValue(SelectedSuggestionIndexProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the input field is focused.
    /// </summary>
    public bool IsInputFocused
    {
        get => GetValue(IsInputFocusedProperty);
        set => SetValue(IsInputFocusedProperty, value);
    }

    /// <summary>
    /// Occurs when a chip is added.
    /// </summary>
    public event EventHandler<string>? ChipAdded;

    /// <summary>
    /// Occurs when a chip is removed.
    /// </summary>
    public event EventHandler<string>? ChipRemoved;

    /// <summary>
    /// Occurs when the chip limit is reached.
    /// </summary>
    public event EventHandler? MaxChipsReached;

    /// <summary>
    /// Adds a chip if valid.
    /// </summary>
    /// <param name="value">The chip value to add.</param>
    /// <returns>True if the chip was added.</returns>
    public bool AddChip(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return false;

        var chips = Chips ??= new ObservableCollection<string>();
        var max = MaxChips;

        if (max > 0 && chips.Count >= max)
        {
            MaxChipsReached?.Invoke(this, EventArgs.Empty);
            return false;
        }

        if (!AllowDuplicate && chips.Contains(value))
            return false;

        chips.Add(value);
        ChipAdded?.Invoke(this, value);
        UpdatePseudoClasses();
        return true;
    }

    /// <summary>
    /// Removes a chip by value.
    /// </summary>
    /// <param name="value">The chip value to remove.</param>
    /// <returns>True if the chip was removed.</returns>
    public bool RemoveChip(string value)
    {
        var chips = Chips;
        if (chips == null) return false;

        var removed = chips.Remove(value);
        if (removed)
        {
            ChipRemoved?.Invoke(this, value);
            UpdatePseudoClasses();
        }
        return removed;
    }

    /// <summary>
    /// Removes a chip at the specified index.
    /// </summary>
    public void RemoveChipAt(int index)
    {
        var chips = Chips;
        if (chips == null || index < 0 || index >= chips.Count) return;

        var value = chips[index];
        chips.RemoveAt(index);
        ChipRemoved?.Invoke(this, value);
        UpdatePseudoClasses();
    }

    /// <summary>
    /// Clears all chips.
    /// </summary>
    public void ClearChips()
    {
        Chips?.Clear();
        UpdatePseudoClasses();
    }

    /// <summary>
    /// Filters suggestions based on the current input text.
    /// </summary>
    public ObservableCollection<string> GetFilteredSuggestions()
    {
        var input = InputText;
        var suggestions = Suggestions;
        if (suggestions == null || string.IsNullOrEmpty(input))
            return suggestions ?? new ObservableCollection<string>();

        return new ObservableCollection<string>(
            suggestions.Where(s => s.Contains(input, StringComparison.OrdinalIgnoreCase)));
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        UpdatePseudoClasses();
    }

    private void UpdatePseudoClasses()
    {
        var chips = Chips;
        PseudoClasses.Set(":empty", chips == null || chips.Count == 0);
        PseudoClasses.Set(":focused", IsInputFocused);

        var max = MaxChips;
        PseudoClasses.Set(":max-reached", max > 0 && chips != null && chips.Count >= max);
    }
}
