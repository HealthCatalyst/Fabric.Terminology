namespace Fabric.Terminology.IntegrationTests.Services
{
    using System;
    using System.Collections.Generic;

    using Fabric.Terminology.Domain.Services;
    using Fabric.Terminology.IntegrationTests.Fixtures;
    using Fabric.Terminology.TestsBase;

    using FluentAssertions;

    using Xunit;
    using Xunit.Abstractions;

    public class CodeSystemServiceTests : OutputTestBase, IClassFixture<ServiceFixture>
    {
        private readonly ICodeSystemService codeSystemService;

        public CodeSystemServiceTests(ServiceFixture fixture, ITestOutputHelper output)
            : base(output)
        {
            this.codeSystemService = fixture.CodeSystemService;
        }

        [Fact]
        public void GetAll()
        {
            // Arrange

            // Act
            var codeSystems = this.codeSystemService.GetAll();

            // Assert
            codeSystems.Should().NotBeEmpty();
        }

        [Fact]
        public void GetAllSpecific()
        {
            // Arrange
            var codeSystemGuids = new List<Guid>
            {
                Guid.Parse("87F53B39-2EDF-4045-82CF-93010055A5B8"), // ICD10CM Diagnosis
                Guid.Parse("E7681CF9-7152-41CF-A6D6-1AFB5B46D07B") // ICD10PCS Procedure
            };

            // Act
            var codeSystems = this.codeSystemService.GetAll(codeSystemGuids.ToArray());

            // Assert
            codeSystems.Should().NotBeEmpty();
            codeSystems.Count.Should().Be(2);  // FYI this fails due to error in the view where duplicated CodeSystemGuid(s) exist
        }

        [Theory]
        [InlineData("87F53B39-2EDF-4045-82CF-93010055A5B8")]
        [InlineData("E7681CF9-7152-41CF-A6D6-1AFB5B46D07B")]
        [InlineData("94E16A71-2F13-4023-81A5-090DFD0A4D8F")]
        [InlineData("9C78E8C1-71C4-43E4-B729-8809B4785431")]
        [InlineData("572BC495-9648-4D5E-89C5-F80037082212")]
        [InlineData("03C5FB1C-7567-4C0D-90AD-E2DF6F2CE76F")]
        public void GetCodeSystem(string codeSystemUniqueId)
        {
            // Arrange
            var codeSystemGuid = Guid.Parse(codeSystemUniqueId);

            // Act
            var maybe = this.codeSystemService.GetCodeSystem(codeSystemGuid);

            // Assert
            maybe.HasValue.Should().BeTrue();
        }
    }
}
