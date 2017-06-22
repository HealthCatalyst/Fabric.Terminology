using System;
using System.Threading;
using Fabric.Terminology.SqlServer.Configuration;
using Fabric.Terminology.SqlServer.Threading;
using JetBrains.Annotations;
using Microsoft.Extensions.Caching.Memory;

namespace Fabric.Terminology.SqlServer.Caching
{
    internal class MemoryCacheProvider : IMemoryCacheProvider
    {
        private readonly ReaderWriterLockSlim _locker = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        private IMemoryCache _memCache = Create();

        public MemoryCacheProvider(IMemoryCacheSettings settings)
        {
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            this.InstanceKey = Guid.NewGuid();
        }

        public IMemoryCacheSettings Settings { get; }

        /// Used in tests
        internal Guid InstanceKey { get; private set; }

        public void ClearAll()
        {
            using (new SlimWriterLock(_locker))
            {
                _memCache.Dispose();
                _memCache = Create();
                this.InstanceKey = Guid.NewGuid();
            }
        }

        public void ClearItem(string key)
        {
            using (new SlimWriterLock(_locker))
            {
                if (_memCache.Get(key) == null)
                {
                    return;
                }
                _memCache.Remove(key);
            }
        }

        [CanBeNull]
        public object GetItem(string key)
        {
            object result;
            var success = false;
            using (new SlimWriterLock(_locker))
            {
                success = _memCache.TryGetValue(key, out result);
            }

            return success ? result : null;
        }

        public object GetItem(string key, Func<object> getItem)
        {
            return this.GetItem(key, getItem, TimeSpan.FromMinutes(5), false);
        }

        public object GetItem(string key, Func<object> getItem, TimeSpan? timeout, bool isSliding = false)
        {
            if (!_memCache.TryGetValue(key, out object value))
            {
                value = getItem.Invoke();
                if (value != null)
                {
                    var options = new MemoryCacheEntryOptions();
                    if (timeout.HasValue)
                    {
                        if (isSliding)
                        {
                            options.SetSlidingExpiration(timeout.Value);
                        }
                        else
                        {
                            options.AbsoluteExpiration = DateTime.Now + timeout.Value;
                        }
                    }

                    _memCache.Set(key, value, options);
                }
            }

            return value;
        }

        private static IMemoryCache Create()
        {
            return new MemoryCache(
                new MemoryCacheOptions
                {
                    ExpirationScanFrequency = TimeSpan.FromMinutes(5)
                });
        }
    }
}