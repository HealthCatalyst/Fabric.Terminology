namespace Fabric.Terminology.SqlServer.Caching
{
    using Catalyst.Infrastructure.Caching;

    using Fabric.Terminology.Domain.Models;

    internal class CachingManagerFactory : ICachingManagerFactory
    {
        private readonly IMemoryCacheProvider cache;

        public CachingManagerFactory(IMemoryCacheProvider cache)
        {
            this.cache = cache;
        }

        public IValueSetCachingManager<TResult> ResolveFor<TResult>()
            where TResult : class, IHaveValueSetGuid
        {
            return new ValueSetCachingManager<TResult>(this.cache);
        }
    }
}