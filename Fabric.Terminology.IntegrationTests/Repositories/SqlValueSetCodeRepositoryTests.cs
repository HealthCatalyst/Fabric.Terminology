using System.Linq;
using Fabric.Terminology.Domain.Models;
using Fabric.Terminology.Domain.Persistence;
using Fabric.Terminology.IntegrationTests.Fixtures;
using Fabric.Terminology.TestsBase;
using Xunit;
using Xunit.Abstractions;

namespace Fabric.Terminology.IntegrationTests.Repositories
{
    public class SqlValueSetCodeRepositoryTests : ProfiledTestsBase, IClassFixture<ValueSetCodeRepositoryFixture>
    {
        
        public SqlValueSetCodeRepositoryTests(ValueSetCodeRepositoryFixture fixture, ITestOutputHelper output) 
            : base(output)
        {
            ValueSetCodeRepository = fixture.ValueSetCodeRepository;
        }

        private IValueSetCodeRepository ValueSetCodeRepository { get; }


        [Theory]
        [InlineData("F", "2.16.840.1.113883.5.1", "2.16.840.1.113762.1.4.1")]
        [InlineData("F", "2.16.840.1.113883.5.1", "2.16.840.1.113883.3.560.100.2")]
        [InlineData("321208", "2.16.840.1.113883.6.88", "2.16.840.1.113762.1.4.1021.11")]
        public void GetCode_Found(string code, string codeSystemCode, string valueSetId)
        {
            // Arrange
            
            // Act
            var result = ExecuteTimed(() => ValueSetCodeRepository.GetCode(code, codeSystemCode, valueSetId));

            // Assert
            Assert.NotNull(result);
            Assert.Equal(code, result.Code);
            Assert.Equal(codeSystemCode, result.CodeSystem.Code);
        }

        [Theory]
        [InlineData("TEST", "2.16.840.1.113883.5.1", "2.16.840.1.113762.1.4.1")]
        [InlineData("321208", "NO_WAY_THIS_IS_A_VALID_SYSTEM_CODE", "2.16.840.1.113762.1.4.1021.11")]
        public void GetCode_NotFound(string code, string codeSystemCode, string valueSetId)
        {
            // Arrange

            // Act
            var result = ExecuteTimed(() => ValueSetCodeRepository.GetCode(code, codeSystemCode, valueSetId));

            // Assert
            Assert.Null(result);
        }

        [Theory]
        [Trait(TestTraits.Category, TestCategory.LongRunning)]
        [InlineData("2.16.840.1.113883.6.104", 1, 500)] // ICD9CM - approx 11827 rows
        [InlineData("2.16.840.1.113883.6.104", 2, 500)]
        [InlineData("2.16.840.1.113883.6.104", 3, 500)]
        [InlineData("2.16.840.1.113883.6.90", 1, 500)] // ICD10CM - approx 11
        [InlineData("2.16.840.1.113883.6.90", 3, 500)]
#pragma warning disable AvoidAsyncSuffix // Avoid Async suffix
        public void GetCodesAsync(string codeSystemCode, int currentPage, int itemsPerPage)
#pragma warning restore AvoidAsyncSuffix // Avoid Async suffix
        {
            // Arrange
            var settings = new PagerSettings { CurrentPage = currentPage, ItemsPerPage = itemsPerPage };

            // Act
            var codesPage = ExecuteTimedAysnc(() => ValueSetCodeRepository.GetCodesAsync(codeSystemCode, settings), $"Querying code system code = {codeSystemCode} - Page {currentPage}");
            Output.WriteLine($"Result count: {codesPage.Items.Count}");

            // Assert
            Assert.Equal(currentPage, codesPage.PagerSettings.CurrentPage);
            Assert.Equal(itemsPerPage, codesPage.PagerSettings.ItemsPerPage);
            Assert.True(codesPage.TotalItems > 0);
            Output.WriteLine($"Last page {codesPage.TotalPages}");
        }

