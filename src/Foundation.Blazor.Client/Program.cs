using System.Net.Http;
using System;
using System.Threading.Tasks;
using Blazored.SessionStorage;
using Foundation.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
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

        var tempHttp = new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) };
        var configStream = await tempHttp.GetStreamAsync("appsettings.json");
        builder.Configuration.AddJsonStream(configStream);

        var apiBaseUrl = builder.Configuration["ApiBaseUrl"];
        if (string.IsNullOrWhiteSpace(apiBaseUrl))
            throw new Exception("ApiBaseUrl is missing or empty in appsettings.json");

        builder.Services.AddScoped(sp => new HttpClient
        {
            BaseAddress = new Uri(apiBaseUrl)
        });

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
