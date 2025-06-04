using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Foundation.Dtos;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.FileManager;
using Syncfusion.Blazor.ImageEditor;
using Syncfusion.Blazor.Navigations;
using Syncfusion.Blazor.Spinner;
using Volo.Abp.Account;

namespace Foundation.Blazor.Client.Pages;

public partial class ImageEditor
{
    [Parameter]
    [SupplyParameterFromQuery]
    public string Path { get; set; }

    public bool IsImageEditorVisible { get; set; } = false;
    public SfImageEditor? SfImageEditor;
    public bool ShowSpinner;

    [Inject]
    public HttpClient httpClient { get; set; }
    [Inject]
    public NavigationManager NavigationManager { get; set; }

    private List<ImageEditorToolbarItemModel> customToolbarItem = new List<ImageEditorToolbarItemModel>()
    {
        new ImageEditorToolbarItemModel { Name = "Open" },
        new ImageEditorToolbarItemModel { Name = "Undo" },
        new ImageEditorToolbarItemModel { Name = "Redo" },
        new ImageEditorToolbarItemModel { Name = "Zoom" },
        new ImageEditorToolbarItemModel { Name = "Crop" },
        new ImageEditorToolbarItemModel { Name = "Rotate" },
        new ImageEditorToolbarItemModel { Name = "HorizontalFlip" },
        new ImageEditorToolbarItemModel { Name = "VerticalFlip" },
        new ImageEditorToolbarItemModel { Name = "Straightening" },
        new ImageEditorToolbarItemModel { Name = "Annotation" },
        new ImageEditorToolbarItemModel { Name = "FineTune" },
        new ImageEditorToolbarItemModel { Name = "Filter" },
        new ImageEditorToolbarItemModel { Name = "Frame" },
        new ImageEditorToolbarItemModel { Name = "Redact" },
        new ImageEditorToolbarItemModel { Name = "Reset" },
        new ImageEditorToolbarItemModel { Name = "Save" },
        new ImageEditorToolbarItemModel { Text = "", PrefixIcon = "ai-icon", TooltipText = "Get AI Segmented Image", TabIndex = 0 },
        new ImageEditorToolbarItemModel { Text = "", TooltipText = "Get AI Measurement Image", PrefixIcon = "measure-icon", TabIndex = 1 }
    };

    private async void ToolbarItemClicked(Syncfusion.Blazor.Navigations.ClickEventArgs args)
    {
        if (args.Item.TooltipText == "Get AI Segmented Image")
        {
            await GetAISegmentedImage(args);
        }
        else if (args.Item.TooltipText == "Get AI Measurement Image")
        {
            await GetAIMeasurementImage(args);
        }
    }

    protected override async Task OnParametersSetAsync()
    {
        if (!string.IsNullOrWhiteSpace(Path))
        {
            await OpenImageEditor(Path);
        }

        await Task.CompletedTask;
    }

    public async Task OpenImageEditor(string Path)
    {
        this.ShowSpinner = true;
        StateHasChanged();
        this.IsImageEditorVisible = true;
        // load the selected image in the image editor
        // convert the selected file to a base64 string
        var imageStream = await this.httpClient.GetStreamAsync($"/api/FileProvider/AmazonS3GetImage?Path={Path}");
        var imageBytes = await imageStream.GetAllBytesAsync();
        var imageBase64 = Convert.ToBase64String(imageBytes);
        string dataUrl = $"data:image/png;base64,{imageBase64}";
        await SfImageEditor?.OpenAsync(dataUrl);
        this.ShowSpinner = false;
        StateHasChanged();
    }

    public void BackToFileManager()
    {
        this.IsImageEditorVisible = false;
        NavigationManager.NavigateTo("/FileManager");
    }

