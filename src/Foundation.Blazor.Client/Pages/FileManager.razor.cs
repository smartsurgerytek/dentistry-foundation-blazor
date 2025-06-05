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
using Castle.Core.Configuration;
using Microsoft.AspNetCore.WebUtilities;

namespace Foundation.Blazor.Client.Pages;

public partial class FileManager
{
    public bool IsImageEditorVisible { get; set; } = false;
    public bool IsOpenImageEditorBtnDisabled { get; set; } = true;
    public bool IsExaminationRecordDisabled { get; set; } = false;
    public bool IsAIImageBtnDisabled { get; set; } = true;
    public bool IsAIMeasurementBtnDisabled { get; set; } = true;
    public SfFileManager<FileManagerDirectoryContent>? SfFileManager;
    public SfImageEditor? ImageEditor;
    public string ApiUrlInternal { get; set; }
    public bool ShowSpinner;

    protected override async Task OnInitializedAsync()
    {
        ApiUrlInternal = Configuration["ApiUrlInternal"];
    }

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
        // new ToolBarItemModel() { Name = "OpenImageEditor" , TooltipText ="Click to edit the selected image" },
        new ToolBarItemModel() { Name = "AIImage" , TooltipText ="Click to get the segmented image" },
        new ToolBarItemModel() { Name = "MeasurementImage" , TooltipText ="Click to get the measurement image" },
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
        var selectedItems = this.SfFileManager?.GetSelectedFiles();        
        var selectedFileNames = selectedItems?.Select(i => i.FilterPath + i.Name).ToArray();
        var selectedFileNamesString = string.Join(",", selectedFileNames ?? Array.Empty<string>());        
        string patientId = string.Empty;
        var firstFilterPath = selectedItems[0].FilterPath; 
        var folderName = firstFilterPath.TrimEnd('/').Split('/').LastOrDefault();

        
// Get the current URI
    var uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);

        patientId = "1";

        if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("PatientId", out var queryPatientId))
        {
            patientId = queryPatientId;
        }



        NavigationManager.NavigateTo($"/ExaminationRecord?imageNames={selectedFileNamesString}&patientId={patientId}");
    }

    public void OnSelectedItemsChanged(string[] args)
    {
        IsExaminationRecordDisabled = true;
        IsAIImageBtnDisabled = true;
        IsAIMeasurementBtnDisabled = true;

        // check if any of the selected items is a folder or
        // check if any of the selected files is a non-image
        var imageExtensions = new[] { ".jpg", ".jpeg", ".png" }; // ".dcm" removed for now
        var selectedItem = this.SfFileManager?.GetSelectedFiles();
        if (selectedItem?.Any(i => !i.IsFile && !imageExtensions.Contains(i.Type?.ToLower())) ?? true)
        {
            return;
        }

        // get total selections: if 0 disable the buttons
        // if 1 enable the image editor button
        var totalSelections = args.Length;
        switch (totalSelections)
        {
            case 0:
                break;
            case 1:
                IsExaminationRecordDisabled = false;
                IsAIImageBtnDisabled = false;
                IsAIMeasurementBtnDisabled = false;
                break;
            default: // > 1
                IsExaminationRecordDisabled = false;
                break;
        }

        // new change, disable measurement btn for pano image
        var filterPath = selectedItem?[0].FilterPath;
        if (totalSelections == 1 && selectedItem != null && filterPath.Contains("pano", StringComparison.OrdinalIgnoreCase))
        {
            IsAIMeasurementBtnDisabled = true;
        }
    }

    public async Task GetAISegmentedImage(Microsoft.AspNetCore.Components.Web.MouseEventArgs args)
    {
        try
        {
            ShowSpinner = true;
            var selectedItem = this.SfFileManager?.GetSelectedFiles()[0];
            if (selectedItem == null || !selectedItem.IsFile)
            {
                return;
            }

            await HttpClient.GetAsync($"/api/FileProvider/GetSegmentedImage?filterPath={selectedItem.FilterPath}&fileName={selectedItem.Name}");
            ShowSpinner = false;
            SfFileManager?.RefreshFilesAsync();
        }
        catch (System.Exception ex)
        {
            Console.WriteLine($"An error occurred while getting the AI segmented image.{ex.Message}");
            throw;
        }
        finally
        {
            ShowSpinner = false;
        }
    }

    public async Task GetAIMeasurementImage(Microsoft.AspNetCore.Components.Web.MouseEventArgs args)
    {
        try
        {
            ShowSpinner = true;
            var selectedItem = this.SfFileManager?.GetSelectedFiles()[0];
            if (selectedItem == null || !selectedItem.IsFile)
            {
                return;
            }

            await HttpClient.GetAsync($"/api/FileProvider/GetMeasurementImage?filterPath={selectedItem.FilterPath}&fileName={selectedItem.Name}");
            ShowSpinner = false;
            SfFileManager?.RefreshFilesAsync();
        }
        catch (System.Exception ex)
        {
            Console.WriteLine($"An error occurred while getting the AI measurement image.{ex.Message}");
            throw;
        }
        finally
        {
            ShowSpinner = false;
        }
    }
}