namespace Fabric.Terminology.SqlServer.Persistence.UnitOfWork
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using CallMeMaybe;

    using Fabric.Terminology.Domain.Exceptions;
    using Fabric.Terminology.Domain.Persistence;
    using Fabric.Terminology.SqlServer.Models.Dto;
    using Fabric.Terminology.SqlServer.Persistence.DataContext;

    using Microsoft.EntityFrameworkCore;

    using Serilog;

    internal class ClientTermUnitOfWork : IClientTermUnitOfWork
    {
        private readonly Lazy<ClientTermContext> context;

        private readonly ILogger logger;

        private Lazy<ICrudRepository<ValueSetCodeDto>> valueSetCode;

        private Lazy<ICrudRepository<ValueSetCodeCountDto>> valueSetCodeCount;

        private Lazy<ICrudRepository<ValueSetDescriptionBaseDto>> valueSetDescription;

        public ClientTermUnitOfWork(
            Lazy<ClientTermContext> context,
            ILogger logger)
        {
            this.context = context;
            this.logger = logger;
            this.Initialize();
        }

        public ClientTermContext Context => this.context.Value;

        internal ICrudRepository<ValueSetDescriptionBaseDto> ValueSetDescriptions => this.valueSetDescription.Value;

        internal ICrudRepository<ValueSetCodeDto> ValueSetCodes => this.valueSetCode.Value;

        internal ICrudRepository<ValueSetCodeCountDto> ValueSetCodeCounts => this.valueSetCodeCount.Value;

        public void Commit(Queue<Operation> operations)
        {
            var expectedOperations = operations.Count;

            this.BatchLoad(operations);

            var ctx = this.context.Value;
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
                    this.logger.Error(ex, "Failed to commit unit of work transaction. Rolling back.");
                    transaction.Rollback();
                    throw;
                }
                finally
                {
                    ctx.ChangeTracker.AutoDetectChangesEnabled = true;
                }
            }
        }

        public Maybe<ValueSetDescriptionBaseDto> GetValueSetDescriptionDto(Guid valueSetGuid) =>
            Maybe.From(this.ValueSetDescriptions.Single(vsd => vsd.ValueSetGUID == valueSetGuid));

        public IReadOnlyCollection<ValueSetCodeDto> GetCodeDtos(Guid valueSetGuid) =>
            this.ValueSetCodes.Get(vsc => vsc.ValueSetGUID == valueSetGuid);

        public IReadOnlyCollection<ValueSetCodeCountDto> GetCodeCountDtos(Guid valueSetGuid) =>
            this.ValueSetCodeCounts.Get(vscc => vscc.ValueSetGUID == valueSetGuid);

        private void BatchLoad(Queue<Operation> operations)
        {
            while (operations.Count > 0)
            {
                var op = operations.Dequeue();
                switch (op.Value.GetType())
                {
                    case Type vsd when vsd == typeof(ValueSetDescriptionBaseDto):
                        this.ProcessOperation(this.ValueSetDescriptions, op);
                        break;
                    case Type vsc when vsc == typeof(ValueSetCodeDto):
                        this.ProcessOperation(this.ValueSetCodes, op);
                        break;
                    case Type vscc when vscc == typeof(ValueSetCodeCountDto):
                        this.ProcessOperation(this.ValueSetCodeCounts, op);
                        break;
                    default:
                        var invalid =
                            new InvalidOperationException($"ClientTermUnitOfWork cannot process an operation with value of type {op.Value.GetType().FullName}");
                        this.logger.Error(invalid, "Could not process operation");
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
                this.logger.Error(e, "Failed to process unit of work operation.");
                throw;
            }
        }

        private void Initialize()
        {
            this.valueSetDescription = new Lazy<ICrudRepository<ValueSetDescriptionBaseDto>>(
                () => new SqlCrudRepository<ValueSetDescriptionBaseDto>(this.context.Value, this.logger));

            this.valueSetCode = new Lazy<ICrudRepository<ValueSetCodeDto>>(
                () => new SqlCrudRepository<ValueSetCodeDto>(this.context.Value, this.logger));

            this.valueSetCodeCount = new Lazy<ICrudRepository<ValueSetCodeCountDto>>(
                () => new SqlCrudRepository<ValueSetCodeCountDto>(this.context.Value, this.logger));
        }
    }
}