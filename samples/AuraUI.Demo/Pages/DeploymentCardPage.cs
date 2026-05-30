using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using AuraUI.Controls.Display;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class DeploymentCardPage : ComponentPageBase
{
    public override string ComponentName => "DeploymentCard";
    public override string Description => "A card displaying deployment information including environment, status, version, deployer, duration, and commit hash.";
    public override string Category => "Display";

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
                    "Use DeploymentCard in CI/CD dashboards, deployment history views, and DevOps monitoring panels.",
                    "Show environment badges prominently. Include commit hash for traceability. Use status colors consistently. Show timestamp for recency.")
            }
        };
    }

    private Control BuildBasicExample()
    {
        var card1 = new DeploymentCard
        {
            Title = "Frontend App",
            Environment = DeploymentEnvironment.Prod,
            DeployStatus = DeploymentStatus.Success,
            Version = "v2.4.1",
            Deployer = "CI Pipeline",
            Duration = TimeSpan.FromMinutes(2).Add(TimeSpan.FromSeconds(34)),
            CommitHash = "a1b2c3d4e5f6g7h8",
            Timestamp = DateTime.Now.AddHours(-1),
            Width = 400,
            Margin = new Thickness(0, 0, 0, 12)
        };

        var card2 = new DeploymentCard
        {
            Title = "API Service",
            Environment = DeploymentEnvironment.Staging,
            DeployStatus = DeploymentStatus.Building,
            Version = "v2.5.0-rc1",
            Deployer = "manual-deploy",
            CommitHash = "f8e7d6c5b4a3",
            Width = 400
        };

        var panel = new StackPanel { Spacing = 12 };
        panel.Children.Add(card1);
        panel.Children.Add(card2);

        return CreateExampleSection("Deployment Cards", panel,
            @"<display:DeploymentCard Title=""Frontend App""
    Environment=""Prod"" DeployStatus=""Success""
    Version=""v2.4.1"" Deployer=""CI Pipeline""
    CommitHash=""a1b2c3d4e5f6""/>

<display:DeploymentCard Title=""API Service""
    Environment=""Staging"" DeployStatus=""Building""
    Version=""v2.5.0-rc1""/>");
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "Title", Type = "string", Default = "null", Description = "Card title" },
        new ApiProperty { PropertyName = "Environment", Type = "DeploymentEnvironment", Default = "Dev", Description = "Dev, Staging, or Prod" },
        new ApiProperty { PropertyName = "DeployStatus", Type = "DeploymentStatus", Default = "Pending", Description = "Pending, Building, Success, Failed" },
        new ApiProperty { PropertyName = "Version", Type = "string", Default = "null", Description = "Version string" },
        new ApiProperty { PropertyName = "Deployer", Type = "string", Default = "null", Description = "Who triggered the deploy" },
        new ApiProperty { PropertyName = "Duration", Type = "TimeSpan", Default = "0", Description = "Deployment duration" },
        new ApiProperty { PropertyName = "CommitHash", Type = "string", Default = "null", Description = "Git commit hash" },
        new ApiProperty { PropertyName = "Timestamp", Type = "DateTime?", Default = "null", Description = "Deployment timestamp" },
    };
}