    public async void SaveAsync()
    {
        this.ShowSpinner = true;
        StateHasChanged();
        // save the image
        var imageDataUrl = await SfImageEditor?.GetImageDataUrlAsync();

        // get the selected item
        var allSeparated = Path?.Split("/");
        var fullFileName = allSeparated != null ? allSeparated[^1] : "";
        var fileNameSeparated = fullFileName.Split(".");

        // get the selected item name
        var fileName = fileNameSeparated != null ? string.Join(".", fileNameSeparated[..^1]) : "";
        // get the selected item extension
        var fileExtension = fileNameSeparated != null ? fileNameSeparated[^1] : "";

        var filterPath = (allSeparated != null ? string.Join("/", allSeparated[..^1]) : "") + "/";

        var uploadStreamDto = new ImageEditorUploadAndReplaceDto()
        {
            FileName = (fileName.Contains("_edited") ? fileName : fileName + "_edited") + "." + fileExtension,
            OldFileName = fileName + "." + fileExtension,
            FilePath = filterPath,
            FileStream = imageDataUrl
        };
        await httpClient.PostAsJsonAsync("/api/FileProvider/UploadImageAsStream", uploadStreamDto);

        // close the image editor
        this.IsImageEditorVisible = false;

        this.ShowSpinner = false;
        StateHasChanged();
        NavigationManager.NavigateTo("/FileManager");
    }

    public async Task GetAISegmentedImage(Syncfusion.Blazor.Navigations.ClickEventArgs args)
    {
        try
        {
            this.ShowSpinner = true;
            StateHasChanged();

            var pathSplit = Path?.Split("/");
            var fullFileName = pathSplit != null ? pathSplit[^1] : "";
            var filterPath = (pathSplit != null ? string.Join("/", pathSplit[..^1]) : "") + "/";

            // call the API to get the AI segmented image
            // make an a+b image, a = original image, b = AI segmented image
            // the API will return a+b image as a base64 string
            // and the image will be opened in the image editor

            var combinedImageFileName = System.IO.Path.GetFileNameWithoutExtension(fullFileName) + "_ai_ab." + "png";
            var abImage = await this.httpClient.GetStringAsync($"/api/FileProvider/GetSegmentedImage?filterPath={filterPath}&fileName={fullFileName}&combinedImageFileName={combinedImageFileName}");

            // open the image editor with the a+b image
            Path = System.IO.Path.Combine(filterPath, combinedImageFileName);
            Console.WriteLine($"Path: {Path}");
            await SfImageEditor?.OpenAsync($"data:image/png;base64,{abImage}");
            this.ShowSpinner = false;
            StateHasChanged();
        }
        catch (System.Exception ex)
        {
            Console.WriteLine($"An error occurred while getting the AI segmented image.{ex.Message}");
            throw;
        }
        finally
        {
            this.ShowSpinner = false;
            StateHasChanged();
        }
    }

    public async Task GetAIMeasurementImage(Syncfusion.Blazor.Navigations.ClickEventArgs args)
    {
        try
        {
            ShowSpinner = true;
            StateHasChanged();

            var pathSplit = Path?.Split("/");
            var fullFileName = pathSplit != null ? pathSplit[^1] : "";
            var filterPath = (pathSplit != null ? string.Join("/", pathSplit[..^1]) : "") + "/";

            var combinedImageFileName = System.IO.Path.GetFileNameWithoutExtension(fullFileName) + "_measurement_ab." + "png";

            var abImage = await this.httpClient.GetStringAsync($"/api/FileProvider/GetMeasurementImage?filterPath={filterPath}&fileName={fullFileName}");

            // open the image editor with the a+b image
            Path = System.IO.Path.Combine(filterPath, combinedImageFileName);
            Console.WriteLine($"Path: {Path}");
            await SfImageEditor?.OpenAsync($"data:image/png;base64,{abImage}");
            this.ShowSpinner = false;
            StateHasChanged();
        }
        catch (System.Exception ex)
        {
            Console.WriteLine($"An error occurred while getting the AI measurement image.{ex.Message}");
            throw;
        }
        finally
        {
            ShowSpinner = false;
            StateHasChanged();
        }
    }
}