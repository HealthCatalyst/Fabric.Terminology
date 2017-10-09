namespace Fabric.Terminology.IntegrationTests.Services
{
    using System;
    using System.Linq;

    using Fabric.Terminology.API;
    using Fabric.Terminology.Domain.Services;
    using Fabric.Terminology.IntegrationTests.Fixtures;
    using Fabric.Terminology.TestsBase;
    using Fabric.Terminology.TestsBase.Mocks;

    using FluentAssertions;

    using Newtonsoft.Json;

    using Xunit;
    using Xunit.Abstractions;

    public class ClientTermValueSetServiceTests : OutputTestBase, IClassFixture<ServiceFixture>
    {
        private readonly IClientTermValueSetService clientTermValueSetService;

        public ClientTermValueSetServiceTests(ServiceFixture fixture, ITestOutputHelper output)
            : base(output)
        {
            this.clientTermValueSetService = fixture.ClientTermValueSetService;
        }

        [Theory]
        [InlineData("Code Count Check", 50)]
        public void CanCreateValueSet(string name, int codeCount)
        {
            // Arrange
            var apiModel = MockApiModelBuilder.ValueSetCreationApiModel(name, codeCount);

            // Act
            var attempt = this.clientTermValueSetService.Create(apiModel);

            // Assert
            attempt.Success.Should().BeTrue();
            attempt.Result.HasValue.Should().BeTrue();

            var vs = attempt.Result.Single();

            vs.ValueSetCodes.Should().NotBeEmpty();
            vs.CodeCounts.Should().NotBeEmpty();
            vs.CodeCounts.Sum(cc => cc.CodeCount).Should().Be(codeCount);
        }

        [Theory]
        [InlineData("Add VS 3", 5)]
        [InlineData("Add VS 2", 1000)]
        [InlineData("Add VS 4", 4000)]
        public void CanAddValueSet(string name, int codeCount)
        {
            // Arrange
            var apiModel = MockApiModelBuilder.ValueSetCreationApiModel(name, codeCount);

            var attempt = this.clientTermValueSetService.Create(apiModel);
            attempt.Success.Should().BeTrue();
            attempt.Result.HasValue.Should().BeTrue();

            var vs = attempt.Result.Single();

            // Act
            this.Profiler.ExecuteTimed(() => this.clientTermValueSetService.Save(vs));

            // Assert
            vs.ValueSetGuid.Should().NotBe(Guid.Empty);
            vs.ValueSetCodes.Count.Should().Be(codeCount);
            vs.Name.Should().BeEquivalentTo(name);

            // cleanup
            this.clientTermValueSetService.Delete(vs);
        }
    }
}
