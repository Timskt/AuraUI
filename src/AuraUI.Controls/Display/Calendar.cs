using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;

namespace AuraUI.Controls.Display;

/// <summary>
/// Specifies the calendar view mode.
/// </summary>
public enum CalendarMode
{
    /// <summary>Month view showing days.</summary>
    Month,
    /// <summary>Year view showing months.</summary>
    Year
}

/// <summary>
/// A calendar control displaying a month or year view with date selection.
/// Supports custom cell rendering and header display.
/// Inspired by Ant Design's Calendar component.
/// </summary>
[TemplatePart("PART_Header", typeof(Panel))]
[TemplatePart("PART_PreviousButton", typeof(Button))]
[TemplatePart("PART_NextButton", typeof(Button))]
[TemplatePart("PART_MonthYearButton", typeof(Button))]
[PseudoClasses(":month-mode", ":year-mode", ":has-selection")]
public class Calendar : TemplatedControl
{
    private Button? _previousButton;
    private Button? _nextButton;
    private Button? _monthYearButton;

    /// <summary>
    /// Defines the <see cref="Value"/> styled property.
    /// </summary>
    public static readonly StyledProperty<DateTime> ValueProperty =
        AvaloniaProperty.Register<Calendar, DateTime>(nameof(Value), DateTime.Today);

    /// <summary>
    /// Defines the <see cref="DisplayDate"/> styled property.
    /// The month/year currently being displayed.
    /// </summary>
    public static readonly StyledProperty<DateTime> DisplayDateProperty =
        AvaloniaProperty.Register<Calendar, DateTime>(nameof(DisplayDate), DateTime.Today);

    /// <summary>
    /// Defines the <see cref="Mode"/> styled property.
    /// </summary>
    public static readonly StyledProperty<CalendarMode> ModeProperty =
        AvaloniaProperty.Register<Calendar, CalendarMode>(nameof(Mode), CalendarMode.Month);

    /// <summary>
    /// Defines the <see cref="ShowHeader"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> ShowHeaderProperty =
        AvaloniaProperty.Register<Calendar, bool>(nameof(ShowHeader), true);

    /// <summary>
    /// Defines the <see cref="FirstDayOfWeek"/> styled property.
    /// </summary>
    public static readonly StyledProperty<DayOfWeek> FirstDayOfWeekProperty =
        AvaloniaProperty.Register<Calendar, DayOfWeek>(
            nameof(FirstDayOfWeek), DayOfWeek.Sunday);

    /// <summary>
    /// Defines the <see cref="IsReadOnly"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsReadOnlyProperty =
        AvaloniaProperty.Register<Calendar, bool>(nameof(IsReadOnly));

    /// <summary>
    /// Defines the <see cref="DisplayMonth"/> readback property.
    /// </summary>
    public static readonly DirectProperty<Calendar, int> DisplayMonthProperty =
        AvaloniaProperty.RegisterDirect<Calendar, int>(
            nameof(DisplayMonth), o => o.DisplayMonth);

    /// <summary>
    /// Defines the <see cref="DisplayYear"/> readback property.
    /// </summary>
    public static readonly DirectProperty<Calendar, int> DisplayYearProperty =
        AvaloniaProperty.RegisterDirect<Calendar, int>(
            nameof(DisplayYear), o => o.DisplayYear);

    private int _displayMonth;
    private int _displayYear;

    static Calendar()
    {
        ValueProperty.Changed.AddClassHandler<Calendar>((x, _) => x.UpdatePseudoClasses());
        ModeProperty.Changed.AddClassHandler<Calendar>((x, _) => x.UpdatePseudoClasses());
        DisplayDateProperty.Changed.AddClassHandler<Calendar>((x, _) => x.OnDisplayDateChanged());
    }

    /// <summary>
    /// Gets or sets the selected date.
    /// </summary>
    public DateTime Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    /// <summary>
    /// Gets or sets the date whose month/year is being displayed.
    /// </summary>
    public DateTime DisplayDate
    {
        get => GetValue(DisplayDateProperty);
        set => SetValue(DisplayDateProperty, value);
    }

    /// <summary>
    /// Gets or sets the calendar view mode.
    /// </summary>
    public CalendarMode Mode
    {
        get => GetValue(ModeProperty);
        set => SetValue(ModeProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the header is visible.
    /// </summary>
    public bool ShowHeader
    {
        get => GetValue(ShowHeaderProperty);
        set => SetValue(ShowHeaderProperty, value);
    }

    /// <summary>
    /// Gets or sets the first day of the week.
    /// </summary>
    public DayOfWeek FirstDayOfWeek
    {
        get => GetValue(FirstDayOfWeekProperty);
        set => SetValue(FirstDayOfWeekProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the calendar is read-only.
    /// </summary>
    public bool IsReadOnly
    {
        get => GetValue(IsReadOnlyProperty);
        set => SetValue(IsReadOnlyProperty, value);
    }

    /// <summary>
    /// Gets the currently displayed month (1-12).
    /// </summary>
    public int DisplayMonth
    {
        get => _displayMonth;
        private set => SetAndRaise(DisplayMonthProperty, ref _displayMonth, value);
    }

    /// <summary>
    /// Gets the currently displayed year.
    /// </summary>
    public int DisplayYear
    {
        get => _displayYear;
        private set => SetAndRaise(DisplayYearProperty, ref _displayYear, value);
    }

    /// <summary>
    /// Occurs when the selected date changes.
    /// </summary>
    public event EventHandler<SelectionChangedEventArgs>? SelectedDateChanged;

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        _previousButton = e.NameScope.Find<Button>("PART_PreviousButton");
        _nextButton = e.NameScope.Find<Button>("PART_NextButton");
        _monthYearButton = e.NameScope.Find<Button>("PART_MonthYearButton");

        if (_previousButton != null)
        {
            _previousButton.Click += (_, _) => NavigatePrevious();
        }

        if (_nextButton != null)
        {
            _nextButton.Click += (_, _) => NavigateNext();
        }

        if (_monthYearButton != null)
        {
            _monthYearButton.Click += (_, _) => ToggleMode();
        }

        OnDisplayDateChanged();
        UpdatePseudoClasses();
    }

    /// <summary>
    /// Navigates to the previous month (or year in year mode).
    /// </summary>
    public void NavigatePrevious()
    {
        if (Mode == CalendarMode.Month)
        {
            DisplayDate = DisplayDate.AddMonths(-1);
        }
        else
        {
            DisplayDate = DisplayDate.AddYears(-1);
        }
    }

    /// <summary>
    /// Navigates to the next month (or year in year mode).
    /// </summary>
    public void NavigateNext()
    {
        if (Mode == CalendarMode.Month)
        {
            DisplayDate = DisplayDate.AddMonths(1);
        }
        else
        {
            DisplayDate = DisplayDate.AddYears(1);
        }
    }

    /// <summary>
    /// Toggles between month and year view.
    /// </summary>
    public void ToggleMode()
    {
        Mode = Mode == CalendarMode.Month ? CalendarMode.Year : CalendarMode.Month;
    }

    /// <summary>
    /// Navigates to today's date.
    /// </summary>
    public void GoToToday()
    {
        DisplayDate = DateTime.Today;
        Value = DateTime.Today;
    }

    private void OnDisplayDateChanged()
    {
        DisplayMonth = DisplayDate.Month;
        DisplayYear = DisplayDate.Year;
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(":month-mode", Mode == CalendarMode.Month);
        PseudoClasses.Set(":year-mode", Mode == CalendarMode.Year);
        PseudoClasses.Set(":has-selection", Value != default);
    }
}
