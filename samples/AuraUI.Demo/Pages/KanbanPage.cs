using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using AuraUI.Controls.Layout;

namespace AuraUI.Demo.Pages;

public class KanbanPage : ComponentPageBase
{
    public override string ComponentName => "Kanban Board";
    public override string Description => "A Kanban board scenario demonstrating multi-column task management with drag-and-drop cards between columns.";
    public override string Category => "Scenarios";

    private readonly Dictionary<string, List<KanbanTask>> _columns = new()
    {
        ["To Do"] = new List<KanbanTask>
        {
            new("Design login page", "High", "Sarah K.", "Design"),
            new("Set up CI/CD pipeline", "Medium", "Mike R.", "DevOps"),
            new("Write API docs", "Low", "Lisa M.", "Docs"),
            new("User research interviews", "Medium", "Anna S.", "Research"),
        },
        ["In Progress"] = new List<KanbanTask>
        {
            new("Build dashboard UI", "High", "John D.", "Frontend"),
            new("Database migration", "High", "Tom B.", "Backend"),
            new("Integration tests", "Medium", "Sarah K.", "QA"),
        },
        ["Done"] = new List<KanbanTask>
        {
            new("Project setup", "High", "John D.", "DevOps"),
            new("Wireframe review", "Medium", "Lisa M.", "Design"),
            new("API endpoint: /users", "High", "Tom B.", "Backend"),
            new("Component library audit", "Low", "Mike R.", "Frontend"),
            new("Sprint planning", "Medium", "Anna S.", "Planning"),
        }
    };

    private TextBlock? _todoCount;
    private TextBlock? _inProgressCount;
    private TextBlock? _doneCount;

    protected override Control BuildContent()
    {
        return new StackPanel
        {
            Spacing = 32,
            Children =
            {
                BuildKanbanExample(),
                BuildGuidelinesSection()
            }
        };
    }

    private Control BuildKanbanExample()
    {
        var board = new Grid
        {
            ColumnDefinitions = new ColumnDefinitions("*,*,*"),
            MaxWidth = 950,
            MinHeight = 450,
            Children =
            {
                SetColumn(BuildColumn("To Do", "#D83B01", ref _todoCount), 0),
                SetColumn(BuildColumn("In Progress", "#0078D4", ref _inProgressCount), 1),
                SetColumn(BuildColumn("Done", "#107C10", ref _doneCount), 2),
            }
        };

        UpdateCounts();

        return CreateExampleSection("Task Management Board", board,
            @"<!-- Kanban Column -->
<Border CornerRadius=""8"" Padding=""12"">
    <DockPanel>
        <DockPanel Dock=""Top"">
            <TextBlock Text=""To Do"" FontWeight=""SemiBold""/>
            <TextBlock Text=""4""/>  <!-- count badge -->
        </DockPanel>
        <ScrollViewer>
            <StackPanel Spacing=""8"">
                <!-- Task Card -->
                <Border CornerRadius=""6"" Padding=""12"">
                    <StackPanel Spacing=""4"">
                        <TextBlock Text=""Design login page""
                                   FontWeight=""SemiBold""/>
                        <StackPanel Orientation=""Horizontal"">
                            <layout:Tag Content=""High"" Variant=""Error""/>
                            <TextBlock Text=""Sarah K.""/>
                        </StackPanel>
                    </StackPanel>
                </Border>
            </StackPanel>
        </ScrollViewer>
    </DockPanel>
</Border>",
            @"private void OnDragOver(object? sender, DragEventArgs e)
{
    e.DragEffects = DragDropEffects.Move;
}

private void OnDrop(object? sender, DragEventArgs e)
{
    if (e.Data.Get(""KanbanTask"") is KanbanTask task)
    {
        // Move task to new column
        targetColumn.Add(task);
        sourceColumn.Remove(task);
    }
}");
    }

    private Control BuildColumn(string title, string accentColor, ref TextBlock? countRef)
    {
        var tasks = _columns[title];
        var headerBg = new SolidColorBrush(Color.Parse(accentColor)) { Opacity = 0.1 };
        var accentBrush = new SolidColorBrush(Color.Parse(accentColor));
        var borderBrush = GetBrush("AuraBorderBrush", "#E0E0E0");
        var cardBg = GetBrush("AuraCardBrush", "#FFFFFF");
        var fg = GetBrush("AuraForegroundBrush", "#000000");
        var fgSecondary = GetBrush("AuraForegroundSecondaryBrush", "#666666");
        var mutedBg = GetBrush("AuraMutedBrush", "#F5F5F5");

        var countBadge = new Border
        {
            Background = headerBg,
            CornerRadius = new CornerRadius(10),
            Padding = new Thickness(8, 2),
            VerticalAlignment = VerticalAlignment.Center,
            Child = new TextBlock
            {
                Text = tasks.Count.ToString(),
                FontSize = 12,
                FontWeight = FontWeight.SemiBold,
                Foreground = accentBrush
            }
        };
        var countText = (TextBlock)countBadge.Child!;
        countRef = countText;

        var cardsPanel = new StackPanel { Spacing = 8, Margin = new Thickness(0, 8, 0, 0) };

        foreach (var task in tasks)
        {
            cardsPanel.Children.Add(BuildTaskCard(task, accentColor));
        }

        // Build header panel
        var headerPanel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 8,
            Margin = new Thickness(0, 0, 0, 4),
            Children =
            {
                new TextBlock
                {
                    Text = title,
                    FontSize = 15,
                    FontWeight = FontWeight.SemiBold,
                    Foreground = fg,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(0, 0, 8, 0)
                },
                countBadge
            }
        };

        // Make the column a drop target
        var columnBorder = new Border
        {
            Background = GetBrush("AuraCardBrush", "#FFFFFF"),
            CornerRadius = new CornerRadius(8),
            BorderBrush = borderBrush,
            BorderThickness = new Thickness(1),
            Margin = new Thickness(4),
            Padding = new Thickness(12),
            Child = new DockPanel
            {
                Children =
                {
                    // Header
                    headerPanel,
                    // Cards list
                    new ScrollViewer
                    {
                        HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
                        Content = cardsPanel
                    }
                }
            }
        };

        DockPanel.SetDock(headerPanel, Dock.Top);

        // Set up drag-and-drop
        columnBorder.AddHandler(DragDrop.DragOverEvent, (_, e) =>
        {
            e.DragEffects = DragDropEffects.Move;
        });

        columnBorder.AddHandler(DragDrop.DropEvent, (_, e) =>
        {
            if (e.Data.Get("KanbanTask") is KanbanTask task)
            {
                // Remove from source
                foreach (var col in _columns.Values)
                    col.Remove(task);

                // Add to target
                _columns[title].Add(task);
                cardsPanel.Children.Add(BuildTaskCard(task, accentColor));
                UpdateCounts();
            }
        });

        return columnBorder;
    }

