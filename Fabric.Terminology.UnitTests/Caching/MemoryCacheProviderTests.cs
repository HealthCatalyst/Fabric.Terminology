using System;
using Fabric.Terminology.SqlServer.Caching;
using FluentAssertions;
using Xunit;

namespace Fabric.Terminology.UnitTests.Caching
{
    public class MemoryCacheProviderTests 
    {        
        [Fact]
        public void GetItem_ReturnsNullWhenNotFound()
        {
            // Arrange
            var cache = new MemoryCacheProvider();
            const string key = "key";

            // Act
            var val = cache.GetItem(key);

            // Assert
            Assert.Null(val);
        }

        [Fact]
        public void GetItem_CanAddToCache()
        {
            // Arrange
            var cache = new MemoryCacheProvider();
            const string key = "key";
            var dt = DateTime.Now;
            var obj = new TestObject {Text = "Test", Stamp = dt };

            // Act           
            var tmp = cache.GetItem(key, () => obj);
            var cached = cache.GetItem(key) as TestObject;

            // Assert
            Assert.NotNull(cached);
            Assert.Equal(dt, cached.Stamp);
        }

        [Fact]
        public void ClearItem_ClearsSingleItem()
        {
            // Arrange
            var cache = new MemoryCacheProvider();
            var key1 = "key1";
            var key2 = "key2";
            var key3 = "key3";
            var obj1 = new TestObject { Text = "Test1", Stamp = DateTime.Now };
            var obj2 = new TestObject { Text = "Test2", Stamp = DateTime.Now };
            var obj3 = new TestObject { Text = "Test3", Stamp = DateTime.Now };

            cache.GetItem(key1, () => obj1);
            cache.GetItem(key2, () => obj2);
            cache.GetItem(key3, () => obj3);

            // Act
            cache.GetItem(key2).Should().NotBeNull();
            
            cache.ClearItem(key2);

            // Assert
            cache.GetItem(key2).Should().BeNull();
            cache.GetItem(key1).Should().NotBeNull();
            cache.GetItem(key3).Should().NotBeNull();

        }

        [Fact]
        public void ClearAll_ClearsAllCache()
        {
            // Arrange
            var cache = new MemoryCacheProvider();
            const string key1 = "key1";
            const string key2 = "key2";
            const string key3 = "key3";
            var obj1 = new TestObject { Text = "Test1", Stamp = DateTime.Now };
            var obj2 = new TestObject { Text = "Test2", Stamp = DateTime.Now };
            var obj3 = new TestObject { Text = "Test3", Stamp = DateTime.Now };

            cache.GetItem(key1, () => obj1);
            cache.GetItem(key2, () => obj2);
            cache.GetItem(key3, () => obj3);

            cache.GetItem(key1).Should().NotBeNull();
            cache.GetItem(key2).Should().NotBeNull();
            cache.GetItem(key3).Should().NotBeNull();

            var instanceKey = cache.InstanceKey;

            // Act
            cache.ClearAll();

            // Assert
            cache.GetItem(key1).Should().BeNull();
            cache.GetItem(key2).Should().BeNull();
            cache.GetItem(key3).Should().BeNull();
            instanceKey.Should().NotBe(cache.InstanceKey);
        }

        [Fact]
        public void GetItem_AddsItemByFunction()
        {
            // Arrange
            var cache = new MemoryCacheProvider();
            const string key = "key";
            cache.GetItem(key).Should().BeNull();

            // Act
            cache.GetItem(key, () => new TestObject {Text = "Test string", Stamp = DateTime.Now});

            // Assert
            cache.GetItem(key).Should().NotBeNull();
        }

        private class TestObject
        {
            public string Text { get; set; }
            public DateTime Stamp { get; set; }
        }
    }
}
