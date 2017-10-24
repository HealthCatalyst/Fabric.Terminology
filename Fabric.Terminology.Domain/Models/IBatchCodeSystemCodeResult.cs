namespace Fabric.Terminology.Domain.Models
{
    using System.Collections.Generic;

    public interface IBatchCodeSystemCodeResult
    {
        IReadOnlyCollection<ICodeSystemCode> Matches { get; }

        IReadOnlyCollection<string> NotFound { get; }
    }
}
