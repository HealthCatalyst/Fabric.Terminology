namespace Fabric.Terminology.UnitTests.AutoMapper
{
    using System.Linq;

    using Fabric.Terminology.API.Models;
    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.TestsBase;
    using Fabric.Terminology.TestsBase.Fixtures;

    using FluentAssertions;

    using global::AutoMapper;

    using Xunit;

    public class AutoMapperTests : IClassFixture<AppConfigurationFixture>
    {
        private readonly AppConfigurationFixture fixture;

        public AutoMapperTests(AppConfigurationFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public void CanMapValueSetToValueSetApiModel()
        {
            // Arrange
            var valueSet = DataHelper.SingleValueSet("1", 3);

            // Act
            var mapped = Mapper.Map<IValueSet, ValueSetApiModel>(valueSet);

            // Assert
            mapped.Should().NotBeNull();
            mapped.Should().BeEquivalentTo(valueSet, o => o.Excluding(p => p.ValueSetCodes));
            //mapped.Identifier.Should().BeEquivalentTo(valueSet.ValueSetId);

            var codes = valueSet.ValueSetCodes.ToArray();
            var mappedCodes = mapped.ValueSetCodes.ToArray();
            for (var i = 0; i < codes.Length; i++)
            {
                var code = codes[i];
                mappedCodes[i].Should().BeEquivalentTo(code, o => o.ExcludingMissingMembers());
              //  mappedCodes[i].CodeSystem.Code.Should().Be(code.CodeSystem.Code);
            }
        }
    }
}