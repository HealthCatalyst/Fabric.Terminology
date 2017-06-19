using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Fabric.Terminology.API;
using Fabric.Terminology.API.Modules;
using Fabric.Terminology.UnitTests.TestSetup;
using Nancy;
using Nancy.Testing;
using Xunit;
using Xunit.Abstractions;

namespace Fabric.Terminology.UnitTests.Modules
{
    // TODO remove this testfixture - this is junk for test setup
    public class HomeModuleTests : ModuleTestsBase<HomeModule>
    {

        [Fact]
        public void ShouldReturnOkStatusForDefaultRoute()
        {
            //// Arrange
            var browser = CreateBrowser();

            //// Act
            var result = browser.Get("/", with => { with.HttpRequest(); }).Result;

            //// Assert
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        }        
    }
}