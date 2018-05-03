namespace Fabric.Terminology.Domain.Models
{
    using System;

    using NullGuard;

    internal class CodeSystemCode : ICodeSystemCode
    {
        public Guid CodeGuid { get; internal set; }

        public Guid CodeSystemGuid { get; set; }

        public string CodeSystemName { get; set; }

        public string Code { get; internal set; }

        [AllowNull]
        public string Name { get; internal set; }

        public bool Retired { get; set; }
    }
}