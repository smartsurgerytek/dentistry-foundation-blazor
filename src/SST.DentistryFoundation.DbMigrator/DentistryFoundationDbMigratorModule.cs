using SST.DentistryFoundation.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace SST.DentistryFoundation.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(DentistryFoundationEntityFrameworkCoreModule),
    typeof(DentistryFoundationApplicationContractsModule)
)]
public class DentistryFoundationDbMigratorModule : AbpModule
{
}
