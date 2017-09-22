using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fabric.Terminology.Domain.Models;

namespace Fabric.Terminology.SqlServer.Persistence.Factories
{
    internal interface IValueSetCachingManager
    {
        TResult GetOrQuery<TResult>(Guid valueSetGuid, Func<Guid, string> getCacheKey, Func<Guid, TResult> doQuery)
            where TResult : class;

        Task<Dictionary<Guid, IReadOnlyCollection<TResult>>> GetCachedValueDictionary<TResult>(
            IEnumerable<Guid> valueSetGuids,
            Func<Guid, string> getCacheKey,
            Func<IEnumerable<Guid>,
            ILookup<Guid, TResult>> doQuery);

        IReadOnlyCollection<TResult> GetMultipleWithFallBack<TResult>(
            IEnumerable<Guid> valueSetGuids,
            Func<Guid, string> getCacheKey,
            Func<IEnumerable<Guid>,
            ILookup<Guid, TResult>> getLookup) where TResult : class, IHaveValueSetGuid;
    }
}