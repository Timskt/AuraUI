using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using AuraUI.Demo.Components;

namespace AuraUI.Demo.Pages;

/// <summary>
/// Welcome / landing page shown when no component is selected.
/// </summary>
public class WelcomePage : ComponentPageBase
{
    public override string ComponentName => "Welcome to AuraUI";
    public override string Description => "A comprehensive, modern UI component library for Avalonia. Explore every control with live examples, code, and API documentation.";
    public override string Category => "Getting Started";

    protected override Control BuildContent()
    {
        return new StackPanel
        {
            Spacing = 24,
            Children =
            {
                BuildFeatureGrid(),
                BuildQuickStart(),
                BuildThemeSection()
            }
        };
    }

    private Control BuildFeatureGrid()
    {
        var grid = new Grid
        {
            ColumnDefinitions = new ColumnDefinitions("*,*,*"),
            RowDefinitions = new RowDefinitions("Auto,Auto"),
            MaxWidth = 800
        };

        var features = new (string title, string desc, string color)[]
        {
            ("100+ Controls", "Buttons, inputs, charts, navigation, feedback, and more.", "#0078D4"),
            ("Fluent & Material", "Switch between Microsoft Fluent and Material Design themes.", "#5C2D91"),
            ("Design Tokens", "70+ semantic tokens for colors, spacing, typography, and shadows.", "#107C10"),
            ("Chart Library", "15+ chart types with animations, tooltips, and data transforms.", "#D83B01"),
            ("MVVM Ready", "ViewModelBase, RelayCommand, Messenger, and state management.", "#E3008C"),
            ("Accessible", "Keyboard navigation, screen reader support, and ARIA attributes.", "#FFB900"),
        };

        for (int i = 0; i < features.Length; i++)
        {
            var (title, desc, color) = features[i];
            var col = i % 3;
            var row = i / 3;

            var card = new Border
            {
                Background = GetBrush("AuraCardBrush", "#FFFFFF"),
                BorderBrush = GetBrush("AuraBorderBrush", "#E0E0E0"),
                BorderThickness = new Avalonia.Thickness(1),
                CornerRadius = new CornerRadius(10),
                Padding = new Avalonia.Thickness(20),
                Margin = new Avalonia.Thickness(0, 0, 12, 12),
                Child = new StackPanel
                {
                    Spacing = 8,
                    Children =
                    {
                        new Border
                        {
                            Background = new SolidColorBrush(Color.Parse(color)),
                            CornerRadius = new CornerRadius(6),
                            Width = 36, Height = 36,
                            Child = new TextBlock
                            {
                                Text = title[..1],
                                FontSize = 18,
                                FontWeight = FontWeight.Bold,
                                Foreground = Brushes.White,
                                HorizontalAlignment = HorizontalAlignment.Center,
                                VerticalAlignment = VerticalAlignment.Center
                            }
                        },
                        new TextBlock
                        {
                            Text = title,
                            FontWeight = FontWeight.SemiBold,
                            FontSize = 15,
                            Foreground = GetBrush("AuraForegroundBrush")
                        },
                        new TextBlock
                        {
                            Text = desc,
                            FontSize = 13,
                            TextWrapping = TextWrapping.Wrap,
                            Foreground = GetBrush("AuraForegroundSecondaryBrush", "#666666")
                        }
                    }
                }
            };

            Grid.SetColumn(card, col);
            Grid.SetRow(card, row);
            grid.Children.Add(card);
        }

        return new StackPanel
        {
            Spacing = 8,
            Children =
            {
                CreateSectionTitle("Features"),
                grid
            }
        };
    }

    private Control BuildQuickStart()
    {
        return new StackPanel
        {
            Spacing = 8,
            Children =
            {
                CreateSectionTitle("Quick Start"),
                new CodeBlock
                {
                    Code = "dotnet new avalonia.app -n MyApp\ncd MyApp\ndotnet add package AuraUI.Controls\ndotnet add package AuraUI.Themes.Fluent",
                    Language = "bash"
                }
            }
        };
    }

    private Control BuildThemeSection()
    {
        return CreateExampleSection("Theme Toggle",
            new StackPanel
            {
                Spacing = 12,
                Orientation = Orientation.Horizontal,
                Children =
                {
                    new Button { Content = "Light Theme", Classes = { "primary" } },
                    new Button { Content = "Dark Theme" }
                }
            },
            @"<StackPanel Orientation=""Horizontal"" Spacing=""12"">
    <Button Content=""Light Theme"" Classes=""primary""/>
    <Button Content=""Dark Theme""/>
</StackPanel>",
            @"// In App.axaml.cs
if (Application.Current is { } app)
{
    app.RequestedThemeVariant = ThemeVariant.Light; // or ThemeVariant.Dark
}");
    }
}
