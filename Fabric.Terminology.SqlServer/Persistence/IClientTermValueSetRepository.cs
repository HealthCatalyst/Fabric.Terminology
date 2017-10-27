namespace Fabric.Terminology.SqlServer.Persistence
{
    using System;
    using System.Collections.Generic;

    using CallMeMaybe;

    using Fabric.Terminology.Domain;
    using Fabric.Terminology.Domain.Models;

    public interface IClientTermValueSetRepository
    {
        Attempt<IValueSet> Add(IValueSet valueSet);

        void Delete(IValueSet valueSet);

        Maybe<IValueSet> GetValueSet(Guid valueSetGuid);
    }
}