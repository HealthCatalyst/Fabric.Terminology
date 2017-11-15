namespace Fabric.Terminology.SqlServer.Persistence.UnitOfWork
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Linq;

    using Fabric.Terminology.Domain.Exceptions;
    using Fabric.Terminology.Domain.Persistence;
    using Fabric.Terminology.SqlServer.Models.Dto;

    using Microsoft.EntityFrameworkCore;

    internal partial class ClientTermValueUnitOfWorkManager
    {
        public IUnitOfWork CreateUnitOfWork(IEnumerable<Operation> operations) =>
            new UnitOfWork(this, operations);

        public IUnitOfWork CreateBulkCopyUnitOfWork<T>(IReadOnlyCollection<T> entities)
            where T : class
        {
            return new BulkCopyUnitOfWork<T>(this, entities);
        }

        private class UnitOfWork : IUnitOfWork
        {
            private readonly ClientTermValueUnitOfWorkManager manager;

            private readonly IEnumerable<Operation> operations;

            public UnitOfWork(ClientTermValueUnitOfWorkManager manager, IEnumerable<Operation> operations)
            {
                this.manager = manager;
                this.operations = operations;
            }

            public void Commit()
            {
                var executable = this.operations.Where(op => op.OperationType != OperationType.None).ToList();
                var expectedOperations = executable.Count();

                this.BatchLoad(new Queue<Operation>(executable));

                var ctx = this.manager.Context;
                ctx.ChangeTracker.AutoDetectChangesEnabled = false;
                using (var transaction = ctx.Database.BeginTransaction())
                {
                    try
                    {
                        var changes = ctx.SaveChanges();
                        if (changes != expectedOperations)
                        {
                            var ex = new ValueSetOperationException(
                                $"When saving a ValueSet, we expected {expectedOperations} changes, but was told there were {changes} changes");
                            throw ex;
                        }

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        ctx.ChangeTracker.AutoDetectChangesEnabled = true;
                        this.manager.logger.Error(ex, $"Failed to commit unit of work transaction. Rolling back. Expected operations {expectedOperations}");
                        transaction.Rollback();
                        throw;
                    }
                    finally
                    {
                        ctx.ChangeTracker.AutoDetectChangesEnabled = true;
                    }
                }
            }

            private void BatchLoad(Queue<Operation> ops)
            {
                while (ops.Count > 0)
                {
                    var op = ops.Dequeue();
                    switch (op.Value)
                    {
                        case ValueSetDescriptionBaseDto vsd:
                            this.ProcessOperation(this.manager.ValueSetDescriptions, op);
                            break;
                        case ValueSetCodeDto vsc:
                            this.ProcessOperation(this.manager.ValueSetCodes, op);
                            break;
                        case ValueSetCodeCountDto vscc:
                            this.ProcessOperation(this.manager.ValueSetCodeCounts, op);
                            break;
                        default:
                            var invalid =
                                new InvalidOperationException($"ClientTermValueUnitOfWorkManager cannot process an operation with value of type {op.Value.GetType().FullName}");
                            this.manager.logger.Error(invalid, "Could not process operation");
                            throw invalid;
                    }
                }
            }

            private void ProcessOperation<TEntity>(ICrudRepository<TEntity> repo, Operation op)
                where TEntity : class
            {
                try
                {
                    var value = (TEntity)op.Value;
                    switch (op.OperationType)
                    {
                        case OperationType.Create:
                            repo.Insert(value);
                            break;
                        case OperationType.Update:
                            repo.Update(value);
                            break;
                        case OperationType.Delete:
                            repo.Delete(value);
                            break;
                        case OperationType.None:
                        default:
                            break;
                    }
                }
                catch (Exception e)
                {
                    this.manager.logger.Error(e, "Failed to process unit of work operation.");
                    throw;
                }
            }
        }

        private class BulkCopyUnitOfWork<T> : IUnitOfWork where T : class
        {
            private readonly ClientTermValueUnitOfWorkManager manager;

            private readonly IReadOnlyCollection<T> entities;

            public BulkCopyUnitOfWork(ClientTermValueUnitOfWorkManager manager, IReadOnlyCollection<T> entities)
            {
                this.manager = manager;
                this.entities = entities;
            }

            public void Commit()
            {
                var annotations = this.manager.Context.Model.FindEntityType(typeof(T)).SqlServer();
                var tableName = annotations.TableName;
                var schema = annotations.Schema;
                var destinationTableName = $"[{schema}].[{tableName}]";
                try
                {
                    using (var bulkCopy = new SqlBulkCopy(this.manager.Context.Database.GetDbConnection().ConnectionString, SqlBulkCopyOptions.Default))
                    {
                        bulkCopy.DestinationTableName = destinationTableName;
                        using (var reader = DtoDataReader<T>.Create(this.entities))
                        {
                            bulkCopy.WriteToServer(reader);
                        }
                    }
                }
                catch (Exception e)
                {
                    this.manager.logger.Error(e, "SqlBulkCopy Exception");
                    throw;
                }
            }
        }
    }
}