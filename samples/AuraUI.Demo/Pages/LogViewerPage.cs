using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class LogViewerPage : ComponentPageBase
{
    public override string ComponentName => "LogViewer";
    public override string Description => "A log viewer control for displaying structured log entries with severity levels, timestamps, source filtering, and color-coded output.";
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
                    "Use LogViewer for application logs, server monitoring, debugging consoles, and system event viewers.",
                    "Color-code by severity level. Support filtering by level and source. Auto-scroll for live logs. Allow search within logs. Show timestamps.")
            }
        };
    }

    private Control BuildBasicExample()
    {
        // Standard Avalonia fallback for LogViewer (custom control template not yet available)
        var logEntries = new[]
        {
            (DateTime.Now.AddMinutes(-5), "INFO", "App", "Application started successfully", "#107C10"),
            (DateTime.Now.AddMinutes(-4), "DEBUG", "DB", "Database connection pool initialized (max=20)", "#0078D4"),
            (DateTime.Now.AddMinutes(-3), "WARN", "API", "Rate limit threshold approaching: 85% of quota used", "#FFB900"),
            (DateTime.Now.AddMinutes(-2), "ERROR", "Auth", "Failed login attempt from 192.168.1.100", "#D83B01"),
            (DateTime.Now.AddMinutes(-1), "INFO", "App", "Health check passed", "#107C10"),
            (DateTime.Now, "DEBUG", "Cache", "Cache hit ratio: 94.2%", "#0078D4"),
        };

        var logPanel = new StackPanel { Spacing = 0 };
        foreach (var (ts, level, source, msg, color) in logEntries)
        {
            logPanel.Children.Add(new Border
            {
                BorderBrush = GetBrush("AuraBorderBrush", "#E0E0E0"),
                BorderThickness = new Thickness(0, 0, 0, 0.5),
                Padding = new Thickness(8, 4),
                Child = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Spacing = 8,
                    Children =
                    {
                        new TextBlock
                        {
                            Text = ts.ToString("HH:mm:ss"),
                            FontFamily = new FontFamily("Consolas,Menlo,Monaco,monospace"),
                            FontSize = 12,
                            Foreground = GetBrush("AuraForegroundSecondaryBrush", "#666666"),
                            Width = 70
                        },
                        new Border
                        {
                            Background = new SolidColorBrush(Color.Parse(color)) { Opacity = 0.12 },
                            CornerRadius = new CornerRadius(3),
                            Padding = new Thickness(6, 1),
                            Child = new TextBlock
                            {
                                Text = level,
                                FontSize = 11,
                                FontWeight = FontWeight.SemiBold,
                                Foreground = new SolidColorBrush(Color.Parse(color)),
                                FontFamily = new FontFamily("Consolas,Menlo,Monaco,monospace"),
                                Width = 45
                            }
                        },
                        new TextBlock
                        {
                            Text = source,
                            FontSize = 12,
                            FontWeight = FontWeight.SemiBold,
                            Foreground = GetBrush("AuraForegroundBrush", "#000000"),
                            Width = 50
                        },
                        new TextBlock
                        {
                            Text = msg,
                            FontSize = 12,
                            Foreground = GetBrush("AuraForegroundBrush", "#000000"),
                            TextWrapping = TextWrapping.Wrap
                        }
                    }
                }
            });
        }

        return CreateExampleSection("Log Viewer",
            new Border
            {
                Width = 600,
                Height = 300,
                Background = GetBrush("AuraCardBrush", "#FFFFFF"),
                BorderBrush = GetBrush("AuraBorderBrush", "#E0E0E0"),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(6),
                Clip = new RectangleGeometry(new Rect(0, 0, 600, 300)),
                Child = new ScrollViewer
                {
                    Content = logPanel
                }
            },
            @"<display:LogViewer Width=""600"" Height=""300"">
    <display:LogEntry Level=""Info"" Source=""App""
        Message=""Application started""/>
    <display:LogEntry Level=""Warn"" Source=""API""
        Message=""Rate limit approaching""/>
    <display:LogEntry Level=""Error"" Source=""Auth""
        Message=""Failed login attempt""/>
</display:LogViewer>");
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "Entries", Type = "ObservableCollection<LogEntry>", Default = "null", Description = "Log entries" },
        new ApiProperty { PropertyName = "MaxEntries", Type = "int", Default = "10000", Description = "Max entries before pruning" },
        new ApiProperty { PropertyName = "AutoScroll", Type = "bool", Default = "true", Description = "Auto-scroll on new entries" },
        new ApiProperty { PropertyName = "ShowTimestamp", Type = "bool", Default = "true", Description = "Show timestamps" },
        new ApiProperty { PropertyName = "FilterLevel", Type = "LogLevel?", Default = "null", Description = "Filter by minimum level" },
        new ApiProperty { PropertyName = "FilterSource", Type = "string", Default = "null", Description = "Filter by source" },
    };
}
