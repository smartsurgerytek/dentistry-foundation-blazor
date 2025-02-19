using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Foundation.Data;

/* This is used if database provider does't define
 * IFoundationDbSchemaMigrator implementation.
 */
public class NullFoundationDbSchemaMigrator : IFoundationDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}
