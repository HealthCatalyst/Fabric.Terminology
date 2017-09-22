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

        //[Theory]
        //[Trait(TestTraits.Category, TestCategory.LongRunning)]
        //[InlineData("413B430E-42EC-41B2-939A-4A4E757124DD")]
        //[InlineData("5389CD6C-10E2-443E-A7A9-E0C1C6A0325F")]
        //[InlineData("E729FD76-77FA-4897-B855-DDCB67E8B648")]
        //[InlineData("93E5CCB3-6937-4391-A7E8-64D7E8683384")]
        //public void GetValueSetCodes(string valueSetUniqueId)
        //{
        //    // Arrange
        //    // Handled in inline data
        //    var valueSetGuid = new Guid(valueSetUniqueId);

        //    // Act
        //    var codes = this.Profiler.ExecuteTimed(() => this.ValueSetCodeRepository.GetValueSetCodes(valueSetGuid, new string[] { }), $"Querying ValueSetId = {valueSetUniqueId}");
        //    this.Output.WriteLine($"Result count: {codes.Count}");

        //    // Assert
        //    Assert.True(codes.Any());
        //}

        //[Fact]
        //public void BuildValueSetCodesDictionary()
        //{
        //    // Arrange
        //    var ids = new List<Guid>
        //    {
        //        new Guid("413B430E-42EC-41B2-939A-4A4E757124DD"),
        //        new Guid("5389CD6C-10E2-443E-A7A9-E0C1C6A0325F"),
        //        new Guid("E729FD76-77FA-4897-B855-DDCB67E8B648"),
        //        new Guid("93E5CCB3-6937-4391-A7E8-64D7E8683384")
        //    };

        //    var builder = new MockDbSetBuilder<string>();
        //    var mockDbSet = builder.Build(ids);

        //    // Act
        //    var lookup = this.Profiler.ExecuteTimed(() => this.ValueSetCodeRepository.BuildValueSetCodesDictionary(mockDbSet.Object.ToList(), new string[] { }));

        //    // Assert
        //    lookup.Should().NotBeNull();
        //}
    }
}