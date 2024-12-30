using SST.DentistryFoundation.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace SST.DentistryFoundation.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class DentistryFoundationController : AbpControllerBase
{
    protected DentistryFoundationController()
    {
        LocalizationResource = typeof(DentistryFoundationResource);
    }
}
