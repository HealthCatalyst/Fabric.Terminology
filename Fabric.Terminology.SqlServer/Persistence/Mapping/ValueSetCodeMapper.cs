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
            return new ValueSetCode
            {
                ValueSetGuid = dto.ValueSetGUID,
                CodeGuid = dto.CodeGUID,
                Code = dto.CodeCD,
                CodeSystemCode = dto.CodeSystemCD,
                Name = dto.CodeDSC
            };
        }
    }
}