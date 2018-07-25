namespace Fabric.Terminology.UnitTests.Configuration
{
    using System;

    using Fabric.Terminology.API.Configuration;
    using Fabric.Terminology.TestsBase;
    using Fabric.Terminology.TestsBase.Models;

    using FluentAssertions;

    using JetBrains.Annotations;

    using Xunit;
    using Xunit.Abstractions;

    public class TerminologyVersionTests : OutputTestBase
    {
        private readonly VersionConfig versionConfig;

        public TerminologyVersionTests([NotNull] ITestOutputHelper output)
            : base(output)
        {
            this.versionConfig = TerminologyTestHelper.GetVersionJson();
        }

        [Fact]
        public void CanReadCurrentVersion()
        {
            // Arrange
            var expected = new Version(this.versionConfig.Version);

            // Act
            var current = TerminologyVersion.Current;

            // Assert
            current.Should().NotBeNull();
            current.Major.Should().Be(expected.Major);
            current.Minor.Should().Be(expected.Minor);
            current.Build.Should().Be(expected.Build);
        }

        [Fact]
        public void CanReadSemVersion()
        {
            TerminologyVersion.SemanticVersion.Should().NotBeNull();
            this.Output.WriteLine(TerminologyVersion.SemanticVersion.ToString());
        }
    }
}