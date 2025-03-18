using Volo.Abp.Modularity;

namespace Foundation;

[DependsOn(
    typeof(FoundationDomainModule),
    typeof(FoundationTestBaseModule)
)]
public class FoundationDomainTestModule : AbpModule
{

}
