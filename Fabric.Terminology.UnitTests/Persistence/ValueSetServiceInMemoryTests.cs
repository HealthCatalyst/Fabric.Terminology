namespace Fabric.Terminology.UnitTests.Persistence
{
    using System;
    using System.Linq;

    using Fabric.Terminology.API;
    using Fabric.Terminology.Domain;
    using Fabric.Terminology.Domain.Services;
    using Fabric.Terminology.SqlServer.Persistence;
    using Fabric.Terminology.TestsBase;
    using Fabric.Terminology.TestsBase.Fixtures;
    using Fabric.Terminology.TestsBase.Mocks;

    using FluentAssertions;

    using Newtonsoft.Json;

    using Xunit;
    using Xunit.Abstractions;

    public class ValueSetServiceInMemoryTests : OutputTestBase, IClassFixture<CustomValueSetFixture>
    {
        private readonly SqlValueSetRepository valueSetRepository;

        private readonly SqlValueSetCodeRepository valueSetCodeRepository;

        private readonly IValueSetService valueSetService;

        public ValueSetServiceInMemoryTests(ITestOutputHelper output, CustomValueSetFixture fixture)
            : base(output)
        {
            this.valueSetRepository = fixture.ValueSetRepository;
            this.valueSetService = fixture.ValueSetService;
            this.valueSetCodeRepository = fixture.ValueSetCodeRepository;
        }

        [Fact]
        public void EnsureRepositorySeeded()
        {
            // Arrange
            // Handled in Class Fixture

            // Act
            var vs = this.valueSetRepository.GetCustomValueSet("test.valueset.1");

            // Assert
            vs.Should().NotBeNull();
        }

        [Fact]
        public void CanCreateValueSet()
        {
            // Arrange
            var apiModel = MockApiModelBuilder.ValueSetCreationApiModel("Custom Value Set");
            var emptyId = Guid.Empty.ToString();

            // Act
            var attempt = this.valueSetService.Create(apiModel);

            // Assert
            attempt.Success.Should().BeTrue();
            attempt.Result.HasValue.Should().BeTrue();
            var valueSet = attempt.Result.Single();
            valueSet.Should().NotBeNull();
            valueSet.IsCustom.Should().BeTrue();
            valueSet.ValueSetUniqueId.Should().Be(emptyId);
            valueSet.ValueSetId.Should().Be(emptyId);
            valueSet.ValueSetOId.Should().Be(emptyId);
            valueSet.Name.Should().Be("Custom Value Set");
            foreach (var code in valueSet.ValueSetCodes)
            {
                code.ValueSetUniqueId.Should().Be(emptyId);
            }
        }

        [Theory]
        [InlineData("Add VS 1", 5)]
        [InlineData("Add VS 2", 1000)]
        [InlineData("Add VS 3", 2000)]
        [InlineData("Add VS 4", 3000)]
        [InlineData("Add VS 5", 4000)]
        [InlineData("Add VS 6", 8000)]
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
        }

        [Fact]
        public void CanDeleteValueSet()
        {
            // Arrange
            var apiModel = MockApiModelBuilder.ValueSetCreationApiModel("VS FOR DELETE", 25);
            var attempt = this.valueSetService.Create(apiModel);
            attempt.Success.Should().BeTrue();
            attempt.Result.HasValue.Should().BeTrue();

            var vs = attempt.Result.Single();
            this.Profiler.ExecuteTimed(() => this.valueSetService.Save(vs), "ValueSet Save");

            vs.ValueSetUniqueId.Should().NotBe(Guid.Empty.ToString());
            vs.IsNew().Should().BeFalse();
            var uid = vs.ValueSetUniqueId;

            // Act
            this.valueSetService.Delete(vs);

            // Assert
            var retreived = this.valueSetRepository.GetCustomValueSet(uid);
            var codes = this.valueSetCodeRepository.GetCustomValueSetCodes(uid, Enumerable.Empty<string>());

            retreived.HasValue.Should().BeFalse();
            codes.Should().BeEmpty();
        }
    }
}