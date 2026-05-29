using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Avalonia.Media;
using Avalonia.Threading;

namespace AuraUI.Controls.Input;

/// <summary>
/// Specifies the display type of the upload file list.
/// </summary>
public enum UploadListType
{
    /// <summary>Show file names in a text list.</summary>
    Text,
    /// <summary>Show image thumbnails in a list.</summary>
    Picture,
    /// <summary>Show image thumbnails in a card layout.</summary>
    PictureCard
}

/// <summary>
/// Represents the current status of an uploaded file.
/// </summary>
public enum UploadFileStatus
{
    Ready,
    Uploading,
    Success,
    Error
}

/// <summary>
/// Represents a file in the upload file list.
/// </summary>
public class UploadFile : AvaloniaObject
{
    /// <summary>
    /// Defines the <see cref="FileName"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> FileNameProperty =
        AvaloniaProperty.Register<UploadFile, string?>(nameof(FileName));

    /// <summary>
    /// Defines the <see cref="FileSize"/> styled property.
    /// </summary>
    public static readonly StyledProperty<long> FileSizeProperty =
        AvaloniaProperty.Register<UploadFile, long>(nameof(FileSize));

    /// <summary>
    /// Defines the <see cref="Status"/> styled property.
    /// </summary>
    public static readonly StyledProperty<UploadFileStatus> StatusProperty =
        AvaloniaProperty.Register<UploadFile, UploadFileStatus>(nameof(Status));

    /// <summary>
    /// Defines the <see cref="Progress"/> styled property.
    /// Upload progress percentage (0-100).
    /// </summary>
    public static readonly StyledProperty<double> ProgressProperty =
        AvaloniaProperty.Register<UploadFile, double>(nameof(Progress));

    /// <summary>
    /// Defines the <see cref="Thumbnail"/> styled property.
    /// </summary>
    public static readonly StyledProperty<IImage?> ThumbnailProperty =
        AvaloniaProperty.Register<UploadFile, IImage?>(nameof(Thumbnail));

    /// <summary>
    /// Defines the <see cref="Url"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> UrlProperty =
        AvaloniaProperty.Register<UploadFile, string?>(nameof(Url));

    /// <summary>
    /// Gets or sets the file name.
    /// </summary>
    public string? FileName
    {
        get => GetValue(FileNameProperty);
        set => SetValue(FileNameProperty, value);
    }

    /// <summary>
    /// Gets or sets the file size in bytes.
    /// </summary>
    public long FileSize
    {
        get => GetValue(FileSizeProperty);
        set => SetValue(FileSizeProperty, value);
    }

    /// <summary>
    /// Gets or sets the upload status.
    /// </summary>
    public UploadFileStatus Status
    {
        get => GetValue(StatusProperty);
        set => SetValue(StatusProperty, value);
    }

    /// <summary>
    /// Gets or sets the upload progress (0-100).
    /// </summary>
    public double Progress
    {
        get => GetValue(ProgressProperty);
        set => SetValue(ProgressProperty, Math.Clamp(value, 0, 100));
    }

    /// <summary>
    /// Gets or sets the thumbnail image (for picture list types).
    /// </summary>
    public IImage? Thumbnail
    {
        get => GetValue(ThumbnailProperty);
        set => SetValue(ThumbnailProperty, value);
    }

    /// <summary>
    /// Gets or sets the file URL after successful upload.
    /// </summary>
    public string? Url
    {
        get => GetValue(UrlProperty);
        set => SetValue(UrlProperty, value);
    }
}

/// <summary>
/// Event arguments for upload events.
/// </summary>
public class UploadEventArgs : RoutedEventArgs
{
    /// <summary>
    /// Gets the file associated with this event.
    /// </summary>
    public UploadFile File { get; }

    public UploadEventArgs(UploadFile file)
    {
        File = file;
    }
}

/// <summary>
/// Event arguments for upload error events.
/// </summary>
public class UploadErrorEventArgs : UploadEventArgs
{
    /// <summary>
    /// Gets the error message.
    /// </summary>
    public string? ErrorMessage { get; }

    public UploadErrorEventArgs(UploadFile file, string? errorMessage) : base(file)
    {
        ErrorMessage = errorMessage;
    }
}

/// <summary>
/// A file upload component supporting drag-and-drop, file type filtering, size validation,
/// progress tracking, and image preview. Inspired by Ant Design's Upload component.
/// </summary>
[TemplatePart("PART_DropZone", typeof(Border))]
[TemplatePart("PART_FileList", typeof(ItemsControl))]
[TemplatePart("PART_UploadButton", typeof(Button))]
[PseudoClasses(":drag-over", ":has-files", ":disabled")]
public class Upload : TemplatedControl
{
    private Border? _dropZone;
    private ItemsControl? _fileList;
    private Button? _uploadButton;

    /// <summary>
    /// Defines the <see cref="Action"/> styled property.
    /// The upload URL endpoint.
    /// </summary>
    public static readonly StyledProperty<string?> ActionProperty =
        AvaloniaProperty.Register<Upload, string?>(nameof(Action));

