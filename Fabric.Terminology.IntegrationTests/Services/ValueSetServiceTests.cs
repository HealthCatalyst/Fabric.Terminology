namespace Fabric.Terminology.IntegrationTests.Services
{
    using System;

    using Fabric.Terminology.API;
    using Fabric.Terminology.Domain.Services;
    using Fabric.Terminology.IntegrationTests.Fixtures;
    using Fabric.Terminology.TestsBase;
    using Fabric.Terminology.TestsBase.Mocks;

    using FluentAssertions;

    using JetBrains.Annotations;

    using Xunit;
    using Xunit.Abstractions;

    public class ValueSetServiceTests : OutputTestBase, IClassFixture<ValueSetServiceFixture>
    {
        private readonly ValueSetServiceFixture fixture;

        private readonly IValueSetService valueSetService;

        public ValueSetServiceTests(ValueSetServiceFixture fixture, [NotNull] ITestOutputHelper output)
            : base(output)
        {
            this.fixture = fixture;
            this.valueSetService = fixture.ValueSetService;
        }

        [Theory]
        [InlineData("Add VS 1", 5)]
        [InlineData("Add VS 2", 1000)]
        [InlineData("Add VS 4", 4000)]
        public void CanAddValueSet(string name, int codeCount)
        {
            // Arrange
            var apiModel = MockApiModelBuilder.ValueSetCreationApiModel(name, codeCount);
            var attempt = this.valueSetService.Create(apiModel);
            attempt.Success.Should().BeTrue();
            attempt.Result.HasValue.Should().BeTrue();

            var vs = attempt.Result.Single();

            // Act
            this.Profiler.ExecuteTimed(() => this.valueSetService.Save(vs));

            // Assert
            vs.ValueSetUniqueId.Should().NotBe(Guid.Empty.ToString());
            vs.ValueSetCodes.Count.Should().Be(codeCount);
            vs.Name.Should().BeEquivalentTo(name);

            // cleanup
            this.valueSetService.Delete(vs);
        }
    }
}