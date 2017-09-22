namespace Fabric.Terminology.SqlServer.Persistence
{
    using Fabric.Terminology.Domain;
    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.Domain.Persistence;
    internal class SqlClientTermValueSetRepository : IClientTermValueSetRepository
    {
        public Attempt<IValueSet> Add(IValueSet valueSet)
        {
            throw new System.NotImplementedException();
        }

        public void Delete(IValueSet valueSet)
        {
            throw new System.NotImplementedException();
        }
    }
}