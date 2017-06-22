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
        private readonly IMemoryCacheProvider _cache;
        private readonly IValueSetCodeRepository _valueSetCodeRepository;

        public ValueSetMapper(IMemoryCacheProvider cache, IValueSetCodeRepository valueSetCodeRepository)
        {
            // TODO null protect
            _cache = cache;
            _valueSetCodeRepository = valueSetCodeRepository;
        }

        public IValueSet Map(ValueSetDescriptionDto dto)
        {
            var cacheKey = CacheKeys.ValueSetKey(dto.ValueSetID);
            return (IValueSet)_cache.GetItem(cacheKey, () =>
                {
                    var codes = _valueSetCodeRepository.GetValueSetCodes(dto.ValueSetID);
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
                TimeSpan.FromMinutes(_cache.Settings.MemoryCacheMinDuration),
                _cache.Settings.MemoryCacheSliding);
        }
    }
}