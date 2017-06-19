using System;
using System.IO;
using Fabric.Terminology.UnitTests.TestSetup;
using Xunit;
using Xunit.Abstractions;

namespace Fabric.Terminology.UnitTests
{
    public class ScrapTests : NancyTestsBase
    {
        public ScrapTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void Playground()
        {
            Output.WriteLine(AppConfig.TerminologySqlSettings.UseInMemory.ToString());
        }

       
    }
}