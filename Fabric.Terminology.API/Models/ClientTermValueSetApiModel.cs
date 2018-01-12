namespace Fabric.Terminology.API.Models
{
    using System.Collections.Generic;

    public class ClientTermValueSetApiModel : ValueSetMetaApiModel
    {
        public string Name { get; set; }

        public IEnumerable<CodeSystemCodeApiModel> CodeSetCodes { get; set; }

        public IEnumerable<CodeInstruction> CodeInstructions { get; set; }
    }
}