using System.Threading.Tasks;

namespace DentistryFoundationSSO.Data;

public interface IDentistryFoundationSSODbSchemaMigrator
{
    Task MigrateAsync();
}
