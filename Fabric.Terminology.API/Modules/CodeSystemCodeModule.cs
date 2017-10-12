namespace Fabric.Terminology.API.Modules
{
    using System;

    using AutoMapper;

    using CallMeMaybe;

    using Fabric.Terminology.API.Configuration;
    using Fabric.Terminology.API.Models;
    using Fabric.Terminology.Domain.Services;

    using Nancy;

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

            this.Get("/{codeGuid}", parameters => this.GetCodeSystemCode(parameters.codeGuid), null, "GetCodeSystemCode");
        }

        private object GetCodeSystemCode(Guid codeGuid)
        {
            try
            {
                return this.codeSystemCodeService
                    .GetCodeSystemCode(codeGuid)
                    .Select(m => (object)Mapper.Map<CodeSystemCodeApiModel>(m))
                    .Else(
                        this.CreateFailureResponse(
                            "Code sytem with codeSystemGuid was not found",
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
    }
}
