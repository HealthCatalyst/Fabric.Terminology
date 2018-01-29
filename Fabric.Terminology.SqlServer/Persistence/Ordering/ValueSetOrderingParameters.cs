namespace Fabric.Terminology.SqlServer.Persistence.Ordering
{
    using System.Collections.Generic;
    using System.Linq;

    using Fabric.Terminology.Domain;
    using Fabric.Terminology.Domain.Persistence.Querying;

    public class ValueSetOrderingParameters : IOrderingParameters
    {
        private static readonly IEnumerable<string> ValidValueSetSortFields =
            new[]
            {
                "name",
                "valuesetreferenceid",
                "sourcedescription",
                "versiondate",
                "codecount"
            };

        private string fieldName = "Name";

        public string FieldName
        {
            get => this.fieldName;

            set => this.fieldName = !ValidValueSetSortFields.Contains(value.ToLowerInvariant()) ? "Name" : value;
        }

        public SortDirection Direction { get; set; } = SortDirection.Asc;
    }
}