using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace AuraUI.Controls.Input;

/// <summary>
/// Defines button placement relative to the numeric input.
/// </summary>
public enum NumericButtonPlacement
{
    Right,
    Left,
    Both
}

/// <summary>
/// Enhanced numeric up/down control styled with AuraUI design tokens.
///
/// Supports size classes: .compact, .expanded
/// Pseudo-classes: :min (at minimum), :max (at maximum)
///
/// Template parts:
///   PART_IncreaseButton - Button to increase value
///   PART_DecreaseButton - Button to decrease value
/// </summary>
public class AuraNumericUpDown : NumericUpDown
{
    private Button? _increaseButton;
    private Button? _decreaseButton;

    /// <summary>
    /// Defines the <see cref="ShowButtons"/> property.
    /// </summary>
    public static readonly StyledProperty<bool> ShowButtonsProperty =
        AvaloniaProperty.Register<AuraNumericUpDown, bool>(
            nameof(ShowButtons),
            defaultValue: true);

    /// <summary>
    /// Defines the <see cref="ButtonPlacement"/> property.
    /// </summary>
    public static readonly StyledProperty<NumericButtonPlacement> ButtonPlacementProperty =
        AvaloniaProperty.Register<AuraNumericUpDown, NumericButtonPlacement>(
            nameof(ButtonPlacement),
            defaultValue: NumericButtonPlacement.Right);

    /// <summary>
    /// Defines the <see cref="IncreaseIcon"/> property.
    /// </summary>
    public static readonly StyledProperty<object?> IncreaseIconProperty =
        AvaloniaProperty.Register<AuraNumericUpDown, object?>(
            nameof(IncreaseIcon));

    /// <summary>
    /// Defines the <see cref="DecreaseIcon"/> property.
    /// </summary>
    public static readonly StyledProperty<object?> DecreaseIconProperty =
        AvaloniaProperty.Register<AuraNumericUpDown, object?>(
            nameof(DecreaseIcon));

    /// <summary>
    /// Defines the <see cref="AccelerationInterval"/> property.
    /// Interval in milliseconds for acceleration when holding buttons.
    /// </summary>
    public static readonly StyledProperty<int> AccelerationIntervalProperty =
        AvaloniaProperty.Register<AuraNumericUpDown, int>(
            nameof(AccelerationInterval),
            defaultValue: 100);

    /// <summary>
    /// Gets or sets whether the increase/decrease buttons are shown.
    /// </summary>
    public bool ShowButtons
    {
        get => GetValue(ShowButtonsProperty);
        set => SetValue(ShowButtonsProperty, value);
    }

    /// <summary>
    /// Gets or sets the placement of the increase/decrease buttons.
    /// </summary>
    public NumericButtonPlacement ButtonPlacement
    {
        get => GetValue(ButtonPlacementProperty);
        set => SetValue(ButtonPlacementProperty, value);
    }

    /// <summary>
    /// Gets or sets the icon for the increase button.
    /// </summary>
    public object? IncreaseIcon
    {
        get => GetValue(IncreaseIconProperty);
        set => SetValue(IncreaseIconProperty, value);
    }

    /// <summary>
    /// Gets or sets the icon for the decrease button.
    /// </summary>
    public object? DecreaseIcon
    {
        get => GetValue(DecreaseIconProperty);
        set => SetValue(DecreaseIconProperty, value);
    }

    /// <summary>
    /// Gets or sets the acceleration interval in milliseconds for repeated button presses.
    /// </summary>
    public int AccelerationInterval
    {
        get => GetValue(AccelerationIntervalProperty);
        set => SetValue(AccelerationIntervalProperty, value);
    }

    static AuraNumericUpDown()
    {
        AffectsMeasure<AuraNumericUpDown>(ShowButtonsProperty, ButtonPlacementProperty);

        ValueProperty.Changed.AddClassHandler<AuraNumericUpDown>((x, e) => x.OnValueChanged(e));
        MinimumProperty.Changed.AddClassHandler<AuraNumericUpDown>((x, _) => x.UpdateBoundaryPseudoClasses());
        MaximumProperty.Changed.AddClassHandler<AuraNumericUpDown>((x, _) => x.UpdateBoundaryPseudoClasses());
        ShowButtonsProperty.Changed.AddClassHandler<AuraNumericUpDown>((x, _) => x.UpdateButtonVisibility());
        ButtonPlacementProperty.Changed.AddClassHandler<AuraNumericUpDown>((x, _) => x.UpdateButtonPlacement());
    }

    protected override Type StyleKeyOverride => typeof(NumericUpDown);

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        _increaseButton = e.NameScope.Find<Button>("PART_IncreaseButton");
        _decreaseButton = e.NameScope.Find<Button>("PART_DecreaseButton");

        UpdateBoundaryPseudoClasses();
        UpdateButtonVisibility();
        UpdateButtonPlacement();
    }

    private void OnValueChanged(AvaloniaPropertyChangedEventArgs e)
    {
        UpdateBoundaryPseudoClasses();
    }

    private void UpdateBoundaryPseudoClasses()
    {
        var currentValue = Value;
        var minimum = Minimum;
        var maximum = Maximum;

        var isAtMin = currentValue <= minimum;
        var isAtMax = currentValue >= maximum;

        PseudoClasses.Set("min", isAtMin);
        PseudoClasses.Set("max", isAtMax);
    }

    private void UpdateButtonVisibility()
    {
        var show = ShowButtons;

        if (_increaseButton is not null)
        {
            _increaseButton.IsVisible = show;
        }

        if (_decreaseButton is not null)
        {
            _decreaseButton.IsVisible = show;
        }
    }

    private void UpdateButtonPlacement()
    {
        var placement = ButtonPlacement;

        // Update the placement class for styling
        Classes.Remove("placement-left");
        Classes.Remove("placement-right");
        Classes.Remove("placement-both");

        var placementClass = placement switch
        {
            NumericButtonPlacement.Left => "placement-left",
            NumericButtonPlacement.Right => "placement-right",
            NumericButtonPlacement.Both => "placement-both",
            _ => "placement-right"
        };

        Classes.Add(placementClass);

        // When using Both placement, ensure both buttons are visible
        if (placement == NumericButtonPlacement.Both)
        {
            if (_increaseButton is not null) _increaseButton.IsVisible = true;
            if (_decreaseButton is not null) _decreaseButton.IsVisible = true;
        }
        else if (placement == NumericButtonPlacement.Left)
        {
            // Buttons on left side
            if (_increaseButton is not null) _increaseButton.IsVisible = ShowButtons;
            if (_decreaseButton is not null) _decreaseButton.IsVisible = ShowButtons;
        }
    }
}
