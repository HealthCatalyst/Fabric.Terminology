namespace Fabric.Terminology.UnitTests.AutoMapper
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Fabric.Terminology.API.Models;
    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.TestsBase.Fixtures;

    using FluentAssertions;

    using global::AutoMapper;

    using JetBrains.Annotations;

    using Xunit;

    public class AutoMapperTests : IClassFixture<AppConfigurationFixture>
    {
        private readonly AppConfigurationFixture fixture;

        public AutoMapperTests(AppConfigurationFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public void CanMapValueSetToValueSetItem()
        {
            // Arrange
            var valueSetId = "value.set.id.1";

            var valueSet = new ValueSet
            {
                ValueSetId = valueSetId,
                AuthoringSourceDescription = "author",
                IsCustom = true,
                Name = "test",
                PurposeDescription = "purpose",
                SourceDescription = "source",
                ValueSetCodesCount = 3,
                VersionDescription = "version",
                ValueSetCodes = new List<IValueSetCode>
                {
                    new ValueSetCode
                    {
                        Code = "code1",
                        CodeSystem = new ValueSetCodeSystem { Code = "cd1", Name = "cd1", Version = "cd1version" },
                        Name = "code1",
                        RevisionDate = null,
                        ValueSetId = valueSetId,
                        VersionDescription = "version"
                    },
                    new ValueSetCode
                    {
                        Code = "code2",
                        CodeSystem = new ValueSetCodeSystem { Code = "cd1", Name = "cd1", Version = "cd1version" },
                        Name = "code1",
                        RevisionDate = null,
                        ValueSetId = valueSetId,
                        VersionDescription = "version"
                    },
                    new ValueSetCode
                    {
                        Code = "code3",
                        CodeSystem = new ValueSetCodeSystem { Code = "cd1", Name = "cd1", Version = "cd1version" },
                        Name = "code1",
                        RevisionDate = null,
                        ValueSetId = valueSetId,
                        VersionDescription = "version"
                    }
                }
            };

            // Act
            var mapped = Mapper.Map<IValueSet, ValueSetApiModel>(valueSet);

            // Assert
            mapped.Should().NotBeNull();
            mapped.ShouldBeEquivalentTo(valueSet, o => o.Excluding(p => p.Identifier).Excluding(p => p.ValueSetCodes));
            mapped.Identifier.ShouldBeEquivalentTo(valueSet.ValueSetId);

            var codes = valueSet.ValueSetCodes.ToArray();
            var mappedCodes = mapped.ValueSetCodes.ToArray();
            for (var i = 0; i < codes.Length; i++)
            {
                var code = codes[i];
                mappedCodes[i].ShouldBeEquivalentTo(code, o => o.ExcludingMissingMembers());
                mappedCodes[i].CodeSystemCode.Should().Be(code.CodeSystem.Code);
            }
        }
    }
}