namespace DentistryFoundationSSO.Permissions;

public static class DentistryFoundationSSOPermissions
{
    public const string GroupName = "DentistryFoundationSSO";

    public static class Dashboard
    {
        public const string DashboardGroup = GroupName + ".Dashboard";
        public const string Host = DashboardGroup + ".Host";
        public const string Tenant = DashboardGroup + ".Tenant";
    }

    
    //Add your own permission names. Example:
    //public const string MyPermission1 = GroupName + ".MyPermission1";
}
