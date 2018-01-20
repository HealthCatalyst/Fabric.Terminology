namespace Fabric.Terminology.API.Models
{
    using System;

    public class ValueSetCopyApiModel : ValueSetMetaApiModel
    {
        public string Name { get; set; }

        public Guid OriginGuid { get; set; }
    }
}