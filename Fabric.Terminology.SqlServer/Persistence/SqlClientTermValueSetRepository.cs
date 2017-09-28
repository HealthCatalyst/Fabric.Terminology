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

    using ValueSet = Fabric.Terminology.SqlServer.ValueSet;

    internal class SqlClientTermValueSetRepository : IClientTermValueSetRepository
    {
        private readonly Lazy<ClientTermContext> clientTermContextLazy;

        public SqlClientTermValueSetRepository(ClientTermContextFactory clientTermContextFactory, ILogger logger)
        {
            this.Logger = logger;
            this.clientTermContextLazy = clientTermContextFactory.CreateLazy();
        }

        protected ClientTermContext ClientTermContext => this.clientTermContextLazy.Value;

        protected ILogger Logger { get; }

        public Maybe<IValueSet> GetValueSet(Guid valueSetGuid)
        {
            var desc = this.ClientTermContext.ValueSetDescriptions.AsNoTracking()
                .SingleOrDefault(x => x.ValueSetGUID == valueSetGuid);

            if (desc == null)
            {
                return Maybe.Not;
            }

            var codes = this.ClientTermContext.ValueSetCodes.Where(vsc => vsc.ValueSetGUID == valueSetGuid).ToList();

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

            this.ClientTermContext.ChangeTracker.AutoDetectChangesEnabled = false;
            using (var transaction = this.ClientTermContext.Database.BeginTransaction())
            {
                try
                {
                    this.ClientTermContext.ValueSetDescriptions.Add(valueSetDto);
                    this.ClientTermContext.ValueSetCodes.AddRange(codeDtos);
                    var changes = this.ClientTermContext.SaveChanges();

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
                    this.Logger.Error(ex, "Failed to save a custom ValueSet");
                    this.ClientTermContext.ChangeTracker.AutoDetectChangesEnabled = true;
                    return Attempt<IValueSet>.Failed(
                        new ValueSetOperationException("Failed to save a custom ValueSet", ex),
                        valueSet);
                }
                finally
                {
                    this.ClientTermContext.ChangeTracker.AutoDetectChangesEnabled = true;
                }
            }
        }
    }
}