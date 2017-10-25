namespace Fabric.Terminology.API.Models
{
    using System.Collections.Generic;

    using Fabric.Terminology.Domain;

    public class ValueSetCreationApiModel : ValueSetMetaApiModel
    {
        public string Name { get; set; }

        public string ClientCode { get; set; } = string.Empty;

        public IEnumerable<CodeSystemCodeApiModel> CodeSetCodes { get; set; }
    }
}