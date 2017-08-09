namespace Fabric.Terminology.TestsBase.Seeding
{
    using System.Collections.Generic;
    using System.Linq;

    using Fabric.Terminology.SqlServer.Models.Dto;
    using Fabric.Terminology.SqlServer.Persistence.DataContext;
    using Fabric.Terminology.TestsBase.Mocks;

    internal class ClientTermSeededDatabaseInitializer : ISeededDatabaseInitializer<ClientTermContext>
    {
        public void Initialize(ClientTermContext context)
        {
            var vs1 = MockDtoBuilder.ValueSetDescriptionDto("test.valueset.1", "Test Value Set 1");
            var vs2 = MockDtoBuilder.ValueSetDescriptionDto("test.valueset.2", "Test Value Set 2");

            context.ValueSetDescriptions.Add(vs1);
            context.ValueSetDescriptions.Add(vs2);


            var codes1 = MockDtoBuilder.ValueSetCodeDtoCollection(vs1);
            var codes2 = MockDtoBuilder.ValueSetCodeDtoCollection(vs2);
            var allCodes = new List<ValueSetCodeDto>();
            allCodes.AddRange(codes1.ToArray());
            allCodes.AddRange(codes2.ToArray());

            foreach (var code in allCodes)
            {
                context.ValueSetCodes.Add(code);
            }

            context.SaveChanges();
        }
    }
}