using Fabric.Terminology.API.Configuration;
using Fabric.Terminology.SqlServer.Persistence.DataContext;
using Fabric.Terminology.TestsBase;
using Xunit;
using Xunit.Abstractions;

namespace Fabric.Terminology.UnitTests.Persistence.DataContext
{
    public class SharedContextFactoryTests
    {
        private readonly AppConfiguration _appConfig;

        public SharedContextFactoryTests()
        {
            _appConfig = TestHelper.GetAppConfig();
        }
        
        //[Fact]
        //public void CanCreateInMemoryContext()
        //{
        //    //// Arrange
        //    var settings = _appConfig.TerminologySqlSettings;
        //    settings.UseInMemory = true;

        //    //// Act
        //    var factory = new SharedContextFactory(settings);
        //    var context = factory.Create();

        //    //// Assert
        //    Assert.NotNull(context);
        //    Assert.True(context.IsInMemory);
        //}

        //[Fact]
        //public void CanCreateAttachedSharedContext()
        //{
        //    //// Arrange
        //    var settings = _appConfig.TerminologySqlSettings;
        //    settings.UseInMemory = false;

        //    //// Act
        //    var factory = new SharedContextFactory(settings);
        //    var context = factory.Create();

        //    //// Assert
        //    Assert.NotNull(context);
        //    Assert.False(context.IsInMemory);
        //}
    }
}