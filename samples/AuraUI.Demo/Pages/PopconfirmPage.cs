using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using AuraUI.Controls.Feedback;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class PopconfirmPage : ComponentPageBase
{
    public override string ComponentName => "Popconfirm";
    public override string Description => "A confirmation popover with OK/Cancel buttons that appears on click. Used for destructive or important actions that need confirmation.";
    public override string Category => "Feedback";

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
                    "Use Popconfirm for delete actions, form submissions, navigation away from unsaved changes, and any action that needs lightweight confirmation.",
                    "Use clear, action-oriented titles. Keep descriptions brief. Place near the trigger element. Support keyboard dismissal with Escape.")
            }
        };
    }

    private Control BuildBasicExample()
    {
        var deleteBtn = new Avalonia.Controls.Button
        {
            Content = "Delete Item",
            Classes = { "danger" }
        };

        var popconfirm = new Popconfirm
        {
            Title = "Are you sure?",
            Description = "This action cannot be undone. The item will be permanently deleted.",
            OkText = "Delete",
            CancelText = "Cancel",
            Content = deleteBtn,
            Width = 200
        };

        return CreateExampleSection("Delete Confirmation", popconfirm,
            @"<feedback:Popconfirm Title=""Are you sure?""
    Description=""This action cannot be undone.""
    OkText=""Delete"" CancelText=""Cancel"">
    <Button Content=""Delete Item"" Classes=""danger""/>
</feedback:Popconfirm>");
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "Title", Type = "string", Default = "null", Description = "Confirmation title" },
        new ApiProperty { PropertyName = "Description", Type = "string", Default = "null", Description = "Detailed description" },
        new ApiProperty { PropertyName = "Icon", Type = "object", Default = "null", Description = "Warning icon" },
        new ApiProperty { PropertyName = "OkText", Type = "string", Default = "OK", Description = "OK button text" },
        new ApiProperty { PropertyName = "CancelText", Type = "string", Default = "Cancel", Description = "Cancel button text" },
        new ApiProperty { PropertyName = "OkButtonTheme", Type = "string", Default = "primary", Description = "OK button style" },
    };
}
