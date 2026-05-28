using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Media;

namespace AuraUI.Controls.Selection;

/// <summary>
/// A star/rating control that displays a configurable number of icon items (stars, hearts, etc.)
/// and supports half-star ratings, hover preview, and click-to-rate.
/// </summary>
/// <remarks>
/// <para>
/// Renders <see cref="Max"/> items. Items up to <see cref="Value"/> are highlighted with
/// <see cref="SelectedColor"/>, while the rest use <see cref="UnselectedColor"/>.
/// When <see cref="AllowHalf"/> is <c>true</c>, users can rate in half-step increments by
/// clicking on the left or right half of each item.
/// </para>
/// </remarks>
[TemplatePart("PART_RatingPanel", typeof(Panel))]
[PseudoClasses(":star", ":heart", ":custom", ":readonly", ":hovering")]
public class RateControl : TemplatedControl
{
    private Panel? _ratingPanel;
    private readonly List<RatingItem> _ratingItems = new();
    private double _previewValue;
    private bool _isHovering;

    /// <summary>
    /// Defines the <see cref="Value"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> ValueProperty =
        AvaloniaProperty.Register<RateControl, double>(nameof(Value));

    /// <summary>
    /// Defines the <see cref="Max"/> styled property.
    /// </summary>
    public static readonly StyledProperty<int> MaxProperty =
        AvaloniaProperty.Register<RateControl, int>(nameof(Max), 5);

    /// <summary>
    /// Defines the <see cref="AllowHalf"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> AllowHalfProperty =
        AvaloniaProperty.Register<RateControl, bool>(nameof(AllowHalf));

    /// <summary>
    /// Defines the <see cref="Icon"/> styled property.
    /// </summary>
    public static readonly StyledProperty<object?> IconProperty =
        AvaloniaProperty.Register<RateControl, object?>(nameof(Icon));

    /// <summary>
    /// Defines the <see cref="SelectedColor"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> SelectedColorProperty =
        AvaloniaProperty.Register<RateControl, IBrush?>(nameof(SelectedColor));

    /// <summary>
    /// Defines the <see cref="UnselectedColor"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> UnselectedColorProperty =
        AvaloniaProperty.Register<RateControl, IBrush?>(nameof(UnselectedColor));

    /// <summary>
    /// Defines the <see cref="ItemSize"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> ItemSizeProperty =
        AvaloniaProperty.Register<RateControl, double>(nameof(ItemSize), 24.0);

    /// <summary>
    /// Defines the <see cref="ItemSpacing"/> styled property.
    /// </summary>
    public static readonly StyledProperty<double> ItemSpacingProperty =
        AvaloniaProperty.Register<RateControl, double>(nameof(ItemSpacing), 4.0);

    /// <summary>
    /// Defines the <see cref="IsReadOnly"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsReadOnlyProperty =
        AvaloniaProperty.Register<RateControl, bool>(nameof(IsReadOnly));

    /// <summary>
    /// Defines the <see cref="RateVariant"/> styled property.
    /// </summary>
    public static readonly StyledProperty<RateVariant> VariantProperty =
        AvaloniaProperty.Register<RateControl, RateVariant>(
            nameof(Variant),
            RateVariant.Star);

    /// <summary>
    /// Defines the <see cref="ShowValue"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> ShowValueProperty =
        AvaloniaProperty.Register<RateControl, bool>(nameof(ShowValue));

    static RateControl()
    {
        ValueProperty.Changed.AddClassHandler<RateControl>((x, _) => x.OnValueChanged());
        MaxProperty.Changed.AddClassHandler<RateControl>((x, _) => x.RebuildItems());
        AllowHalfProperty.Changed.AddClassHandler<RateControl>((x, _) => x.RebuildItems());
        VariantProperty.Changed.AddClassHandler<RateControl>((x, _) => x.OnVariantChanged());
        SelectedColorProperty.Changed.AddClassHandler<RateControl>((x, _) => x.UpdateItemHighlights());
        UnselectedColorProperty.Changed.AddClassHandler<RateControl>((x, _) => x.UpdateItemHighlights());
        IsReadOnlyProperty.Changed.AddClassHandler<RateControl>((x, _) => x.UpdatePseudoClasses());
    }

    /// <summary>
    /// Occurs when the rating value changes.
    /// </summary>
    public event EventHandler<RateValueChangedEventArgs>? ValueChanged;

    /// <summary>
    /// Gets or sets the current rating value.
    /// </summary>
    public double Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    /// <summary>
    /// Gets or sets the maximum number of rating items. Default is 5.
    /// </summary>
    public int Max
    {
        get => GetValue(MaxProperty);
        set => SetValue(MaxProperty, value);
    }

    /// <summary>
    /// Gets or sets whether half-step ratings are allowed.
    /// </summary>
    public bool AllowHalf
    {
        get => GetValue(AllowHalfProperty);
        set => SetValue(AllowHalfProperty, value);
    }

