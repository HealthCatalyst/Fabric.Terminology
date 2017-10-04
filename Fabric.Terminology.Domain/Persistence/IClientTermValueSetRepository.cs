namespace Fabric.Terminology.Domain.Persistence
{
    using System;

    using CallMeMaybe;

    using Fabric.Terminology.Domain.Models;

    public interface IClientTermValueSetRepository
    {
        Attempt<IValueSet> Add(IValueSet valueSet);

        void Delete(IValueSet valueSet);

        Maybe<IValueSet> GetValueSet(Guid valueSetGuid);
    }
}