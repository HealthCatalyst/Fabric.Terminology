namespace Fabric.Terminology.API.Models
{
    using System.Collections.Generic;

    public class ValueSetCreationApiModel : ValueSetMetaApiModel
    {
        public string Name { get; set; }

        public IEnumerable<CodeSetCodeApiModel> CodeSetCodes { get; set; }
    }
}