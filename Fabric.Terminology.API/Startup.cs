namespace Fabric.Terminology.API
{
    using System.Linq;
    using System.Text;

    using AutoMapper;

    using Fabric.Terminology.API.Bootstrapping;
    using Fabric.Terminology.API.Bootstrapping.MapperProfiles;
    using Fabric.Terminology.API.Bootstrapping.Middleware;
    using Fabric.Terminology.API.Configuration;
    using Fabric.Terminology.API.Logging;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.IdentityModel.Tokens;

    using Nancy.Owin;
    using Serilog;
    using Serilog.Core;
    using ILogger = Serilog.ILogger;

    public class Startup
    {
        private readonly IAppConfiguration appConfig;

        private readonly ILogger logger;

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
            services.AddAuthentication()
                .AddJwtBearer(
                    cfg =>
                        {
                            cfg.RequireHttpsMetadata = false;
                            cfg.SaveToken = true;
                            cfg.TokenValidationParameters = new TokenValidationParameters()
                            {
                                ValidIssuer = this.appConfig.IdentityServerSettings.Authority,
                                ValidAudience = this.appConfig.IdentityServerSettings.Authority,
                                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.appConfig.IdentityServerSettings.ClientId))
                            };
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

            ////app.UseIdentityServerAuthentication(new IdentityServerAuthenticationOptions
            ////{
            ////    Authority = this.appConfig.IdentityServerSettings.Authority,
            ////    RequireHttpsMetadata = false,
            ////    ApiName = this.appConfig.IdentityServerSettings.ClientId
            ////});

            app.UseStaticFiles()
                .UseOwin()
                .UseAuthPlatform(this.appConfig.IdentityServerSettings.Scopes.ToArray(), this.allowedPaths)
                .UseNancy(opt => opt.Bootstrapper = new Bootstrapper(this.appConfig, this.logger));

            Log.Logger.Information("Fabric.Terminology.API started!");
        }
    }
}
