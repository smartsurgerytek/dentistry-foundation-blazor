using SST.DentistryFoundation.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;

namespace SST.DentistryFoundation.Permissions;

public class DentistryFoundationPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(DentistryFoundationPermissions.GroupName);

        //Define your own permissions here. Example:
        //myGroup.AddPermission(DentistryFoundationPermissions.MyPermission1, L("Permission:MyPermission1"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<DentistryFoundationResource>(name);
    }
}
