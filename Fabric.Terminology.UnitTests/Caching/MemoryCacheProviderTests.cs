namespace Fabric.Terminology.UnitTests.Caching
{
    using System;

    using Catalyst.Infrastructure.Caching;

    using Fabric.Terminology.SqlServer;
    using Fabric.Terminology.SqlServer.Caching;
    using Fabric.Terminology.TestsBase.Fixtures;
    using Fabric.Terminology.TestsBase.Mocks;

    using FluentAssertions;
    using Xunit;

    public class MemoryCacheProviderTests : IClassFixture<AppConfigurationFixture>
    {
        private readonly AppConfigurationFixture fixture;

        public MemoryCacheProviderTests(AppConfigurationFixture classFixture)
        {
            this.fixture = classFixture;
        }

        [Fact]
        public void GetItemReturnsNullWhenNotFound()
        {
            // Arrange
            var cache = new MemoryCacheProvider(this.fixture.AppConfiguration.TerminologySqlSettings.AsMemoryCacheProviderSettings());
            const string Key = "key";

            // Act
            var val = cache.GetItem<object>(Key);

            // Assert
            val.HasValue.Should().BeFalse();
        }

        [Fact]
        public void GetItemCanAddToCache()
        {
            // Arrange
            var cache = new MemoryCacheProvider(this.fixture.AppConfiguration.TerminologySqlSettings.AsMemoryCacheProviderSettings());
            const string Key = "key";
            var dt = DateTime.Now;
            var obj = new TestObject {Text = "Test", Stamp = dt };

            // Act
            var tmp = cache.GetItem(Key, () => obj);
            var cached = cache.GetItem<TestObject>(Key);

            // Assert
            cached.HasValue.Should().BeTrue();
            cached.Single().Stamp.Should().Be(dt);
        }

        [Fact]
        public void ClearItemClearsSingleItem()
        {
            // Arrange
            var cache = new MemoryCacheProvider(this.fixture.AppConfiguration.TerminologySqlSettings.AsMemoryCacheProviderSettings());
            const string Key1 = "key1";
            const string Key2 = "key2";
            const string Key3 = "key3";
            var obj1 = new TestObject { Text = "Test1", Stamp = DateTime.Now };
            var obj2 = new TestObject { Text = "Test2", Stamp = DateTime.Now };
            var obj3 = new TestObject { Text = "Test3", Stamp = DateTime.Now };

            cache.GetItem(Key1, () => obj1);
            cache.GetItem(Key2, () => obj2);
            cache.GetItem(Key3, () => obj3);

            // Act
            cache.GetItem<TestObject>(Key2).HasValue.Should().BeTrue();
            cache.ClearItem(Key2);

            // Assert
            cache.GetItem<TestObject>(Key2).HasValue.Should().BeFalse();
            cache.GetItem<TestObject>(Key1).HasValue.Should().BeTrue();
            cache.GetItem<TestObject>(Key3).HasValue.Should().BeTrue();
        }

        [Fact]
        public void ClearAllClearsAllCache()
        {
            // Arrange
            var cache = new MemoryCacheProvider(this.fixture.AppConfiguration.TerminologySqlSettings.AsMemoryCacheProviderSettings());
            const string Key1 = "key1";
            const string Key2 = "key2";
            const string Key3 = "key3";
            var obj1 = new TestObject { Text = "Test1", Stamp = DateTime.Now };
            var obj2 = new TestObject { Text = "Test2", Stamp = DateTime.Now };
            var obj3 = new TestObject { Text = "Test3", Stamp = DateTime.Now };

            cache.GetItem(Key1, () => obj1);
            cache.GetItem(Key2, () => obj2);
            cache.GetItem(Key3, () => obj3);

            cache.GetItem<TestObject>(Key1).Should().NotBeNull();
            cache.GetItem<TestObject>(Key2).Should().NotBeNull();
            cache.GetItem<TestObject>(Key3).Should().NotBeNull();
            // Act
            cache.ClearAll();

            // Assert
            cache.GetItem<TestObject>(Key1).HasValue.Should().BeFalse();
            cache.GetItem<TestObject>(Key2).HasValue.Should().BeFalse();
            cache.GetItem<TestObject>(Key3).HasValue.Should().BeFalse();
        }

        [Fact]
        public void GetItemAddsItemByFunction()
        {
            // Arrange
            var cache = new MemoryCacheProvider(this.fixture.AppConfiguration.TerminologySqlSettings.AsMemoryCacheProviderSettings());
            const string Key = "key";
            cache.GetItem<TestObject>(Key).HasValue.Should().BeFalse();

            // Act
            cache.GetItem(Key, () => new TestObject { Text = "Test string", Stamp = DateTime.Now });

            // Assert
            cache.GetItem<TestObject>(Key).HasValue.Should().BeTrue();
        }
    }
}
