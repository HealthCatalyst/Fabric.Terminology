namespace Fabric.Terminology.TestsBase
{
    using System;
    using System.Collections.Generic;

    using Fabric.Terminology.Domain;
    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.TestsBase.Mocks;

    public static class DataHelper
    {
        public static IValueSet SingleValueSet(string idSuffix = "1", int codeCount = 1)
        {
            return SingleValueSet(new MockValueSetSettings { IdSuffix = idSuffix, ValueSetCodeCount = codeCount });
        }

        public static IValueSet SingleValueSet(MockValueSetSettings settings)
        {
            var valueSetGuid = Guid.NewGuid();

            var codes = new List<IValueSetCode>();

            for (var i = 0; i < settings.ValueSetCodeCount; i++)
            {
                var codeSystemGuid = Guid.NewGuid();
                codes.Add(
                    new ValueSetCode
                    {
                        Code = $"code{i}",
                        CodeSystemGuid = codeSystemGuid,
                        Name = $"code{i}",
                        CodeGuid = Guid.Empty,
                        CodeSystemName = $"cs-{codeSystemGuid}",
                        ValueSetGuid = valueSetGuid
                    });
            }

            var backingItem = new ValueSetBackingItem
            {
                ValueSetGuid = valueSetGuid,
                AuthorityDescription = "author",
                IsCustom = true,
                Name = $"test{settings.IdSuffix}",
                ClientCode = "TEST001",
                ValueSetReferenceId = $"refid-{valueSetGuid}",
                SourceDescription = "source",
                DefinitionDescription = "definition"
            };

            return new ValueSet(backingItem, codes, codes.GetCodeCountsFromCodes());
        }
    }
}