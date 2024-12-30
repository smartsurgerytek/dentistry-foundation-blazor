using Xunit;

namespace SST.DentistryFoundation.EntityFrameworkCore;

[CollectionDefinition(DentistryFoundationTestConsts.CollectionDefinitionName)]
public class DentistryFoundationEntityFrameworkCoreCollection : ICollectionFixture<DentistryFoundationEntityFrameworkCoreFixture>
{

}
