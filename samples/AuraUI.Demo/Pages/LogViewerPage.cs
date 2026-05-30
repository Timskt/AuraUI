using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;
using AuraUI.Controls.Display;
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
        var viewer = new LogViewer
        {
            Width = 600,
            Height = 300,
            Entries = new ObservableCollection<LogEntry>
            {
                new LogEntry { Timestamp = DateTime.Now.AddMinutes(-5), Level = LogLevel.Info, Source = "App", Message = "Application started successfully" },
                new LogEntry { Timestamp = DateTime.Now.AddMinutes(-4), Level = LogLevel.Debug, Source = "DB", Message = "Database connection pool initialized (max=20)" },
                new LogEntry { Timestamp = DateTime.Now.AddMinutes(-3), Level = LogLevel.Warn, Source = "API", Message = "Rate limit threshold approaching: 85% of quota used" },
                new LogEntry { Timestamp = DateTime.Now.AddMinutes(-2), Level = LogLevel.Error, Source = "Auth", Message = "Failed login attempt from 192.168.1.100" },
                new LogEntry { Timestamp = DateTime.Now.AddMinutes(-1), Level = LogLevel.Info, Source = "App", Message = "Health check passed" },
                new LogEntry { Timestamp = DateTime.Now, Level = LogLevel.Debug, Source = "Cache", Message = "Cache hit ratio: 94.2%" },
            }
        };

        return CreateExampleSection("Log Viewer", viewer,
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
