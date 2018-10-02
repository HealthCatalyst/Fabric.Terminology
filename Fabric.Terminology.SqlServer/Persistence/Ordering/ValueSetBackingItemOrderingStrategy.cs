namespace Fabric.Terminology.SqlServer.Persistence.Ordering
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    using Fabric.Terminology.Domain;
    using Fabric.Terminology.Domain.Persistence.Querying;
    using Fabric.Terminology.SqlServer.Models.Dto;
    using Fabric.Terminology.SqlServer.Persistence.DataContext;

#pragma warning disable CA1308 // Normalize strings to uppercase
    internal class ValueSetBackingItemOrderingStrategy : OrderingStrategyBase<ValueSetDescriptionDto>
    {
        private const string CodeCountFieldName = "codecount";

        private readonly SharedContext sharedContext;

        private readonly IReadOnlyCollection<Guid> codeSystemGuids;

        public ValueSetBackingItemOrderingStrategy(
            SharedContext sharedContext,
            IReadOnlyCollection<Guid> codeSystemGuids)
        {
            this.sharedContext = sharedContext;
            this.codeSystemGuids = codeSystemGuids;
        }

        protected override IQueryable<ValueSetDescriptionDto> PerformOrdering(
            IQueryable<ValueSetDescriptionDto> query,
            IOrderingParameters parameters)
        {
            return this.IsCodeCountOrdering(parameters)
                       ? this.CodeCountFieldOrderBy(query, parameters)
                       : this.ValueSetFieldOrderBy(query, parameters);
        }

        private IQueryable<ValueSetDescriptionDto> CodeCountFieldOrderBy(
            IQueryable<ValueSetDescriptionDto> query,
            IOrderingParameters parameters)
        {
            var countQuery = this.sharedContext.ValueSetCounts.AsQueryable();
            if (this.codeSystemGuids.Any())
            {
                countQuery = countQuery.Where(cc => this.codeSystemGuids.Contains(cc.CodeSystemGUID));
            }

            var countsDtos = countQuery.GroupBy(cc => cc.ValueSetGUID)
                .Select(group => new CodeCountResultDto
                {
                    ValueSetGuid = group.Key,
                    CodeCount = group.Sum(s => s.CodeSystemPerValueSetNBR)
                });

            var combined = query.Join(
                countsDtos,
                vsDto => vsDto.ValueSetGUID,
                countDto => countDto.ValueSetGuid,
                (vsDto, countDto) => new
                {
                    ValueSetDescriptionDto = vsDto,
                    countDto.CodeCount
                });

            return (parameters.Direction == SortDirection.Asc
                          ? combined.OrderBy(c => c.CodeCount)
                          : combined.OrderByDescending(c => c.CodeCount))
                .Select(c => c.ValueSetDescriptionDto);
        }

        private IQueryable<ValueSetDescriptionDto> ValueSetFieldOrderBy(
            IQueryable<ValueSetDescriptionDto> query,
            IOrderingParameters parameters)
        {
            var orderByExpr = this.GetOrderExpression(parameters);
            return parameters.Direction == SortDirection.Asc
                         ? query.OrderBy(orderByExpr)
                         : query.OrderByDescending(orderByExpr);
        }

        private bool IsCodeCountOrdering(IOrderingParameters parameters) =>
            parameters.FieldName.ToLowerInvariant() == CodeCountFieldName;

        private Expression<Func<ValueSetDescriptionDto, object>> GetOrderExpression(IOrderingParameters parameters)
        {
            switch (parameters.FieldName.ToLowerInvariant())
            {
                case "valuesetreferenceid":
                    return dto => dto.ValueSetReferenceID;
                case "sourcedescription":
                    return dto => dto.SourceDSC;
                case "versiondate":
                    return dto => dto.VersionDTS;
                case "authoritydescription":
                    return dto => dto.AuthorityDSC;
                case "lastmodifieddate":
                    return dto => dto.LastModifiedDTS;
                case "name":
                default:
                    return dto => dto.ValueSetNM;
            }
        }

        private class CodeCountResultDto
        {
            public Guid ValueSetGuid { get; set; }

            public int CodeCount { get; set; }
        }
    }
#pragma warning restore CA1308 // Normalize strings to uppercase
}