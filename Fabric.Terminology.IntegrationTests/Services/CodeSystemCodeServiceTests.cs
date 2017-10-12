namespace Fabric.Terminology.IntegrationTests.Services
{
    using System;

    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.Domain.Services;
    using Fabric.Terminology.IntegrationTests.Fixtures;
    using Fabric.Terminology.TestsBase;

    using FluentAssertions;

    using Xunit;
    using Xunit.Abstractions;

    public class CodeSystemCodeServiceTests : OutputTestBase, IClassFixture<ServiceFixture>
    {
        private readonly ICodeSystemCodeService codeSystemCodeService;

        public CodeSystemCodeServiceTests(ServiceFixture fixture, ITestOutputHelper output)
            : base(output)
        {
            this.codeSystemCodeService = fixture.CodeSystemCodeService;
        }

        [Theory]
        [InlineData("31E98653-4DD5-4A31-9CDC-CCA74CEFF74B")]
        [InlineData("31EA3D79-D9CE-4E21-AC24-EB6F66FF0B3B")]
        [InlineData("31EBB934-6DCE-4FC6-A5FB-76071B4DB2DC")]
        public void GetCodeSystemCode(string codeUniqueId)
        {
            // Arrange
            var codeGuid = Guid.Parse(codeUniqueId);

            // Act
            var maybe = this.codeSystemCodeService.GetCodeSystemCode(codeGuid);

            // Assert
            maybe.HasValue.Should().BeTrue();
        }
    }
}
