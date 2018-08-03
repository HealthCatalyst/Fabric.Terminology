namespace Fabric.Terminology.API.Modules
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using AutoMapper;

    using Fabric.Terminology.API.Configuration;
    using Fabric.Terminology.API.Models;
    using Fabric.Terminology.API.Services;
    using Fabric.Terminology.Domain.Services;

    using Nancy;
    using Nancy.Security;

    using Serilog;

    public sealed class ValueSetCodeModule : TerminologyModule<ValueSetCodeApiModel>
    {
        private readonly IValueSetCodeService valueSetCodeService;

        public ValueSetCodeModule(
            IValueSetCodeService valueSetCodeService,
            IAppConfiguration config,
            ILogger logger,
            UserAccessService userAccessService)
            : base($"/{TerminologyVersion.Route}/valuesetcodes", config, logger, userAccessService)
        {
            this.valueSetCodeService = valueSetCodeService;

            this.Get("/", async _ => await this.GetAllValueSetCodePageAsync().ConfigureAwait(false), null, "GetAllValueSetCodesPaged");

            this.Get("/{codeGuid}", parameters => this.GetValueSetCodes(parameters.codeGuid), null, "GetValueSetCodes");

            this.Get("/valueset/{valueSetGuid}", async parameters => await this.GetValueSetCodePageAsync(parameters.valueSetGuid), null, "GetValueSetCodePagedByValueSet");
        }

        private object GetValueSetCodes(Guid codeGuid)
        {
            this.RequireAccessorAuthorization();
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

        private async Task<object> GetAllValueSetCodePageAsync()
        {
            this.RequireAccessorAuthorization();
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

        private async Task<object> GetValueSetCodePageAsync(Guid valueSetGuid)
        {
            this.RequireAccessorAuthorization();
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
