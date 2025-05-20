using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Foundation.Application.Contracts.Dtos;
using Foundation.Dtos;
using Syncfusion.Blazor.FileManager;
using Syncfusion.Blazor.Popups;
using Syncfusion.Blazor.ImageEditor;
using Volo.Abp.EventBus.Distributed;
using System.Diagnostics;
using Microsoft.AspNetCore.Components;
using System.IO;
using System.Linq;

namespace Foundation.Blazor.Client.Pages;

public partial class FileManager
{
    public bool IsImageEditorVisible { get; set; } = false;
    public bool IsOpenImageEditorBtnDisabled { get; set; } = true;
    public bool IsExaminationRecordDisabled { get; set; } = false;
    public SfFileManager<FileManagerDirectoryContent>? SfFileManager;
    public SfImageEditor? ImageEditor;

    [Inject]
    public HttpClient httpClient { get; set; }

    public List<ToolBarItemModel> Items = new List<ToolBarItemModel>(){
        new ToolBarItemModel() { Name = "NewFolder" },
        new ToolBarItemModel() { Name = "Cut" },
        new ToolBarItemModel() { Name = "Copy" },
        new ToolBarItemModel() { Name = "Paste" },
        new ToolBarItemModel() { Name = "Delete" },
        new ToolBarItemModel() { Name = "Download" },
        new ToolBarItemModel() { Name = "Upload" },
        new ToolBarItemModel() { Name = "Rename" },
        new ToolBarItemModel() { Name = "SortBy" },
        new ToolBarItemModel() { Name = "Refresh" },
        new ToolBarItemModel() { Name = "ExaminationRecord" , TooltipText ="Click to generate an examination record" },
        new ToolBarItemModel() { Name = "OpenImageEditor" , TooltipText ="Click to edit the selected image" },
        new ToolBarItemModel() { Name = "Selection" },
        new ToolBarItemModel() { Name = "View" },
        new ToolBarItemModel() { Name = "Details" },
    };

    public Task HandleEventAsync(ImageUploadEto eventData)
    {
        throw new System.NotImplementedException();
    }

    public async void OpenFilePreview(FileOpenEventArgs<FileManagerDirectoryContent> args)
    {
        string[] imageExtensions = { ".jpg", ".jpeg", ".png" };
        if (args.FileDetails.IsFile && args.FileDetails.Type?.ToLower() == ".pdf")
        {
            await Task.Delay(50);
            NavigationManager.NavigateTo("/RecordViewer?fileName=" + args.FileDetails.Name);
        }
        else if (args.FileDetails.IsFile && imageExtensions.Contains(args.FileDetails.Type?.ToLower()))
        {
            await Task.Delay(50);
            await OpenImageEditor(null);
        }
    }

    public async void SuppressImagePreview(BeforePopupOpenCloseEventArgs args)
    {
        if (args.PopupName == "Image Preview")
        {
            args.Cancel = true;
            await OpenImageEditor(null);
        }
    }

    public async Task OpenImageEditor(Microsoft.AspNetCore.Components.Web.MouseEventArgs args)
    {
        var selectedItem = this.SfFileManager?.GetSelectedFiles()[0];
        var imagePath = selectedItem?.FilterPath + selectedItem?.Name;
        NavigationManager.NavigateTo($"/ImageEditor?Path={imagePath}");
    }

    public async Task GenerateRecord(Microsoft.AspNetCore.Components.Web.MouseEventArgs args)
    {
        Console.WriteLine("Generate Record");
        var selectedItems = this.SfFileManager?.GetSelectedFiles();
        Console.WriteLine(selectedItems?.Count ?? 0);
        var selectedFileNames = selectedItems?.Select(i => i.FilterPath + i.Name).ToArray();
        var selectedFileNamesString = string.Join(",", selectedFileNames ?? Array.Empty<string>());
        Console.WriteLine(selectedFileNamesString);

        NavigationManager.NavigateTo($"/ExaminationRecord?imageNames={selectedFileNamesString}");
    }

    public void OnSelectedItemsChanged(string[] args)
    {
        IsExaminationRecordDisabled = false;
        IsOpenImageEditorBtnDisabled = false;

        // check if any of the selected items is a folder or
        // check if any of the selected files is a non-image
        var imageExtensions = new[] { ".jpg", ".jpeg", ".png" }; // ".dcm" removed for now
        var selectedItem = this.SfFileManager?.GetSelectedFiles();
        if (selectedItem?.Any(i => !i.IsFile && !imageExtensions.Contains(i.Type?.ToLower())) ?? true)
        {
            IsExaminationRecordDisabled = true;
            IsOpenImageEditorBtnDisabled = true;
            return;
        }

        // get total selections: if 0 disable the buttons
        // if 1 enable the image editor button
        var totalSelections = args.Length;
        if (totalSelections == 0)
        {
            IsExaminationRecordDisabled = true;
            IsOpenImageEditorBtnDisabled = true;
            return;
        }
        else if (totalSelections > 1)
        {
            IsOpenImageEditorBtnDisabled = true;
            return;
        }
    }
}