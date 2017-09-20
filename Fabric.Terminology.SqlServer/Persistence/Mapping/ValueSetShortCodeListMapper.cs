namespace Fabric.Terminology.SqlServer.Persistence.Mapping
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.Domain.Persistence.Mapping;
    using Fabric.Terminology.SqlServer.Caching;
    using Fabric.Terminology.SqlServer.Models.Dto;

    using JetBrains.Annotations;

    internal sealed class ValueSetShortCodeListMapper : ValueSetMapperBase, IModelMapper<ValueSetDescriptionDto, IValueSet>
    {
        private readonly IMemoryCacheProvider cache;

        private readonly ILookup<Guid, IValueSetCode> lookupCodes;

        private readonly IDictionary<Guid, IValueSet> stash;

        private readonly Func<Guid, string[], int> getCount;

        private readonly string[] codeSystemCds;

        public ValueSetShortCodeListMapper(
            IMemoryCacheProvider memCache,
            ILookup<Guid, IValueSetCode> lookup,
            IDictionary<Guid, IValueSet> previouslyCached,
            Func<Guid, string[], int> getCount,
            IEnumerable<string> codeSystemCodes)
        {
            this.cache = memCache;
            this.lookupCodes = lookup;
            this.stash = previouslyCached;
            this.getCount = getCount;
            this.codeSystemCds = codeSystemCodes.ToArray();
        }

        [CanBeNull]
        public IValueSet Map(ValueSetDescriptionDto dto)
        {
            if (this.stash.ContainsKey(dto.ValueSetGUID))
            {
                return this.stash[dto.ValueSetGUID];
            }

            var cacheKey = CacheKeys.ValueSetKey(dto.ValueSetGUID, this.codeSystemCds);
            return (IValueSet)this.cache.GetItem(
                cacheKey, () =>
                {
                    var codes = this.lookupCodes[dto.ValueSetGUID].ToArray();
                    return this.Build(dto, codes, this.getCount.Invoke(dto.ValueSetGUID, this.codeSystemCds));
                },
                TimeSpan.FromMinutes(this.cache.Settings.MemoryCacheMinDuration),
                this.cache.Settings.MemoryCacheSliding);
        }
    }
}