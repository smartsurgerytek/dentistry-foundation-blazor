using SST.DentistryFoundation.Localization;
using Volo.Abp.Application.Services;

namespace SST.DentistryFoundation;

/* Inherit your application services from this class.
 */
public abstract class DentistryFoundationAppService : ApplicationService
{
    protected DentistryFoundationAppService()
    {
        LocalizationResource = typeof(DentistryFoundationResource);
    }
}
