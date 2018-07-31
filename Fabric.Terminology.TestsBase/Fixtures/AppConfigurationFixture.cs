namespace Fabric.Terminology.TestsBase.Fixtures
{
    using System.IO;

    using Fabric.Terminology.API.Bootstrapping;
    using Fabric.Terminology.API.Configuration;

    using Microsoft.Extensions.Configuration;

    public class AppConfigurationFixture : TestFixtureBase
    {
        public AppConfigurationFixture()
        {
            AutoMapperConfig.Initialize();

            var builder = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(TerminologyTestHelper.GetAppConfigFile().FullName);

            this.Configuration = builder.Build();

            this.AppConfiguration = new AppConfiguration();
            this.Configuration.Bind(this.AppConfiguration);
        }

        public IAppConfiguration AppConfiguration { get; }

        protected IConfigurationRoot Configuration { get; }
    }
}