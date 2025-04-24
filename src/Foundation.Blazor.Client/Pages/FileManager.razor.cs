using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Foundation.Application.Contracts.Dtos;
using Foundation.Dtos;
using Syncfusion.Blazor.FileManager;
using Volo.Abp.EventBus.Distributed;

namespace Foundation.Blazor.Client.Pages;

public partial class FileManager
{
    public SfFileManager<FileManagerDirectoryContent>? SfFileManager;
    public List<ToolBarItemModel> Items = new List<ToolBarItemModel>(){
        new ToolBarItemModel() { Name = "NewFolder" },
        new ToolBarItemModel() { Name = "Cut" },
        new ToolBarItemModel() { Name = "Copy" },
        new ToolBarItemModel() { Name = "Paste" },
        new ToolBarItemModel() { Name = "Delete" },
        new ToolBarItemModel() { Name = "Download" },
        new ToolBarItemModel() { Name = "Rename" },
        new ToolBarItemModel() { Name = "SortBy" },
        new ToolBarItemModel() { Name = "Refresh" },
        new ToolBarItemModel() { Name = "ExaminationRecord" , TooltipText ="Click to generate an examination record" },
        new ToolBarItemModel() { Name = "Selection" },
        new ToolBarItemModel() { Name = "View" },
        new ToolBarItemModel() { Name = "Details" },
    };

    public bool IsExaminationRecordDisabled { get; set; } = false;

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
            return;
        }

        // check if the selected item is a folder
        var selectedItem = this.SfFileManager?.GetSelectedFiles()[0];
        if (!(selectedItem?.IsFile ?? true))
        {
            IsExaminationRecordDisabled = true;
            return;
        }

        // get the selected item
        var allSeparated = selectedItem?.Name?.Split(".");
        var selectedItemName = allSeparated?[0] ?? "";
        Console.WriteLine(selectedItemName);
        var selectedItemExtension = allSeparated?[^1] ?? "";
        Console.WriteLine(selectedItemExtension);

        // check if the selected item is an original image: contains _a with jpg,png,jpeg extensions
        var isOriginalImage = selectedItemName.EndsWith("_a") && (selectedItemExtension == "jpg" || selectedItemExtension == "png" || selectedItemExtension == "jpeg");
        // if it is an image, enable the button
        IsExaminationRecordDisabled = !isOriginalImage;
    }
}