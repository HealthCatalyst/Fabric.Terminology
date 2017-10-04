namespace Fabric.Terminology.IntegrationTests.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Fabric.Terminology.Domain.Persistence;
    using Fabric.Terminology.IntegrationTests.Fixtures;
    using Fabric.Terminology.TestsBase;

    using FluentAssertions;

    using Xunit;
    using Xunit.Abstractions;

    public class SqlValueSetCodeRepositoryTests : OutputTestBase, IClassFixture<ValueSetCodeRepositoryFixture>
    {
        public SqlValueSetCodeRepositoryTests(ValueSetCodeRepositoryFixture fixture, ITestOutputHelper output)
            : base(output)
        {
            this.ValueSetCodeRepository = fixture.ValueSetCodeRepository;
        }

        private IValueSetCodeRepository ValueSetCodeRepository { get; }

        [Theory]
        [InlineData("35F6B1A6-A72B-48F5-B319-F6CCAF15734D")]
        public void GetValueSetCodes(string key)
        {
            // Arrange
            var valueSetGuid = Guid.Parse(key);

            // Act
            var codes = this.Profiler.ExecuteTimed(() => this.ValueSetCodeRepository.GetValueSetCodes(valueSetGuid), $"Querying ValueSetGuid {valueSetGuid}");

            // Assert
            codes.Should().NotBeEmpty();
            codes.All(c => c.ValueSetGuid == valueSetGuid).Should().BeTrue();
        }

        [Fact]
        public void GetValueSetCodesForMultipleValueSets()
        {
            // Arrange
            var testValues = new List<Tuple<Guid, int>>
            {
                new Tuple<Guid, int>(new Guid("A2216AAC-8513-43D8-85C2-00057F92394B"), 30), // hypertension
                new Tuple<Guid, int>(new Guid("31EA98DC-D050-47A2-9435-002C19CEBF8F"), 57), // kidney failure
                new Tuple<Guid, int>(new Guid("35F6B1A6-A72B-48F5-B319-F6CCAF15734D"), 424) // diabetes
            };

            // Act
            var codeDictionary = this.Profiler.ExecuteTimed(async () => await this.ValueSetCodeRepository.BuildValueSetCodesDictionary(testValues.Select(t => t.Item1)));

            // Assert
            codeDictionary.Keys.Count.Should().Be(3);
            foreach (var tv in testValues)
            {
                codeDictionary[tv.Item1].Count.Should().Be(tv.Item2);
            }
        }
    }
}