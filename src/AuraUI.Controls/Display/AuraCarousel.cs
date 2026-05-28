using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Controls.Templates;
using Avalonia.Threading;

namespace AuraUI.Controls.Display;

/// <summary>
/// A carousel control that cycles through items with navigation arrows, dot indicators,
/// auto-play, and transition classes (.fade, .slide).
/// </summary>
[TemplatePart("PART_PreviousButton", typeof(Button))]
[TemplatePart("PART_NextButton", typeof(Button))]
[TemplatePart("PART_Indicators", typeof(ItemsControl))]
[TemplatePart("PART_ItemsPresenter", typeof(ItemsPresenter))]
[PseudoClasses(":has-previous", ":has-next")]
public class AuraCarousel : TemplatedControl
{
    private Button? _previousButton;
    private Button? _nextButton;
    private ItemsControl? _indicators;
    private ItemsPresenter? _itemsPresenter;
    private DispatcherTimer? _autoPlayTimer;
    private bool _isHovering;

    /// <summary>
    /// Defines the <see cref="SelectedIndex"/> styled property.
    /// </summary>
    public static readonly StyledProperty<int> SelectedIndexProperty =
        AvaloniaProperty.Register<AuraCarousel, int>(nameof(SelectedIndex), coerce: (o, v) => CoerceSelectedIndex((AuraCarousel)o, v));

    /// <summary>
    /// Defines the <see cref="AutoPlay"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> AutoPlayProperty =
        AvaloniaProperty.Register<AuraCarousel, bool>(nameof(AutoPlay));

    /// <summary>
    /// Defines the <see cref="Interval"/> styled property.
    /// </summary>
    public static readonly StyledProperty<TimeSpan> IntervalProperty =
        AvaloniaProperty.Register<AuraCarousel, TimeSpan>(nameof(Interval), TimeSpan.FromSeconds(5));

    /// <summary>
    /// Defines the <see cref="ShowIndicators"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> ShowIndicatorsProperty =
        AvaloniaProperty.Register<AuraCarousel, bool>(nameof(ShowIndicators), true);

    /// <summary>
    /// Defines the <see cref="ShowNavigation"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> ShowNavigationProperty =
        AvaloniaProperty.Register<AuraCarousel, bool>(nameof(ShowNavigation), true);

    /// <summary>
    /// Defines the <see cref="Items"/> styled property.
    /// </summary>
    public static readonly StyledProperty<Avalonia.Controls.Controls?> ItemsProperty =
        AvaloniaProperty.Register<AuraCarousel, Avalonia.Controls.Controls?>(nameof(Items));

    /// <summary>
    /// Defines the <see cref="ItemTemplate"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IDataTemplate?> ItemTemplateProperty =
        AvaloniaProperty.Register<AuraCarousel, IDataTemplate?>(nameof(ItemTemplate));

    /// <summary>
    /// Defines the <see cref="ItemCount"/> styled property.
    /// </summary>
    public static readonly StyledProperty<int> ItemCountProperty =
        AvaloniaProperty.Register<AuraCarousel, int>(nameof(ItemCount));

    static AuraCarousel()
    {
        SelectedIndexProperty.Changed.AddClassHandler<AuraCarousel>((x, e) => x.OnSelectedIndexChanged(e));
        AutoPlayProperty.Changed.AddClassHandler<AuraCarousel>((x, _) => x.ConfigureAutoPlay());
        IntervalProperty.Changed.AddClassHandler<AuraCarousel>((x, _) => x.ConfigureAutoPlay());
        ItemsProperty.Changed.AddClassHandler<AuraCarousel>((x, _) => x.OnItemsChanged());
    }

