namespace Fabric.Terminology.UnitTests.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Fabric.Terminology.Domain.Services;
    using Fabric.Terminology.SqlServer;
    using Fabric.Terminology.SqlServer.Persistence;
    using Fabric.Terminology.TestsBase.Mocks;

    using FluentAssertions;

    using Nancy.Diagnostics;

    using Xunit;
    using Fabric.Terminology.Domain;

    public class SqlServerLibraryTests
    {
        internal static IEnumerable<PersistenceOperation> TestOperations => new List<PersistenceOperation>
        {
            new PersistenceOperation { Value = new TestObject(), OperationType = OperationType.Create },
            new PersistenceOperation { Value = new TestObject(), OperationType = OperationType.Update },
            new PersistenceOperation { Value = new TestObject(), OperationType = OperationType.Update },
            new PersistenceOperation { Value = new TestObject(), OperationType = OperationType.Delete },
            new PersistenceOperation { Value = new TestObject(), OperationType = OperationType.None },
            new PersistenceOperation { Value = new NotTestObject(), OperationType = OperationType.None },
            new PersistenceOperation { Value = new NotTestObject(), OperationType = OperationType.Create },
            new PersistenceOperation { Value = new NotTestObject(), OperationType = OperationType.Update },
            new PersistenceOperation { Value = new NotTestObject(), OperationType = OperationType.Delete },
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
