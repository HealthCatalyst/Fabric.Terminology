namespace Fabric.Terminology.API.Validators
{
    using System;

    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.Domain.Services;

    using FluentValidation;

    public class ValueSetValidatorCollection : AbstractValidator<IValueSet>
    {
        private readonly IClientTermValueSetService service;

        public ValueSetValidatorCollection(IClientTermValueSetService clientTermValueSetService)
        {
            this.service = clientTermValueSetService;
            this.ConfigureRules();
        }

        private void ConfigureRules()
        {
            //// this.RuleFor(vs => vs.ValueSetId).NotEmpty().WithMessage("Please specify an Id for this client");

            this.RuleFor(vs => vs.Name).Must(this.BeUnique).When(vs => !string.IsNullOrEmpty(vs.Name));

            this.RuleFor(vs => vs.Name).NotEmpty().WithMessage("Please specify a Name for this client");

            // this.RuleFor(vs => vs.ValueSetGuid).NotEmpty().Must(this.BeUnique);
        }

        private bool BeUnique(string name)
        {
            return this.service.NameIsUnique(name);
        }

        private bool BeUnique(Guid valueSetGuid)
        {
            return this.service.ValueSetGuidIsUnique(valueSetGuid);
        }
    }
}