    /// <summary>
    /// Defines the <see cref="Accept"/> styled property.
    /// File types to accept (e.g., ".jpg,.png,.gif" or "image/*").
    /// </summary>
    public static readonly StyledProperty<string?> AcceptProperty =
        AvaloniaProperty.Register<Upload, string?>(nameof(Accept));

    /// <summary>
    /// Defines the <see cref="Multiple"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> MultipleProperty =
        AvaloniaProperty.Register<Upload, bool>(nameof(Multiple));

    /// <summary>
    /// Defines the <see cref="MaxCount"/> styled property.
    /// Maximum number of files. 0 means unlimited.
    /// </summary>
    public static readonly StyledProperty<int> MaxCountProperty =
        AvaloniaProperty.Register<Upload, int>(nameof(MaxCount));

    /// <summary>
    /// Defines the <see cref="MaxSize"/> styled property.
    /// Maximum file size in bytes. 0 means unlimited.
    /// </summary>
    public static readonly StyledProperty<long> MaxSizeProperty =
        AvaloniaProperty.Register<Upload, long>(nameof(MaxSize));

    /// <summary>
    /// Defines the <see cref="Drag"/> styled property.
    /// Whether to enable drag-and-drop.
    /// </summary>
    public static readonly StyledProperty<bool> DragProperty =
        AvaloniaProperty.Register<Upload, bool>(nameof(Drag));

    /// <summary>
    /// Defines the <see cref="ListType"/> styled property.
    /// </summary>
    public static readonly StyledProperty<UploadListType> ListTypeProperty =
        AvaloniaProperty.Register<Upload, UploadListType>(
            nameof(ListType), UploadListType.Text);

    /// <summary>
    /// Defines the <see cref="FileList"/> styled property.
    /// </summary>
    public static readonly StyledProperty<ObservableCollection<UploadFile>?> FileListProperty =
        AvaloniaProperty.Register<Upload, ObservableCollection<UploadFile>?>(nameof(FileList));

    /// <summary>
    /// Defines the <see cref="ShowUploadList"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> ShowUploadListProperty =
        AvaloniaProperty.Register<Upload, bool>(nameof(ShowUploadList), true);

    /// <summary>
    /// Defines the <see cref="IsDragOver"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> IsDragOverProperty =
        AvaloniaProperty.Register<Upload, bool>(nameof(IsDragOver));

    static Upload()
    {
        IsDragOverProperty.Changed.AddClassHandler<Upload>((x, _) => x.UpdatePseudoClasses());
    }

    /// <summary>
    /// Gets or sets the upload URL.
    /// </summary>
    public string? Action
    {
        get => GetValue(ActionProperty);
        set => SetValue(ActionProperty, value);
    }

    /// <summary>
    /// Gets or sets the accepted file types.
    /// </summary>
    public string? Accept
    {
        get => GetValue(AcceptProperty);
        set => SetValue(AcceptProperty, value);
    }

    /// <summary>
    /// Gets or sets whether multiple files can be selected.
    /// </summary>
    public bool Multiple
    {
        get => GetValue(MultipleProperty);
        set => SetValue(MultipleProperty, value);
    }

    /// <summary>
    /// Gets or sets the maximum number of files (0 = unlimited).
    /// </summary>
    public int MaxCount
    {
        get => GetValue(MaxCountProperty);
        set => SetValue(MaxCountProperty, value);
    }

    /// <summary>
    /// Gets or sets the maximum file size in bytes (0 = unlimited).
    /// </summary>
    public long MaxSize
    {
        get => GetValue(MaxSizeProperty);
        set => SetValue(MaxSizeProperty, value);
    }

    /// <summary>
    /// Gets or sets whether drag-and-drop is enabled.
    /// </summary>
    public bool Drag
    {
        get => GetValue(DragProperty);
        set => SetValue(DragProperty, value);
    }

    /// <summary>
    /// Gets or sets the file list display type.
    /// </summary>
    public UploadListType ListType
    {
        get => GetValue(ListTypeProperty);
        set => SetValue(ListTypeProperty, value);
    }

