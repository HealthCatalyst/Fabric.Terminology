namespace Fabric.Terminology.IntegrationTests.Services
{
    using System;
    using System.Collections.Generic;

    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.Domain.Services;
    using Fabric.Terminology.IntegrationTests.Fixtures;
    using Fabric.Terminology.TestsBase;

    using FluentAssertions;

    using Nest;

    using Xunit;
    using Xunit.Abstractions;

    public class ElasticValueSetServiceTests : OutputTestBase, IClassFixture<ElasticServiceFixture>
    {
        private readonly ElasticClient client;

        private readonly IValueSetService valueSetService;

        private readonly IValueSetSummaryService valueSetSummaryService;

        public ElasticValueSetServiceTests(ElasticServiceFixture fixture, ITestOutputHelper output)
            : base(output)
        {
            this.client = fixture.ElasticClient;
            this.valueSetService = fixture.ValueSetService;
            this.valueSetSummaryService = fixture.ValueSetSummaryService;
        }

        [Theory]
        [InlineData("b209106e-2dff-4e85-ae71-c2fdec3385f0")]
        [InlineData("601ab4c1-c6e7-42b6-b055-4db2a8227914")]
        [InlineData("d7ba46ff-31d2-4135-b5a3-758734c78971")]
        [InlineData("35f6b1a6-a72b-48f5-b319-f6ccaf15734d")]
        public void CanGetValueSetByGuid(string valueSetUniqueId)
        {
            // Arrange
            var valueSetGuid = Guid.Parse(valueSetUniqueId);

            // Act
            var valueSet = this.Profiler.ExecuteTimed(() => this.valueSetService.GetValueSet(valueSetGuid)).Single();
            this.Output.WriteLine(valueSet.Name);
            this.Output.WriteLine($"Code count: {valueSet.ValueSetCodes.Count}");

            // Assert
            valueSet.ValueSetGuid.Should().Be(valueSetGuid);
        }

        [Theory]
        [InlineData("b209106e-2dff-4e85-ae71-c2fdec3385f0")]
        [InlineData("601ab4c1-c6e7-42b6-b055-4db2a8227914")]
        [InlineData("d7ba46ff-31d2-4135-b5a3-758734c78971")]
        public void CanGetValueSetSummaryByGuid(string valueSetUniqueId)
        {
            // Arrange
            var valueSetGuid = Guid.Parse(valueSetUniqueId);

            // Act
            var valueSetSummary = this.Profiler.ExecuteTimed(() => this.valueSetSummaryService.GetValueSetSummary(valueSetGuid)).Single();
            this.Output.WriteLine(valueSetSummary.Name);
            this.Output.WriteLine($"Code count: {valueSetSummary.CodeCounts.Count}");

            // Assert
            valueSetSummary.ValueSetGuid.Should().Be(valueSetGuid);
        }


        [Theory]
        [InlineData("b209106e-2dff-4e85-ae71-c2fdec3385f0")]
        [InlineData("601ab4c1-c6e7-42b6-b055-4db2a8227914")]
        [InlineData("d7ba46ff-31d2-4135-b5a3-758734c78971")]
        public void CanGetValueSetByGuidWithCodeSystem(string valueSetUniqueId)
        {
            // Arrange
            var valueSetGuid = Guid.Parse(valueSetUniqueId);
            var codeSystems = new List<Guid> { Guid.Parse("9c78e8c1-71c4-43e4-b729-8809b4785431") };

            // Act
            var valueSet = this.Profiler.ExecuteTimed(() => this.valueSetService.GetValueSet(valueSetGuid, codeSystems)).Single();
            this.Output.WriteLine(valueSet.Name);
            this.Output.WriteLine($"Code count: {valueSet.ValueSetCodes.Count}");

            // Assert
            valueSet.ValueSetGuid.Should().Be(valueSetGuid);
        }

        [Theory]
        [InlineData("b209106e-2dff-4e85-ae71-c2fdec3385f0")]
        [InlineData("601ab4c1-c6e7-42b6-b055-4db2a8227914")]
        [InlineData("d7ba46ff-31d2-4135-b5a3-758734c78971")]
        public void CanGetValueSetSummaryByGuidWithCodeSystem(string valueSetUniqueId)
        {
            // Arrange
            var valueSetGuid = Guid.Parse(valueSetUniqueId);
            var codeSystems = new List<Guid> { Guid.Parse("9c78e8c1-71c4-43e4-b729-8809b4785431") };

            // Act
            var valueSetSummary = this.Profiler.ExecuteTimed(() => this.valueSetSummaryService.GetValueSetSummary(valueSetGuid, codeSystems)).Single();
            this.Output.WriteLine(valueSetSummary.Name);
            this.Output.WriteLine($"Code count: {valueSetSummary.CodeCounts.Count}");

            // Assert
            valueSetSummary.ValueSetGuid.Should().Be(valueSetGuid);
        }



        [Theory]
        [InlineData("2.16.840.1.113883.3.464.1003.109.12.1028")]
        [InlineData("2.16.840.1.113883.3.464.1003.111.12.1011")]
        public void CanGetValueSetVersions(string valueSetReferenceId)
        {
            // Arrange

            // Act
            var versions = this.Profiler.ExecuteTimed(async () => await this.valueSetService.GetValueSetVersions(valueSetReferenceId));

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
            var versions = this.Profiler.ExecuteTimed(async () => await this.valueSetSummaryService.GetValueSetVersions(valueSetReferenceId));

            // Assert
            versions.Should().NotBeEmpty();
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
    }
}