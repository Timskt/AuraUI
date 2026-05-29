using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Media;
using AuraUI.Demo.Components;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

/// <summary>
/// Base class for all component demo pages. Provides helper methods to create
/// consistent sections: header, live examples, code blocks, API tables, and guidelines.
/// </summary>
public abstract class ComponentPageBase : UserControl
{
    public abstract string ComponentName { get; }
    public abstract string Description { get; }
    public abstract string Category { get; }

    /// <summary>Helper to look up a resource by key.</summary>
    protected bool TryFindResource(string key, out object? resource)
    {
        try
        {
            if (Application.Current?.TryGetResource(key, ActualThemeVariant, out var res) == true)
            {
                resource = res;
                return true;
            }
        }
        catch { }
        resource = null;
        return false;
    }

    /// <summary>
    /// Override to build the page content using the helper methods.
    /// </summary>
    protected abstract Control BuildContent();

    protected override void OnInitialized()
    {
        base.OnInitialized();
        Content = BuildPageLayout();
    }

    private Control BuildPageLayout()
    {
        var scrollViewer = new ScrollViewer
        {
            HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
            Content = new StackPanel
            {
                Spacing = 24,
                Margin = new Thickness(32, 24, 32, 48),
                Children =
                {
                    BuildHeader(),
                    BuildContent()
                }
            }
        };
        return scrollViewer;
    }

    private Control BuildHeader()
    {
        return new StackPanel
        {
            Spacing = 8,
            Children =
            {
                new TextBlock
                {
                    Text = ComponentName,
                    FontSize = 28,
                    FontWeight = FontWeight.Bold,
                    Foreground = TryFindResource("AuraForegroundBrush", out var fb) ? (IBrush)fb! : Brushes.Black
                },
                new TextBlock
                {
                    Text = Description,
                    FontSize = 14,
                    TextWrapping = TextWrapping.Wrap,
                    Foreground = TryFindResource("AuraForegroundSecondaryBrush", out var sb) ? (IBrush)sb! : Brushes.Gray
                },
                CreateImportBadge()
            }
        };
    }

    private Control CreateImportBadge()
    {
        var ns = GetNamespace();
        var border = new Border
        {
            Background = TryFindResource("AuraMutedBrush", out var mb) ? (IBrush)mb! : new SolidColorBrush(Color.Parse("#F0F0F0")),
            CornerRadius = new CornerRadius(6),
            Padding = new Thickness(12, 6),
            MaxWidth = 600,
            Child = new TextBlock
            {
                Text = $"xmlns:{GetXmlPrefix()}=\"using:{ns}\"",
                FontFamily = new FontFamily("Consolas,Menlo,Monaco,monospace"),
                FontSize = 13,
                Foreground = TryFindResource("AuraForegroundBrush", out var fb) ? (IBrush)fb! : Brushes.Black,
                TextTrimming = TextTrimming.CharacterEllipsis
            }
        };
        return border;
    }

    private string GetNamespace()
    {
        return Category switch
        {
            "Layout" => "AuraUI.Controls.Layout",
            "Input" => "AuraUI.Controls.Input",
            "Selection" => "AuraUI.Controls.Selection",
            "Display" => "AuraUI.Controls.Display",
            "Navigation" => "AuraUI.Controls.Navigation",
            "Feedback" => "AuraUI.Controls.Feedback",
            "Charts" => "AuraUI.Controls.Charts",
            "Windowing" => "AuraUI.Controls.Windowing",
            _ => "AuraUI.Controls"
        };
    }

    private string GetXmlPrefix()
    {
        return Category switch
        {
            "Layout" => "layout",
            "Input" => "input",
            "Selection" => "selection",
            "Display" => "display",
            "Navigation" => "nav",
            "Feedback" => "feedback",
            "Charts" => "charts",
            "Windowing" => "windowing",
            _ => "aura"
        };
    }

    // ============================================================
    //  SECTION BUILDERS
    // ============================================================

