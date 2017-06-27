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
            var mapped = Mapper.Map<IValueSet, ValueSetItem>(valueSet);

            // Assert
            mapped.Should().NotBeNull();
            mapped.Name.ShouldBeEquivalentTo(valueSet.Name);
            mapped.AuthoringSourceDescription.ShouldBeEquivalentTo(valueSet.AuthoringSourceDescription);
            mapped.IsCustom.ShouldBeEquivalentTo(valueSet.IsCustom);
            mapped.PurposeDescription.ShouldBeEquivalentTo(valueSet.PurposeDescription);
            mapped.SourceDescription.ShouldBeEquivalentTo(valueSet.SourceDescription);
            mapped.ValueSetCodesCount.ShouldBeEquivalentTo(valueSet.ValueSetCodesCount);
            mapped.AllCodesLoaded.ShouldBeEquivalentTo(valueSet.AllCodesLoaded);
            mapped.ValueSetCodes.Count.Should().Be(3);

            // code assertions
            var code1 = mapped.ValueSetCodes.FirstOrDefault(code => code.Code == "code1");
            code1.Should().NotBeNull();
            code1.Code.ShouldBeEquivalentTo("code1");
            code1.CodeSystemCode.ShouldBeEquivalentTo("cd1");
            code1.Name.ShouldBeEquivalentTo("code1");
            code1.ValueSetId.ShouldBeEquivalentTo(valueSet.ValueSetId);
        }
    }
}