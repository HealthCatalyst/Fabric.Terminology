namespace Fabric.Terminology.IntegrationTests.Repositories
{
    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.Domain.Persistence;
    using Fabric.Terminology.IntegrationTests.Fixtures;
    using Fabric.Terminology.TestsBase;
    using FluentAssertions;
    using Xunit;
    using Xunit.Abstractions;

    public class SqlValueSetRepositoryTests : OutputTestBase, IClassFixture<ValueSetRepositoryFixture>
    {
        private readonly IValueSetRepository valueSetRepository;

        public SqlValueSetRepositoryTests(ValueSetRepositoryFixture fixture, ITestOutputHelper output)
            : base(output)
        {
            this.valueSetRepository = fixture.ValueSetRepository;
        }

        [Theory]
        [InlineData(1, 20)]
        [InlineData(2, 20)]
        [InlineData(3, 20)]
        [InlineData(4, 20)]
        [InlineData(5, 20)]
        public void GetValueSetsAsyncReturnsPageOfValueSets(int currentPage, int itemsPerPage)
        {
            // Arrange
            var pagerSettings = new PagerSettings {CurrentPage = currentPage, ItemsPerPage = itemsPerPage};

            // Act
            var valueSets = this.Profiler.ExecuteTimed(() => this.valueSetRepository.GetValueSetsAsync(pagerSettings));
            this.Output.WriteLine($"Total Items {valueSets.TotalItems}");
            this.Output.WriteLine($"Total Pages {valueSets.TotalPages}");

            // Assert
            valueSets.Should().NotBeNull();
            valueSets.TotalItems.Should().BeGreaterThan(0);
            valueSets.TotalPages.Should().BeGreaterThan(0);

            // Call again - to time cached
            var cached = this.Profiler.ExecuteTimed(() => this.valueSetRepository.GetValueSetsAsync(pagerSettings), "Cached time: ");
        }

        [Theory]
        [InlineData("Cancer")]
        [InlineData("tumor")]
        [InlineData("hiv")]
        public void FindValueSetsAsyncReturnsResults(string nameFilter)
        {
            // Arrange
            var pagerSettings = new PagerSettings {CurrentPage = 1, ItemsPerPage = 20};

            // Act
            var valueSets = this.Profiler.ExecuteTimed(() => this.valueSetRepository.FindValueSetsAsync(nameFilter, pagerSettings));

            // Assert
            valueSets.Should().NotBeNull();
            valueSets.TotalItems.Should().BeGreaterThan(0);
            valueSets.TotalPages.Should().BeGreaterThan(0);
        }
    }
}