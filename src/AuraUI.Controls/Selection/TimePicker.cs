using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace AuraUI.Controls.Selection;

/// <summary>
/// Specifies the time picker display mode.
/// </summary>
public enum TimePickerMode
{
    /// <summary>12-hour clock with AM/PM selector.</summary>
    TwelveHour,
    /// <summary>24-hour clock.</summary>
    TwentyFourHour
}

/// <summary>
/// A time picker control that allows selecting hours, minutes, and optionally seconds.
/// Supports 12-hour (AM/PM) and 24-hour modes, customizable step intervals,
/// and time range constraints.
/// </summary>
[TemplatePart("PART_TextBox", typeof(TextBox))]
[TemplatePart("PART_DropdownButton", typeof(Button))]
[TemplatePart("PART_Popup", typeof(Popup))]
[TemplatePart("PART_HourSelector", typeof(ListBox))]
[TemplatePart("PART_MinuteSelector", typeof(ListBox))]
[TemplatePart("PART_SecondSelector", typeof(ListBox))]
[TemplatePart("PART_AmPmSelector", typeof(ListBox))]
[PseudoClasses(":open", ":selected", ":12hour", ":24hour")]
public class TimePicker : TemplatedControl
{
    private TextBox? _textBox;
    private Popup? _popup;
    private ListBox? _hourSelector;
    private ListBox? _minuteSelector;
    private ListBox? _secondSelector;
    private ListBox? _amPmSelector;

    /// <summary>
    /// Defines the <see cref="SelectedTime"/> styled property.
    /// </summary>
    public static readonly StyledProperty<TimeSpan?> SelectedTimeProperty =
        AvaloniaProperty.Register<TimePicker, TimeSpan?>(nameof(SelectedTime));

    /// <summary>
    /// Defines the <see cref="MinTime"/> styled property.
    /// </summary>
    public static readonly StyledProperty<TimeSpan> MinTimeProperty =
        AvaloniaProperty.Register<TimePicker, TimeSpan>(
            nameof(MinTime),
            TimeSpan.Zero);

    /// <summary>
    /// Defines the <see cref="MaxTime"/> styled property.
    /// </summary>
    public static readonly StyledProperty<TimeSpan> MaxTimeProperty =
        AvaloniaProperty.Register<TimePicker, TimeSpan>(
            nameof(MaxTime),
            new TimeSpan(23, 59, 59));

    /// <summary>
    /// Defines the <see cref="Mode"/> styled property.
    /// </summary>
    public static readonly StyledProperty<TimePickerMode> ModeProperty =
        AvaloniaProperty.Register<TimePicker, TimePickerMode>(
            nameof(Mode),
            TimePickerMode.TwentyFourHour);

    /// <summary>
    /// Defines the <see cref="Format"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string> FormatProperty =
        AvaloniaProperty.Register<TimePicker, string>(nameof(Format), "HH:mm");

    /// <summary>
    /// Defines the <see cref="PlaceholderText"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> PlaceholderTextProperty =
        AvaloniaProperty.Register<TimePicker, string?>(
            nameof(PlaceholderText),
            "Select time...");

    /// <summary>
    /// Defines the <see cref="IsDropDownOpen"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsDropDownOpenProperty =
        AvaloniaProperty.Register<TimePicker, bool>(nameof(IsDropDownOpen));

    /// <summary>
    /// Defines the <see cref="ShowSeconds"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> ShowSecondsProperty =
        AvaloniaProperty.Register<TimePicker, bool>(nameof(ShowSeconds));

    /// <summary>
    /// Defines the <see cref="MinuteStep"/> styled property.
    /// </summary>
    public static readonly StyledProperty<int> MinuteStepProperty =
        AvaloniaProperty.Register<TimePicker, int>(nameof(MinuteStep), 1);

    /// <summary>
    /// Defines the <see cref="SecondStep"/> styled property.
    /// </summary>
    public static readonly StyledProperty<int> SecondStepProperty =
        AvaloniaProperty.Register<TimePicker, int>(nameof(SecondStep), 1);

    /// <summary>
    /// Defines the <see cref="IsReadOnly"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsReadOnlyProperty =
        AvaloniaProperty.Register<TimePicker, bool>(nameof(IsReadOnly));

    static TimePicker()
    {
        SelectedTimeProperty.Changed.AddClassHandler<TimePicker>((x, _) => x.OnSelectedTimeChanged());
        IsDropDownOpenProperty.Changed.AddClassHandler<TimePicker>((x, _) => x.OnIsDropDownOpenChanged());
        ModeProperty.Changed.AddClassHandler<TimePicker>((x, _) => x.OnModeChanged());
    }

