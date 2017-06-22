using System.IO;
using Fabric.Terminology.API.Configuration;
using Microsoft.Extensions.Configuration;

namespace Fabric.Terminology.TestsBase.Fixtures
{
    public class AppConfigurationFixture : TestFixtureBase
    {
        public AppConfigurationFixture()
        {
            var builder = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(TestHelper.GetAppConfigFile().FullName);

            Configuration = builder.Build();

            AppConfiguration = new AppConfiguration();
            Configuration.Bind(AppConfiguration);
        }

        public IAppConfiguration AppConfiguration { get; }

        protected IConfigurationRoot Configuration { get; }
    }
}