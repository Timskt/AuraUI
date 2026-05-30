using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using AuraUI.Controls.Navigation;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class FileExplorerPage : ComponentPageBase
{
    public override string ComponentName => "FileExplorer";
    public override string Description => "A file system browser control with tree, list, and grid view modes. Displays file system items with icons, sizes, and navigation.";
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
                    "Use FileExplorer for file management features, document pickers, or project browsers in desktop applications.",
                    "Default to Tree view for hierarchical data. Provide breadcrumbs for navigation context. Support keyboard shortcuts for common operations.")
            }
        };
    }

    private Control BuildBasicExample()
    {
        var explorer = new FileExplorer
        {
            ViewMode = FileViewMode.Tree,
            Width = 500,
            Height = 300
        };

        return CreateExampleSection("File Explorer", explorer,
            @"<nav:FileExplorer ViewMode=""Tree""
    Width=""500"" Height=""300""/>");
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "ViewMode", Type = "FileViewMode", Default = "Tree", Description = "Tree, List, or Grid view" },
        new ApiProperty { PropertyName = "RootPath", Type = "string", Default = "null", Description = "Root directory path" },
        new ApiProperty { PropertyName = "ShowHiddenFiles", Type = "bool", Default = "false", Description = "Show hidden files" },
        new ApiProperty { PropertyName = "FileFilter", Type = "string", Default = "null", Description = "File extension filter" },
        new ApiProperty { PropertyName = "SelectedFile", Type = "FileSystemItem", Default = "null", Description = "Currently selected file" },
    };
}
