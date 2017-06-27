namespace Fabric.Terminology.API.Models
{
    using System.Collections.Generic;

    using Fabric.Terminology.Domain.Models;

    public class FindByTermQuery
    {
        public string Term { get; set; }

        public PagerSettings PagerSettings { get; set; }

        public IEnumerable<string> CodeSystemCodes { get; set; }
    }
}