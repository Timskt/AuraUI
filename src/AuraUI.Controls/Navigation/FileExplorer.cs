using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;

namespace AuraUI.Controls.Navigation;

/// <summary>
/// Specifies the view mode for the file explorer.
/// </summary>
public enum FileViewMode
{
    Tree,
    List,
    Grid
}

/// <summary>
/// Represents a file system item (file or directory).
/// </summary>
public class FileSystemItem : AvaloniaObject
{
    /// <summary>
    /// Defines the <see cref="Name"/> property.
    /// </summary>
    public static readonly StyledProperty<string?> NameProperty =
        AvaloniaProperty.Register<FileSystemItem, string?>(nameof(Name));

    /// <summary>
    /// Defines the <see cref="FullPath"/> property.
    /// </summary>
    public static readonly StyledProperty<string?> FullPathProperty =
        AvaloniaProperty.Register<FileSystemItem, string?>(nameof(FullPath));

    /// <summary>
    /// Defines the <see cref="IsDirectory"/> property.
    /// </summary>
    public static readonly StyledProperty<bool> IsDirectoryProperty =
        AvaloniaProperty.Register<FileSystemItem, bool>(nameof(IsDirectory));

    /// <summary>
    /// Defines the <see cref="Extension"/> property.
    /// </summary>
    public static readonly StyledProperty<string?> ExtensionProperty =
        AvaloniaProperty.Register<FileSystemItem, string?>(nameof(Extension));

    /// <summary>
    /// Defines the <see cref="Size"/> property.
    /// </summary>
    public static readonly StyledProperty<long> SizeProperty =
        AvaloniaProperty.Register<FileSystemItem, long>(nameof(Size));

    /// <summary>
    /// Defines the <see cref="LastModified"/> property.
    /// </summary>
    public static readonly StyledProperty<DateTime> LastModifiedProperty =
        AvaloniaProperty.Register<FileSystemItem, DateTime>(nameof(LastModified));

    /// <summary>
    /// Defines the <see cref="IsExpanded"/> property.
    /// </summary>
    public static readonly StyledProperty<bool> IsExpandedProperty =
        AvaloniaProperty.Register<FileSystemItem, bool>(nameof(IsExpanded));

    /// <summary>
    /// Defines the <see cref="Children"/> property.
    /// </summary>
    public static readonly StyledProperty<IList<FileSystemItem>?> ChildrenProperty =
        AvaloniaProperty.Register<FileSystemItem, IList<FileSystemItem>?>(nameof(Children));

    /// <summary>
    /// Gets or sets the display name.
    /// </summary>
    public string? Name
    {
        get => GetValue(NameProperty);
        set => SetValue(NameProperty, value);
    }

    /// <summary>
    /// Gets or sets the full file system path.
    /// </summary>
    public string? FullPath
    {
        get => GetValue(FullPathProperty);
        set => SetValue(FullPathProperty, value);
    }

    /// <summary>
    /// Gets or sets whether this item is a directory.
    /// </summary>
    public bool IsDirectory
    {
        get => GetValue(IsDirectoryProperty);
        set => SetValue(IsDirectoryProperty, value);
    }

    /// <summary>
    /// Gets or sets the file extension (e.g., ".cs", ".json").
    /// </summary>
    public string? Extension
    {
        get => GetValue(ExtensionProperty);
        set => SetValue(ExtensionProperty, value);
    }

    /// <summary>
    /// Gets or sets the file size in bytes.
    /// </summary>
    public long Size
    {
        get => GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }

