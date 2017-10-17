namespace Fabric.Terminology.API
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using AutoMapper;

    using Fabric.Terminology.API.Models;
    using Fabric.Terminology.Domain.Models;

    using FluentValidation.Results;

    public static partial class Extensions
    {
        public static ValueSetApiModel ToValueSetApiModel(this IValueSet valueSet) =>
            valueSet.ToValueSetApiModel(new List<Guid>());

        public static ValueSetApiModel ToValueSetApiModel(
            this IValueSet valueSet,
            IReadOnlyCollection<Guid> codeSystemGuids)
        {
            var apiModel = Mapper.Map<IValueSet, ValueSetApiModel>(valueSet);
            if (codeSystemGuids.Any())
            {
                apiModel.CodeCounts = apiModel.CodeCounts.Where(cc => codeSystemGuids.Contains(cc.CodeSystemGuid)).ToList();
                apiModel.ValueSetCodes = apiModel.ValueSetCodes.Where(csc => codeSystemGuids.Contains(csc.CodeSystemGuid)).ToList();
            }

            return apiModel;
        }

        public static ValueSetItemApiModel ToValueSetItemApiModel(this IValueSetSummary valueSetSummary) =>
            valueSetSummary.ToValueSetItemApiModel(new List<Guid>());

        public static ValueSetItemApiModel ToValueSetItemApiModel(
            this IValueSetSummary valueSetSummary,
            IReadOnlyCollection<Guid> codeSystemGuids)
        {
            var apiModel = Mapper.Map<IValueSetSummary, ValueSetItemApiModel>(valueSetSummary);
            if (codeSystemGuids.Any())
            {
                apiModel.CodeCounts = apiModel.CodeCounts.Where(cc => codeSystemGuids.Contains(cc.CodeSystemGuid)).ToList();
            }

            return apiModel;
        }

        public static PagedCollection<ValueSetItemApiModel> ToValueSetApiModelPage<T>(this PagedCollection<T> items, IReadOnlyCollection<Guid> codeSystemGuids, Func<T, IReadOnlyCollection<Guid>, ValueSetItemApiModel> mapper)
            where T : IValueSetSummary
        {
            return new PagedCollection<ValueSetItemApiModel>
            {
                PagerSettings = items.PagerSettings,
                TotalItems = items.TotalItems,
                TotalPages = items.TotalPages,
                Values = items.Values.Select(vsi => mapper(vsi, codeSystemGuids)).ToList()
            };
        }

        public static PagedCollection<CodeSystemCodeApiModel> ToCodeSystemCodeApiModelPage(
            this PagedCollection<ICodeSystemCode> items)
        {
            return new PagedCollection<CodeSystemCodeApiModel>
            {
                PagerSettings = items.PagerSettings,
                TotalItems = items.TotalItems,
                TotalPages = items.TotalPages,
                Values = items.Values.Select(Mapper.Map<CodeSystemCodeApiModel>).ToList()
            };
        }

        // acquired from Fabric.Authorization.Domain (renamed from ToError)
        public static Error ToError(this ValidationResult validationResult)
        {
            var details = validationResult.Errors.Select(
                    validationResultError => new Error
                    {
                        Code = validationResultError.ErrorCode,
                        Message = validationResultError.ErrorMessage,
                        Target = validationResultError.PropertyName
                    })
                .ToList();

            var error = new Error
            {
                Message = details.Count > 1 ? "Multiple Errors" : details.First().Message,
                Details = details.ToArray()
            };

            return error;
        }
    }
}