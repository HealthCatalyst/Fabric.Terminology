using System.Collections.Generic;
using Fabric.Terminology.Domain.Models;
using Fabric.Terminology.Domain.Persistence;

namespace Fabric.Terminology.Domain.Services
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

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
        public IValueSet GetValueSet(string valueSetId, params string[] codeSystemCodes)
        {
            return this.repository.GetValueSet(valueSetId, codeSystemCodes);
        }

        public IEnumerable<IValueSet> GetValueSets(IEnumerable<string> valueSetIds, params string[] codeSystemCodes)
        {
            return this.repository.GetValueSets(valueSetIds, true, codeSystemCodes);
        }

        public IEnumerable<IValueSet> GetValueSetSummaries(IEnumerable<string> valueSetIds, params string[] codeSystemCodes)
        {
            return this.repository.GetValueSets(valueSetIds, false, codeSystemCodes);
        }

        public Task<PagedCollection<IValueSet>> GetValueSetsAsync(IPagerSettings settings, params string[] codeSystemCodes)
        {
            return this.repository.GetValueSetsAsync(settings, true, codeSystemCodes);
        }

        public Task<PagedCollection<IValueSet>> GetValueSetSummariesAsync(IPagerSettings settings, params string[] codeSystemCodes)
        {
            return this.repository.GetValueSetsAsync(settings, false, codeSystemCodes);
        }

        public Task<PagedCollection<IValueSet>> FindValueSetsAsync(
            string nameFilterText,
            IPagerSettings pagerSettings,
            bool includeAllValueSetCodes = false,
            params string[] codeSystemCodes)
        {
            return this.repository.FindValueSetsAsync(
                nameFilterText,
                pagerSettings,
                includeAllValueSetCodes,
                codeSystemCodes);
        }

        public bool NameIsUnique(string name)
        {
            return this.repository.NameExists(name);
        }

        public Attempt<IValueSet> Create(string name, IValueSetMeta meta, IEnumerable<IValueSetCodeItem> valueSetCodes)
        {
            if (!this.NameIsUnique(name))
            {
                return Attempt<IValueSet>.Failed(new ArgumentException($"A value set named '{name}' already exists."));
            }

            if (!ValidateValueSetMeta(meta, out string msg))
            {
                return Attempt<IValueSet>.Failed(new ArgumentException(msg));
            }

            var valueSetCodeItems = valueSetCodes as IValueSetCodeItem[] ?? valueSetCodes.ToArray();
            if (!valueSetCodeItems.Any())
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
                ValueSetCodes = valueSetCodeItems
                    .Select(
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
                ValueSetCodesCount = valueSetCodeItems.Length
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