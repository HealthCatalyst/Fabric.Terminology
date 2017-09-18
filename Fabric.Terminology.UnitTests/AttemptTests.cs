namespace Fabric.Terminology.UnitTests
{
    using System;

    using Fabric.Terminology.Domain;
    using Fabric.Terminology.TestsBase.Mocks;

    using FluentAssertions;

    using Xunit;

    public class AttemptTests
    {
        [Fact]
        public void CanReturnSuccessfulAttempt()
        {
            // Arrange
            var dt = DateTime.Today;
            var obj = new TestObject { Text = "attempt", Stamp = dt };

            // Act
            var attempt = Attempt<TestObject>.Successful(obj);

            // Assert
            attempt.Success.Should().BeTrue();
            attempt.Exception.HasValue.Should().BeFalse();
            attempt.Result.Single().Should().BeEquivalentTo(obj);
        }

        [Fact]
        public void CanReturnFailedAttemptWithoutResult()
        {
            // Arrange
            var argEx = new ArgumentException("test");

            // Act
            var attempt = Attempt<TestObject>.Failed(argEx);

            // Assert
            attempt.Success.Should().BeFalse();
            attempt.Exception.HasValue.Should().BeTrue();
            attempt.Exception.Single().Should().BeEquivalentTo(argEx);
            attempt.Result.HasValue.Should().BeFalse();
        }

        [Fact]
        public void CanReturnFailedAttemptWithResult()
        {
            // Arrange
            var dt = DateTime.Today;
            var obj = new TestObject { Text = "attempt", Stamp = dt };
            var argEx = new ArgumentException("test");

            // Act
            var attempt = Attempt<TestObject>.Failed(argEx, obj);

            // Assert
            attempt.Success.Should().BeFalse();
            attempt.Exception.HasValue.Should().BeTrue();
            attempt.Result.HasValue.Should().BeTrue();
            attempt.Exception.Single().Should().BeEquivalentTo(argEx);
            attempt.Result.Single().Should().BeSameAs(obj);
        }
    }
}