namespace Fabric.Terminology.Domain.Models
{
    using System;
    using System.Collections.Generic;

    public class ValueSetCodeComparison
    {
        public ICodeSystemCode Code { get; set; }

        public IEnumerable<Guid> ValueSetGuids { get; set; }
    }
}