using DentistryFoundationSSO.Localization;
using Volo.Abp.Application.Services;

namespace DentistryFoundationSSO;

/* Inherit your application services from this class.
 */
public abstract class DentistryFoundationSSOAppService : ApplicationService
{
    protected DentistryFoundationSSOAppService()
    {
        LocalizationResource = typeof(DentistryFoundationSSOResource);
    }
}
