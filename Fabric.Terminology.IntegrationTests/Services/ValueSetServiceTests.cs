namespace Fabric.Terminology.IntegrationTests.Services
{
    using System;

    using Fabric.Terminology.API;
    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.Domain.Services;
    using Fabric.Terminology.IntegrationTests.Fixtures;
    using Fabric.Terminology.TestsBase;
    using Fabric.Terminology.TestsBase.Mocks;

    using FluentAssertions;

    using JetBrains.Annotations;

    using Xunit;
    using Xunit.Abstractions;

    public class ValueSetServiceTests : OutputTestBase, IClassFixture<ValueSetServiceFixture>
    {
        private readonly IValueSetService valueSetService;

        public ValueSetServiceTests(ValueSetServiceFixture fixture, [NotNull] ITestOutputHelper output)
            : base(output)
        {
            this.valueSetService = fixture.ValueSetService;
        }

        [Theory]
        [InlineData("35F6B1A6-A72B-48F5-B319-F6CCAF15734D")]
        [InlineData("A2216AAC-8513-43D8-85C2-00057F92394B")]
        [InlineData("31EA98DC-D050-47A2-9435-002C19CEBF8F")]
        public void CanGetValueSet(string key)
        {
            // Arrange
            var valueSetGuid = Guid.Parse(key);

            // Act
            var valueSet = this.Profiler.ExecuteTimed(() => this.valueSetService.GetValueSet(valueSetGuid), $"Querying ValueSetGuid {valueSetGuid}").Single();
            this.Output.WriteLine(valueSet.Name);
            this.Output.WriteLine($"Code count: {valueSet.ValueSetCodes.Count}");

            // Assert
            valueSet.ValueSetGuid.Should().Be(valueSetGuid);

        }

        [Theory]
        [InlineData(10, 1)]
        [InlineData(20, 2)]
        [InlineData(20, 3)]
        [InlineData(100, 1)]
        [InlineData(100, 2)]
        public void CanGetValueSetPages(int itemsPerPage, int pageNumber)
        {
            // Arrange
            var pagerSettings = new PagerSettings { CurrentPage = pageNumber, ItemsPerPage = itemsPerPage };

            // Act
            var page = this.Profiler.ExecuteTimed(async () => await this.valueSetService.GetValueSetsAsync(pagerSettings));
            this.Output.WriteLine($"Total Values {page.TotalItems}");
            this.Output.WriteLine($"Total Pages {page.TotalPages}");

            // Assert
            page.TotalItems.Should().BeGreaterThan(0);
            page.TotalPages.Should().BeGreaterThan(0);
            page.Values.Count.Should().BeLessOrEqualTo(itemsPerPage);

        }

        [Theory]
        [InlineData("Add VS 1", 5)]
        [InlineData("Add VS 2", 1000)]
        [InlineData("Add VS 4", 4000)]
        public void CanAddValueSet(string name, int codeCount)
        {
            // Arrange
            var apiModel = MockApiModelBuilder.ValueSetCreationApiModel(name, codeCount);
            var attempt = this.valueSetService.Create(apiModel);
            attempt.Success.Should().BeTrue();
            attempt.Result.HasValue.Should().BeTrue();

            var vs = attempt.Result.Single();

            // Act
            this.Profiler.ExecuteTimed(() => this.valueSetService.Save(vs));

            // Assert
            vs.ValueSetGuid.Should().NotBe(Guid.Empty);
            vs.ValueSetCodes.Count.Should().Be(codeCount);
            vs.Name.Should().BeEquivalentTo(name);

            // cleanup
            this.valueSetService.Delete(vs);
        }
    }
}