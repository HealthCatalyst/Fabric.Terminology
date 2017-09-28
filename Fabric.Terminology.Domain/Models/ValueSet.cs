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

        internal ValueSet(string name, IValueSetMeta meta, IEnumerable<ICodeSetCode> valueSetCodes)
        {
            this.Name = name;
            this.VersionDate = meta.VersionDate;
            this.DefinitionDescription = meta.DefinitionDescription;
            this.AuthoringSourceDescription = meta.AuthoringSourceDescription;
            this.SourceDescription = meta.SourceDescription;
            this.ValueSetCodes = valueSetCodes.Select(c => new ValueSetCode(c)).ToList();
        }

        public IReadOnlyCollection<IValueSetCodeCount> CodeCounts { get; }

        public IReadOnlyCollection<IValueSetCode> ValueSetCodes { get; internal set; }
    }
}