namespace Fabric.Terminology.IntegrationTests.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Fabric.Terminology.Domain.Persistence;
    using Fabric.Terminology.IntegrationTests.Fixtures;
    using Fabric.Terminology.TestsBase;
    using Fabric.Terminology.TestsBase.Mocks;

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
        //public void LookupValueSetCodes()
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
        //    var lookup = this.Profiler.ExecuteTimed(() => this.ValueSetCodeRepository.LookupValueSetCodes(mockDbSet.Object.ToList(), new string[] { }));

        //    // Assert
        //    lookup.Should().NotBeNull();
        //}
    }
}