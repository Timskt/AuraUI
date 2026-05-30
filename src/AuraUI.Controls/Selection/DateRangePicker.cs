using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace AuraUI.Controls.Selection;

/// <summary>
/// A date range picker that allows selecting a start and end date.
/// Displays a calendar dropdown where users can click two dates to define
/// a range. Supports minimum/maximum date constraints, custom formatting,
/// and preset range shortcuts.
/// </summary>
[TemplatePart("PART_TextBox", typeof(TextBox))]
[TemplatePart("PART_DropdownButton", typeof(Button))]
[TemplatePart("PART_Popup", typeof(Popup))]
[TemplatePart("PART_Calendar", typeof(Calendar))]
[TemplatePart("PART_PresetPanel", typeof(ItemsControl))]
[PseudoClasses(":open", ":range-selected", ":selecting-end")]
public class DateRangePicker : TemplatedControl
{
    private TextBox? _textBox;
    private Button? _dropdownButton;
    private Popup? _popup;
    private Calendar? _calendar;
    private bool _isSelectingEnd;

    /// <summary>
    /// Defines the <see cref="StartDate"/> styled property.
    /// </summary>
    public static readonly StyledProperty<DateTime?> StartDateProperty =
        AvaloniaProperty.Register<DateRangePicker, DateTime?>(nameof(StartDate));

    /// <summary>
    /// Defines the <see cref="EndDate"/> styled property.
    /// </summary>
    public static readonly StyledProperty<DateTime?> EndDateProperty =
        AvaloniaProperty.Register<DateRangePicker, DateTime?>(nameof(EndDate));

    /// <summary>
    /// Defines the <see cref="MinDate"/> styled property.
    /// </summary>
    public static readonly StyledProperty<DateTime> MinDateProperty =
        AvaloniaProperty.Register<DateRangePicker, DateTime>(
            nameof(MinDate),
            new DateTime(1900, 1, 1));

    /// <summary>
    /// Defines the <see cref="MaxDate"/> styled property.
    /// </summary>
    public static readonly StyledProperty<DateTime> MaxDateProperty =
        AvaloniaProperty.Register<DateRangePicker, DateTime>(
            nameof(MaxDate),
            new DateTime(2100, 12, 31));

    /// <summary>
    /// Defines the <see cref="Format"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string> FormatProperty =
        AvaloniaProperty.Register<DateRangePicker, string>(
            nameof(Format),
            "yyyy-MM-dd");

    /// <summary>
    /// Defines the <see cref="Separator"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string> SeparatorProperty =
        AvaloniaProperty.Register<DateRangePicker, string>(
            nameof(Separator),
            " to ");

    /// <summary>
    /// Defines the <see cref="PlaceholderText"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> PlaceholderTextProperty =
        AvaloniaProperty.Register<DateRangePicker, string?>(
            nameof(PlaceholderText),
            "Select date range...");

    /// <summary>
    /// Defines the <see cref="IsDropDownOpen"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsDropDownOpenProperty =
        AvaloniaProperty.Register<DateRangePicker, bool>(nameof(IsDropDownOpen));

    /// <summary>
    /// Defines the <see cref="Presets"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IList<DateRangePreset>?> PresetsProperty =
        AvaloniaProperty.Register<DateRangePicker, IList<DateRangePreset>?>(nameof(Presets));

    static DateRangePicker()
    {
        StartDateProperty.Changed.AddClassHandler<DateRangePicker>((x, _) => x.OnRangeChanged());
        EndDateProperty.Changed.AddClassHandler<DateRangePicker>((x, _) => x.OnRangeChanged());
        IsDropDownOpenProperty.Changed.AddClassHandler<DateRangePicker>((x, _) => x.OnIsDropDownOpenChanged());
    }

    /// <summary>
    /// Occurs when the date range changes.
    /// </summary>
    public event EventHandler<DateRangeChangedEventArgs>? DateRangeChanged;

    /// <summary>
    /// Gets or sets the start date of the range.
    /// </summary>
    public DateTime? StartDate
    {
        get => GetValue(StartDateProperty);
        set => SetValue(StartDateProperty, value);
    }

