namespace Fabric.Terminology.SqlServer.Persistence.Mapping
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.SqlServer.Caching;
    using Fabric.Terminology.SqlServer.Configuration;
    using Fabric.Terminology.SqlServer.Models.Dto;
    internal sealed class ValueSetFullCodeListMapper : IModelMapper<ValueSetDescriptionDto, IValueSet>
    {
        private readonly IMemoryCacheProvider cache;

        private readonly Func<string, IReadOnlyCollection<IValueSetCode>> fetch;

        public ValueSetFullCodeListMapper(IMemoryCacheProvider memCache, Func<string, IReadOnlyCollection<IValueSetCode>> fetchCodes)
        {
            this.cache = memCache;
            this.fetch = fetchCodes;
        }

        public IValueSet Map(ValueSetDescriptionDto dto)
        {
            // Ensure not already cached with full codes list
            var found = this.cache.GetCachedValueSetWithAllCodes(dto.ValueSetID);
            if (found != null)
            {
                return found;
            }

            // Clears cache item in case a short list item is stored (forces cache update)
            var cacheKey = CacheKeys.ValueSetKey(dto.ValueSetID);
            this.cache.ClearItem(cacheKey);

            return (IValueSet)this.cache.GetItem(
                cacheKey, () =>
                    {
                        var codes = this.fetch.Invoke(dto.ValueSetID);
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
                            ValueSetCodesCount = codes.Count
                        };
                    },
                TimeSpan.FromMinutes(this.cache.Settings.MemoryCacheMinDuration),
                this.cache.Settings.MemoryCacheSliding);
        }
    }
}