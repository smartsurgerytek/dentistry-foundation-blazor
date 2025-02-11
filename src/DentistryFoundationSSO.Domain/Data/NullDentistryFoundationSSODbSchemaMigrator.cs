using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace DentistryFoundationSSO.Data;

/* This is used if database provider does't define
 * IDentistryFoundationSSODbSchemaMigrator implementation.
 */
public class NullDentistryFoundationSSODbSchemaMigrator : IDentistryFoundationSSODbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}
