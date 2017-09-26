namespace Fabric.Terminology.API.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using CallMeMaybe;

    using Fabric.Terminology.API.Configuration;
    using Fabric.Terminology.API.Models;
    using Fabric.Terminology.API.Validators;
    using Fabric.Terminology.Domain.Exceptions;
    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.Domain.Services;

    using Nancy;

    using Serilog;

    public sealed class ValueSetModule : TerminologyModule<ValueSetApiModel>
    {
        private readonly IValueSetService valueSetService;

        private readonly IValueSetSummaryService valueSetSummaryService;

        private readonly ValueSetValidator valueSetValidator;

        public ValueSetModule(
                IValueSetService valueSetService,
                IValueSetSummaryService valueSetSummaryService,
                IAppConfiguration config,
                ILogger logger,
                ValueSetValidator valueSetValidator)
            : base($"/{TerminologyVersion.Route}/valuesets", config, logger)
        {
            this.valueSetService = valueSetService;
            this.valueSetSummaryService = valueSetSummaryService;
            this.valueSetValidator = valueSetValidator;

            this.Get("/", _ => this.GetValueSetPage(), null, "GetPaged");

            this.Get("/{valueSetGuid}", parameters => this.GetValueSets(parameters.valueSetGuid), null, "GetValueSet");

            //this.Post("/find/", _ => this.Find(), null, "Find");

            //this.Post("/", _ => this.AddValueSet(), null, "AddValueSet");

            //this.Delete("/{valueSetUniqueId}", parameters => this.DeleteValueSet(parameters), null, "DeleteValueSet");
        }

        private object GetValueSet(Guid valueSetGuid, IEnumerable<Guid> codeSystems, bool summary = true)
        {
            try
            {
                var model = summary ? (Maybe<object>)this.valueSetSummaryService
                                        .GetValueSetSummary(valueSetGuid, codeSystems)
                                        .Select(vs => vs.ToValueSetItemApiModel()) :

                                      this.valueSetService
                                        .GetValueSet(valueSetGuid, codeSystems)
                                        .Select(vs => vs.ToValueSetApiModel());

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
                var codeSystems = this.GetCodeSystems();
                var summary = this.GetSummarySetting();

                if (guids.Length == 1)
                {
                    return this.GetValueSet(guids[0], codeSystems, summary);
                }

                if (!valueSetGuids.Any())
                {
                    return this.CreateFailureResponse("An array of value set ids is required.", HttpStatusCode.BadRequest);
                }

                return summary ? this.valueSetSummaryService
                                            .GetValueSetSummaries(guids, codeSystems)
                                            .Select(vs => vs.ToValueSetItemApiModel()) :

                                          this.valueSetService.GetValueSets(guids, codeSystems)
                                            .Select(vs => vs.ToValueSetApiModel());
            }
            catch (Exception ex)
            {
                this.Logger.Error(ex, ex.Message);
                return this.CreateFailureResponse(
                    "Failed to retrieve value sets",
                    HttpStatusCode.InternalServerError);
            }
        }

        private async Task<object> GetValueSetPage()
        {
            try
            {
                var summary = this.GetSummarySetting();
                var pagerSettings = this.GetPagerSettings();
                var codeSystemCds = this.GetCodeSystems();

                return summary
                           ? (await this.valueSetSummaryService.GetValueSetSummariesAsync(pagerSettings, codeSystemCds))
                                .ToValueSetApiModelPage(MapToValueSetItemApiModel)

                           : (await this.valueSetService.GetValueSetsAsync(pagerSettings, codeSystemCds))
                                .ToValueSetApiModelPage(MapToValueSetApiModel);
            }
            catch (Exception ex)
            {
                this.Logger.Error(ex, ex.Message);
                return this.CreateFailureResponse(
                    "Failed to retrieve the page of value sets",
                    HttpStatusCode.InternalServerError);
            }

            ValueSetApiModel MapToValueSetApiModel(IValueSet vs) => vs.ToValueSetApiModel();

            ValueSetItemApiModel MapToValueSetItemApiModel(IValueSetSummary vss) => vss.ToValueSetItemApiModel();
        }

        //private async Task<object> Find()
        //{
        //    try
        //    {
        //        var model = this.EnsureQueryModel(this.Bind<FindByTermQuery>());

        //        throw new NotImplementedException();

        //        //var results = await this.valueSetService.GetValueSetsAsync(
        //        //                  model.Term,
        //        //                  model.PagerSettings,
        //        //                  model.CodeSystemCodes.ToArray(),
        //        //                  !model.Summary);

        //        //return results.ToValueSetApiModelPage(model.Summary, this.config.ValueSetSettings.ShortListCodeCount);
        //    }
        //    catch (Exception ex)
        //    {
        //        this.Logger.Error(ex, ex.Message);
        //        return this.CreateFailureResponse(
        //            "Failed to find the page of value sets",
        //            HttpStatusCode.InternalServerError);
        //    }
        //}

        //private object AddValueSet()
        //{
        //    try
        //    {
        //        var model = this.Bind<ValueSetCreationApiModel>();
        //        var attempt = this.valueSetService.Create(model);
        //        if (attempt.Success && attempt.Result.HasValue)
        //        {
        //            var valueSet = attempt.Result.Single();
        //            this.valueSetService.Save(valueSet);
        //            return valueSet.ValueSetItemApiModel(false);
        //        }

        //        throw attempt.Exception.HasValue ? attempt.Exception.Single() : new ArgumentException("Failed to add value set.");
        //    }
        //    catch (Exception ex)
        //    {
        //        this.Logger.Error(ex, ex.Message);
        //        return this.CreateFailureResponse(
        //            "Failed to create a value set",
        //            HttpStatusCode.InternalServerError);
        //    }
        //}

        //private object DeleteValueSet(dynamic parameters)
        //{
        //    return this.CreateFailureResponse("Not implemented", HttpStatusCode.NotImplemented);
        //}
    }
}