using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Media;

namespace AuraUI.Controls.Display;

/// <summary>
/// Specifies the status of a step in the step indicator.
/// </summary>
public enum StepStatus
{
    Pending,
    Active,
    Completed,
    Error
}

/// <summary>
/// Specifies the orientation of the step indicator.
/// </summary>
public enum StepOrientation
{
    Horizontal,
    Vertical
}

/// <summary>
/// A step indicator control showing progression through a multi-step process,
/// with status-based colors, connecting lines, and icons.
/// </summary>
[PseudoClasses(":horizontal", ":vertical")]
public class StepIndicator : ItemsControl
{
    /// <summary>
    /// Defines the <see cref="CurrentStep"/> styled property.
    /// </summary>
    public static readonly StyledProperty<int> CurrentStepProperty =
        AvaloniaProperty.Register<StepIndicator, int>(nameof(CurrentStep), coerce: (o, v) => CoerceCurrentStep((StepIndicator)o, v));

    /// <summary>
    /// Defines the <see cref="Orientation"/> styled property.
    /// </summary>
    public static readonly StyledProperty<StepOrientation> OrientationProperty =
        AvaloniaProperty.Register<StepIndicator, StepOrientation>(nameof(StepOrientation), StepOrientation.Horizontal);

    static StepIndicator()
    {
        CurrentStepProperty.Changed.AddClassHandler<StepIndicator>((x, _) => x.OnCurrentStepChanged());
        OrientationProperty.Changed.AddClassHandler<StepIndicator>((x, _) => x.UpdatePseudoClasses());
    }

    /// <summary>
    /// Gets or sets the current (active) step index.
    /// </summary>
    public int CurrentStep
    {
        get => GetValue(CurrentStepProperty);
        set => SetValue(CurrentStepProperty, value);
    }

    /// <summary>
    /// Gets or sets the orientation of the step indicator.
    /// </summary>
    public StepOrientation Orientation
    {
        get => GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        UpdatePseudoClasses();
        UpdateStepStatuses();
    }

    protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
    {
        return new StepIndicatorItem();
    }

    protected override bool NeedsContainerOverride(object? item, int index, out object? recycleKey)
    {
        return NeedsContainer<StepIndicatorItem>(item, out recycleKey);
    }

    protected override void PrepareContainerForItemOverride(Control container, object? item, int index)
    {
        base.PrepareContainerForItemOverride(container, item, index);
        if (container is StepIndicatorItem stepItem)
        {
            stepItem.StepIndex = index;
            stepItem.IsLast = index == (ItemCount - 1);
            UpdateItemStatus(stepItem, index);
        }
    }

    private static int CoerceCurrentStep(StepIndicator sender, int value)
    {
        return Math.Max(0, value);
    }

    private void OnCurrentStepChanged()
    {
        UpdateStepStatuses();
    }

    private void UpdateStepStatuses()
    {
        var count = ItemCount;
        for (var i = 0; i < count; i++)
        {
            if (ContainerFromIndex(i) is StepIndicatorItem stepItem)
            {
                UpdateItemStatus(stepItem, stepItem.StepIndex);
            }
        }
    }

    private void UpdateItemStatus(StepIndicatorItem item, int index)
    {
        if (index < CurrentStep)
            item.Status = StepStatus.Completed;
        else if (index == CurrentStep)
            item.Status = StepStatus.Active;
        else
            item.Status = StepStatus.Pending;
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(":horizontal", Orientation == StepOrientation.Horizontal);
        PseudoClasses.Set(":vertical", Orientation == StepOrientation.Vertical);
    }
}

/// <summary>
/// Represents a single step within a <see cref="StepIndicator"/>.
/// </summary>
[PseudoClasses(":pending", ":active", ":completed", ":error", ":last")]
public class StepIndicatorItem : ContentControl
{
    /// <summary>
    /// Defines the <see cref="Title"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> TitleProperty =
        AvaloniaProperty.Register<StepIndicatorItem, string?>(nameof(Title));

    /// <summary>
    /// Defines the <see cref="Description"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> DescriptionProperty =
        AvaloniaProperty.Register<StepIndicatorItem, string?>(nameof(Description));

    /// <summary>
    /// Defines the <see cref="Icon"/> styled property.
    /// </summary>
    public static readonly StyledProperty<object?> IconProperty =
        AvaloniaProperty.Register<StepIndicatorItem, object?>(nameof(Icon));

    /// <summary>
    /// Defines the <see cref="Status"/> styled property.
    /// </summary>
    public static readonly StyledProperty<StepStatus> StatusProperty =
        AvaloniaProperty.Register<StepIndicatorItem, StepStatus>(nameof(Status));

    /// <summary>
    /// Defines the <see cref="StepIndex"/> styled property.
    /// </summary>
    public static readonly StyledProperty<int> StepIndexProperty =
        AvaloniaProperty.Register<StepIndicatorItem, int>(nameof(StepIndex));

    /// <summary>
    /// Defines the <see cref="IsLast"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsLastProperty =
        AvaloniaProperty.Register<StepIndicatorItem, bool>(nameof(IsLast));

    static StepIndicatorItem()
    {
        StatusProperty.Changed.AddClassHandler<StepIndicatorItem>((x, _) => x.UpdatePseudoClasses());
        IsLastProperty.Changed.AddClassHandler<StepIndicatorItem>((x, _) => x.UpdatePseudoClasses());
    }

    /// <summary>
    /// Gets or sets the title text for this step.
    /// </summary>
    public string? Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    /// <summary>
    /// Gets or sets the description text for this step.
    /// </summary>
    public string? Description
    {
        get => GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }

    /// <summary>
    /// Gets or sets the icon for this step.
    /// </summary>
    public object? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    /// <summary>
    /// Gets or sets the status of this step.
    /// </summary>
    public StepStatus Status
    {
        get => GetValue(StatusProperty);
        set => SetValue(StatusProperty, value);
    }

    /// <summary>
    /// Gets or sets the step index.
    /// </summary>
    public int StepIndex
    {
        get => GetValue(StepIndexProperty);
        set => SetValue(StepIndexProperty, value);
    }

    /// <summary>
    /// Gets or sets whether this is the last step.
    /// </summary>
    public bool IsLast
    {
        get => GetValue(IsLastProperty);
        set => SetValue(IsLastProperty, value);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        UpdatePseudoClasses();
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(":pending", Status == StepStatus.Pending);
        PseudoClasses.Set(":active", Status == StepStatus.Active);
        PseudoClasses.Set(":completed", Status == StepStatus.Completed);
        PseudoClasses.Set(":error", Status == StepStatus.Error);
        PseudoClasses.Set(":last", IsLast);
    }
}
