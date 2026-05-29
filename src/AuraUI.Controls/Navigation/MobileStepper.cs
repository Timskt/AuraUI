using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Media;

namespace AuraUI.Controls.Navigation;

/// <summary>
/// Specifies the visual variant of the stepper.
/// </summary>
public enum StepperVariant
{
    Dots,
    Progress,
    Number
}

/// <summary>
/// A mobile-style stepper/step indicator control. Displays progress through a
/// sequence of steps using dots, a progress bar, or numbered circles.
/// </summary>
[PseudoClasses(":dots", ":progress", ":number", ":horizontal", ":vertical", ":first", ":last", ":completed")]
public class MobileStepper : TemplatedControl
{
    /// <summary>
    /// Defines the <see cref="Steps"/> styled property.
    /// </summary>
    public static readonly StyledProperty<ObservableCollection<string>?> StepsProperty =
        AvaloniaProperty.Register<MobileStepper, ObservableCollection<string>?>(nameof(Steps));

    /// <summary>
    /// Defines the <see cref="CurrentStep"/> styled property.
    /// Zero-based index of the current step.
    /// </summary>
    public static readonly StyledProperty<int> CurrentStepProperty =
        AvaloniaProperty.Register<MobileStepper, int>(nameof(CurrentStep));

    /// <summary>
    /// Defines the <see cref="StepperOrientation"/> styled property.
    /// </summary>
    public static readonly StyledProperty<Orientation> StepperOrientationProperty =
        AvaloniaProperty.Register<MobileStepper, Orientation>(nameof(StepperOrientation), Orientation.Horizontal);

    /// <summary>
    /// Defines the <see cref="ShowLabels"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> ShowLabelsProperty =
        AvaloniaProperty.Register<MobileStepper, bool>(nameof(ShowLabels));

    /// <summary>
    /// Defines the <see cref="Variant"/> styled property.
    /// </summary>
    public static readonly StyledProperty<StepperVariant> VariantProperty =
        AvaloniaProperty.Register<MobileStepper, StepperVariant>(nameof(Variant), StepperVariant.Dots);

    /// <summary>
    /// Defines the <see cref="ActiveColor"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> ActiveColorProperty =
        AvaloniaProperty.Register<MobileStepper, IBrush?>(nameof(ActiveColor));

    /// <summary>
    /// Defines the <see cref="InactiveColor"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> InactiveColorProperty =
        AvaloniaProperty.Register<MobileStepper, IBrush?>(nameof(InactiveColor));

    /// <summary>
    /// Defines the <see cref="CompletedColor"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> CompletedColorProperty =
        AvaloniaProperty.Register<MobileStepper, IBrush?>(nameof(CompletedColor));

    /// <summary>
    /// Defines the <see cref="StepSize"/> styled property.
    /// Size of each step indicator (dot/circle diameter).
    /// </summary>
    public static readonly StyledProperty<double> StepSizeProperty =
        AvaloniaProperty.Register<MobileStepper, double>(nameof(StepSize), 12);

    /// <summary>
    /// Defines the <see cref="StepSpacing"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> StepSpacingProperty =
        AvaloniaProperty.Register<MobileStepper, double>(nameof(StepSpacing), 8);

    /// <summary>
    /// Defines the <see cref="ProgressThickness"/> styled property.
    /// Thickness of the progress bar line.
    /// </summary>
    public static readonly StyledProperty<double> ProgressThicknessProperty =
        AvaloniaProperty.Register<MobileStepper, double>(nameof(ProgressThickness), 4);

    static MobileStepper()
    {
        VariantProperty.Changed.AddClassHandler<MobileStepper>((x, _) => x.UpdatePseudoClasses());
        StepperOrientationProperty.Changed.AddClassHandler<MobileStepper>((x, _) => x.UpdatePseudoClasses());
        CurrentStepProperty.Changed.AddClassHandler<MobileStepper>((x, _) => x.UpdatePseudoClasses());
    }

    /// <summary>
    /// Gets or sets the step labels.
    /// </summary>
    public ObservableCollection<string>? Steps
    {
        get => GetValue(StepsProperty);
        set => SetValue(StepsProperty, value);
    }

