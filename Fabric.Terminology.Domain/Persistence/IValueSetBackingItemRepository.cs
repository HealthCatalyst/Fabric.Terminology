namespace Fabric.Terminology.Domain.Persistence
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using CallMeMaybe;

    using Fabric.Terminology.Domain.Models;

    internal interface IValueSetBackingItemRepository
    {
        Maybe<IValueSetBackingItem> GetValueSetBackingItem(Guid valueSetGuid);

        IReadOnlyCollection<IValueSetBackingItem> GetValueSetBackingItems(IEnumerable<Guid> valueSetGuids);

        Task<PagedCollection<IValueSetBackingItem>> GetValueSetBackingItemsAsync(IPagerSettings pagerSettings, IEnumerable<Guid> codeSystemGuids);

        Task<PagedCollection<IValueSetBackingItem>> GetValueSetBackingItemsAsync(string filterText, IPagerSettings pagerSettings, IEnumerable<Guid> codeSystemGuids);
    }
}
