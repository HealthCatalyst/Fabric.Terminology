using Fabric.Terminology.API.Modules;
using Nancy;
using Xunit;

namespace Fabric.Terminology.UnitTests.Modules
{
    // TODO remove this testfixture - this is junk for test setup
    public class HomeModuleTests : ModuleTestsBase<HomeModule>
    {

        [Fact]
        public void ShouldReturnOkStatusForDefaultRoute()
        {
            // Arrange
            var browser = CreateBrowser();

            // Act
            var result = browser.Get("/", with => { with.HttpRequest(); }).Result;

            // Assert
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        }        
    }
}