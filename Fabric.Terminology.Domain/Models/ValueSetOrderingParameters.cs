namespace Fabric.Terminology.Domain.Models
{
    using System.Collections.Generic;
    using System.Linq;

    public class ValueSetOrderingParameters
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