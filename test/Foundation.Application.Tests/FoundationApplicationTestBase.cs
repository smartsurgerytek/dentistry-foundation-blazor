using Volo.Abp.Modularity;

namespace Foundation;

public abstract class FoundationApplicationTestBase<TStartupModule> : FoundationTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
