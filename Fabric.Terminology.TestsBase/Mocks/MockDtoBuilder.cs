namespace Fabric.Terminology.TestsBase.Mocks
{
    using System;
    using System.Collections.Generic;

    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.SqlServer.Models;
    using Fabric.Terminology.SqlServer.Models.Dto;

    public class MockDtoBuilder
    {
        private static readonly EmptySamdBinding EmptyBinding = new EmptySamdBinding();

        internal static ValueSetDescriptionDto BuildValueSetDescriptionDto(
            string valueSetId,
            string valueSetNm)
        {
            return new ValueSetDescriptionDto
            {
                BindingID = EmptyBinding.BindingID,
                BindingNM = EmptyBinding.BindingNM,
                LastLoadDTS = EmptyBinding.GetLastLoadDTS(),
                AuthoringSourceDSC = "TEST-SRC",
                CreatedByNM = "TESTBASE",
                DefinitionDSC = "TEST DECRIPTION",
                SourceDSC = "UNIT OR INTEGRATION TEST",
                VersionDSC = "CURRENT TEST VERSION",
                BindingDSC = string.Empty,
                LatestVersionFLG = "Y",
                PublicFLG = "Y",
                PurposeDSC = "UNIT or INTEGRATION TESTING",
                TypeDSC = "TESTING",
                ValueSetOID = valueSetId,
                ValueSetUniqueID = valueSetId,
                ValueSetNM = valueSetNm,
                ValueSetID = valueSetId
            };
        }


        internal static IEnumerable<ValueSetCodeDto> BuildValueSetCodeDtoCollection(
            ValueSetDescriptionDto valueSetDto,
            int count = 10)
        {
            for (var i = 0; i < 10; i++)
            {
                yield return BuildValueSetCodeDto(valueSetDto, "TEST" + i.ToString(), "TEST" + i.ToString() + " DESC");
            }
        }

        internal static ValueSetCodeDto BuildValueSetCodeDto(
            ValueSetDescriptionDto valueSetDto,
            string codeCd,
            string codeDsc)
        {
            return BuildValueSetCodeDto(
                valueSetDto,
                codeCd,
                codeDsc,
                new ValueSetCodeSystem { Code = "TEST", Name = "TEST-CODE-SYSTEM", Version = "TEST-VERSION" });
        }

        internal static ValueSetCodeDto BuildValueSetCodeDto(
            ValueSetDescriptionDto valueSetDto,
            string codeCd,
            string codeDsc,
            IValueSetCodeSystem codeSystem)
        {
            return new ValueSetCodeDto
            {
                BindingID = EmptyBinding.BindingID,
                BindingNM = EmptyBinding.BindingNM,
                LastLoadDTS = EmptyBinding.GetLastLoadDTS(),
                RevisionDTS = null,
                SourceDSC = "TestBase",
                VersionDSC = "Current Test Version",
                CodeCD = codeCd,
                CodeDSC = codeDsc,
                CodeSystemCD = codeSystem.Code,
                CodeSystemNM = codeSystem.Name,
                CodeSystemVersionTXT = "CURRENT",
                ValueSetOID = valueSetDto.ValueSetOID,
                ValueSetID = valueSetDto.ValueSetID,
                ValueSetUniqueID = valueSetDto.ValueSetUniqueID,
                ValueSetNM = valueSetDto.ValueSetNM
            };
        }
    }
}