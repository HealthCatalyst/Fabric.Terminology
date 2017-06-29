namespace Fabric.Terminology.API.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Fabric.Terminology.API.Configuration;
    using Fabric.Terminology.API.Models;
    using Fabric.Terminology.API.Validators;
    using Fabric.Terminology.Domain;
    using Fabric.Terminology.Domain.Exceptions;
    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.Domain.Services;

    using Nancy;
    using Nancy.ModelBinding;

    using Serilog;

    public sealed class ValueSetModule : TerminologyModule<IValueSet>
    {
        private readonly IValueSetService valueSetService;

        private readonly IAppConfiguration config;

        private readonly ValueSetValidator valueSetValidator;

        public ValueSetModule(IValueSetService valueSetService, IAppConfiguration config, ILogger logger, ValueSetValidator valueSetValidator)
            : base($"/{TerminologyVersion.Route}/valueset", logger)
        {
            this.valueSetService = valueSetService;
            this.config = config;
            this.valueSetValidator = valueSetValidator;

            this.Get("/{valueSetId}", parameters => this.GetValueSet(parameters, false), null, "GetValueSet");

            this.Get("/valuesets/", _ => this.GetValueSets(false), null, "GetValueSets");

            this.Get("/summary/{valueSetId}", parameters => this.GetValueSet(parameters, true), null, "GetSummary");

            this.Get("/summaries/", _ => this.GetValueSets(true), null, "GetSummaries");

            this.Get("/paged/", _ => this.GetValueSetPage(false), null, "GetPaged");

            this.Get("/paged/summaries/", _ => this.GetValueSetPage(true), null, "GetPagedSummaries");

            this.Post("/find/", _ => this.Find(false), null, "Find");

            this.Post("/find/summaries/", _ => this.Find(true), null, "FindSummaries");

            this.Post("/", _ => this.AddValueSet(), null, "AddValueSet");

            this.Delete("/{valueSetId}", parameters => this.DeleteValueSet(parameters), null, "DeleteValueSet");
        }

        private object GetValueSet(dynamic parameters, bool summary = true)
        {
            try
            {
                var valueSetId = parameters.valueSetId.ToString();
                var codeSystems = this.GetCodeSystems();

                IValueSet valueSet = this.valueSetService.GetValueSet(valueSetId, codeSystems);
                if (valueSet != null)
                {
                    return valueSet.ToValueSetApiModel(summary, this.config.ValueSetSettings.ShortListCodeCount);
                }

                throw new NullReferenceException();
            }
            catch (ValueSetNotFoundException ex)
            {
                this.Logger.Error(ex, ex.Message, parameters.valueSetId);
                return this.CreateFailureResponse(
                    $"The ValueSet with id: {parameters.valueSetId} was not found.",
                    HttpStatusCode.BadRequest);
            }
        }

        private object GetValueSets(bool summaries = true)
        {
            try
            {
                var valueSetIds = this.GetValueSetIds();

                if (!valueSetIds.Any())
                {
                    throw new ArgumentException("An array of value set ids is required.");
                }

                var codeSystemCds = this.GetCodeSystems();

                var valueSets = summaries
                                    ? this.valueSetService.GetValueSetSummaries(valueSetIds, codeSystemCds)
                                    : this.valueSetService.GetValueSets(valueSetIds, codeSystemCds);

                return valueSets.Select(vs => vs.ToValueSetApiModel(summaries, this.config.ValueSetSettings.ShortListCodeCount));
            }
            catch (Exception ex)
            {
                this.Logger.Error(ex, ex.Message);
                return this.CreateFailureResponse(
                    "Failed to retrieve the list of value sets",
                    HttpStatusCode.BadRequest);
            }
        }

        private async Task<object> GetValueSetPage(bool summary = true)
        {
            try
            {
                var pagerSettings = this.GetPagerSettings();
                var codeSystemCds = this.GetCodeSystems();

                var pc = summary ?
                    await this.valueSetService.GetValueSetSummariesAsync(pagerSettings, codeSystemCds) :
                    await this.valueSetService.GetValueSetsAsync(pagerSettings, codeSystemCds);

                return pc.ToValueSetApiModelPage(summary, this.config.ValueSetSettings.ShortListCodeCount);
            }
            catch (Exception ex)
            {
                this.Logger.Error(ex, ex.Message);
                return this.CreateFailureResponse(
                    "Failed to retrieve the page of value sets",
                    HttpStatusCode.BadRequest);
            }
        }

        private async Task<object> Find(bool summary = true)
        {
            try
            {
                var model = this.EnsureQueryModel(this.Bind<FindByTermQuery>());

                var results = await this.valueSetService.FindValueSetsAsync(
                                  model.Term,
                                  model.PagerSettings,
                                  !summary,
                                  model.CodeSystemCodes.ToArray());

                return results.ToValueSetApiModelPage(summary, this.config.ValueSetSettings.ShortListCodeCount);
            }
            catch (Exception ex)
            {
                this.Logger.Error(ex, ex.Message);
                return this.CreateFailureResponse(
                    "Failed to find the page of value sets",
                    HttpStatusCode.BadRequest);
            }
        }

        private object AddValueSet()
        {
            return this.CreateFailureResponse("Not implemented", HttpStatusCode.NotImplemented);
        }

        private object DeleteValueSet(dynamic parameters)
        {
            return this.CreateFailureResponse("Not implemented", HttpStatusCode.NotImplemented);
        }

        private IPagerSettings GetPagerSettings()
        {
            var cp = (int)this.Request.Query.@p;
            var count = (int)this.Request.Query.@count;
            return new PagerSettings
            {
                CurrentPage = cp == 0 ? 1 : cp,
                ItemsPerPage = count == 0 ? this.config.TerminologySqlSettings.DefaultItemsPerPage : count
            };
        }

        private FindByTermQuery EnsureQueryModel(FindByTermQuery model)
        {
            if (model.PagerSettings == null)
            {
                model.PagerSettings = new PagerSettings
                {
                    CurrentPage = 1,
                    ItemsPerPage = this.config.TerminologySqlSettings.DefaultItemsPerPage
                };
            }

            if (model.CodeSystemCodes == null)
            {
                model.CodeSystemCodes = Enumerable.Empty<string>();
            }

            if (model.Term == null)
            {
                model.Term = string.Empty;
            }

            return model;
        }

        private string[] CreateParameterArray(string value)
        {
            if (value.IsNullOrWhiteSpace())
            {
                return new string[] { };
            }

            var cds = value.Split(',');
            return cds.Select(cd => cd.Trim()).ToArray();
        }

        private string[] GetCodeSystems()
        {
            return this.CreateParameterArray((string)this.Request.Query.@codesystem);
        }

        private string[] GetValueSetIds()
        {
            return this.CreateParameterArray((string)this.Request.Query.@valuesetid);
        }
    }
}