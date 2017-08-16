namespace Fabric.Terminology.TestsBase.Mocks
{
    using System;
    using System.Collections.Generic;

    using Fabric.Terminology.API.Models;
    using Fabric.Terminology.Domain.Models;

    public class MockApiModelBuilder
    {
        public static ValueSetCreationApiModel ValueSetCreationApiModel(
            string name,
            int codeCount = 10)
        {
            return new ValueSetCreationApiModel
            {
                Name = name,
                AuthoringSourceDescription = "Test Authoring Source Description",
                PurposeDescription = "Test Purpose Description",
                SourceDescription = "Test Source Description",
                VersionDescription = "Test Version Description",
                CodeSetCodes = CodeSetCodeApiModelCollection(codeCount)
            };
        }

        public static IEnumerable<CodeSetCodeApiModel> CodeSetCodeApiModelCollection(int count = 10)
        {
            for (var i = 0; i < count; i++)
            {
                yield return CodeSetCodeApiModel($"Code.Set.Code.{i}", $"Code.Set.Code.{i} Name");
            }
        }

        public static CodeSetCodeApiModel CodeSetCodeApiModel(string code, string name)
        {
            var codeSystem = new CodeSystem
            {
                Code = "TEST-CODE-SYSTEM",
                Name = "TEST CODE SYSTEM",
                Version = "TEST-CODE-SYSTEM-VERSION"
            };

            return new CodeSetCodeApiModel
            {
                Code = code,
                Name = name,
                SourceDescription = "TEST Source",
                VersionDescription = "TEST Version",
                CodeSystem = codeSystem,
                LastLoadDate = DateTime.UtcNow
            };
        }
    }
}
