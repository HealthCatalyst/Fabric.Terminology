using System;
using System.Collections.Generic;

namespace Fabric.Terminology.SqlServer.Services
{
    using System.Linq;

    using Fabric.Terminology.Domain;
    using Fabric.Terminology.Domain.Exceptions;
    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.Domain.Services;
    using Fabric.Terminology.SqlServer.Persistence;

    using Serilog;

    internal class SqlClientTermValueSetService : IClientTermValueSetService
    {
        private readonly IValueSetBackingItemRepository valueSetBackingItemRepository;

        private readonly ILogger logger;

        private readonly IClientTermUnitOfWorkRepository clientTermValueSetRepository;

        public SqlClientTermValueSetService(
            ILogger logger,
            IValueSetBackingItemRepository valueSetBackingItemRepository,
            IClientTermUnitOfWorkRepository clientTermValueSetRepository)
        {
            this.valueSetBackingItemRepository = valueSetBackingItemRepository;
            this.clientTermValueSetRepository = clientTermValueSetRepository;
            this.logger = logger;
        }

        public static event EventHandler<IValueSet> Created;

        public static event EventHandler<IValueSet> Saving;

        public static event EventHandler<IValueSet> Saved;

        public static event EventHandler<IValueSet> Deleting;

        public static event EventHandler<IValueSet> Deleted;

        public bool NameIsUnique(string name)
        {
            return !this.valueSetBackingItemRepository.NameExists(name);
        }

        public bool ValueSetGuidIsUnique(Guid valueSetGuid)
        {
            return !this.valueSetBackingItemRepository.ValueSetGuidExists(valueSetGuid);
        }

        public Attempt<IValueSet> Create(string name, IValueSetMeta meta, IReadOnlyCollection<ICodeSystemCode> codeSetCodes)
        {
            if (!this.NameIsUnique(name))
            {
                return Attempt<IValueSet>.Failed(new ArgumentException($"A value set named '{name}' already exists."));
            }

            if (!ValidateValueSetMeta(meta, out string msg))
            {
                return Attempt<IValueSet>.Failed(new ArgumentException(msg));
            }

            var setCodes = codeSetCodes as IValueSetCode[] ?? codeSetCodes.ToArray();
            if (!setCodes.Any())
            {
                return Attempt<IValueSet>.Failed(new ArgumentException("A value set must include at least one code."));
            }

            var valueSet = new ValueSet(name, meta, codeSetCodes) { StatusCode = ValueSetStatus.Draft };
            Created?.Invoke(this, valueSet);
            return Attempt<IValueSet>.Successful(valueSet);
        }

        public void SaveAsNew(IValueSet valueSet)
        {
            Saving?.Invoke(this, valueSet);

            var attempt = this.clientTermValueSetRepository.Add(valueSet);
            if (attempt.Success && attempt.Result.HasValue)
            {
                Saved?.Invoke(this, attempt.Result.Single());
                return;
            }

            if (!attempt.Exception.HasValue)
            {
                var vsex = new ValueSetOperationException(
                    "An exception was not returned by the attempt to save a ValueSet but the save failed.", attempt.Exception.Single());
                this.logger.Error(vsex, "An exception was not returned by the attempt to save a ValueSet but the save failed.");
                throw vsex;
            }

            throw attempt.Exception.Single();
        }

        public void Delete(IValueSet valueSet)
        {
            Deleting?.Invoke(this, valueSet);

            this.clientTermValueSetRepository.Delete(valueSet);

            Deleted?.Invoke(this, valueSet);
        }

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
