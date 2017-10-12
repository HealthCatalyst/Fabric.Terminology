namespace Fabric.Terminology.SqlServer.Persistence.Factories
{
    using System.Collections.Generic;

    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.Domain.Persistence.Factories;
    using Fabric.Terminology.SqlServer.Models.Dto;

    using Microsoft.EntityFrameworkCore.Metadata;
    internal class ValueSetFactory
    {
        public IValueSet Build(
            ValueSetDescriptionBASEDto descDto,
            IReadOnlyCollection<ValueSetCodeDto> codeDtos,
            string clientTermFlg = "N")
        {
            return new ValueSet
            {
                ValueSetGuid = descDto.ValueSetGUID,
                AuthoringSourceDescription = descDto.AuthorityDSC,
                ClientCode = descDto.ClientCD,
                IsCustom = clientTermFlg == "Y",
                CodeCounts = { }
            };
        }
    }
}
