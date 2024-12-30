using Microsoft.Extensions.Localization;
using SST.DentistryFoundation.Localization;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace SST.DentistryFoundation;

[Dependency(ReplaceServices = true)]
public class DentistryFoundationBrandingProvider : DefaultBrandingProvider
{
    private IStringLocalizer<DentistryFoundationResource> _localizer;

    public DentistryFoundationBrandingProvider(IStringLocalizer<DentistryFoundationResource> localizer)
    {
        _localizer = localizer;
    }

    public override string AppName => _localizer["AppName"];
}
