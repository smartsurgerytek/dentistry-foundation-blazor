using Foundation.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace Foundation.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class FoundationController : AbpControllerBase
{
    protected FoundationController()
    {
        LocalizationResource = typeof(FoundationResource);
    }
}
