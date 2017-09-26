namespace Fabric.Terminology.API.Models
{
    using System.Collections.Generic;

    public class ValueSetApiModel : ValueSetItemApiModel
    {
        public IReadOnlyCollection<ValueSetCodeApiModel> ValueSetCodes { get; set; }
    }
}