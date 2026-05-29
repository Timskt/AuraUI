using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace AuraUI.Controls.Selection;

/// <summary>
/// A dual-thumb slider control for selecting a numeric range between a minimum and maximum value.
/// Supports tick snapping and step increments.
/// </summary>
[TemplatePart("PART_Track", typeof(Border))]
[TemplatePart("PART_LowerThumb", typeof(Thumb))]
[TemplatePart("PART_UpperThumb", typeof(Thumb))]
[TemplatePart("PART_RangeFill", typeof(Border))]
[PseudoClasses(":pressed", ":disabled")]
public class RangeSlider : TemplatedControl
{
    private Border? _track;
    private Thumb? _lowerThumb;
    private Thumb? _upperThumb;
    private Border? _rangeFill;
#pragma warning disable CS0414
    private bool _isDragging; // Preserved for drag state tracking infrastructure

    /// <summary>
    /// Defines the <see cref="Minimum"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> MinimumProperty =
        AvaloniaProperty.Register<RangeSlider, double>(nameof(Minimum), 0d);

    /// <summary>
    /// Defines the <see cref="Maximum"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> MaximumProperty =
        AvaloniaProperty.Register<RangeSlider, double>(nameof(Maximum), 100d);

    /// <summary>
    /// Defines the <see cref="LowerValue"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> LowerValueProperty =
        AvaloniaProperty.Register<RangeSlider, double>(nameof(LowerValue), 20d);

    /// <summary>
    /// Defines the <see cref="UpperValue"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> UpperValueProperty =
        AvaloniaProperty.Register<RangeSlider, double>(nameof(UpperValue), 80d);

    /// <summary>
    /// Defines the <see cref="Step"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> StepProperty =
        AvaloniaProperty.Register<RangeSlider, double>(nameof(Step), 1d);

    /// <summary>
    /// Defines the <see cref="IsSnapToTick"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsSnapToTickProperty =
        AvaloniaProperty.Register<RangeSlider, bool>(nameof(IsSnapToTick), false);

    static RangeSlider()
    {
        MinimumProperty.Changed.AddClassHandler<RangeSlider>((x, _) => x.OnRangeChanged());
        MaximumProperty.Changed.AddClassHandler<RangeSlider>((x, _) => x.OnRangeChanged());
        LowerValueProperty.Changed.AddClassHandler<RangeSlider>((x, e) => x.OnLowerValueChanged(e));
        UpperValueProperty.Changed.AddClassHandler<RangeSlider>((x, e) => x.OnUpperValueChanged(e));
        IsSnapToTickProperty.Changed.AddClassHandler<RangeSlider>((x, _) => x.OnRangeChanged());
        StepProperty.Changed.AddClassHandler<RangeSlider>((x, _) => x.OnRangeChanged());
    }

    /// <summary>
    /// Gets or sets the minimum value of the range.
    /// </summary>
    public double Minimum
    {
        get => GetValue(MinimumProperty);
        set => SetValue(MinimumProperty, value);
    }

    /// <summary>
    /// Gets or sets the maximum value of the range.
    /// </summary>
    public double Maximum
    {
        get => GetValue(MaximumProperty);
        set => SetValue(MaximumProperty, value);
    }

    /// <summary>
    /// Gets or sets the lower (start) value of the selected range.
    /// </summary>
    public double LowerValue
    {
        get => GetValue(LowerValueProperty);
        set => SetValue(LowerValueProperty, value);
    }

    /// <summary>
    /// Gets or sets the upper (end) value of the selected range.
    /// </summary>
    public double UpperValue
    {
        get => GetValue(UpperValueProperty);
        set => SetValue(UpperValueProperty, value);
    }

    /// <summary>
    /// Gets or sets the step increment for value changes.
    /// </summary>
    public double Step
    {
        get => GetValue(StepProperty);
        set => SetValue(StepProperty, value);
    }

    /// <summary>
    /// Gets or sets whether values snap to the nearest step increment.
    /// </summary>
    public bool IsSnapToTick
    {
        get => GetValue(IsSnapToTickProperty);
        set => SetValue(IsSnapToTickProperty, value);
    }

    /// <summary>
    /// Occurs when either the lower or upper range value changes.
    /// </summary>
    public event EventHandler<RangeChangedEventArgs>? RangeChanged;

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        if (_lowerThumb != null)
        {
            _lowerThumb.DragStarted -= OnLowerThumbDragStarted;
            _lowerThumb.DragDelta -= OnLowerThumbDragDelta;
            _lowerThumb.DragCompleted -= OnLowerThumbDragCompleted;
        }

        if (_upperThumb != null)
        {
            _upperThumb.DragStarted -= OnUpperThumbDragStarted;
            _upperThumb.DragDelta -= OnUpperThumbDragDelta;
            _upperThumb.DragCompleted -= OnUpperThumbDragCompleted;
        }

        _track = e.NameScope.Find<Border>("PART_Track");
        _lowerThumb = e.NameScope.Find<Thumb>("PART_LowerThumb");
        _upperThumb = e.NameScope.Find<Thumb>("PART_UpperThumb");
        _rangeFill = e.NameScope.Find<Border>("PART_RangeFill");

