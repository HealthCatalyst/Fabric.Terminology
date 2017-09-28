namespace Fabric.Terminology.SqlServer.Models.Dto
{
    using System;

    internal class ValueSetCode : Domain.Models.ValueSetCode
    {
        public ValueSetCode()
        {
        }

        public ValueSetCode(ValueSetCodeDto dto)
        {
            this.ValueSetGuid = dto.ValueSetGUID;
            this.CodeGuid = dto.CodeGUID ?? Guid.Empty;
            this.Code = dto.CodeCD;
            this.Name = dto.CodeDSC;
            this.CodeSystemGuid = dto.CodeSystemGuid;
            this.CodeSystemName = dto.CodeSystemNM;
        }
    }
}