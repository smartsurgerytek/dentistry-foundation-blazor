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

namespace Foundation.Blazor.Client.Pages;

public partial class FileManager
{
    public bool IsImageEditorVisible { get; set; } = true;
    public bool IsOpenImageEditorBtnDisabled { get; set; } = true;
    public bool IsExaminationRecordDisabled { get; set; } = false;
    public SfFileManager<FileManagerDirectoryContent>? SfFileManager;
    public SfImageEditor? ImageEditor;

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

    public async Task OpenImageEditor()
    {
        this.IsImageEditorVisible = true;
        // load the selected image in the image editor
        var selectedItem = this.SfFileManager?.GetSelectedFiles()[0];
        await ImageEditor?.OpenAsync("https://ej2.syncfusion.com/react/demos/src/image-editor/images/bridge.png");
    }

    public void OnOverlayclick(OverlayModalClickEventArgs arg)
    {
        this.IsImageEditorVisible = false;
    }

    public async void OpenAsync()
    {
        await ImageEditor?.OpenAsync("https://ej2.syncfusion.com/react/demos/src/image-editor/images/bridge.png");
    }

    public Task HandleEventAsync(ImageUploadEto eventData)
    {
        throw new System.NotImplementedException();
    }

    public async void OpenFilePreview(FileOpenEventArgs<FileManagerDirectoryContent> args)
    {
        if (args.FileDetails.Type?.ToLower() == ".pdf")
        {
            await Task.Delay(50);
            NavigationManager.NavigateTo("/RecordViewer?fileName=" + args.FileDetails.Name);
        }
    }

    public async Task GenerateRecord(Microsoft.AspNetCore.Components.Web.MouseEventArgs args)
    {
        Console.WriteLine("Generate Record");
        // download the image as byte[]
        var selectedFile = this.SfFileManager?.GetSelectedFiles()[0];
        var imagePath = selectedFile?.FilterPath;
        var imageName = selectedFile?.Name;

        NavigationManager.NavigateTo($"/ExaminationRecord?imagePath={imagePath}&imageName={imageName}");
    }

    public void OnSelectedItemsChanged(string[] args)
    {
        // get total selections: if more than 1 disable the button
        var totalSelections = args.Length;
        if (totalSelections != 1)
        {
            IsExaminationRecordDisabled = true;
            IsOpenImageEditorBtnDisabled = true;
            return;
        }

        // check if the selected item is a folder
        var selectedItem = this.SfFileManager?.GetSelectedFiles()[0];
        if (!(selectedItem?.IsFile ?? true))
        {
            IsExaminationRecordDisabled = true;
            IsOpenImageEditorBtnDisabled = true;
            return;
        }

        // get the selected item
        var allSeparated = selectedItem?.Name?.Split(".");
        var selectedItemName = allSeparated != null ? string.Join(".", allSeparated[..^1]) : "";
        Console.WriteLine(selectedItemName);
        // get the selected item extension
        var selectedItemExtension = allSeparated?[^1] ?? "";
        Console.WriteLine(selectedItemExtension);

        // check if the selected item is an original image: contains _a with jpg,png,jpeg,dcm extensions
        var isSelectedFileAnImage = selectedItemExtension == "jpg" || selectedItemExtension == "png" || selectedItemExtension == "jpeg" || selectedItemExtension == "dcm";
        var isOriginalImage = selectedItemName.EndsWith("_a") && isSelectedFileAnImage;
        // if it is an image, enable the button
        IsExaminationRecordDisabled = !isOriginalImage;

        // disable image editor btn, if dicom image selected
        IsOpenImageEditorBtnDisabled = !(isSelectedFileAnImage && selectedItemExtension != "dcm");
    }
}