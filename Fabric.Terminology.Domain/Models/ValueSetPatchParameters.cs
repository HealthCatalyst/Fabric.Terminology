namespace Fabric.Terminology.Domain.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class ValueSetPatchParameters : IHaveValueSetGuid
    {
        public Guid ValueSetGuid { get; set; } = Guid.Empty;

        public string Name { get; set; }

        public IValueSetMeta ValueSetMeta { get; set; }

        public IEnumerable<ICodeSystemCode> CodesToAdd { get; set; } = new List<ICodeSystemCode>();

        public IEnumerable<ICodeSystemCode> CodesToRemove { get; set; } = new List<ICodeSystemCode>();

        public bool IsValid()
        {
            return !this.CodesToAdd.Select(cta => cta.CodeGuid)
                       .Any(ctag => this.CodesToRemove.Select(ctr => ctr.CodeGuid).Contains(ctag));
        }
    }
}