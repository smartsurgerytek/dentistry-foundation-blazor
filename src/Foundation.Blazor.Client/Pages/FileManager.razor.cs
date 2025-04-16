using System.Threading.Tasks;
using Foundation.Application.Contracts.Dtos;
using Syncfusion.Blazor.FileManager;
using Volo.Abp.EventBus.Distributed;

namespace Foundation.Blazor.Client.Pages;

public partial class FileManager : IDistributedEventHandler<ImageUploadEto>
{
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
}