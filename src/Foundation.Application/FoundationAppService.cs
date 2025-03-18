using Foundation.Localization;
using Volo.Abp.Application.Services;

namespace Foundation;

/* Inherit your application services from this class.
 */
public abstract class FoundationAppService : ApplicationService
{
    protected FoundationAppService()
    {
        LocalizationResource = typeof(FoundationResource);
    }
}
