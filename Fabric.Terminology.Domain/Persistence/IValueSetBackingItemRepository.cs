﻿namespace Fabric.Terminology.Domain.Persistence
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using CallMeMaybe;

    using Fabric.Terminology.Domain.Models;

    public interface IValueSetBackingItemRepository
    {
        bool NameExists(string name);

        Maybe<IValueSetBackingItem> GetValueSetBackingItem(Guid valueSetGuid);

        IReadOnlyCollection<IValueSetBackingItem> GetValueSetBackingItems(string valueSetReferenceId);

        IReadOnlyCollection<IValueSetBackingItem> GetValueSetBackingItems(IEnumerable<Guid> valueSetGuids);

        Task<PagedCollection<IValueSetBackingItem>> GetValueSetBackingItemsAsync(IPagerSettings pagerSettings, IEnumerable<Guid> codeSystemGuids);

        Task<PagedCollection<IValueSetBackingItem>> GetValueSetBackingItemsAsync(string filterText, IPagerSettings pagerSettings, IEnumerable<Guid> codeSystemGuids);
    }
}