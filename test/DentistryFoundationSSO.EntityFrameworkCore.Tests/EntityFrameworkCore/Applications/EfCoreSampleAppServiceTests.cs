using DentistryFoundationSSO.Samples;
using Xunit;

namespace DentistryFoundationSSO.EntityFrameworkCore.Applications;

[Collection(DentistryFoundationSSOTestConsts.CollectionDefinitionName)]
public class EfCoreSampleAppServiceTests : SampleAppServiceTests<DentistryFoundationSSOEntityFrameworkCoreTestModule>
{

}
