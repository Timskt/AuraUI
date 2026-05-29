using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using AuraUI.Controls.Layout;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class TagPage : ComponentPageBase
{
    public override string ComponentName => "Tag";
    public override string Description => "A tag component for categorizing or marking items with optional icon and close button.";
    public override string Category => "Layout";

    protected override Control BuildContent()
    {
        return new StackPanel
        {
            Spacing = 32,
            Children =
            {
                BuildVariantsExample(),
                BuildClosableExample(),
                CreateApiTable(GetApiProperties()),
                CreateGuidelines(
                    "Use tags for categorization, status labels, and filters. They work well in lists, cards, and filter panels.",
                    "Keep tag text short (1-2 words). Use closable tags for removable filters. Group related tags with consistent spacing.")
            }
        };
    }

    private Control BuildVariantsExample()
    {
        var panel = new WrapPanel();
        foreach (var (text, variant) in new[] {
            ("Default", TagVariant.Default), ("Primary", TagVariant.Primary),
            ("Success", TagVariant.Success), ("Warning", TagVariant.Warning),
            ("Error", TagVariant.Error), ("Outlined", TagVariant.Outlined)
        })
        {
            panel.Children.Add(new Tag { Content = text, Variant = variant, Margin = new Thickness(0, 0, 6, 6) });
        }
        return CreateExampleSection("Variants", panel,
            @"<layout:Tag Content=""Default""/>
<layout:Tag Content=""Primary"" Variant=""Primary""/>
<layout:Tag Content=""Success"" Variant=""Success""/>
<layout:Tag Content=""Warning"" Variant=""Warning""/>
<layout:Tag Content=""Error"" Variant=""Error""/>
<layout:Tag Content=""Outlined"" Variant=""Outlined""/>");
    }

    private Control BuildClosableExample()
    {
        var panel = new WrapPanel();
        foreach (var (text, variant) in new[] {
            ("Removable", TagVariant.Primary), ("Filter", TagVariant.Success), ("Active", TagVariant.Warning)
        })
        {
            var tag = new Tag { Content = text, Variant = variant, IsClosable = true, Margin = new Thickness(0, 0, 6, 6) };
            tag.Closed += (_, e) => { if (tag.Parent is Panel p) p.Children.Remove(tag); };
            panel.Children.Add(tag);
        }
        return CreateExampleSection("Closable Tags", panel,
            @"<layout:Tag Content=""Removable"" Variant=""Primary"" IsClosable=""True""/>
<layout:Tag Content=""Filter"" Variant=""Success"" IsClosable=""True""/>
<layout:Tag Content=""Active"" Variant=""Warning"" IsClosable=""True""/>",
            @"tag.Closed += (sender, e) =>
{
    // Remove tag from parent when closed
    if (tag.Parent is Panel p)
        p.Children.Remove(tag);
};");
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "Content", Type = "object", Default = "null", Description = "Tag text content" },
        new ApiProperty { PropertyName = "Variant", Type = "TagVariant", Default = "Default", Description = "Visual variant: Default, Primary, Success, Warning, Error, Outlined" },
        new ApiProperty { PropertyName = "IsClosable", Type = "bool", Default = "false", Description = "Shows a close button" },
        new ApiProperty { PropertyName = "Icon", Type = "object", Default = "null", Description = "Icon displayed before the tag text" },
        new ApiProperty { PropertyName = "CloseButtonContent", Type = "object", Default = "null", Description = "Custom close button content (defaults to x)" },
    };
}
