using SST.DentistryFoundation.Localization;
using Volo.Abp.AspNetCore.Components;

namespace SST.DentistryFoundation.Blazor.Client;

public abstract class DentistryFoundationComponentBase : AbpComponentBase
{
    protected DentistryFoundationComponentBase()
    {
        LocalizationResource = typeof(DentistryFoundationResource);
    }
}