    private Control BuildTaskCard(KanbanTask task, string columnColor)
    {
        var priorityColor = task.Priority switch
        {
            "High" => "#D83B01",
            "Medium" => "#FFB900",
            "Low" => "#107C10",
            _ => "#666666"
        };

        var categoryColor = task.Category switch
        {
            "Design" => "#5C2D91",
            "Frontend" => "#0078D4",
            "Backend" => "#107C10",
            "DevOps" => "#D83B01",
            "QA" => "#FFB900",
            "Docs" => "#666666",
            "Research" => "#0078D4",
            "Planning" => "#5C2D91",
            _ => "#666666"
        };

        var card = new Border
        {
            Background = GetBrush("AuraCardBrush", "#FFFFFF"),
            CornerRadius = new CornerRadius(6),
            BorderBrush = GetBrush("AuraBorderBrush", "#E0E0E0"),
            BorderThickness = new Thickness(1),
            Padding = new Thickness(12),
            Cursor = new Cursor(StandardCursorType.Hand),
            Child = new StackPanel
            {
                Spacing = 8,
                Children =
                {
                    new TextBlock
                    {
                        Text = task.Title,
                        FontSize = 13,
                        FontWeight = FontWeight.SemiBold,
                        Foreground = GetBrush("AuraForegroundBrush", "#000000"),
                        TextWrapping = TextWrapping.Wrap
                    },
                    new StackPanel
                    {
                        Orientation = Orientation.Horizontal,
                        Spacing = 6,
                        Children =
                        {
                            // Priority badge
                            new Border
                            {
                                Background = new SolidColorBrush(Color.Parse(priorityColor)) { Opacity = 0.12 },
                                CornerRadius = new CornerRadius(4),
                                Padding = new Thickness(6, 2),
                                Child = new TextBlock
                                {
                                    Text = task.Priority,
                                    FontSize = 11,
                                    FontWeight = FontWeight.SemiBold,
                                    Foreground = new SolidColorBrush(Color.Parse(priorityColor))
                                }
                            },
                            // Category badge
                            new Border
                            {
                                Background = new SolidColorBrush(Color.Parse(categoryColor)) { Opacity = 0.12 },
                                CornerRadius = new CornerRadius(4),
                                Padding = new Thickness(6, 2),
                                Child = new TextBlock
                                {
                                    Text = task.Category,
                                    FontSize = 11,
                                    FontWeight = FontWeight.SemiBold,
                                    Foreground = new SolidColorBrush(Color.Parse(categoryColor))
                                }
                            }
                        }
                    },
                    new TextBlock
                    {
                        Text = task.Assignee,
                        FontSize = 12,
                        Foreground = GetBrush("AuraForegroundSecondaryBrush", "#666666")
                    }
                }
            }
        };

        // Drag support
        card.PointerPressed += async (_, e) =>
        {
            var data = new DataObject();
            data.Set("KanbanTask", task);
            await DragDrop.DoDragDrop(e, data, DragDropEffects.Move);
        };

        return card;
    }

    private void UpdateCounts()
    {
        if (_todoCount != null) _todoCount.Text = _columns["To Do"].Count.ToString();
        if (_inProgressCount != null) _inProgressCount.Text = _columns["In Progress"].Count.ToString();
        if (_doneCount != null) _doneCount.Text = _columns["Done"].Count.ToString();
    }

    private Control BuildGuidelinesSection()
    {
        return CreateGuidelines(
            "Use Kanban boards for project management, task tracking, and workflow visualization. They help teams see work status at a glance.",
            "Limit columns to 3-5 stages. Show task count per column. Use color-coded priority and category badges. Support drag-and-drop for task reordering. Include assignee information on each card.");
    }

    private static T SetColumn<T>(T control, int column) where T : Control
    {
        Grid.SetColumn(control, column);
        return control;
    }

    private record KanbanTask(string Title, string Priority, string Assignee, string Category);
}
