namespace Fabric.Terminology.SqlServer.Persistence
{
    using System;
    using System.Collections.Generic;

    using CallMeMaybe;

    using Fabric.Terminology.Domain;
    using Fabric.Terminology.Domain.Models;

    internal interface IClientTermValueSetRepository
    {
        Maybe<IValueSet> GetValueSet(Guid valueSetGuid);

        Attempt<IValueSet> Add(IValueSet valueSet);

        Attempt<IValueSet> AddRemoveCodes(
            Guid valueSetGuid,
            IReadOnlyCollection<ICodeSystemCode> codesToAdd,
            IReadOnlyCollection<ICodeSystemCode> codesToRemove);

        Attempt<IValueSet> ChangeStatus(Guid valueSetGuid, ValueSetStatus newStatus);

        void Delete(IValueSet valueSet);
    }
}
