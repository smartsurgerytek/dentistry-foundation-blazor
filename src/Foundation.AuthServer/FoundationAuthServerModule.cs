using System;
using System.IO;
using System.Linq;
using Localization.Resources.AbpUi;
using Medallion.Threading;
using Medallion.Threading.Redis;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Extensions.DependencyInjection;
using Volo.Abp.Caching.StackExchangeRedis;
using Volo.Abp.DistributedLocking;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Foundation.EntityFrameworkCore;
using Foundation.Localization;
using Foundation.MultiTenancy;
using OpenIddict.Server.AspNetCore;
using OpenIddict.Validation.AspNetCore;
using StackExchange.Redis;
using Volo.Abp;
using Volo.Abp.Studio;
using Volo.Abp.AspNetCore.Mvc.UI;
using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap;
using Volo.Abp.AspNetCore.Mvc.UI.Bundling;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.LeptonXLite;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.LeptonXLite.Bundling;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared;
using Volo.Abp.AspNetCore.Serilog;
using Volo.Abp.Auditing;
using Volo.Abp.Autofac;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Caching;
using Volo.Abp.Identity;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.UI.Navigation.Urls;
using Volo.Abp.UI;
using Volo.Abp.VirtualFileSystem;
using Volo.Abp.Account;
using Volo.Abp.Account.Web;
using Volo.Abp.Account.Public.Web;
using Volo.Abp.Account.Public.Web.ExternalProviders;
using Volo.Abp.Account.Public.Web.Impersonation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using Microsoft.AspNetCore.Authentication.Twitter;
using Volo.Saas.Host;
using Volo.Abp.OpenIddict;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Volo.Abp.Account.Localization;
using Volo.Abp.Security.Claims;
using Volo.Abp.Studio.Client.AspNetCore;
using Volo.Abp.AspNetCore.Security;
using Microsoft.AspNetCore.Http;
using Volo.Abp.AspNetCore.Mvc.Libs;

namespace Foundation;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(AbpStudioClientAspNetCoreModule),
    typeof(AbpCachingStackExchangeRedisModule),
    typeof(AbpDistributedLockingModule),
    typeof(SaasHostApplicationContractsModule),
    typeof(AbpAccountPublicWebOpenIddictModule),
    typeof(AbpAccountPublicHttpApiModule),
    typeof(AbpAccountPublicApplicationModule),
    typeof(AbpAccountPublicWebImpersonationModule),
    typeof(AbpAspNetCoreSerilogModule),
    typeof(AbpAspNetCoreMvcUiLeptonXLiteThemeModule),
    typeof(FoundationEntityFrameworkCoreModule)
    )]
