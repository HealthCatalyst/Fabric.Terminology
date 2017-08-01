namespace Fabric.Terminology.TestsBase.Seeding
{
    using Fabric.Terminology.SqlServer.Persistence.DataContext;
    using Fabric.Terminology.TestsBase.Mocks;

    internal class ClientTermSeededDatabaseInitializer : ISeededDatabaseInitializer<ClientTermContext>
    {
        public void Initialize(ClientTermContext context)
        {
            var valueSet = MockDtoBuilder.BuildValueSetDescriptionDto("test.valueset.1", "Test Value Set 1");

            context.ValueSetDescriptions.Add(valueSet);

            var codes = MockDtoBuilder.BuildValueSetCodeDtoCollection(valueSet);
            foreach (var code in codes)
            {
                context.ValueSetCodes.Add(code);
            }

            context.SaveChanges();
        }
    }
}