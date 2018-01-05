namespace Fabric.Terminology.API.Models
{
    using System.Collections.Generic;

    public class ValueSetUpdateModel : ValueSetMetaApiModel
    {
        public string Name { get; set; }

        public IEnumerable<CodeInstruction> CodeInstructions { get; set; }
    }
}