namespace Fabric.Terminology.API.Modules
{
    using System;

    using CallMeMaybe;

    using Fabric.Terminology.API.Configuration;
    using Fabric.Terminology.API.Models;
    using Fabric.Terminology.Domain.Exceptions;
    using Fabric.Terminology.Domain.Services;

    using Nancy;

    using Serilog;

    public class ValueSetSummaryModule : TerminologyModule<ValueSetSummaryApiModel>
    {
        private readonly IValueSetSummaryService valueSetSummaryService;


        public ValueSetSummaryModule(
            IValueSetSummaryService valueSetSummaryService,
            IAppConfiguration config,
            ILogger logger)
            : base($"/{TerminologyVersion.Route}/valuesetsummaries", config, logger)
        {
            this.valueSetSummaryService = valueSetSummaryService;

            this.Get(
                "/{valueSetGuid}",
                parameters => this.GetValueSetSummary(parameters.valueSetGuid),
                null,
                "GetValueSetSummary");
        }

        private object GetValueSetSummary(Guid valueSetGuid)
        {
            try
            {
                var codeSystems = this.GetCodeSystems();

                var maybe = (Maybe<object>)this.valueSetSummaryService.GetValueSetSummary(valueSetGuid, codeSystems)
                    .Select(vs => vs.ToValueSetSummaryApiModel());

                return maybe.Else(
                    () => this.CreateFailureResponse(
                        "ValueSet with matching ID was not found",
                        HttpStatusCode.NotFound));
            }
            catch (ValueSetNotFoundException ex)
            {
                this.Logger.Error(ex, ex.Message, valueSetGuid);
                return this.CreateFailureResponse(
                    $"The ValueSet with id: {valueSetGuid} was not found.",
                    HttpStatusCode.InternalServerError);
            }
        }
    }
}