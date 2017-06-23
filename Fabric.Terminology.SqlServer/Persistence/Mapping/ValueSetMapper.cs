using System;
using Fabric.Terminology.Domain.Models;
using Fabric.Terminology.Domain.Persistence;
using Fabric.Terminology.SqlServer.Caching;
using Fabric.Terminology.SqlServer.Configuration;
using Fabric.Terminology.SqlServer.Models.Dto;

namespace Fabric.Terminology.SqlServer.Persistence.Mapping
{
    internal sealed class ValueSetMapper : IModelMapper<ValueSetDescriptionDto, IValueSet>
    {
        private readonly IMemoryCacheProvider cache;
        private readonly IValueSetCodeRepository repository;

        public ValueSetMapper(IMemoryCacheProvider memCache, IValueSetCodeRepository valueSetCodeRepository)
        {
            // TODO null protect
            this.cache = memCache;
            this.repository = valueSetCodeRepository;
        }

        public IValueSet Map(ValueSetDescriptionDto dto)
        {
            var cacheKey = CacheKeys.ValueSetKey(dto.ValueSetID);
            return (IValueSet)this.cache.GetItem(cacheKey, () =>
                {
                    var codes = this.repository.GetValueSetCodes(dto.ValueSetID);
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