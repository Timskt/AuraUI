using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;

namespace AuraUI.Controls.Feedback;

/// <summary>
/// Represents a single step in a guided tour.
/// </summary>
public class TourStep : AvaloniaObject
{
    /// <summary>
    /// Defines the <see cref="Target"/> styled property.
    /// The control to highlight for this step.
    /// </summary>
    public static readonly StyledProperty<Control?> TargetProperty =
        AvaloniaProperty.Register<TourStep, Control?>(nameof(Target));

    /// <summary>
    /// Defines the <see cref="Title"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> TitleProperty =
        AvaloniaProperty.Register<TourStep, string?>(nameof(Title));

    /// <summary>
    /// Defines the <see cref="Description"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> DescriptionProperty =
        AvaloniaProperty.Register<TourStep, string?>(nameof(Description));

    /// <summary>
    /// Defines the <see cref="Placement"/> styled property.
    /// </summary>
    public static readonly StyledProperty<PlacementMode> PlacementProperty =
        AvaloniaProperty.Register<TourStep, PlacementMode>(
            nameof(Placement), PlacementMode.Bottom);

    /// <summary>
    /// Defines the <see cref="Content"/> styled property.
    /// Custom content for the step (overrides Title/Description).
    /// </summary>
    public static readonly StyledProperty<object?> ContentProperty =
        AvaloniaProperty.Register<TourStep, object?>(nameof(Content));

    /// <summary>
    /// Defines the <see cref="ShowArrow"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> ShowArrowProperty =
        AvaloniaProperty.Register<TourStep, bool>(nameof(ShowArrow), true);

    /// <summary>
    /// Gets or sets the target control to highlight.
    /// </summary>
    public Control? Target
    {
        get => GetValue(TargetProperty);
        set => SetValue(TargetProperty, value);
    }

    /// <summary>
    /// Gets or sets the step title.
    /// </summary>
    public string? Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    /// <summary>
    /// Gets or sets the step description.
    /// </summary>
    public string? Description
    {
        get => GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }

    /// <summary>
    /// Gets or sets the tooltip placement relative to the target.
    /// </summary>
    public PlacementMode Placement
    {
        get => GetValue(PlacementProperty);
        set => SetValue(PlacementProperty, value);
    }

    /// <summary>
    /// Gets or sets custom content for the step.
    /// </summary>
    public object? Content
    {
        get => GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }

    /// <summary>
    /// Gets or sets whether to show an arrow pointing to the target.
    /// </summary>
    public bool ShowArrow
    {
        get => GetValue(ShowArrowProperty);
        set => SetValue(ShowArrowProperty, value);
    }
}

/// <summary>
/// A guided tour control that highlights target controls one at a time
/// with overlay, descriptions, and navigation. Inspired by Ant Design's Tour component.
/// </summary>
[TemplatePart("PART_Overlay", typeof(Border))]
[TemplatePart("PART_Tooltip", typeof(Border))]
[TemplatePart("PART_NextButton", typeof(Button))]
[TemplatePart("PART_PreviousButton", typeof(Button))]
[TemplatePart("PART_CloseButton", typeof(Button))]
[PseudoClasses(":open", ":closed", ":first-step", ":last-step")]
public class Tour : TemplatedControl
{
    private Button? _nextButton;
    private Button? _previousButton;
    private Button? _closeButton;

    /// <summary>
    /// Defines the <see cref="Steps"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IList<TourStep>?> StepsProperty =
        AvaloniaProperty.Register<Tour, IList<TourStep>?>(nameof(Steps));

    /// <summary>
    /// Defines the <see cref="CurrentStepIndex"/> styled property.
    /// </summary>
    public static readonly StyledProperty<int> CurrentStepIndexProperty =
        AvaloniaProperty.Register<Tour, int>(nameof(CurrentStepIndex));

    /// <summary>
    /// Defines the <see cref="IsOpen"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsOpenProperty =
        AvaloniaProperty.Register<Tour, bool>(nameof(IsOpen));

    /// <summary>
    /// Defines the <see cref="ShowClose"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> ShowCloseProperty =
        AvaloniaProperty.Register<Tour, bool>(nameof(ShowClose), true);

    /// <summary>
    /// Defines the <see cref="ShowArrow"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> ShowArrowProperty =
        AvaloniaProperty.Register<Tour, bool>(nameof(ShowArrow), true);

    /// <summary>
    /// Defines the <see cref="OverlayBrush"/> styled property.
    /// </summary>
    public static readonly StyledProperty<Avalonia.Media.IBrush?> OverlayBrushProperty =
        AvaloniaProperty.Register<Tour, Avalonia.Media.IBrush?>(nameof(OverlayBrush));

    /// <summary>
    /// Defines the <see cref="MaskClosable"/> styled property.
    /// Whether clicking the overlay closes the tour.
    /// </summary>
    public static readonly StyledProperty<bool> MaskClosableProperty =
        AvaloniaProperty.Register<Tour, bool>(nameof(MaskClosable), true);