        [Fact]
        public void GetCodesAsync_MultipleCodeSystems()
        {
            // Arrange
            var codeSystemCodes = new[] { "2.16.840.1.113883.6.104", "2.16.840.1.113883.6.90" };
            var settings = new PagerSettings {CurrentPage = 1, ItemsPerPage = 500};

            // Act
            var codesPage = ExecuteTimedAysnc(() => ValueSetCodeRepository.GetCodesAsync(codeSystemCodes, settings), $"Querying code system code = {codeSystemCodes} - Page {settings.CurrentPage}");

            // Assert
            Assert.Equal(settings.CurrentPage, codesPage.PagerSettings.CurrentPage);
            Assert.Equal(settings.ItemsPerPage, codesPage.PagerSettings.ItemsPerPage);
            Assert.True(codesPage.TotalItems > 0);
            Output.WriteLine($"Total items: {codesPage.TotalItems}");
            Output.WriteLine($"Last page {codesPage.TotalPages}");
        }

        [Theory]
        [InlineData("Low forceps", "2.16.840.1.113883.6.104", 1, 500)]
        [InlineData("Low forceps operation", "2.16.840.1.113883.6.104", 1, 500)]
        [InlineData("episiotomy", "2.16.840.1.113883.6.104", 1, 500)]
        public void FindCodesAsync_ShouldHaveResults(string codeDsc, string codeSystemCode, int currentPage, int itemsPerPage)
        {
            //// Arrange
            var settings = new PagerSettings { CurrentPage = currentPage, ItemsPerPage = itemsPerPage };

            //// Act
            var codesPage = ExecuteTimedAysnc(() => ValueSetCodeRepository.FindCodesAsync(codeDsc, codeSystemCode, settings), $"Querying code system code = {codeSystemCode} - Page {currentPage}");
            Output.WriteLine($"Result count: {codesPage.Items.Count}");

            //// Assert
            Assert.Equal(currentPage, codesPage.PagerSettings.CurrentPage);
            Assert.Equal(itemsPerPage, codesPage.PagerSettings.ItemsPerPage);
            Assert.True(codesPage.TotalItems > 0);
            Output.WriteLine($"Last page {codesPage.TotalPages}");
        }

        [Theory]
        [InlineData("NO_WAY_THIS_IS_A_VALID_NAME", "2.16.840.1.113883.6.104", 1, 500)]
        [InlineData("NO_WAY_THIS_IS_A_VALID_NAME", "2.16.840.1.113883.6.104", 2, 500)]
        [InlineData("Low forceps", "NO_WAY_THIS_IS_A_VALID_SYSTEM_CODE", 1, 500)]
        public void FindCodesAsync_ShouldReturnEmpty(string codeDsc, string codeSystemCode, int currentPage, int itemsPerPage)
        {
            // Arrange
            var settings = new PagerSettings { CurrentPage = currentPage, ItemsPerPage = itemsPerPage };

            // Act
            var codesPage = ExecuteTimedAysnc(() => ValueSetCodeRepository.FindCodesAsync(codeDsc, codeSystemCode, settings), $"Querying code system code = {codeSystemCode} - Page {currentPage}");
            Output.WriteLine($"Result count: {codesPage.Items.Count}");

            // Assert
            Assert.Equal(0, codesPage.TotalPages);
            Assert.Equal(itemsPerPage, codesPage.PagerSettings.ItemsPerPage);
            Assert.True(codesPage.TotalItems == 0);
            Assert.False(codesPage.Items.Any());
            Output.WriteLine($"Last page {codesPage.TotalPages}");
        }

        [Theory]
        [Trait(TestTraits.Category, TestCategory.LongRunning)]
        [InlineData("2.16.840.1.113883.3.464.1003.108.12.1011")]
        [InlineData("2.16.840.1.113762.1.4.1045.36")]
        [InlineData("2.16.840.1.113883.3.526.3.1459")]
        [InlineData("2.16.840.1.113883.3.67.1.101.1.278")]
        public void GetValueSetCodes(string valueSetId)
        {
            // Arrange
            // Handled in inline data

            //// Act
            var codes = ExecuteTimed(() => ValueSetCodeRepository.GetValueSetCodes(valueSetId), $"Querying ValueSetId = {valueSetId}");
            Output.WriteLine($"Result count: {codes.Count}");

            //// Assert
            Assert.True(codes.Any());
        }
    }
}