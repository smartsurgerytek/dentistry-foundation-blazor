using Volo.Abp.Modularity;

namespace DentistryFoundationSSO;

/* Inherit from this class for your domain layer tests. */
public abstract class DentistryFoundationSSODomainTestBase<TStartupModule> : DentistryFoundationSSOTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
