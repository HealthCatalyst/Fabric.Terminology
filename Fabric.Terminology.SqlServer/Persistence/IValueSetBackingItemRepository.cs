namespace Fabric.Terminology.SqlServer.Persistence
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

        Maybe<IValueSetBackingItem> GetValueSetBackingItem(Guid valueSetGuid, IEnumerable<Guid> codeSystemGuids);

        IReadOnlyCollection<IValueSetBackingItem> GetValueSetBackingItemVersions(string valueSetReferenceId);

        IReadOnlyCollection<IValueSetBackingItem> GetValueSetBackingItems(IEnumerable<Guid> valueSetGuids);

        IReadOnlyCollection<IValueSetBackingItem> GetValueSetBackingItems(
            IEnumerable<Guid> valueSetGuids,
            IEnumerable<Guid> codeSystemGuids);

        Task<PagedCollection<IValueSetBackingItem>> GetValueSetBackingItemsAsync(
            IPagerSettings pagerSettings,
            IEnumerable<Guid> codeSystemGuids,
            bool latestVersionsOnly = true);

        Task<PagedCollection<IValueSetBackingItem>> GetValueSetBackingItemsAsync(
            string filterText,
            IPagerSettings pagerSettings,
            IEnumerable<Guid> codeSystemGuids,
            bool latestVersionsOnly = true);

        bool ValueSetGuidExists(Guid valueSetGuid);
    }
}
