namespace Fabric.Terminology.SqlServer.Persistence.Ordering
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Fabric.Terminology.SqlServer.Models.Dto;
    using Fabric.Terminology.SqlServer.Persistence.DataContext;

    internal class OrderingStrategyFactory : IOrderingStrategyFactory
    {
        public IOrderingStrategy<ValueSetDescriptionDto> GetValueSetStrategy(SharedContext sharedContext, IReadOnlyCollection<Guid> codeSystemGuids)
        {
            return new ValueSetBackingItemOrderingStrategy(sharedContext, codeSystemGuids);
        }

        public IOrderingStrategy<ValueSetCodeDto> GetValueSetCodeStrategy()
        {
            return new ValueSetCodeOrderingStrategy();
        }
    }
}