        if (_lowerThumb != null)
        {
            _lowerThumb.DragStarted += OnLowerThumbDragStarted;
            _lowerThumb.DragDelta += OnLowerThumbDragDelta;
            _lowerThumb.DragCompleted += OnLowerThumbDragCompleted;
        }

        if (_upperThumb != null)
        {
            _upperThumb.DragStarted += OnUpperThumbDragStarted;
            _upperThumb.DragDelta += OnUpperThumbDragDelta;
            _upperThumb.DragCompleted += OnUpperThumbDragCompleted;
        }

        UpdateThumbs();
    }

    protected override void OnSizeChanged(SizeChangedEventArgs e)
    {
        base.OnSizeChanged(e);
        UpdateThumbs();
    }

    private void OnLowerThumbDragStarted(object? sender, VectorEventArgs e)
    {
        _isDragging = true;
        PseudoClasses.Set(":pressed", true);
    }

    private void OnLowerThumbDragDelta(object? sender, VectorEventArgs e)
    {
        if (_track == null) return;
        var trackWidth = _track.Bounds.Width;
        if (trackWidth <= 0) return;

        var delta = e.Vector.X / trackWidth * (Maximum - Minimum);
        var newValue = LowerValue + delta;
        newValue = CoerceValue(newValue);
        newValue = Math.Min(newValue, UpperValue - Step);
        newValue = Math.Max(newValue, Minimum);

        if (IsSnapToTick)
        {
            newValue = SnapToStep(newValue);
        }

        SetCurrentValue(LowerValueProperty, newValue);
    }

    private void OnLowerThumbDragCompleted(object? sender, VectorEventArgs e)
    {
        _isDragging = false;
        PseudoClasses.Set(":pressed", false);
    }

    private void OnUpperThumbDragStarted(object? sender, VectorEventArgs e)
    {
        _isDragging = true;
        PseudoClasses.Set(":pressed", true);
    }

    private void OnUpperThumbDragDelta(object? sender, VectorEventArgs e)
    {
        if (_track == null) return;
        var trackWidth = _track.Bounds.Width;
        if (trackWidth <= 0) return;

        var delta = e.Vector.X / trackWidth * (Maximum - Minimum);
        var newValue = UpperValue + delta;
        newValue = CoerceValue(newValue);
        newValue = Math.Max(newValue, LowerValue + Step);
        newValue = Math.Min(newValue, Maximum);

        if (IsSnapToTick)
        {
            newValue = SnapToStep(newValue);
        }

        SetCurrentValue(UpperValueProperty, newValue);
    }

    private void OnUpperThumbDragCompleted(object? sender, VectorEventArgs e)
    {
        _isDragging = false;
        PseudoClasses.Set(":pressed", false);
    }

    private void OnLowerValueChanged(AvaloniaPropertyChangedEventArgs e)
    {
        UpdateThumbs();
        RangeChanged?.Invoke(this, new RangeChangedEventArgs(
            (double)e.OldValue!, (double)e.NewValue!, UpperValue, UpperValue));
    }

    private void OnUpperValueChanged(AvaloniaPropertyChangedEventArgs e)
    {
        UpdateThumbs();
        RangeChanged?.Invoke(this, new RangeChangedEventArgs(
            LowerValue, LowerValue, (double)e.OldValue!, (double)e.NewValue!));
    }

    private void OnRangeChanged()
    {
        UpdateThumbs();
    }

    private double CoerceValue(double value)
    {
        return Math.Clamp(value, Minimum, Maximum);
    }

    private double SnapToStep(double value)
    {
        if (Step <= 0) return value;
        var steps = Math.Round((value - Minimum) / Step);
        return Minimum + steps * Step;
    }

    private void UpdateThumbs()
    {
        if (_track == null || _lowerThumb == null || _upperThumb == null || _rangeFill == null)
            return;

        var trackWidth = _track.Bounds.Width;
        if (trackWidth <= 0) return;

        var range = Maximum - Minimum;
        if (range <= 0) return;

        var lowerFraction = (LowerValue - Minimum) / range;
        var upperFraction = (UpperValue - Minimum) / range;

        var lowerPos = lowerFraction * trackWidth;
        var upperPos = upperFraction * trackWidth;

        Canvas.SetLeft(_lowerThumb, lowerPos - _lowerThumb.DesiredSize.Width / 2);
        Canvas.SetLeft(_upperThumb, upperPos - _upperThumb.DesiredSize.Width / 2);

        Canvas.SetLeft(_rangeFill, lowerPos);
        _rangeFill.Width = Math.Max(0, upperPos - lowerPos);
    }
}

/// <summary>
/// Event arguments for the <see cref="RangeSlider.RangeChanged"/> event.
/// </summary>
public class RangeChangedEventArgs : EventArgs
{
    public double OldLowerValue { get; }
    public double NewLowerValue { get; }
    public double OldUpperValue { get; }
    public double NewUpperValue { get; }

    public RangeChangedEventArgs(double oldLower, double newLower, double oldUpper, double newUpper)
    {
        OldLowerValue = oldLower;
        NewLowerValue = newLower;
        OldUpperValue = oldUpper;
        NewUpperValue = newUpper;
    }
}
