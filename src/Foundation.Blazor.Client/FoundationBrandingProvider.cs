using Microsoft.Extensions.Localization;
using Foundation.Localization;
using Microsoft.Extensions.Localization;
using Foundation.Localization;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace Foundation.Blazor.Client;

[Dependency(ReplaceServices = true)]
public class FoundationBrandingProvider : DefaultBrandingProvider
{
    private IStringLocalizer<FoundationResource> _localizer;

    public FoundationBrandingProvider(IStringLocalizer<FoundationResource> localizer)
    {
        _localizer = localizer;
    }

    public override string AppName => _localizer["AppName"];
}
