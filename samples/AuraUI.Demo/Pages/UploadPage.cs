using System.Collections.Generic;
using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;
using AuraUI.Controls.Input;
using AuraUI.Demo.Models;

namespace AuraUI.Demo.Pages;

public class UploadPage : ComponentPageBase
{
    public override string ComponentName => "Upload";
    public override string Description => "A file upload component supporting drag-and-drop, file type filtering, size validation, progress tracking, and image preview.";
    public override string Category => "Input";

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
                    "Use Upload for file submission forms, media uploads, document management, and any workflow requiring file input from users.",
                    "Set Accept to limit file types. Show progress for large files. Support drag-and-drop for convenience. Validate file size before upload.")
            }
        };
    }

    private Control BuildBasicExample()
    {
        var upload = new Upload
        {
            Drag = true,
            Multiple = true,
            Accept = "*.jpg,*.png,*.gif,*.pdf",
            MaxCount = 5,
            MaxSize = 10 * 1024 * 1024, // 10MB
            ListType = UploadListType.Text,
            ShowUploadList = true,
            Width = 450,
            FileList = new ObservableCollection<UploadFile>
            {
                new UploadFile { FileName = "photo.jpg", FileSize = 2048000, Status = UploadFileStatus.Success },
                new UploadFile { FileName = "document.pdf", FileSize = 1048576, Status = UploadFileStatus.Uploading, Progress = 65 }
            }
        };

        return CreateExampleSection("File Upload", upload,
            @"<input:Upload Drag=""True"" Multiple=""True""
    Accept=""*.jpg,*.png,*.gif,*.pdf""
    MaxCount=""5"" MaxSize=""10485760""
    ListType=""Text"" ShowUploadList=""True""/>");
    }

    private static IReadOnlyList<ApiProperty> GetApiProperties() => new[]
    {
        new ApiProperty { PropertyName = "Action", Type = "string", Default = "null", Description = "Upload URL endpoint" },
        new ApiProperty { PropertyName = "Accept", Type = "string", Default = "null", Description = "Accepted file types" },
        new ApiProperty { PropertyName = "Multiple", Type = "bool", Default = "false", Description = "Allow multiple files" },
        new ApiProperty { PropertyName = "MaxCount", Type = "int", Default = "0", Description = "Max file count (0 = unlimited)" },
        new ApiProperty { PropertyName = "MaxSize", Type = "long", Default = "0", Description = "Max file size in bytes" },
        new ApiProperty { PropertyName = "Drag", Type = "bool", Default = "false", Description = "Enable drag-and-drop" },
        new ApiProperty { PropertyName = "ListType", Type = "UploadListType", Default = "Text", Description = "Text, Picture, or PictureCard" },
        new ApiProperty { PropertyName = "FileList", Type = "ObservableCollection<UploadFile>", Default = "null", Description = "Uploaded files" },
    };
}