    static Tour()
    {
        IsOpenProperty.Changed.AddClassHandler<Tour>((x, _) => x.UpdatePseudoClasses());
        CurrentStepIndexProperty.Changed.AddClassHandler<Tour>((x, _) => x.UpdatePseudoClasses());
    }

    /// <summary>
    /// Gets or sets the tour steps.
    /// </summary>
    public IList<TourStep>? Steps
    {
        get => GetValue(StepsProperty);
        set => SetValue(StepsProperty, value);
    }

    /// <summary>
    /// Gets or sets the current step index.
    /// </summary>
    public int CurrentStepIndex
    {
        get => GetValue(CurrentStepIndexProperty);
        set => SetValue(CurrentStepIndexProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the tour is open.
    /// </summary>
    public bool IsOpen
    {
        get => GetValue(IsOpenProperty);
        set => SetValue(IsOpenProperty, value);
    }

    /// <summary>
    /// Gets or sets whether to show the close button.
    /// </summary>
    public bool ShowClose
    {
        get => GetValue(ShowCloseProperty);
        set => SetValue(ShowCloseProperty, value);
    }

    /// <summary>
    /// Gets or sets whether to show the arrow on tooltips.
    /// </summary>
    public bool ShowArrow
    {
        get => GetValue(ShowArrowProperty);
        set => SetValue(ShowArrowProperty, value);
    }

    /// <summary>
    /// Gets or sets the overlay background brush.
    /// </summary>
    public Avalonia.Media.IBrush? OverlayBrush
    {
        get => GetValue(OverlayBrushProperty);
        set => SetValue(OverlayBrushProperty, value);
    }

    /// <summary>
    /// Gets or sets whether clicking the overlay closes the tour.
    /// </summary>
    public bool MaskClosable
    {
        get => GetValue(MaskClosableProperty);
        set => SetValue(MaskClosableProperty, value);
    }

    /// <summary>
    /// Gets the current step.
    /// </summary>
    public TourStep? CurrentStep
    {
        get
        {
            var steps = Steps;
            if (steps == null || CurrentStepIndex < 0 || CurrentStepIndex >= steps.Count)
                return null;
            return steps[CurrentStepIndex];
        }
    }

    /// <summary>
    /// Occurs when the tour is closed.
    /// </summary>
    public event EventHandler<RoutedEventArgs>? Closed;

    /// <summary>
    /// Occurs when the current step changes.
    /// </summary>
    public event EventHandler<RoutedEventArgs>? StepChanged;

    /// <summary>
    /// Occurs when the tour finishes (last step completed).
    /// </summary>
    public event EventHandler<RoutedEventArgs>? Finished;

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        if (_nextButton != null) _nextButton.Click -= OnNextClick;
        if (_previousButton != null) _previousButton.Click -= OnPreviousClick;
        if (_closeButton != null) _closeButton.Click -= OnCloseClick;

        _nextButton = e.NameScope.Find<Button>("PART_NextButton");
        _previousButton = e.NameScope.Find<Button>("PART_PreviousButton");
        _closeButton = e.NameScope.Find<Button>("PART_CloseButton");

        if (_nextButton != null) _nextButton.Click += OnNextClick;
        if (_previousButton != null) _previousButton.Click += OnPreviousClick;
        if (_closeButton != null) _closeButton.Click += OnCloseClick;

        UpdatePseudoClasses();
    }

    /// <summary>
    /// Starts the tour from the beginning.
    /// </summary>
    public void Start()
    {
        CurrentStepIndex = 0;
        IsOpen = true;
    }

    /// <summary>
    /// Advances to the next step.
    /// </summary>
    public void Next()
    {
        var steps = Steps;
        if (steps == null) return;

        if (CurrentStepIndex < steps.Count - 1)
        {
            CurrentStepIndex++;
            StepChanged?.Invoke(this, new RoutedEventArgs());
        }
        else
        {
            Finish();
        }
    }

    /// <summary>
    /// Goes back to the previous step.
    /// </summary>
    public void Previous()
    {
        if (CurrentStepIndex > 0)
        {
            CurrentStepIndex--;
            StepChanged?.Invoke(this, new RoutedEventArgs());
        }
    }

    /// <summary>
    /// Closes the tour.
    /// </summary>
    public void Close()
    {
        IsOpen = false;
        Closed?.Invoke(this, new RoutedEventArgs());
    }

    /// <summary>
    /// Finishes the tour (completes the last step).
    /// </summary>
    public void Finish()
    {
        IsOpen = false;
        Finished?.Invoke(this, new RoutedEventArgs());
    }

    private void OnNextClick(object? sender, RoutedEventArgs e) => Next();
    private void OnPreviousClick(object? sender, RoutedEventArgs e) => Previous();
    private void OnCloseClick(object? sender, RoutedEventArgs e) => Close();

    private void UpdatePseudoClasses()
    {
        var steps = Steps;
        var count = steps?.Count ?? 0;

        PseudoClasses.Set(":open", IsOpen);
        PseudoClasses.Set(":closed", !IsOpen);
        PseudoClasses.Set(":first-step", CurrentStepIndex == 0);
        PseudoClasses.Set(":last-step", CurrentStepIndex >= count - 1);
    }
}
