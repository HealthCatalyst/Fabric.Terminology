namespace Fabric.Terminology.Domain.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using CallMeMaybe;

    using Fabric.Terminology.Domain.Exceptions;
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

        public static event EventHandler<IValueSet> Created;

        public static event EventHandler<IValueSet> Saving;

        public static event EventHandler<IValueSet> Saved;

        public static event EventHandler<IValueSet> Deleting;

        public static event EventHandler<IValueSet> Deleted;


        public Maybe<IValueSet> GetValueSet(Guid valueSetGuid)
        {
            throw new NotImplementedException();
        }

        public Maybe<IValueSet> GetValueSet(Guid valueSetGuid, IEnumerable<Guid> codeSystemGuids)
        {
            throw new NotImplementedException();
        }

        public IReadOnlyCollection<IValueSet> GetValueSets(IEnumerable<Guid> valueSetGuids)
        {
            throw new NotImplementedException();
        }

        public IReadOnlyCollection<IValueSet> GetValueSets(IEnumerable<Guid> valueSetGuids, IEnumerable<Guid> codeSystemGuids)
        {
            throw new NotImplementedException();
        }

        public IReadOnlyCollection<IValueSetSummary> GetValueSetSummaries(IEnumerable<Guid> valueSetGuid)
        {
            throw new NotImplementedException();
        }

        public IReadOnlyCollection<IValueSetSummary> GetValueSetSummaries(IEnumerable<Guid> valueSetGuids, IEnumerable<Guid> codeSystemGuids)
        {
            throw new NotImplementedException();
        }

        public Task<PagedCollection<IValueSet>> GetValueSetsAsync(IPagerSettings settings)
        {
            throw new NotImplementedException();
        }

        public Task<PagedCollection<IValueSet>> GetValueSetsAsync(IPagerSettings settings, IEnumerable<Guid> codeSystemGuids)
        {
            throw new NotImplementedException();
        }

        public Task<PagedCollection<IValueSetSummary>> GetValueSetSummariesAsync(IPagerSettings settings)
        {
            throw new NotImplementedException();
        }

        public Task<PagedCollection<IValueSetSummary>> GetValueSetSummariesAsync(IPagerSettings settings, IEnumerable<Guid> codeSystemGuids)
        {
            throw new NotImplementedException();
        }

        public Task<PagedCollection<IValueSet>> FindValueSetsAsync(string nameFilterText, IPagerSettings pagerSettings)
        {
            throw new NotImplementedException();
        }

        public Task<PagedCollection<IValueSet>> FindValueSetsAsync(string nameFilterText, IPagerSettings pagerSettings, IEnumerable<Guid> codeSystemGuids)
        {
            throw new NotImplementedException();
        }

        public Task<PagedCollection<IValueSetSummary>> FindValueSetSummariesAsync(string nameFilterText, IPagerSettings pagerSettings)
        {
            throw new NotImplementedException();
        }

        public Task<PagedCollection<IValueSetSummary>> FindValueSetSummariesAsync(string nameFilterText, IPagerSettings pagerSettings, IEnumerable<Guid> codeSystemGuids)
        {
            throw new NotImplementedException();
        }

        public bool NameIsUnique(string name)
        {
            return !this.repository.NameExists(name);
        }

        public Attempt<IValueSet> Create(string name, IValueSetMeta meta, IEnumerable<ICodeSetCode> valueSetCodes)
        {
            throw new NotImplementedException();
        }

        public void Save(IValueSet valueSet)
        {
            throw new NotImplementedException();
        }

        public void Delete(IValueSet valueSet)
        {
            throw new NotImplementedException();
        }

        #region remove

        //public Maybe<IValueSet> GetValueSet(string valueSetUniqueId)
        //{
        //    return this.GetValueSet(valueSetUniqueId, new string[] { });
        //}

        //public Maybe<IValueSet> GetValueSet(string valueSetUniqueId, IEnumerable<string> codeSystemCodes)
        //{
        //    return this.repository.GetValueSet(valueSetUniqueId, codeSystemCodes);
        //}

        //public IReadOnlyCollection<IValueSet> GetValueSets(IEnumerable<string> valueSetUniqueIds)
        //{
        //    return this.GetValueSets(valueSetUniqueIds, new string[] { });
        //}

        //public IReadOnlyCollection<IValueSet> GetValueSets(
        //    IEnumerable<string> valueSetUniqueIds,
        //    IEnumerable<string> codeSystemCodes)
        //{
        //    return this.repository.GetValueSets(valueSetUniqueIds, codeSystemCodes, true);
        //}

        //public IReadOnlyCollection<IValueSet> GetValueSetSummaries(IEnumerable<string> valueSetUniqueIds)
        //{
        //    return this.GetValueSetSummaries(valueSetUniqueIds, new string[] { });
        //}

        //public IReadOnlyCollection<IValueSet> GetValueSetSummaries(
        //    IEnumerable<string> valueSetUniqueIds,
        //    IEnumerable<string> codeSystemCodes)
        //{
        //    return this.repository.GetValueSets(valueSetUniqueIds, codeSystemCodes, false);
        //}

        //public Task<PagedCollection<IValueSet>> GetValueSetsAsync(IPagerSettings settings)
        //{
        //    return this.GetValueSetsAsync(settings, new string[] { });
        //}

        //public Task<PagedCollection<IValueSet>> GetValueSetsAsync(
        //    IPagerSettings settings,
        //    IEnumerable<string> codeSystemCodes)
        //{
        //    return this.repository.GetValueSetsAsync(settings, codeSystemCodes, true);
        //}

        //public Task<PagedCollection<IValueSet>> GetValueSetSummariesAsync(IPagerSettings settings)
        //{
        //    return this.GetValueSetSummariesAsync(settings, new string[] { });
        //}

        //public Task<PagedCollection<IValueSet>> GetValueSetSummariesAsync(
        //    IPagerSettings settings,
        //    IEnumerable<string> codeSystemCodes)
        //{
        //    return this.repository.GetValueSetsAsync(settings, codeSystemCodes, false);
        //}

        //public Task<PagedCollection<IValueSet>> FindValueSetsAsync(
        //    string nameFilterText,
        //    IPagerSettings pagerSettings,
        //    bool includeAllValueSetCodes = false)
        //{
        //    return this.FindValueSetsAsync(nameFilterText, pagerSettings, new string[] { }, includeAllValueSetCodes);
        //}

        //public Task<PagedCollection<IValueSet>> FindValueSetsAsync(
        //    string nameFilterText,
        //    IPagerSettings pagerSettings,
        //    IEnumerable<string> codeSystemCodes,
        //    bool includeAllValueSetCodes = false)
        //{
        //    return this.repository.FindValueSetsAsync(
        //        nameFilterText,
        //        pagerSettings,
        //        codeSystemCodes,
        //        includeAllValueSetCodes);
        //}

        //public bool NameIsUnique(string name)
        //{
        //    return !this.repository.NameExists(name);
        //}

        //public Attempt<IValueSet> Create(
        //    string name,
        //    IValueSetMeta meta,
        //    IEnumerable<ICodeSetCode> codeSetCodes)
        //{
        //    if (!this.NameIsUnique(name))
        //    {
        //        return Attempt<IValueSet>.Failed(new ArgumentException($"A value set named '{name}' already exists."));
        //    }

        //    if (!ValidateValueSetMeta(meta, out string msg))
        //    {
        //        return Attempt<IValueSet>.Failed(new ArgumentException(msg));
        //    }

        //    var setCodes = codeSetCodes as IValueSetCode[] ?? codeSetCodes.ToArray();
        //    if (!setCodes.Any())
        //    {
        //        return Attempt<IValueSet>.Failed(new ArgumentException("A value set must include at least one code."));
        //    }

        //    var emptyId = Guid.Empty.ToString();

        //    var valueSet = new ValueSet();

        //    throw new NotImplementedException();

        //    //var valueSet = new ValueSet(
        //    //    emptyId,
        //    //    name,
        //    //    meta.AuthoringSourceDescription,
        //    //    meta.DefinitionDescription,
        //    //    meta.SourceDescription,
        //    //    meta.VersionDescription,
        //    //    setCodes.Select(code => code.AsCodeForValueSet(emptyId, name)).ToList().AsReadOnly())
        //    //{
        //    //    ValueSetCodesCount = setCodes.Length,
        //    //    IsCustom = true
        //    //};

        //    Created?.Invoke(this, valueSet);

        //    return Attempt<IValueSet>.Successful(valueSet);
        //}

        ///// <summary>
        ///// Saves a <see cref="IValueSet"/>
        ///// </summary>
        ///// <param name="valueSet">The <see cref="IValueSet"/> to be saved</param>
        ///// <remarks>
        ///// At this point, we can only save "new" value sets.  Updates are out of scope at the moment - To be discussed.
        ///// </remarks>
        //public void Save(IValueSet valueSet)
        //{
        //    if (valueSet.IsCustom && this.isCustomValue.Get(valueSet) && valueSet.IsNew())
        //    {
        //        Saving?.Invoke(this, valueSet);

        //        var attempt = this.repository.Add(valueSet);
        //        if (attempt.Success && attempt.Result.HasValue)
        //        {
        //            Saved?.Invoke(this, attempt.Result.Single());
        //            return;
        //        }

        //        if (!attempt.Exception.HasValue)
        //        {
        //            throw new ValueSetOperationException(
        //                "An exception was not returned by the attempt to save a ValueSet but the save failed.");
        //        }

        //        throw attempt.Exception.Single();
        //    }

        //    throw new InvalidOperationException("ValueSet was not a custom value set and cannot be saved.");
        //}

        //public void Delete(IValueSet valueSet)
        //{
        //    if (valueSet.IsCustom && this.isCustomValue.Get(valueSet) && !valueSet.IsNew())
        //    {
        //        Deleting?.Invoke(this, valueSet);

        //        this.repository.Delete(valueSet);

        //        Deleted?.Invoke(this, valueSet);

        //        return;
        //    }

        //    throw new InvalidOperationException("ValueSet was not a custom value set and cannot be deleted.");
        //}

        #endregion

        private static bool ValidateValueSetMeta(IValueSetMeta meta, out string msg)
        {
            var errors = new List<string>
            {
                ValidateProperty("AuthoringSourceDescription", meta.AuthoringSourceDescription),
                ValidateProperty("DefinitionDescription", meta.DefinitionDescription),
                ValidateProperty("SourceDescription", meta.SourceDescription)
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