    /// <summary>
    /// Occurs when the selected time changes.
    /// </summary>
    public event EventHandler<TimeChangedEventArgs>? TimeChanged;

    /// <summary>
    /// Gets or sets the selected time.
    /// </summary>
    public TimeSpan? SelectedTime
    {
        get => GetValue(SelectedTimeProperty);
        set => SetValue(SelectedTimeProperty, value);
    }

    /// <summary>
    /// Gets or sets the minimum selectable time.
    /// </summary>
    public TimeSpan MinTime
    {
        get => GetValue(MinTimeProperty);
        set => SetValue(MinTimeProperty, value);
    }

    /// <summary>
    /// Gets or sets the maximum selectable time.
    /// </summary>
    public TimeSpan MaxTime
    {
        get => GetValue(MaxTimeProperty);
        set => SetValue(MaxTimeProperty, value);
    }

    /// <summary>
    /// Gets or sets the display mode (12-hour or 24-hour).
    /// </summary>
    public TimePickerMode Mode
    {
        get => GetValue(ModeProperty);
        set => SetValue(ModeProperty, value);
    }

    /// <summary>
    /// Gets or sets the time format string.
    /// </summary>
    public string Format
    {
        get => GetValue(FormatProperty);
        set => SetValue(FormatProperty, value);
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
    /// Gets or sets whether to show seconds selector.
    /// </summary>
    public bool ShowSeconds
    {
        get => GetValue(ShowSecondsProperty);
        set => SetValue(ShowSecondsProperty, value);
    }

    /// <summary>
    /// Gets or sets the minute step interval.
    /// </summary>
    public int MinuteStep
    {
        get => GetValue(MinuteStepProperty);
        set => SetValue(MinuteStepProperty, value);
    }

    /// <summary>
    /// Gets or sets the second step interval.
    /// </summary>
    public int SecondStep
    {
        get => GetValue(SecondStepProperty);
        set => SetValue(SecondStepProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the picker is read-only.
    /// </summary>
    public bool IsReadOnly
    {
        get => GetValue(IsReadOnlyProperty);
        set => SetValue(IsReadOnlyProperty, value);
    }

    /// <summary>
    /// Clears the selected time.
    /// </summary>
    public void Clear()
    {
        SelectedTime = null;
    }

    /// <summary>
    /// Sets the time from hour and minute values.
    /// </summary>
    public void SetTime(int hour, int minute, int second = 0)
    {
        SelectedTime = new TimeSpan(
            Math.Clamp(hour, 0, 23),
            Math.Clamp(minute, 0, 59),
            Math.Clamp(second, 0, 59));
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        DetachParts();

        _textBox = e.NameScope.Find<TextBox>("PART_TextBox");
        _popup = e.NameScope.Find<Popup>("PART_Popup");
        _hourSelector = e.NameScope.Find<ListBox>("PART_HourSelector");
        _minuteSelector = e.NameScope.Find<ListBox>("PART_MinuteSelector");
        _secondSelector = e.NameScope.Find<ListBox>("PART_SecondSelector");
        _amPmSelector = e.NameScope.Find<ListBox>("PART_AmPmSelector");

        AttachParts();
        PopulateSelectors();
        UpdateDisplayText();
        UpdatePseudoClasses();
    }

    private void AttachParts()
    {
        if (_hourSelector != null)
            _hourSelector.SelectionChanged += OnHourSelected;
        if (_minuteSelector != null)
            _minuteSelector.SelectionChanged += OnMinuteSelected;
        if (_secondSelector != null)
            _secondSelector.SelectionChanged += OnSecondSelected;
        if (_amPmSelector != null)
            _amPmSelector.SelectionChanged += OnAmPmSelected;
    }

    private void DetachParts()
    {
        if (_hourSelector != null)
            _hourSelector.SelectionChanged -= OnHourSelected;
        if (_minuteSelector != null)
            _minuteSelector.SelectionChanged -= OnMinuteSelected;
        if (_secondSelector != null)
            _secondSelector.SelectionChanged -= OnSecondSelected;
        if (_amPmSelector != null)
            _amPmSelector.SelectionChanged -= OnAmPmSelected;
    }

    private void PopulateSelectors()
    {
        if (_hourSelector != null)
        {
            var hours = Mode == TimePickerMode.TwelveHour
                ? Enumerable.Range(1, 12).Select(h => h.ToString("00")).ToList()
                : Enumerable.Range(0, 24).Select(h => h.ToString("00")).ToList();
            _hourSelector.ItemsSource = hours;
        }

        if (_minuteSelector != null)
        {
            var minutes = Enumerable.Range(0, 60)
                .Where(m => m % MinuteStep == 0)
                .Select(m => m.ToString("00"))
                .ToList();
            _minuteSelector.ItemsSource = minutes;
        }

        if (_secondSelector != null)
        {
            var seconds = Enumerable.Range(0, 60)
                .Where(s => s % SecondStep == 0)
                .Select(s => s.ToString("00"))
                .ToList();
            _secondSelector.ItemsSource = seconds;
        }

        if (_amPmSelector != null)
        {
            _amPmSelector.ItemsSource = new[] { "AM", "PM" };
        }
    }

    private void OnHourSelected(object? sender, SelectionChangedEventArgs e)
    {
        if (_hourSelector?.SelectedItem is string hourStr && int.TryParse(hourStr, out var hour))
        {
            var current = SelectedTime ?? TimeSpan.Zero;
            var isPm = Mode == TimePickerMode.TwelveHour && _amPmSelector?.SelectedIndex == 1;

            if (Mode == TimePickerMode.TwelveHour)
            {
                if (isPm && hour < 12) hour += 12;
                if (!isPm && hour == 12) hour = 0;
            }

            SelectedTime = new TimeSpan(hour, current.Minutes, ShowSeconds ? current.Seconds : 0);
        }
    }

    private void OnMinuteSelected(object? sender, SelectionChangedEventArgs e)
    {
        if (_minuteSelector?.SelectedItem is string minStr && int.TryParse(minStr, out var minute))
        {
            var current = SelectedTime ?? TimeSpan.Zero;
            SelectedTime = new TimeSpan(current.Hours, minute, ShowSeconds ? current.Seconds : 0);
        }
    }

    private void OnSecondSelected(object? sender, SelectionChangedEventArgs e)
    {
        if (_secondSelector?.SelectedItem is string secStr && int.TryParse(secStr, out var second))
        {
            var current = SelectedTime ?? TimeSpan.Zero;
            SelectedTime = new TimeSpan(current.Hours, current.Minutes, second);
        }
    }

    private void OnAmPmSelected(object? sender, SelectionChangedEventArgs e)
    {
        if (_amPmSelector == null || SelectedTime == null) return;

        var current = SelectedTime.Value;
        var isPm = _amPmSelector.SelectedIndex == 1;
        var hour = current.Hours;

        if (isPm && hour < 12) hour += 12;
        if (!isPm && hour >= 12) hour -= 12;

        SelectedTime = new TimeSpan(hour, current.Minutes, current.Seconds);
    }

    private void OnSelectedTimeChanged()
    {
        UpdateDisplayText();
        UpdatePseudoClasses();
        TimeChanged?.Invoke(this, new TimeChangedEventArgs(SelectedTime));
    }

    private void OnIsDropDownOpenChanged()
    {
        UpdatePseudoClasses();
    }

    private void OnModeChanged()
    {
        PopulateSelectors();
        UpdateDisplayText();
        UpdatePseudoClasses();
    }

    private void UpdateDisplayText()
    {
        if (_textBox == null) return;

        if (SelectedTime.HasValue)
        {
            var time = SelectedTime.Value;
            if (Mode == TimePickerMode.TwelveHour)
            {
                var hour12 = time.Hours % 12;
                if (hour12 == 0) hour12 = 12;
                var amPm = time.Hours >= 12 ? "PM" : "AM";
                _textBox.Text = ShowSeconds
                    ? $"{hour12:00}:{time.Minutes:00}:{time.Seconds:00} {amPm}"
                    : $"{hour12:00}:{time.Minutes:00} {amPm}";
            }
            else
            {
                _textBox.Text = ShowSeconds
                    ? $"{time.Hours:00}:{time.Minutes:00}:{time.Seconds:00}"
                    : $"{time.Hours:00}:{time.Minutes:00}";
            }
        }
        else
        {
            _textBox.Text = null;
        }
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(":open", IsDropDownOpen);
        PseudoClasses.Set(":selected", SelectedTime.HasValue);
        PseudoClasses.Set(":12hour", Mode == TimePickerMode.TwelveHour);
        PseudoClasses.Set(":24hour", Mode == TimePickerMode.TwentyFourHour);
    }
}

/// <summary>
/// Event arguments for <see cref="TimePicker.TimeChanged"/>.
/// </summary>
public class TimeChangedEventArgs : EventArgs
{
    public TimeSpan? Time { get; }

    public TimeChangedEventArgs(TimeSpan? time)
    {
        Time = time;
    }
}
