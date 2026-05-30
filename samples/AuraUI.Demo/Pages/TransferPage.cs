using System.Collections.Generic;
using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;
using AuraUI.Controls.Selection;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class TransferPage : ComponentPageBase
{
    public override string ComponentName => "Transfer";
    public override string Description => "A dual-list transfer component allowing users to move items between source and target lists. Supports search filtering and select all.";
    public override string Category => "Selection";

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
                    "Use Transfer for assigning items between two groups, such as role assignments, permission grants, or data migration.",
                    "Provide search when lists are long. Use descriptive Left/Right titles. Support Select All for batch operations. Show item counts.")
            }
        };
    }

    private Control BuildBasicExample()
    {
        var transfer = new Transfer
        {
            Width = 550,
            Height = 300,
            LeftTitle = "Available",
            RightTitle = "Selected",
            ShowSearch = true,
            SourceItems = new ObservableCollection<TransferItem>
            {
                new TransferItem { Key = "1", Title = "Read Access" },
                new TransferItem { Key = "2", Title = "Write Access" },
                new TransferItem { Key = "3", Title = "Delete Access" },
                new TransferItem { Key = "4", Title = "Admin Access" },
                new TransferItem { Key = "5", Title = "Execute Access", IsDisabled = true },
            },
            TargetItems = new ObservableCollection<TransferItem>
            {
                new TransferItem { Key = "6", Title = "View Reports" },
            }
        };

        return CreateExampleSection("Transfer List", transfer,
            @"<selection:Transfer Width=""550"" Height=""300""
    LeftTitle=""Available"" RightTitle=""Selected""
    ShowSearch=""True"">
    <!-- SourceItems and TargetItems with TransferItem objects -->
</selection:Transfer>");
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "SourceItems", Type = "ObservableCollection<TransferItem>", Default = "null", Description = "Source (left) items" },
        new ApiProperty { PropertyName = "TargetItems", Type = "ObservableCollection<TransferItem>", Default = "null", Description = "Target (right) items" },
        new ApiProperty { PropertyName = "LeftTitle", Type = "string", Default = "Source", Description = "Source list title" },
        new ApiProperty { PropertyName = "RightTitle", Type = "string", Default = "Target", Description = "Target list title" },
        new ApiProperty { PropertyName = "ShowSearch", Type = "bool", Default = "false", Description = "Show search boxes" },
        new ApiProperty { PropertyName = "Operations", Type = "IList<string>", Default = "null", Description = "Move button labels" },
    };
}
