namespace Fabric.Terminology.Domain.Services
{
    using System;
    using System.Collections.Generic;

    using Fabric.Terminology.Domain.Models;

    public interface ICodeSystemService
    {
        IReadOnlyCollection<ICodeSystem> GetAll();

        ICodeSystem GetCodeSystem(Guid codeSystemGuid);

        IReadOnlyCollection<ICodeSetCode> GetCodeSystemCodes(Guid codeSystemGuid);
    }
}