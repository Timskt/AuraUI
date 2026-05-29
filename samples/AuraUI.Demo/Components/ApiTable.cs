using System.Collections;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Media;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Components;

/// <summary>
/// Renders an API property documentation table with Property/Type/Default/Description columns.
/// </summary>
public class ApiTable : Control
{
    public static readonly StyledProperty<IList<ApiProperty>?> PropertiesProperty =
        AvaloniaProperty.Register<ApiTable, IList<ApiProperty>?>(nameof(Properties));

    public IList<ApiProperty>? Properties
    {
        get => GetValue(PropertiesProperty);
        set => SetValue(PropertiesProperty, value);
    }

    public override void Render(DrawingContext context) { }

    protected override Size MeasureOverride(Size availableSize)
    {
        var props = Properties;
        if (props == null || props.Count == 0)
            return new Size(0, 0);

        // Build visual tree manually
        return base.MeasureOverride(availableSize);
    }
}
