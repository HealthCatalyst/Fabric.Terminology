namespace Fabric.Terminology.API.Configuration
{
    using Microsoft.Extensions.Configuration;

    public class TerminologyConfigurationProvider
    {
        public IAppConfiguration GetAppConfiguration(IConfiguration configuration)
        {
            var appConfig = new AppConfiguration();
            configuration.Bind(appConfig);
            return appConfig;
        }
    }
}