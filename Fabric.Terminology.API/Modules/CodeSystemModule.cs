namespace Fabric.Terminology.API.Modules
{
    using System;
    using System.Linq;

    using AutoMapper;

    using CallMeMaybe;

    using Fabric.Terminology.API.Configuration;
    using Fabric.Terminology.API.Models;
    using Fabric.Terminology.Domain.Services;

    using Nancy;
    using Nancy.ModelBinding;

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

            this.Get("/{codeSystemGuid}", parameters => this.GetCodeSystem(parameters.codeSystemGuid), null, "GetCodeSystem");

            this.Post("/multiple/", _ => this.GetMultiple(), null, "GetCodeSystems");
        }

        private static MultipleCodeSystemQuery EnsureQueryModel(MultipleCodeSystemQuery model)
        {
            if (model.CodeSystemGuids == null)
            {
                model.CodeSystemGuids = new Guid[] { };
            }

            return model;
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
            try
            {
                var model = (Maybe<object>)this.codeSystemService
                            .GetCodeSystem(Guid.Parse(codeSystemUniqueId))
                            .Select(Mapper.Map<CodeSystemApiModel>);

                return model.Else(() =>
                    this.CreateFailureResponse(
                        "Code sytem with codeSystemGuid was not found",
                        HttpStatusCode.NotFound));
            }
            catch (Exception ex)
            {
                this.Logger.Error(ex, ex.Message);
                return this.CreateFailureResponse(
                    $"Failed to GetAll CodeSystems. {ex.Message}",
                    HttpStatusCode.InternalServerError);
            }
        }

        private object GetMultiple()
        {
            try
            {
                var model = EnsureQueryModel(this.Bind<MultipleCodeSystemQuery>(new BindingConfig { BodyOnly = true }));

                return this.codeSystemService.GetAll(model.CodeSystemGuids)
                    .Select(Mapper.Map<CodeSystemApiModel>)
                    .ToList();
            }
            catch (Exception ex)
            {
                this.Logger.Error(ex, ex.Message);
                return this.CreateFailureResponse(
                    ex.Message,
                    HttpStatusCode.InternalServerError);
            }
        }
    }
}
