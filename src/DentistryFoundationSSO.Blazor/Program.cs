//using DentistryFoundationSSO.Blazor;
//using DentistryFoundationSSO.Blazor.Client;
//using DentistryFoundationSSO.Blazor.Components;
//using Syncfusion.Blazor;
//using Volo.Abp.AspNetCore.Components.WebAssembly.WebApp;

//var builder = WebApplication.CreateBuilder(args);

////https://github.com/dotnet/aspnetcore/issues/52530
//builder.Services.Configure<RouteOptions>(options =>
//{
//    options.SuppressCheckForUnhandledSecurityMetadata = true;
//});

//// Add services to the container.
//builder.Services.AddRazorComponents()
//    .AddInteractiveWebAssemblyComponents();

//builder.Services.AddAntiforgery(options =>
//{
//    options.SuppressXFrameOptionsHeader = true;
//});

//var app = builder.Build();

//// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseWebAssemblyDebugging();
//}
//else
//{
//    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
//    app.UseHsts();
//}

//app.UseHttpsRedirection();

//app.MapStaticAssets();
//app.UseAntiforgery();

//app.MapRazorComponents<App>()
//    .AddInteractiveWebAssemblyRenderMode()
//    .AddAdditionalAssemblies(WebAppAdditionalAssembliesHelper.GetAssemblies<DentistryFoundationSSOBlazorClientModule>());

//await app.RunAsync();


//using SST.DentistryFoundation.Blazor;
//using SST.DentistryFoundation.Blazor.Client;
//using Volo.Abp.AspNetCore.Components.WebAssembly.WebApp;
//using Syncfusion.Blazor;

using DentistryFoundationSSO.Blazor.Client;
using DentistryFoundationSSO.Blazor.Components;
using Syncfusion.Blazor;
using Volo.Abp.AspNetCore.Components.WebAssembly.WebApp;

var builder = WebApplication.CreateBuilder(args);

//https://github.com/dotnet/aspnetcore/issues/52530
builder.Services.Configure<RouteOptions>(options =>
{
    options.SuppressCheckForUnhandledSecurityMetadata = true;
});

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents()
    .AddInteractiveServerComponents();
builder.Services.AddSyncfusionBlazor();
builder.Services.AddControllers();

var app = builder.Build();

var syncFusionAPIKey = Environment.GetEnvironmentVariable("SyncFusionAPIKey");
Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(syncFusionAPIKey);


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

app.UseRouting();
app.MapControllers();
app.MapStaticAssets();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()  
    .AddInteractiveServerRenderMode()
    .AddAdditionalAssemblies(WebAppAdditionalAssembliesHelper.GetAssemblies<DentistryFoundationSSOBlazorClientModule>());

await app.RunAsync();