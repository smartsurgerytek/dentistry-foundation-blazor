using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Foundation.Localization;
using Foundation.Permissions;
using Foundation.MultiTenancy;
using Volo.Abp.Account.Localization;
using Volo.Abp.UI.Navigation;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.SettingManagement.Blazor.Menus;
using Volo.Abp.Users;
using Volo.Abp.Identity.Pro.Blazor.Navigation;
using Volo.Abp.AuditLogging.Blazor.Menus;
using Volo.Abp.LanguageManagement.Blazor.Menus;
using Volo.Abp.TextTemplateManagement.Blazor.Menus;
using Volo.Abp.OpenIddict.Pro.Blazor.Menus;
using Volo.Saas.Host.Blazor.Navigation;

namespace Foundation.Blazor.Client.Navigation;

public class FoundationMenuContributor : IMenuContributor
{
    private readonly IConfiguration _configuration;

    public FoundationMenuContributor(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task ConfigureMenuAsync(MenuConfigurationContext context)
    {
        if (context.Menu.Name == StandardMenus.Main)
        {
            await ConfigureMainMenuAsync(context);
        }
        else if (context.Menu.Name == StandardMenus.User)
        {
            await ConfigureUserMenuAsync(context);
        }
    }

    private static async Task ConfigureMainMenuAsync(MenuConfigurationContext context)
    {
        var l = context.GetLocalizer<FoundationResource>();
        
        //Administration
        var administration = context.Menu.GetAdministration();
        administration.Order = 5;

        context.Menu.AddItem(new ApplicationMenuItem(
            FoundationMenus.Home,
            l["Menu:Home"],
            "/",
            icon: "fas fa-home",
            order: 1
        ));

        context.Menu.AddItem(new ApplicationMenuItem(
            FoundationMenus.FileManager,
            "FileManager",
            "/FileManager",
            icon: "fa-solid fa-file",
            order: 2
        ));

        context.Menu.AddItem(new ApplicationMenuItem(
            FoundationMenus.WordProcessor,
            "WordProcessor",
            "/WordProcessor",
            icon: "fa-solid fa-folder-open",
            order: 3
        ));

        context.Menu.AddItem(new ApplicationMenuItem(
            FoundationMenus.RecordViewer,
            "RecordViewer",
            "/RecordViewer",
            icon: "fa-solid fa-file-lines",
            order: 4
        ));

        context.Menu.AddItem(new ApplicationMenuItem(
            FoundationMenus.Organization,
            "Organization",
            "/Organization",
            icon: "fa-solid fa-university",
            order: 5
        ));

        context.Menu.AddItem(new ApplicationMenuItem(
            FoundationMenus.Record,
            "Records",
            "/Records",
            icon: "fa-solid fa-file",
            order: 6
        ));

        //context.Menu.AddItem(new ApplicationMenuItem(
        //    FoundationMenus.Department,
        //    "Department",
        //    "/Department",
        //    icon: "fa-solid fa-university",
        //    order: 5
        //));

        //HostDashboard
        context.Menu.AddItem(
            new ApplicationMenuItem(
                FoundationMenus.HostDashboard,
                l["Menu:Dashboard"],
                "/HostDashboard",
                icon: "fa fa-chart-line",
                order: 3
            ).RequirePermissions(FoundationPermissions.Dashboard.Host)
        );

        //TenantDashboard
        context.Menu.AddItem(
            new ApplicationMenuItem(
                FoundationMenus.TenantDashboard,
                l["Menu:Dashboard"],
                "/Dashboard",
                icon: "fa fa-chart-line",
                order: 4
            ).RequirePermissions(FoundationPermissions.Dashboard.Tenant)
        );

        //Saas
        administration.SetSubItemOrder(SaasHostMenus.GroupName, 1);

        //Administration->Identity
        administration.SetSubItemOrder(IdentityProMenus.GroupName, 2);

        //Administration->OpenId
        administration.SetSubItemOrder(OpenIddictProMenus.GroupName, 3);

        //Administration->Language Management
        administration.SetSubItemOrder(LanguageManagementMenus.GroupName, 4);

        //Administration->Text Template Management
        administration.SetSubItemOrder(TextTemplateManagementMenus.GroupName, 5);

        //Administration->Audit Logs
        administration.SetSubItemOrder(AbpAuditLoggingMenus.GroupName, 6);

        //Administration->Settings
        administration.SetSubItemOrder(SettingManagementMenus.GroupName, 7);
    }

    private async Task ConfigureUserMenuAsync(MenuConfigurationContext context)
    {
        var accountStringLocalizer = context.GetLocalizer<AccountResource>();
        var authServerUrl = _configuration["AuthServer:Authority"] ?? "";

        context.Menu.AddItem(new ApplicationMenuItem(
            "Account.Manage",
            accountStringLocalizer["MyAccount"],
            $"{authServerUrl.EnsureEndsWith('/')}Account/Manage",
            icon: "fa fa-cog",
            order: 1000,
            target: "_blank").RequireAuthenticated());

        context.Menu.AddItem(new ApplicationMenuItem(
            "Account.SecurityLogs",
            accountStringLocalizer["MySecurityLogs"],
            $"{authServerUrl.EnsureEndsWith('/')}Account/SecurityLogs",
            icon: "fa fa-user-shield",
            order: 1001,
            target: "_blank").RequireAuthenticated());

        context.Menu.AddItem(new ApplicationMenuItem(
            "Account.Sessions", 
            accountStringLocalizer["Sessions"], 
            url: $"{authServerUrl.EnsureEndsWith('/')}Account/Sessions", 
            icon: "fa fa-clock", 
            order: 1002,
            target: "_blank").RequireAuthenticated());

        await Task.CompletedTask;
    }
}
