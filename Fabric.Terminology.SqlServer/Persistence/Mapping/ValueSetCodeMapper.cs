namespace Fabric.Terminology.SqlServer.Persistence.Mapping
{
    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.Domain.Persistence.Mapping;
    using Fabric.Terminology.SqlServer.Models;
    using Fabric.Terminology.SqlServer.Models.Dto;

    using JetBrains.Annotations;

    internal sealed class ValueSetCodeMapper : IModelMapper<ValueSetCodeDto, IValueSetCode>
    {
        private static readonly EmptySamdBinding EmptyBinding = new EmptySamdBinding();

        [CanBeNull]
        public IValueSetCode Map(ValueSetCodeDto dto)
        {
            var codeSystem = new CodeSystem
            {
                Code = dto.CodeSystemCD,
                Name = dto.CodeSystemNM,
                Version = dto.CodeSystemVersionTXT
            };

            return new ValueSetCode
            {
                ValueSetUniqueId = dto.ValueSetUniqueID,
                ValueSetId = dto.ValueSetID,
                Code = dto.CodeCD,
                CodeSystem = codeSystem,
                Name = dto.CodeDSC,
                RevisionDate = dto.RevisionDTS,
                VersionDescription = dto.VersionDSC,
                LastLoadDate = dto.LastLoadDTS,
                ValueSetName = dto.ValueSetNM
            };
        }
    }
}