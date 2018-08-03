namespace Fabric.Terminology.IntegrationTests.Services
{
    using System;

    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.Domain.Services;
    using Fabric.Terminology.IntegrationTests.Fixtures;
    using Fabric.Terminology.TestsBase;

    using FluentAssertions;

    using Xunit;
    using Xunit.Abstractions;

    public class SqlValueSetCodeServiceTests : OutputTestBase, IClassFixture<SqlServiceFixture>
    {
        private readonly IValueSetCodeService valueSetCodeService;

        public SqlValueSetCodeServiceTests(SqlServiceFixture fixture, ITestOutputHelper output)
            : base(output)
        {
            this.valueSetCodeService = fixture.ValueSetCodeService;
        }

        [Theory]
        [InlineData("35F6B1A6-A72B-48F5-B319-F6CCAF15734D")]
        [InlineData("A2216AAC-8513-43D8-85C2-00057F92394B")]
        [InlineData("31EA98DC-D050-47A2-9435-002C19CEBF8F")]
        public void CanGetValueSetCodesByValueSet(string valueSetReferenceId)
        {
            // Arrange
            var valueSetGuid = Guid.Parse(valueSetReferenceId);

            // Act
            var codes = this.Profiler.ExecuteTimed(() => this.valueSetCodeService.GetValueSetCodes(valueSetGuid));

            // Assert
            codes.Should().NotBeEmpty();
            this.Output.WriteLine($"Code count: {codes.Count}");
        }

        [Theory]
        [InlineData("35F6B1A6-A72B-48F5-B319-F6CCAF15734D", 10, 6)]
        [InlineData("A2216AAC-8513-43D8-85C2-00057F92394B", 10, 2)]
        [InlineData("31EA98DC-D050-47A2-9435-002C19CEBF8F", 20, 2)]
        public void CanGetValueSetCodesPageByValueSet(string valueSetReferenceId, int itemsPerPage, int pageNumber)
        {
            // Arrange
            var valueSetGuid = Guid.Parse(valueSetReferenceId);
            var pageSettings = new PagerSettings { CurrentPage = pageNumber, ItemsPerPage = itemsPerPage };

            // Act
            var page = this.Profiler.ExecuteTimed(async () => await this.valueSetCodeService.GetValueSetCodesAsync(valueSetGuid, pageSettings));

            this.Output.WriteLine($"Total Values {page.TotalItems}");
            this.Output.WriteLine($"Total Pages {page.TotalPages}");
            this.Output.WriteLine($"Current page {page.PagerSettings.CurrentPage}");

            // Assert
            page.TotalItems.Should().BeGreaterThan(0);
            page.TotalPages.Should().BeGreaterThan(0);
            page.Values.Count.Should().BeLessOrEqualTo(itemsPerPage);
        }

        [Theory]
        [InlineData("96B49FE8-7636-4741-AB2E-0C6EF5E1BC8B")]
        public void GetValueSetCodesByCodeGuid(string codeReferenceId)
        {
            // Arrange
            var codeGuid = Guid.Parse(codeReferenceId);

            // Act
            var codes = this.valueSetCodeService.GetValueSetCodesByCodeGuid(codeGuid);

            // Assert
            codes.Should().NotBeEmpty();
            this.Output.WriteLine($"Code count: {codes.Count}");
        }

        [Theory]
        [InlineData(10, 1)]
        [InlineData(20, 2)]
        [InlineData(20, 3)]
        [InlineData(100, 1)]
        [InlineData(100, 2)]
        public void GetValueSetCodes(int itemsPerPage, int pageNumber)
        {
            // Arrange
            var pagerSettings = new PagerSettings { CurrentPage = pageNumber, ItemsPerPage = itemsPerPage };

            // Act
            var page = this.Profiler.ExecuteTimed(async () => await this.valueSetCodeService.GetValueSetCodesAsync(pagerSettings));

            this.Output.WriteLine($"Total Values {page.TotalItems}");
            this.Output.WriteLine($"Total Pages {page.TotalPages}");

            // Assert
            page.TotalItems.Should().BeGreaterThan(0);
            page.TotalPages.Should().BeGreaterThan(0);
            page.Values.Count.Should().BeLessOrEqualTo(itemsPerPage);
        }
    }
}
