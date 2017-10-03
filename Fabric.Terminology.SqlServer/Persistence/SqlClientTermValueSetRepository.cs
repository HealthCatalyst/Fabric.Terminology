namespace Fabric.Terminology.SqlServer.Persistence
{
    using System;
    using System.Linq;

    using CallMeMaybe;

    using Fabric.Terminology.Domain;
    using Fabric.Terminology.Domain.Exceptions;
    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.Domain.Persistence;
    using Fabric.Terminology.SqlServer.Models.Dto;
    using Fabric.Terminology.SqlServer.Persistence.DataContext;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Storage;

    using Serilog;

    using ValueSet = Fabric.Terminology.SqlServer.Models.Dto.ValueSet;

    internal class SqlClientTermValueSetRepository : IClientTermValueSetRepository
    {
        private readonly ClientTermContext clientTermContext;

        private readonly ILogger logger;

        public SqlClientTermValueSetRepository(ClientTermContext clientTermContext, ILogger logger)
        {
            this.clientTermContext = clientTermContext;
            this.logger = logger;
        }

        public Maybe<IValueSet> GetValueSet(Guid valueSetGuid)
        {
            var desc = this.clientTermContext.ValueSetDescriptions.AsNoTracking()
                .SingleOrDefault(x => x.ValueSetGUID == valueSetGuid);

            if (desc == null)
            {
                return Maybe.Not;
            }

            var codes = this.clientTermContext.ValueSetCodes.Where(vsc => vsc.ValueSetGUID == valueSetGuid).ToList();

            return new Maybe<IValueSet>(new ValueSet(desc, codes));
        }

        public Attempt<IValueSet> Add(IValueSet valueSet)
        {
            return this.AttemptAdd(valueSet, t => t.Rollback());
        }

        public void Delete(IValueSet valueSet)
        {
            throw new NotImplementedException($"Deleting an {nameof(IValueSet)} is not supported yet.");
        }

        public void Save(IValueSet valueSet)
        {
            if (valueSet.ValueSetGuid == Guid.Empty)
            {
                this.AttemptAdd(valueSet, t => t.Commit());
            }
            else
            {
                throw new NotImplementedException($"Updating an {nameof(IValueSet)} is not supported yet.");
            }
        }

        private Attempt<IValueSet> AttemptAdd(IValueSet valueSet, Action<IDbContextTransaction> transactionChangeAction)
        {
            valueSet.SetIdsForCustomInsert();

            var valueSetDto = new ValueSetDescriptionDto(valueSet);
            var codeDtos = valueSet.ValueSetCodes.Select(code => new ValueSetCodeDto(code)).ToList();

            this.clientTermContext.ChangeTracker.AutoDetectChangesEnabled = false;
            using (var transaction = this.clientTermContext.Database.BeginTransaction())
            {
                try
                {
                    this.clientTermContext.ValueSetDescriptions.Add(valueSetDto);
                    this.clientTermContext.ValueSetCodes.AddRange(codeDtos);
                    var changes = this.clientTermContext.SaveChanges();

                    var expectedChanges = codeDtos.Count + 1;
                    if (changes != expectedChanges)
                    {
                        return Attempt<IValueSet>.Failed(
                            new ValueSetNotFoundException(
                                $"When saving a ValueSet, we expected {expectedChanges} changes, but was told there were {changes} changes"));
                    }

                    // Get the updated ValueSet
                    var added = this.GetValueSet(valueSetDto.ValueSetGUID);

                    transactionChangeAction(transaction);

                    return added.Select(Attempt<IValueSet>.Successful)
                        .Else(
                            () => Attempt<IValueSet>.Failed(
                                new ValueSetNotFoundException("Could not retrieved newly saved ValueSet")));
                }
                catch (Exception ex)
                {
                    this.logger.Error(ex, "Failed to save a custom ValueSet");
                    this.clientTermContext.ChangeTracker.AutoDetectChangesEnabled = true;
                    return Attempt<IValueSet>.Failed(
                        new ValueSetOperationException("Failed to save a custom ValueSet", ex),
                        valueSet);
                }
                finally
                {
                    this.clientTermContext.ChangeTracker.AutoDetectChangesEnabled = true;
                }
            }
        }
    }
}