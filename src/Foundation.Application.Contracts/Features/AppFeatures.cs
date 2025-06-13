using Volo.Abp.Features;

namespace Foundation.Application.Contracts.AppFeatures
{
    public class AppFeatures : FeatureDefinitionProvider
    {
        public override void Define(IFeatureDefinitionContext context)
        {
            var myGroup = context.AddGroup("Foundation");

            myGroup.AddFeature(
                "Foundation.IsPanoEnabled", 
                defaultValue: "false"
            );
        }
    }
}