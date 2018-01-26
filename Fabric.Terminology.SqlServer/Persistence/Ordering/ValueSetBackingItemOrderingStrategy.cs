namespace Fabric.Terminology.SqlServer.Persistence.Ordering
{
    using System.Linq;

    using Fabric.Terminology.Domain.Persistence.Querying;
    using Fabric.Terminology.SqlServer.Models.Dto;

    internal class ValueSetBackingItemOrderingStrategy : OrderingStrategyBase<ValueSetDescriptionDto>
    {
        protected override IQueryable<ValueSetDescriptionDto> PerformOrdering(IQueryable<ValueSetDescriptionDto> query, IOrderingParameters parameters)
        {
            throw new System.NotImplementedException();
        }
    }
}