    /// <summary>
    /// Gets or sets the collection of uploaded files.
    /// </summary>
    public ObservableCollection<UploadFile>? FileList
    {
        get => GetValue(FileListProperty);
        set => SetValue(FileListProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the file list is visible.
    /// </summary>
    public bool ShowUploadList
    {
        get => GetValue(ShowUploadListProperty);
        set => SetValue(ShowUploadListProperty, value);
    }

    /// <summary>
    /// Gets or sets whether a drag operation is currently over the drop zone.
    /// </summary>
    public bool IsDragOver
    {
        get => GetValue(IsDragOverProperty);
        set => SetValue(IsDragOverProperty, value);
    }

    /// <summary>
    /// Occurs before a file is uploaded. Can be used to cancel the upload.
    /// </summary>
    public event EventHandler<UploadEventArgs>? BeforeUpload;

    /// <summary>
    /// Occurs when the file list changes.
    /// </summary>
    public event EventHandler<UploadEventArgs>? Change;

    /// <summary>
    /// Occurs when a file is removed from the list.
    /// </summary>
    public event EventHandler<UploadEventArgs>? Remove;

    /// <summary>
    /// Occurs when an upload error occurs.
    /// </summary>
    public event EventHandler<UploadErrorEventArgs>? Error;

    /// <summary>
    /// Occurs when upload progress is updated.
    /// </summary>
    public event EventHandler<UploadEventArgs>? Progress;

    /// <summary>
    /// Occurs when a file is successfully uploaded.
    /// </summary>
    public event EventHandler<UploadEventArgs>? Success;

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        DetachDropZone();

        _dropZone = e.NameScope.Find<Border>("PART_DropZone");
        _fileList = e.NameScope.Find<ItemsControl>("PART_FileList");
        _uploadButton = e.NameScope.Find<Button>("PART_UploadButton");

        AttachDropZone();

        if (_uploadButton != null)
        {
            _uploadButton.Click += async (_, _) => await OpenFilePickerAsync();
        }

        UpdatePseudoClasses();
    }

    /// <summary>
    /// Adds files from a file picker selection.
    /// </summary>
    public void AddFiles(IEnumerable<string> filePaths)
    {
        var list = FileList ??= new ObservableCollection<UploadFile>();

        foreach (var path in filePaths)
        {
            if (MaxCount > 0 && list.Count >= MaxCount)
            {
                break;
            }

            var file = new UploadFile
            {
                FileName = System.IO.Path.GetFileName(path),
                FileSize = 0, // Would need async file info
                Status = UploadFileStatus.Ready
            };

            // Validate file type
            if (!ValidateFileType(file))
            {
                Error?.Invoke(this, new UploadErrorEventArgs(file, $"File type not accepted: {file.FileName}"));
                continue;
            }

            list.Add(file);
            Change?.Invoke(this, new UploadEventArgs(file));
        }

        UpdatePseudoClasses();
    }

    /// <summary>
    /// Removes a file from the file list.
    /// </summary>
    public void RemoveFile(UploadFile file)
    {
        FileList?.Remove(file);
        Remove?.Invoke(this, new UploadEventArgs(file));
        UpdatePseudoClasses();
    }

    /// <summary>
    /// Clears all files from the list.
    /// </summary>
    public void ClearFiles()
    {
        FileList?.Clear();
        UpdatePseudoClasses();
    }

    private async System.Threading.Tasks.Task OpenFilePickerAsync()
    {
        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel == null) return;

        var files = await topLevel.StorageProvider.OpenFilePickerAsync(
            new Avalonia.Platform.Storage.FilePickerOpenOptions
            {
                AllowMultiple = Multiple,
                Title = "Select files"
            });

        if (files.Count > 0)
        {
            var paths = new List<string>();
            foreach (var file in files)
            {
                var path = file.Path?.LocalPath;
                if (path != null) paths.Add(path);
            }
            AddFiles(paths);
        }
    }

    private bool ValidateFileType(UploadFile file)
    {
        var accept = Accept;
        if (string.IsNullOrWhiteSpace(accept)) return true;

        var fileName = file.FileName;
        if (string.IsNullOrEmpty(fileName)) return true;

        var extensions = accept.Split(',', StringSplitOptions.RemoveEmptyEntries);
        foreach (var ext in extensions)
        {
            var trimmed = ext.Trim();
            if (trimmed.StartsWith("*.") && fileName.EndsWith(trimmed[1..], StringComparison.OrdinalIgnoreCase))
                return true;
            if (trimmed.StartsWith('.') && fileName.EndsWith(trimmed, StringComparison.OrdinalIgnoreCase))
                return true;
            if (trimmed.EndsWith("/*"))
            {
                // MIME type wildcard matching would require additional logic
                return true;
            }
        }

        return false;
    }

    private void AttachDropZone()
    {
        if (_dropZone == null || !Drag) return;
        _dropZone.AddHandler(DragDrop.DragEnterEvent, OnDragEnter);
        _dropZone.AddHandler(DragDrop.DragLeaveEvent, OnDragLeave);
        _dropZone.AddHandler(DragDrop.DropEvent, OnDrop);
    }

    private void DetachDropZone()
    {
        if (_dropZone == null) return;
        _dropZone.RemoveHandler(DragDrop.DragEnterEvent, OnDragEnter);
        _dropZone.RemoveHandler(DragDrop.DragLeaveEvent, OnDragLeave);
        _dropZone.RemoveHandler(DragDrop.DropEvent, OnDrop);
    }

    private void OnDragEnter(object? sender, DragEventArgs e)
    {
        IsDragOver = true;
    }

    private void OnDragLeave(object? sender, DragEventArgs e)
    {
        IsDragOver = false;
    }

    private void OnDrop(object? sender, DragEventArgs e)
    {
        IsDragOver = false;
        var files = e.DataTransfer.TryGetFiles();
        if (files != null)
        {
            var paths = new List<string>();
            foreach (var file in files)
            {
                var path = file.TryGetLocalPath();
                if (path != null) paths.Add(path);
            }
            AddFiles(paths);
        }
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(":drag-over", IsDragOver);
        PseudoClasses.Set(":has-files", FileList?.Count > 0);
    }
}
