using Fabric.Terminology.Domain.Caching;

namespace Fabric.Terminology.Domain.Persistence
{
    public abstract class SqlPageableRepositoryBase : PageableRepositoryBase
    {
        private readonly IMemoryCacheProvider _cache;

        // TODO we should inject a logger here for error catching and debuging.  Discuss with JJ
        // private readonly ILog _logger;

        // TODO decide which ORM we are going to go with - e.g. Dapper or EF.

        protected SqlPageableRepositoryBase(IMemoryCacheProvider cache)
        {
            _cache = cache;
        }
    }
}