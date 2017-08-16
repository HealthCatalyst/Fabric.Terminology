namespace Fabric.Terminology.SqlServer.Persistence.Mapping
{
    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.Domain.Persistence.Mapping;
    using Fabric.Terminology.SqlServer.Models;
    using Fabric.Terminology.SqlServer.Models.Dto;

    using JetBrains.Annotations;

    internal sealed class ValueSetCodeMapper : IModelMapper<ValueSetCodeDto, IValueSetCode>
    {
        [CanBeNull]
        public IValueSetCode Map(ValueSetCodeDto dto)
        {
            var codeSystem = new CodeSystem
            {
                Code = dto.CodeSystemCD,
                Name = dto.CodeSystemNM,
                Version = dto.CodeSystemVersionTXT
            };

            return new ValueSetCode(
                dto.ValueSetID,
                dto.ValueSetUniqueID,
                dto.ValueSetOID,
                dto.ValueSetNM,
                dto.CodeCD,
                dto.CodeDSC,
                codeSystem,
                dto.VersionDSC,
                dto.SourceDSC,
                dto.LastLoadDTS,
                dto.RevisionDTS);
        }
    }
}