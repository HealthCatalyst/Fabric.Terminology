namespace Fabric.Terminology.IntegrationTests.Services
{
    using System;

    using Fabric.Terminology.Domain.Services;
    using Fabric.Terminology.IntegrationTests.Fixtures;
    using Fabric.Terminology.TestsBase;

    using FluentAssertions;

    using Xunit;
    using Xunit.Abstractions;

    public class ElasticCodeSystemServiceTests : OutputTestBase, IClassFixture<ElasticServiceFixture>
    {
        private readonly ICodeSystemService codeSystemService;

        public ElasticCodeSystemServiceTests(ElasticServiceFixture fixture, ITestOutputHelper output)
            : base(output)
        {
            this.codeSystemService = fixture.CodeSystemService;
        }

        [Fact]
        public void GetAll()
        {
            // Arrange

            // Act
            var codeSystems = this.Profiler.ExecuteTimed(() => this.codeSystemService.GetAll());

            // Assert
            codeSystems.Should().NotBeEmpty();
            this.Output.WriteLine($"Code system count: {codeSystems.Count}");
        }

        [Theory]
        [InlineData("9C78E8C1-71C4-43E4-B729-8809B4785431")]
        [InlineData("87F53B39-2EDF-4045-82CF-93010055A5B8")]
        public void CanGetCodeSystem(string codeSystemRefernceId)
        {
            // Arrange
            var codeSystemGuid = Guid.Parse(codeSystemRefernceId);

            // Act
            var maybe = this.Profiler.ExecuteTimed(() => this.codeSystemService.GetCodeSystem(codeSystemGuid));

            // Assert
            maybe.HasValue.Should().BeTrue();
            var codeSystem = maybe.Single();

            codeSystem.CodeSystemGuid.Should().Be(codeSystemGuid);
        }
    }
}
