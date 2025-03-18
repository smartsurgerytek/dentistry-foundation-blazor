using Xunit;

namespace Foundation.EntityFrameworkCore;

[CollectionDefinition(FoundationTestConsts.CollectionDefinitionName)]
public class FoundationEntityFrameworkCoreCollection : ICollectionFixture<FoundationEntityFrameworkCoreFixture>
{

}
