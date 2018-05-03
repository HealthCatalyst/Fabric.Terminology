namespace Fabric.Terminology.API.Models
{
    using System;

    using NullGuard;

    public class ValueSetCopyApiModel : ValueSetMetaApiModel
    {
        public string Name { get; set; }

        [AllowNull]
        public Guid OriginGuid { get; set; }
    }
}