    /// <summary>
    /// Gets or sets the end date of the range.
    /// </summary>
    public DateTime? EndDate
    {
        get => GetValue(EndDateProperty);
        set => SetValue(EndDateProperty, value);
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
    /// Gets or sets the date format string.
    /// </summary>
    public string Format
    {
        get => GetValue(FormatProperty);
        set => SetValue(FormatProperty, value);
    }

    /// <summary>
    /// Gets or sets the separator between start and end dates.
    /// </summary>
    public string Separator
    {
        get => GetValue(SeparatorProperty);
        set => SetValue(SeparatorProperty, value);
    }

    /// <summary>
    /// Gets or sets the placeholder text.
    /// </summary>
    public string? PlaceholderText
    {
        get => GetValue(PlaceholderTextProperty);
        set => SetValue(PlaceholderTextProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the dropdown is open.
    /// </summary>
    public bool IsDropDownOpen
    {
        get => GetValue(IsDropDownOpenProperty);
        set => SetValue(IsDropDownOpenProperty, value);
    }

    /// <summary>
    /// Gets or sets preset date range shortcuts.
    /// </summary>
    public IList<DateRangePreset>? Presets
    {
        get => GetValue(PresetsProperty);
        set => SetValue(PresetsProperty, value);
    }

    /// <summary>
    /// Clears the selected date range.
    /// </summary>
    public void Clear()
    {
        _isSelectingEnd = false;
        StartDate = null;
        EndDate = null;
    }

    /// <summary>
    /// Applies a preset date range.
    /// </summary>
    public void ApplyPreset(DateRangePreset preset)
    {
        StartDate = preset.Start;
        EndDate = preset.End;
        IsDropDownOpen = false;
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        DetachCalendar();

        _textBox = e.NameScope.Find<TextBox>("PART_TextBox");
        _dropdownButton = e.NameScope.Find<Button>("PART_DropdownButton");
        _popup = e.NameScope.Find<Popup>("PART_Popup");
        _calendar = e.NameScope.Find<Calendar>("PART_Calendar");

        AttachCalendar();
        UpdateDisplayText();
        UpdatePseudoClasses();
    }

    private void AttachCalendar()
    {
        if (_calendar != null)
        {
            _calendar.SelectedDatesChanged += OnCalendarSelectedDatesChanged;
        }
    }

    private void DetachCalendar()
    {
        if (_calendar != null)
        {
            _calendar.SelectedDatesChanged -= OnCalendarSelectedDatesChanged;
            _calendar = null;
        }
    }

    private void OnCalendarSelectedDatesChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (_calendar?.SelectedDate is not DateTime selected)
            return;

        if (!_isSelectingEnd)
        {
            // First click: set start date
            _isSelectingEnd = true;
            StartDate = selected;
            EndDate = null;
            _calendar.SelectedDate = null;
            UpdatePseudoClasses();
        }
        else
        {
            // Second click: set end date
            _isSelectingEnd = false;

            if (selected >= StartDate)
            {
                EndDate = selected;
            }
            else
            {
                // Swap if end is before start
                EndDate = StartDate;
                StartDate = selected;
            }

            IsDropDownOpen = false;
            UpdatePseudoClasses();
        }
    }

    private void OnRangeChanged()
    {
        UpdateDisplayText();
        UpdatePseudoClasses();
        DateRangeChanged?.Invoke(this, new DateRangeChangedEventArgs(StartDate, EndDate));
    }

    private void OnIsDropDownOpenChanged()
    {
        if (!IsDropDownOpen)
        {
            _isSelectingEnd = false;
        }
        UpdatePseudoClasses();
    }

    private void UpdateDisplayText()
    {
        if (_textBox == null) return;

        if (StartDate.HasValue && EndDate.HasValue)
        {
            _textBox.Text = $"{StartDate.Value.ToString(Format)}{Separator}{EndDate.Value.ToString(Format)}";
        }
        else if (StartDate.HasValue)
        {
            _textBox.Text = $"{StartDate.Value.ToString(Format)}{Separator}...";
        }
        else
        {
            _textBox.Text = null;
        }
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(":open", IsDropDownOpen);
        PseudoClasses.Set(":range-selected", StartDate.HasValue && EndDate.HasValue);
        PseudoClasses.Set(":selecting-end", _isSelectingEnd);
    }
}

/// <summary>
/// A preset date range shortcut for <see cref="DateRangePicker"/>.
/// </summary>
public class DateRangePreset
{
    /// <summary>
    /// Gets or sets the display label.
    /// </summary>
    public string Label { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the start date.
    /// </summary>
    public DateTime Start { get; set; }

    /// <summary>
    /// Gets or sets the end date.
    /// </summary>
    public DateTime End { get; set; }
}

/// <summary>
/// Event arguments for <see cref="DateRangePicker.DateRangeChanged"/>.
/// </summary>
public class DateRangeChangedEventArgs : EventArgs
{
    public DateTime? StartDate { get; }
    public DateTime? EndDate { get; }

    public DateRangeChangedEventArgs(DateTime? startDate, DateTime? endDate)
    {
        StartDate = startDate;
        EndDate = endDate;
    }
}
