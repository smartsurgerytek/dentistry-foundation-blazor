using System.Threading.Tasks;

namespace SST.DentistryFoundation.Data;

public interface IDentistryFoundationDbSchemaMigrator
{
    Task MigrateAsync();
}
