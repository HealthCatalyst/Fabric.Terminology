namespace Fabric.Terminology.SqlServer.Persistence
{
    using Fabric.Terminology.Domain;
    using Fabric.Terminology.Domain.Models;

    public interface IClientTermValueSetRepository
    {
        Attempt<IValueSet> Add(IValueSet valueSet);

        void Delete(IValueSet valueSet);
    }
}