using Volo.Abp.Modularity;

namespace DentistryFoundationSSO;

[DependsOn(
    typeof(DentistryFoundationSSODomainModule),
    typeof(DentistryFoundationSSOTestBaseModule)
)]
public class DentistryFoundationSSODomainTestModule : AbpModule
{

}
