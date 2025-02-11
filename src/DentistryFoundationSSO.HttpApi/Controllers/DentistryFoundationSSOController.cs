using DentistryFoundationSSO.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace DentistryFoundationSSO.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class DentistryFoundationSSOController : AbpControllerBase
{
    protected DentistryFoundationSSOController()
    {
        LocalizationResource = typeof(DentistryFoundationSSOResource);
    }
}
