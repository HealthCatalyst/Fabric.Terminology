using Fabric.Terminology.Domain.Models;
using Fabric.Terminology.SqlServer.Models.Dto;

namespace Fabric.Terminology.SqlServer.Persistence.Mappers
{
    internal static class ModelFactory
    {
        public static IValueSetCode BuildModel(ValueSetCodeDto dto)
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