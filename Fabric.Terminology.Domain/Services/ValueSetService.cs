namespace Fabric.Terminology.Domain.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.Domain.Persistence;

    using JetBrains.Annotations;

    public class ValueSetService : IValueSetService
    {
        private readonly IValueSetRepository repository;

        public ValueSetService(IValueSetRepository valueSetRepository)
        {
            this.repository = valueSetRepository;
        }

        #region Events

        public static event EventHandler<IValueSet> Created;

        public static event EventHandler<IValueSet> Saving;

        public static event EventHandler<IValueSet> Saved;

        public static event EventHandler<IValueSet> Deleting;

        public static event EventHandler<IValueSet> Deleted;

        #endregion

        [CanBeNull]
        public IValueSet GetValueSet(string valueSetId)
        {
            return this.GetValueSet(valueSetId, new string[] { });
        }

        [CanBeNull]
        public IValueSet GetValueSet(string valueSetId, IReadOnlyCollection<string> codeSystemCodes)
        {
            return this.repository.GetValueSet(valueSetId, codeSystemCodes);
        }

        public IReadOnlyCollection<IValueSet> GetValueSets(IReadOnlyCollection<string> valueSetIds)
        {
            return this.GetValueSets(valueSetIds, new string[] { });
        }

        public IReadOnlyCollection<IValueSet> GetValueSets(IReadOnlyCollection<string> valueSetIds, IReadOnlyCollection<string> codeSystemCodes)
        {
            return this.repository.GetValueSets(valueSetIds, codeSystemCodes, true);
        }

        public IReadOnlyCollection<IValueSet> GetValueSetSummaries(IReadOnlyCollection<string> valueSetIds)
        {
            return this.GetValueSetSummaries(valueSetIds, new string[] { });
        }

        public IReadOnlyCollection<IValueSet> GetValueSetSummaries(IReadOnlyCollection<string> valueSetIds, IReadOnlyCollection<string> codeSystemCodes)
        {
            return this.repository.GetValueSets(valueSetIds, codeSystemCodes, false);
        }

        public Task<PagedCollection<IValueSet>> GetValueSetsAsync(IPagerSettings settings)
        {
            return this.GetValueSetsAsync(settings, new string[] { });
        }

        public Task<PagedCollection<IValueSet>> GetValueSetsAsync(
            IPagerSettings settings,
            IReadOnlyCollection<string> codeSystemCodes)
        {
            return this.repository.GetValueSetsAsync(settings, codeSystemCodes, true);
        }

        public Task<PagedCollection<IValueSet>> GetValueSetSummariesAsync(IPagerSettings settings)
        {
            return this.GetValueSetSummariesAsync(settings, new string[] { });
        }

        public Task<PagedCollection<IValueSet>> GetValueSetSummariesAsync(
            IPagerSettings settings,
            IReadOnlyCollection<string> codeSystemCodes)
        {
            return this.repository.GetValueSetsAsync(settings, codeSystemCodes, false);
        }

        public Task<PagedCollection<IValueSet>> FindValueSetsAsync(string nameFilterText, IPagerSettings pagerSettings, bool includeAllValueSetCodes = false)
        {
            return this.FindValueSetsAsync(nameFilterText, pagerSettings, new string[] { }, includeAllValueSetCodes);
        }

        public Task<PagedCollection<IValueSet>> FindValueSetsAsync(
            string nameFilterText,
            IPagerSettings pagerSettings,
            IReadOnlyCollection<string> codeSystemCodes,
            bool includeAllValueSetCodes = false)
        {
            return this.repository.FindValueSetsAsync(
                nameFilterText,
                pagerSettings,
                codeSystemCodes,
                includeAllValueSetCodes);
        }

        public bool NameIsUnique(string name)
        {
            return this.repository.NameExists(name);
        }

        public Attempt<IValueSet> Create(string name, IValueSetMeta meta, IReadOnlyCollection<IValueSetCodeItem> valueSetCodes)
        {
            if (!this.NameIsUnique(name))
            {
                return Attempt<IValueSet>.Failed(new ArgumentException($"A value set named '{name}' already exists."));
            }

            if (!ValidateValueSetMeta(meta, out string msg))
            {
                return Attempt<IValueSet>.Failed(new ArgumentException(msg));
            }

            if (!valueSetCodes.Any())
            {
                return Attempt<IValueSet>.Failed(new ArgumentException("A value set must include at least one code."));
            }

            // TODO discuss key gen
            var valueSetId = Guid.NewGuid().ToString();
            var valueSet = new ValueSet
            {
                ValueSetId = valueSetId,
                Name = name,
                AuthoringSourceDescription = meta.AuthoringSourceDescription,
                PurposeDescription = meta.PurposeDescription,
                SourceDescription = meta.SourceDescription,
                VersionDescription = meta.VersionDescription,
                ValueSetCodes =
                    valueSetCodes.Select(
                            code => new ValueSetCode
                            {
                                Code = code.Code,
                                CodeSystem = new ValueSetCodeSystem { Code = code.CodeSystemCode },
                                Name = code.Name,
                                RevisionDate = null,
                                ValueSetId = valueSetId
                            })
                        .ToList()
                        .AsReadOnly(),
                IsCustom = true,
                ValueSetCodesCount = valueSetCodes.Count
            };

            Created?.Invoke(this, valueSet);

            return Attempt<IValueSet>.Successful(valueSet);
        }

        // TODO need a table to insert/update
        public void Save(IValueSet valueSet)
        {
            throw new System.NotImplementedException();
        }

        // TODO need a table to delete
        public void Delete(IValueSet valueSet)
        {
            // assert is custom
            throw new System.NotImplementedException();
        }

        private static bool ValidateValueSetMeta(IValueSetMeta meta, out string msg)
        {
            msg = string.Empty;
            msg += ValidateProperty("AuthoringSourceDescription", meta.AuthoringSourceDescription);
            msg += ValidateProperty("PurposeDescription", meta.PurposeDescription);
            msg += ValidateProperty("SourceDescription", meta.SourceDescription);
            msg += ValidateProperty("VersionDescription", meta.VersionDescription);

            msg = msg.Trim();

            return msg.IsNullOrWhiteSpace();
        }

        private static string ValidateProperty(string propName, string value)
        {
            return value.IsNullOrWhiteSpace() ? $"The {propName} property must have a value. " : string.Empty;
        }
    }
}