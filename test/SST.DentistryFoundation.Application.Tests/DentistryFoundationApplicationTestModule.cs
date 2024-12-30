using Volo.Abp.Modularity;

namespace SST.DentistryFoundation;

[DependsOn(
    typeof(DentistryFoundationApplicationModule),
    typeof(DentistryFoundationDomainTestModule)
)]
public class DentistryFoundationApplicationTestModule : AbpModule
{

}
