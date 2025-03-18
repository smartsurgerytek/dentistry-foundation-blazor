using Foundation.Localization;
using Volo.Abp.AspNetCore.Components;

namespace Foundation.Blazor.Client;

public abstract class FoundationComponentBase : AbpComponentBase
{
    protected FoundationComponentBase()
    {
        LocalizationResource = typeof(FoundationResource);
    }
}
