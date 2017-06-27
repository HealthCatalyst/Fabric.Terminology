namespace Fabric.Terminology.API.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Fabric.Terminology.API.Configuration;
    using Fabric.Terminology.Domain;
    using Fabric.Terminology.Domain.Exceptions;
    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.Domain.Services;
    using Fabric.Terminology.Domain.Validators;

    using Nancy;

    using Serilog;

    public sealed class ValueSetsModule : TerminologyModule<IValueSet>
    {
        private readonly IValueSetService valueSetService;

        private readonly IAppConfiguration config;

        public ValueSetsModule(IValueSetService valueSetService, IAppConfiguration config, ILogger logger, ValueSetValidator valueSetValidator)
            : base("/api/valueset", logger, valueSetValidator)
        {
            this.valueSetService = valueSetService;
            this.config = config;

            this.Get("/{valueSetId}", parameters => this.GetValueSet(parameters, false));

            this.Get("/valuesets/", _ => this.GetValueSets(this.valueSetService.GetValueSets, false));

            this.Get("/summary/{valueSetId}", parameters => this.GetValueSet(parameters, true));

            this.Get("/summaries/", _ => this.GetValueSets(this.valueSetService.GetValueSets, true));

            this.Get("/paged/", _ => this.GetValueSetPage(false));

            this.Get("/paged/summaries/", _ => this.GetValueSetPage(true));

            this.Post("/find/", parameters => "something");

            this.Post("/find/summaries/", parameters => "somethings");
        }

        private dynamic GetValueSet(dynamic parameters, bool summary = true)
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

        private dynamic GetValueSets(Func<string[], string[], IEnumerable<IValueSet>> query, bool summaries = true)
        {
            try
            {
                var valueSetIds = this.GetValueSetIds();

                if (!valueSetIds.Any())
                {
                    throw new ArgumentException("An array of value set ids is required.");
                }

                var codeSystemCds = this.GetCodeSystems();

                var valueSets = query.Invoke(valueSetIds, codeSystemCds);

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

        private async Task<dynamic> GetValueSetPage(bool summary = true)
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