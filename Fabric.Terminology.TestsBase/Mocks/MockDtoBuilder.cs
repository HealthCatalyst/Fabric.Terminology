namespace Fabric.Terminology.TestsBase.Mocks
{
    using System.Collections.Generic;

    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.SqlServer.Models;
    using Fabric.Terminology.SqlServer.Models.Dto;

    public class MockDtoBuilder
    {
        private static readonly EmptySamdBinding EmptyBinding = new EmptySamdBinding();

        internal static ValueSetDescriptionDto ValueSetDescriptionDto(
            string valueSetId,
            string valueSetNm)
        {
            return new ValueSetDescriptionDto();
        }

        internal static IEnumerable<ValueSetCodeDto> ValueSetCodeDtoCollection(
            ValueSetDescriptionDto valueSetDto,
            int count = 10)
        {
            for (var i = 0; i < count; i++)
            {
                yield return ValueSetCodeDto(valueSetDto, "TEST" + i.ToString(), "TEST" + i.ToString() + " DESC");
            }
        }

        internal static ValueSetCodeDto ValueSetCodeDto(
            ValueSetDescriptionDto valueSetDto,
            string codeCd,
            string codeDsc)
        {
            return new ValueSetCodeDto();
        }

        internal static ValueSetCodeDto ValueSetCodeDto(
            ValueSetDescriptionDto valueSetDto,
            string codeCd,
            string codeDsc,
            ICodeSystem codeSystem)
        {
            return new ValueSetCodeDto();
        }
    }
}