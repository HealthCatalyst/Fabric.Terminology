namespace Fabric.Terminology.TestsBase.Mocks
{
    using System.Collections.Generic;

    using Fabric.Terminology.SqlServer.Models;
    using Fabric.Terminology.SqlServer.Models.Dto;

    public static class MockDtoBuilder
    {
        internal static ValueSetDescriptionDto ValueSetDescriptionDto(
            string valueSetId,
            string valueSetNm)
        {
            return new ValueSetDescriptionDto { ValueSetReferenceID = valueSetId, ValueSetNM = valueSetNm };
        }

        internal static IEnumerable<ValueSetCodeDto> ValueSetCodeDtoCollection(int count = 10)
        {
            for (var i = 0; i < count; i++)
            {
                yield return ValueSetCodeDto("TEST" + i, "TEST" + i + " DESC");
            }
        }

        internal static ValueSetCodeDto ValueSetCodeDto(
            string codeCd,
            string codeDsc)
        {
            return new ValueSetCodeDto { CodeCD = codeCd, CodeDSC = codeDsc };
        }
    }
}