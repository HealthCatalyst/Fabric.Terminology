namespace Fabric.Terminology.Domain.Services
{
    using System;
    using System.Collections.Generic;

    using Fabric.Terminology.Domain.Models;

    internal interface IClientTermValueSetCustomizationManager
    {
        IReadOnlyCollection<PersistenceOperation> GetRemoveCodesOperations(
            Guid valueSetGuid,
            IEnumerable<ICodeSystemCode> codeSystemCodes);

        IReadOnlyCollection<PersistenceOperation> GetAddCodesOperations(
            Guid valueSetGuid,
            IEnumerable<IValueSetCode> valueSetCodes);
    }
}
