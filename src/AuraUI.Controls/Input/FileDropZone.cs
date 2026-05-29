using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace AuraUI.Controls.Input;

/// <summary>
/// A content control that acts as a drop zone for files.
/// Provides visual feedback on drag over, validates files on drop,
/// and exposes events for drag/drop lifecycle.
/// </summary>
/// <example>
/// <code>
/// &lt;input:FileDropZone Accept=".jpg;.png" MaxFiles="5"&gt;
///     &lt;TextBlock Text="Drop images here"/&gt;
/// &lt;/input:FileDropZone&gt;
/// </code>
/// </example>
public class FileDropZone : ContentControl
{
    private bool _isDragOver;

    #region IsDragOver

    /// <summary>
    /// Defines the <see cref="IsDragOver"/> direct property.
    /// Indicates whether a drag operation is currently over the drop zone.
    /// </summary>
    public static readonly DirectProperty<FileDropZone, bool> IsDragOverProperty =
        AvaloniaProperty.RegisterDirect<FileDropZone, bool>(
            nameof(IsDragOver),
            o => o.IsDragOver);

    /// <summary>
    /// Gets whether a drag operation is currently over the drop zone.
    /// </summary>
    public bool IsDragOver
    {
        get => _isDragOver;
        private set => SetAndRaise(IsDragOverProperty, ref _isDragOver, value);
    }

    #endregion

    #region Accept

    /// <summary>
    /// Defines the <see cref="Accept"/> styled property.
    /// A semicolon-separated list of accepted file extensions (e.g. ".jpg;.png;.gif").
    /// When empty, all file types are accepted.
    /// </summary>
    public static readonly StyledProperty<string?> AcceptProperty =
        AvaloniaProperty.Register<FileDropZone, string?>(nameof(Accept));

    /// <summary>
    /// Gets or sets the accepted file types.
    /// </summary>
    public string? Accept
    {
        get => GetValue(AcceptProperty);
        set => SetValue(AcceptProperty, value);
    }

    #endregion

    #region MaxFiles

    /// <summary>
    /// Defines the <see cref="MaxFiles"/> styled property.
    /// The maximum number of files that can be dropped. 0 means no limit.
    /// </summary>
    public static readonly StyledProperty<int> MaxFilesProperty =
        AvaloniaProperty.Register<FileDropZone, int>(nameof(MaxFiles), 0);

    /// <summary>
    /// Gets or sets the maximum number of files.
    /// </summary>
    public int MaxFiles
    {
        get => GetValue(MaxFilesProperty);
        set => SetValue(MaxFilesProperty, value);
    }

    #endregion

    #region DragOverText

    /// <summary>
    /// Defines the <see cref="DragOverText"/> styled property.
    /// Text shown as an overlay when files are dragged over the zone.
    /// </summary>
    public static readonly StyledProperty<string?> DragOverTextProperty =
        AvaloniaProperty.Register<FileDropZone, string?>(nameof(DragOverText), "Drop files here");

    /// <summary>
    /// Gets or sets the drag-over overlay text.
    /// </summary>
    public string? DragOverText
    {
        get => GetValue(DragOverTextProperty);
        set => SetValue(DragOverTextProperty, value);
    }

    #endregion

    #region RejectedFiles

    /// <summary>
    /// Defines the <see cref="RejectedFiles"/> direct property.
    /// The list of files that were rejected during the last drop.
    /// </summary>
    public static readonly DirectProperty<FileDropZone, IReadOnlyList<string>> RejectedFilesProperty =
        AvaloniaProperty.RegisterDirect<FileDropZone, IReadOnlyList<string>>(
            nameof(RejectedFiles),
            o => o.RejectedFiles);

    private IReadOnlyList<string> _rejectedFiles = Array.Empty<string>();

