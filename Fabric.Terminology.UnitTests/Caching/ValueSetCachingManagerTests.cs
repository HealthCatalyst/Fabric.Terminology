namespace Fabric.Terminology.UnitTests.Caching
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Fabric.Terminology.SqlServer.Caching;
    using Fabric.Terminology.TestsBase;
    using Fabric.Terminology.TestsBase.Mocks;

    using FluentAssertions;

    using Xunit;
    using Xunit.Abstractions;

    public class ValueSetCachingManagerTests : OutputTestBase, IClassFixture<MockValueSetCachingManagerFixture>
    {
        private readonly ExposedDictionaryCacheProvider wrappedCache;

        private readonly IValueSetCachingManager<TestObject> cachingManager;

        public ValueSetCachingManagerTests(MockValueSetCachingManagerFixture fixture, ITestOutputHelper output)
            : base(output)
        {
            this.wrappedCache = fixture.Cache;
            this.cachingManager = fixture.ValueSetCachingManager;
        }

        private static event EventHandler WasQueried;

        [Fact]
        public void CanGetOrSetDirect()
        {
            // Arrange
            var text = "CanGetOrSetDirect";
            var co = new TestObject { ValueSetGuid = Guid.NewGuid(), Text = text };

            // Act
            var cached = this.cachingManager.GetOrSet(co.ValueSetGuid, () => co);

            // Assert
            this.wrappedCache.CachedItems.ContainsKey(GetCacheKey(co.ValueSetGuid)).Should().BeTrue();
            cached.Should().BeEquivalentTo(co);
            cached.Text.Should().Be(text);
        }

        [Fact]
        public void CanGetOrSetViaFunction()
        {
            // Arrange
            var text = "CanGetOrSetViaFunction";
            var valueSetGuid = Guid.NewGuid();

            // Act
            var cached = this.cachingManager.GetOrSet(valueSetGuid, () => GetTestObject(valueSetGuid, text));

            // Assert
            this.wrappedCache.CachedItems.ContainsKey(GetCacheKey(valueSetGuid)).Should().BeTrue();
            cached.Text.Should().Be(text);
        }

        [Fact]
        public void GetCachedValueDictionaryDoesCacheItems()
        {
            // Arrange
            var doLookupWasQueried = false;

            var firstCallWasQueried = false;
            var secondCallWasQueried = false;

            var valueSetGuid = Guid.NewGuid();
            var setup = new List<Tuple<Guid, int>>
            {
                new Tuple<Guid, int>(valueSetGuid, 25), // 5 items
            };

            WasQueried += SetWasQueried;

            // Act
            this.Profiler.ExecuteTimed(async () => await this.cachingManager.GetCachedValueDictionary(new[] { valueSetGuid }, FakeGetLookup));
            firstCallWasQueried = doLookupWasQueried;

            doLookupWasQueried = false; // reset

            this.Profiler.ExecuteTimed(async () => await this.cachingManager.GetCachedValueDictionary(new[] { valueSetGuid }, FakeGetLookup));
            secondCallWasQueried = doLookupWasQueried;

            // Assert
            firstCallWasQueried.Should().BeTrue();
            secondCallWasQueried.Should().BeFalse();

            WasQueried -= SetWasQueried;

            void SetWasQueried(object sender, EventArgs e)
            {
                doLookupWasQueried = true;
            }

            ILookup<Guid, TestObject> FakeGetLookup(IEnumerable<Guid> keys)
            {
                return this.GetLookup(setup);
            }
        }

        [Fact]
        public void CanGetCachedValueDictionary()
        {
            // Arrange
            var setup = new List<Tuple<Guid, int>>
            {
                new Tuple<Guid, int>(Guid.NewGuid(), 5), // 5 items
                new Tuple<Guid, int>(Guid.NewGuid(), 3), // 3 items
                new Tuple<Guid, int>(Guid.NewGuid(), 10) // 10 items
            };

            var valueSetGuids = setup.Select(kc => kc.Item1).ToList();

            // Act
            var dictionary1 = this.Profiler.ExecuteTimed(async () => await this.cachingManager.GetCachedValueDictionary(valueSetGuids, FakeGetLookup));

            // Assert
            foreach (var valueSetGuid in valueSetGuids)
            {
                var count = setup.First(t => t.Item1 == valueSetGuid).Item2;

                // Ensure the dictionary has a key for each ValueSetGuid and item count matches
                dictionary1.ContainsKey(valueSetGuid).Should().BeTrue();
                var items = dictionary1[valueSetGuid];
                items.Count.Should().Be(count);

                this.wrappedCache.CachedItems.ContainsKey(GetCacheKey(valueSetGuid)).Should().BeTrue();
            }

            ILookup<Guid, TestObject> FakeGetLookup(IEnumerable<Guid> keys)
            {
                return this.GetLookup(setup);
            }
        }

        private static string GetCacheKey(Guid valueSetGuid) => $"{typeof(TestObject)}-{valueSetGuid}";

        private static TestObject GetTestObject(Guid valueSetGuid, string text) => new TestObject { ValueSetGuid = valueSetGuid, Text = text };

        private ILookup<Guid, TestObject> GetLookup(IEnumerable<Tuple<Guid, int>> keyAndCount)
        {
            var dictionary = new Dictionary<Guid, IReadOnlyCollection<TestObject>>();

            foreach (var kc in keyAndCount)
            {
                var testObjects = new List<TestObject>();
                for (var i = 0; i < kc.Item2; i++)
                {
                    testObjects.Add(new TestObject { ValueSetGuid = Guid.NewGuid(), Text = $"Item {i} - {kc.Item1}" });
                }

                dictionary.Add(kc.Item1, testObjects);
            }

            WasQueried?.Invoke(this, new EventArgs());

            return dictionary
                    .SelectMany(p => p.Value.Select(x => new { p.Key, Value = x }))
                    .ToLookup(pair => pair.Key, pair => pair.Value);
        }
    }
}
