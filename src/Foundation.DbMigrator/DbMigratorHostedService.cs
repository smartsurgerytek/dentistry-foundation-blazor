using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Foundation.Data;
using Serilog;
using Volo.Abp;
using Volo.Abp.Data;
using System;

namespace Foundation.DbMigrator;

public class DbMigratorHostedService : IHostedService
{
    private readonly IHostApplicationLifetime _hostApplicationLifetime;
    private readonly IConfiguration _configuration;

    public DbMigratorHostedService(IHostApplicationLifetime hostApplicationLifetime, IConfiguration configuration)
    {
        _hostApplicationLifetime = hostApplicationLifetime;
        _configuration = configuration;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using (var application = await AbpApplicationFactory.CreateAsync<FoundationDbMigratorModule>(options =>
        {
            // options.Services.ReplaceConfiguration(_configuration);
            options.Services.ReplaceConfiguration(BuildConfiguration());
            options.UseAutofac();
            options.Services.AddLogging(c => c.AddSerilog());
            options.AddDataMigrationEnvironment();
        }))
        {
            await application.InitializeAsync();

            await application
                .ServiceProvider
                .GetRequiredService<FoundationDbMigrationService>()
                .MigrateAsync();

            await application.ShutdownAsync();

            _hostApplicationLifetime.StopApplication();
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private static IConfiguration BuildConfiguration()
    {
        var configurationBuilder = new ConfigurationBuilder();
        
        configurationBuilder.AddJsonFile("appsettings.secrets.json", true);

        var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        if (environmentName != null)
        {
            configurationBuilder.AddJsonFile($"appsettings.{environmentName}.json", true);
        }
        else
        {
            configurationBuilder.AddJsonFile("appsettings.json");
        }
        
        return configurationBuilder
            .AddEnvironmentVariables()
            .Build(); ;
    }
}
