using Microsoft.Extensions.Localization;
using DentistryFoundationSSO.Localization;
using Microsoft.Extensions.Localization;
using DentistryFoundationSSO.Localization;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace DentistryFoundationSSO.Blazor.Client;

[Dependency(ReplaceServices = true)]
public class DentistryFoundationSSOBrandingProvider : DefaultBrandingProvider
{
    private IStringLocalizer<DentistryFoundationSSOResource> _localizer;

    public DentistryFoundationSSOBrandingProvider(IStringLocalizer<DentistryFoundationSSOResource> localizer)
    {
        _localizer = localizer;
    }

    public override string AppName => _localizer["AppName"];
}
