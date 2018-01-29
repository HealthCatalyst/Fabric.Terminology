namespace Fabric.Terminology.SqlServer.Persistence.Ordering
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;

    using Fabric.Terminology.Domain;
    using Fabric.Terminology.Domain.Persistence.Querying;
    using Fabric.Terminology.SqlServer.Models.Dto;

    internal class ValueSetCodeOrderingStrategy : OrderingStrategyBase<ValueSetCodeDto>
    {
        protected override IQueryable<ValueSetCodeDto> PerformOrdering(IQueryable<ValueSetCodeDto> query, IOrderingParameters parameters)
        {
            var orderExpression = this.GetOrderByExpression(parameters);
            return parameters.Direction == SortDirection.Asc
                         ? query.OrderBy(orderExpression)
                         : query.OrderByDescending(orderExpression);
        }

        private Expression<Func<ValueSetCodeDto, string>> GetOrderByExpression(IOrderingParameters parameters)
        {
            if (parameters.FieldName != "Name")
            {
                throw new ArgumentException("ValueSetCodes may only be ordered by Name");
            }

            return dto => dto.CodeDSC;
        }
    }
}