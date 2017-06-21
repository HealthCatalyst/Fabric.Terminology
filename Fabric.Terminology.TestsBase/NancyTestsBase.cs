using Fabric.Terminology.API;
using Xunit.Abstractions;

namespace Fabric.Terminology.TestsBase
{
    public abstract class NancyTestsBase : RuntimeTestsBase
    {
        protected NancyTestsBase(ITestOutputHelper output, ConfigTestFor testType = ConfigTestFor.Unit) 
            : base(output, testType)
        {
            Bootstrapper = new Bootstrapper(AppConfig, Logger);
        }

        internal Bootstrapper Bootstrapper { get; }
    }
}