using System;
using System.Globalization;
using System.Linq;
using Fabric.Terminology.Domain.Persistence;
using Fabric.Terminology.SqlServer.Persistence;
using Fabric.Terminology.SqlServer.Persistence.DataContext;
using Fabric.Terminology.TestsBase;
using Fabric.Terminology.TestsBase.Mocks;
using Xunit;
using Xunit.Abstractions;

namespace Fabric.Terminology.IntegrationTests.Repositories
{
    public class SqlValueSetCodeRepositoryTests : RuntimeTestsBase
    {
        private readonly IValueSetCodeRepository _valueSetCodeRepository;

        public SqlValueSetCodeRepositoryTests(ITestOutputHelper output, ConfigTestFor testType = ConfigTestFor.Integration) : base(output, testType)
        {
            var factory = new SharedContextFactory(AppConfig.TerminologySqlSettings, Logger);
            var context = factory.Create();
            if (context.IsInMemory) throw new InvalidOperationException();
            _valueSetCodeRepository = new SqlValueSetCodeRepository(factory.Create(), new NullMemoryCacheProvider());
        }


        [Theory]
        [Trait(TestTraits.Category, TestCategory.LongRunning)]
        [InlineData("2.16.840.1.113883.3.464.1003.108.12.1011")]
        [InlineData("2.16.840.1.113762.1.4.1045.36")]
        [InlineData("2.16.840.1.113883.3.526.3.1459")]
        [InlineData("2.16.840.1.113883.3.67.1.101.1.278")]
        public void GetCodesByValueSet(string valueSetId)
        {
            //// Arrange

            //// Act
            var codes = ExecuteTimed(() => _valueSetCodeRepository.GetCodesByValueSet(valueSetId), $"Querying ValueSetId = {valueSetId}");
            Output.WriteLine($"Result count: {codes.Count}");

            //// Assert
            Assert.True(codes.Any());
            
        }

        [Theory]
        [Trait(TestTraits.Category, TestCategory.LongRunning)]
        [InlineData("2.16.840.1.113883.6.104", 1, 500)] // ICD9CM - approx 11827 rows
        [InlineData("2.16.840.1.113883.6.104", 2, 500)]
        [InlineData("2.16.840.1.113883.6.104", 3, 500)]
        [InlineData("2.16.840.1.113883.6.90", 1, 500)] // ICD10CM - approx 11
        [InlineData("2.16.840.1.113883.6.90", 3, 500)]
        public void GetCodesByValueSetCodeSystem(string codeSystemCode, int currentPage, int itemsPerPage)
        {
            //// Arrange
            //// Handled in inline data
 
            //// Act
            var codesPage = ExecuteTimed(async () => await _valueSetCodeRepository.GetValueSetCodesAsync(codeSystemCode, currentPage, itemsPerPage), $"Querying code system code = {codeSystemCode} - Page {currentPage}").Result;
            Output.WriteLine($"Result count: {codesPage.Items}");

            //// Assert
            Assert.Equal(currentPage, codesPage.PagerSettings.CurrentPage);
            Assert.Equal(itemsPerPage, codesPage.PagerSettings.ItemsPerPage);
            Assert.True(codesPage.TotalItems > 0);
            Output.WriteLine($"Last page {codesPage.TotalPages}");
        }
    }
}