    /// <summary>
    /// Gets or sets the last modified timestamp.
    /// </summary>
    public DateTime LastModified
    {
        get => GetValue(LastModifiedProperty);
        set => SetValue(LastModifiedProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the directory is expanded in the tree view.
    /// </summary>
    public bool IsExpanded
    {
        get => GetValue(IsExpandedProperty);
        set => SetValue(IsExpandedProperty, value);
    }

    /// <summary>
    /// Gets or sets child items (for directories).
    /// </summary>
    public IList<FileSystemItem>? Children
    {
        get => GetValue(ChildrenProperty);
        set => SetValue(ChildrenProperty, value);
    }
}

/// <summary>
/// A file explorer control with tree view, list view, and grid view modes.
/// Supports file icons based on extension, context menus, and navigation events.
///
/// Template parts:
///   PART_TreeView         - TreeView for hierarchical display
///   PART_ListView         - ListBox for list display
///   PART_GridView         - ItemsRepeater for grid display
///   PART_Breadcrumb       - Panel for breadcrumb navigation
///   PART_ContextMenu      - ContextMenu for file operations
///
/// Pseudo-classes: :tree, :list, :grid, :selected, :has-filter
/// </summary>
[TemplatePart("PART_TreeView", typeof(TreeView))]
[TemplatePart("PART_ListView", typeof(ListBox))]
[TemplatePart("PART_GridView", typeof(ItemsControl))]
[TemplatePart("PART_Breadcrumb", typeof(StackPanel))]
[TemplatePart("PART_ContextMenu", typeof(ContextMenu))]
[PseudoClasses(":tree", ":list", ":grid", ":selected", ":has-filter")]
public class FileExplorer : TemplatedControl
{
    private TreeView? _treeView;
    private ListBox? _listView;
    private ItemsControl? _gridView;
    private StackPanel? _breadcrumb;
    private ContextMenu? _contextMenu;

    /// <summary>
    /// Defines the <see cref="RootPath"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> RootPathProperty =
        AvaloniaProperty.Register<FileExplorer, string?>(nameof(RootPath));

    /// <summary>
    /// Defines the <see cref="SelectedPath"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> SelectedPathProperty =
        AvaloniaProperty.Register<FileExplorer, string?>(nameof(SelectedPath));

    /// <summary>
    /// Defines the <see cref="ShowHidden"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> ShowHiddenProperty =
        AvaloniaProperty.Register<FileExplorer, bool>(nameof(ShowHidden));

    /// <summary>
    /// Defines the <see cref="FileFilter"/> styled property.
    /// E.g., "*.cs;*.json" to show only matching files.
    /// </summary>
    public static readonly StyledProperty<string?> FileFilterProperty =
        AvaloniaProperty.Register<FileExplorer, string?>(nameof(FileFilter));

    /// <summary>
    /// Defines the <see cref="ViewMode"/> styled property.
    /// </summary>
    public static readonly StyledProperty<FileViewMode> ViewModeProperty =
        AvaloniaProperty.Register<FileExplorer, FileViewMode>(nameof(ViewMode), FileViewMode.Tree);

    /// <summary>
    /// Defines the <see cref="Items"/> styled property.
    /// The root-level items displayed in the explorer.
    /// </summary>
    public static readonly StyledProperty<IList<FileSystemItem>?> ItemsProperty =
        AvaloniaProperty.Register<FileExplorer, IList<FileSystemItem>?>(nameof(Items));

    /// <summary>
    /// Defines the <see cref="CurrentPath"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> CurrentPathProperty =
        AvaloniaProperty.Register<FileExplorer, string?>(nameof(CurrentPath));

    /// <summary>
    /// Raised when a file is selected.
    /// </summary>
    public event EventHandler<FileSystemItem>? FileSelected;

    /// <summary>
    /// Raised when a file is opened (double-click or Enter).
    /// </summary>
    public event EventHandler<FileSystemItem>? FileOpened;

    /// <summary>
    /// Raised when the current path changes.
    /// </summary>
    public event EventHandler<string?>? PathChanged;

    static FileExplorer()
    {
        ViewModeProperty.Changed.AddClassHandler<FileExplorer>((x, _) => x.UpdateViewModePseudoClasses());
        SelectedPathProperty.Changed.AddClassHandler<FileExplorer>((x, _) => x.OnSelectedPathChanged());
        RootPathProperty.Changed.AddClassHandler<FileExplorer>((x, _) => x.OnRootPathChanged());
        FileFilterProperty.Changed.AddClassHandler<FileExplorer>((x, _) => x.UpdatePseudoClasses());
    }

    /// <summary>
    /// Gets or sets the root directory path.
    /// </summary>
    public string? RootPath
    {
        get => GetValue(RootPathProperty);
        set => SetValue(RootPathProperty, value);
    }

    /// <summary>
    /// Gets or sets the currently selected file/directory path.
    /// </summary>
    public string? SelectedPath
    {
        get => GetValue(SelectedPathProperty);
        set => SetValue(SelectedPathProperty, value);
    }

    /// <summary>
    /// Gets or sets whether to show hidden files.
    /// </summary>
    public bool ShowHidden
    {
        get => GetValue(ShowHiddenProperty);
        set => SetValue(ShowHiddenProperty, value);
    }

    /// <summary>
    /// Gets or sets the file filter pattern.
    /// </summary>
    public string? FileFilter
    {
        get => GetValue(FileFilterProperty);
        set => SetValue(FileFilterProperty, value);
    }

    /// <summary>
    /// Gets or sets the current view mode.
    /// </summary>
    public FileViewMode ViewMode
    {
        get => GetValue(ViewModeProperty);
        set => SetValue(ViewModeProperty, value);
    }

    /// <summary>
    /// Gets or sets the root-level items to display.
    /// </summary>
    public IList<FileSystemItem>? Items
    {
        get => GetValue(ItemsProperty);
        set => SetValue(ItemsProperty, value);
    }

    /// <summary>
    /// Gets or sets the current navigation path.
    /// </summary>
    public string? CurrentPath
    {
        get => GetValue(CurrentPathProperty);
        set => SetValue(CurrentPathProperty, value);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        // Detach old parts
        if (_treeView is not null)
            _treeView.SelectionChanged -= OnTreeSelectionChanged;
        if (_listView is not null)
            _listView.SelectionChanged -= OnListSelectionChanged;

        base.OnApplyTemplate(e);

        _treeView = e.NameScope.Find<TreeView>("PART_TreeView");
        _listView = e.NameScope.Find<ListBox>("PART_ListView");
        _gridView = e.NameScope.Find<ItemsControl>("PART_GridView");
        _breadcrumb = e.NameScope.Find<StackPanel>("PART_Breadcrumb");
        _contextMenu = e.NameScope.Find<ContextMenu>("PART_ContextMenu");

        if (_treeView is not null)
            _treeView.SelectionChanged += OnTreeSelectionChanged;

        if (_listView is not null)
            _listView.SelectionChanged += OnListSelectionChanged;

        UpdateViewModePseudoClasses();
        UpdatePseudoClasses();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        if (_treeView is not null)
            _treeView.SelectionChanged -= OnTreeSelectionChanged;
        if (_listView is not null)
            _listView.SelectionChanged -= OnListSelectionChanged;
    }

    /// <summary>
    /// Navigates to a directory path and updates the view.
    /// </summary>
    public void NavigateTo(string path)
    {
        CurrentPath = path;
        PathChanged?.Invoke(this, path);
    }

    /// <summary>
    /// Loads the file system items from the root path.
    /// </summary>
    public void LoadFromRoot()
    {
        var root = RootPath;
        if (string.IsNullOrEmpty(root) || !Directory.Exists(root))
            return;

        var items = new ObservableCollection<FileSystemItem>();
        LoadDirectoryItems(root, items, 0, 2); // Load 2 levels deep
        Items = items;
        CurrentPath = root;
    }

    private void LoadDirectoryItems(string path, IList<FileSystemItem> items, int currentDepth, int maxDepth)
    {
        try
        {
            // Add directories
            foreach (var dir in Directory.GetDirectories(path))
            {
                var dirInfo = new DirectoryInfo(dir);

                if (!ShowHidden && dirInfo.Name.StartsWith('.'))
                    continue;

                var item = new FileSystemItem
                {
                    Name = dirInfo.Name,
                    FullPath = dirInfo.FullName,
                    IsDirectory = true,
                    LastModified = dirInfo.LastWriteTime,
                    Children = new ObservableCollection<FileSystemItem>()
                };

                if (currentDepth < maxDepth)
                {
                    LoadDirectoryItems(dirInfo.FullName, (ObservableCollection<FileSystemItem>)item.Children, currentDepth + 1, maxDepth);
                }

                items.Add(item);
            }

            // Add files
            var filter = FileFilter;
            var files = string.IsNullOrEmpty(filter)
                ? Directory.GetFiles(path)
                : Directory.GetFiles(path).Where(f => MatchesFilter(f, filter)).ToArray();

            foreach (var file in files)
            {
                var fileInfo = new FileInfo(file);

                if (!ShowHidden && fileInfo.Name.StartsWith('.'))
                    continue;

                items.Add(new FileSystemItem
                {
                    Name = fileInfo.Name,
                    FullPath = fileInfo.FullName,
                    IsDirectory = false,
                    Extension = fileInfo.Extension,
                    Size = fileInfo.Length,
                    LastModified = fileInfo.LastWriteTime
                });
            }
        }
        catch (UnauthorizedAccessException)
        {
            // Skip directories we can't access
        }
        catch (IOException)
        {
            // Skip IO errors
        }
    }

    private static bool MatchesFilter(string filePath, string filter)
    {
        var patterns = filter.Split(';', StringSplitOptions.RemoveEmptyEntries);
        var fileName = Path.GetFileName(filePath);

        foreach (var pattern in patterns)
        {
            var trimmedPattern = pattern.Trim();
            if (trimmedPattern.StartsWith("*."))
            {
                var ext = trimmedPattern[1..]; // e.g., ".cs"
                if (fileName.EndsWith(ext, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            else if (fileName.Contains(trimmedPattern, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Gets a default icon character based on the file extension.
    /// </summary>
    public static string GetFileIcon(string? extension)
    {
        return extension?.ToLowerInvariant() switch
        {
            ".cs" => "",       // Code icon
            ".js" or ".ts" => "",
            ".json" => "",
            ".xml" or ".xaml" or ".axaml" => "",
            ".py" => "",
            ".html" or ".htm" => "",
            ".css" or ".scss" => "",
            ".md" => "",
            ".txt" or ".log" => "",
            ".png" or ".jpg" or ".jpeg" or ".gif" or ".bmp" or ".svg" => "",
            ".pdf" => "",
            ".zip" or ".tar" or ".gz" or ".7z" => "",
            _ => ""
        };
    }

    private void OnTreeSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count > 0 && e.AddedItems[0] is FileSystemItem item)
        {
            SelectedPath = item.FullPath;
            FileSelected?.Invoke(this, item);

            if (!item.IsDirectory)
            {
                FileOpened?.Invoke(this, item);
            }
        }
    }

    private void OnListSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count > 0 && e.AddedItems[0] is FileSystemItem item)
        {
            SelectedPath = item.FullPath;
            FileSelected?.Invoke(this, item);
        }
    }

    private void OnSelectedPathChanged()
    {
        PseudoClasses.Set(":selected", !string.IsNullOrEmpty(SelectedPath));
    }

    private void OnRootPathChanged()
    {
        LoadFromRoot();
    }

    private void UpdateViewModePseudoClasses()
    {
        PseudoClasses.Set(":tree", ViewMode == FileViewMode.Tree);
        PseudoClasses.Set(":list", ViewMode == FileViewMode.List);
        PseudoClasses.Set(":grid", ViewMode == FileViewMode.Grid);
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(":has-filter", !string.IsNullOrEmpty(FileFilter));
    }
}
