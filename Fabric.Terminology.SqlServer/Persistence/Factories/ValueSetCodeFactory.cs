namespace Fabric.Terminology.SqlServer.Persistence.Factories
{
    using System;

    using CallMeMaybe;

    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.Domain.Persistence.Factories;
    using Fabric.Terminology.SqlServer.Models.Dto;

    internal sealed class ValueSetCodeFactory : IModelFactory<ValueSetCodeDto, IValueSetCode>
    {
        public IValueSetCode Build(ValueSetCodeDto dto)
        {
            return new ValueSetCode
            {
                ValueSetGuid = dto.ValueSetGUID,
                CodeGuid = Maybe.From(dto.CodeGUID).Else(Guid.Empty),
                Code = dto.CodeCD,
                Name = dto.CodeDSC,
                CodeSystemGuid = dto.CodeSystemGuid,
                CodeSystemName = dto.CodeSystemNM
            };
        }
    }
}