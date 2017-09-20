namespace Fabric.Terminology.API.Models
{
    using System;

    public class CodeSetCodeApiModel
    {
        public Guid CodeGuid { get; internal set; }

        public string Code { get; internal set; }

        public string Name { get; internal set; }

        public string CodeSystemCode { get; internal set; }
    }
}