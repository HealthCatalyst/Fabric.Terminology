namespace Fabric.Terminology.Domain.Models
{
    using System.Collections.Generic;
    using System.Linq;

    internal class ValueSet : ValueSetBase, IValueSet
    {
        internal ValueSet()
        {
            this.ValueSetCodes = new List<IValueSetCode>();
        }

        internal ValueSet(IValueSetBackingItem backingItem, IReadOnlyCollection<IValueSetCode> codes, IReadOnlyCollection<IValueSetCodeCount> counts)
            : base(backingItem)
        {
            this.ValueSetCodes = codes;
            this.CodeCounts = counts;
        }

        internal ValueSet(string name, IValueSetMeta meta, IEnumerable<ICodeSystemCode> codeSetCodes)
        {
            this.Name = name;
            this.VersionDate = meta.VersionDate;
            this.DefinitionDescription = meta.DefinitionDescription;
            this.AuthoringSourceDescription = meta.AuthoringSourceDescription;
            this.SourceDescription = meta.SourceDescription;
            var codes = codeSetCodes.Select(c => new ValueSetCode(c)).ToList();
            this.ValueSetCodes = codes;
            this.CodeCounts = codes.GetCodeCountsFromCodes();
        }

        public IReadOnlyCollection<IValueSetCodeCount> CodeCounts { get; }

        public IReadOnlyCollection<IValueSetCode> ValueSetCodes { get; internal set; }
    }
}