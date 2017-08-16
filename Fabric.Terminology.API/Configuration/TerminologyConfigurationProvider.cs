namespace Fabric.Terminology.API.Configuration
{
    using Microsoft.Extensions.Configuration;

    public class TerminologyConfigurationProvider
    {
        public IAppConfiguration GetAppConfiguration(string basePath)
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .SetBasePath(basePath)
                .Build();

            var appConfig = new AppConfiguration();
            config.Bind(appConfig);
            return appConfig;
        }
    }
}