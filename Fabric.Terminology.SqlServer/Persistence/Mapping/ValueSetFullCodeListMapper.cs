namespace Fabric.Terminology.SqlServer.Persistence.Mapping
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.Domain.Persistence.Mapping;
    using Fabric.Terminology.SqlServer.Caching;
    using Fabric.Terminology.SqlServer.Models.Dto;
    internal sealed class ValueSetFullCodeListMapper : IModelMapper<ValueSetDescriptionDto, IValueSet>
    {
        private readonly IMemoryCacheProvider cache;

        private readonly Func<string, string[], IReadOnlyCollection<IValueSetCode>> fetch;

        private readonly IEnumerable<string> codeSystemCodes;

        public ValueSetFullCodeListMapper(IMemoryCacheProvider memCache, Func<string, string[], IReadOnlyCollection<IValueSetCode>> fetchCodes, IEnumerable<string> codeSystemCDs)
        {
            this.cache = memCache;
            this.fetch = fetchCodes;
            this.codeSystemCodes = codeSystemCDs;
        }

        public IValueSet Map(ValueSetDescriptionDto dto)
        {
            // Ensure not already cached with full codes list
            var found = this.cache.GetCachedValueSetWithAllCodes(dto.ValueSetID, this.codeSystemCodes.ToArray());
            if (found != null)
            {
                return found;
            }

            // Clears cache item in case a short list item is stored (forces cache update)
            var cacheKey = CacheKeys.ValueSetKey(dto.ValueSetID, this.codeSystemCodes.ToArray());
            this.cache.ClearItem(cacheKey);

            return (IValueSet)this.cache.GetItem(
                cacheKey, () =>
                    {
                        var codes = this.fetch.Invoke(dto.ValueSetID, this.codeSystemCodes.ToArray());
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