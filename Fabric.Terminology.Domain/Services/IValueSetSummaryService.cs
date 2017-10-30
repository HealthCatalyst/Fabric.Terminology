﻿namespace Fabric.Terminology.Domain.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using CallMeMaybe;

    using Fabric.Terminology.Domain.Models;

    public interface IValueSetSummaryService
    {
        Maybe<IValueSetSummary> GetValueSetSummary(Guid valueSetGuid);

        Maybe<IValueSetSummary> GetValueSetSummary(Guid valueSetGuid, IEnumerable<Guid> codeSystemGuids);

        Task<IReadOnlyCollection<IValueSetSummary>> GetValueSetSummariesListAsync(IEnumerable<Guid> valueSetGuids);

        Task<IReadOnlyCollection<IValueSetSummary>> GetValueSetSummariesListAsync(
            IEnumerable<Guid> valueSetGuids,
            IEnumerable<Guid> codeSystemGuids);

        Task<IReadOnlyCollection<IValueSetSummary>> GetValueSetVersionsAsync(string valueSetReferenceId);

        Task<PagedCollection<IValueSetSummary>> GetValueSetSummariesAsync(
            IPagerSettings settings,
            ValueSetStatus statusCode = ValueSetStatus.Active,
            bool latestVersionsOnly = true);

        Task<PagedCollection<IValueSetSummary>> GetValueSetSummariesAsync(
            IPagerSettings settings,
            IEnumerable<Guid> codeSystemGuids,
            ValueSetStatus statusCode = ValueSetStatus.Active,
            bool latestVersionsOnly = true);

        Task<PagedCollection<IValueSetSummary>> GetValueSetSummariesAsync(
            string nameFilterText,
            IPagerSettings pagerSettings,
            ValueSetStatus statusCode = ValueSetStatus.Active,
            bool latestVersionsOnly = true);

        Task<PagedCollection<IValueSetSummary>> GetValueSetSummariesAsync(
            string nameFilterText,
            IPagerSettings pagerSettings,
            IEnumerable<Guid> codeSystemGuids,
            ValueSetStatus statusCode = ValueSetStatus.Active,
            bool latestVersionsOnly = true);
    }
}