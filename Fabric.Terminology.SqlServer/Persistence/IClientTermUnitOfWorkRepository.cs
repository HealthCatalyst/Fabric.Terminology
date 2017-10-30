namespace Fabric.Terminology.SqlServer.Persistence
{
    using System;

    using CallMeMaybe;

    using Fabric.Terminology.Domain;
    using Fabric.Terminology.Domain.Models;

    internal interface IClientTermUnitOfWorkRepository
    {
        Maybe<IValueSet> GetValueSet(Guid valueSetGuid);

        Attempt<IValueSet> Add(IValueSet valueSet);

        void Delete(IValueSet valueSet);
    }
}
