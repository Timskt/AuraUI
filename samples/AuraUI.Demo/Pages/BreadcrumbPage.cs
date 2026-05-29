using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using AuraUI.Controls.Navigation;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class BreadcrumbPage : ComponentPageBase
{
    public override string ComponentName => "Breadcrumb";
    public override string Description => "A breadcrumb navigation trail showing the current page location in a hierarchy.";
    public override string Category => "Navigation";

    protected override Control BuildContent()
    {
        return new StackPanel
        {
            Spacing = 32,
            Children =
            {
                BuildBasicExample(),
                CreateApiTable(GetApiProperties()),
                CreateGuidelines(
                    "Use breadcrumbs for hierarchical navigation in applications with deep page structures. They help users understand their location.",
                    "Always show the full path from root to current page. Make each level clickable. Use separators consistently (usually '/').")
            }
        };
    }

    private Control BuildBasicExample()
    {
        var breadcrumb = new Breadcrumb { MaxWidth = 500 };
        breadcrumb.Items.Add(new BreadcrumbItem { Content = "Home", Tag = "home" });
        breadcrumb.Items.Add(new BreadcrumbItem { Content = "Products", Tag = "products" });
        breadcrumb.Items.Add(new BreadcrumbItem { Content = "Electronics", Tag = "electronics" });
        breadcrumb.Items.Add(new BreadcrumbItem { Content = "Laptops", Tag = "laptops" });

        return CreateExampleSection("Basic Breadcrumb", breadcrumb,
            @"<nav:Breadcrumb MaxWidth=""500"">
    <nav:BreadcrumbItem Content=""Home""/>
    <nav:BreadcrumbItem Content=""Products""/>
    <nav:BreadcrumbItem Content=""Electronics""/>
    <nav:BreadcrumbItem Content=""Laptops""/>
</nav:Breadcrumb>");
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "Items", Type = "IList<BreadcrumbItem>", Default = "[]", Description = "Breadcrumb items" },
        new ApiProperty { PropertyName = "Separator", Type = "string", Default = "/", Description = "Separator character" },
    };
}
