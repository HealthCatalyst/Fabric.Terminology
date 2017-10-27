namespace Fabric.Terminology.UnitTests.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Fabric.Terminology.SqlServer;
    using Fabric.Terminology.SqlServer.Persistence;
    using Fabric.Terminology.TestsBase.Mocks;

    using FluentAssertions;

    using Nancy.Diagnostics;

    using Xunit;

    public class SqlServerLibraryTests
    {
        internal static IEnumerable<RepositoryOperation> TestOperations => new List<RepositoryOperation>
        {
            new RepositoryOperation { Value = new TestObject(), OperationType = OperationType.Create },
            new RepositoryOperation { Value = new TestObject(), OperationType = OperationType.Update },
            new RepositoryOperation { Value = new TestObject(), OperationType = OperationType.Update },
            new RepositoryOperation { Value = new TestObject(), OperationType = OperationType.Delete },
            new RepositoryOperation { Value = new TestObject(), OperationType = OperationType.None },
            new RepositoryOperation { Value = new NotTestObject(), OperationType = OperationType.None },
            new RepositoryOperation { Value = new NotTestObject(), OperationType = OperationType.Create },
            new RepositoryOperation { Value = new NotTestObject(), OperationType = OperationType.Update },
            new RepositoryOperation { Value = new NotTestObject(), OperationType = OperationType.Delete },
        };

        [Theory]
        [InlineData(typeof(TestObject), 5, 1, 2, 1, 1)]
        [InlineData(typeof(NotTestObject), 4, 1, 1, 1, 1)]
        public void IsOperationOfType(
            Type valueType,
            int typeCount,
            int createCount,
            int updateCount,
            int deleteCouunt,
            int noneCount)
        {
            // Arrange
            var operations = TestOperations;

            // Act
            var ops = operations.IsOperationOfType(valueType);

            // Assert
            ops.Should().NotBeEmpty();
            ops.Count().Should().Be(typeCount);
            ops.Where(op => op.OperationType == OperationType.Create).Count().Should().Be(createCount);
            ops.Where(op => op.OperationType == OperationType.Update).Count().Should().Be(updateCount);
            ops.Where(op => op.OperationType == OperationType.Delete).Count().Should().Be(deleteCouunt);
            ops.Where(op => op.OperationType == OperationType.None).Count().Should().Be(noneCount);
        }

        private class NotTestObject
        {
            public string Text { get; set; }
        }
    }
}
