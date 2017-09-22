namespace Fabric.Terminology.SqlServer.Caching
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Fabric.Terminology.Domain.Models;

    internal interface IValueSetCachingManager<TResult>
        where TResult : IHaveValueSetGuid
    {
        TResult GetOrSet(Guid valueSetGuid, TResult value);

        TResult GetOrSet(Guid valueSetGuid, Func<TResult> value);

        TResult GetOrQuery(Guid valueSetGuid, Func<Guid, TResult> doQuery);

        IReadOnlyCollection<TResult> GetMultipleOrQuery(
            Guid valueSetGuid,
            Func<Guid, IReadOnlyCollection<TResult>> doQuery);

        Task<Dictionary<Guid, IReadOnlyCollection<TResult>>> GetCachedValueDictionary(
            IEnumerable<Guid> valueSetGuids,
            Func<IEnumerable<Guid>,
            ILookup<Guid, TResult>> doQuery);

        IReadOnlyCollection<TResult> GetMultipleWithFallBack(
            IEnumerable<Guid> valueSetGuids,
            Func<IEnumerable<Guid>,
            ILookup<Guid, TResult>> getLookup);

        IReadOnlyCollection<TResult> GetMultipleExisting(IEnumerable<Guid> valueSetGuids);
    }
}