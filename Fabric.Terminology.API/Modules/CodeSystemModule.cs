namespace Fabric.Terminology.API.Modules
{
    using System;
    using System.Linq;

    using AutoMapper;

    using Fabric.Terminology.API.Configuration;
    using Fabric.Terminology.API.Models;
    using Fabric.Terminology.Domain.Services;

    using Nancy;

    using Serilog;

    public sealed class CodeSystemModule : TerminologyModule<CodeSystemApiModel>
    {
        private readonly ICodeSystemService codeSystemService;

        public CodeSystemModule(
            ICodeSystemService codeSystemService,
            IAppConfiguration config,
            ILogger logger)
            : base($"/{TerminologyVersion.Route}/codesystems", config, logger)
        {
            this.codeSystemService = codeSystemService;

            this.Get("/", _ => this.GetAll(), null, "GetAll");

            this.Get("/{codeSystemGuid}", parameters => this.GetCodeSystem(parameters.valueSetGuid), null, "GetValueSet");

            this.Post("/multiple/", _ => this.GetMultiple(), null, "GetMultiple");
        }

        private object GetAll()
        {
            try
            {
                return this.codeSystemService.GetAll().Select(Mapper.Map<CodeSystemApiModel>).ToList();
            }
            catch (Exception ex)
            {
                this.Logger.Error(ex, ex.Message);
                return this.CreateFailureResponse(
                    $"Failed to GetAll CodeSystems. {ex.Message}",
                    HttpStatusCode.InternalServerError);
            }
        }

        private object GetCodeSystem(string codeSystemUniqueId)
        {
            return null;
        }

        private object GetMultiple()
        {
            return null;
        }
    }
}
