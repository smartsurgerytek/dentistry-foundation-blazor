using Volo.Abp.Modularity;

namespace Foundation;

[DependsOn(
    typeof(FoundationApplicationModule),
    typeof(FoundationDomainTestModule)
)]
public class FoundationApplicationTestModule : AbpModule
{

}
