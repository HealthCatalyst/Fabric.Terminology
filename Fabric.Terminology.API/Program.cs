namespace Fabric.Terminology.API
{
    using System.IO;
    using Microsoft.AspNetCore.Hosting;

    public static class Program
    {
        public static void Main()
        {
            var appConfig = new Configuration.TerminologyConfigurationProvider().GetAppConfiguration(Directory.GetCurrentDirectory());

            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIisIntegrationIfConfigured(appConfig) // this is required for the IIS Express button
                .UseStartup<Startup>()
                .UseApplicationInsights()
                .Build();

            host.Run();
        }
    }
}
