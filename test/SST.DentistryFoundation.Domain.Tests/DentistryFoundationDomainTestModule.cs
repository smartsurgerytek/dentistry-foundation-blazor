using Volo.Abp.Modularity;

namespace SST.DentistryFoundation;

[DependsOn(
    typeof(DentistryFoundationDomainModule),
    typeof(DentistryFoundationTestBaseModule)
)]
public class DentistryFoundationDomainTestModule : AbpModule
{

}
