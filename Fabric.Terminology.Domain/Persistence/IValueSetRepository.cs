namespace Fabric.Terminology.Domain.Persistence
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using CallMeMaybe;

    using Fabric.Terminology.Domain.Models;

    public interface IValueSetRepository
    {
        bool NameExists(string name);

        Maybe<IValueSet> GetValueSet(Guid valueSetGuid);

        Maybe<IValueSet> GetValueSet(Guid valueSetGuid, IEnumerable<Guid> codeSystemGuids);

        IReadOnlyCollection<IValueSet> GetValueSets(IEnumerable<Guid> valueSetUniqueGuids, IEnumerable<Guid> codeSystemGuids);

        Task<PagedCollection<IValueSet>> GetValueSetsAsync(IPagerSettings pagerSettings, IEnumerable<Guid> codeSystemGuids);

        Task<PagedCollection<IValueSet>> GetValueSetsAsync(string filterText, IPagerSettings pagerSettings, IEnumerable<Guid> codeSystemGuids);

        Attempt<IValueSet> Add(IValueSet valueSet);

        void Delete(IValueSet valueSet);
    }
}