    /// <summary>
    /// Gets or sets the icon used for rating items. Can be a path data, geometry, or glyph.
    /// </summary>
    public object? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    /// <summary>
    /// Gets or sets the brush used for selected/highlighted items.
    /// </summary>
    public IBrush? SelectedColor
    {
        get => GetValue(SelectedColorProperty);
        set => SetValue(SelectedColorProperty, value);
    }

    /// <summary>
    /// Gets or sets the brush used for unselected items.
    /// </summary>
    public IBrush? UnselectedColor
    {
        get => GetValue(UnselectedColorProperty);
        set => SetValue(UnselectedColorProperty, value);
    }

    /// <summary>
    /// Gets or sets the size of each rating item.
    /// </summary>
    public double ItemSize
    {
        get => GetValue(ItemSizeProperty);
        set => SetValue(ItemSizeProperty, value);
    }

    /// <summary>
    /// Gets or sets the spacing between rating items.
    /// </summary>
    public double ItemSpacing
    {
        get => GetValue(ItemSpacingProperty);
        set => SetValue(ItemSpacingProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the control is read-only (no user interaction).
    /// </summary>
    public bool IsReadOnly
    {
        get => GetValue(IsReadOnlyProperty);
        set => SetValue(IsReadOnlyProperty, value);
    }

    /// <summary>
    /// Gets or sets the visual variant (star, heart, or custom icon).
    /// </summary>
    public RateVariant Variant
    {
        get => GetValue(VariantProperty);
        set => SetValue(VariantProperty, value);
    }

    /// <summary>
    /// Gets or sets whether to display the numeric value alongside the rating items.
    /// </summary>
    public bool ShowValue
    {
        get => GetValue(ShowValueProperty);
        set => SetValue(ShowValueProperty, value);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        _ratingPanel = e.NameScope.Find<Panel>("PART_RatingPanel");

        RebuildItems();
        UpdatePseudoClasses();
    }

    protected override void OnPointerEntered(PointerEventArgs e)
    {
        base.OnPointerEntered(e);

        if (!IsReadOnly)
        {
            _isHovering = true;
            PseudoClasses.Set(":hovering", true);
        }
    }

    protected override void OnPointerExited(PointerEventArgs e)
    {
        base.OnPointerExited(e);

        _isHovering = false;
        _previewValue = 0;
        PseudoClasses.Set(":hovering", false);
        UpdateItemHighlights();
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        base.OnPointerMoved(e);

        if (IsReadOnly || !_isHovering)
            return;

        var position = e.GetPosition(this);
        var newValue = CalculateValueFromPosition(position);

        if (Math.Abs(newValue - _previewValue) > 0.01)
        {
            _previewValue = newValue;
            UpdateItemHighlights();
        }
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);

        if (IsReadOnly)
            return;

        var position = e.GetPosition(this);
        var newValue = CalculateValueFromPosition(position);

        if (Math.Abs(newValue - Value) > 0.01)
        {
            var oldValue = Value;
            SetCurrentValue(ValueProperty, newValue);
            ValueChanged?.Invoke(this, new RateValueChangedEventArgs(oldValue, newValue));
        }
    }

    private void RebuildItems()
    {
        if (_ratingPanel == null)
            return;

        _ratingPanel.Children.Clear();
        _ratingItems.Clear();

        var max = Math.Max(0, Max);

        for (var i = 0; i < max; i++)
        {
            var item = new RatingItem
            {
                Index = i,
                Width = ItemSize,
                Height = ItemSize,
                Margin = new Thickness(0, 0, i < max - 1 ? ItemSpacing : 0, 0)
            };

            _ratingItems.Add(item);
            _ratingPanel.Children.Add(item);
        }

        UpdateItemHighlights();
    }

    private void OnValueChanged()
    {
        UpdateItemHighlights();
    }

    private void OnVariantChanged()
    {
        UpdatePseudoClasses();
    }

    private void UpdateItemHighlights()
    {
        var displayValue = _isHovering && _previewValue > 0 ? _previewValue : Value;
        var selectedBrush = SelectedColor ?? new SolidColorBrush(Colors.Gold);
        var unselectedBrush = UnselectedColor ?? new SolidColorBrush(Colors.LightGray);

        foreach (var item in _ratingItems)
        {
            var itemIndex = item.Index;
            var fillLevel = CalculateFillLevel(itemIndex, displayValue);

            item.SelectedBrush = selectedBrush;
            item.UnselectedBrush = unselectedBrush;
            item.FillLevel = fillLevel;
            item.Icon = Icon;
            item.InvalidateVisual();
        }
    }

    /// <summary>
    /// Calculates how much of the item at the given index should be highlighted.
    /// Returns a value from 0.0 (unselected) to 1.0 (fully selected).
    /// </summary>
    private double CalculateFillLevel(int index, double displayValue)
    {
        var oneBased = index + 1;

        if (displayValue >= oneBased)
            return 1.0;

        if (displayValue > index)
            return displayValue - index;

        return 0.0;
    }

    /// <summary>
    /// Calculates the rating value from a pointer position within the control.
    /// </summary>
    private double CalculateValueFromPosition(Point position)
    {
        var spacing = ItemSpacing;
        var itemWidth = ItemSize;
        var totalItemWidth = itemWidth + spacing;

        if (totalItemWidth <= 0)
            return 0;

        // Determine which item the pointer is over.
        var rawIndex = position.X / totalItemWidth;
        var itemIndex = (int)Math.Floor(rawIndex);

        if (itemIndex < 0)
            return 0;

        if (itemIndex >= Max)
            return Max;

        if (AllowHalf)
        {
            // Determine if the pointer is in the left or right half of the item.
            var positionWithinItem = position.X - (itemIndex * totalItemWidth);
            var isLeftHalf = positionWithinItem < (itemWidth / 2.0);

            return isLeftHalf ? itemIndex + 0.5 : itemIndex + 1.0;
        }

        return itemIndex + 1.0;
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(":star", Variant == RateVariant.Star);
        PseudoClasses.Set(":heart", Variant == RateVariant.Heart);
        PseudoClasses.Set(":custom", Variant == RateVariant.Custom);
        PseudoClasses.Set(":readonly", IsReadOnly);
    }

    /// <summary>
    /// A single rating item (star/heart) that renders using partial fill.
    /// </summary>
    private class RatingItem : Control
    {
        /// <summary>
        /// Gets or sets the item index (0-based).
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Gets or sets the fill level (0.0 to 1.0).
        /// </summary>
        public double FillLevel { get; set; }

        /// <summary>
        /// Gets or sets the brush for the selected portion.
        /// </summary>
        public IBrush? SelectedBrush { get; set; }

        /// <summary>
        /// Gets or sets the brush for the unselected portion.
        /// </summary>
        public IBrush? UnselectedBrush { get; set; }

        /// <summary>
        /// Gets or sets the icon geometry (optional, for custom rendering).
        /// </summary>
        public object? Icon { get; set; }

        public override void Render(DrawingContext context)
        {
            var bounds = new Rect(Bounds.Size);
            if (bounds.Width <= 0 || bounds.Height <= 0)
                return;

            var fillLevel = FillLevel;
            var selectedBrush = SelectedBrush ?? new SolidColorBrush(Colors.Gold);
            var unselectedBrush = UnselectedBrush ?? new SolidColorBrush(Colors.LightGray);

            // Draw the unselected (background) star.
            DrawStar(context, bounds, unselectedBrush);

            // Clip and draw the selected (foreground) star.
            if (fillLevel > 0)
            {
                var clipWidth = bounds.Width * fillLevel;
                var clipRect = new Rect(0, 0, clipWidth, bounds.Height);

                using (context.PushClip(clipRect))
                {
                    DrawStar(context, bounds, selectedBrush);
                }
            }
        }

        private void DrawStar(DrawingContext context, Rect bounds, IBrush brush)
        {
            var centerX = bounds.Width / 2.0;
            var centerY = bounds.Height / 2.0;
            var outerRadius = Math.Min(centerX, centerY) * 0.95;
            var innerRadius = outerRadius * 0.4;
            const int points = 5;

            var geometry = new StreamGeometry();
            using (var sgCtx = geometry.Open())
            {
                for (var i = 0; i < points * 2; i++)
                {
                    var angle = (Math.PI / points) * i - Math.PI / 2.0;
                    var radius = i % 2 == 0 ? outerRadius : innerRadius;
                    var x = centerX + radius * Math.Cos(angle);
                    var y = centerY + radius * Math.Sin(angle);

                    if (i == 0)
                        sgCtx.BeginFigure(new Point(x, y), true);
                    else
                        sgCtx.LineTo(new Point(x, y));
                }

                sgCtx.EndFigure(true);
            }

            context.DrawGeometry(brush, null, geometry);
        }
    }
}

/// <summary>
/// Visual variant for <see cref="RateControl"/> items.
/// </summary>
public enum RateVariant
{
    /// <summary>
    /// Star-shaped rating items.
    /// </summary>
    Star,

    /// <summary>
    /// Heart-shaped rating items.
    /// </summary>
    Heart,

    /// <summary>
    /// Uses the custom <see cref="RateControl.Icon"/> for rating items.
    /// </summary>
    Custom
}

/// <summary>
/// Event arguments for the <see cref="RateControl.ValueChanged"/> event.
/// </summary>
public class RateValueChangedEventArgs : EventArgs
{
    /// <summary>
    /// Gets the previous rating value.
    /// </summary>
    public double OldValue { get; }

    /// <summary>
    /// Gets the new rating value.
    /// </summary>
    public double NewValue { get; }

    public RateValueChangedEventArgs(double oldValue, double newValue)
    {
        OldValue = oldValue;
        NewValue = newValue;
    }
}
