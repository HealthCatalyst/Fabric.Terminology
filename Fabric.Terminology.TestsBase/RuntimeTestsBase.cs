using System.IO;
using Fabric.Terminology.API.Configuration;
using Microsoft.Extensions.Configuration;
using Xunit.Abstractions;

namespace Fabric.Terminology.TestsBase
{
    public abstract class RuntimeTestsBase : ProfiledTestsBase
    {
        protected RuntimeTestsBase(ITestOutputHelper output, ConfigTestFor testType = ConfigTestFor.Unit) 
            : base(output, testType)
        {
            var builder = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(TestHelper.GetAppConfigFile().FullName);

            Configuration = builder.Build();

            AppConfig = new AppConfiguration();
            Configuration.Bind(AppConfig);
        }


        protected AppConfiguration AppConfig { get; }

        protected IConfigurationRoot Configuration { get; }

    }
}