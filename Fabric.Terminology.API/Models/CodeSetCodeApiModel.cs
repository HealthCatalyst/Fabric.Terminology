namespace Fabric.Terminology.API.Models
{
    using System;

    using Swagger.ObjectModel;

    public class CodeSetCodeApiModel
    {
        public string CodeGuid { get; internal set; }

        public string Code { get; internal set; }

        public string Name { get; internal set; }

        public string CodeSystemGuid { get; internal set; }
    }
}