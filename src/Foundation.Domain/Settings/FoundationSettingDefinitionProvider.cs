using Volo.Abp.Settings;

namespace Foundation.Settings;

public class FoundationSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        //Define your own settings here. Example:
        //context.Add(new SettingDefinition(FoundationSettings.MySetting1));
    }
}