    /// <summary>
    /// Creates a titled section with an optional subtitle.
    /// </summary>
    protected Control CreateSectionTitle(string title, string? subtitle = null)
    {
        var stack = new StackPanel { Spacing = 4 };
        stack.Children.Add(new TextBlock
        {
            Text = title,
            FontSize = 20,
            FontWeight = FontWeight.SemiBold,
            Foreground = TryFindResource("AuraForegroundBrush", out var fb) ? (IBrush)fb! : Brushes.Black
        });
        if (subtitle != null)
        {
            stack.Children.Add(new TextBlock
            {
                Text = subtitle,
                FontSize = 13,
                TextWrapping = TextWrapping.Wrap,
                Foreground = TryFindResource("AuraForegroundSecondaryBrush", out var sb) ? (IBrush)sb! : Brushes.Gray
            });
        }
        return stack;
    }

    /// <summary>
    /// Creates a tabbed example section with Preview/AXAML/C# tabs.
    /// </summary>
    protected Control CreateExampleSection(string title, Control example, string axamlCode, string? csharpCode = null)
    {
        var tabControl = new TabControl { MaxWidth = 800 };

        var previewTab = new TabItem
        {
            Header = "Preview",
            Content = new Border
            {
                Padding = new Thickness(20),
                BorderThickness = new Thickness(1),
                BorderBrush = TryFindResource("AuraBorderBrush", out var bb) ? (IBrush)bb! : Brushes.LightGray,
                CornerRadius = new CornerRadius(8),
                Background = TryFindResource("AuraCardBrush", out var cb) ? (IBrush)cb! : Brushes.White,
                Child = example
            }
        };

        var axamlTab = new TabItem
        {
            Header = "AXAML",
            Content = new CodeBlock
            {
                Code = axamlCode,
                Language = "axaml",
                Margin = new Thickness(0, -4, 0, 0)
            }
        };

        tabControl.Items.Add(previewTab);
        tabControl.Items.Add(axamlTab);

        if (csharpCode != null)
        {
            tabControl.Items.Add(new TabItem
            {
                Header = "C#",
                Content = new CodeBlock
                {
                    Code = csharpCode,
                    Language = "csharp",
                    Margin = new Thickness(0, -4, 0, 0)
                }
            });
        }

        return new StackPanel
        {
            Spacing = 8,
            Children =
            {
                CreateSectionTitle(title),
                tabControl
            }
        };
    }

    /// <summary>
    /// Creates an API property documentation table.
    /// </summary>
    protected Control CreateApiTable(IReadOnlyList<ApiProperty> properties)
    {
        var grid = new Grid
        {
            ColumnDefinitions = new ColumnDefinitions("180,120,100,*"),
            MaxWidth = 800,
            RowDefinitions = new RowDefinitions()
        };

        // Header row
        grid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
        var headers = new[] { "Property", "Type", "Default", "Description" };
        for (int i = 0; i < headers.Length; i++)
        {
            var headerBorder = new Border
            {
                Padding = new Thickness(10, 8),
                Background = TryFindResource("AuraMutedBrush", out var mb) ? (IBrush)mb! : new SolidColorBrush(Color.Parse("#F5F5F5")),
                BorderBrush = TryFindResource("AuraBorderBrush", out var bb) ? (IBrush)bb! : Brushes.LightGray,
                BorderThickness = new Thickness(0, 0, 1, 1),
                Child = new TextBlock
                {
                    Text = headers[i],
                    FontWeight = FontWeight.SemiBold,
                    FontSize = 13,
                    Foreground = TryFindResource("AuraForegroundBrush", out var fb) ? (IBrush)fb! : Brushes.Black
                }
            };
            Grid.SetColumn(headerBorder, i);
            Grid.SetRow(headerBorder, 0);
            grid.Children.Add(headerBorder);
        }

        // Data rows
        for (int r = 0; r < properties.Count; r++)
        {
            grid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
            var prop = properties[r];
            var values = new[] { prop.PropertyName, prop.Type, prop.Default, prop.Description };

            for (int c = 0; c < values.Length; c++)
            {
                var cellBorder = new Border
                {
                    Padding = new Thickness(10, 8),
                    BorderBrush = TryFindResource("AuraBorderBrush", out var bb2) ? (IBrush)bb2! : Brushes.LightGray,
                    BorderThickness = new Thickness(0, 0, 1, 1),
                    Child = new TextBlock
                    {
                        Text = values[c],
                        FontSize = 13,
                        FontFamily = c <= 2 ? new FontFamily("Consolas,Menlo,Monaco,monospace") : FontFamily.Default,
                        Foreground = TryFindResource("AuraForegroundBrush", out var fb2) ? (IBrush)fb2! : Brushes.Black,
                        TextWrapping = c == 3 ? TextWrapping.Wrap : TextWrapping.NoWrap,
                        VerticalAlignment = VerticalAlignment.Top
                    }
                };
                Grid.SetColumn(cellBorder, c);
                Grid.SetRow(cellBorder, r + 1);
                grid.Children.Add(cellBorder);
            }
        }

        return new StackPanel
        {
            Spacing = 8,
            Children =
            {
                CreateSectionTitle("API"),
                grid
            }
        };
    }

