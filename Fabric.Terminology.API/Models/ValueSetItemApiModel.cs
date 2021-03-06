﻿namespace Fabric.Terminology.API.Models
{
    using System;
    using System.Collections.Generic;

    public class ValueSetItemApiModel : ValueSetMetaApiModel, IIdentifiable
    {
        public string Identifier { get; set; }

        public Guid ValueSetGuid { get; set; }

        public string ValueSetReferenceId { get; set; }

        public string Name { get; set; }

        public Guid OriginGuid { get; set; }

        public string StatusCode { get; set; }

        public bool IsCustom { get; set; }

        public bool IsLatestVersion { get; set; }

        public IReadOnlyCollection<ValueSetCodeCountApiModel> CodeCounts { get; set; }
    }
}