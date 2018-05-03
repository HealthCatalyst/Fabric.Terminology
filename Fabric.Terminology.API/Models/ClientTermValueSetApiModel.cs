namespace Fabric.Terminology.API.Models
{
    using System;
    using System.Collections.Generic;

    public class ClientTermValueSetApiModel : ValueSetMetaApiModel
    {
        public string Name { get; set; } = string.Empty;

        public IEnumerable<CodeOperation> CodeOperations { get; set; } = Array.Empty<CodeOperation>();
    }
}