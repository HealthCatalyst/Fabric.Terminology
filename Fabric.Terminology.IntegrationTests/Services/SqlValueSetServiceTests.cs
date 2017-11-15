namespace Fabric.Terminology.IntegrationTests.Services
{
    using System;
    using System.Collections.Generic;

    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.Domain.Services;
    using Fabric.Terminology.IntegrationTests.Fixtures;
    using Fabric.Terminology.TestsBase;

    using FluentAssertions;

    using JetBrains.Annotations;

    using Xunit;
    using Xunit.Abstractions;

    public class SqlValueSetServiceTests : OutputTestBase, IClassFixture<SqlServiceFixture>
    {
        private readonly IValueSetService valueSetService;

        private readonly IValueSetSummaryService valueSetSummaryService;

        public SqlValueSetServiceTests(SqlServiceFixture fixture, [NotNull] ITestOutputHelper output)
            : base(output)
        {
            this.valueSetService = fixture.ValueSetService;
            this.valueSetSummaryService = fixture.ValueSetSummaryService;
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
            var valueSet = this.Profiler.ExecuteTimed(() => this.valueSetService.GetValueSet(valueSetGuid), $"Querying ValueSet {valueSetGuid}").Single();
           //// var summary = this.Profiler.ExecuteTimed(() => this.valueSetSummaryService.GetValueSetSummary(valueSetGuid), $"Querying ValueSetSummary {valueSetGuid}").SingleOrDefault();

            this.Output.WriteLine(valueSet.Name);
            this.Output.WriteLine($"Code count: {valueSet.ValueSetCodes.Count}");

            // Assert
            valueSet.ValueSetGuid.Should().Be(valueSetGuid);
        }

        [Fact]
        public void CanGetMultipleValueSets()
        {
            // Arrange
            var valueSetGuids = new List<Guid>
            {
                Guid.Parse("35F6B1A6-A72B-48F5-B319-F6CCAF15734D"),
                Guid.Parse("A2216AAC-8513-43D8-85C2-00057F92394B"),
                Guid.Parse("31EA98DC-D050-47A2-9435-002C19CEBF8F")
            };

            // Act
            var valueSets = this.Profiler.ExecuteTimed(() => this.valueSetService.GetValueSetsListAsync(valueSetGuids));

            // Assert
            valueSets.Should().NotBeEmpty();
            valueSets.Count.Should().Be(valueSetGuids.Count);
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
            var summaryPage = this.Profiler.ExecuteTimed(async () => await this.valueSetSummaryService.GetValueSetSummariesAsync(pagerSettings));

            this.Output.WriteLine($"Total Values {page.TotalItems}");
            this.Output.WriteLine($"Total Pages {page.TotalPages}");

            // Assert
            page.TotalItems.Should().BeGreaterThan(0);
            page.TotalPages.Should().BeGreaterThan(0);
            page.Values.Count.Should().BeLessOrEqualTo(itemsPerPage);

            summaryPage.TotalItems.Should().BeGreaterThan(0);
            summaryPage.TotalPages.Should().BeGreaterThan(0);
            summaryPage.Values.Count.Should().BeLessOrEqualTo(itemsPerPage);
        }

        [Theory]
        [InlineData("2.16.840.1.113883.3.464.1003.109.12.1028")]
        [InlineData("2.16.840.1.113883.3.464.1003.111.12.1011")]
        public void CanGetValueSetVersions(string valueSetReferenceId)
        {
            // Arrange

            // Act
            var versions = this.Profiler.ExecuteTimed(async () => await this.valueSetService.GetValueSetVersionsAsync(valueSetReferenceId));

            // Assert
            versions.Should().NotBeEmpty();
        }

        [Theory]
        [InlineData("2.16.840.1.113883.3.464.1003.109.12.1028")]
        [InlineData("2.16.840.1.113883.3.464.1003.111.12.1011")]
        public void CanGetValueSetSummaryVersions(string valueSetReferenceId)
        {
            // Arrange

            // Act
            var versions = this.Profiler.ExecuteTimed(async () => await this.valueSetSummaryService.GetValueSetVersionsAsync(valueSetReferenceId));

            // Assert
            versions.Should().NotBeEmpty();
        }
    }
}