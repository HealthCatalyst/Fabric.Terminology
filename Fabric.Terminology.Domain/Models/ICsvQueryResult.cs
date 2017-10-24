namespace Fabric.Terminology.Domain.Models
{
    using System.Collections.Generic;

    public interface ICsvQueryResult
    {
        IReadOnlyCollection<ICodeSystemCode> Matches { get; }

        IReadOnlyCollection<string> NotFound { get; }
    }
}
