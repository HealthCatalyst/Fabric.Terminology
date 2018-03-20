namespace Fabric.Terminology.SqlServer.Persistence.Ordering
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Fabric.Terminology.SqlServer.Models.Dto;
    using Fabric.Terminology.SqlServer.Persistence.DataContext;

    internal interface IOrderingStrategyFactory
    {
        IOrderingStrategy<ValueSetDescriptionDto> GetValueSetStrategy(
            SharedContext sharedContext,
            IReadOnlyCollection<Guid> codeSystemGuids);

        IOrderingStrategy<ValueSetCodeDto> GetValueSetCodeStrategy();
    }
}