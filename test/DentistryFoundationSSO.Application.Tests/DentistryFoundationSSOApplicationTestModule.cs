using Volo.Abp.Modularity;

namespace DentistryFoundationSSO;

[DependsOn(
    typeof(DentistryFoundationSSOApplicationModule),
    typeof(DentistryFoundationSSODomainTestModule)
)]
public class DentistryFoundationSSOApplicationTestModule : AbpModule
{

}