    /// <summary>
    /// Creates a design guidelines section with "When to use" and "Best practices".
    /// </summary>
    protected Control CreateGuidelines(string whenToUse, string bestPractices)
    {
        var panel = new StackPanel { Spacing = 16, MaxWidth = 800 };

        panel.Children.Add(CreateSectionTitle("Design Guidelines"));

        // When to use
        panel.Children.Add(new Border
        {
            Background = TryFindResource("AuraSuccessLightBrush", out var slb) ? (IBrush)slb! : new SolidColorBrush(Color.Parse("#E6F4EA")),
            CornerRadius = new CornerRadius(8),
            Padding = new Thickness(16),
            Child = new StackPanel
            {
                Spacing = 6,
                Children =
                {
                    new TextBlock
                    {
                        Text = "When to use",
                        FontWeight = FontWeight.SemiBold,
                        FontSize = 14,
                        Foreground = TryFindResource("AuraSuccessBrush", out var sb) ? (IBrush)sb! : new SolidColorBrush(Color.Parse("#107C10"))
                    },
                    new TextBlock
                    {
                        Text = whenToUse,
                        FontSize = 13,
                        TextWrapping = TextWrapping.Wrap,
                        Foreground = TryFindResource("AuraForegroundBrush", out var fb) ? (IBrush)fb! : Brushes.Black
                    }
                }
            }
        });

        // Best practices
        panel.Children.Add(new Border
        {
            Background = TryFindResource("AuraPrimaryLightBrush", out var plb) ? (IBrush)plb! : new SolidColorBrush(Color.Parse("#E8F0FE")),
            CornerRadius = new CornerRadius(8),
            Padding = new Thickness(16),
            Child = new StackPanel
            {
                Spacing = 6,
                Children =
                {
                    new TextBlock
                    {
                        Text = "Best Practices",
                        FontWeight = FontWeight.SemiBold,
                        FontSize = 14,
                        Foreground = TryFindResource("AuraPrimaryBrush", out var pb) ? (IBrush)pb! : new SolidColorBrush(Color.Parse("#0078D4"))
                    },
                    new TextBlock
                    {
                        Text = bestPractices,
                        FontSize = 13,
                        TextWrapping = TextWrapping.Wrap,
                        Foreground = TryFindResource("AuraForegroundBrush", out var fb2) ? (IBrush)fb2! : Brushes.Black
                    }
                }
            }
        });

        return panel;
    }

    /// <summary>
    /// Creates a simple wrap panel of example controls with a title.
    /// </summary>
    protected Control CreateSimpleExample(string title, params Control[] controls)
    {
        var wrap = new WrapPanel();
        foreach (var c in controls)
        {
            c.Margin = new Thickness(0, 0, 8, 8);
            wrap.Children.Add(c);
        }

        return new StackPanel
        {
            Spacing = 8,
            Children = { CreateSectionTitle(title), wrap }
        };
    }

    /// <summary>
    /// Helper: find a resource or return a fallback brush.
    /// </summary>
    protected IBrush GetBrush(string key, string fallback = "#000000")
    {
        return TryFindResource(key, out var val) && val is IBrush brush ? brush : new SolidColorBrush(Color.Parse(fallback));
    }
}
