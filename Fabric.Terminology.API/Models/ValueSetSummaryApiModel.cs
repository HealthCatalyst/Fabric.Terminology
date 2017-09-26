namespace Fabric.Terminology.API.Models
{
    using System;
    using System.Collections.Generic;

    using Fabric.Terminology.Domain.Models;

    public class ValueSetSummaryApiModel : ValueSetMetaApiModel, IIdentifiable
    {
        public string Identifier { get; set; }

        public string ValueSetGuid { get; set; }

        public string ValueSetReferenceId { get; set; }

        public string Name { get; set; }

        public string OriginGuid { get; set; }

        public string ClientCode { get; set; }

        public bool IsCustom { get; set; }

        public bool IsLatestVersion { get; set; }

        public IReadOnlyCollection<ValueSetCodeCountApiModel> ValueSetStats { get; set; }
    }
}