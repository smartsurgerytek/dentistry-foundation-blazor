using SST.DentistryFoundation.Samples;
using Xunit;

namespace SST.DentistryFoundation.EntityFrameworkCore.Applications;

[Collection(DentistryFoundationTestConsts.CollectionDefinitionName)]
public class EfCoreSampleAppServiceTests : SampleAppServiceTests<DentistryFoundationEntityFrameworkCoreTestModule>
{

}
