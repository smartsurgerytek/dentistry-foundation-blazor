using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace SST.DentistryFoundation.Data;

/* This is used if database provider does't define
 * IDentistryFoundationDbSchemaMigrator implementation.
 */
public class NullDentistryFoundationDbSchemaMigrator : IDentistryFoundationDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}
