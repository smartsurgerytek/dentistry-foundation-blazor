using DentistryFoundationSSO.Localization;
using Volo.Abp.AspNetCore.Components;

namespace DentistryFoundationSSO.Blazor.Client;

public abstract class DentistryFoundationSSOComponentBase : AbpComponentBase
{
    protected DentistryFoundationSSOComponentBase()
    {
        LocalizationResource = typeof(DentistryFoundationSSOResource);
    }
}
