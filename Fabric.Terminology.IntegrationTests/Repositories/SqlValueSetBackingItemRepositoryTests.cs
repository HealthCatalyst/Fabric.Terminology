namespace Fabric.Terminology.IntegrationTests.Repositories
{
    using System;
    using System.Collections.Generic;

    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.Domain.Persistence;
    using Fabric.Terminology.IntegrationTests.Fixtures;
    using Fabric.Terminology.SqlServer.Persistence;
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

        [Theory]
        [InlineData(10, 1)]
        [InlineData(20, 2)]
        [InlineData(20, 3)]
        [InlineData(100, 1)]
        [InlineData(100, 2)]
        public void CanGetAPageOfValueSetBackingItem(int itemsPerPage, int pageNumber)
        {
            // Arrange
            var pagerSettings = new PagerSettings { CurrentPage = pageNumber, ItemsPerPage = itemsPerPage };

            // Act
            var page = this.Profiler.ExecuteTimed(async () => await this.repository.GetValueSetBackingItemsAsync(pagerSettings, new List<Guid>()));
            this.Output.WriteLine($"Total Values {page.TotalItems}");
            this.Output.WriteLine($"Total Pages {page.TotalPages}");

            // Assert
            page.TotalItems.Should().BeGreaterThan(0);
            page.TotalPages.Should().BeGreaterThan(0);
            page.Values.Count.Should().BeLessOrEqualTo(itemsPerPage);
        }
    }
}
