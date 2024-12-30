using Volo.Abp.Modularity;

namespace SST.DentistryFoundation;

public abstract class DentistryFoundationApplicationTestBase<TStartupModule> : DentistryFoundationTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
