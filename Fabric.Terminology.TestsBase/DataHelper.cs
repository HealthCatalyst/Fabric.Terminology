namespace Fabric.Terminology.TestsBase
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.TestsBase.Mocks;

    public class DataHelper
    {
        public static IValueSet SingleValueSet(string idSuffix = "1", int codeCount = 1)
        {
            return SingleValueSet(new MockValueSetSettings { IdSuffix = idSuffix, ValueSetCodeCount = codeCount });
        }

        public static IValueSet SingleValueSet(MockValueSetSettings settings)
        {
            var valueSetId = $"value.set.id.{settings.IdSuffix}";

            var codes = new List<IValueSetCode>();

            for (var i = 0; i < settings.ValueSetCodeCount; i++)
            {
                codes.Add(
                    new ValueSetCode
                    {
                        Code = $"code{i}",
                        CodeSystem = new CodeSystem { Code = "cd1", Name = "cd1", Version = "cd1version" },
                        Name = $"code{i}",
                        RevisionDate = null,
                        ValueSetId = valueSetId,
                        VersionDescription = "version"
                    });
            }

            return new ValueSet
            {
                ValueSetId = valueSetId,
                AuthoringSourceDescription = "author",
                IsCustom = true,
                Name = $"test{settings.IdSuffix}",
                DefinitionDescription = "purpose",
                SourceDescription = "source",
                ValueSetCodesCount = settings.ValueSetCodeCount,
                VersionDescription = "version",
                ValueSetCodes = codes
            };
        }

        public static IReadOnlyCollection<IValueSet> CollectionOfValueSets(IEnumerable<MockValueSetSettings> settings)
        {
            var mockSettings = settings as MockValueSetSettings[] ?? settings.ToArray();
            var distinctSuffixes = mockSettings.Select(x => x.IdSuffix).Distinct();
            if (distinctSuffixes.Count() != mockSettings.Count())
            {
                throw new Exception("Duplicate ValueSetIds will be generated");
            }

            return mockSettings.Select(SingleValueSet).ToList().AsReadOnly();
        }

        public static PagedCollection<IValueSet> PagedCollectionOfValueSets(IReadOnlyCollection<IValueSet> valueSets, int currentPage = 1, int itemsPerPage = 20)
        {
            return new PagedCollection<IValueSet>
            {
                TotalItems = valueSets.Count,
                PagerSettings = new PagerSettings { CurrentPage = currentPage, ItemsPerPage = itemsPerPage },
                TotalPages = (int)Math.Ceiling((double)valueSets.Count / itemsPerPage),
                Values = valueSets.Take(itemsPerPage).ToList().AsReadOnly()
            };
        }
    }
}