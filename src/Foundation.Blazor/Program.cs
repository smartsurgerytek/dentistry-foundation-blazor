using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using Blazored.SessionStorage;
using Foundation.Blazor;
using Foundation.Blazor.Client;
using Foundation.Blazor.Components;
using Foundation.Blazor.Services;
using Foundation.Dtos;
using Foundation.Services;


// using Foundation.Services;
using Syncfusion.Blazor;
using Syncfusion.EJ2.FileManager.FileProvider;
using Volo.Abp.AspNetCore.Components.WebAssembly.WebApp;

var builder = WebApplication.CreateBuilder(args);

//https://github.com/dotnet/aspnetcore/issues/52530
builder.Services.Configure<RouteOptions>(options =>
{
    options.SuppressCheckForUnhandledSecurityMetadata = true;
});

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddSyncfusionBlazor();
builder.Services.AddControllers();
builder.Services.AddHttpClient();
builder.Services.AddScoped<FileProvider>();
builder.Services.AddSingleton<IAmazonS3>((options) => {
    var configuration = builder.Configuration;

    var fileProvider = configuration["FileProvider"];
    if (!string.IsNullOrEmpty(fileProvider))
    {
        var bucketName = configuration[$"{fileProvider}:BucketName"];
        var accessKey = configuration[$"{fileProvider}:AccessKey"];
        var secretKey = configuration[$"{fileProvider}:SecretKey"];
        var region = configuration[$"{fileProvider}:Region"];
        var serviceUrl = configuration[$"{fileProvider}:ServiceUrl"];

        if (fileProvider != null && string.Compare(fileProvider, "amazons3", StringComparison.OrdinalIgnoreCase) == 0)
        {
            RegionEndpoint bucketRegion = RegionEndpoint.GetBySystemName(region);
            return new AmazonS3Client(accessKey, secretKey, bucketRegion);
        }
        else
        {
            var config = new AmazonS3Config
            {
                AuthenticationRegion = "",
                ServiceURL = serviceUrl,
                ForcePathStyle = true // MUST be true to work correctly with MinIO server
            };

            return new AmazonS3Client(accessKey, secretKey, config);
        }
    }

    throw new Exception("FileProvider configuration is missing.");
});
builder.Services.AddSingleton<TransferUtility>((options) =>
{
    var s3Client = options.GetRequiredService<IAmazonS3>();
    return new TransferUtility(s3Client);
});
System.Net.ServicePointManager.Expect100Continue = false;
builder.Services.AddScoped<DentistryApiService>();
builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
builder.Services.AddBlazoredSessionStorage();

var app = builder.Build();
var syncFusionAPIKey = Environment.GetEnvironmentVariable("SyncFusionAPIKey");
Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(syncFusionAPIKey);

app.MapGet("/environment", (IWebHostEnvironment env) =>
{
    return Results.Ok(new
    {
        EnvironmentName = env.EnvironmentName,
        WebRootPath = env.WebRootPath,
        ContentRootPath = env.ContentRootPath
    });
});


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.MapControllers();
//app.MapStaticAssets();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(WebAppAdditionalAssembliesHelper.GetAssemblies<FoundationBlazorClientModule>());

await app.RunAsync();