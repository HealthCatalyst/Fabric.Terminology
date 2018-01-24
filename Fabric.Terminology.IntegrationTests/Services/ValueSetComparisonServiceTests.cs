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

    public class ValueSetComparisonServiceTests : OutputTestBase, IClassFixture<ValueSetComparisonServiceFixture>
    {
        private readonly IValueSetComparisonService valueSetComparisonService;

        public ValueSetComparisonServiceTests(ValueSetComparisonServiceFixture fixture, ITestOutputHelper output)
            : base(output)
        {
            this.valueSetComparisonService = fixture.ValueSetComparisonService;
        }

        [Theory]
        [InlineData("A10BE32F-A086-41E2-B14F-9724E5D9DC29", "A10BE32F-A086-41E2-B14F-9724E5D9DC29")]
        public void ValueSetComparisonServiceThrowsArgumentException(string key1, string key2)
        {
            // Arrange
            var valueSetGuids = new List<Guid> { Guid.Parse(key1), Guid.Parse(key2) };

            // Act
            var task = this.valueSetComparisonService.CompareValueSetCodes(valueSetGuids);

            var taskException = Record.ExceptionAsync(async () => await task);

            // Assert
            taskException.Should().NotBeNull();
            taskException.Result.Should().NotBeNull();

            var ex = taskException.Result;
            ex.GetType().Should().Be<ArgumentException>();
        }

        [Theory]
        [InlineData("Comparing one value ValueSetGUID and one invalid", "A10BE32F-A086-41E2-B14F-9724E5D9DC29", "A4DBC992-D7FD-4ADD-9F3D-0028864AC61B")]
        [InlineData("Comparing two invalid ValueSetGUID", "A10BE32F-A086-41E2-B14F-9724E5D9DC29", "A4DBC992-D7FD-4ADD-9F3D-0028864AC61B")]
        public void ValueSetComparisonServiceThrowsInvalidOperationException(string description, string key1, string key2)
        {
            // Arrange
            Console.WriteLine(description);
            var valueSetGuids = new List<Guid> { Guid.Parse(key1), Guid.Parse(key2) };

            // Act
            var task = this.valueSetComparisonService.CompareValueSetCodes(valueSetGuids);

            var taskException = Record.ExceptionAsync(async () => await task);

            // Assert
            taskException.Should().NotBeNull();
            taskException.Result.Should().NotBeNull();

            var ex = taskException.Result;
            ex.GetType().Should().Be<InvalidOperationException>();
        }

        [Theory]
        [InlineData("Abdominal Aortic Aneurysm", "bebbf8d1-b4ca-495e-96dd-ed4e2dc32d69", "Abdominal Hysterectomy", "95af1a63-6787-443d-a73a-28b8ba15c8db")]
        public void CanCompareTwoValueSets(string vsName1, string key1, string vsName2, string key2)
        {
            // Arrange
            Console.WriteLine($"Comparing '{vsName1}' to '{vsName2}'");
            var valueSetGuids = new List<Guid> { Guid.Parse(key1), Guid.Parse(key2) };

            // Act
            var comparison = this.Profiler.ExecuteTimed(
                async () => await this.valueSetComparisonService.CompareValueSetCodes(valueSetGuids));

            // Assert
            comparison.Should().NotBeNull();
        }
    }
}