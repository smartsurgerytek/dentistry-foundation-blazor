using System.Threading.Tasks;

namespace Foundation.Data;

public interface IFoundationDbSchemaMigrator
{
    Task MigrateAsync();
}
