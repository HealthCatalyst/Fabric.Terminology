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
            var pagerSettings = new PagerSettings { CurrentPage = currentPage, ItemsPerPage = itemsPerPage };

            // Act
            var valueSets = this.Profiler.ExecuteTimed(
                () => this.valueSetRepository.GetValueSetsAsync(pagerSettings, new string[] { }));
            this.Output.WriteLine($"Total Values {valueSets.TotalItems}");
            this.Output.WriteLine($"Total Pages {valueSets.TotalPages}");

            // Assert
            valueSets.Should().NotBeNull();
            valueSets.TotalItems.Should().BeGreaterThan(0);
            valueSets.TotalPages.Should().BeGreaterThan(0);

            // Call again - to time cached
            var cached = this.Profiler.ExecuteTimed(
                () => this.valueSetRepository.GetValueSetsAsync(pagerSettings, new string[] { }),
                "Cached time: ");
        }

        [Theory]
        [InlineData("Cancer")]
        [InlineData("tumor")]
        [InlineData("hiv")]
        public void FindValueSetsAsyncReturnsResults(string nameFilter)
        {
            // Arrange
            var pagerSettings = new PagerSettings { CurrentPage = 1, ItemsPerPage = 20 };

            // Act
            var valueSets = this.Profiler.ExecuteTimed(
                () => this.valueSetRepository.FindValueSetsAsync(nameFilter, pagerSettings, new string[] { }));

            // Assert
            valueSets.Should().NotBeNull();
            valueSets.TotalItems.Should().BeGreaterThan(0);
            valueSets.TotalPages.Should().BeGreaterThan(0);
        }

        [Theory]
        [InlineData("2.16.840.1.113883.3.464.1003.108.12.1011", "2.16.840.1.113883.6.103-nope")]
        public void ValueSetWillReturnNullIfIdExistsButCodeSystemsDontMatch(string valueSetId, string codeSystemCd)
        {
            // Arrange
            var cds = new string[] { codeSystemCd };

            // Act
            var valueSet = this.valueSetRepository.GetValueSet(valueSetId, cds);

            // Assert
            valueSet.HasValue.Should().BeFalse();
        }

        [Fact]
        public void WillNotBeAnEmptyCollectionIfCodesDontMatchFilter()
        {
            // Arrange
            var pagerSettings = new PagerSettings { CurrentPage = 1, ItemsPerPage = 20 };
            var cds = new string[] { "2.16.840.1.113883.6.103-nope" };

            // Act
            var collection = this.Profiler.ExecuteTimed(
                () => this.valueSetRepository.GetValueSetsAsync(pagerSettings, cds));

            // Assert
            collection.TotalItems.Should().Be(0);

            var cached =
                this.Profiler.ExecuteTimed(() => this.valueSetRepository.GetValueSetsAsync(pagerSettings, cds));
        }

        [Fact]
        public void WillReturnACollectionIfCodesMatchFilter()
        {
            // Arrange
            var pagerSettings = new PagerSettings { CurrentPage = 1, ItemsPerPage = 20 };
            var cds = new string[] { "2.16.840.1.113883.6.103" };

            // Act
            var collection = this.Profiler.ExecuteTimed(
                () => this.valueSetRepository.GetValueSetsAsync(pagerSettings, cds));

            // Assert
            collection.TotalItems.Should().BeGreaterThan(0);

            var cached =
                this.Profiler.ExecuteTimed(() => this.valueSetRepository.GetValueSetsAsync(pagerSettings, cds));
        }
    }
}