namespace Fabric.Terminology.API
{
    using System.Linq;

    using AutoMapper;

    using Fabric.Terminology.API.Bootstrapping;
    using Fabric.Terminology.API.Bootstrapping.MapperProfiles;
    using Fabric.Terminology.API.Bootstrapping.Middleware;
    using Fabric.Terminology.API.Configuration;
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

        //// private readonly IContainer container;

        private readonly string[] allowedPaths =
        {
            "/",
            "/swagger/index.html",
            "/api-docs/swagger.json",
            "/api-docs"
        };

        public Startup(IConfiguration configuration)
        {
            this.appConfig = new TerminologyConfigurationProvider().GetAppConfiguration(configuration);

            this.logger = LogFactory.CreateTraceLogger(new LoggingLevelSwitch(), this.appConfig.ApplicationInsightsSettings);
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddWebEncoders();

            //// services.AddCors()

            // TODO should this be moved to the Nancy Bootstrapper?
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(
                    options =>
                        {
                            options.Authority = this.appConfig.IdentityServerSettings.Authority;
                            options.ApiName = this.appConfig.IdentityServerSettings.ClientId;
                            options.ApiSecret = this.appConfig.IdentityServerSettings.ClientSecret;
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
                            opt.Bootstrapper = new Bootstrapper(this.appConfig, this.logger);
                            opt.PassThroughWhenStatusCodesAre(Nancy.HttpStatusCode.Unauthorized);
                        });

            // After upgrading to .net 2.0 => 401 response isn't redirecting to IdServer
            app.Use((context, next) => context.ChallengeAsync());

            Log.Logger.Information("Fabric.Terminology.API started!");
        }
    }
}
