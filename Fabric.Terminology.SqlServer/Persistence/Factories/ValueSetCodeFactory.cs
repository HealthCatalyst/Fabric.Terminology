namespace Fabric.Terminology.SqlServer.Persistence.Factories
{
    using Fabric.Terminology.Domain;
    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.Domain.Persistence.Factories;
    using Fabric.Terminology.SqlServer.Models.Dto;

    internal class ValueSetCodeFactory : IModelFactory<ValueSetCodeDto, IValueSetCode>
    {
        public IValueSetCode Build(ValueSetCodeDto dto)
        {
            return new ValueSetCode
            {
                ValueSetGuid = dto.ValueSetGUID,
                CodeSystemGuid = dto.CodeSystemGuid,
                Code = dto.CodeCD,
                CodeGuid = dto.CodeGUID.GetValueOrDefault(),
                CodeSystemName = dto.CodeSystemNM,
                Name = dto.CodeDSC.OrEmptyIfNull()
            };
        }
    }
}
