namespace Fabric.Terminology.TestsBase.Seeding
{
    using System.Linq;

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

            var codes1 = MockDtoBuilder.ValueSetCodeDtoCollection();
            var codes2 = MockDtoBuilder.ValueSetCodeDtoCollection();
            var allCodes = codes1.Concat(codes2).ToList();

            foreach (var code in allCodes)
            {
                context.ValueSetCodes.Add(code);
            }

            context.SaveChanges();
        }
    }
}