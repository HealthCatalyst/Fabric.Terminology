namespace Fabric.Terminology.IntegrationTests.Services
{
    using Fabric.Terminology.Domain.Services;
    using Fabric.Terminology.IntegrationTests.Fixtures;
    using Fabric.Terminology.TestsBase;

    using JetBrains.Annotations;

    using Xunit;
    using Xunit.Abstractions;

    public class ValueSetServiceTests : OutputTestBase, IClassFixture<ValueSetServiceFixture>
    {
        private IValueSetService valueSetService;

        public ValueSetServiceTests(ValueSetServiceFixture fixture, [NotNull] ITestOutputHelper output)
            : base(output)
        {
            this.valueSetService = fixture.ValueSetService;
        }
    }
}