using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Syncfusion.Blazor;

namespace SST.DentistryFoundation.Blazor.Client;

public class Program
{
    public async static Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.Services.AddSyncfusionBlazor();

        var application = await builder.AddApplicationAsync<DentistryFoundationBlazorClientModule>(options =>
        {
            options.UseAutofac();
        });

        var host = builder.Build();
        Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MzY0MjIzMEAzMjM4MmUzMDJlMzBDWGVhMGYreWNZTlJQYkdNWjRFczkxZkdxSFh5UW1wNHpiaVRaRFFBT1cwPQ==");


        await application.InitializeApplicationAsync(host.Services);

        await host.RunAsync();
    }
}
