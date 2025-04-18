using System.Threading.Tasks;
using Blazored.SessionStorage;
using Foundation.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Syncfusion.Blazor;

namespace Foundation.Blazor.Client;

public class Program
{
    public async static Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.Services.AddSyncfusionBlazor();
        builder.Services.AddBlazoredSessionStorage();


        var application = await builder.AddApplicationAsync<FoundationBlazorClientModule>(options =>
        {
            options.UseAutofac();
        });

        var host = builder.Build();        
        Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MzY0MjIzMEAzMjM4MmUzMDJlMzBDWGVhMGYreWNZTlJQYkdNWjRFczkxZkdxSFh5UW1wNHpiaVRaRFFBT1cwPQ==");        

        await application.InitializeApplicationAsync(host.Services);

        await host.RunAsync();
    }
}
