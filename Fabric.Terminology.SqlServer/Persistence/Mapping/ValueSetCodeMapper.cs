using Fabric.Terminology.Domain.Models;
using Fabric.Terminology.SqlServer.Models.Dto;

namespace Fabric.Terminology.SqlServer.Persistence.Mapping
{
    using System.Threading.Tasks;

    internal sealed class ValueSetCodeMapper : IModelMapper<ValueSetCodeDto,IValueSetCode>
    {
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