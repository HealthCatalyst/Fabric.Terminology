namespace Fabric.Terminology.API
{
    using System.Linq;

    using AutoMapper;

    using Fabric.Terminology.API.Models;
    using Fabric.Terminology.Domain.Models;

    using FluentValidation.Results;

    public static partial class Extensions
    {
        public static ValueSetApiModel ToValueSetApiModel(
            this IValueSet valueSet,
            bool summary = true,
            int shortListCount = 5)
        {
            var apiModel = Mapper.Map<IValueSet, ValueSetApiModel>(valueSet);
            if (!summary)
            {
                return apiModel;
            }

            apiModel.ValueSetCodes = apiModel.ValueSetCodes.Take(shortListCount).ToList().AsReadOnly();

            return apiModel;
        }

        public static PagedCollection<ValueSetApiModel> ToValueSetApiModelPage(
            this PagedCollection<IValueSet> valuesets,
            bool summaries = true,
            int shortListCount = 5)
        {
            return new PagedCollection<ValueSetApiModel>
            {
                PagerSettings = valuesets.PagerSettings,
                TotalItems = valuesets.TotalItems,
                TotalPages = valuesets.TotalPages,
                Values = valuesets.Values.Select(vs => vs.ToValueSetApiModel(summaries, shortListCount)).ToList()
            };
        }

        public static ICodeSetCode ToCodeSetCode(this CodeSetCodeApiModel model)
        {
            return Mapper.Map<CodeSetCode>(model);
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
                Message = details.Count > 1 ? "Multiple Errors" : details.FirstOrDefault().Message,
                Details = details.ToArray()
            };

            return error;
        }
    }
}