namespace Fabric.Terminology.API.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using AutoMapper;

    using CallMeMaybe;

    using Fabric.Terminology.API.Configuration;
    using Fabric.Terminology.API.Models;
    using Fabric.Terminology.API.Validators;
    using Fabric.Terminology.Domain.Exceptions;
    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.Domain.Services;

    using Nancy;
    using Nancy.ModelBinding;

    using Serilog;

    public sealed class ValueSetModule : TerminologyModule<ValueSetApiModel>
    {
        private readonly IValueSetService valueSetService;

        private readonly IValueSetSummaryService valueSetSummaryService;

        private readonly IClientTermValueSetService clientTermValueSetService;

        private readonly ValueSetValidator valueSetValidator;

        public ValueSetModule(
                IValueSetService valueSetService,
                IValueSetSummaryService valueSetSummaryService,
                IClientTermValueSetService clientTermValueSetService,
                IAppConfiguration config,
                ILogger logger,
                ValueSetValidator valueSetValidator)
            : base($"/{TerminologyVersion.Route}/valuesets", config, logger)
        {
            this.valueSetService = valueSetService;
            this.valueSetSummaryService = valueSetSummaryService;
            this.clientTermValueSetService = clientTermValueSetService;
            this.valueSetValidator = valueSetValidator;

            this.Get("/", _ => this.GetValueSetPage(), null, "GetPaged");

            this.Get("/{valueSetGuid}", parameters => this.GetValueSets(parameters.valueSetGuid), null, "GetValueSet");

            this.Get("/versions/{referenceId}", parameters => this.GetValueSetVersions(parameters.referenceId), null, "GetValueSetVersions");

            this.Post("/search/", _ => this.Search(), null, "Search");

            this.Post("/", _ => this.AddValueSet(), null, "AddValueSet");

           // this.Delete("/{valueSetGuid}", parameters => this.DeleteValueSet(parameters), null, "DeleteValueSet");
        }

        private static ValueSetApiModel MapToValueSetApiModel(IValueSet vs, IReadOnlyCollection<Guid> codeSystemGuids) =>
            vs.ToValueSetApiModel(codeSystemGuids);

        private static ValueSetItemApiModel MapToValueSetItemApiModel(IValueSetSummary vss, IReadOnlyCollection<Guid> codeSystemGuids) =>
                vss.ToValueSetItemApiModel(codeSystemGuids);

        private static async Task<T> Execute<T>(Func<Task<T>> query) => await query.Invoke();

        private object GetValueSet(Guid valueSetGuid, IReadOnlyCollection<Guid> codeSystemGuids, bool summary = true)
        {
            try
            {
                var model = summary ? (Maybe<object>)this.valueSetSummaryService
                                        .GetValueSetSummary(valueSetGuid, codeSystemGuids)
                                        .Select(vs => vs.ToValueSetItemApiModel(codeSystemGuids)) :

                                      this.valueSetService
                                        .GetValueSet(valueSetGuid, codeSystemGuids)
                                        .Select(vs => vs.ToValueSetApiModel(codeSystemGuids));

                return model.Else(() => this.CreateFailureResponse("ValueSet with matching ID was not found", HttpStatusCode.NotFound));
            }
            catch (ValueSetNotFoundException ex)
            {
                this.Logger.Error(ex, ex.Message, valueSetGuid);
                return this.CreateFailureResponse(
                    $"The ValueSet with id: {valueSetGuid} was not found.",
                    HttpStatusCode.InternalServerError);
            }
        }

        private object GetValueSets(string valueSetGuids)
        {
            try
            {
                var guids = this.GetValueSetGuids(valueSetGuids);
                var codeSystemGuids = this.GetCodeSystems();
                var summary = this.GetSummarySetting();

                if (guids.Length == 1)
                {
                    return this.GetValueSet(guids[0], codeSystemGuids, summary);
                }

                if (!valueSetGuids.Any())
                {
                    return this.CreateFailureResponse("An array of value set ids is required.", HttpStatusCode.BadRequest);
                }

                return summary
                           ? Execute(() => this.valueSetSummaryService.GetValueSetSummariesListAsync(guids, codeSystemGuids))
                               .Result.Select(vss => vss.ToValueSetItemApiModel(codeSystemGuids))
                               .ToList()

                           : (IReadOnlyCollection<object>)Execute(() => this.valueSetService.GetValueSetsListAsync(guids, codeSystemGuids))
                               .Result.Select(vs => vs.ToValueSetApiModel(codeSystemGuids))
                               .ToList();
            }
            catch (Exception ex)
            {
                this.Logger.Error(ex, ex.Message);
                return this.CreateFailureResponse(
                    "Failed to retrieve value sets",
                    HttpStatusCode.InternalServerError);
            }
        }

        private object GetValueSetVersions(string valueSetReferenceId)
        {
            try
            {
                var codeSystemGuids = this.GetCodeSystems();
                var summary = this.GetSummarySetting();



                var versions = summary
                        ? Execute(() => this.valueSetSummaryService.GetValueSetVersionsAsync(valueSetReferenceId)).Result
                            .Select(vss => vss.ToValueSetItemApiModel(codeSystemGuids))

                        : Execute(() => this.valueSetSummaryService.GetValueSetVersionsAsync(valueSetReferenceId)).Result
                            .Select(vs => vs.ToValueSetItemApiModel(codeSystemGuids)).ToList();

                if (!versions.Any())
                {
                    return this.CreateFailureResponse("ValueSet with matching ValueSetReferenceID was not found", HttpStatusCode.NotFound);
                }

                return versions;
            }
            catch (Exception ex)
            {
                this.Logger.Error(ex, ex.Message);
                return this.CreateFailureResponse("Failed to retrieve value sets", HttpStatusCode.InternalServerError);
            }
        }

        private async Task<object> GetValueSetPage()
        {
            try
            {
                var summary = this.GetSummarySetting();
                var pagerSettings = this.GetPagerSettings();
                var codeSystemGuids = this.GetCodeSystems();

                return summary
                           ? (await this.valueSetSummaryService.GetValueSetSummariesAsync(pagerSettings, codeSystemGuids))
                                .ToValueSetApiModelPage(codeSystemGuids, MapToValueSetItemApiModel)

                           : (await this.valueSetService.GetValueSetsAsync(pagerSettings, codeSystemGuids))
                                .ToValueSetApiModelPage(codeSystemGuids, MapToValueSetApiModel);
            }
            catch (Exception ex)
            {
                this.Logger.Error(ex, ex.Message);
                return this.CreateFailureResponse(
                    "Failed to retrieve the page of value sets",
                    HttpStatusCode.InternalServerError);
            }
        }

        private async Task<object> Search()
        {
            try
            {
                var model = this.EnsureQueryModel(this.Bind<FindByTermQuery>(new BindingConfig { BodyOnly = true }));

                var codeSystemGuids = model.CodeSystemGuids.ToList();

                return model.Summary
                           ? (await this.valueSetSummaryService.GetValueSetSummariesAsync(
                                  model.Term,
                                  model.PagerSettings,
                                  codeSystemGuids)).ToValueSetApiModelPage(codeSystemGuids, MapToValueSetItemApiModel)

                           : (await this.valueSetService.GetValueSetsAsync(
                                  model.Term,
                                  model.PagerSettings,
                                  codeSystemGuids)).ToValueSetApiModelPage(codeSystemGuids, MapToValueSetApiModel);
            }
            catch (Exception ex)
            {
                this.Logger.Error(ex, ex.Message);
                return this.CreateFailureResponse(
                    ex.Message,
                    HttpStatusCode.InternalServerError);
            }
        }

        private object AddValueSet()
        {
            try
            {
                var model = this.Bind<ValueSetCreationApiModel>();
                var attempt = this.clientTermValueSetService.Create(model);
                if (!attempt.Success || !attempt.Result.HasValue)
                {
                    throw attempt.Exception.HasValue
                              ? attempt.Exception.Single()
                              : new ArgumentException("Failed to add value set.");
                }

                var valueSet = attempt.Result.Single();
                var validationResult = this.valueSetValidator.Validate(valueSet);
                if (validationResult.IsValid)
                {
                    this.clientTermValueSetService.Save(valueSet);
                    return Mapper.Map<IValueSet, ValueSetApiModel>(valueSet);
                }

                throw new InvalidOperationException(
                    "ValueSet validation failed: "
                    + string.Join(Environment.NewLine, validationResult.Errors.Select(vr => vr.ErrorMessage)));
            }
            catch (Exception ex)
            {
                this.Logger.Error(ex, ex.Message);
                return this.CreateFailureResponse(
                    "Failed to create a value set",
                    HttpStatusCode.InternalServerError);
            }
        }

        private object DeleteValueSet(dynamic parameters)
        {
            return this.CreateFailureResponse("Not Implemented", HttpStatusCode.NotImplemented);
        }
    }
}