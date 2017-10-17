namespace Fabric.Terminology.UnitTests.Caching
{
    using Fabric.Terminology.SqlServer.Caching;
    using Fabric.Terminology.TestsBase.Fixtures;
    using Fabric.Terminology.TestsBase.Mocks;

    public class MockValueSetCachingManagerFixture : TestFixtureBase
    {
        public MockValueSetCachingManagerFixture()
        {
            this.Cache = new ExposedDictionaryCacheProvider();
            this.ValueSetCachingManager = new ValueSetCachingManager<TestObject>(this.Cache);
        }

        public ExposedDictionaryCacheProvider Cache { get; }

        internal IValueSetCachingManager<TestObject> ValueSetCachingManager { get; }
    }
}