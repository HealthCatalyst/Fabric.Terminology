namespace Fabric.Terminology.API.Models
{
    using System;

    using Fabric.Terminology.Domain.Models;

    using Swagger.ObjectModel;

    public class CodeSetCodeApiModel : ICodeSetCode
    {
        public Guid CodeGuid { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public Guid CodeSystemGuid { get; set; }

        public string CodeSystemName { get; set; }
    }
}