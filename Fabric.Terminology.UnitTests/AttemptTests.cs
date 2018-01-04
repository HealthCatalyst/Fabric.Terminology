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
            attempt.Exception.Should().BeNull();
            attempt.Result.Should().BeEquivalentTo(obj);
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
            attempt.Exception.Should().NotBeNull();
            attempt.Exception.Should().BeEquivalentTo(argEx);
            attempt.Result.Should().BeNull();
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
            attempt.Exception.Should().NotBeNull();
            attempt.Result.Should().NotBeNull();
            attempt.Exception.Should().BeEquivalentTo(argEx);
            attempt.Result.Should().BeSameAs(obj);
        }
    }
}