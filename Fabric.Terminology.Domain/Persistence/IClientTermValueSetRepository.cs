namespace Fabric.Terminology.Domain.Persistence
{
    using Fabric.Terminology.Domain.Models;

    public interface IClientTermValueSetRepository
    {
        Attempt<IValueSet> Add(IValueSet valueSet);

        void Delete(IValueSet valueSet);
    }
}