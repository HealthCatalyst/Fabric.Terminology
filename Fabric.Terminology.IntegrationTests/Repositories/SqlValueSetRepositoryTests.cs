namespace Fabric.Terminology.IntegrationTests.Repositories
{
    using Fabric.Terminology.Domain.Persistence;
    using Fabric.Terminology.IntegrationTests.Fixtures;
    using Fabric.Terminology.TestsBase;

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

        //[Theory]
        //[InlineData(1, 5)]
        //[InlineData(2, 5)]
        //[InlineData(3, 5)]
        //[InlineData(4, 5)]
        //[InlineData(5, 5)]
        //public void GetValueSetsAsyncReturnsPageOfValueSets(int currentPage, int itemsPerPage)
        //{
        //    // Arrange
        //    var pagerSettings = new PagerSettings { CurrentPage = currentPage, ItemsPerPage = itemsPerPage };

        //    // Act
        //    var valueSets = this.Profiler.ExecuteTimed(
        //        () => this.valueSetRepository.GetValueSetsAsync(pagerSettings, new string[] { }));
        //    this.Output.WriteLine($"Total Values {valueSets.TotalItems}");
        //    this.Output.WriteLine($"Total Pages {valueSets.TotalPages}");

        //    // Assert
        //    this.Output.WriteLine(string.Join(",", valueSets.Values.Select(vs => vs.ValueSetGuid)));
        //    valueSets.Should().NotBeNull();
        //    valueSets.TotalItems.Should().BeGreaterThan(0);
        //    valueSets.TotalPages.Should().BeGreaterThan(0);

        //    valueSets.Values.All(vs => vs.ValueSetCodes.Any()).Should().BeTrue("value sets must have codes");

        //    // Call again - to time cached
        //    var cached = this.Profiler.ExecuteTimed(
        //        () => this.valueSetRepository.GetValueSetsAsync(pagerSettings, new string[] { }),
        //        "Cached time: ");
        //}

        //[Theory]
        //[InlineData("Cancer")]
        //[InlineData("tumor")]
        //[InlineData("hiv")]
        //public void FindValueSetsAsyncReturnsResults(string nameFilter)
        //{
        //    // Arrange
        //    var pagerSettings = new PagerSettings { CurrentPage = 1, ItemsPerPage = 20 };

        //    // Act
        //    var valueSets = this.Profiler.ExecuteTimed(
        //        () => this.valueSetRepository.GetValueSetsAsync(nameFilter, pagerSettings, new string[] { }));

        //    // Assert
        //    valueSets.Should().NotBeNull();
        //    valueSets.TotalItems.Should().BeGreaterThan(0);
        //    valueSets.TotalPages.Should().BeGreaterThan(0);
        //    valueSets.Values.All(vs => vs.ValueSetCodes.Any()).Should().BeTrue("value sets must have codes");
        //}

        //[Theory]
        //[InlineData("413B430E-42EC-41B2-939A-4A4E757124DD", "2.16.840.1.113883.6.103-nope")]
        //public void ValueSetWillReturnNullIfIdExistsButCodeSystemsDontMatch(string valueSetUniqueId, string codeSystemCd)
        //{
        //    // Arrange
        //    var cds = new string[] { codeSystemCd };

        //    // Act
        //    var valueSet = this.valueSetRepository.GetValueSet(new Guid(valueSetUniqueId), cds);

        //    // Assert
        //    valueSet.HasValue.Should().BeFalse();
        //}

        //[Fact]
        //public void WillNotBeAnEmptyCollectionIfCodesDontMatchFilter()
        //{
        //    // Arrange
        //    var pagerSettings = new PagerSettings { CurrentPage = 1, ItemsPerPage = 20 };
        //    var cds = new string[] { "2.16.840.1.113883.6.103-nope" };

        //    // Act
        //    var collection = this.Profiler.ExecuteTimed(
        //        () => this.valueSetRepository.GetValueSetsAsync(pagerSettings, cds));

        //    // Assert
        //    collection.TotalItems.Should().Be(0);

        //    var cached =
        //        this.Profiler.ExecuteTimed(() => this.valueSetRepository.GetValueSetsAsync(pagerSettings, cds));
        //}

        //[Fact]
        //public void WillReturnACollectionIfCodesMatchFilter()
        //{
        //    // Arrange
        //    var pagerSettings = new PagerSettings { CurrentPage = 1, ItemsPerPage = 20 };
        //    var cds = new string[] { "2.16.840.1.113883.6.103" };

        //    // Act
        //    var collection = this.Profiler.ExecuteTimed(
        //        () => this.valueSetRepository.GetValueSetsAsync(pagerSettings, cds));

        //    // Assert
        //    collection.TotalItems.Should().BeGreaterThan(0);

        //    var cached =
        //        this.Profiler.ExecuteTimed(() => this.valueSetRepository.GetValueSetsAsync(pagerSettings, cds));
        //}
    }
}