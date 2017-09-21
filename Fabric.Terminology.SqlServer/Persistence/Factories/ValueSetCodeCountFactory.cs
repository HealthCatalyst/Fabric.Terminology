namespace Fabric.Terminology.SqlServer.Persistence.Mapping
{
    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.Domain.Persistence.Factories;
    using Fabric.Terminology.SqlServer.Models.Dto;

    internal sealed class ValueSetCodeCountFactory : IModelFactory<ValueSetCodeCountDto, IValueSetCodeCount>
    {
        public IValueSetCodeCount Build(ValueSetCodeCountDto dto)
        {
            return new ValueSetCodeCount
            {
                CodeCount = dto.CodeSystemPerValueSetNBR,
                CodeSystemGuid = dto.CodeSystemGUID,
                ValueSetGuid = dto.ValueSetGUID
            };
        }
    }
}
