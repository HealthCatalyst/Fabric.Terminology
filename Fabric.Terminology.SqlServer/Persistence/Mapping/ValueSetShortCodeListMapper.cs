namespace Fabric.Terminology.SqlServer.Persistence.Mapping
{
    using System.Collections.Generic;
    using System.Linq;
    using System;

    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.SqlServer.Caching;
    using Fabric.Terminology.SqlServer.Models.Dto;
    using Fabric.Terminology.Domain.Persistence.Mapping;

    internal sealed class ValueSetShortCodeListMapper : IModelMapper<ValueSetDescriptionDto, IValueSet>
    {
        private readonly IMemoryCacheProvider cache;

        private readonly ILookup<string, IValueSetCode> lookupCodes;

        private readonly IDictionary<string, IValueSet> stash;

        private readonly Func<string, int> getCount;

        public ValueSetShortCodeListMapper(
            IMemoryCacheProvider memCache, 
            ILookup<string, IValueSetCode> lookup, 
            IDictionary<string, IValueSet> previouslyCached,
            Func<string, int> getCount)
        {
            this.cache = memCache;
            this.lookupCodes = lookup;
            this.stash = previouslyCached;
            this.getCount = getCount;
        }

        public IValueSet Map(ValueSetDescriptionDto dto)
        {
            if (this.stash.ContainsKey(dto.ValueSetID))
            {
                return this.stash[dto.ValueSetID];
            }

            var cacheKey = CacheKeys.ValueSetKey(dto.ValueSetID);
            return (IValueSet)this.cache.GetItem(
                cacheKey, () =>
                {
                    var codes = this.lookupCodes[dto.ValueSetID].ToArray();
                    return new ValueSet
                    {
                        ValueSetId = dto.ValueSetID,
                        AuthoringSourceDescription = dto.AuthoringSourceDSC,
                        Name = dto.ValueSetNM,
                        IsCustom = false,
                        PurposeDescription = dto.PurposeDSC,
                        SourceDescription = dto.SourceDSC,
                        VersionDescription = dto.VersionDSC,
                        ValueSetCodes = codes,
                        ValueSetCodesCount = this.getCount.Invoke(dto.ValueSetID)
                    };
                },
                TimeSpan.FromMinutes(this.cache.Settings.MemoryCacheMinDuration),
                this.cache.Settings.MemoryCacheSliding);
        }
    }
}