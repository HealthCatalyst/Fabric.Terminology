namespace Fabric.Terminology.Domain.Services
{
    using System;
    using System.Collections.Generic;

    using CallMeMaybe;

    using Fabric.Terminology.Domain.Models;

    public interface ICodeSystemService
    {
        Maybe<ICodeSystem> GetCodeSystem(Guid codeSystemGuid);

        IReadOnlyCollection<ICodeSystem> GetAll(params Guid[] codeSystemGuids);
    }
}