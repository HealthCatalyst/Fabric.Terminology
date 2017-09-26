namespace Fabric.Terminology.SqlServer.Persistence.Mapping
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.Domain.Persistence.Factories;
    using Fabric.Terminology.SqlServer.Caching;
    using Fabric.Terminology.SqlServer.Models;
    using Fabric.Terminology.SqlServer.Models.Dto;

    using JetBrains.Annotations;

    internal sealed class ValueSetFullCodeListFactory : ValueSetMapperBase, IModelFactory<ValueSetDescriptionDto, IValueSet>
    {
        private static readonly EmptySamdBinding EmptyBinding = new EmptySamdBinding();

        private readonly IMemoryCacheProvider cache;

        private readonly Func<Guid, string[], IReadOnlyCollection<IValueSetCode>> fetch;

        private readonly IEnumerable<string> codeSystemCodes;

        public ValueSetFullCodeListFactory(
            IMemoryCacheProvider memCache,
            Func<Guid, string[], IReadOnlyCollection<IValueSetCode>> fetchCodes,
            IEnumerable<string> codeSystemCDs)
        {
            this.cache = memCache;
            this.fetch = fetchCodes;
            this.codeSystemCodes = codeSystemCDs;
        }

        [CanBeNull]
        public IValueSet Build(ValueSetDescriptionDto dto)
        {
            // Ensure not already cached with full codes list
            var found = this.cache.GetCachedValueSetWithAllCodes(dto.ValueSetUniqueID, this.codeSystemCodes.ToArray());
            if (found != null)
            {
                return found;
            }

            // Clears cache item in case a short list item is stored (forces cache update)
            var cacheKey = CacheKeys.ValueSetBackingItemKey(dto.ValueSetGUID, this.codeSystemCodes.ToArray());
            this.cache.ClearItem(cacheKey);

            // ValueSet must have codes
            var codes = this.fetch.Invoke(dto.ValueSetGUID, this.codeSystemCodes.ToArray());
            if (!codes.Any())
            {
                return null;
            }

            return (IValueSet)this.cache.GetItem(
                cacheKey, () => this.Build(dto, codes, codes.Count),
                TimeSpan.FromMinutes(this.cache.Settings.MemoryCacheMinDuration),
                this.cache.Settings.MemoryCacheSliding);
        }
    }
}