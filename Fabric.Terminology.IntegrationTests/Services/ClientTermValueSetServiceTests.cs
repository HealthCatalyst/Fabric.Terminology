﻿namespace Fabric.Terminology.IntegrationTests.Services
{
    using System;
    using System.Linq;

    using Fabric.Terminology.API;
    using Fabric.Terminology.Domain;
    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.Domain.Services;
    using Fabric.Terminology.IntegrationTests.Fixtures;
    using Fabric.Terminology.TestsBase;
    using Fabric.Terminology.TestsBase.Mocks;

    using FluentAssertions;

    using Xunit;
    using Xunit.Abstractions;

    public class ClientTermValueSetServiceTests : OutputTestBase, IClassFixture<SqlServiceFixture>
    {
        private readonly IValueSetService valueSetService;

        private readonly IClientTermValueSetService clientTermValueSetService;

        public ClientTermValueSetServiceTests(SqlServiceFixture fixture, ITestOutputHelper output)
            : base(output)
        {
            this.clientTermValueSetService = fixture.ClientTermValueSetService;
            this.valueSetService = fixture.ValueSetService;
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
            this.Profiler.ExecuteTimed(() => this.clientTermValueSetService.SaveAsNew(vs));

            // Assert
            vs.ValueSetGuid.Should().NotBe(Guid.Empty);
            vs.ValueSetCodes.Count.Should().Be(codeCount);
            vs.Name.Should().BeEquivalentTo(name);
            vs.IsLatestVersion.Should().BeTrue();
            vs.IsCustom.Should().BeTrue();
            vs.StatusCode.Should().Be(ValueSetStatus.Draft);

            // cleanup
            this.clientTermValueSetService.Delete(vs);
        }

        [Theory]
        [InlineData("35F6B1A6-A72B-48F5-B319-F6CCAF15734D", "Diabetes copy")] // bsql -> 2.4694085
        [InlineData("A2216AAC-8513-43D8-85C2-00057F92394B", "Hypotension copy")] // bsql ->  0.2887891
        [InlineData("A10BE32F-A086-41E2-B14F-9724E5D9DC29", "Trauma copy")] // bsql -> 48.5546188 seconds
        public void CanCopyValueSet(string valueSetReferenceId, string name)
        {
            // Arrange
            var valueSetGuid = Guid.Parse(valueSetReferenceId);
            var valueSet = this.valueSetService.GetValueSet(valueSetGuid).Single();
            var meta = MockApiModelBuilder.ValueSetCreationApiModel(name, 1) as IValueSetMeta;

            // Act
            var attempt = this.Profiler.ExecuteTimed(() => this.clientTermValueSetService.Copy(valueSet, name, meta));

            // Assert
            attempt.Success.Should().BeTrue();
            var copy = attempt.Result.Single();
            copy.OriginGuid.Should().Be(valueSet.ValueSetGuid);
            copy.ValueSetCodes.Count.Should().Be(valueSet.ValueSetCodes.Count);
            copy.IsCustom.Should().BeTrue();
            copy.IsLatestVersion.Should().BeTrue();
            copy.StatusCode.Should().Be(ValueSetStatus.Draft);

            // cleanup
           this.clientTermValueSetService.Delete(copy);
        }
    }
}
