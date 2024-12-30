using Volo.Abp.Modularity;

namespace SST.DentistryFoundation;

/* Inherit from this class for your domain layer tests. */
public abstract class DentistryFoundationDomainTestBase<TStartupModule> : DentistryFoundationTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
