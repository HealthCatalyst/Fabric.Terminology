namespace Fabric.Terminology.IntegrationTests.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

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
            ex.Should().BeOfType<ArgumentException>();
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
            ex.Should().BeOfType<InvalidOperationException>();
        }

        [Theory]
        [InlineData("Obstetrics", "CE787E4B-2588-41CF-9377-1DB9CBDBD223", "Pregnancy", "E27C0B63-6335-44BA-9629-24C64733AA15", 5252, 3642)]
        [InlineData("Substance Abuse", "FDA1B3D5-28C7-4148-9F6B-08D4DF68709D", "Alcohol and Drug Dependence", "989DA48C-0794-41E7-AAE7-61F1C490655D", 997, 700)]
        public void CanCompareTwoValueSets(string vsName1, string key1, string vsName2, string key2, int totalCodes, int expectedAggregate)
        {
            // Arrange
            Console.WriteLine($"Comparing '{vsName1}' to '{vsName2}'");
            var valueSetGuids = new List<Guid> { Guid.Parse(key1), Guid.Parse(key2) };

            // Act
            var comparison = this.Profiler.ExecuteTimed(
                async () => await this.valueSetComparisonService.CompareValueSetCodes(valueSetGuids));

            // Assert
            comparison.Should().NotBeNull();
            comparison.Compared.Should().NotBeEmpty();

            var actualTotalCodeCount =
                comparison.Compared.SelectMany(vss => vss.CodeCounts.Select(cc => cc.CodeCount)).Sum();

            actualTotalCodeCount.Should().Be(totalCodes);

            comparison.AggregateCodeCount.Should().Be(expectedAggregate);
        }
    }
}