    /// <summary>
    /// Gets or sets the index of the currently selected item.
    /// </summary>
    public int SelectedIndex
    {
        get => GetValue(SelectedIndexProperty);
        set => SetValue(SelectedIndexProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the carousel auto-plays.
    /// </summary>
    public bool AutoPlay
    {
        get => GetValue(AutoPlayProperty);
        set => SetValue(AutoPlayProperty, value);
    }

    /// <summary>
    /// Gets or sets the interval between auto-play transitions.
    /// </summary>
    public TimeSpan Interval
    {
        get => GetValue(IntervalProperty);
        set => SetValue(IntervalProperty, value);
    }

    /// <summary>
    /// Gets or sets whether indicator dots are shown.
    /// </summary>
    public bool ShowIndicators
    {
        get => GetValue(ShowIndicatorsProperty);
        set => SetValue(ShowIndicatorsProperty, value);
    }

    /// <summary>
    /// Gets or sets whether navigation arrows are shown.
    /// </summary>
    public bool ShowNavigation
    {
        get => GetValue(ShowNavigationProperty);
        set => SetValue(ShowNavigationProperty, value);
    }

    /// <summary>
    /// Gets or sets the items collection.
    /// </summary>
    public Avalonia.Controls.Controls? Items
    {
        get => GetValue(ItemsProperty);
        set => SetValue(ItemsProperty, value);
    }

    /// <summary>
    /// Gets or sets the item template.
    /// </summary>
    public IDataTemplate? ItemTemplate
    {
        get => GetValue(ItemTemplateProperty);
        set => SetValue(ItemTemplateProperty, value);
    }

    /// <summary>
    /// Gets the number of items.
    /// </summary>
    public int ItemCount
    {
        get => GetValue(ItemCountProperty);
        set => SetValue(ItemCountProperty, value);
    }

    /// <summary>
    /// Advances to the next item.
    /// </summary>
    public void Next()
    {
        var count = GetItemCount();
        if (count <= 0) return;
        SelectedIndex = (SelectedIndex + 1) % count;
    }

    /// <summary>
    /// Goes to the previous item.
    /// </summary>
    public void Previous()
    {
        var count = GetItemCount();
        if (count <= 0) return;
        SelectedIndex = (SelectedIndex - 1 + count) % count;
    }

    /// <summary>
    /// Navigates to a specific index.
    /// </summary>
    public void GoTo(int index)
    {
        var count = GetItemCount();
        if (count <= 0) return;
        SelectedIndex = Math.Clamp(index, 0, count - 1);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        if (_previousButton != null)
            _previousButton.Click -= OnPreviousClick;
        if (_nextButton != null)
            _nextButton.Click -= OnNextClick;

        _previousButton = e.NameScope.Find<Button>("PART_PreviousButton");
        _nextButton = e.NameScope.Find<Button>("PART_NextButton");
        _indicators = e.NameScope.Find<ItemsControl>("PART_Indicators");
        _itemsPresenter = e.NameScope.Find<ItemsPresenter>("PART_ItemsPresenter");

        if (_previousButton != null)
            _previousButton.Click += OnPreviousClick;
        if (_nextButton != null)
            _nextButton.Click += OnNextClick;

        UpdateNavigationPseudoClasses();
        UpdateIndicators();
        ConfigureAutoPlay();
    }

    protected override void OnPointerEntered(PointerEventArgs e)
    {
        base.OnPointerEntered(e);
        _isHovering = true;
        if (AutoPlay)
            StopAutoPlay();
    }

    protected override void OnPointerExited(PointerEventArgs e)
    {
        base.OnPointerExited(e);
        _isHovering = false;
        if (AutoPlay)
            StartAutoPlay();
    }

    private void OnPreviousClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e) => Previous();
    private void OnNextClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e) => Next();

    private static int CoerceSelectedIndex(AuraCarousel sender, int value)
    {
        var count = sender.GetItemCount();
        if (count <= 0) return 0;
        return Math.Clamp(value, 0, count - 1);
    }

    private void OnSelectedIndexChanged(AvaloniaPropertyChangedEventArgs e)
    {
        UpdateNavigationPseudoClasses();
        UpdateIndicators();
        ResetAutoPlayTimer();
    }

    private void OnItemsChanged()
    {
        var items = Items;
        SetCurrentValue(ItemCountProperty, items?.Count ?? 0);
        UpdateNavigationPseudoClasses();
        UpdateIndicators();
    }

    private int GetItemCount()
    {
        var items = Items;
        return items?.Count ?? 0;
    }

    private void UpdateNavigationPseudoClasses()
    {
        var index = SelectedIndex;
        var count = GetItemCount();
        PseudoClasses.Set(":has-previous", count > 1);
        PseudoClasses.Set(":has-next", count > 1);
    }

    private void UpdateIndicators()
    {
        if (_indicators == null) return;
        var count = GetItemCount();
        var indices = new List<int>(count);
        for (int i = 0; i < count; i++)
            indices.Add(i);
        _indicators.ItemsSource = indices;
    }

    private void ConfigureAutoPlay()
    {
        StopAutoPlay();
        if (AutoPlay && !_isHovering)
            StartAutoPlay();
    }

    private void StartAutoPlay()
    {
        StopAutoPlay();
        _autoPlayTimer = new DispatcherTimer { Interval = Interval };
        _autoPlayTimer.Tick += OnAutoPlayTick;
        _autoPlayTimer.Start();
    }

    private void StopAutoPlay()
    {
        if (_autoPlayTimer != null)
        {
            _autoPlayTimer.Tick -= OnAutoPlayTick;
            _autoPlayTimer.Stop();
            _autoPlayTimer = null;
        }
    }

    private void ResetAutoPlayTimer()
    {
        if (AutoPlay && !_isHovering)
        {
            StopAutoPlay();
            StartAutoPlay();
        }
    }

    private void OnAutoPlayTick(object? sender, EventArgs e) => Next();

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        StopAutoPlay();
    }
}
