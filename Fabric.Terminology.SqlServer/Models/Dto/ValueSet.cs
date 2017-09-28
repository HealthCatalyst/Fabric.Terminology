namespace Fabric.Terminology.SqlServer
{
    using System.Collections.Generic;

    using Fabric.Terminology.SqlServer.Models.Dto;

    internal class ValueSet : Domain.Models.ValueSet
    {
        public ValueSet()
        {
        }

        public ValueSet(
            ValueSetDescriptionDto valueSetDescriptionDto,
            IReadOnlyCollection<ValueSetCodeDto> valueSetCodeDtos)
        {
            //cody
        }
    }
}