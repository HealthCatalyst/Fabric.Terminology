namespace Fabric.Terminology.API
{
    using AutoMapper;

    using Fabric.Terminology.API.Configuration;
    using Fabric.Terminology.API.Logging;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Nancy.Owin;
    using Serilog;
    using Serilog.Core;

    public class Startup
    {
        private readonly IAppConfiguration appConfig;

        public Startup(IConfiguration configuration)
        {
            this.appConfig = new TerminologyConfigurationProvider().GetAppConfiguration(configuration);

            var logger = LogFactory.CreateLogger(new LoggingLevelSwitch());
            Log.Logger = logger;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddSerilog();

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

            app.UseStaticFiles()
                .UseOwin()
                .UseNancy(opt => opt.Bootstrapper = new Bootstrapper(this.appConfig, Log.Logger));

            Log.Logger.Information("Fabric.Terminology.API started!");
        }
    }
}
