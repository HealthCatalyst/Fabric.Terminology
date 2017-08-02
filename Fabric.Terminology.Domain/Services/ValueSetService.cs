namespace Fabric.Terminology.Domain.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.Domain.Persistence;
    using Fabric.Terminology.Domain.Strategy;

    using JetBrains.Annotations;

    public class ValueSetService : IValueSetService
    {
        private readonly IValueSetRepository repository;

        private readonly IIdentifyIsCustomStrategy identifyIsCustom;

        public ValueSetService(IValueSetRepository valueSetRepository, IIdentifyIsCustomStrategy identifyIsCustom)
        {
            this.repository = valueSetRepository;
            this.identifyIsCustom = identifyIsCustom;
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
        public IValueSet GetValueSet(string valueSetId, IEnumerable<string> codeSystemCodes)
        {
            return this.repository.GetValueSet(valueSetId, codeSystemCodes);
        }

        public IReadOnlyCollection<IValueSet> GetValueSets(IEnumerable<string> valueSetIds)
        {
            return this.GetValueSets(valueSetIds, new string[] { });
        }

        public IReadOnlyCollection<IValueSet> GetValueSets(
            IEnumerable<string> valueSetIds,
            IEnumerable<string> codeSystemCodes)
        {
            return this.repository.GetValueSets(valueSetIds, codeSystemCodes, true);
        }

        public IReadOnlyCollection<IValueSet> GetValueSetSummaries(IEnumerable<string> valueSetIds)
        {
            return this.GetValueSetSummaries(valueSetIds, new string[] { });
        }

        public IReadOnlyCollection<IValueSet> GetValueSetSummaries(
            IEnumerable<string> valueSetIds,
            IEnumerable<string> codeSystemCodes)
        {
            return this.repository.GetValueSets(valueSetIds, codeSystemCodes, false);
        }

        public Task<PagedCollection<IValueSet>> GetValueSetsAsync(IPagerSettings settings)
        {
            return this.GetValueSetsAsync(settings, new string[] { });
        }

        public Task<PagedCollection<IValueSet>> GetValueSetsAsync(
            IPagerSettings settings,
            IEnumerable<string> codeSystemCodes)
        {
            return this.repository.GetValueSetsAsync(settings, codeSystemCodes, true);
        }

        public Task<PagedCollection<IValueSet>> GetValueSetSummariesAsync(IPagerSettings settings)
        {
            return this.GetValueSetSummariesAsync(settings, new string[] { });
        }

        public Task<PagedCollection<IValueSet>> GetValueSetSummariesAsync(
            IPagerSettings settings,
            IEnumerable<string> codeSystemCodes)
        {
            return this.repository.GetValueSetsAsync(settings, codeSystemCodes, false);
        }

        public Task<PagedCollection<IValueSet>> FindValueSetsAsync(
            string nameFilterText,
            IPagerSettings pagerSettings,
            bool includeAllValueSetCodes = false)
        {
            return this.FindValueSetsAsync(nameFilterText, pagerSettings, new string[] { }, includeAllValueSetCodes);
        }

        public Task<PagedCollection<IValueSet>> FindValueSetsAsync(
            string nameFilterText,
            IPagerSettings pagerSettings,
            IEnumerable<string> codeSystemCodes,
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

        public Attempt<IValueSet> Create(
            string name,
            IValueSetMeta meta,
            IEnumerable<IValueSetCode> valueSetCodes)
        {
            if (!this.NameIsUnique(name))
            {
                return Attempt<IValueSet>.Failed(new ArgumentException($"A value set named '{name}' already exists."));
            }

            if (!ValidateValueSetMeta(meta, out string msg))
            {
                return Attempt<IValueSet>.Failed(new ArgumentException(msg));
            }

            var setCodes = valueSetCodes as IValueSetCode[] ?? valueSetCodes.ToArray();
            if (!setCodes.Any())
            {
                return Attempt<IValueSet>.Failed(new ArgumentException("A value set must include at least one code."));
            }

            var valueSetId = Guid.NewGuid().ToString();
            var valueSet = new ValueSet
            {
                ValueSetId = valueSetId,
                ValueSetOId = valueSetId,
                ValueSetUniqueId = valueSetId,
                Name = name,
                AuthoringSourceDescription = meta.AuthoringSourceDescription,
                PurposeDescription = meta.PurposeDescription,
                SourceDescription = meta.SourceDescription,
                VersionDescription = meta.VersionDescription,
                ValueSetCodes = setCodes                    
                        .ToList()
                        .AsReadOnly(),
                IsCustom = true,
                ValueSetCodesCount = setCodes.Count()
            };

            Created?.Invoke(this, valueSet);

            return Attempt<IValueSet>.Successful(valueSet);
        }

        /// <summary>
        /// Saves a <see cref="IValueSet"/>
        /// </summary>
        /// <param name="valueSet">The <see cref="IValueSet"/> to be saved</param>
        /// <remarks>
        /// At this point, we can only save "new" value sets.  Updates are out of scope at the moment - To be discussed.
        /// </remarks>
        public void Save(IValueSet valueSet)
        {
            if (this.identifyIsCustom.Execute(valueSet))
            {             
            }

            throw new InvalidOperationException("ValueSet was not a custom value set and cannot be saved.");
        }

        // TODO need a table to delete
        public void Delete(IValueSet valueSet)
        {
            if (this.identifyIsCustom.Execute(valueSet))
            {

            }

            // assert is custom
            throw new InvalidOperationException("ValueSet was not a custom value set and cannot be saved.");
        }

        private static bool ValidateValueSetMeta(IValueSetMeta meta, out string msg)
        {
            var errors = new List<string>
            {
                ValidateProperty("AuthoringSourceDescription", meta.AuthoringSourceDescription),
                ValidateProperty("PurposeDescription", meta.PurposeDescription),
                ValidateProperty("SourceDescription", meta.SourceDescription),
                ValidateProperty("VersionDescription", meta.VersionDescription)
            };

            msg = string.Join(", ", errors.Where(m => !m.IsNullOrWhiteSpace()));

            return msg.IsNullOrWhiteSpace();
        }

        private static string ValidateProperty(string propName, string value)
        {
            return value.IsNullOrWhiteSpace() ? $"The {propName} property must have a value. " : string.Empty;
        }
    }
}