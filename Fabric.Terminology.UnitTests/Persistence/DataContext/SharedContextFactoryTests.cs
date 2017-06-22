using Fabric.Terminology.API.Configuration;
using Fabric.Terminology.SqlServer.Persistence.DataContext;
using Fabric.Terminology.TestsBase;
using Fabric.Terminology.TestsBase.Fixtures;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace Fabric.Terminology.UnitTests.Persistence.DataContext
{
    public class SharedContextFactoryTests : IClassFixture<AppConfigurationFixture>
    {
        private readonly AppConfigurationFixture _fixture;

        public SharedContextFactoryTests(AppConfigurationFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void CanCreateInMemoryContext()
        {
            // Arrange
            var settings = _fixture.AppConfiguration.TerminologySqlSettings;
            settings.UseInMemory = true;

            // Act
            var factory = new SharedContextFactory(settings, _fixture.Logger);
            var context = factory.Create();

            // Assert
            context.Should().NotBeNull();
            context.IsInMemory.Should().BeTrue();
        }

        [Fact]
        public void CanCreateAttachedSharedContext()
        {
            // Arrange
            var settings = _fixture.AppConfiguration.TerminologySqlSettings;
            settings.UseInMemory = false;

            // Act
            var factory = new SharedContextFactory(settings, _fixture.Logger);
            var context = factory.Create();

            // Assert
            context.Should().NotBeNull();
            context.IsInMemory.Should().BeFalse();
        }
    }
}