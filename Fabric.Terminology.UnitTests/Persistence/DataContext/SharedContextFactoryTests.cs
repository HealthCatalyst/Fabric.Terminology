namespace Fabric.Terminology.UnitTests.Persistence.DataContext
{
    using Fabric.Terminology.SqlServer.Persistence.DataContext;
    using Fabric.Terminology.TestsBase.Fixtures;
    using FluentAssertions;
    using Xunit;

    public class SharedContextFactoryTests : IClassFixture<AppConfigurationFixture>
    {
        private readonly AppConfigurationFixture fixture;

        public SharedContextFactoryTests(AppConfigurationFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public void CanCreateInMemoryContext()
        {
            // Arrange
            var settings = this.fixture.AppConfiguration.TerminologySqlSettings;
            settings.UseInMemory = true;

            // Act
            var factory = new SharedContextFactory(settings, this.fixture.Logger);
            var context = factory.Create();

            // Assert
            context.Should().NotBeNull();
            context.IsInMemory.Should().BeTrue();
        }

        [Fact]
        public void CanCreateAttachedSharedContext()
        {
            // Arrange
            var settings = this.fixture.AppConfiguration.TerminologySqlSettings;
            settings.UseInMemory = false;

            // Act
            var factory = new SharedContextFactory(settings, this.fixture.Logger);
            var context = factory.Create();

            // Assert
            context.Should().NotBeNull();
            context.IsInMemory.Should().BeFalse();
        }
    }
}