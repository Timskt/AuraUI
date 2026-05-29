using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using AuraUI.Controls.Display;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class ResultPage : ComponentPageBase
{
    public override string ComponentName => "Result";
    public override string Description => "An operation result page for displaying success, error, warning, or info feedback.";
    public override string Category => "Display";

    protected override Control BuildContent()
    {
        return new StackPanel
        {
            Spacing = 32,
            Children =
            {
                BuildSuccessExample(),
                BuildErrorExample(),
                CreateApiTable(GetApiProperties()),
                CreateGuidelines(
                    "Use result pages after completing operations like form submission, payment, or file upload. They provide clear feedback on the outcome.",
                    "Include a title, description, and action button. Use appropriate status icon. Provide navigation options to continue.")
            }
        };
    }

    private Control BuildSuccessExample()
    {
        return CreateExampleSection("Success Result",
            new Result
            {
                Status = ResultStatus.Success,
                Title = "Operation Successful",
                SubTitle = "Your order has been placed. You will receive a confirmation email shortly.",
                Width = 500, Height = 250
            },
            @"<display:Result Status=""Success""
             Title=""Operation Successful""
             SubTitle=""Your order has been placed.""
             Width=""500"" Height=""250""/>");
    }

    private Control BuildErrorExample()
    {
        return CreateExampleSection("Error Result",
            new Result
            {
                Status = ResultStatus.Error,
                Title = "Operation Failed",
                SubTitle = "Something went wrong. Please try again later.",
                Width = 500, Height = 250
            },
            @"<display:Result Status=""Error""
             Title=""Operation Failed""
             SubTitle=""Something went wrong.""
             Width=""500"" Height=""250""/>");
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "Status", Type = "ResultStatus", Default = "Info", Description = "Result status: Success, Error, Warning, Info" },
        new ApiProperty { PropertyName = "Title", Type = "string", Default = "null", Description = "Result title" },
        new ApiProperty { PropertyName = "SubTitle", Type = "string", Default = "null", Description = "Result description" },
    };
}
