namespace Fabric.Terminology.API.Modules
{
    using System;
    using System.Linq;

    using AutoMapper;

    using Fabric.Terminology.API.Configuration;
    using Fabric.Terminology.API.Models;
    using Fabric.Terminology.API.Validators;
    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.Domain.Services;

    using Nancy;
    using Nancy.ModelBinding;

    using Serilog;

    public sealed class ClientTermValueSetModule : TerminologyModule<ValueSetApiModel>
    {
        public const string PostActionName = nameof(AddValueSet);

        public const string DeleteActionName = nameof(DeleteValueSet);

        private readonly IValueSetService valueSetService;

        private readonly ValueSetValidator valueSetValidator;

        public ClientTermValueSetModule(
            IValueSetService valueSetService,
            IAppConfiguration config,
            ILogger logger,
            ValueSetValidator valueSetValidator)
            : base($"/{TerminologyVersion.Route}/valuesets", config, logger)
        {
            this.valueSetService = valueSetService;
            this.valueSetValidator = valueSetValidator;
            this.Post("/", _ => this.AddValueSet(), null, PostActionName);

            this.Delete("/{valueSetGuid}", parameters => this.DeleteValueSet(parameters), null, DeleteActionName);
        }

        private object AddValueSet()
        {
            try
            {
                var model = this.Bind<ValueSetCreationApiModel>();
                var attempt = this.valueSetService.Create(model);
                if (!attempt.Success || !attempt.Result.HasValue)
                {
                    throw attempt.Exception.HasValue
                              ? attempt.Exception.Single()
                              : new ArgumentException("Failed to add value set.");
                }

                var valueSet = attempt.Result.Single();
                var validationResult = this.valueSetValidator.Validate(valueSet);
                if (validationResult.IsValid)
                {
                    this.valueSetService.Save(valueSet);
                    return Mapper.Map<IValueSet, ValueSetApiModel>(valueSet);
                }

                throw new InvalidOperationException(
                    "ValueSet validation failed: "
                    + string.Join(Environment.NewLine, validationResult.Errors.Select(vr => vr.ErrorMessage)));
            }
            catch (Exception ex)
            {
                this.Logger.Error(ex, ex.Message);
                return this.CreateFailureResponse(
                    "Failed to create a value set",
                    HttpStatusCode.InternalServerError);
            }
        }

        private object DeleteValueSet(dynamic parameters)
        {
            return this.CreateFailureResponse("Not implemented", HttpStatusCode.NotImplemented);
        }
    }
}