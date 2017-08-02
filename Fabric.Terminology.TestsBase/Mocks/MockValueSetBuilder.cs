namespace Fabric.Terminology.TestsBase.Mocks
{
    using System;

    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.SqlServer.Models.Dto;

    public class MockValueSetBuilder
    {
        internal static IValueSetCode BuildValueSetCode(
            IValueSet valueSet,
            string name,
            string code,
            ICodeSystem codeSystem)
        {
            return new ValueSetCode
            {
                Name = name,
                CodeSystem = codeSystem,
                Code = code,
                LastLoadDate = DateTime.UtcNow,
                SourceDescription = "TEST",
                ValueSetUniqueId = valueSet.ValueSetUniqueId,
                ValueSetId = valueSet.ValueSetId
            };
        }
    }
}
