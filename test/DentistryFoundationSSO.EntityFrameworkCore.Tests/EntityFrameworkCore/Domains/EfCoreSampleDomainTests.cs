using DentistryFoundationSSO.Samples;
using Xunit;

namespace DentistryFoundationSSO.EntityFrameworkCore.Domains;

[Collection(DentistryFoundationSSOTestConsts.CollectionDefinitionName)]
public class EfCoreSampleDomainTests : SampleDomainTests<DentistryFoundationSSOEntityFrameworkCoreTestModule>
{

}
