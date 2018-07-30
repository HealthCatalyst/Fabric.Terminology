namespace Fabric.Terminology.API
{
    using System;
    using System.Linq;
    using System.Net.Http;

    using AutoMapper;

    using Catalyst.DosApi.Common;
    using Catalyst.DosApi.Discovery;
    using Catalyst.Infrastructure.Caching;

    using Fabric.Terminology.API.Bootstrapping;
    using Fabric.Terminology.API.Bootstrapping.MapperProfiles;
    using Fabric.Terminology.API.Bootstrapping.Middleware;
    using Fabric.Terminology.API.Configuration;
    using Fabric.Terminology.API.Constants;
    using Fabric.Terminology.API.Logging;

    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    using Nancy.Owin;

    using Serilog;
    using Serilog.Core;
    using ILogger = Serilog.ILogger;

    public class Startup
    {
        private readonly IAppConfiguration appConfig;

        private readonly ILogger logger;

        private readonly IDiscoveryServiceClient discoveryClient;

        private readonly string[] allowedPaths =
        {
            string.Empty,
            "/",
            "/terminology",
            "/terminology/swagger/index",
            "/terminology/swagger/swagger.json",
            "/terminology/swagger/index.html",
            "/swagger/index",
            "/swagger/swagger.json",
            "/swagger/index.html"
        };

        public Startup(IConfiguration configuration)
        {
            this.appConfig = new TerminologyConfigurationProvider().GetAppConfiguration(configuration);

            this.logger = LogFactory.CreateTraceLogger(new LoggingLevelSwitch(), this.appConfig.ApplicationInsightsSettings);

            this.discoveryClient = this.GetDiscoveryServiceClientInstance(this.appConfig);
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddWebEncoders();

            var discoveryTask = this.discoveryClient.RequestServiceUriByKeyAsync(DiscoveryServicesKeys.Identity);
            discoveryTask.Wait();
            var authority = discoveryTask.Result;

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(o =>
                    {
                        o.Authority = authority.AbsoluteUri;
                    });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddSerilog(this.logger);

            Log.Logger.Information("Fabric.Terminology.API starting.");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            Log.Logger.Information("Initializing AutoMapper");

            Mapper.Initialize(cfg =>
                {
                    cfg.AddProfile<CodeSystemApiProfile>();
                    cfg.AddProfile<CodeSystemCodeApiProfile>();
                    cfg.AddProfile<ValueSetApiProfile>();
                });

            app.UseAuthentication();

            app.UseStaticFiles()
                .UseOwin()
                .UseAuthPlatform(this.appConfig.IdentityServerSettings.Scopes.ToArray(), this.allowedPaths)
                .UseNancy(
                    opt =>
                        {
                            opt.Bootstrapper = new Bootstrapper(this.appConfig, this.logger, this.discoveryClient);
                            opt.PassThroughWhenStatusCodesAre(Nancy.HttpStatusCode.Unauthorized);
                        });

            //// After upgrading to .net 2.0 => 401 response isn't redirecting to IdServer
            app.Use((context, next) => context.ChallengeAsync());

            Log.Logger.Information("Fabric.Terminology.API started!");
        }

        private IDiscoveryServiceClient GetDiscoveryServiceClientInstance(IAppConfiguration config)
        {
            // Setup discovery service client
            var staticCacheProvider = new StaticCacheProvider();
            var apiClient = new ApiClient(
                HttpClientFactory.Create(
                    new HttpClientHandler
                    {
                        UseDefaultCredentials = true
                    }));

            var client = new DiscoveryServiceClient(apiClient, staticCacheProvider, this.appConfig.DiscoveryServiceClientSettings);
            if (config.DiscoveryRegistrationSettings.RegisterHeatbeat)
            {
                var serviceUri =
                    new Uri(config.BaseTerminologyEndpoint, UriKind.Absolute).CombineUri(TerminologyVersion.Route);
                var settings = config.DiscoveryRegistrationSettings;
                var task = client.RegisterServiceAsync(
                    settings.ServiceName,
                    settings.Version,
                    serviceUri,
                    TerminologyVersion.SemanticVersion.ToString());

                task.Wait();
            }

            return client;
        }
    }
}
