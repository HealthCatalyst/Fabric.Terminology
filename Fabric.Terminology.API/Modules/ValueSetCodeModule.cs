namespace Fabric.Terminology.API.Modules
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using AutoMapper;

    using Fabric.Terminology.API.Configuration;
    using Fabric.Terminology.API.Models;
    using Fabric.Terminology.Domain.Services;

    using Nancy;

    using Serilog;

    public sealed class ValueSetCodeModule : TerminologyModule<ValueSetCodeApiModel>
    {
        private readonly IValueSetCodeService valueSetCodeService;

        public ValueSetCodeModule(
            IValueSetCodeService valueSetCodeService,
            IAppConfiguration config,
            ILogger logger)
            : base($"/{TerminologyVersion.Route}/valuesetcodes", config, logger)
        {
            this.valueSetCodeService = valueSetCodeService;

            this.Get("/", async _ => await this.GetAllValueSetCodePage().ConfigureAwait(false), null, "GetAllValueSetCodesPaged");

            this.Get("/{codeGuid}", parameters => this.GetValueSetCodes(parameters.codeGuid), null, "GetValueSetCodes");

            this.Get("/valueset/{valueSetGuid}", async parameters => await this.GetValueSetCodePage(parameters.valueSetGuid), null, "GetValueSetCodePagedByValueSet");
        }

        private object GetValueSetCodes(Guid codeGuid)
        {
            try
            {
                return this.valueSetCodeService.GetValueSetCodesByCodeGuid(codeGuid)
                    .Select(Mapper.Map<ValueSetCodeApiModel>);
            }
            catch (Exception ex)
            {
                this.Logger.Error(ex, ex.Message);
                return this.CreateFailureResponse(
                    "Failed to retrieve value set codes for Guid " + codeGuid.ToString(),
                    HttpStatusCode.InternalServerError);
            }
        }

        private async Task<object> GetAllValueSetCodePage()
        {
            try
            {
                var pagerSettings = this.GetPagerSettings();
                var codeSystemGuids = this.GetCodeSystems();
                return (await this.valueSetCodeService.GetValueSetCodesAsync(pagerSettings, codeSystemGuids).ConfigureAwait(false)).ToValueSetCodeApiModelPage();
            }
            catch (Exception ex)
            {
                this.Logger.Error(ex, ex.Message);
                return this.CreateFailureResponse(
                    "Failed to retrieve the page of value set codes",
                    HttpStatusCode.InternalServerError);
            }
        }

        private async Task<object> GetValueSetCodePage(Guid valueSetGuid)
        {
            try
            {
                var pagerSettings = this.GetPagerSettings();
                var codeSystemGuids = this.GetCodeSystems();

                return (await this.valueSetCodeService.GetValueSetCodesAsync(valueSetGuid, pagerSettings, codeSystemGuids).ConfigureAwait(false)).ToValueSetCodeApiModelPage();
            }
            catch (Exception ex)
            {
                this.Logger.Error(ex, ex.Message);
                return this.CreateFailureResponse(
                    "Failed to retrieve the page of value set codes",
                    HttpStatusCode.InternalServerError);
            }
        }
    }
}