public class FoundationAuthServerModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        var hostingEnvironment = context.Services.GetHostingEnvironment();
        var configuration = context.Services.GetConfiguration();

        PreConfigure<OpenIddictBuilder>(builder =>
        {
            builder.AddValidation(options =>
            {
                options.AddAudiences("Foundation");
                options.UseLocalServer();
                options.UseAspNetCore();
            });
        });

        if (!hostingEnvironment.IsDevelopment())
        {
            PreConfigure<AbpOpenIddictAspNetCoreOptions>(options =>
            {
                options.AddDevelopmentEncryptionAndSigningCertificate = false;
            });

            PreConfigure<OpenIddictServerBuilder>(serverBuilder =>
            {
                serverBuilder.AddProductionEncryptionAndSigningCertificate("openiddict.pfx", configuration["AuthServer:CertificatePassPhrase"]!);
                serverBuilder.SetIssuer(new Uri(configuration["AuthServer:Authority"]!));
            });
        }
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var hostingEnvironment = context.Services.GetHostingEnvironment();
        var configuration = context.Services.GetConfiguration();

        if (!configuration.GetValue<bool>("App:DisablePII"))
        {
            Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;
            Microsoft.IdentityModel.Logging.IdentityModelEventSource.LogCompleteSecurityArtifact = true;
        }

        if (!configuration.GetValue<bool>("AuthServer:RequireHttpsMetadata"))
        {
            Configure<OpenIddictServerAspNetCoreOptions>(options =>
            {
                options.DisableTransportSecurityRequirement = true;
            });
            
            Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedProto;
            });
        }

        context.Services.ForwardIdentityAuthenticationForBearer(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);

        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources
                .Get<FoundationResource>()
                .AddBaseTypes(
                    typeof(AbpUiResource),
                    typeof(AccountResource)
                );
        });

        Configure<AbpBundlingOptions>(options =>
        {
            options.StyleBundles.Configure(
                LeptonXLiteThemeBundles.Styles.Global,
                bundle =>
                {
                    bundle.AddFiles("/global-styles.css");
                }
            );
        });

        Configure<AbpAuditingOptions>(options =>
        {
            //options.IsEnabledForGetRequests = true;
            options.ApplicationName = "AuthServer";
        });

        if (hostingEnvironment.IsDevelopment())
        {
            Configure<AbpVirtualFileSystemOptions>(options =>
            {
                options.FileSets.ReplaceEmbeddedByPhysical<FoundationDomainSharedModule>(Path.Combine(hostingEnvironment.ContentRootPath, string.Format("..{0}Foundation.Domain.Shared", Path.DirectorySeparatorChar)));
                options.FileSets.ReplaceEmbeddedByPhysical<FoundationDomainModule>(Path.Combine(hostingEnvironment.ContentRootPath, string.Format("..{0}Foundation.Domain", Path.DirectorySeparatorChar)));
            });
        }

        Configure<AppUrlOptions>(options =>
        {
            options.Applications["MVC"].RootUrl = configuration["App:SelfUrl"];
            options.RedirectAllowedUrls.AddRange(configuration["App:RedirectAllowedUrls"]?.Split(',') ?? Array.Empty<string>());
        });

        Configure<AbpBackgroundJobOptions>(options =>
        {
            options.IsJobExecutionEnabled = false;
        });

        Configure<AbpDistributedCacheOptions>(options =>
        {
            options.KeyPrefix = "Foundation:";
        });

        if (!AbpStudioAnalyzeHelper.IsInAnalyzeMode)
        {
            var dataProtectionBuilder = context.Services.AddDataProtection().SetApplicationName("Foundation");
            if (!hostingEnvironment.IsDevelopment())
            {
                var redis = ConnectionMultiplexer.Connect(configuration["Redis:Configuration"]!);
                dataProtectionBuilder.PersistKeysToStackExchangeRedis(redis, "Foundation-Protection-Keys");
            }
        
            context.Services.AddSingleton<IDistributedLockProvider>(sp =>
            {
                var connection = ConnectionMultiplexer
                    .Connect(configuration["Redis:Configuration"]!);
                return new RedisDistributedSynchronizationProvider(connection.GetDatabase());
            });
        }

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
        
        context.Services.Configure<AbpClaimsPrincipalFactoryOptions>(options =>
        {
            options.IsDynamicClaimsEnabled = true;
        });
        
        context.Services.AddAuthentication()
        .AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
        {
            options.ClaimActions.MapJsonKey(AbpClaimTypes.Picture, "picture");
        })
        .WithDynamicOptions<GoogleOptions, GoogleHandler>(
            GoogleDefaults.AuthenticationScheme,
            options =>
            {
                options.WithProperty(x => x.ClientId);
                options.WithProperty(x => x.ClientSecret, isSecret: true);
            }
        )
        .AddMicrosoftAccount(MicrosoftAccountDefaults.AuthenticationScheme, options =>
        {
            //Personal Microsoft accounts as an example.
            options.AuthorizationEndpoint = "https://login.microsoftonline.com/consumers/oauth2/v2.0/authorize";
            options.TokenEndpoint = "https://login.microsoftonline.com/consumers/oauth2/v2.0/token";

            options.ClaimActions.MapCustomJson("picture", _ => "https://graph.microsoft.com/v1.0/me/photo/$value");
            options.SaveTokens = true;
        })
        .WithDynamicOptions<MicrosoftAccountOptions, MicrosoftAccountHandler>(
            MicrosoftAccountDefaults.AuthenticationScheme,
            options =>
            {
                options.WithProperty(x => x.ClientId);
                options.WithProperty(x => x.ClientSecret, isSecret: true);
            }
        )
        .AddTwitter(TwitterDefaults.AuthenticationScheme, options =>
        {
            options.ClaimActions.MapJsonKey(AbpClaimTypes.Picture, "profile_image_url_https");
            options.RetrieveUserDetails = true;
        })
        .WithDynamicOptions<TwitterOptions, TwitterHandler>(
            TwitterDefaults.AuthenticationScheme,
            options =>
            {
                options.WithProperty(x => x.ConsumerKey);
                options.WithProperty(x => x.ConsumerSecret, isSecret: true);
            }
        );
        
        context.Services.Configure<AbpAccountOptions>(options =>
        {
            options.TenantAdminUserName = "admin";
            options.ImpersonationTenantPermission = SaasHostPermissions.Tenants.Impersonation;
            options.ImpersonationUserPermission = IdentityPermissions.Users.Impersonation;
        });

        // Configure CORS for frontend applications
        // allowed origins are configured in appsettings.json
        // below code allows auth server rendering within iframes for mentioned domains
        context.Services.Configure<AbpSecurityHeadersOptions>(options =>
        {
            options.UseContentSecurityPolicyHeader = true; //false by default
            options.ContentSecurityPolicyValue = "frame-ancestors https://localhost:44355 http://localhost:3000"; //default value
        });

        Configure<AbpMvcLibsOptions>(options =>
        {
            options.CheckLibs = false;
        });
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        // cookie settings to allow cross-site cookie sharing
        // need this for react app to send identity cookie to auth server
        var cookiePolicyOptions = new CookiePolicyOptions
        {
            OnAppendCookie = cookieContext =>
            {
                cookieContext.CookieOptions.SameSite = SameSiteMode.None;
                cookieContext.CookieOptions.Secure = true;
            }
        };

        var app = context.GetApplicationBuilder();
        var env = context.GetEnvironment();

        app.UseForwardedHeaders();

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseAbpRequestLocalization();

        if (!env.IsDevelopment())
        {
            app.UseErrorPage();
        }
        app.UseCorrelationId();
        app.UseStaticFiles();
        app.UseAbpStudioLink();
        app.UseRouting();
        app.UseAbpSecurityHeaders();
        app.UseCors();
        // configuring cookie middleware with the cookie policy options
        app.UseCookiePolicy(cookiePolicyOptions);

        app.UseAuthentication();
        app.UseAbpOpenIddictValidation();

        if (MultiTenancyConsts.IsEnabled)
        {
            app.UseMultiTenancy();
        }

        app.UseUnitOfWork();
        app.UseDynamicClaims();
        app.UseAuthorization();

        app.UseAuditing();
        app.UseAbpSerilogEnrichers();
        app.UseConfiguredEndpoints();
    }
}
