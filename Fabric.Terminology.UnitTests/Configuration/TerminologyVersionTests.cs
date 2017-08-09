namespace Fabric.Terminology.UnitTests.Configuration
{
    using System;
    using System.Reflection;

    using Fabric.Terminology.API.Configuration;
    using Fabric.Terminology.TestsBase;

    using FluentAssertions;

    using JetBrains.Annotations;

    using Xunit;
    using Xunit.Abstractions;

    public class TerminologyVersionTests : OutputTestBase
    {
        public TerminologyVersionTests([NotNull] ITestOutputHelper output)
            : base(output)
        {
        }

        [Fact]
        public void CanReadCurrentVersion()
        {
            // Arrange

            // Act
            var current = TerminologyVersion.Current;

            // Assert
            current.Should().NotBeNull();
        }

        [Fact]
        public void CanReadSemVersion()
        {
            TerminologyVersion.SemanticVersion.Should().NotBeNull();
            this.Output.WriteLine(TerminologyVersion.SemanticVersion.ToString());
        }
    }
}