    /// <summary>
    /// Gets or sets the current step index (zero-based).
    /// </summary>
    public int CurrentStep
    {
        get => GetValue(CurrentStepProperty);
        set => SetValue(CurrentStepProperty, value);
    }

    /// <summary>
    /// Gets or sets the orientation of the stepper.
    /// </summary>
    public Orientation StepperOrientation
    {
        get => GetValue(StepperOrientationProperty);
        set => SetValue(StepperOrientationProperty, value);
    }

    /// <summary>
    /// Gets or sets whether step labels are displayed.
    /// </summary>
    public bool ShowLabels
    {
        get => GetValue(ShowLabelsProperty);
        set => SetValue(ShowLabelsProperty, value);
    }

    /// <summary>
    /// Gets or sets the visual variant.
    /// </summary>
    public StepperVariant Variant
    {
        get => GetValue(VariantProperty);
        set => SetValue(VariantProperty, value);
    }

    /// <summary>
    /// Gets or sets the active step color.
    /// </summary>
    public IBrush? ActiveColor
    {
        get => GetValue(ActiveColorProperty);
        set => SetValue(ActiveColorProperty, value);
    }

    /// <summary>
    /// Gets or sets the inactive step color.
    /// </summary>
    public IBrush? InactiveColor
    {
        get => GetValue(InactiveColorProperty);
        set => SetValue(InactiveColorProperty, value);
    }

    /// <summary>
    /// Gets or sets the completed step color.
    /// </summary>
    public IBrush? CompletedColor
    {
        get => GetValue(CompletedColorProperty);
        set => SetValue(CompletedColorProperty, value);
    }

    /// <summary>
    /// Gets or sets the step indicator size.
    /// </summary>
    public double StepSize
    {
        get => GetValue(StepSizeProperty);
        set => SetValue(StepSizeProperty, value);
    }

    /// <summary>
    /// Gets or sets the spacing between steps.
    /// </summary>
    public double StepSpacing
    {
        get => GetValue(StepSpacingProperty);
        set => SetValue(StepSpacingProperty, value);
    }

    /// <summary>
    /// Gets or sets the thickness of the progress bar (for Progress variant).
    /// </summary>
    public double ProgressThickness
    {
        get => GetValue(ProgressThicknessProperty);
        set => SetValue(ProgressThicknessProperty, value);
    }

    /// <summary>
    /// Gets the total number of steps.
    /// </summary>
    public int TotalSteps => Steps?.Count ?? 0;

    /// <summary>
    /// Gets the progress as a fraction (0.0 to 1.0).
    /// </summary>
    public double ProgressFraction
    {
        get
        {
            var total = TotalSteps;
            if (total <= 1) return 1.0;
            return (double)CurrentStep / (total - 1);
        }
    }

    /// <summary>
    /// Gets the label for the current step.
    /// </summary>
    public string? CurrentStepLabel
    {
        get
        {
            var steps = Steps;
            if (steps == null || CurrentStep < 0 || CurrentStep >= steps.Count)
                return null;
            return steps[CurrentStep];
        }
    }

    /// <summary>
    /// Advances to the next step if possible.
    /// </summary>
    /// <returns>True if advanced, false if already at the last step.</returns>
    public bool NextStep()
    {
        var total = TotalSteps;
        if (CurrentStep >= total - 1) return false;
        CurrentStep++;
        return true;
    }

    /// <summary>
    /// Goes back to the previous step if possible.
    /// </summary>
    /// <returns>True if moved back, false if already at the first step.</returns>
    public bool PreviousStep()
    {
        if (CurrentStep <= 0) return false;
        CurrentStep--;
        return true;
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        UpdatePseudoClasses();
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(":dots", Variant == StepperVariant.Dots);
        PseudoClasses.Set(":progress", Variant == StepperVariant.Progress);
        PseudoClasses.Set(":number", Variant == StepperVariant.Number);
        PseudoClasses.Set(":horizontal", StepperOrientation == Orientation.Horizontal);
        PseudoClasses.Set(":vertical", StepperOrientation == Orientation.Vertical);
        PseudoClasses.Set(":first", CurrentStep == 0);
        PseudoClasses.Set(":last", CurrentStep == TotalSteps - 1);
        PseudoClasses.Set(":completed", CurrentStep >= TotalSteps - 1 && TotalSteps > 0);
    }
}
