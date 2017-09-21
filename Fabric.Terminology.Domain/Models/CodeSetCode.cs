namespace Fabric.Terminology.Domain.Models
{
    using System;

    internal class CodeSetCode : ICodeSetCode
    {
        public Guid CodeGuid { get; internal set; }

        public Guid CodeSystemGuid { get; set; }

        public string CodeSystemName { get; set; }

        public string Code { get; internal set; }

        public string Name { get; internal set; }
    }
}