namespace Fabric.Terminology.SqlServer.Caching
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using CallMeMaybe;

    using Fabric.Terminology.Domain.Models;

    public interface IValueSetCachingManager<TResult>
        where TResult : class, IHaveValueSetGuid
    {
        Maybe<TResult> GetOrSet(Guid valueSetGuid, Func<TResult> value);

        IReadOnlyCollection<TResult> GetMultipleOrQuery(
            Guid valueSetGuid,
            Func<Guid, IReadOnlyCollection<TResult>> doQuery);

        Task<Dictionary<Guid, IReadOnlyCollection<TResult>>> GetCachedValueDictionary(
            IEnumerable<Guid> valueSetGuids,
            Func<IEnumerable<Guid>, ILookup<Guid, TResult>> doQuery);

        IReadOnlyCollection<TResult> GetMultipleExisting(IEnumerable<Guid> valueSetGuids);

        void Clear(Guid valueSetGuid);
    }
}