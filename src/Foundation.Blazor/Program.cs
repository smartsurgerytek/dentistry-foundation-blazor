using Blazored.SessionStorage;
using Foundation.Blazor;
using Foundation.Blazor.Client;
using Foundation.Blazor.Components;
using Foundation.Blazor.Services;
using Foundation.Dtos;
using Foundation.Services;


// using Foundation.Services;
using Syncfusion.Blazor;
using Syncfusion.EJ2.FileManager.AmazonS3FileProvider;
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
builder.Services.AddScoped<AmazonS3FileProvider>();

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