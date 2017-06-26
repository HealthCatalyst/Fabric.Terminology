namespace Fabric.Terminology.API
{
    using Fabric.Terminology.API.Configuration;
    using Fabric.Terminology.API.Logging;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Nancy.Owin;
    using Serilog;
    using Serilog.Core;

    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json");

            this.Configuration = builder.Build();

            var logger = LogFactory.CreateLogger(new LoggingLevelSwitch());
            Log.Logger = logger;
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IApplicationLifetime appLifetime)
        {
            var appConfig = new AppConfiguration();
            this.Configuration.Bind(appConfig);

            loggerFactory.AddSerilog();

            Log.Logger.Information("Fabric.Terminology.API starting.");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //// app.UseStaticFiles(); // <- requires Microsoft.AspNetCore.StaticFiles
            app.UseOwin()
                .UseNancy(opt => opt.Bootstrapper = new Bootstrapper(appConfig, Log.Logger));

            Log.Logger.Information("Fabric.Terminology.API started!");
        }
    }
}
