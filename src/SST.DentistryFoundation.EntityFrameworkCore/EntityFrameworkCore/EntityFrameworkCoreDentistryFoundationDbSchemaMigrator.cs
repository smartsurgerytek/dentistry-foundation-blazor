using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SST.DentistryFoundation.Data;
using Volo.Abp.DependencyInjection;

namespace SST.DentistryFoundation.EntityFrameworkCore;

public class EntityFrameworkCoreDentistryFoundationDbSchemaMigrator
    : IDentistryFoundationDbSchemaMigrator, ITransientDependency
{
    private readonly IServiceProvider _serviceProvider;

    public EntityFrameworkCoreDentistryFoundationDbSchemaMigrator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task MigrateAsync()
    {
        /* We intentionally resolving the DentistryFoundationDbContext
         * from IServiceProvider (instead of directly injecting it)
         * to properly get the connection string of the current tenant in the
         * current scope.
         */

        await _serviceProvider
            .GetRequiredService<DentistryFoundationDbContext>()
            .Database
            .MigrateAsync();
    }
}
