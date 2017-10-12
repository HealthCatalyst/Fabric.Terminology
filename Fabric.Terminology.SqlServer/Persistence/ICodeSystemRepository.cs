namespace Fabric.Terminology.SqlServer.Persistence
{
    using System;
    using System.Collections.Generic;

    using CallMeMaybe;

    using Fabric.Terminology.Domain.Models;

    internal interface ICodeSystemRepository
    {
        IReadOnlyCollection<ICodeSystem> GetAll(params Guid[] codeSystemGuids);

        Maybe<ICodeSystem> GetCodeSystem(Guid codeSystemGuid);
    }
}