    /// <summary>
    /// Gets the files that were rejected during the last drop.
    /// </summary>
    public IReadOnlyList<string> RejectedFiles
    {
        get => _rejectedFiles;
        private set => SetAndRaise(RejectedFilesProperty, ref _rejectedFiles, value);
    }

    #endregion

    #region Events

    /// <summary>
    /// Raised when valid files are dropped onto the zone.
    /// </summary>
    public event EventHandler<FilesDroppedEventArgs>? FilesDropped;

    /// <summary>
    /// Raised when a drag operation enters the drop zone.
    /// </summary>
    public event EventHandler<DragEventArgs>? DragEntered;

    /// <summary>
    /// Raised when a drag operation exits the drop zone.
    /// </summary>
    public event EventHandler<DragEventArgs>? DragExited;

    #endregion

    static FileDropZone()
    {
        IsDragOverProperty.Changed.AddClassHandler<FileDropZone>((x, _) => x.UpdatePseudoClasses());
    }

    public FileDropZone()
    {
        AddHandler(DragDrop.DragEnterEvent, OnDragEnter);
        AddHandler(DragDrop.DragLeaveEvent, OnDragLeave);
        AddHandler(DragDrop.DragOverEvent, OnDragOver);
        AddHandler(DragDrop.DropEvent, OnDrop);
        DragDrop.SetDrop(this, DropOperation.Copy);
    }

    private void OnDragEnter(object? sender, DragEventArgs e)
    {
        IsDragOver = true;
        e.DragEffects = DragDropEffects.Copy;
        DragEntered?.Invoke(this, e);
    }

    private void OnDragLeave(object? sender, DragEventArgs e)
    {
        IsDragOver = false;
        DragExited?.Invoke(this, e);
    }

    private void OnDragOver(object? sender, DragEventArgs e)
    {
        e.DragEffects = DragDropEffects.Copy;
    }

    private void OnDrop(object? sender, DragEventArgs e)
    {
        IsDragOver = false;

        var files = e.Data.GetFileNames()?.ToList();
        if (files is null || files.Count == 0)
        {
            RejectedFiles = Array.Empty<string>();
            return;
        }

        var accepted = new List<string>();
        var rejected = new List<string>();

        foreach (var path in files)
        {
            if (IsAccepted(path))
            {
                accepted.Add(path);
            }
            else
            {
                rejected.Add(path);
            }
        }

        // Enforce max files limit
        if (MaxFiles > 0 && accepted.Count > MaxFiles)
        {
            rejected.AddRange(accepted.Skip(MaxFiles));
            accepted = accepted.Take(MaxFiles).ToList();
        }

        RejectedFiles = rejected;

        if (accepted.Count > 0)
        {
            FilesDropped?.Invoke(this, new FilesDroppedEventArgs(accepted, rejected));
        }
    }

    private bool IsAccepted(string filePath)
    {
        if (string.IsNullOrWhiteSpace(Accept))
            return true;

        var extension = Path.GetExtension(filePath);
        if (string.IsNullOrEmpty(extension))
            return true;

        var accepted = Accept.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        return accepted.Any(a =>
            a.Equals(extension, StringComparison.OrdinalIgnoreCase) ||
            a.Equals("*" + extension, StringComparison.OrdinalIgnoreCase));
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(":dragover", IsDragOver);
    }
}

/// <summary>
/// Event arguments for file drop events.
/// </summary>
public class FilesDroppedEventArgs : EventArgs
{
    /// <summary>
    /// Gets the accepted file paths.
    /// </summary>
    public IReadOnlyList<string> AcceptedFiles { get; }

    /// <summary>
    /// Gets the rejected file paths.
    /// </summary>
    public IReadOnlyList<string> RejectedFiles { get; }

    public FilesDroppedEventArgs(IReadOnlyList<string> acceptedFiles, IReadOnlyList<string> rejectedFiles)
    {
        AcceptedFiles = acceptedFiles;
        RejectedFiles = rejectedFiles;
    }
}
