using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using SST.DentistryFoundation.Localization;
using SST.DentistryFoundation.Permissions;
using SST.DentistryFoundation.MultiTenancy;
using Volo.Abp.Account.Localization;
using Volo.Abp.UI.Navigation;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.SettingManagement.Blazor.Menus;
using Volo.Abp.Users;
using Volo.Abp.TenantManagement.Blazor.Navigation;
using Volo.Abp.Identity.Blazor;

namespace SST.DentistryFoundation.Blazor.Client.Navigation;

public class DentistryFoundationMenuContributor : IMenuContributor
{
    private readonly IConfiguration _configuration;

    public DentistryFoundationMenuContributor(IConfiguration configuration)
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
        var l = context.GetLocalizer<DentistryFoundationResource>();
        
        //Administration
        var administration = context.Menu.GetAdministration();
        administration.Order = 5;

        context.Menu.AddItem(new ApplicationMenuItem(
            DentistryFoundationMenus.Home,
            l["Menu:Home"],
            "/",
            icon: "fas fa-home",
            order: 1
        ));

        context.Menu.AddItem(new ApplicationMenuItem(
            DentistryFoundationMenus.FileManager,
            l["FileManager"],
            "/FileManager",
            icon: "fa-solid fa-file",
            order: 2
        ));

        context.Menu.AddItem(new ApplicationMenuItem(
            DentistryFoundationMenus.CvatWindow,
            l["CvatWindowIframe"],
            "/CvatWindowIframe",
            icon: "fa fa-external-link-square",
            order: 3
        ));

        context.Menu.AddItem(new ApplicationMenuItem(
            DentistryFoundationMenus.CvatWindow,
            l["CvatWindowJSInterop"],
            "/CvatWindowJSInterop",
            icon: "fa fa-external-link-square",
            order: 4
        ));
        if (MultiTenancyConsts.IsEnabled)
        {
            administration.SetSubItemOrder(TenantManagementMenuNames.GroupName, 1);
        }
        else
        {
            administration.TryRemoveMenuItem(TenantManagementMenuNames.GroupName);
        }

        administration.SetSubItemOrder(IdentityMenuNames.GroupName, 2);
        administration.SetSubItemOrder(SettingManagementMenus.GroupName, 3);
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

        await Task.CompletedTask;
    }
}
