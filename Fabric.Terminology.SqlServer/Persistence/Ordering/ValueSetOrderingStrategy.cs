namespace Fabric.Terminology.SqlServer.Persistence.Ordering
{
    using System.Linq;

    using Fabric.Terminology.Domain.Persistence.Querying;
    using Fabric.Terminology.SqlServer.Models.Dto;
    using Fabric.Terminology.SqlServer.Persistence.DataContext;

    internal class ValueSetOrderingStrategy : IOrderingStrategy<ValueSetDescriptionDto>
    {
        private readonly SharedContext sharedContext;

        public ValueSetOrderingStrategy(SharedContext sharedContext)
        {
            this.sharedContext = sharedContext;
        }

        public IQueryable<ValueSetDescriptionDto> SetOrdering(IQueryable<ValueSetDescriptionDto> query, IOrderingParameters parameters)
        {
            throw new System.NotImplementedException();
        }
    }
}