using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Syncfusion.Blazor;

namespace Foundation.Blazor.Client;

public class Program
{
    public async static Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.Services.AddSyncfusionBlazor();


        var application = await builder.AddApplicationAsync<FoundationBlazorClientModule>(options =>
        {
            options.UseAutofac();
        });

        var host = builder.Build();        
        Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Mzc4Mjg0MkAzMjM4MmUzMDJlMzBtWTVJL051ZTkrbnlLVkQvOTlseTIvQ29QNlRVM2M4UWRNbWtYWmhwTUVJPQ==");        

        await application.InitializeApplicationAsync(host.Services);

        await host.RunAsync();
    }
}
