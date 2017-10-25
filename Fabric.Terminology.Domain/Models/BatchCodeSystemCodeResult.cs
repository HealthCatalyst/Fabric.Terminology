namespace Fabric.Terminology.Domain.Models
{
    using System.Collections.Generic;

    internal class BatchCodeSystemCodeResult : IBatchCodeSystemCodeResult
    {
        public IReadOnlyCollection<ICodeSystemCode> Matches { get; set; }

        public IReadOnlyCollection<string> NotFound { get; set; }
    }
}