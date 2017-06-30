namespace Fabric.Terminology.UnitTests.Modules
{
    using System;
    using System.Security.Claims;

    using Castle.Components.DictionaryAdapter;

    using Fabric.Terminology.API.Configuration;
    using Fabric.Terminology.API.Modules;
    using Fabric.Terminology.Domain.Services;
    using Fabric.Terminology.TestsBase.Fixtures;

    using Moq;

    using Nancy.Testing;

    using Serilog;

    using Xunit;

    public class ValueSetModuleTests : ModuleTestsBase<ValueSetsModule>, IClassFixture<AppConfigurationFixture>
    {
        private readonly IAppConfiguration appConfig;
        private readonly ILogger logger;

        private readonly Mock<IValueSetService> mockValueSetService;

        public ValueSetModuleTests(AppConfigurationFixture fixture)
        {
            this.appConfig = fixture.AppConfiguration;
            this.logger = fixture.Logger;

            this.mockValueSetService = new Mock<IValueSetService>();
        }

        [Fact]
        public void GetValueSetShouldReturnOk()
        {
            // Arrange
            var browser = this.CreateBrowser();

            // Act
            throw new NotImplementedException();
        }

        protected override ConfigurableBootstrapper.ConfigurableBootstrapperConfigurator ConfigureBootstrapper(ConfigurableBootstrapper configurableBootstrapper, params Claim[] claims)
        {
            return base.ConfigureBootstrapper(configurableBootstrapper, claims);
        }
    }
}