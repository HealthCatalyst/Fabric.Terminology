namespace Fabric.Terminology.SqlServer.Persistence
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using CallMeMaybe;

    using Fabric.Terminology.Domain;
    using Fabric.Terminology.Domain.Exceptions;
    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.SqlServer.Models.Dto;
    using Fabric.Terminology.SqlServer.Persistence.DataContext;
    using Fabric.Terminology.SqlServer.Persistence.Factories;

    using Microsoft.EntityFrameworkCore;

    using Serilog;

    internal class SqlClientTermValueSetRepository : IClientTermValueSetRepository
    {
        private readonly Lazy<ClientTermContext> clientTermContext;

        private readonly ILogger logger;

        public SqlClientTermValueSetRepository(Lazy<ClientTermContext> clientTermContext, ILogger logger)
        {
            this.clientTermContext = clientTermContext;
            this.logger = logger;
        }

        protected ClientTermContext ClientTermContext => this.clientTermContext.Value;

        public Maybe<IValueSet> GetValueSet(Guid valueSetGuid)
        {
            var desc = this.ClientTermContext.ValueSetDescriptions.AsNoTracking()
                .SingleOrDefault(x => x.ValueSetGUID == valueSetGuid);

            if (desc == null)
            {
                return Maybe.Not;
            }

            var factory = new ValueSetBackingItemFactory();
            var item = factory.Build(desc);

            var codes = this.GetCodes(item.ValueSetGuid);
            var counts = this.GetCodeCounts(item.ValueSetGuid);

            return new Maybe<IValueSet>(new ValueSet(item, codes, counts));
        }

        public Attempt<IValueSet> Add(IValueSet valueSet)
        {
            valueSet.SetIdsForCustomInsert();

            var valueSetDto = new ValueSetDescriptionBaseDto(valueSet);
            var codeDtos = valueSet.ValueSetCodes.Select(code => new ValueSetCodeDto(code)).ToList();
            var countDtos = valueSet.CodeCounts.Select(count => new ValueSetCodeCountDto(count)).ToList();

            this.ClientTermContext.ChangeTracker.AutoDetectChangesEnabled = false;
            using (var transaction = this.ClientTermContext.Database.BeginTransaction())
            {
                try
                {
                    this.ClientTermContext.ValueSetDescriptions.Add(valueSetDto);
                    this.ClientTermContext.ValueSetCodes.AddRange(codeDtos);
                    this.ClientTermContext.ValueSetCodeCounts.AddRange(countDtos);

                    var changes = this.ClientTermContext.SaveChanges();

                    var expectedChanges = codeDtos.Count + countDtos.Count + 1;
                    if (changes != expectedChanges)
                    {
                        transaction.Rollback();
                        return Attempt<IValueSet>.Failed(
                            new ValueSetNotFoundException(
                                $"When saving a ValueSet, we expected {expectedChanges} changes, but was told there were {changes} changes"));
                    }

                    transaction.Commit();

                    // Get the updated ValueSet
                    var added = this.GetValueSet(valueSetDto.ValueSetGUID);

                    return added.Select(Attempt<IValueSet>.Successful)
                        .Else(
                            () => Attempt<IValueSet>.Failed(
                                new ValueSetNotFoundException("Could not retrieved newly saved ValueSet")));
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    this.logger.Error(ex, "Failed to save a custom ValueSet");
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

        public void Delete(IValueSet valueSet)
        {
            using (var transaction = this.ClientTermContext.Database.BeginTransaction())
            try
            {
                this.ClientTermContext.BulkDelete(
                    new[] { typeof(ValueSetDescriptionBaseDto), typeof(ValueSetCodeDto), typeof(ValueSetCodeCountDto) },
                    new Dictionary<string, object>
                    {
                        { nameof(ValueSetDescriptionBaseDto.ValueSetGUID), valueSet.ValueSetGuid }
                    });
                transaction.Commit();
            }
            catch (Exception ex)
            {
                var operationException = new ValueSetOperationException(
                    $"Failed to delete custom ValueSet with ID {valueSet.ValueSetGuid}",
                    ex);
                this.logger.Error(operationException, "Failed to delete custom ValueSet");
                throw operationException;
            }
        }

        private IReadOnlyCollection<IValueSetCode> GetCodes(Guid valueSetGuid)
        {
            var factory = new ValueSetCodeFactory();
            var codes = this.ClientTermContext.ValueSetCodes.Where(vsc => vsc.ValueSetGUID == valueSetGuid).ToList();

            return codes.Select(factory.Build).ToList();
        }

        private IReadOnlyCollection<IValueSetCodeCount> GetCodeCounts(Guid valueSetGuid)
        {
            var factory = new ValueSetCodeCountFactory();
            var counts = this.ClientTermContext.ValueSetCodeCounts.Where(vscc => vscc.ValueSetGUID == valueSetGuid).ToList();

            return counts.Select(factory.Build).ToList();
        }
    }
}