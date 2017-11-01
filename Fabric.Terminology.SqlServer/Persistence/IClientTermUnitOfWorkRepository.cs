namespace Fabric.Terminology.SqlServer.Persistence
{
    using System;
    using System.Collections.Generic;

    using CallMeMaybe;

    using Fabric.Terminology.Domain;
    using Fabric.Terminology.Domain.Models;

    internal interface IClientTermUnitOfWorkRepository
    {
        Maybe<IValueSet> GetValueSet(Guid valueSetGuid);

        Attempt<IValueSet> Add(IValueSet valueSet);

        Attempt<IValueSet> AddSqlBulkCopy(IValueSet valueSet);

        Attempt<IValueSet> AddRemoveCodes(
            Guid valueSetGuid,
            IEnumerable<ICodeSystemCode> codesToAdd,
            IEnumerable<ICodeSystemCode> codesToRemove);

        void Delete(IValueSet valueSet);
    }
}
