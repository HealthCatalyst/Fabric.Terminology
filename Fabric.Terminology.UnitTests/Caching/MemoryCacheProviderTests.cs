using System;
using System.Collections.Generic;
using System.Text;
using Fabric.Terminology.SqlServer.Caching;
using Xunit;

namespace Fabric.Terminology.UnitTests.Caching
{
    public class MemoryCacheProviderTests
    {
        [Fact]
        public void GetItem_ReturnsNullWhenNotFound()
        {
            //// Arrange
            var cache = new MemoryCacheProvider();
            var key = "key";

            //// Act
            var val = cache.GetItem(key);

            //// Assert
            Assert.Null(val);
        }

        [Fact]
        public void GetItem_CanAddToCache()
        {
            //// Arrange
            var cache = new MemoryCacheProvider();
            var key = "key";
            var dt = DateTime.Now;
            var obj = new TestObject {Text = "Test", Stamp = dt };

            //// Act           
            var tmp = cache.GetItem(key, () => obj);
            var cached = cache.GetItem(key) as TestObject;

            //// Assert
            Assert.NotNull(cached);
            Assert.Equal(dt, cached.Stamp);
        }

        [Fact]
        public void ClearItem_ClearsSingleItem()
        {
            //// Arrange
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

            //// Act
            Assert.NotNull(cache.GetItem(key2));

            cache.ClearItem(key2);

            //// Assert
            Assert.Null(cache.GetItem(key2));
            Assert.NotNull(cache.GetItem(key1));
            Assert.NotNull(cache.GetItem(key3));

        }

        [Fact]
        public void ClearAll_ClearsAllCache()
        {
            //// Arrange
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

            Assert.NotNull(cache.GetItem(key1));
            Assert.NotNull(cache.GetItem(key2));
            Assert.NotNull(cache.GetItem(key3));

            var instanceKey = cache.InstanceKey;

            //// Act
            cache.ClearAll();

            //// Assert
            Assert.Null(cache.GetItem(key1));
            Assert.Null(cache.GetItem(key2));
            Assert.Null(cache.GetItem(key2));
            Assert.NotEqual(instanceKey, cache.InstanceKey);
        }

        [Fact]
        public void GetItem_AddsItemByFunction()
        {
            //// Arrange
            var cache = new MemoryCacheProvider();
            var key = "key";
            Assert.Null(cache.GetItem(key));

            //// Act
            cache.GetItem(key, () => new TestObject {Text = "Test string", Stamp = DateTime.Now});

            //// Assert
            Assert.NotNull(cache.GetItem(key));
        }

        private class TestObject
        {
            public string Text { get; set; }
            public DateTime Stamp { get; set; }
        }
    }
}
