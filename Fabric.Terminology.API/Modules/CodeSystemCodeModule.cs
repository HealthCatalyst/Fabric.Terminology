namespace Fabric.Terminology.API.Modules
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using AutoMapper;

    using Fabric.Terminology.API.Configuration;
    using Fabric.Terminology.API.Models;
    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.Domain.Services;

    using Nancy;
    using Nancy.ModelBinding;
    using Nancy.Security;

    using Serilog;

    public sealed class CodeSystemCodeModule : TerminologyModule<CodeSystemCodeApiModel>
    {
        private readonly ICodeSystemCodeService codeSystemCodeService;

        public CodeSystemCodeModule(
            ICodeSystemCodeService codeSystemCodeService,
            IAppConfiguration config,
            ILogger logger)
            : base($"/{TerminologyVersion.Route}/codes", config, logger)
        {
            this.codeSystemCodeService = codeSystemCodeService;

            this.Get("/", async _ => await this.GetCodeSystemCodePageAysnc().ConfigureAwait(false), null, "GetPagedCodeSystemCodes");

            this.Get("/{codeGuid}", parameters => this.GetCodeSystemCode(parameters.codeGuid), null, "GetCodeSystemCode");

            this.Post("/batch/", async _ => await this.GetBatchAsync().ConfigureAwait(false), null, "GetBatchCodes");

            this.Post("/multiple/", _ => this.GetMultiple(), null, "GetCodeSystemCodes");

            this.Post("/search/", async _ => await this.SearchAsync().ConfigureAwait(false), null, "SearchCodeSystemCodes");
        }

        private static MultipleCodeSystemCodeQuery EnsureQueryModel(MultipleCodeSystemCodeQuery model)
        {
            if (model.CodeGuids == null)
            {
                model.CodeGuids = Array.Empty<Guid>();
            }

            return model;
        }

        private static BatchCodeQuery EnsureQueryModel(BatchCodeQuery model)
        {
            if (model.Codes == null)
            {
                model.Codes = Array.Empty<string>();
            }

            if (model.CodeSystemGuids == null)
            {
                model.CodeSystemGuids = Array.Empty<Guid>();
            }

            return model;
        }

        private object GetCodeSystemCode(Guid codeGuid)
        {
            this.RequiresClaims(this.TerminologyReadClaim);
            try
            {
                return this.codeSystemCodeService
                    .GetCodeSystemCode(codeGuid)
                    .Select(m => (object)Mapper.Map<CodeSystemCodeApiModel>(m))
                    .Else(() => this.CreateFailureResponse(
                            "Code system with codeSystemGuid was not found",
                            HttpStatusCode.NotFound));
            }
            catch (Exception ex)
            {
                this.Logger.Error(ex, ex.Message);
                return this.CreateFailureResponse(
                    $"Failed to get CodeSystemCode. {ex.Message}",
                    HttpStatusCode.InternalServerError);
            }
        }

        private async Task<object> GetCodeSystemCodePageAysnc()
        {
            this.RequiresClaims(this.TerminologyReadClaim);
            try
            {
                var pagerSettings = this.GetPagerSettings();
                var codeSystemGuids = this.GetCodeSystems();

                return (await this.codeSystemCodeService.GetCodeSystemCodesAsync(pagerSettings, codeSystemGuids).ConfigureAwait(false))
                    .ToCodeSystemCodeApiModelPage();
            }
            catch (Exception ex)
            {
                this.Logger.Error(ex, ex.Message);
                return this.CreateFailureResponse(
                    "Failed to retrieve the page of code system codes",
                    HttpStatusCode.InternalServerError);
            }
        }

        private object GetMultiple()
        {
            this.RequiresClaims(this.TerminologyReadClaim);
            try
            {
                var model = EnsureQueryModel(this.Bind<MultipleCodeSystemCodeQuery>(new BindingConfig { BodyOnly = true }));

                return this.codeSystemCodeService.GetCodeSystemCodes(model.CodeGuids)
                    .Select(Mapper.Map<CodeSystemCodeApiModel>);
            }
            catch (Exception ex)
            {
                this.Logger.Error(ex, ex.Message);
                return this.CreateFailureResponse(
                    ex.Message,
                    HttpStatusCode.InternalServerError);
            }
        }

        private async Task<object> GetBatchAsync()
        {
            this.RequiresClaims(this.TerminologyReadClaim);
            try
            {
                var model = EnsureQueryModel(this.Bind<BatchCodeQuery>(new BindingConfig { BodyOnly = true }));

                if (!model.Codes.Any())
                {
                    return this.CreateFailureResponse("Codes array cannot be empty.", HttpStatusCode.BadRequest);
                }

                var results =
                    await this.codeSystemCodeService.GetCodeSystemCodesBatchAsync(model.Codes, model.CodeSystemGuids).ConfigureAwait(false);

                return Mapper.Map<BatchCodeResultApiModel>(results);
            }
            catch (Exception ex)
            {
                this.Logger.Error(ex, ex.Message);
                return this.CreateFailureResponse(
                    ex.Message,
                    HttpStatusCode.InternalServerError);
            }
        }

        private async Task<object> SearchAsync()
        {
            this.RequiresClaims(this.TerminologyReadClaim);
            try
            {
                var model = this.EnsureQueryModel(this.Bind<FindByTermQuery>(new BindingConfig { BodyOnly = true }));

                return (await this.codeSystemCodeService.GetCodeSystemCodesAsync(
                            model.Term,
                            model.PagerSettings,
                            model.CodeSystemGuids)
                            .ConfigureAwait(false)).ToCodeSystemCodeApiModelPage();
            }
            catch (Exception ex)
            {
                this.Logger.Error(ex, ex.Message);
                return this.CreateFailureResponse(ex.Message, HttpStatusCode.InternalServerError);
            }
        }

        private FindByTermQuery EnsureQueryModel(FindByTermQuery model)
        {
            if (model.PagerSettings == null)
            {
                model.PagerSettings = new PagerSettings
                {
                    CurrentPage = 1,
                    ItemsPerPage = this.Config.TerminologySqlSettings.DefaultItemsPerPage
                };
            }

            if (model.CodeSystemGuids == null)
            {
                model.CodeSystemGuids = Array.Empty<Guid>();
            }

            if (model.Term == null)
            {
                model.Term = string.Empty;
            }

            return model;
        }
    }
}