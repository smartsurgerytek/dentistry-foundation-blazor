using Volo.Abp.PermissionManagement;
using Volo.Abp.SettingManagement;
using Volo.Abp.Account;
using Volo.Abp.Identity;
using Volo.Abp.AutoMapper;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Modularity;
using Volo.Abp.AuditLogging;
using Volo.Abp.Gdpr;
using Volo.Abp.LanguageManagement;
using Volo.FileManagement;
using Volo.Abp.OpenIddict;
using Volo.Abp.TextTemplateManagement;
using Volo.Saas.Host;
using Volo.Chat;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using System.IO;
using System;
using Polly;
using Foundation.Services;
using Polly.Extensions.Http;
using Microsoft.AspNetCore.Cors;
using System.Linq;

namespace Foundation;

[DependsOn(
    typeof(FoundationDomainModule),
    typeof(FoundationApplicationContractsModule),
    typeof(AbpPermissionManagementApplicationModule),
    typeof(AbpFeatureManagementApplicationModule),
    typeof(AbpIdentityApplicationModule),
    typeof(AbpAccountPublicApplicationModule),
    typeof(AbpAccountAdminApplicationModule),
    typeof(SaasHostApplicationModule),
    typeof(ChatApplicationModule),
    typeof(AbpAuditLoggingApplicationModule),
    typeof(TextTemplateManagementApplicationModule),
    typeof(AbpOpenIddictProApplicationModule),
    typeof(LanguageManagementApplicationModule),
    typeof(FileManagementApplicationModule),
    typeof(AbpGdprApplicationModule),
    typeof(AbpSettingManagementApplicationModule)
    )]
public class FoundationApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<FoundationApplicationModule>();
        });

        AsyncPolicy circuitBreakerPolicy = Policy.Handle<HttpRequestException>()
                                                    .CircuitBreakerAsync(
                                                        exceptionsAllowedBeforeBreaking: 2,
                                                        durationOfBreak: TimeSpan.FromSeconds(3),
                                                        onBreak: (exception, timespan) =>
                                                        {
                                                            Console.WriteLine($"Circuit broken due to: {exception.Message}");
                                                        },
                                                        onReset: () => Console.WriteLine("Circuit closed."),
                                                        onHalfOpen: () => Console.WriteLine("Circuit in half-open state.")
                                                    );

        context.Services.AddHttpClient<DentistryApiAppService>(client =>
        {
            var configuration = context.Services.GetConfiguration();
            var apiUrl = configuration["ApiUrl"];
            var apiKey = configuration["ApiKey"];

            client.BaseAddress = new Uri(apiUrl);
        })
        .AddPolicyHandler(GetRetryPolicy())
        .AddPolicyHandler(GetCircuitBreakerPolicy());


        context.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder =>
            {
                builder
                    .WithOrigins(
                        configuration["App:CorsOrigins"]?
                            .Split(",", StringSplitOptions.RemoveEmptyEntries)
                            .Select(o => o.Trim().RemovePostFix("/"))
                            .ToArray() ?? Array.Empty<string>()
                    )
                    .WithAbpExposedHeaders()
                    .SetIsOriginAllowedToAllowWildcardSubdomains()
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });
    }

    private IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
    {
        return HttpPolicyExtensions.HandleTransientHttpError().CircuitBreakerAsync(3, TimeSpan.FromSeconds(30));
    }

    private IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return Policy<HttpResponseMessage>.Handle<HttpRequestException>()
            .RetryAsync(3,
                onRetry: (result, retryCount) =>
                {
                    Console.WriteLine($"Retry {retryCount} due to: {result.Exception?.Message}");
                });
    }
}
