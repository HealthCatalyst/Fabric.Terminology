namespace Fabric.Terminology.IntegrationTests.Repositories
{
    using System;

    using Fabric.Terminology.Domain.Persistence;
    using Fabric.Terminology.IntegrationTests.Fixtures;
    using Fabric.Terminology.TestsBase;

    using FluentAssertions;

    using Xunit;
    using Xunit.Abstractions;

    public class SqlValueSetBackingItemRepositoryTests : OutputTestBase, IClassFixture<ValueSetBackingItemRepositoryFixture>
    {
        private readonly IValueSetBackingItemRepository repository;

        public SqlValueSetBackingItemRepositoryTests(ValueSetBackingItemRepositoryFixture fixture, ITestOutputHelper output)
            : base(output)
        {
            this.repository = fixture.ValueSetBackingItemRepository;
        }

        [Theory]
        [InlineData("35F6B1A6-A72B-48F5-B319-F6CCAF15734D")]
        public void CanGetValueSetBackingItem(string key)
        {
            // Arrange
            var valueSetGuid = Guid.Parse(key);

            // Act
            var backingItem = this.Profiler.ExecuteTimed(() => this.repository.GetValueSetBackingItem(valueSetGuid), $"Querying ValueSetGuid {valueSetGuid}").Single();

            // Assert
            backingItem.ValueSetGuid.Should().Be(valueSetGuid);
        }
    }
}
