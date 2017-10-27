namespace Fabric.Terminology.SqlServer.Persistence
{
    using System;
    using System.Collections.Generic;

    using Fabric.Terminology.Domain.Models;

    internal interface IClientTermValueSetCustomizationManager
    {
        IReadOnlyCollection<RepositoryOperation> GetRemoveCodesOperations(
            Guid valueSetGuid,
            IEnumerable<ICodeSystemCode> codeSystemCodes);

        IReadOnlyCollection<RepositoryOperation> GetAddCodesOperations(
            Guid valueSetGuid,
            IEnumerable<IValueSetCode> valueSetCodes);
    }
}
