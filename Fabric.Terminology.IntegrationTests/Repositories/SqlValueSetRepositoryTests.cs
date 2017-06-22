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

        public SqlValueSetRepositoryTests(ValueSetRepositoryFixture fixture, ITestOutputHelper output, ConfigTestFor testType = ConfigTestFor.Unit) : base(output, testType)
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
            //// Arrange
            var pagerSettings = new PagerSettings {CurrentPage = currentPage, ItemsPerPage = itemsPerPage};

            //// Act
            var valueSets = ExecuteTimed(async () => await _valueSetRepository.GetValueSetsAsync(pagerSettings)).Result;
            Output.WriteLine($"Total Items {valueSets.TotalItems}");
            Output.WriteLine($"Total Pages {valueSets.TotalPages}");
            //// Assert
            Assert.NotNull(valueSets);
        }
    }
}