using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using AuraUI.Controls.Navigation;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class PaginationPage : ComponentPageBase
{
    public override string ComponentName => "Pagination";
    public override string Description => "A pagination control for navigating through multi-page content with page numbers and navigation buttons.";
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
                    "Use pagination for lists, tables, and grids with many items. It helps users navigate through large datasets without loading everything at once.",
                    "Show total page count. Include previous/next buttons. Highlight the current page. Consider showing 5-7 page numbers at a time.")
            }
        };
    }

    private Control BuildBasicExample()
    {
        var pagination = new Pagination { TotalItems = 200, PageSize = 10, CurrentPage = 1 };
        return CreateExampleSection("Basic Pagination", pagination,
            @"<nav:Pagination TotalItems=""200"" PageSize=""10"" CurrentPage=""1""/>");
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "TotalItems", Type = "int", Default = "0", Description = "Total number of items" },
        new ApiProperty { PropertyName = "PageSize", Type = "int", Default = "10", Description = "Items per page" },
        new ApiProperty { PropertyName = "CurrentPage", Type = "int", Default = "1", Description = "Current page number" },
    };
}
