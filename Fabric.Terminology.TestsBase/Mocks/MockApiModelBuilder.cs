namespace Fabric.Terminology.TestsBase.Mocks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Fabric.Terminology.API.Models;

    public class MockApiModelBuilder
    {
        public static ClientTermValueSetApiModel ClientTermValueSetApiModel(
            string name,
            int codeCount = 10,
            bool secondaryCodeGuids = false)
        {
            var codeGuids = secondaryCodeGuids
                                ? SharedTerminologyCodeHelper.ValidCodeGuidsSafeToAdd
                                : SharedTerminologyCodeHelper.ValidCodeGuids;

            var instructions = codeGuids.OrderBy(x => Guid.NewGuid()).Take(codeCount)
                .Select(x => new CodeOperation
                {
                    Value = x,
                    Instruction = OperationInstruction.Add,
                    Source = CodeOperationSource.CodeSystemCode
                });

            return new ClientTermValueSetApiModel
            {
                Name = name,
                AuthorityDescription = "Test Authoring Source Description",
                DefinitionDescription = "Test Purpose Description",
                SourceDescription = "Test Source Description",
                CodeOperations = instructions,
                VersionDate = DateTime.UtcNow,
                ClientCode = "UnitTest"
            };
        }

        public static IEnumerable<CodeSystemCodeApiModel> CodeSetCodeApiModelCollection(int count = 10)
        {
            var codeSystems = new List<Guid>
            {
                Guid.NewGuid(),
                Guid.NewGuid()
            }.ToArray();

            for (var i = 0; i < count; i++)
            {
                yield return CodeSetCodeApiModel($"Code.Set.Code.{i}", $"Code.Set.Code.{i} Name", i % 2 == 0 ? codeSystems[0] : codeSystems[1]);
            }
        }

        public static CodeSystemCodeApiModel CodeSetCodeApiModel(string code, string name, Guid codeSystem)
        {
            return new CodeSystemCodeApiModel
            {
                Code = code,
                Name = name,
                CodeGuid = Guid.NewGuid(),
                CodeSystemGuid = codeSystem,
                CodeSystemName = "Generated code system"
            };
        }
    }
}
