using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Foundation.Dtos;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.ImageEditor;
using Syncfusion.Blazor.Spinner;

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
        this.IsImageEditorVisible = true;
        // load the selected image in the image editor
        // convert the selected file to a base64 string
        var imageStream = await this.httpClient.GetStreamAsync($"https://localhost:44355/api/FileProvider/AmazonS3GetImage?Path={Path}");
        var imageBytes = await imageStream.GetAllBytesAsync();
        var imageBase64 = Convert.ToBase64String(imageBytes);
        Console.WriteLine(imageBase64);
        string dataUrl = $"data:image/png;base64,{imageBase64}";
        await SfImageEditor?.OpenAsync(dataUrl);
        this.ShowSpinner = false;
    }

    public async void SaveAsync()
    {
        this.ShowSpinner = true;
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
        await httpClient.PostAsJsonAsync("https://localhost:44355/api/FileProvider/UploadImageAsStream", uploadStreamDto);

        // close the image editor
        this.IsImageEditorVisible = false;
        
        this.ShowSpinner = false;
        NavigationManager.NavigateTo("/FileManager");
    }
}