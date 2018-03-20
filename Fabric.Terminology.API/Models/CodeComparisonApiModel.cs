namespace Fabric.Terminology.API.Models
{
    using System;
    using System.Collections.Generic;

    public class CodeComparisonApiModel
    {
        public CodeSystemCodeApiModel Code { get; set; }

        public IEnumerable<Guid> ValueSetGuids { get; set; }
    }
}