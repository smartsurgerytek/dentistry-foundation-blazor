using Volo.Abp.Modularity;

namespace Foundation;

/* Inherit from this class for your domain layer tests. */
public abstract class FoundationDomainTestBase<TStartupModule> : FoundationTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
