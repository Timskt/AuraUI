using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace AuraUI.Controls.Selection;

/// <summary>
/// A date/time picker control that displays a calendar dropdown with month/year navigation.
/// Supports date range constraints, custom formatting, and placeholder text.
/// </summary>
[TemplatePart("PART_TextBox", typeof(TextBox))]
[TemplatePart("PART_DropdownButton", typeof(Button))]
[TemplatePart("PART_Popup", typeof(Popup))]
[TemplatePart("PART_Calendar", typeof(Calendar))]
[PseudoClasses(":open", ":selected", ":focused")]
public class DateTimePicker : TemplatedControl
{
    private TextBox? _textBox;
    private Button? _dropdownButton;
    private Popup? _popup;
    private Calendar? _calendar;

    /// <summary>
    /// Defines the <see cref="SelectedDate"/> styled property.
    /// </summary>
    public static readonly StyledProperty<DateTime?> SelectedDateProperty =
        AvaloniaProperty.Register<DateTimePicker, DateTime?>(nameof(SelectedDate));

    /// <summary>
    /// Defines the <see cref="MinDate"/> styled property.
    /// </summary>
    public static readonly StyledProperty<DateTime> MinDateProperty =
        AvaloniaProperty.Register<DateTimePicker, DateTime>(
            nameof(MinDate),
            new DateTime(1900, 1, 1));

    /// <summary>
    /// Defines the <see cref="MaxDate"/> styled property.
    /// </summary>
    public static readonly StyledProperty<DateTime> MaxDateProperty =
        AvaloniaProperty.Register<DateTimePicker, DateTime>(
            nameof(MaxDate),
            new DateTime(2100, 12, 31));

    /// <summary>
    /// Defines the <see cref="Format"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string> FormatProperty =
        AvaloniaProperty.Register<DateTimePicker, string>(
            nameof(Format),
            "yyyy-MM-dd");

    /// <summary>
    /// Defines the <see cref="PlaceholderText"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> PlaceholderTextProperty =
        AvaloniaProperty.Register<DateTimePicker, string?>(
            nameof(PlaceholderText),
            "Select a date...");

    /// <summary>
    /// Defines the <see cref="IsDropDownOpen"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsDropDownOpenProperty =
        AvaloniaProperty.Register<DateTimePicker, bool>(nameof(IsDropDownOpen));

    static DateTimePicker()
    {
        SelectedDateProperty.Changed.AddClassHandler<DateTimePicker>((x, e) => x.OnSelectedDateChanged(e));
        IsDropDownOpenProperty.Changed.AddClassHandler<DateTimePicker>((x, e) => x.OnIsDropDownOpenChanged(e));
    }

    /// <summary>
    /// Gets or sets the currently selected date.
    /// </summary>
    public DateTime? SelectedDate
    {
        get => GetValue(SelectedDateProperty);
        set => SetValue(SelectedDateProperty, value);
    }

    /// <summary>
    /// Gets or sets the minimum selectable date.
    /// </summary>
    public DateTime MinDate
    {
        get => GetValue(MinDateProperty);
        set => SetValue(MinDateProperty, value);
    }

    /// <summary>
    /// Gets or sets the maximum selectable date.
    /// </summary>
    public DateTime MaxDate
    {
        get => GetValue(MaxDateProperty);
        set => SetValue(MaxDateProperty, value);
    }

    /// <summary>
    /// Gets or sets the date display format string.
    /// </summary>
    public string Format
    {
        get => GetValue(FormatProperty);
        set => SetValue(FormatProperty, value);
    }

    /// <summary>
    /// Gets or sets the placeholder text shown when no date is selected.
    /// </summary>
    public string? PlaceholderText
    {
        get => GetValue(PlaceholderTextProperty);
        set => SetValue(PlaceholderTextProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the calendar dropdown is open.
    /// </summary>
    public bool IsDropDownOpen
    {
        get => GetValue(IsDropDownOpenProperty);
        set => SetValue(IsDropDownOpenProperty, value);
    }

    /// <summary>
    /// Occurs when the selected date changes.
    /// </summary>
    public event EventHandler<DateTimeChangedEventArgs>? SelectedDateChanged;

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        if (_dropdownButton != null)
        {
            _dropdownButton.Click -= OnDropdownButtonClick;
        }

        if (_calendar != null)
        {
            _calendar.SelectedDatesChanged -= OnCalendarSelectedDatesChanged;
        }

        _textBox = e.NameScope.Find<TextBox>("PART_TextBox");
        _dropdownButton = e.NameScope.Find<Button>("PART_DropdownButton");
        _popup = e.NameScope.Find<Popup>("PART_Popup");
        _calendar = e.NameScope.Find<Calendar>("PART_Calendar");

        if (_dropdownButton != null)
        {
            _dropdownButton.Click += OnDropdownButtonClick;
        }

        if (_calendar != null)
        {
            _calendar.SelectedDatesChanged += OnCalendarSelectedDatesChanged;
            _calendar.DisplayDateStart = MinDate;
            _calendar.DisplayDateEnd = MaxDate;

            if (SelectedDate.HasValue)
            {
                _calendar.SelectedDate = SelectedDate.Value;
                _calendar.DisplayDate = SelectedDate.Value;
            }
        }

        UpdateTextDisplay();
    }

    private void OnDropdownButtonClick(object? sender, RoutedEventArgs e)
    {
        SetCurrentValue(IsDropDownOpenProperty, !IsDropDownOpen);
    }

    private void OnIsDropDownOpenChanged(AvaloniaPropertyChangedEventArgs e)
    {
        var isOpen = (bool)e.NewValue!;
        PseudoClasses.Set(":open", isOpen);

        if (isOpen && _calendar != null && SelectedDate.HasValue)
        {
            _calendar.DisplayDate = SelectedDate.Value;
        }
    }

    private void OnSelectedDateChanged(AvaloniaPropertyChangedEventArgs e)
    {
        var oldValue = (DateTime?)e.OldValue;
        var newValue = (DateTime?)e.NewValue;

        PseudoClasses.Set(":selected", newValue.HasValue);
        UpdateTextDisplay();

        if (_calendar != null && newValue.HasValue)
        {
            _calendar.SelectedDate = newValue.Value;
            _calendar.DisplayDate = newValue.Value;
        }

        SelectedDateChanged?.Invoke(this, new DateTimeChangedEventArgs(oldValue, newValue));
    }

    private void OnCalendarSelectedDatesChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (_calendar?.SelectedDate != null)
        {
            var selected = _calendar.SelectedDate.Value;
            if (selected >= MinDate && selected <= MaxDate)
            {
                SetCurrentValue(SelectedDateProperty, selected);
                SetCurrentValue(IsDropDownOpenProperty, false);
            }
        }
    }

    private void UpdateTextDisplay()
    {
        if (_textBox == null) return;

        _textBox.Text = SelectedDate.HasValue
            ? SelectedDate.Value.ToString(Format)
            : null;
    }
}

/// <summary>
/// Event arguments for date/time picker selection changes.
/// </summary>
public class DateTimeChangedEventArgs : EventArgs
{
    /// <summary>
    /// Gets the previously selected date.
    /// </summary>
    public DateTime? OldValue { get; }

    /// <summary>
    /// Gets the newly selected date.
    /// </summary>
    public DateTime? NewValue { get; }

    public DateTimeChangedEventArgs(DateTime? oldValue, DateTime? newValue)
    {
        OldValue = oldValue;
        NewValue = newValue;
    }
}
