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

        void PrepareAddCodesOperations(
            Guid valueSetGuid,
            IEnumerable<IValueSetCode> valueSetCodes);

        void PrepareRemoveCodesOperations(
            Guid valueSetGuid,
            IEnumerable<ICodeSystemCode> codeSystemCodes);

        void Delete(IValueSet valueSet);
    }
}
