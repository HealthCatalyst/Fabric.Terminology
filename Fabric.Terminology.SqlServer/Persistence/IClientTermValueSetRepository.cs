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

        Attempt<IValueSet> Update(IValueSet valueSet);

        void Delete(IValueSet valueSet);

        Maybe<IValueSet> GetValueSet(Guid valueSetGuid);

        void AddCodes(Guid valueSetGuid, IEnumerable<ICodeSystemCode> codeSystemCodes);

        void RemoveCodes(Guid valueSetGuid, IEnumerable<Guid> codeGuids);
    }
}