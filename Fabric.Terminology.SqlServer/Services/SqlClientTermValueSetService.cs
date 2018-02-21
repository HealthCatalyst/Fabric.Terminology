namespace Fabric.Terminology.SqlServer.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Fabric.Terminology.Domain;
    using Fabric.Terminology.Domain.Exceptions;
    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.Domain.Services;
    using Fabric.Terminology.SqlServer.Persistence;

    using Serilog;

    internal class SqlClientTermValueSetService : IClientTermValueSetService
    {
        private readonly IClientTermValueSetRepository clientTermValueSetRepository;

        private readonly ILogger logger;

        private readonly IValueSetBackingItemRepository valueSetBackingItemRepository;

        public SqlClientTermValueSetService(
            ILogger logger,
            IValueSetBackingItemRepository valueSetBackingItemRepository,
            IClientTermValueSetRepository clientTermValueSetRepository)
        {
            this.valueSetBackingItemRepository = valueSetBackingItemRepository;
            this.clientTermValueSetRepository = clientTermValueSetRepository;
            this.logger = logger;
        }

        public static event EventHandler<IValueSet> Created;

        public static event EventHandler<IValueSet> Saved;

        public static event EventHandler<IValueSet> Deleted;

        public bool NameIsUnique(string name)
        {
            return !this.valueSetBackingItemRepository.NameExists(name);
        }

        public bool ValueSetGuidIsUnique(Guid valueSetGuid)
        {
            return !this.valueSetBackingItemRepository.ValueSetGuidExists(valueSetGuid);
        }

        public Attempt<IValueSet> Create(
            string name,
            IValueSetMeta meta,
            IReadOnlyCollection<ICodeSystemCode> codeSetCodes)
        {
            if (!this.NameIsUnique(name))
            {
                return Attempt<IValueSet>.Failed(new ArgumentException($"A value set named '{name}' already exists."));
            }

            if (!this.ValidateValueSetMeta(meta, out var msg))
            {
                return Attempt<IValueSet>.Failed(new ArgumentException(msg));
            }

            var setCodes = codeSetCodes as IValueSetCode[] ?? codeSetCodes.ToArray();

            var valueSet = new ValueSet(name, meta, setCodes)
            {
                StatusCode = ValueSetStatus.Draft,
                IsCustom = true,
                IsLatestVersion = true
            };

            Created?.Invoke(this, valueSet);
            return Attempt<IValueSet>.Successful(valueSet);
        }

        public Attempt<IValueSet> Patch(ValueSetPatchParameters parameters)
        {
            return this.clientTermValueSetRepository.GetValueSet(parameters.ValueSetGuid)
                .Select(
                    vs =>
                        {
                            if (!vs.Name.Equals(parameters.Name, StringComparison.OrdinalIgnoreCase) && !this.NameIsUnique(parameters.Name))
                            {
                                return Attempt<IValueSet>.Failed(new ArgumentException($"A value set named '{parameters.Name}' already exists."));
                            }

                            if (!this.ValidateValueSetMeta(parameters.ValueSetMeta, out var msg))
                            {
                                return Attempt<IValueSet>.Failed(new ArgumentException(msg));
                            }

                            return this.clientTermValueSetRepository.Patch(parameters);
                        })
                .Else(Attempt<IValueSet>
                        .Failed(new InvalidOperationException(FormattableString.Invariant($"ValueSet with ValueSetGUID {parameters.ValueSetGuid} was not found."))));
        }

        public void SaveAsNew(IValueSet valueSet)
        {
            var attempt = this.clientTermValueSetRepository.Add(valueSet);
            if (attempt.Success && attempt.Result != null)
            {
                Saved?.Invoke(this, attempt.Result);
                return;
            }

            if (attempt.Exception == null)
            {
                var vsex = new ValueSetOperationException(
                    "An exception was not returned by the attempt to save a ValueSet but the save failed.",
                    attempt.Exception);
                this.logger.Error(
                    vsex,
                    "An exception was not returned by the attempt to save a ValueSet but the save failed.");
                throw vsex;
            }

            throw attempt.Exception;
        }

        public Attempt<IValueSet> Copy(IValueSet originalValueSet, string newName, IValueSetMeta meta)
        {
            var attempt = this.Create(newName, meta, originalValueSet.ValueSetCodes);
            if (!attempt.Success || attempt.Result == null)
            {
                return attempt;
            }

            var valueSet = attempt.Result;
            ((ValueSet)valueSet).OriginGuid = originalValueSet.ValueSetGuid;
            this.SaveAsNew(valueSet);

            return Attempt<IValueSet>.Successful(valueSet);
        }

        public Attempt<IValueSet> ChangeStatus(Guid valueSetGuid, ValueSetStatus newStatus)
        {
            var attempt = this.clientTermValueSetRepository.ChangeStatus(valueSetGuid, newStatus);
            if (attempt.Success)
            {
                Saved?.Invoke(this, attempt.Result);
            }

            return attempt;
        }

        public Attempt<IValueSet> AddRemoveCodes(
            Guid valueSetGuid,
            IEnumerable<ICodeSystemCode> codesToAdd,
            IEnumerable<ICodeSystemCode> codesToRemove)
        {
            var listToAdd = codesToAdd.ToList();
            var listToRemove = codesToRemove.ToList();
            var duplicates = GetIntersectingCodeGuids(listToAdd, listToRemove);
            var attempt = !duplicates.Any()
                       ? this.clientTermValueSetRepository.AddRemoveCodes(valueSetGuid, listToAdd, listToRemove)
                       : Attempt<IValueSet>.Failed(new InvalidOperationException($"One or more codes were being attempted to be both add and removed to the value set with ValueSetGuid {valueSetGuid}.  Offending CodeGuid(s) {string.Join(",", duplicates)}"));

            if (attempt.Success)
            {
                Saved?.Invoke(this, attempt.Result);
            }

            return attempt;
        }

        public void Delete(IValueSet valueSet)
        {
            this.clientTermValueSetRepository.Delete(valueSet);

            Deleted?.Invoke(this, valueSet);
        }

        private static string ValidateProperty(string propName, string value)
        {
            return value.IsNullOrWhiteSpace() ? $"The {propName} property must have a value. " : string.Empty;
        }

        private static IReadOnlyCollection<Guid> GetIntersectingCodeGuids(IReadOnlyCollection<ICodeSystemCode> list1, IReadOnlyCollection<ICodeSystemCode> list2)
        {
            var list1Guids = list1.Select(x => x.CodeGuid).ToHashSet();
            var list2Guids = list2.Select(x => x.CodeGuid).ToHashSet();
            return list1Guids.Intersect(list2Guids).ToList();
        }

        private bool ValidateValueSetMeta(IValueSetMeta meta, out string msg)
        {
            var errors = new List<string>
            {
                ValidateProperty(nameof(meta.ClientCode), meta.ClientCode),
                ValidateProperty(nameof(meta.AuthorityDescription), meta.AuthorityDescription),
                ValidateProperty(nameof(meta.DefinitionDescription), meta.DefinitionDescription),
                ValidateProperty(nameof(meta.SourceDescription), meta.SourceDescription)
            };

            msg = string.Join(", ", errors.Where(m => !m.IsNullOrWhiteSpace()));

            return msg.IsNullOrWhiteSpace();
        }
    }
}