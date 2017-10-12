namespace Fabric.Terminology.IntegrationTests.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.Domain.Services;
    using Fabric.Terminology.IntegrationTests.Fixtures;
    using Fabric.Terminology.TestsBase;

    using FluentAssertions;

    using Xunit;
    using Xunit.Abstractions;

    public class CodeSystemCodeServiceTests : OutputTestBase, IClassFixture<ServiceFixture>
    {
        private readonly ICodeSystemCodeService codeSystemCodeService;

        public CodeSystemCodeServiceTests(ServiceFixture fixture, ITestOutputHelper output)
            : base(output)
        {
            this.codeSystemCodeService = fixture.CodeSystemCodeService;
        }

        [Theory]
        [InlineData("31E98653-4DD5-4A31-9CDC-CCA74CEFF74B")]
        [InlineData("31EA3D79-D9CE-4E21-AC24-EB6F66FF0B3B")]
        [InlineData("31EBB934-6DCE-4FC6-A5FB-76071B4DB2DC")]
        public void GetCodeSystemCode(string codeUniqueId)
        {
            // Arrange
            var codeGuid = Guid.Parse(codeUniqueId);

            // Act
            var maybe = this.codeSystemCodeService.GetCodeSystemCode(codeGuid);

            // Assert
            maybe.HasValue.Should().BeTrue();
        }

        [Fact]
        public void GetCodeSystemCodeList()
        {
            // Arrange
            var codeGuids = new List<Guid>
            {
                Guid.Parse("31E98653-4DD5-4A31-9CDC-CCA74CEFF74B"),
                Guid.Parse("31E9BCFD-9C4F-4B63-8E65-DA7CD347D6DA"),
                Guid.Parse("31EA3D79-D9CE-4E21-AC24-EB6F66FF0B3B"),
                Guid.Parse("31EA5DF2-5DA8-43B2-9BC3-A89AF4E34855"),
                Guid.Parse("31EA7943-921F-47EF-982E-469D6B21ED6B"),
                Guid.Parse("31EA9959-E908-4612-BD19-6744CE99D80F"),
                Guid.Parse("31EB06F3-03E7-4AFA-82D9-83DAD84C5144"),
                Guid.Parse("31EB2610-8F53-4E72-8DBB-5DB823D1D8AE"),
                Guid.Parse("31EB7BE2-D348-4A4B-A4A9-C775C910D886"),
                Guid.Parse("31EB87D7-D27B-4F05-ABCC-B07EB01ED73C")
            };

            // Act
            var codes = this.codeSystemCodeService.GetCodeSystemCodes(codeGuids);

            // Assert
            codes.Should().NotBeEmpty();
            codes.Count.Should().Be(codeGuids.Count);
        }

        [Theory]
        [InlineData(10, 1)]
        [InlineData(20, 2)]
        [InlineData(20, 3)]
        [InlineData(100, 1)]
        [InlineData(100, 2)]
        public void GetCodeSystemCodeAsyncPages(int itemsPerPage, int pageNumber)
        {
            // Arrange
            var pagerSettings = new PagerSettings { CurrentPage = pageNumber, ItemsPerPage = itemsPerPage };

            // Act
            var page = this.Profiler
                .ExecuteTimed(async () => await this.codeSystemCodeService.GetCodeSystemCodesAsync(pagerSettings));

            this.Output.WriteLine($"Total Values {page.TotalItems}");
            this.Output.WriteLine($"Total Pages {page.TotalPages}");

            // Assert
            page.TotalItems.Should().BeGreaterThan(0);
            page.TotalPages.Should().BeGreaterThan(0);
            page.Values.Count.Should().BeLessOrEqualTo(itemsPerPage);
        }

        [Theory]
        [InlineData("9C78E8C1-71C4-43E4-B729-8809B4785431", 10, 1)] // RXNORM
        public void GetCodesForCodeSystem(string codeSystemUniqueId, int itemsPerPage, int pageNumber)
        {
            // Arrange
            var codeSystemGuid = Guid.Parse(codeSystemUniqueId);
            var pagerSettings = new PagerSettings { CurrentPage = pageNumber, ItemsPerPage = itemsPerPage };

            // Act
            var page = this.Profiler
                .ExecuteTimed(async () => await this.codeSystemCodeService.GetCodeSystemCodesAsync(pagerSettings, codeSystemGuid));

            this.Output.WriteLine($"Total Values {page.TotalItems}");
            this.Output.WriteLine($"Total Pages {page.TotalPages}");

            // Assert
            page.TotalItems.Should().BeGreaterThan(0);
            page.TotalPages.Should().BeGreaterThan(0);
            page.Values.Count.Should().BeLessOrEqualTo(itemsPerPage);
            var codeSystems = page.Values.Select(c => c.CodeSystemGuid).Distinct().ToArray();
            codeSystems.Length.Should().Be(1);
            codeSystems[0].Should().Be(codeSystemGuid);
        }
    }
}
