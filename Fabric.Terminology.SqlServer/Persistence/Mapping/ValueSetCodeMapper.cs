namespace Fabric.Terminology.SqlServer.Persistence.Mapping
{
    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.Domain.Persistence.Mapping;
    using Fabric.Terminology.SqlServer.Models.Dto;

    using JetBrains.Annotations;

    internal sealed class ValueSetCodeMapper : IModelMapper<ValueSetCodeDto, IValueSetCode>
    {
        [CanBeNull]
        public IValueSetCode Map(ValueSetCodeDto dto)
        {
            var codeSystem = new ValueSetCodeSystem
            {
                Code = dto.CodeSystemCD,
                Name = dto.CodeSystemNM,
                Version = dto.CodeSystemVersionTXT
            };

            return new ValueSetCode
            {
                ValueSetId = dto.ValueSetID,
                Code = dto.CodeCD,
                CodeSystem = codeSystem,
                Name = dto.CodeDSC,
                RevisionDate = dto.RevisionDTS,
                VersionDescription = dto.VersionDSC
            };
        }
    }
}