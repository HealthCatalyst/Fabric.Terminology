using Fabric.Terminology.Domain.Models;
using Fabric.Terminology.Domain.Persistence;
using Fabric.Terminology.IntegrationTests.Fixtures;
using Fabric.Terminology.TestsBase;
using Xunit;
using Xunit.Abstractions;

namespace Fabric.Terminology.IntegrationTests.Repositories
{
    public class SqlValueSetRepositoryTests : ProfiledTestsBase, IClassFixture<ValueSetRepositoryFixture>
    {
        private readonly IValueSetRepository _valueSetRepository;

        public SqlValueSetRepositoryTests(ValueSetRepositoryFixture fixture, ITestOutputHelper output) 
            : base(output)
        {
            _valueSetRepository = fixture.ValueSetRepository;
        }

        [Theory]
        [InlineData(1, 20)]
        [InlineData(2, 20)]
        [InlineData(3, 20)]
        [InlineData(4, 20)]
        [InlineData(5, 20)]
        public void GetValueSetsAsync_ReturnsPageOfValueSets(int currentPage, int itemsPerPage)
        {
            // Arrange
            var pagerSettings = new PagerSettings {CurrentPage = currentPage, ItemsPerPage = itemsPerPage};

            // Act
            var valueSets = ExecuteTimedAysnc(() => _valueSetRepository.GetValueSetsAsync(pagerSettings));
            Output.WriteLine($"Total Items {valueSets.TotalItems}");
            Output.WriteLine($"Total Pages {valueSets.TotalPages}");
            
            // Assert
            Assert.NotNull(valueSets);
            Assert.True(valueSets.TotalPages > 0);
            Assert.True(valueSets.TotalItems > 0);

            // Call again - to time cached
            var cached = ExecuteTimedAysnc(() => _valueSetRepository.GetValueSetsAsync(pagerSettings), "Cached time: ");
        }

        [Theory]
        [InlineData("Cancer")]
        [InlineData("tumor")]
        [InlineData("hiv")]
        public void FindValueSetsAsync_ReturnsResults(string nameFilter)
        {
            // Arrange
            var pagerSettings = new PagerSettings {CurrentPage = 1, ItemsPerPage = 20};

            // Act
            var valueSets = ExecuteTimedAysnc(() => _valueSetRepository.FindValueSetsAsync(nameFilter, pagerSettings));

            // Assert
            Assert.NotNull(valueSets);
            Assert.True(valueSets.TotalPages > 0);
            Assert.True(valueSets.TotalItems > 0);
        }
    }
}