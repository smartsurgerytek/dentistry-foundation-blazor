using SST.DentistryFoundation.Samples;
using Xunit;

namespace SST.DentistryFoundation.EntityFrameworkCore.Domains;

[Collection(DentistryFoundationTestConsts.CollectionDefinitionName)]
public class EfCoreSampleDomainTests : SampleDomainTests<DentistryFoundationEntityFrameworkCoreTestModule>
{

}
