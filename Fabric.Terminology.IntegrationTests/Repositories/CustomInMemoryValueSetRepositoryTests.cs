namespace Fabric.Terminology.IntegrationTests.Repositories
{
    using Fabric.Terminology.IntegrationTests.Fixtures;
    using Fabric.Terminology.SqlServer.Persistence;
    using Fabric.Terminology.TestsBase;

    using FluentAssertions;

    using Xunit;
    using Xunit.Abstractions;

    public class CustomInMemoryValueSetRepositoryTests : OutputTestBase, IClassFixture<CustomValueSetRepositoryFixture>
    {
        private readonly SqlValueSetRepository valueSetRepository;

        public CustomInMemoryValueSetRepositoryTests(ITestOutputHelper output, CustomValueSetRepositoryFixture fixture)
            : base(output)
        {
            this.valueSetRepository = fixture.ValueSetRepository;
        }

        [Fact]
        public void EnsureRepositorySeeded()
        {
            // Arrange
            // Handled in Class Fixture

            // Act
            var vs = this.valueSetRepository.GetCustomValueSet("test.valueset.1");

            // Assert
            vs.Should().NotBeNull();
        }
    }
}